using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity;
using System.Data;
using FormBot.Helper.Helper;
using System.IO;
using System.Net;
using FormBot.Helper;
using Newtonsoft.Json;
using System.Web.Mvc;
using FormBot.Entity.Email;

namespace FormBot.BAL.Service.CommonRules
{
    public class JobRulesBAL : IJobRulesBAL
    {
        private readonly ICreateJobBAL _createJob;
        private readonly IEmailBAL _emailBAL;
        private readonly IGenerateStcReportBAL _generateStcReportBAL;
        private readonly ISTCInvoiceBAL _stcInvoiceBal;

        public JobRulesBAL(ICreateJobBAL createJob, IEmailBAL emailBAL, IGenerateStcReportBAL generateStcReportBAL, ISTCInvoiceBAL stcInvoiceBal)
        {
            this._createJob = createJob;
            this._emailBAL = emailBAL;
            this._generateStcReportBAL = generateStcReportBAL;
            this._stcInvoiceBal = stcInvoiceBal;
        }

        public List<string> RemoveRequiredFields(CreateJob createJob)
        {
            List<string> lstField = new List<string>();
            lstField.Add("BasicDetails.Notes");
            lstField.Add("panelXml");
            lstField.Add("inverterXml");
            lstField.Add("JobElectricians.StreetNumber");
            lstField.Add("JobElectricians.StreetName");
            lstField.Add("JobElectricians.StreetTypeID");
            lstField.Add("JobElectricians.PostalAddressID");
            lstField.Add("JobElectricians.PostalDeliveryNumber");
            lstField.Add("JobElectricians.Town");
            lstField.Add("JobElectricians.State");
            lstField.Add("JobElectricians.PostCode");
            lstField.Add("JobElectricians.Phone");
            lstField.Add("JobElectricians.LicenseNumber");
            lstField.Add("JobInstallerDetails.FirstName");
            lstField.Add("JobInstallerDetails.Surname");
            lstField.Add("JobInstallerDetails.Phone");
            lstField.Add("JobInstallerDetails.UnitTypeID");
            lstField.Add("JobInstallerDetails.UnitNumber");
            lstField.Add("JobInstallerDetails.StreetNumber");
            lstField.Add("JobInstallerDetails.StreetName");
            lstField.Add("JobInstallerDetails.StreetTypeID");
            lstField.Add("JobInstallerDetails.PostalAddressID");
            lstField.Add("JobInstallerDetails.PostalDeliveryNumber");
            lstField.Add("JobInstallerDetails.Town");
            lstField.Add("JobInstallerDetails.State");
            lstField.Add("JobInstallerDetails.PostCode");
            lstField.Add("JobOwnerDetails.OwnerType");
            lstField.Add("JobOwnerDetails.CompanyName");
            lstField.Add("JobOwnerDetails.CompanyABN");
            lstField.Add("JobOwnerDetails.FirstName");
            lstField.Add("JobOwnerDetails.LastName");
            lstField.Add("JobInstallerDetails.ElectricianID");
            lstField.Add("JobInstallerDetails.LicenseNumber");
            lstField.Add("JobInstallerDetails.SWHInstallerDesignerId");

            lstField.Add("InstallerDesignerView.FirstName");
            lstField.Add("InstallerDesignerView.LastName");
            lstField.Add("InstallerDesignerView.FindInstallerDesignerFirstName");
            lstField.Add("InstallerDesignerView.FindInstallerDesignerLastName");
            lstField.Add("InstallerDesignerView.Phone");
            lstField.Add("InstallerDesignerView.UnitTypeID");
            lstField.Add("InstallerDesignerView.UnitNumber");
            lstField.Add("InstallerDesignerView.StreetNumber");
            lstField.Add("InstallerDesignerView.StreetName");
            lstField.Add("InstallerDesignerView.StreetTypeID");
            lstField.Add("InstallerDesignerView.PostalAddressID");
            lstField.Add("InstallerDesignerView.PostalDeliveryNumber");
            lstField.Add("InstallerDesignerView.Town");
            lstField.Add("InstallerDesignerView.State");
            lstField.Add("InstallerDesignerView.PostCode");
            lstField.Add("InstallerDesignerView.CECAccreditationNumber");
            lstField.Add("InstallerDesignerView.CECDesignerNumber");
            lstField.Add("InstallerDesignerView.SEDesignRoleId");
            lstField.Add("InstallerDesignerView.ElectricalContractorsLicenseNumber");
            lstField.Add("InstallerDesignerView.SWHLicenseNumber");

            lstField.Add("InstallerView.FirstName");
            lstField.Add("InstallerView.LastName");
            lstField.Add("InstallerView.FindInstallerDesignerFirstName");
            lstField.Add("InstallerView.FindInstallerDesignerLastName");
            lstField.Add("InstallerView.Phone");
            lstField.Add("InstallerView.UnitTypeID");
            lstField.Add("InstallerView.UnitNumber");
            lstField.Add("InstallerView.StreetNumber");
            lstField.Add("InstallerView.StreetName");
            lstField.Add("InstallerView.StreetTypeID");
            lstField.Add("InstallerView.PostalAddressID");
            lstField.Add("InstallerView.PostalDeliveryNumber");
            lstField.Add("InstallerView.Town");
            lstField.Add("InstallerView.State");
            lstField.Add("InstallerView.PostCode");
            lstField.Add("InstallerView.CECAccreditationNumber");
            lstField.Add("InstallerView.CECDesignerNumber");
            lstField.Add("InstallerView.SEDesignRoleId");
            lstField.Add("InstallerView.ElectricalContractorsLicenseNumber");
            lstField.Add("InstallerView.SWHLicenseNumber");

            lstField.Add("DesignerView.FirstName");
            lstField.Add("DesignerView.LastName");
            lstField.Add("DesignerView.FindInstallerDesignerFirstName");
            lstField.Add("DesignerView.FindInstallerDesignerLastName");
            lstField.Add("DesignerView.Phone");
            lstField.Add("DesignerView.UnitTypeID");
            lstField.Add("DesignerView.UnitNumber");
            lstField.Add("DesignerView.StreetNumber");
            lstField.Add("DesignerView.StreetName");
            lstField.Add("DesignerView.StreetTypeID");
            lstField.Add("DesignerView.PostalAddressID");
            lstField.Add("DesignerView.PostalDeliveryNumber");
            lstField.Add("DesignerView.Town");
            lstField.Add("DesignerView.State");
            lstField.Add("DesignerView.PostCode");
            lstField.Add("DesignerView.CECAccreditationNumber");
            lstField.Add("DesignerView.CECDesignerNumber");
            lstField.Add("DesignerView.SEDesignRoleId");
            lstField.Add("DesignerView.ElectricalContractorsLicenseNumber");
            lstField.Add("DesignerView.SWHLicenseNumber");

            if (createJob.JobInstallationDetails.AddressID == 2)
            {
                lstField.Add("JobInstallationDetails.StreetNumber");
                lstField.Add("JobInstallationDetails.StreetName");
                lstField.Add("JobInstallationDetails.StreetTypeID");
            }

            if (createJob.JobInstallationDetails.AddressID == 1)
            {
                lstField.Add("JobInstallationDetails.PostalAddressID");
                lstField.Add("JobInstallationDetails.PostalDeliveryNumber");
                if (createJob.JobInstallationDetails.UnitNumber != null && createJob.JobInstallationDetails.UnitTypeID != 0)
                {
                    lstField.Add("JobInstallationDetails.StreetNumber");
                }
            }

            if (createJob.JobOwnerDetails.AddressID == 2)
            {
                lstField.Add("JobOwnerDetails.StreetNumber");
                lstField.Add("JobOwnerDetails.StreetName");
                lstField.Add("JobOwnerDetails.StreetTypeID");
            }
            if (createJob.JobOwnerDetails.AddressID == 1)
            {
                lstField.Add("JobOwnerDetails.PostalAddressID");
                lstField.Add("JobOwnerDetails.PostalDeliveryNumber");
                if (createJob.JobOwnerDetails.UnitNumber != null && createJob.JobOwnerDetails.UnitTypeID != 0)
                {
                    lstField.Add("JobOwnerDetails.StreetNumber");
                }
            }
            if (createJob.BasicDetails.JobType == 2)
            {
                lstField.Add("JobInstallationDetails.NMI");
            }

            return lstField;
        }

        public KeyValuePair<bool,Int32> InsertCreateJobData(ref CreateJob createJob, string panelXml, string inverterXml, int solarCompanyId, int userId, string calculatSTCUrl = "")
        {
            KeyValuePair<bool, decimal?> keyValue = new KeyValuePair<bool, decimal?>();
            if (createJob.JobSystemDetails != null && createJob.JobSTCDetails != null)
            {
                string STCUrl = string.Empty;
                if (createJob.IsVendorApi == true)
                    STCUrl = calculatSTCUrl;
                else
                {
                    if (createJob.BasicDetails.JobType == 1)
                        STCUrl = ProjectSession.CalculateSTCUrl;
                    else
                        STCUrl = ProjectSession.CalculateSWHSTCUrl;
                }

                //createJob.JobSystemDetails.CalculatedSTC = GetSTCValue(createJob.BasicDetails.JobType, createJob.BasicDetails.strInstallationDate, createJob.JobSTCDetails.DeemingPeriod, createJob.JobInstallationDetails.PostCode, createJob.JobSystemDetails.SystemSize, createJob.JobSystemDetails.SystemBrand, createJob.JobSystemDetails.SystemModel, STCUrl);
                keyValue = GetSTCValue(createJob.BasicDetails.JobType, createJob.BasicDetails.strInstallationDate, createJob.JobSTCDetails.DeemingPeriod, createJob.JobInstallationDetails.PostCode, createJob.JobSystemDetails.SystemSize, createJob.JobSystemDetails.SystemBrand, createJob.JobSystemDetails.SystemModel, STCUrl);
                createJob.JobSystemDetails.ModifiedCalculatedSTC = keyValue.Value;
                #region commented code
                //string stcValue = string.Empty;

                //string installationDate = createJob.BasicDetails.strInstallationDate != null ? Convert.ToDateTime(createJob.BasicDetails.strInstallationDate).ToString("yyyy-MM-dd") : string.Empty;
                //if (createJob.BasicDetails.JobType == 1 && createJob.JobSystemDetails.SystemSize > 0)
                //    stcValue = CalculateSTC("SolarDeemed", installationDate, createJob.JobSTCDetails.DeemingPeriod, createJob.JobInstallationDetails.PostCode, Convert.ToString(createJob.JobSystemDetails.SystemSize));

                //if (createJob.BasicDetails.JobType == 2 && createJob.BasicDetails.strInstallationDate != null)
                //    stcValue = CalculateSWHSTC(installationDate, createJob.JobInstallationDetails.PostCode, createJob.JobSystemDetails.SystemBrand, createJob.JobSystemDetails.SystemModel);

                //if(!string.IsNullOrEmpty(stcValue))
                //{
                //    dynamic STCJsonData = JsonConvert.DeserializeObject(stcValue);
                //    if (STCJsonData.result != null)
                //    {
                //        dynamic noofSTC = JsonConvert.DeserializeObject(Convert.ToString(STCJsonData.result));
                //        if (noofSTC != null)
                //        {
                //            if (createJob.BasicDetails.JobType == 1)
                //                createJob.JobSystemDetails.CalculatedSTC = noofSTC.numberOfStcs;
                //            else
                //                createJob.JobSystemDetails.CalculatedSTC = noofSTC.numStc;
                //        }
                //        else
                //            createJob.JobSystemDetails.CalculatedSTC = null;
                //    }
                //    else
                //        createJob.JobSystemDetails.CalculatedSTC = null;
                //}
                //else
                //    createJob.JobSystemDetails.CalculatedSTC = null;
                #endregion commented code
            }

            if (createJob.BasicDetails.JobType == 2)
            {
                //if (createJob.JobSystemDetails != null)
                //{
                //    createJob.JobSystemDetails.CalculatedSTC = createJob.JobSystemDetails.CalculatedSTCForSWH;
                //}

                if (!(createJob.UserType == 1 || createJob.UserType == 3|| createJob.UserType==2||createJob.UserType==4))
                {
                    if (createJob.JobSTCDetails != null)
                    {
                        createJob.JobSTCDetails.CertificateCreated = "No";
                        createJob.JobSTCDetails.FailedAccreditationCode = "";
                    }
                }
            }

            if (createJob.BasicDetails.strInstallationDate != null)
            {
                createJob.BasicDetails.InstallationDate = Convert.ToDateTime(createJob.BasicDetails.strInstallationDate);
            }
            else
            {
                createJob.BasicDetails.InstallationDate = null;
            }

            /* IsPostalAddress */
            if (createJob.BasicDetails.strSoldByDate != null)
            {
                createJob.BasicDetails.SoldByDate = Convert.ToDateTime(createJob.BasicDetails.strSoldByDate);
            }
            else if(createJob.BasicDetails.SoldByDate!=null)
            {
                createJob.BasicDetails.SoldByDate = Convert.ToDateTime(createJob.BasicDetails.SoldByDate); 
            }
            else
            {
                createJob.BasicDetails.SoldByDate = null;
            }

            if (createJob.JobElectricians != null)
            {
                if (createJob.JobElectricians.AddressID == 2)
                {
                    createJob.JobElectricians.IsPostalAddress = true;
                }
                else
                {
                    createJob.JobElectricians.IsPostalAddress = false;
                }
            }

            if (createJob.JobOwnerDetails.AddressID == 2)
            {
                createJob.JobOwnerDetails.IsPostalAddress = true;
            }
            else
            {
                createJob.JobOwnerDetails.IsPostalAddress = false;
            }

            if (createJob.JobInstallationDetails.AddressID == 2)
            {
                createJob.JobInstallationDetails.IsPostalAddress = true;
            }
            else
            {
                createJob.JobInstallationDetails.IsPostalAddress = false;
            }

            if (createJob.JobInstallerDetails != null)
            {
                if (createJob.JobInstallerDetails.AddressID == 2)
                {
                    createJob.JobInstallerDetails.IsPostalAddress = true;
                }
                else
                {
                    createJob.JobInstallerDetails.IsPostalAddress = false;
                }
            }



            Int32 jobID = 0;
            DataTable dtCustomField = Common.GetCustomFieldDetail();
            if (createJob.lstCustomDetails != null)
            {
                for (int i = 0; i < createJob.lstCustomDetails.Count; i++)
                {
                    if (createJob.lstCustomDetails[i].FieldValue != null && createJob.lstCustomDetails[i].SeparatorId > 0)
                    {
                        if (createJob.lstCustomDetails[i].SeparatorId != Convert.ToInt32(SystemEnums.SerialNumberSeparatorId.NewLine))
                        {
                            createJob.lstCustomDetails[i].FieldValue = createJob.lstCustomDetails[i].FieldValue.Replace("\n", Convert.ToString((char)(13)) + Convert.ToString((char)(10)));
                        }
                        if (createJob.lstCustomDetails[i].SeparatorId == Convert.ToInt32(SystemEnums.SerialNumberSeparatorId.Comma))
                        {
                            createJob.lstCustomDetails[i].FieldValue = createJob.lstCustomDetails[i].FieldValue.Replace(",", Convert.ToString((char)(13)) + Convert.ToString((char)(10)));
                        }
                        if (createJob.lstCustomDetails[i].SeparatorId == Convert.ToInt32(SystemEnums.SerialNumberSeparatorId.NewLine))
                        {
                            createJob.lstCustomDetails[i].FieldValue = createJob.lstCustomDetails[i].FieldValue.Replace(Environment.NewLine, Convert.ToString((char)(13)) + Convert.ToString((char)(10)));
                        }
                        if (createJob.lstCustomDetails[i].SeparatorId == Convert.ToInt32(SystemEnums.SerialNumberSeparatorId.Colon))
                        {
                            createJob.lstCustomDetails[i].FieldValue = createJob.lstCustomDetails[i].FieldValue.Replace(":", Convert.ToString((char)(13)) + Convert.ToString((char)(10)));
                        }
                        if (createJob.lstCustomDetails[i].SeparatorId == Convert.ToInt32(SystemEnums.SerialNumberSeparatorId.SemiColon))
                        {
                            createJob.lstCustomDetails[i].FieldValue = createJob.lstCustomDetails[i].FieldValue.Replace(";", Convert.ToString((char)(13)) + Convert.ToString((char)(10)));
                        }
                    }
                    else
                    {
                        if (createJob.lstCustomDetails[i].FieldValue != null)
                        {
                            createJob.lstCustomDetails[i].FieldValue = createJob.lstCustomDetails[i].FieldValue.Replace("\n", " ");
                        }
                    }
                    dtCustomField.Rows.Add(new object[] { createJob.BasicDetails.JobID, createJob.lstCustomDetails[i].JobCustomFieldId, createJob.lstCustomDetails[i].FieldValue, userId, DateTime.Now, userId, DateTime.Now, 0, createJob.lstCustomDetails[i].VendorJobCustomFieldId, createJob.lstCustomDetails[i].FieldName });
                }
                jobID = _createJob.InsertJob(createJob, panelXml, inverterXml, solarCompanyId, userId, dtCustomField);
            }
            else
            {
                jobID = _createJob.InsertJob(createJob, panelXml, inverterXml, solarCompanyId, userId);
            }
            ////for update job details data in stc submission screen (cache update)
            //if (jobID > 0)
            //{
            //   SortedList<string, string> data = new SortedList<string, string>();
            //    DataTable dt = _createJob.GetSTCDetailsAndJobDataForCache(null, jobID.ToString());

            //    if (dt.Rows.Count > 0)
            //    {
            //        string isSPVRequired = dt.Rows[0]["IsSPVRequired"].ToString();
            //        string OwnerCompany = createJob.JobOwnerDetails.CompanyName == null ? "" : createJob.JobOwnerDetails.CompanyName;
            //        string RefNumberOwnerName = createJob.BasicDetails.RefNumber + ((createJob.JobOwnerDetails.CompanyName) == null ? "" : (" - " + createJob.JobOwnerDetails.CompanyName)) + " - " + createJob.JobOwnerDetails.FirstName + " " + createJob.JobOwnerDetails.LastName;
            //        string installationAddress = _createJob.GetInstallationAddressForCache(jobID).Rows[0]["InstallationAddress"].ToString();
            //        data.Add("InstallationTown", createJob.JobInstallationDetails.Town);

            //        data.Add("InstallationState", createJob.JobInstallationDetails.State);
            //        data.Add("JobID", jobID.ToString());
            //        data.Add("JobTypeId", createJob.BasicDetails.JobType.ToString());
            //        data.Add("IsSPVRequired", isSPVRequired);
            //        data.Add("OwnerName",createJob.JobOwnerDetails.FirstName+" "+createJob.JobOwnerDetails.LastName);
            //        data.Add("OwnerCompany", createJob.JobOwnerDetails.CompanyName==null?"": createJob.JobOwnerDetails.CompanyName);
            //        data.Add("InstallationDate",createJob.BasicDetails.InstallationDate.ToString());
            //        data.Add("InstallationAddress",installationAddress);
            //        data.Add("SystemSize",createJob.JobSystemDetails.SystemSize.ToString());
            //        data.Add("RefNumberOwnerName",RefNumberOwnerName);
            //        data.Add("STC", createJob.JobSystemDetails.CalculatedSTC.ToString());
            //       // data.Add("IsGst", dt.Rows[0]["IsGst"].ToString());
            //        CommonBAL.SetCacheDataForSTCSubmission(null, jobID, data);
            //    }
               
            //}
                   // CommonBAL.SetCacheDataForSTCSubmission(stcJobDetailsId, 0);
                
            
            return new KeyValuePair<bool,Int32>(keyValue.Key,jobID);
        }

        public KeyValuePair<bool, string> CalculateSTC(string sguType, string expectedInstallDate, string deemingPeriod, string postcode, string systemsize, string CalculateSTCUrl)
        {
            try
            {
                //DateTime dtUtc = Convert.ToDateTime(expectedInstallDate);
                //string resp;
                //System.Net.WebRequest req = System.Net.WebRequest.Create(CalculateSTCUrl);
                //req.ContentType = "application/json; charset=UTF-8";
                //req.Method = "POST";
                //using (var streamWriter = new StreamWriter(req.GetRequestStream()))
                //{
                //    deemingPeriod = deemingPeriod.Replace(" ", "_");
                //    string json = "{\"sguType\":\"" + sguType + "\",\"expectedInstallDate\":\"" + dtUtc.ToString("yyyy-MM-dd") + "T00:00:00.000Z" + "\",\"ratedPowerOutputInKw\":" + Convert.ToDecimal(systemsize) + ",\"deemingPeriod\":\"" + deemingPeriod.ToUpper() + "\",\"postcode\":\"" + postcode + "\",\"sguDisclaimer\":true}";
                //    streamWriter.Write(json);
                //    streamWriter.Flush();
                //}

                //var httpResponse = (HttpWebResponse)req.GetResponse();
                //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                //{
                //    resp = streamReader.ReadToEnd();
                //}

                //KeyValuePair<bool, string> keyvalue = new KeyValuePair<bool, string>(true, resp);
                //return keyvalue;
                //Helper.Log.WriteLog("Calculate STC");
                int deeminPeriodInYears = Common.numberTable[deemingPeriod.Split(' ')[0].ToLower()];
                decimal rating = _createJob.GetPostCodeZoneRating(Convert.ToInt32(postcode));
                decimal value = Convert.ToDecimal(systemsize) * rating * deeminPeriodInYears;
                string STCValue = Convert.ToString(Math.Floor(value));
                KeyValuePair<bool, string> keyvalue = new KeyValuePair<bool, string>(true, STCValue);
                return keyvalue;
            }
            catch (Exception e)
            {
                Helper.Log.WriteError(e,"Calculate STC");
                //Log.WriteError(e);
                KeyValuePair<bool, string> keyvalue = new KeyValuePair<bool, string>(false, null);
                return keyvalue;
            }
        }

        public KeyValuePair<bool, string> CalculateSWHSTC(string expectedInstallDate, string postcode, string systemBrand, string systemModel, string CalculateSWHSTCUrl)
        {
            try
            {
                DateTime dtUtc = Convert.ToDateTime(expectedInstallDate);
                string resp;
                System.Net.WebRequest req = System.Net.WebRequest.Create(CalculateSWHSTCUrl);
                req.ContentType = "application/json; charset=UTF-8";
                req.Method = "POST";
                using (var streamWriter = new StreamWriter(req.GetRequestStream()))
                {
                    string json = "{\"postcode\":\"" + postcode + "\",\"systemBrand\":\"" + systemBrand + "\",\"systemModel\":\"" + systemModel + "\",\"installationDate\":\"" + dtUtc.ToString("yyyy-MM-dd") + "T00:00:00.000Z\"}";
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }

                var httpResponse = (HttpWebResponse)req.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    resp = streamReader.ReadToEnd();
                }
                KeyValuePair<bool, string> keyvalue = new KeyValuePair<bool, string>(true, resp);
                return keyvalue;
            }
            catch (Exception e)
            {
                Log.WriteError(e);
                KeyValuePair<bool, string> keyvalue = new KeyValuePair<bool, string>(false, null);
                return keyvalue;
            }
        }

        public KeyValuePair<bool, decimal?> GetSTCValue(int jobType, string jobInstallationDate, string deemingPeriod, string postcode, decimal? SystemSize, string systemBrand, string systemModel, string CalculateSTCUrl)
        {
            decimal? jobSTCValue = null;

            string stcValue = string.Empty;

            KeyValuePair<bool, string> value = new KeyValuePair<bool, string>();

            string installationDate = jobInstallationDate != null ? Convert.ToDateTime(jobInstallationDate).ToString("yyyy-MM-dd") : string.Empty;
            if (jobType == 1 && SystemSize > 0)
            {
                value = CalculateSTC("SolarDeemed", installationDate, deemingPeriod, postcode, Convert.ToString(SystemSize), CalculateSTCUrl);
                if (string.IsNullOrEmpty(value.Value))
                    jobSTCValue = null;
                else
                    jobSTCValue = Convert.ToDecimal(value.Value);
            }
            if (jobType == 2 && jobInstallationDate != null)
            {
                value = CalculateSWHSTC(installationDate, postcode, systemBrand, systemModel, CalculateSTCUrl);
                if (value.Key)
                {
                    stcValue = value.Value;
                }

                if (!string.IsNullOrEmpty(stcValue))
                {
                    dynamic STCJsonData = JsonConvert.DeserializeObject(stcValue);
                    if (STCJsonData.result != null)
                    {
                        dynamic noofSTC = JsonConvert.DeserializeObject(Convert.ToString(STCJsonData.result));
                        if (noofSTC != null)
                        {
                            if (jobType == 1)
                                jobSTCValue = noofSTC.numberOfStcs;
                            else
                                jobSTCValue = noofSTC.numStc;
                        }
                        else
                            jobSTCValue = null;
                    }
                    else
                        jobSTCValue = null;
                }
                else
                    jobSTCValue = null;
            }

            

            KeyValuePair<bool, decimal?> calculatedStcValue = new KeyValuePair<bool, decimal?>(value.Key,jobSTCValue);
            return calculatedStcValue;
        }

        public bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        public List<SelectListItem> GetDeemingPeriod(string year)
        {
            int jobYear = DateTime.Now.Year;
            if (!string.IsNullOrEmpty(year))
            {
                int.TryParse(year, out jobYear);
            }
            List<SelectListItem> Items = _createJob.GetDeemingPeriod(jobYear).Select(a => new SelectListItem { Text = a, Value = a }).ToList();
            return Items;
        }

        public void SendMailOnCERFailed(string stcjobids, bool isWindowService = false)
        {
            if (isWindowService)
                Common.Log("CER failed stcjobIDS : " + stcjobids);

            foreach (string id in stcjobids.Split(','))
            {
                int STCJobDetailsId = Convert.ToInt32(id);
                DataSet ds = _createJob.GetDetailsOfCERFailedJobForMail(STCJobDetailsId);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    EmailInfo emailInfo = new EmailInfo();
                    emailInfo.TemplateID = 34;
                    emailInfo.ReferenceNumber = Convert.ToString(dr["ReferenceNumber"]);
                    emailInfo.OwnerName = Convert.ToString(dr["OwnerName"]);
                    emailInfo.InstallationAddress = Convert.ToString(dr["InstallationAddress"]);
                    emailInfo.SystemSize = (dr["SystemSize"]).ToString() != "" ? Convert.ToDecimal(dr["SystemSize"]) : Convert.ToDecimal(0);
                    emailInfo.STCsValue = Convert.ToString(dr["STCsValue"]);
                    emailInfo.TotalValue = (dr["TotalValue"]).ToString() != "" ? Convert.ToDecimal(dr["TotalValue"]) : Convert.ToDecimal(0);
                    emailInfo.ResellerFullName = Convert.ToString(dr["ResellerFullName"]);
                    emailInfo.JobID = Convert.ToInt32(dr["JobId"]);
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(dr["FailureNotice"])) && Convert.ToString(dr["FailureNotice"]).Contains("Auditor"))
                        emailInfo.FailureNotice = Convert.ToString(dr["FailureNotice"]).Remove(Convert.ToString(dr["FailureNotice"]).IndexOf("Auditor"));
                    else
                        emailInfo.FailureNotice = Convert.ToString(dr["FailureNotice"]);
                    emailInfo.Date = DateTime.Now;
                    _emailBAL.ComposeAndSendEmail(emailInfo, Convert.ToString(dr["EmailAddresses"]), null, null, default(Guid), id);
                    if (isWindowService)
                        Common.Log("Send email on CER failed stcjobdetailID : " + STCJobDetailsId);
                    else
                        Helper.Log.WriteLog("Send email on CER failed stcjobdetailID : " + STCJobDetailsId);
                }
            }
        }

        public void CreateSTCInvoicePDFForRECData(DataTable dt, bool isWindowService = false)
        {
            string message = string.Empty;
            try
            {
                DataTable dtUpdate = new DataTable();
                dtUpdate.Columns.Add("STCInvoiceNumber", typeof(string));
                dtUpdate.Columns.Add("STCInvoiceFilePath", typeof(string));

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        string filepath = _generateStcReportBAL.CreateStcReportNew("CreateStcReport", "Pdf", Convert.ToInt32(dr["STCJobDetailsID"]), Convert.ToString(dr["STCInvoiceNumber"]), Convert.ToString(dr["SolarCompanyId"]), "2", Convert.ToInt32(dr["ResellerUserId"]), Convert.ToInt32(dr["ResellerID"]), isWindowService);
                        DataRow drUpdate = dtUpdate.NewRow();
                        drUpdate["STCInvoiceNumber"] = Convert.ToString(dr["STCInvoiceNumber"]);
                        drUpdate["STCInvoiceFilePath"] = filepath;
                        dtUpdate.Rows.Add(drUpdate);
                        message = "STCInvoice file is generate successfully for " + Convert.ToString(dr["STCInvoiceNumber"]);
                        if (isWindowService)
                            Common.Log(message);
                        else
                            Helper.Log.WriteLog(message);
                    }
                    catch (Exception ex)
                    {
                        message = "An error has occured while generating STCInvioce file for " + Convert.ToString(dr["STCInvoiceNumber"]) + ": " + ex.Message;
                        if (isWindowService)
                            Common.Log(message);
                        else
                            Helper.Log.WriteError(ex, message);
                    }
                }
                var values = _stcInvoiceBal.UpdateRecGeneratedInvoiceFilePath(dtUpdate);
                message = "Invoice file paths are updated successfully in STCInvoice.";
                if (isWindowService)
                    Common.Log(message);
                else
                    Helper.Log.WriteLog(message);
            }
            catch (Exception ex)
            {
                message = "An error has occured while generating STCInvoice files: " + ex.Message;
                if (isWindowService)
                    Common.Log(message);
                else
                    Helper.Log.WriteError(ex, message);
            }
        }

    }
}
  