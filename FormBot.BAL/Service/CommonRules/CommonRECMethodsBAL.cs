using FormBot.BAL.Service.CommonRules;
using FormBot.Entity.Email;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Helper.Helper;
using FormBot.Helper;

namespace FormBot.BAL.Service
{
    public class CommonRECMethodsBAL : ICommonRECMethodsBAL
    {
        CreateJobBAL objCreateJobBAL = null;
        STCInvoiceBAL _stcInvoiceServiceBAL = null;
        GenerateStcReportBAL _generateStcReportBAL = null;
        CommonBAL _commonBAL = null;
        private readonly IJobRulesBAL _jobRules;

        public CommonRECMethodsBAL(IJobRulesBAL jobRules)
        {
            objCreateJobBAL = new CreateJobBAL();
            _stcInvoiceServiceBAL = new STCInvoiceBAL();
            _generateStcReportBAL = new GenerateStcReportBAL(_stcInvoiceServiceBAL);
            _commonBAL = new CommonBAL();
            _jobRules = jobRules;
        }
        public void ProcessRecData(DataSet ds, DataTable dtReason, bool IsWindowService = false)
        {
            try
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    _jobRules.CreateSTCInvoicePDFForRECData(ds.Tables[0], IsWindowService);
                }

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                {
                    foreach (string id in Convert.ToString(ds.Tables[1].Rows[0]["STCJobDetailsId"]).Split(','))
                    {
                        try
                        {
                            SetCacheOnSTCStatusChangeAfterRECInsertion(14, Convert.ToInt32(id));
                        }
                        catch (Exception ex)
                        {
                            Common.Log(ex.Message);
                        }
                    }
                    _jobRules.SendMailOnCERFailed(Convert.ToString(ds.Tables[1].Rows[0][0]));
                }

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                {
                    try
                    {
                        //string stcJobDetailIds = string.Empty;
                        List<int> peakpaySTCJobIds = new List<int>();
                        peakpaySTCJobIds= ds.Tables[2].AsEnumerable().Where(r => r.Field<int>("STCSettlementTerm") == 12 || (r.Field<int>("STCSettlementTerm") == 10 && r.Field<int>("CustomSettlementTerm") == 12)).Select(dr => dr.Field<int>("STCJobDetailsId")).Distinct().ToList();
                        List<int> stcJobDetailIds = ds.Tables[2].AsEnumerable().Select(dr => dr.Field<int>("STCJobDetailsId")).Distinct().ToList();
                        for (int i = 0; i < stcJobDetailIds.Count; i++)
                        {
                            //stcJobDetailIds = stcJobDetailIds + Convert.ToString(ds.Tables[2].Rows[i]["STCJobDetailsId"]);
                            SetCacheOnSTCStatusChangeAfterRECInsertion(22, Convert.ToInt32(stcJobDetailIds[i]));
                        }
                        if(peakpaySTCJobIds.Count>0)
                            CommonBAL.SetCacheDataForPeakPayFromJobId("", string.Join(",", peakpaySTCJobIds)).Wait();
                    }
                    catch (Exception ex)
                    {
                        Common.Log(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                //Common.Log(ex.Message.ToString());
                Helper.Log.WriteError(ex, "");
            }
        }

        public void SetCacheOnSTCStatusChangeAfterRECInsertion(int STCJobStageID, int STCJobDetailsId)
        {
            SortedList<string, string> data = new SortedList<string, string>();
            string stcStatus = Common.GetDescription((SystemEnums.STCJobStatus)STCJobStageID, "");
            string colorCode = Common.GetSubDescription((SystemEnums.STCJobStatus)STCJobStageID, "");
            data.Add("STCStatus", stcStatus);
            data.Add("ColorCode", colorCode);
            data.Add("STCStatusId", STCJobStageID.ToString());
            CommonBAL.SetCacheDataForSTCSubmission(STCJobDetailsId, null, data).Wait();
            Helper.Log.WriteLog(DateTime.Now.ToString() + " setcachedataForStcId on " + stcStatus + ": " + STCJobDetailsId);
        }

        //private void Log(string text)
        //{
        //    string path = AppDomain.CurrentDomain.BaseDirectory + "ServiceLog.txt";
        //    using (StreamWriter writer = new StreamWriter(path, true))
        //    {
        //        writer.WriteLine(DateTime.Now.ToString() + Environment.NewLine + text + Environment.NewLine + "------------------------------------------------------------------------");
        //        writer.Close();
        //    }
        //}

        //public void CreateSTCInvoicePDFForRECData(DataTable dt)
        //{
        //    try
        //    {
        //        DataTable dtUpdate = new DataTable();
        //        dtUpdate.Columns.Add("STCInvoiceNumber", typeof(string));
        //        dtUpdate.Columns.Add("STCInvoiceFilePath", typeof(string));

        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            try
        //            {
        //                //string filepath = CreateStcReport("CreateStcReport", "Pdf", Convert.ToInt32(dr["STCJobDetailsID"]), Convert.ToString(dr["STCInvoiceNumber"]), Convert.ToString(dr["SolarCompanyId"]), Convert.ToInt32(dr["ResellerUserId"]), Convert.ToInt32(dr["ResellerID"]));
        //                string filepath = _generateStcReportBAL.CreateStcReportNew("CreateStcReport", "Pdf", Convert.ToInt32(dr["STCJobDetailsID"]), Convert.ToString(dr["STCInvoiceNumber"]), Convert.ToString(dr["SolarCompanyId"]), "2", Convert.ToInt32(dr["ResellerUserId"]), Convert.ToInt32(dr["ResellerID"]), true);
        //                DataRow drUpdate = dtUpdate.NewRow();
        //                drUpdate["STCInvoiceNumber"] = Convert.ToString(dr["STCInvoiceNumber"]);
        //                drUpdate["STCInvoiceFilePath"] = filepath;
        //                dtUpdate.Rows.Add(drUpdate);
        //                Helper.Log.WriteLog("STCInvoice file is generate successfully for " + Convert.ToString(dr["STCInvoiceNumber"]));
        //            }
        //            catch (Exception ex)
        //            {
        //                Helper.Log.WriteError(ex,"An error has occured while generating STCInvioce file for " + Convert.ToString(dr["STCInvoiceNumber"]) + ": " + ex.Message);
        //            }

        //        }

        //        Helper.Log.WriteLog("start update Invoice file path.");
        //        var values =  _stcInvoiceServiceBAL.UpdateRecGeneratedInvoiceFilePath(dtUpdate);
        //        //foreach(var id in values)
        //        //{
        //        //    CommonBAL.SetCacheDataForSTCInvoice(id, 0);
        //        //}
        //        Helper.Log.WriteLog("Invoice file paths are updated successfully in STCInvoice.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Helper.Log.WriteError(ex,"An error has occured while generating STCInvoice files: " + ex.Message);
        //    }

        //}

        //private void InsertRECFailureJobReason(DataTable dtReason)
        //{
        //    try
        //    {
        //        objCreateJobBAL.InsertRECFailureJobReason(dtReason);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log("An error has occured while Insert Job Failure Reason: " + ex.Message);
        //    }
        //}

        //public void SendMailOnCERFailed(string stcjobids)
        //{
        //    foreach (string id in stcjobids.Split(','))
        //    {
        //        int STCJobDetailsId = Convert.ToInt32(id);
        //        DataSet ds = objCreateJobBAL.GetDetailsOfCERFailedJobForMail(STCJobDetailsId);
        //        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
        //        {
        //            DataRow dr = ds.Tables[0].Rows[0];
        //            EmailInfo emailInfo = new EmailInfo();
        //            emailInfo.TemplateID = 33;
        //            emailInfo.ReferenceNumber = Convert.ToString(dr["ReferenceNumber"]);
        //            emailInfo.OwnerName = Convert.ToString(dr["OwnerName"]);
        //            emailInfo.InstallationAddress = Convert.ToString(dr["InstallationAddress"]);
        //            emailInfo.SystemSize = Convert.ToDecimal(dr["SystemSize"]);
        //            emailInfo.STCsValue = Convert.ToString(dr["STCsValue"]);
        //            emailInfo.TotalValue = Convert.ToDecimal(dr["TotalValue"]);
        //            emailInfo.ResellerFullName = Convert.ToString(dr["ResellerFullName"]);
        //            emailInfo.FailureNotice = Convert.ToString(dr["FailureNotice"]);
        //            emailInfo.Date = DateTime.Now;
        //            emailBAL.ComposeAndSendEmail(emailInfo, Convert.ToString(dr["EmailAddresses"]));
        //        }
        //    }
        //}

    }
}

