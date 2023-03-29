using FormBot.BAL.Service;
using FormBot.BAL.Service.CheckList;
using FormBot.BAL.Service.CommonRules;
using FormBot.BAL.Service.SPV;
using FormBot.Entity;
using FormBot.Entity.Email;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.ServiceProcess;
using System.Timers;
using FormBot.Helper.Helper;
using FormBot.Helper;
using System.Text;
using Ionic.Zip;
using FormBot.BAL.Service.RECRegistry;

namespace PriorityWindowService
{
    public partial class PriorityService : ServiceBase
    {
        private static readonly Logger _logger = new Logger();
        System.Timers.Timer priorityServiceTimer = null;
        System.Timers.Timer RECDataServiceTimer = null;
        System.Timers.Timer RECAutoUploadServiceTimer = null;
        System.Timers.Timer resellerEmailTimer = null;
        System.Timers.Timer solarCompanyRegisteredWithGST = null;
        System.Timers.Timer jobOwnerRegisteredWithGST = null;
        System.Timers.Timer deleteTempCheckListItem = null;
        System.Timers.Timer resetPwdflag = null;
        System.Timers.Timer updateCacheTimer = null;

        System.Timers.Timer sentMailTimer = null;
        System.Timers.Timer recUploadTimer = null;
        System.Timers.Timer SPVProductVerificationTimer = null;
        System.Timers.Timer SyncVEEcScheduleActivityPremiseFromVEECPortal = null;
        System.Timers.Timer urgentJobsTimer = null;
        Timer updateEntityName = null;
        Timer recDeleteFolderTimer = null;

        CreateJobBAL objCreateJobBAL = null;
        EmailBAL emailBAL = null;
        FormBot.BAL.ResellerBAL objResellerBAL = null;
        STCInvoiceBAL _stcInvoiceServiceBAL = null;
        SAASInvoiceBAL _saasInvoiceServiceBAL = null;
        SolarCompanyBAL _solarCompanyBAL = null;
        CheckListItemBAL _checkListItemBAL = null;
        GenerateStcReportBAL _generateStcReportBAL = null;
        SMTPDetails smtpDetail = null;
        //CommonRECMethodsBAL commonRECMethodsBAL = null;
        LoginBAL _login = null;
        SpvLogBAL _spvLog = null;
        CreateJobBAL _createJob = null;
        UserBAL _userBAL = null;
        JobRulesBAL _jobRuleBAL = null;
        EmailBAL _emailBAL = null;
        CommonBulkUploadToCERBAL _commonBulkUploadToCER = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityService"/> class.
        /// </summary>
        public PriorityService()
        {
            InitializeComponent();
            objCreateJobBAL = new CreateJobBAL();
            emailBAL = new EmailBAL();
            objResellerBAL = new FormBot.BAL.ResellerBAL();
            _stcInvoiceServiceBAL = new STCInvoiceBAL();
            _saasInvoiceServiceBAL = new SAASInvoiceBAL();
            _solarCompanyBAL = new SolarCompanyBAL();
            _checkListItemBAL = new CheckListItemBAL();
            _generateStcReportBAL = new GenerateStcReportBAL(_stcInvoiceServiceBAL);
            _commonBulkUploadToCER = new CommonBulkUploadToCERBAL(objCreateJobBAL);
            //commonRECMethodsBAL = new CommonRECMethodsBAL();

            _emailBAL = new EmailBAL();
            _login = new LoginBAL();
            _spvLog = new SpvLogBAL();
            _createJob = new CreateJobBAL();
            _userBAL = new UserBAL();
            _jobRuleBAL = new JobRulesBAL(_createJob, _emailBAL, _generateStcReportBAL, _stcInvoiceServiceBAL);
            smtpDetail = new SMTPDetails();
            smtpDetail.MailFrom = ConfigurationManager.AppSettings["MailFrom"];
            smtpDetail.SMTPUserName = ConfigurationManager.AppSettings["SMTPUserName"];
            smtpDetail.SMTPPassword = ConfigurationManager.AppSettings["SMTPPassword"];
            smtpDetail.SMTPHost = ConfigurationManager.AppSettings["SMTPHost"];
            smtpDetail.SMTPPort = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
            smtpDetail.IsSMTPEnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSMTPEnableSsl"]);
        }

        #region Prority Service

        /// <summary>
        /// Handles the Elapsed event of the serviceTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        void serviceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Common.Log("serviceTimer_Elapsed call");
            UpdatePriorityForJobs();
            priorityServiceTimer.Interval = CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["TimeToRun"]));
            Common.Log("serviceTimer_Elapsed completed");
        }

        #endregion;

        #region Make Solar Company Registered with GST

        /// <summary>
        /// Handles the Elapsed event of the serviceTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        void SolarCompanyGSTserviceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Common.Log("SolarCompanyGSTserviceTimer_Elapsed call");
            MakeSCARegisteredWithGST();
            solarCompanyRegisteredWithGST.Interval = CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["SolarCompanyGSTTimeToRun"]));
            Common.Log("SolarCompanyGSTserviceTimer_Elapsed completed");
        }

        /// <summary>
        /// Handles the Elapsed event of the serviceTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        void DeleteTempCheckListItemserviceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Common.Log("DeleteTempCheckListItemserviceTimer_Elapsed call");
            DeleteTempCheckList();
            deleteTempCheckListItem.Interval = CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["SolarCompanyGSTTimeToRun"]));
            Common.Log("DeleteTempCheckListItemserviceTimer_Elapsed completed");
        }

        /// <summary>
		/// Handles the Elapsed event of the serviceTimer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
		void JobOwnerGSTserviceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Common.Log("JobOwnerGSTserviceTimer_Elapsed call");
            MakeOwnerRegisteredWithGST();
            jobOwnerRegisteredWithGST.Interval = CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["JobOwnerGSTTimeToRun"]));
            Common.Log("JobOwnerGSTserviceTimer_Elapsed completed");
        }
        /// <summary>
        /// Handles the Elapsed event of the serviceTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        void UpdateCacheTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (DateTime.Now.DayOfWeek.ToString().ToLower() == Convert.ToString(ConfigurationManager.AppSettings["UpdateCacheOnDayOfWeek"]).ToLower())
            {
                Common.Log("UpdateCacheTimer_Elapsed call");
                UpdateCacheForStcSubmission(0);
                updateCacheTimer.Interval = CalculateTimeIntervalInDays(Convert.ToString(ConfigurationManager.AppSettings["UpdateCacheTimeToRun"]));
                Common.Log("UpdateCacheTimer_Elapsed completed");
            }
        }

        #endregion

        #region REC Data Service

        /// <summary>
        /// Handles the Elapsed event of the RECDataServiceTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        void RECDataServiceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Common.Log("RECDataServiceTimer_Elapsed call");
            GetRECData();
            RECDataServiceTimer.Interval = CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["RECDataServiceTimeToRun"]));
            Common.Log("RECDataServiceTimer_Elapsed completed");
        }

        /// <summary>
        /// Handles the Elapsed event of the RECDataServiceTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        void RECAutoUploadServiceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Common.Log("RECAutoUploadServiceTimer_Elapsed call");
                AutoCreateRECBatch(true);
                AutoCreateRECBatch(false);
                RECAutoUploadServiceTimer.Interval = CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["RECAutoUploadServiceTimeToRun"]));
                Common.Log("RECAutoUploadServiceTimer_Elapsed completed");
            }
            catch (Exception ex)
            {
                Common.Log("Error:" + ex.ToString());
            }
        }

        void RECUploadTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Common.Log("start recUploadTimer");
                GetRECDataForUpload();
            }
            catch (Exception ex)
            {
                Common.Log("ERROR:" + ex.ToString());
            }
            finally
            {
            }
        }

        void RECDeleteFolderTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Common.Log("start recDeleteFolderTimer");
                RECDeleteFolder();
                recDeleteFolderTimer.Interval = CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["RECDeleteFolderTimeToRun"]));
                Common.Log("End recDeleteFolderTimer");
            }
            catch (Exception ex)
            {
                Common.Log("ERROR:" + ex.ToString());
            }
            finally
            {
            }
        }
        public void GetRECDataForUpload()
        {
            try
            {
                DataSet ds = _createJob.GetJobsForRecSubmission();
                DataTable dt = new DataTable();
                string jobids = string.Empty;
                string jobs = string.Empty;
                if (ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                    FormBot.Entity.RECAccount objResellerUser = new FormBot.Entity.RECAccount();
                    if (dt.Rows.Count > 0)
                    {
                        var distinctIds = dt.AsEnumerable().Select(s => new { id = s.Field<string>("GBRecBulkUploadId"), }).Distinct().ToList();
                        Common.Log("Batch Count: " + distinctIds.Count);
                        for (int i = 0; i < distinctIds.Count; i++)
                        {
                            jobids = string.Empty;
                            jobs = string.Empty;
                            Common.Log(distinctIds[i].id);
                            objResellerUser = new FormBot.Entity.RECAccount();
                            DataRow[] drs = dt.Select("GBRecBulkUploadId='" + distinctIds[i].id.ToString() + "'");
                            foreach (DataRow dr in drs)
                            {
                                jobids = jobids + dr["JobId"].ToString() + ",";

                                int settlementterm = !string.IsNullOrEmpty(dr["STCSettlementTerm"].ToString()) ? Convert.ToInt32(dr["STCSettlementTerm"]) : 0;
                                int customsettlementterm = !string.IsNullOrEmpty(dr["CustomSettlementTerm"].ToString()) ? Convert.ToInt32(dr["CustomSettlementTerm"]) : 0;

                                if ((settlementterm != 4 && settlementterm != 8 && settlementterm != 12 && settlementterm != 10) ||
                                    (settlementterm == 10 && (customsettlementterm != 4 && customsettlementterm != 8 && customsettlementterm != 12)))
                                {
                                    jobs = jobs + dr["STCJobDetailsID"].ToString() + "_" + dr["STCSettlementTerm"] + "_" + dr["JobId"].ToString() + ",";
                                }

                                //jobs = jobs + dr["STCJobDetailsID"].ToString()+"_"+dr["STCSettlementTerm"]+"_" + dr["JobId"].ToString() + ",";
                            }
                            _stcInvoiceServiceBAL.UpdateQueuedSubmissionStatus(distinctIds[i].id.ToString(), "REC Start");
                            jobids = jobids.Remove(jobids.Length - 1);
                            if (jobs.Length > 0)
                                jobs = jobs.Remove(jobs.Length - 1);
                            objResellerUser.CERLoginId = drs[0]["CERLoginId"].ToString();
                            objResellerUser.CERPassword = drs[0]["CERPassword"].ToString();
                            objResellerUser.RECAccName = drs[0]["RECAccName"].ToString();

                            InsertEntryInRec(drs[0]["ResellerId"].ToString(), jobs, jobids, drs[0]["JobType"].ToString(), Convert.ToInt32(drs[0]["CreatedBy"]), Convert.ToInt32(drs[0]["UserTypeId"]), drs[0]["datetimeticks"].ToString(), objResellerUser);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.Log(ex.Message);
            }
        }

        public void AutoCreateRECBatch(bool IsBeforeAprilInstallation)
        {
            Common.Log("AutoCreateRECBatch");
            string FilePath = string.Empty;
            string UploadURL = string.Empty;
            string referer = string.Empty;
            string paramname = string.Empty;
            string spvParamName = string.Empty;
            string spvFilePath = string.Empty;
            string sguBulkUploadDocumentsParamName = string.Empty;
            string sguBulkUploadDocumentsFilePath = string.Empty;
            string JobIdForException = string.Empty;
            string JobIDs = "";
            string JobType = "1";
            int UserId = 0;
            decimal TotalSTC = 0;
            string CERLoginId = "";
            string CERPassword = "";
            string RECAccName = "";
            string RECCompanyName = "";
            string LoginType = "";
            string stcJobDetailsId = "";

            List<CheckSPVrequired> chkspvRequired = new List<CheckSPVrequired>();
            DataSet dsJobDetailsID = new DataSet();

            List<Reseller> resellers = new List<Reseller>();
            resellers = objResellerBAL.GetAllResellersForREC();
            foreach (Reseller reseller in resellers)
            {
                clsUploadedFileJsonResponseObject JsonResponseObj = new clsUploadedFileJsonResponseObject();
                JobIDs = "";
                CERLoginId = "";
                CERPassword = "";
                RECAccName = "";
                LoginType = "";
                stcJobDetailsId = "";

                List<RECAccount> recAcc = new List<RECAccount>();
                recAcc = _userBAL.GetAllResellerUserRECCredentialsByResellerID(reseller.ResellerID);
                if (recAcc != null && recAcc.Count > 0 && !string.IsNullOrWhiteSpace(recAcc[0].CERLoginId))
                {
                    try
                    {
                        dsJobDetailsID = _stcInvoiceServiceBAL.GetJobIdsForBatchByResellerWise(reseller.ResellerID, IsBeforeAprilInstallation);
                        dsJobDetailsID.Tables[0].AsEnumerable().ToList().ForEach(d =>
                        {
                            if (d.Field<bool>("IsSPVRequired") == false || (d.Field<bool>("IsSPVRequired") == true && d.Field<bool>("IsSPVInstallationVerified") == true))
                            {
                                stcJobDetailsId += d.Field<int>("STCJobDetailsID").ToString() + ",";
                                JobIDs += d.Field<int>("JobID").ToString() + ",";
                                TotalSTC += d.Field<decimal>("TradedSTC");
                                CheckSPVrequired chkspv = new CheckSPVrequired();
                                chkspv.IsSPVRequired = d.Field<bool>("IsSPVRequired");
                                chkspv.JobId = d.Field<int>("JobID");
                                chkspv.STCJobDetailsID = d.Field<int>("STCJobDetailsID");
                                chkspvRequired.Add(chkspv);
                            }
                        });

                        //if (!string.IsNullOrWhiteSpace(stcJobDetailsId) && chkspvRequired.Count > 0)
                        //{
                        //    AutoChangeSTCJobStageRTC(stcJobDetailsId, reseller.UserId, chkspvRequired);
                        //}
                        List<string> lstJobIds = JobIDs.Split(',').ToList();
                        int BatchRecUploadCount = Convert.ToInt32(ConfigurationManager.AppSettings["BatchRecUploadCount"]);
                        Common.Log("JobIds: " + JobIDs);
                        List<string> lstBatches = SplitList<string>(lstJobIds, BatchRecUploadCount);
                        //DataTable dtBatches = ListToDataTable(lstBatches);
                        // For caching purpose

                        foreach (var JobIds in lstBatches)
                        {
                            List<string> jids = new List<string>();
                            jids.Add(JobIds);
                            DataTable dtBatches = ListToDataTable(jids);

                            if (!string.IsNullOrWhiteSpace(JobIDs))
                            {

                                RECAccount recAccReseller = recAcc.Where(x => x.RECLoginType == "Reseller").FirstOrDefault();
                                if (recAccReseller != null && !string.IsNullOrWhiteSpace(recAccReseller.RECAccName) && !string.IsNullOrWhiteSpace(recAccReseller.CERLoginId) && !string.IsNullOrWhiteSpace(recAccReseller.CERPassword))
                                {
                                    CERLoginId = recAccReseller.CERLoginId;
                                    CERPassword = recAccReseller.CERPassword;
                                    RECAccName = recAccReseller.RECAccName;
                                    LoginType = "Reseller";
                                    RECCompanyName = recAccReseller.RECCompName;
                                }
                                else
                                {
                                    RECAccount recAccAdmin = recAcc.Where(x => x.RECLoginType == "Admin").FirstOrDefault();
                                    if (recAccAdmin != null && !string.IsNullOrWhiteSpace(recAccAdmin.RECAccName) && !string.IsNullOrWhiteSpace(recAccAdmin.CERLoginId) && !string.IsNullOrWhiteSpace(recAccAdmin.CERPassword))
                                    {
                                        CERLoginId = recAccAdmin.CERLoginId;
                                        CERPassword = recAccAdmin.CERPassword;
                                        RECAccName = recAccAdmin.RECAccName;
                                        LoginType = "Admin";
                                        RECCompanyName = recAccAdmin.RECCompName;
                                    }
                                }
                                Common.Log("Activity: Updating BatchID in DB");
                                long dateTimeTicks = DateTime.Now.Ticks;
                                DataSet dsStcJobDetailsId = _createJob.InsertGBBatchRECUploadId(dtBatches, Convert.ToInt32(reseller.ResellerID), dateTimeTicks.ToString(), TotalSTC, reseller.UserId, CERLoginId, CERPassword, RECAccName, LoginType, RECCompanyName);
                                Common.Log("Activity: Update BatchID In db Completed");
                                string stcjobids = string.Empty;
                                string gbBatchRecUploadId = "";
                                if (dsStcJobDetailsId.Tables.Count > 0)
                                {
                                    if (dsStcJobDetailsId.Tables[0].Rows.Count > 0)
                                    {
                                        foreach (DataRow dr in dsStcJobDetailsId.Tables[0].Rows)
                                        {
                                            stcjobids = stcjobids + dr["StcJobDetailsId"].ToString() + ",";
                                        }
                                        stcjobids = stcjobids.Remove(stcjobids.Length - 1);
                                        DataTable dtGetSTCData = _createJob.GetSTCDetailsAndJobDataForCache(stcjobids, null);

                                        if (dtGetSTCData.Rows.Count > 0)
                                        {
                                            for (int i = 0; i < dtGetSTCData.Rows.Count; i++)
                                            {
                                                SortedList<string, string> data = new SortedList<string, string>();
                                                gbBatchRecUploadId = dtGetSTCData.Rows[i]["GBBatchRECUploadId"].ToString();
                                                data.Add("GBBatchRECUploadId", gbBatchRecUploadId);
                                                SetCacheDataForStcJobIdFromService(Convert.ToInt32(dtGetSTCData.Rows[i]["StcJobDetailsId"].ToString()), null, data);
                                                //CommonBAL.SetCacheDataForSTCSubmission(Convert.ToInt32(dtGetSTCData.Rows[i]["StcJobDetailsId"].ToString()), null, data);
                                                Common.Log(DateTime.Now + " Update cache for stcid from AutoCreateRECBatch: " + (dtGetSTCData.Rows[i]["StcJobDetailsId"].ToString()) + " BulkUploadId: " + gbBatchRecUploadId);
                                            }
                                        }
                                    }
                                    //foreach (var JobIds in lstBatches)
                                    //{
                                    JobIdForException = string.Empty;
                                    JobIdForException = JobIds;
                                    DataSet ds = _createJob.GetJobsForRecInsertOrUpdateNew(JobIds);

                                    if (ds.Tables.Count > 0)
                                    {
                                        // Get Reseller user REC Credentials from User table                    
                                        string NotInRecSTCJobDetailsID = "";
                                        if (ds.Tables[0].Rows.Count > 0 || ds.Tables[1].Rows.Count > 0)
                                        {
                                            string NotInRecJobIds = ds.Tables[0].Rows.Count > 0 ? string.Join(",", ds.Tables[0].AsEnumerable().Select(s => s.Field<int>("JobID")).ToArray()) : string.Join(",", ds.Tables[1].AsEnumerable().Select(s => s.Field<int>("JobID")).ToArray());
                                            NotInRecSTCJobDetailsID = ds.Tables[0].Rows.Count > 0 ? string.Join(",", ds.Tables[0].AsEnumerable().Select(s => s.Field<int>("STCJobDetailsID")).ToArray()) : string.Join(",", ds.Tables[1].AsEnumerable().Select(s => s.Field<int>("STCJobDetailsID")).ToArray());

                                            DataSet dsCSV_JobID = new DataSet();
                                            DataTable dtSPVXmlPath = new DataTable();

                                            #region Generate CSV File Based on Selected Job IDs

                                            FilePath = ProjectSession.ProofDocuments + "\\UserDocuments\\" + dateTimeTicks + ".csv";

                                            if (JobType == "1") // PVD Jobs
                                                _commonBulkUploadToCER.GetBulkUploadCSV_PVD(NotInRecJobIds, FilePath, ref dsCSV_JobID, ref dtSPVXmlPath, true);
                                            else  // SWH Jobs
                                                _commonBulkUploadToCER.GetBulkUploadSWHCSV(NotInRecJobIds, FilePath, ref dsCSV_JobID, true);

                                            if (JobType == "1")
                                            {
                                                if (dsCSV_JobID.Tables.Count > 0 && dsCSV_JobID.Tables[0] != null)

                                                    foreach (DataRow dr in dsCSV_JobID.Tables[0].Rows)
                                                    {
                                                        string srcPath = ProjectSession.ProofDocuments + "\\JobDocuments\\" + Convert.ToString(dr["JobId"]) + "\\" + dr["Documents Zip File"].ToString();
                                                        string fileName = System.IO.Path.GetFileName(srcPath);
                                                        string destFolder = ProjectSession.ProofDocuments + "\\UserDocuments\\" + dateTimeTicks;
                                                        string destPath = Path.Combine(destFolder + "\\" + fileName);

                                                        if (!Directory.Exists(destFolder))
                                                        {
                                                            GC.Collect();
                                                            GC.WaitForPendingFinalizers();
                                                            Directory.CreateDirectory(destFolder);
                                                        }

                                                        if (System.IO.File.Exists(srcPath))
                                                        {
                                                            System.IO.File.Copy(srcPath, destPath, true);
                                                        }
                                                    }

                                                #region Creating zip file

                                                string InputDirectory = ProjectSession.ProofDocuments + "\\UserDocuments\\" + dateTimeTicks;

                                                sguBulkUploadDocumentsFilePath = ProjectSession.ProofDocuments + "\\UserDocuments\\" + dateTimeTicks + "_REC.zip";

                                                using (Stream zipStream = new FileStream(Path.GetFullPath(sguBulkUploadDocumentsFilePath), FileMode.Create, FileAccess.Write))
                                                using (System.IO.Compression.ZipArchive archive = new System.IO.Compression.ZipArchive(zipStream, System.IO.Compression.ZipArchiveMode.Create))
                                                {
                                                    bool IsAnyFileExists = false;
                                                    foreach (var filePath in System.IO.Directory.GetFiles(InputDirectory, "*.*", System.IO.SearchOption.AllDirectories))
                                                    {
                                                        var relativePath = Path.GetFileName(filePath);//filePath.Replace(InputDirectory, string.Empty);
                                                        using (Stream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                                                        using (Stream fileStreamInZip = archive.CreateEntry(relativePath).Open())
                                                            fileStream.CopyTo(fileStreamInZip);

                                                        IsAnyFileExists = true;
                                                    }
                                                    if (!IsAnyFileExists)
                                                        sguBulkUploadDocumentsFilePath = "";
                                                }
                                                #endregion Creating zip file

                                                #endregion
                                            }

                                            #region REC Regsitry call and get response from REC
                                            // PVD Jobs
                                            if (JobType == "1")
                                            {
                                                // Zip files for SPV Jobs
                                                int spvDataCount = 0;
                                                if (dsCSV_JobID.Tables[0] != null)
                                                {
                                                    spvDataCount = dsCSV_JobID.Tables[0].Select("[Signed data package] <> ''").Count();
                                                }
                                                if (spvDataCount > 0)
                                                {
                                                    spvParamName = "sguBulkUploadSdpZip";
                                                    spvFilePath = ProjectSession.ProofDocuments + "\\UserDocuments\\" + dateTimeTicks + ".zip";
                                                    Common.Log("spvfilePath: " + spvFilePath);
                                                    using (ZipFile zip = new ZipFile())
                                                    {
                                                        foreach (DataRow dr in dtSPVXmlPath.Rows)
                                                        {
                                                            zip.AddFile(Path.Combine(ProjectSession.ProofDocuments, dr["Path"].ToString()), "");
                                                            zip.Save(spvFilePath);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        _stcInvoiceServiceBAL.UpdateQueuedSubmissionStatus(gbBatchRecUploadId, "In Queue");
                                    }

                                    #endregion
                                    // }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _stcInvoiceServiceBAL.RemoveFromQueuedRecSubmission(JobIdForException);
                        Common.Log(DateTime.Now + " Exception in insert in rec:" + ex.Message);
                        // It will execute when throws error before REC Registry                 
                        JsonResponseObj = new clsUploadedFileJsonResponseObject();
                        JsonResponseObj.status = "Failed";
                        JsonResponseObj.strErrors = "<ul><li>" + ex.Message + "</li></ul>";
                    }
                }
            }
        }

        public void InsertEntryInRec(string resellerId, string jobs, string JobIDs, string JobType, int UserId, int UserTypeId, string datetimeticks, RECAccount objResellerUser)
        {
            string FilePath = string.Empty;
            string UploadURL = string.Empty;
            string referer = string.Empty;
            string paramname = string.Empty;
            string spvParamName = string.Empty;
            string spvFilePath = string.Empty;
            bool IsPVDJob = false;
            string sguBulkUploadDocumentsParamName = string.Empty;
            string sguBulkUploadDocumentsFilePath = string.Empty;
            string BatchUploadId = string.Empty;
            string stcjobids = string.Empty;
            clsUploadedFileJsonResponseObject JsonResponseObj = new clsUploadedFileJsonResponseObject();
            try
            {
                List<string> lstJobIds = JobIDs.Split(',').ToList();
                List<int> peakpaySTCJobIds = new List<int>();
                List<int> lstSTCJobids = new List<int>();
                int BatchRecUploadCount = Convert.ToInt32(ConfigurationManager.AppSettings["BatchRecUploadCount"]);

                List<string> lstBatches = SplitList<string>(lstJobIds, BatchRecUploadCount);
                DataTable dtBatches = ListToDataTable(lstBatches);
                Common.Log("InsertINREC Completed");
                foreach (var JobIds in lstBatches)
                {
                    DataSet ds = _createJob.GetJobsForRecInsertOrUpdate(JobIds);
                    

                    if (ds.Tables.Count > 0)
                    {
                        #region for update cache get stcjobdetails IDs
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                             peakpaySTCJobIds = ds.Tables[0].AsEnumerable().Where(r => r.Field<int>("STCSettlementTerm") == 12 || (r.Field<int>("STCSettlementTerm") == 10 && r.Field<int>("CustomSettlementTerm") == 12)).Select(dr => dr.Field<int>("STCJobDetailsId")).Distinct().ToList();
                            //foreach (DataRow dr in ds.Tables[0].Rows)
                            //{
                            //    stcjobids = stcjobids + dr["StcJobDetailsId"].ToString() + ",";
                            //}
                            lstSTCJobids = ds.Tables[0].AsEnumerable().Select(r => r.Field<int>("STCJobDetailsId")).Distinct().ToList();
                            stcjobids =lstSTCJobids.Count>0? string.Join(",", lstSTCJobids):string.Empty;

                        }
                        else if (ds.Tables[1].Rows.Count > 0)
                        {
                            peakpaySTCJobIds = ds.Tables[1].AsEnumerable().Where(r => r.Field<int>("STCSettlementTerm") == 12 || (r.Field<int>("STCSettlementTerm") == 10 && r.Field<int>("CustomSettlementTerm") == 12)).Select(dr => dr.Field<int>("STCJobDetailsId")).Distinct().ToList();

                            //foreach (DataRow dr in ds.Tables[1].Rows)
                            //{
                            //    stcjobids = stcjobids + dr["StcJobDetailsId"].ToString() + ",";
                            //}
                            lstSTCJobids = ds.Tables[1].AsEnumerable().Select(r => r.Field<int>("STCJobDetailsId")).Distinct().ToList();
                            stcjobids =lstSTCJobids.Count>0? string.Join(",", lstSTCJobids):string.Empty;
                        }
                        #endregion
                        // Get Reseller user REC Credentials from User table                    
                        //FormBot.Entity.RECAccount objResellerUser = new FormBot.Entity.RECAccount();
                        //objResellerUser = _userBAL.GetResellerUserRECCredentialsByResellerID(Convert.ToInt32(resellerId));
                        FilePath = ProjectSession.ProofDocuments + "\\UserDocuments\\" + datetimeticks + ".csv";
                        Common.Log(FilePath);
                        string InRecSTCJobDetailsID = "";
                        string NotInRecSTCJobDetailsID = "";
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            List<string> UniqueBatchId = ds.Tables[0].AsEnumerable().Select(s => s.Field<string>("GBBatchRECUploadId")).Distinct().ToList();
                            for (int j = 0; j < UniqueBatchId.Count; j++)
                            {
                                Common.Log(UniqueBatchId[j].Split('-')[0]);
                                var tempRECBulkUploadId = UniqueBatchId[j].Split('-');
                                BatchUploadId = string.Empty;
                                BatchUploadId = tempRECBulkUploadId[0] + "-".ToString();
                                string RECBulkuploadId = tempRECBulkUploadId.Length > 1 ? tempRECBulkUploadId[1] : string.Empty;
                                string InRecJobIds = string.Join(",", ds.Tables[0].AsEnumerable().Where(s => s.Field<string>("GBBatchRECUploadId") == UniqueBatchId[j]).Select(s => s.Field<int>("JobID")).ToArray());
                                string SerialNumber = string.Join(",", ds.Tables[0].AsEnumerable().Where(s => s.Field<string>("GBBatchRECUploadId") == UniqueBatchId[j]).Select(s => s.Field<string>("SerialNumbers")).FirstOrDefault());
                                InRecSTCJobDetailsID = string.Join(",", ds.Tables[0].AsEnumerable().Where(s => s.Field<string>("GBBatchRECUploadId") == UniqueBatchId[j]).Select(s => s.Field<int>("STCJobDetailsID")).ToArray());
                                string RefNumber = string.Join(",", ds.Tables[0].AsEnumerable().Where(s => s.Field<string>("GBBatchRECUploadId") == UniqueBatchId[j]).Select(s => s.Field<string>("RefNumber")).FirstOrDefault());
                                string OwnerLastName = string.Join(",", ds.Tables[0].AsEnumerable().Where(s => s.Field<string>("GBBatchRECUploadId") == UniqueBatchId[j]).Select(s => s.Field<string>("LastName")).FirstOrDefault());
                                string FromDate = string.Join(",", ds.Tables[0].AsEnumerable().Where(s => s.Field<string>("GBBatchRECUploadId") == UniqueBatchId[j]).Select(s => s.Field<DateTime>("RECBulkUploadTime")).FirstOrDefault().ToString());
                                DataSet dsCSV = JobType == "1" ? _createJob.GetBulkUploadForJob(InRecJobIds) : _createJob.GetSWHBulkUploadForJob(InRecJobIds);
                                Common.Log("Method: InsertEntryInRec Action:AuthenticateUser_UploadFileForREC when we have GBBatchRECUploadId for RECBulkUploadId:" + RECBulkuploadId);
                                RECRegistry.AuthenticateUser_UploadFileForREC(FilePath, ref JsonResponseObj, ref dsCSV, UploadURL, referer, paramname, JobType == "1" ? true : false, objResellerUser, InRecSTCJobDetailsID, UserId, SerialNumber, RECBulkuploadId, RefNumber, OwnerLastName, FromDate, tempRECBulkUploadId: tempRECBulkUploadId[0] + "-");
                            }
                        }
                        if (ds.Tables[1].Rows.Count > 0)
                        {
                            BatchUploadId = string.Empty;
                            BatchUploadId = ds.Tables[1].Rows[0]["GBBatchRECUploadId"].ToString();
                            string NotInRecJobIds = string.Join(",", ds.Tables[1].AsEnumerable().Select(s => s.Field<int>("JobID")).ToArray());
                            NotInRecSTCJobDetailsID = string.Join(",", ds.Tables[1].AsEnumerable().Select(s => s.Field<int>("STCJobDetailsID")).ToArray());

                            DataSet dsCSV_JobID = new DataSet();
                            DataTable dtSPVXmlPath = new DataTable();

                            #region Generate CSV File Based on Selected Job IDs
                            // Generate CSV Files based on selected job ids and get file path to upload file into REC Registry     
                            long dateTimeTicks = DateTime.Now.Ticks;
                            if (!string.IsNullOrWhiteSpace(datetimeticks))
                                dateTimeTicks = Convert.ToInt64(datetimeticks);

                            FilePath = ProjectSession.ProofDocuments + "\\UserDocuments\\" + dateTimeTicks + ".csv";
                            Common.Log("1");
                            Common.Log(FilePath);
                            if (JobType == "1") // PVD Jobs
                                _commonBulkUploadToCER.GetBulkUploadCSV_PVD(NotInRecJobIds, FilePath, ref dsCSV_JobID, ref dtSPVXmlPath, true);
                            else  // SWH Jobs
                                _commonBulkUploadToCER.GetBulkUploadSWHCSV(NotInRecJobIds, FilePath, ref dsCSV_JobID, true);

                            if (JobType == "1")
                            {
                                string InputDirectory = ProjectSession.ProofDocuments + "\\UserDocuments\\" + dateTimeTicks;
                                sguBulkUploadDocumentsFilePath = ProjectSession.ProofDocuments + "\\UserDocuments\\" + dateTimeTicks + "_REC.zip";
                                bool IsAnyFileExists = false;
                                if (Directory.Exists(InputDirectory))
                                {
                                    foreach (var filePath in System.IO.Directory.GetFiles(InputDirectory, "*.*", System.IO.SearchOption.AllDirectories))
                                    {
                                        IsAnyFileExists = true;
                                    }
                                }
                                if (!IsAnyFileExists)
                                    sguBulkUploadDocumentsFilePath = "";
                            }
                            #endregion
                            #region REC Regsitry call and get response from REC
                            // PVD Jobs
                            if (JobType == "1")
                            {
                                UploadURL = ConfigurationManager.AppSettings["RECUploadUrl"] + "rec-registry/app/smallunits/sgu/register-bulk";
                                referer = ConfigurationManager.AppSettings["RECUploadUrl"] + "rec-registry/app/smallunits/register-bulk-small-generation-unit";
                                paramname = "sguBulkUploadFile";
                                sguBulkUploadDocumentsParamName = "sguBulkUploadDocumentsZip";
                                IsPVDJob = true;

                                // Zip files for SPV Jobs
                                int spvDataCount = 0;
                                if (dsCSV_JobID.Tables[0] != null)
                                {
                                    spvDataCount = dsCSV_JobID.Tables[0].Select("[Signed data package] <> ''").Count();
                                }
                                if (spvDataCount > 0)
                                {
                                    spvFilePath = ProjectSession.ProofDocuments + "\\UserDocuments\\" + dateTimeTicks + ".zip";
                                }
                            }
                            else
                            {
                                // SWH Jobs (Still not confirmed this SWH URL)
                                UploadURL = ConfigurationManager.AppSettings["RECUploadUrl"] + "rec-registry/app/smallunits/swh/register-bulk";
                                referer = ConfigurationManager.AppSettings["RECUploadUrl"] + "rec -registry/app/smallunits/register-bulk-solar-water-heater";
                                paramname = "bulkUploadFile";
                                IsPVDJob = false;
                            }
                            if (objResellerUser != null && !string.IsNullOrEmpty(objResellerUser.CERLoginId) && !string.IsNullOrEmpty(objResellerUser.CERPassword) && !string.IsNullOrEmpty(objResellerUser.RECAccName))
                            {
                                Common.Log("Method: InsertEntryInRec Action:AuthenticateUser_UploadFileForREC when we don't have GBBatchRECUploadId:");
                                RECRegistry.AuthenticateUser_UploadFileForREC(FilePath, ref JsonResponseObj, ref dsCSV_JobID, UploadURL, referer, paramname, IsPVDJob, objResellerUser, NotInRecSTCJobDetailsID, UserId, spvParamName: spvParamName, spvFilePath: spvFilePath, sguBulkUploadDocumentsParamName: sguBulkUploadDocumentsParamName, sguBulkUploadDocumentsFilePath: sguBulkUploadDocumentsFilePath, tempRECBulkUploadId: ds.Tables[1].Rows[0]["GBBatchRECUploadId"].ToString());
                            }
                            else
                            {
                                JsonResponseObj = new clsUploadedFileJsonResponseObject();
                                JsonResponseObj.status = "Failed";
                                JsonResponseObj.strErrors = "<ul><li>Reseller REC Credentails not found.</li></ul>";
                            }
                        }

                        //// Generating Invoice after rec creation
                        List<string> lstSTCjobDetailsId = new List<string>();
                        if (!string.IsNullOrEmpty(InRecSTCJobDetailsID))
                            lstSTCjobDetailsId = InRecSTCJobDetailsID.Split(',').ToList();
                        if (!string.IsNullOrEmpty(NotInRecSTCJobDetailsID))
                            lstSTCjobDetailsId.AddRange(NotInRecSTCJobDetailsID.Split(',').ToList());
                        Common.Log("IspvdCodeUpdated start:" + jobs);

                        if (JsonResponseObj.IsPVDCodeUpdated)
                        {
                            Common.Log("IspvdCodeUpdated:" + JsonResponseObj.IsPVDCodeUpdated);
                            bool IsSuccess = true;
                            if (!string.IsNullOrEmpty(jobs))
                            {
                                List<string> stcjobid = jobs.Split(',').ToList();
                                List<string> lstSTCJobId = new List<string>();
                                for (int k = 0; k < stcjobid.Count; k++)
                                {
                                    for (int l = 0; l < lstSTCjobDetailsId.Count; l++)
                                    {
                                        if (stcjobid[k].Contains(lstSTCjobDetailsId[l] + "_"))
                                            lstSTCJobId.Add(stcjobid[k]);
                                    }
                                }
                                Common.Log("Calling save STCInvoice :" + string.Join(",", lstSTCJobId));
                                IsSuccess = SaveSTCInvoice(resellerId, string.Join(",", lstSTCJobId), UserTypeId: UserTypeId, userId: UserId, IsBackgroundRecProcess: true);
                                Common.Log("End Calling save STCInvoice :" + string.Join(",", lstSTCJobId));
                            }
                            if (IsSuccess)
                            {
                                JsonResponseObj = new clsUploadedFileJsonResponseObject();
                                JsonResponseObj.status = "Completed";
                                JsonResponseObj.strErrors = "<ul><li>File uploaded successfully</li></ul>";
                            }
                            else
                            {
                                JsonResponseObj = new clsUploadedFileJsonResponseObject();
                                JsonResponseObj.status = "Failed";
                                JsonResponseObj.strErrors = "<ul><li>Error while saving data into Invoice table</li></ul>";
                            }
                        }
                    }

                    #endregion
                }
                //stcjobids = stcjobids.Remove(stcjobids.Length - 1);

                if (!string.IsNullOrEmpty(stcjobids ))
                {
                    #region commented code
                    //int RAId = string.IsNullOrEmpty(resellerId) ? 0 : Convert.ToInt32(resellerId);
                    //Common.Log("Cache Update for RA: " + RAId);
                    //UpdateCacheForStcSubmission(RAId);
                    // DataTable dt = _createJob.GetSTCDetailsAndJobDataForCache(stcjobids,null);

                    //if (dt.Rows.Count > 0)
                    //{
                    //    for (int i = 0; i < dt.Rows.Count; i++)
                    //    {
                    //        SortedList<string, string> data = new SortedList<string, string>();
                    //        string RECBulkUploadTimeDate = dt.Rows[i]["RECBulkUploadTime"].ToString();
                    //        data.Add("RECBulkUploadTimeDate", RECBulkUploadTimeDate);
                    //        SetCacheDataForStcJobIdFromService(Convert.ToInt32(dt.Rows[i]["StcJobDetailsId"].ToString()), null, data);
                    //        //CommonBAL.SetCacheDataForSTCSubmission(Convert.ToInt32(dt.Rows[i]["StcJobDetailsId"].ToString()), null, data);
                    //    }
                    //}
                    #endregion
                    SetCacheDataForStcJobIds(stcjobids);
                    Common.Log("inside insertEntryInRec: peakpayjobscount: " + peakpaySTCJobIds.Count);
                  if(peakpaySTCJobIds != null && peakpaySTCJobIds.Count>0)
                    SetCacheDataForPeakPay(string.Join(",", peakpaySTCJobIds));
                }
            }
            catch (Exception ex)
            {
                Common.Log("InsertEntryInRec Error: " + ex);
                STCInvoiceBAL obj = new STCInvoiceBAL();
                obj.UpdateInternalIssue(BatchUploadId, ex.Message.ToString());
                // It will execute when throws error before REC Registry                 
                JsonResponseObj = new clsUploadedFileJsonResponseObject();
                JsonResponseObj.status = "Failed";
                JsonResponseObj.strErrors = "<ul><li>" + ex.Message + "</li></ul>";
            }
        }

        public bool SaveSTCInvoice(string resellerId, string jobs, int IsSTCInvoice = 1, string solarCompanyId = "", int UserTypeId = 0, int userId = 0, bool IsBackgroundRecProcess = false)
        {
            Common.Log("save STCInvoice start:" + jobs);
            string Days = string.Empty;
            DateTime? STCSettlementDateForInvoiceSTC = Common.GetSettlementDate(1, ref Days);

            int ResellerId = 0;

            //if ((UserTypeId == 1 || UserTypeId == 3) || (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 3))
            //{
            ResellerId = Convert.ToInt32(resellerId);
            //}
            //else
            //{
            //    ResellerId = ProjectSession.ResellerId;
            //}

            string[] sID = jobs.Split(',');

            string stcjobids = string.Empty;
            if (sID != null && sID.Length > 0)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("STCJobDetailsID", typeof(int));
                dt.Columns.Add("STCInvoiceNumber", typeof(string));
                dt.Columns.Add("STCInvoiceFilePath", typeof(string));

                foreach (string str in sID)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        DataRow dr = dt.NewRow();
                        dr["STCJobDetailsID"] = str.Split('_')[0];
                        dr["STCInvoiceNumber"] = string.Empty;
                        dr["STCInvoiceFilePath"] = string.Empty;
                        dt.Rows.Add(dr);
                        stcjobids = stcjobids + str.Split('_')[0] + ",";

                    }
                }
                stcjobids = stcjobids.Remove(stcjobids.Length - 1);
                Common.Log("enter into generateSTCInvoice");
                if (dt.Rows.Count > 0)
                {
                    Common.Log(DateTime.Now.ToString() + "datatable row count:" + dt.Rows.Count);
                    //DataSet dsSTCInvoice = _stcInvoiceServiceBAL.GenerateSTCInvoiceForSelectedJobs(UserTypeId == 0 ? ProjectSession.LoggedInUserId : userId, UserTypeId == 0 ? ProjectSession.UserTypeId : UserTypeId, ResellerId, IsSTCInvoice, dt, STCSettlementDateForInvoiceSTC);
                    DataSet dsSTCInvoice = _stcInvoiceServiceBAL.GenerateSTCInvoiceForSelectedJobs(userId, UserTypeId, ResellerId, IsSTCInvoice, dt, STCSettlementDateForInvoiceSTC);
                    DataTable dtSTCInvoice = dsSTCInvoice != null ? dsSTCInvoice.Tables[0] : new DataTable();
                    List<DataRow> stcJobIds = dtSTCInvoice.AsEnumerable().ToList();
                    if (dtSTCInvoice != null && dtSTCInvoice.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtSTCInvoice.Rows.Count; i++)
                        {
                            Common.Log("start createStcReport:" + Convert.ToString(dtSTCInvoice.Rows[i]["STCInvoiceNumber"] + " " + Convert.ToInt32(dtSTCInvoice.Rows[i]["STCJobDetailsID"])));
                            //_generateStcReportBAL.CreateStcReportNew("FormbotSTCReport", "Pdf", Convert.ToInt32(dtSTCInvoice.Rows[i]["STCJobDetailsID"]), Convert.ToString(dtSTCInvoice.Rows[i]["STCInvoiceNumber"]), solarCompanyId, "4", UserTypeId == 0 ? ProjectSession.LoggedInUserId : userId, UserTypeId == 0 ? ProjectSession.ResellerId : ResellerId, false, IsBackgroundRecProcess: IsBackgroundRecProcess);
                            _generateStcReportBAL.CreateStcReportNew("FormbotSTCReport", "Pdf", Convert.ToInt32(dtSTCInvoice.Rows[i]["STCJobDetailsID"]), Convert.ToString(dtSTCInvoice.Rows[i]["STCInvoiceNumber"]), solarCompanyId, "4", userId, ResellerId, false, IsBackgroundRecProcess: IsBackgroundRecProcess);
                            Common.Log("end createStcReport:" + Convert.ToString(dtSTCInvoice.Rows[i]["STCInvoiceNumber"] + " " + Convert.ToInt32(dtSTCInvoice.Rows[i]["STCJobDetailsID"])));
                        }
                        Common.Log("Start GetInvoiceCount: " + stcjobids);
                        DataTable countInvoice = _createJob.GetInvoiceCount(stcjobids);
                        if (countInvoice.Rows.Count > 0)
                        {
                            for (int j = 0; j < countInvoice.Rows.Count; j++)
                            {
                                SortedList<string, string> data = new SortedList<string, string>();
                                string IsInvoiced = countInvoice.Rows[j]["IsInvoiced"].ToString();
                                string IsCreditNote = countInvoice.Rows[j]["IsCreditNote"].ToString();
                                string STCInvoiceCount = countInvoice.Rows[j]["STCInvoiceCount"].ToString();
                                string IsPartialValidForSTCInvoice = countInvoice.Rows[j]["IsPartialValidForSTCInvoice"].ToString();
                                data.Add("IsInvoiced", IsInvoiced);
                                data.Add("IsCreditNote", IsCreditNote);
                                data.Add("STCInvoiceCount", STCInvoiceCount);
                                data.Add("IsPartialValidForSTCInvoice", IsPartialValidForSTCInvoice);
                                SetCacheDataForStcJobIdFromService(null, Convert.ToInt32(countInvoice.Rows[j]["jobid"]), data);
                                //CommonBAL.SetCacheDataForSTCSubmission(null, Convert.ToInt32(countInvoice.Rows[j]["jobid"]), data);
                            }

                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public bool SaveSAASInvoice(string resellerId, string jobs, int IsSTCInvoice = 1, string solarCompanyId = "", int UserTypeId = 0, int userId = 0, bool IsBackgroundRecProcess = false)
        {
            Common.Log("save SAASInvoice start:" + jobs);
            string Days = string.Empty;
            DateTime? STCSettlementDateForInvoiceSTC = Common.GetSettlementDate(1, ref Days);

            int ResellerId = 0;
            ResellerId = Convert.ToInt32(resellerId);

            string[] sID = jobs.Split(',');

            string stcjobids = string.Empty;
            if (sID != null && sID.Length > 0)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("SAASInvoiceId", typeof(Int32));
                dt.Columns.Add("SAASInvoiceNumber", typeof(String));
                dt.Columns.Add("CreatedDate", typeof(DateTime));
                dt.Columns.Add("CreatedBy", typeof(Int32));
                dt.Columns.Add("InvoiceDueDate", typeof(DateTime));
                dt.Columns.Add("SettlementTerm", typeof(string));
                dt.Columns.Add("TotalAmount", typeof(decimal));
                dt.Columns.Add("PaidAmount", typeof(decimal));
                dt.Columns.Add("Status", typeof(string));
                dt.Columns.Add("IsSent", typeof(bool));
                dt.Columns.Add("XeroInvoiceId", typeof(string));
                dt.Columns.Add("IsDeleted", typeof(bool));
                dt.Columns.Add("JobId", typeof(Int32));
                dt.Columns.Add("STCJobDetailId", typeof(Int32));
                dt.Columns.Add("STCAmount", typeof(decimal));

                foreach (string str in sID)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        DataRow dr = dt.NewRow();
                        dr["SAASInvoiceId"] = 0;
                        dr["SAASInvoiceNumber"] = "";
                        dr["CreatedDate"] = DateTime.Now;
                        dr["CreatedBy"] = resellerId;
                        dr["InvoiceDueDate"] = STCSettlementDateForInvoiceSTC;
                        dr["SettlementTerm"] = "";
                        dr["TotalAmount"] = 0;
                        dr["PaidAmount"] = 0;
                        dr["Status"] = "Outstanding";
                        dr["IsSent"] = false;
                        dr["XeroInvoiceId"] = "";
                        dr["IsDeleted"] = false;
                        dr["STCJobDetailsID"] = str.Split('_')[0];
                        dr["JobId"] = str.Split('_')[1];
                        dr["STCAmount"] = 0;
                        dt.Rows.Add(dr);
                        stcjobids = stcjobids + str.Split('_')[0] + ",";

                    }
                }
                stcjobids = stcjobids.Remove(stcjobids.Length - 1);
                Common.Log("enter into generateSSAASInvoice");
                if (dt.Rows.Count > 0)
                {
                    Common.Log(DateTime.Now.ToString() + "datatable row count:" + dt.Rows.Count);
                    DataSet dsSTCInvoice = _saasInvoiceServiceBAL.ImportSAASCSV(dt, Convert.ToInt32(resellerId));
                    DataTable dtSTCInvoice = dsSTCInvoice != null ? dsSTCInvoice.Tables[0] : new DataTable();
                    List<DataRow> stcJobIds = dtSTCInvoice.AsEnumerable().ToList();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Generate CSV File For PVD Jobs
        /// </summary>
        /// <param name="JobID"></param>
        /// <param name="FilePath"></param>
        /// <param name="dtCSV_JobID"></param>
        public void GetBulkUploadCSV_PVD(string JobID, string FilePath, ref DataSet dtCSV_JobID, ref DataTable dtSPVXmlPath, bool IsFileCreation = false)
        {
            //string FilePath=var LogoP = Path.Combine(ProjectSession.ProofDocuments + "UserDocuments" + "\\" + SettingUserId, Logo);
            DataSet ds = _createJob.GetBulkUploadForJob(JobID);
            DataTable dt = new DataTable();
            if (ds != null && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
                // Assign datatable into ref datatable variable (This ref datatable used at a time of  REC Regsitry call)
                dtCSV_JobID = ds;
                dtSPVXmlPath = ds.Tables[1];
            }

            //Installation property name, boat name or chassis number
            if (IsFileCreation)
            {
                StringBuilder csv = new StringBuilder();
                csv.Append(@"Type of system,Installation date,System/panel brand,System/panel model,Inverter manufacturer,Inverter series,Inverter model number,Type of connection to the electricity grid,System mounting type,Is a site specific audit report available?,Are you installing a complete unit (adding capacity to an existing system is not considered a complete unit)?,If this system is additional capacity to an existing system please provide detailed information on the position of the new panels and inverter (if applicable). System upgrades without a note explaining new panel locations will be failed by the Clean Energy Regulator,For what period would you like to create RECs,What is the rated power output (in kW) of your small generation unit,Number of panels,Do you wish to use the default resource availability figure?,What is your resource availability (hours per annum) for your system?,Equipment model serial number(s),Are you creating certificates for a system that has previously been failed by the Clean Energy Regulator?,Accreditation code,Explanatory notes for re-creating certificates previously failed,Reference,Certificate tag,CEC accreditation statement,""Local, State and Territory government siting approvals"",Electrical safety documentation,Australian/New Zealand standards statement,Not grid-connected statement,Installation property type,Single or multi-story,""Installation property name, boat name or chassis number"",Installation unit type,Installation unit number,Installation street number,Installation street name,Installation street type,Installation town/Suburb,Installation state,Installation postcode,Installation latitude,Installation longitude,Is there more than one SGU at this address?,If the address entered above does not adequately describe the location of the system please provide further detailed information for the Clean Energy Regulator to locate the system,Additional system information,Owner type,Owner organisation name,Owner first name,Owner surname,Owner phone,Owner fax,Owner mobile,Owner email,Owner address type,Owner postal delivery type,Owner postal delivery number,Owner unit type,Owner unit number,Owner street number,Owner street name,Owner street type,Owner town/suburb,Owner state,Owner postcode,Owner country,Installer first name,Installer surname,CEC accredited installer number,Installer phone,Installer fax,Installer mobile,Installer email,Installer address type,Installer postal delivery type,Installer postal delivery number,Installer unit type,Installer unit number,Installer street number,Installer street name,Installer street type,Installer town/suburb,Installer state,Installer postcode,Electrician first name,Electrician surname,Licensed electrician number,Electrician phone,Electrician fax,Electrician mobile,Electrician email,Electrician address type,Electrician postal delivery type,Electrician postal delivery number,Electrician unit type,Electrician unit number,Electrician street number,Electrician street name,Electrician street type,Electrician town/suburb,Electrician state,Electrician postcode,Designer first name,Designer surname,CEC accredited designer number,Designer phone,Designer fax,Designer mobile,Designer email,Designer address type,Designer postal delivery type,Designer postal delivery number,Designer unit type,Designer unit number,Designer street number,Designer street name,Designer street type,Designer town/suburb,Designer state,Designer postcode,Retailer Name,Retailer ABN,National metering identifier (NMI),Battery storage manufacturer,Battery storage model,Is the battery system part of an aggregated control?,Has the installer changed default manufacturer setting of the battery storage system?,Signed data package,Documents Zip File,Installation Type,Installation Type Additional Information");
                csv.Append("\r\n");
                foreach (DataRow row in dt.Rows)
                {
                    // Remove JobID from DATATABLE To create CSV
                    bool IsFirstCol = true;
                    foreach (string item in row.ItemArray)
                    {
                        if (!IsFirstCol)
                            csv.Append(FormBot.Helper.Helper.Common.StringToCSVCell(item.Contains(";") ? System.Web.HttpUtility.HtmlDecode(item) : item) + ",");
                        IsFirstCol = false;
                    }
                    csv.Remove(csv.ToString().Length - 1, 1);
                    csv.Append("\r\n");
                }
                StreamWriter sw = new StreamWriter(FilePath, false);
                sw.Write(csv);
                sw.Close();
            }
        }

        /// <summary>
        /// Get Bulk UploadSWHCSV
        /// </summary>
        /// <param name="JobID"></param>
        /// <param name="FilePath"></param>
        /// <param name="dtCSV_JobID"></param>
        public void GetBulkUploadSWHCSV(string JobID, string FilePath, ref DataSet dtCSV_JobID, bool IsFileCreation = false)
        {
            DataSet swhDataset = _createJob.GetSWHBulkUploadForJob(JobID);
            DataTable swhDatatable = new DataTable();
            if (swhDataset != null && swhDataset.Tables.Count > 0)
            {
                swhDatatable = swhDataset.Tables[0];
                // Assign datatable into ref datatable variable (This ref datatable used at a time of  REC Regsitry call)
                dtCSV_JobID = swhDataset;
            }
            if (IsFileCreation)
            {
                StringBuilder csv = new StringBuilder();
                //csv.Append("System brand,System model,Tank serial number(s),Number of panels,Installation date,Installation type,Is the volumetric capacity of this installation greater than 700L,Statutory declarations sent,Is your water heater second hand,Reference,Certificate tag,Installation property type,Installation single or multi-story,Installation property name or boat name or chassis number,Installation unit type,Installation unit number,Installation street number,Installation street name,Installation street type,Installation town/suburb,Installation state,Installation postcode,Installation latitude,Installation longitude,Is there more than one SWH/ASHP at this address,Additional system information,Additional address information,Creating certificates for previously failed SWH,Failed accreditation code,Explanatory notes for failed accreditation code,Installation special address,Owner type,Owner organisation name,Owner first name,Owner surname,Owner phone,Owner fax,Owner mobile,Owner email,Owner address type,Owner postal delivery type,Owner postal delivery number,Owner unit type,Owner unit number,Owner street number,Owner street name,Owner street type,Owner town/suburb,Owner state,Owner postcode,Owner country,Installer first name,Installer surname,Installer phone,Installer fax,Installer mobile,Installer email,Installer address type,Installer postal delivery type,Installer postal delivery number,Installer unit type,Installer unit number,Installer street number,Installer street name,Installer street type,Installer town/suburb,Installer state,Installer postcode");
                //csv.Append("System brand,System model,Tank serial number(s),Number of panels,Installation date,Installation type,Is the volumetric capacity of this installation greater than 700L,Statutory declarations sent,Is your water heater second hand,Reference,Certificate tag,Installation property type,Installation single or multi-story,Installation property name or boat name or chassis number,Installation unit type,Installation unit number,Installation street number,Installation street name,Installation street type,Installation town/suburb,Installation state,Installation postcode,Installation latitude,Installation longitude,Is there more than one SWH/ASHP at this address,Additional system information,Creating certificates for previously failed SWH,Failed accreditation code,Explanatory notes for failed accreditation code,Installation special address,Owner type,Owner organisation name,Owner first name,Owner surname,Owner phone,Owner fax,Owner mobile,Owner email,Owner address type,Owner postal delivery type,Owner postal delivery number,Owner unit type,Owner unit number,Owner street number,Owner street name,Owner street type,Owner town/suburb,Owner state,Owner postcode,Owner country,Installer first name,Installer surname,Installer phone,Installer fax,Installer mobile,Installer email,Installer address type,Installer postal delivery type,Installer postal delivery number,Installer unit type,Installer unit number,Installer street number,Installer street name,Installer street type,Installer town/suburb,Installer state,Installer postcode,Documents Zip File");
                csv.Append("System brand,System model,Tank serial number(s),Number of panels,Installation date,Installation type,Is the volumetric capacity of this installation greater than 700L,Statutory declarations sent,Is your water heater second hand,Reference,Certificate tag,Installation property type,Installation single or multi-story,Installation property name or boat name or chassis number,Installation unit type,Installation unit number,Installation street number,Installation street name,Installation street type,Installation town/suburb,Installation state,Installation postcode,Installation latitude,Installation longitude,Is there more than one SWH/ASHP at this address,Additional system information,Creating certificates for previously failed SWH,Failed accreditation code,Explanatory notes for failed accreditation code,Installation special address,Owner type,Owner organisation name,Owner first name,Owner surname,Owner phone,Owner fax,Owner mobile,Owner email,Owner address type,Owner postal delivery type,Owner postal delivery number,Owner unit type,Owner unit number,Owner street number,Owner street name,Owner street type,Owner town/suburb,Owner state,Owner postcode,Owner country,Installer first name,Installer surname,Installer phone,Installer fax,Installer mobile,Installer email,Installer address type,Installer postal delivery type,Installer postal delivery number,Installer unit type,Installer unit number,Installer street number,Installer street name,Installer street type,Installer town/suburb,Installer state,Installer postcode");
                csv.Append("\r\n");
                foreach (DataRow row in swhDatatable.Rows)
                {
                    bool IsFirstCol = true;
                    foreach (string item in row.ItemArray)
                    {
                        if (!IsFirstCol)
                            csv.Append(FormBot.Helper.Helper.Common.StringToCSVCell(item.Contains(";") ? System.Web.HttpUtility.HtmlDecode(item) : item) + ",");
                        IsFirstCol = false;
                    }

                    csv.Remove(csv.ToString().Length - 1, 1);
                    csv.Append("\r\n");
                }

                // Save File into Specific file path
                StreamWriter sw = new StreamWriter(FilePath, false);
                sw.Write(csv);
                sw.Close();
            }
        }

        public List<T> SplitList<T>(List<T> me, int size = 50)
        {
            var list = new List<List<T>>();
            var lst = new List<T>();

            var lstWithBatchId = new List<T>();
            var lstWithoutBatchId = new List<T>();


            foreach (var item in me)
            {
                if (item.ToString().Contains('_'))
                    lstWithBatchId.Add(item);
                else
                    lstWithoutBatchId.Add(item);

            }
            var DistinctItems = lstWithBatchId.GroupBy(x => x.ToString().Split('_')[1]).Select(y => y.First().ToString().Split('_')[1]).Distinct();

            foreach (var distinctItem in DistinctItems)
            {
                var lstItem = string.Join(",", lstWithBatchId.Where(x => x.ToString().Contains(distinctItem)));
                lstItem = lstItem.Replace("_" + distinctItem, "");
                lst.Add((T)Convert.ChangeType(lstItem, typeof(T)));
            }

            for (int i = 0; i < lstWithoutBatchId.Count; i += size)
            {
                list.Add(lstWithoutBatchId.GetRange(i, Math.Min(size, lstWithoutBatchId.Count - i)));
                lst.Add((T)Convert.ChangeType(string.Join(",", lstWithoutBatchId.GetRange(i, Math.Min(size, lstWithoutBatchId.Count - i))), typeof(T)));
            }

            return lst;
        }

        public DataTable ListToDataTable<T>(List<T> lst)
        {
            DataTable table = new DataTable();
            table.Columns.Add("SrNo", typeof(int));
            table.Columns.Add("JobId", typeof(string));
            int i = 1;
            foreach (var item in lst)
            {
                DataRow row = table.NewRow();
                row["JobId"] = item.ToString();
                row["SrNo"] = i;
                table.Rows.Add(row);
                i++;
            }
            return table;
        }

        /// <summary>
        /// Gets the record data.
        /// </summary>
        public void GetRECData()
        {
            try
            {
                Common.Log("GetRECData start");
                DateTime limitDate = Convert.ToDateTime(ConfigurationManager.AppSettings["RECDataLimitDate"].ToString());
                DateTime processDate = Convert.ToDateTime(DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"));
                //DateTime processDate = Convert.ToDateTime("2016-10-14");
                DateTime latestDate = processDate;

                string jsonRECData = string.Empty;

                while ((processDate >= limitDate))
                {
                    if (RECDataExistForProcessDate(processDate))
                    {
                        processDate = processDate.AddDays(1);
                        break;
                    }
                    else
                    {
                        processDate = processDate.AddDays(-1);
                        continue;
                    }
                }
                //string jsonRECData = "{\"status\":\"Success\",\"result\":[{\"actionType\":\"STC registered\",\"completedTime\":\"2016-08-31T21:46:57.559Z\",\"certificateRanges\":[{\"certificateType\":\"STC\",\"registeredPersonNumber\":714,\"accreditationCode\":\"OnAppTest\",\"generationYear\":2016,\"generationState\":\"QLD\",\"startSerialNumber\":1,\"endSerialNumber\":31,\"fuelSource\":\"S.G.U. - solar (deemed)\",\"ownerAccount\":\"SolarpowerRex Pty Ltd\",\"ownerAccountId\":2114,\"status\":\"Registered\"}]}]}";
                //while ((processDate >= limitDate) && !RECDataExistForProcessDate(processDate))
                while (processDate <= latestDate)
                {
                    Common.Log("fetch data from rec-registry.gov.au start for " + processDate.ToString("yyyy-MM-dd"));
                    string url = ConfigurationManager.AppSettings["RECDataURL"].ToString() + processDate.ToString("yyyy-MM-dd");
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.AutomaticDecompression = DecompressionMethods.GZip;
                    request.Timeout = 1800000;
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        jsonRECData = reader.ReadToEnd();
                    }

                    Common.Log("fetch data from rec-registry.gov.au " + processDate.ToString("yyyy-MM-dd") + " Completed");
                    Common.Log("Prepare DataTable Start");
                    var rootObject = JsonConvert.DeserializeObject<RECData>(jsonRECData);
                    if (rootObject.status == "Success")
                    {
                        DataTable dtRECData = createTable();
                        foreach (var result in rootObject.result)
                        {
                            foreach (var cert in result.certificateRanges)
                            {
                                DataRow dr = dtRECData.NewRow();
                                dr["actionType"] = Convert.ToString(result.actionType);
                                dr["completedTime"] = convertedDate(Convert.ToString(result.completedTime));
                                dr["registeredPersonNumber"] = Convert.ToString(cert.registeredPersonNumber);
                                dr["accreditationCode"] = Convert.ToString(cert.accreditationCode);
                                dr["generationYear"] = Convert.ToString(cert.generationYear);
                                dr["generationState"] = Convert.ToString(cert.generationState);
                                dr["startSerialNumber"] = Convert.ToString(cert.startSerialNumber);
                                dr["endSerialNumber"] = Convert.ToString(cert.endSerialNumber);
                                dr["certificateType"] = Convert.ToString(cert.certificateType);
                                dr["fuelSource"] = Convert.ToString(cert.fuelSource);
                                dr["ownerAccount"] = Convert.ToString(cert.ownerAccount);
                                dr["ownerAccountId"] = Convert.ToString(cert.ownerAccountId);
                                dr["status"] = Convert.ToString(cert.status);
                                dtRECData.Rows.Add(dr);
                            }

                        }

                        Common.Log("Prepare DataTable Completed");
                        Common.Log("Insert REC Data call for: " + processDate.ToString("yyyy-MM-dd"));
                        try
                        {
                            DataSet ds = objCreateJobBAL.InsertRECData(dtRECData, processDate);
                            Common.Log("Insert REC Data " + processDate.ToString("yyyy-MM-dd") + " Completed");
                            //if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                            //{
                            //	CreateSTCInvoicePDFForRECData(ds.Tables[0]);
                            //}

                            // Find Reason for REC Failure Job and Insert into Reason and JobReason Table
                            //if (ds != null && ds.Tables.Count > 0 && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                            {
                                //DataTable dtRECFailureJobDetails = ds.Tables[1];
                                DataTable dtRECFailureJobDetails = ds.Tables[0];
                                DataTable dtReason = new DataTable();
                                dtReason.Clear();
                                dtReason.Columns.Add("jobId");
                                dtReason.Columns.Add("reason");
                                dtReason.Columns.Add("completedTime");
                                dtReason.Columns.Add("LastAuditedDate");
                                for (int i = 0; i < dtRECFailureJobDetails.Rows.Count; i++)
                                {
                                    try
                                    {
                                        Common.Log("Failure Reason Count:" + dtRECFailureJobDetails.Rows.Count.ToString());
                                        Common.Log("Find Reason for REC Failure For PVDCode: " + Convert.ToString(dtRECFailureJobDetails.Rows[i]["STCPVDCode"]));
                                        //Find Reason for REC Failure Code here
                                        //......
                                        //......
                                        FormBot.Entity.RECAccount objAdminUser = new RECAccount();
                                        FormBot.Entity.RECAccount objResellerUser = new RECAccount();
                                        List<string> lstFailureReasons = new List<string>();
                                        string STCPVDCode = Convert.ToString(dtRECFailureJobDetails.Rows[i]["STCPVDCode"]);
                                        objAdminUser.CERLoginId = Convert.ToString(dtRECFailureJobDetails.Rows[i]["RecUserName"]);
                                        objAdminUser.CERPassword = Convert.ToString(dtRECFailureJobDetails.Rows[i]["RecPassword"]);
                                        objAdminUser.RECAccName = Convert.ToString(dtRECFailureJobDetails.Rows[i]["RECAccName"]);
                                        objResellerUser.CERLoginId = Convert.ToString(dtRECFailureJobDetails.Rows[i]["RRecUserName"]);
                                        objResellerUser.CERPassword = Convert.ToString(dtRECFailureJobDetails.Rows[i]["RRecPassword"]);
                                        objResellerUser.RECAccName = Convert.ToString(dtRECFailureJobDetails.Rows[i]["RRECAccName"]);
                                        int JobID = Convert.ToInt32(dtRECFailureJobDetails.Rows[i]["JobID"]);
                                        DateTime CompletedTime = Convert.ToDateTime(dtRECFailureJobDetails.Rows[i]["CompletedTime"]);
                                        DateTime? LastAuditedDate = null;
                                        int jobType = Convert.ToInt32(dtRECFailureJobDetails.Rows[i]["JobType"]);

                                        if (!string.IsNullOrEmpty(STCPVDCode) && JobID > 0 && jobType > 0)
                                        {
                                            // Get Failure reasons from REC using PVDCode 
                                            RECRegistry.AuthenticateUser_UploadFileForREC(ref lstFailureReasons, ref LastAuditedDate, objAdminUser, STCPVDCode, jobType, objResellerUser);
                                            Common.Log("Reason for JobID:" + JobID + " Failure reason count: " + lstFailureReasons.Count);
                                            //Insert into Reason and JobReason Table
                                            foreach (var reason in lstFailureReasons)
                                            {
                                                Common.Log("last audited date for jobid:"+JobID+",LastAUditedDate:"+ LastAuditedDate?.ToString("yyyy-MM-dd")+"CompletedTime+"+CompletedTime+"CompletedTime.TostringFormateForDB--"+CompletedTime.ToString("yyyy-MM-dd"));
                                                dtReason.Rows.Add(JobID, reason, CompletedTime, LastAuditedDate?.ToString("yyyy-MM-dd"));
                                                Common.Log("After add data in dtReason:jobid-"+JobID);
                                            }


                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        Common.Log(ex.Message.ToString());
                                    }

                                }
                                Common.Log("End data in dtReasonCount" );
                                if (dtReason != null && dtReason.Rows.Count > 0)
                                {
                                    Common.Log("Enter data in dtReasonCount:" + dtReason.Rows.Count);
                                    InsertRECFailureJobReason(dtReason);
                                    Common.Log("Insert REC Failure Reason Successfully");
                                }

                                Common.Log("REC Failure reasons inserted into db");
                            }
                            if (ds != null && ds.Tables.Count > 0 && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                            {
                                Common.Log("start creating invoice");
                                _jobRuleBAL.CreateSTCInvoicePDFForRECData(ds.Tables[1], true);
                            }
                            if (ds != null && ds.Tables.Count > 0 && ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                            {
                                Common.Log("start sending email on CER failed");
                                foreach (string id in Convert.ToString(ds.Tables[2].Rows[0]["STCJobDetailsId"]).Split(','))
                                {
                                    try
                                    {
                                        SetCacheOnSTCStatusChange(14, Convert.ToInt32(id));
                                    }
                                    catch (Exception ex)
                                    {
                                        Common.Log(ex.Message);
                                    }
                                }
                                _jobRuleBAL.SendMailOnCERFailed(Convert.ToString(ds.Tables[2].Rows[0]["STCJobDetailsId"]), true);
                                Common.Log("end sending email on CER failed for stcjobdetailsId: " + Convert.ToString(ds.Tables[2].Rows[0]["STCJobDetailsId"]));
                            }
                            if (ds != null && ds.Tables.Count > 0 && ds.Tables[3] != null && ds.Tables[3].Rows.Count > 0)
                            {
                                Common.Log(DateTime.Now+"---Enter inside GetRECData ds.tables[3]");
                                try
                                {
                                    List<int> stcJobDetailIds = ds.Tables[3].AsEnumerable().Select(dr => dr.Field<int>("STCJobDetailsId")).Distinct().ToList();
                                    //List<int> peakpayJobIds = ds.Tables[3].AsEnumerable().Where(r=>r.Field<int>("STCSettlementTerm")==12 || (r.Field<int>("STCSettlementTerm") == 10 && r.Field<int>("CustomSettlementTerm")==12)). Select(dr => dr.Field<int>("STCJobDetailsId")).Distinct().ToList();
                                    
                                    //string stcJobDetailIds = string.Empty;
                                    
                                    //List<int> peakpayJobIds = ds.Tables[3].AsEnumerable().Where(r=>r.Field<int>("STCSettlementTerm")==12 || (r.Field<int>("STCSettlementTerm") == 10 && r.Field<int>("CustomSettlementTerm")==12)). Select(dr => dr.Field<int>("STCJobDetailsId")).Distinct().ToList();
                                    
                                    string strstcjobdetailsid =stcJobDetailIds.Count>0? string.Join(",", stcJobDetailIds):string.Empty;

                                    Common.Log("stcJobdetailsId for update cache: " + strstcjobdetailsid);
                                    for (int i = 0; i < stcJobDetailIds.Count; i++)
                                    {
                                        
                                        //stcJobDetailIds = stcJobDetailIds + Convert.ToString(ds.Tables[3].Rows[i]["STCJobDetailsId"]);
                                        //SetCacheOnSTCStatusChange(22, Convert.ToInt32(ds.Tables[3].Rows[i]["STCJobDetailsId"]));
                                        SetCacheOnSTCStatusChange(22, stcJobDetailIds[i]);
                                        //SetCacheDataForPeakPay(Convert.ToString(stcJobDetailIds[i]));

                                    }
                                    var peakpaySTCIds = ds.Tables[3].AsEnumerable().Where(r => r.Field<int>("STCSettlementTerm") == 12 || (r.Field<int>("STCSettlementTerm") == 10 && r.Field<int>("CustomSettlementTerm") == 12)).Select(dr => dr.Field<int>("STCJobDetailsId")).Distinct().ToList();
                                    if (peakpaySTCIds.Count>0)
                                        SetCacheDataForPeakPay(string.Join(",",peakpaySTCIds));
                                    

                                }
                                catch (Exception ex)
                                {
                                    Common.Log(ex.Message);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Common.Log(ex.Message.ToString());
                        }

                    }
                    processDate = processDate.AddDays(1);
                }

                Common.Log("GetRECData end");
            }
            catch (Exception ex)
            {
                Common.Log(ex.Message.ToString());
            }

        }
        public void getRecDataTest()
        {
            FormBot.Entity.RECAccount objAdminUser = new RECAccount();
            FormBot.Entity.RECAccount objResellerUser = new RECAccount();
            List<string> lstFailureReasons = new List<string>();
            string STCPVDCode = Convert.ToString("PVD4696175");
            objAdminUser.CERLoginId = Convert.ToString("sda");
            objAdminUser.CERPassword = Convert.ToString("ds");
            objAdminUser.RECAccName = Convert.ToString("LAMH53041");
            //objResellerUser.CERLoginId = Convert.ToString(dtRECFailureJobDetails.Rows[i]["RRecUserName"]);
            //objResellerUser.CERPassword = Convert.ToString(dtRECFailureJobDetails.Rows[i]["RRecPassword"]);
            //objResellerUser.RECAccName = Convert.ToString(dtRECFailureJobDetails.Rows[i]["RRECAccName"]);
            int JobID = Convert.ToInt32("457088");
            DataTable dtReason = new DataTable();
            dtReason.Clear();
            dtReason.Columns.Add("jobId");
            dtReason.Columns.Add("reason");
            dtReason.Columns.Add("completedTime");
            dtReason.Columns.Add("LastAuditedDate");
            DateTime? LastAuditedDate = null;
            int jobType = Convert.ToInt32(1);

            if (!string.IsNullOrEmpty(STCPVDCode) && JobID > 0 && jobType > 0)
            {
                // Get Failure reasons from REC using PVDCode 
                RECRegistry.AuthenticateUser_UploadFileForREC(ref lstFailureReasons, ref LastAuditedDate, objAdminUser, STCPVDCode, jobType, objResellerUser);
                Common.Log("Reason for JobID:" + JobID + " Failure reason count: " + lstFailureReasons.Count);
                //Insert into Reason and JobReason Table
                foreach (var reason in lstFailureReasons)
                {
                    Common.Log("converterd date:" + LastAuditedDate?.ToString("yyyy-MM-dd"));
                    dtReason.Rows.Add(JobID, reason, "", LastAuditedDate?.ToString("yyyy-MM-dd"));
                }

            }

        }
        public void SetCacheOnSTCStatusChange(int STCJobStageID, int STCJobDetailsId)
        {
            SortedList<string, string> data = new SortedList<string, string>();
            string stcStatus = Common.GetDescription((SystemEnums.STCJobStatus)STCJobStageID, "");
            string colorCode = Common.GetSubDescription((SystemEnums.STCJobStatus)STCJobStageID, "");
            data.Add("STCStatus", stcStatus);
            data.Add("ColorCode", colorCode);
            data.Add("STCStatusId", STCJobStageID.ToString());
            if (STCJobStageID == 22)
                data.Add("IsUrgentSubmission", "False");

            SetCacheDataForStcJobIdFromService(Convert.ToInt32(STCJobDetailsId), null, data);
            Common.Log(DateTime.Now.ToString() + " setcachedataForStcId on " + stcStatus + ": " + STCJobDetailsId);
        }

        /// <summary>
        /// Records the data exist for process date.
        /// </summary>
        /// <param name="processDate">The process date.</param>
        /// <returns>boolean object</returns>
        public bool RECDataExistForProcessDate(DateTime processDate)
        {
            return objCreateJobBAL.CheckRECDataExistForDate(processDate);
        }

        /// <summary>
        /// Creates the table.
        /// </summary>
        /// <returns>data table</returns>
        public DataTable createTable()
        {
            DataTable dtData = new DataTable();
            DataColumn dc = new DataColumn("actionType", typeof(string));
            dtData.Columns.Add(dc);
            dc = new DataColumn("completedTime", typeof(string));
            dtData.Columns.Add(dc);
            dc = new DataColumn("certificateType", typeof(string));
            dtData.Columns.Add(dc);
            dc = new DataColumn("registeredPersonNumber", typeof(string));
            dtData.Columns.Add(dc);
            dc = new DataColumn("accreditationCode", typeof(string));
            dtData.Columns.Add(dc);
            dc = new DataColumn("generationYear", typeof(string));
            dtData.Columns.Add(dc);
            dc = new DataColumn("generationState", typeof(string));
            dtData.Columns.Add(dc);
            dc = new DataColumn("startSerialNumber", typeof(string));
            dtData.Columns.Add(dc);
            dc = new DataColumn("endSerialNumber", typeof(string));
            dtData.Columns.Add(dc);
            dc = new DataColumn("fuelSource", typeof(string));
            dtData.Columns.Add(dc);
            dc = new DataColumn("ownerAccount", typeof(string));
            dtData.Columns.Add(dc);
            dc = new DataColumn("ownerAccountId", typeof(string));
            dtData.Columns.Add(dc);
            dc = new DataColumn("status", typeof(string));
            dtData.Columns.Add(dc);
            return dtData;
        }

        /// <summary>
        /// convert the date.
        /// </summary>
        /// <param name="strDate">The string date.</param>
        /// <returns>date time</returns>
        public DateTime convertedDate(string strDate)
        {
            //2016-07-31T14:31:33.498Z
            string[] formats = new string[] { "yyyy-MM-ddTHH:mm:ss.fffZ" };
            DateTime retValue;
            if (false == DateTime.TryParseExact(strDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out retValue))
            {
                retValue = DateTime.MinValue;
            }

            retValue = TimeZoneInfo.ConvertTime(retValue, TimeZoneInfo.Local, TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time"));
            return retValue;
        }

        //public void CreateSTCInvoicePDFForRECData(DataTable dt)
        //{
        //	try
        //	{
        //		DataTable dtUpdate = new DataTable();
        //		dtUpdate.Columns.Add("STCInvoiceNumber", typeof(string));
        //		dtUpdate.Columns.Add("STCInvoiceFilePath", typeof(string));

        //		foreach (DataRow dr in dt.Rows)
        //		{
        //			try
        //			{
        //				//string filepath = CreateStcReport("CreateStcReport", "Pdf", Convert.ToInt32(dr["STCJobDetailsID"]), Convert.ToString(dr["STCInvoiceNumber"]), Convert.ToString(dr["SolarCompanyId"]), Convert.ToInt32(dr["ResellerUserId"]), Convert.ToInt32(dr["ResellerID"]));
        //				string filepath = _generateStcReportBAL.CreateStcReport("CreateStcReport", "Pdf", Convert.ToInt32(dr["STCJobDetailsID"]), Convert.ToString(dr["STCInvoiceNumber"]), Convert.ToString(dr["SolarCompanyId"]), "2", Convert.ToInt32(dr["ResellerUserId"]), Convert.ToInt32(dr["ResellerID"]), true);
        //				DataRow drUpdate = dtUpdate.NewRow();
        //				drUpdate["STCInvoiceNumber"] = Convert.ToString(dr["STCInvoiceNumber"]);
        //				drUpdate["STCInvoiceFilePath"] = filepath;
        //				dtUpdate.Rows.Add(drUpdate);
        //				Log("STCInvoice file is generate successfully for " + Convert.ToString(dr["STCInvoiceNumber"]));
        //			}
        //			catch (Exception ex)
        //			{
        //				Log("An error has occured while generating STCInvioce file for " + Convert.ToString(dr["STCInvoiceNumber"]) + ": " + ex.Message);
        //			}

        //		}

        //		_stcInvoiceServiceBAL.UpdateRecGeneratedInvoiceFilePath(dtUpdate);
        //		Log("Invoice file paths are updated successfully in STCInvoice.");
        //	}
        //	catch (Exception ex)
        //	{
        //		Log("An error has occured while generating STCInvoice files: " + ex.Message);
        //	}

        //}

        //public String CreateStcReport(string Filename, string ExportType, int STCJobDetailsID, string InvoiceNo, string solarCompanyId, int ResellerUserId, int ResellerID)
        //{
        //    Microsoft.Reporting.WebForms.Warning[] warnings;
        //    string[] streamIds;
        //    string mimeType = string.Empty;
        //    string encoding = string.Empty;
        //    string extension = string.Empty;
        //    ReportViewer viewer = new ReportViewer();
        //    XmlDocument oXD = new XmlDocument();
        //    oXD.Load(Convert.ToString(ConfigurationManager.AppSettings["RDLCReportFormatFile"]));
        //    STCInvoice stcinvoice = new STCInvoice();
        //    DataSet ds = new DataSet();
        //    string RefNumber = string.Empty;
        //    string CompanyABN = string.Empty;
        //    string CompanyABNReseller = string.Empty;
        //    string InoviceDate = string.Empty;
        //    string InvoiceNumber = string.Empty;
        //    string AmountDue = string.Empty;
        //    string Total = string.Empty;
        //    string DueDate = string.Empty;
        //    string FromAddressLine1 = string.Empty;
        //    string FromAddressLine2 = string.Empty;
        //    string FromAddressLine3 = string.Empty;
        //    string ToAddressLine1 = string.Empty;
        //    string ToAddressLine2 = string.Empty;
        //    string ToAddressLine3 = string.Empty;
        //    string LogoPath = string.Empty;
        //    string InvoiceFooter = string.Empty;
        //    string JobDescription = string.Empty;
        //    string JobDate = string.Empty;
        //    string JobAddress = string.Empty;
        //    string JobTitle = string.Empty;
        //    string Logo = string.Empty;
        //    string ItemCode = string.Empty;
        //    string jobid = string.Empty;
        //    string IsStcInvoice = string.Empty;
        //    string ToName = string.Empty;
        //    string FromName = string.Empty;
        //    string FromCompanyName = string.Empty;
        //    string ToCompanyName = string.Empty;
        //    bool IsJobDescription, IsJobAddress, IsJobDate, IsTitle, IsName, IsTaxInclusive;
        //    decimal? TaxRate = 0;
        //    int SettingUserId = 0;
        //    string path = string.Empty;

        //    string AccountName = string.Empty;
        //    string BSB = string.Empty;
        //    string AccountNumber = string.Empty;

        //    Settings settings = new Settings();
        //    settings = GetSettingsData(solarCompanyId, ResellerUserId, 2, ResellerID);
        //    IsJobDescription = settings.IsJobDescription;
        //    IsJobAddress = settings.IsJobAddress;
        //    IsJobDate = settings.IsJobDate;
        //    IsTitle = settings.IsTitle;
        //    IsName = settings.IsName;
        //    IsTaxInclusive = settings.IsTaxInclusive;
        //    TaxRate = settings.TaxRate;
        //    //Logo = settings.Logo;
        //    //InvoiceFooter = settings.InvoiceFooter;
        //    //SettingUserId = settings.UserId;

        //    //if (Logo != "" && Logo != null)
        //    //{
        //    //    string LogoP = Convert.ToString(ConfigurationManager.AppSettings["ProofUploadFolder"]) + "\\UserDocuments" + "\\" + SettingUserId + "\\" + Logo;
        //    //    LogoPath = new Uri(LogoP).AbsoluteUri;
        //    //}
        //    //else
        //    //{
        //    //    LogoPath = "";
        //    //}

        //    ds = _stcInvoiceServiceBAL.GetStcInvoice(STCJobDetailsID, IsJobAddress, IsJobDate, IsJobDescription, IsTitle, IsName, DateTime.Now, InvoiceNo);
        //    DataTable dt = new DataTable();
        //    try
        //    {
        //        if (ds != null && ds.Tables.Count > 0)
        //        {
        //            if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
        //            {
        //                RefNumber = !string.IsNullOrEmpty(ds.Tables[0].Rows[0]["RefNumber"].ToString()) ? ds.Tables[0].Rows[0]["RefNumber"].ToString() : string.Empty;
        //            }

        //            if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
        //            {
        //                CompanyABN = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["CompanyABN"].ToString()) ? ds.Tables[1].Rows[0]["CompanyABN"].ToString() : string.Empty;

        //                CompanyABN = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["CompanyABN"].ToString()) ? ds.Tables[1].Rows[0]["CompanyABN"].ToString() : string.Empty;
        //                CompanyABNReseller = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["CompanyABNReseller"].ToString()) ? ds.Tables[1].Rows[0]["CompanyABNReseller"].ToString() : string.Empty;
        //                Logo = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["ResellerLogo"].ToString()) ? ds.Tables[1].Rows[0]["ResellerLogo"].ToString() : string.Empty;
        //                SettingUserId = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["SettingUserId"].ToString()) ? Convert.ToInt32(ds.Tables[1].Rows[0]["SettingUserId"]) : 0;
        //                InvoiceFooter = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["InvoiceFooter"].ToString()) ? ds.Tables[1].Rows[0]["InvoiceFooter"].ToString() : string.Empty;

        //                AccountName = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["AccountName"].ToString()) ? ds.Tables[1].Rows[0]["AccountName"].ToString() : string.Empty;
        //                BSB = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["BSB"].ToString()) ? Convert.ToString(ds.Tables[1].Rows[0]["BSB"]) : string.Empty;
        //                AccountNumber = !string.IsNullOrEmpty(ds.Tables[1].Rows[0]["AccountNumber"].ToString()) ? ds.Tables[1].Rows[0]["AccountNumber"].ToString() : string.Empty;

        //                if (Logo != "" && Logo != null)
        //                {
        //                    var LogoP = Path.Combine(Convert.ToString(ConfigurationManager.AppSettings["ProofUploadFolder"]) + "\\UserDocuments" + "\\" + SettingUserId, Logo);
        //                    LogoPath = new Uri(LogoP).AbsoluteUri;
        //                }
        //                else
        //                {
        //                    LogoPath = "";
        //                }
        //            }

        //            if (ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
        //            {
        //                InoviceDate = !string.IsNullOrEmpty(ds.Tables[2].Rows[0]["InoviceDate"].ToString()) ? Convert.ToDateTime(ds.Tables[2].Rows[0]["InoviceDate"]).ToString("dd MMMM yyyy") : string.Empty;
        //                InvoiceNumber = InvoiceNo;
        //                jobid = !string.IsNullOrEmpty(ds.Tables[2].Rows[0]["jobid"].ToString()) ? ds.Tables[2].Rows[0]["jobid"].ToString() : string.Empty;
        //                DueDate = !string.IsNullOrEmpty(ds.Tables[2].Rows[0]["DueDate"].ToString()) ? Convert.ToDateTime(ds.Tables[2].Rows[0]["DueDate"]).ToString("dd MMMM yyyy") : string.Empty;
        //            }

        //            if (ds.Tables[3] != null && ds.Tables[3].Rows.Count > 0)
        //            {
        //                dt = ds.Tables[3];
        //            }

        //            if (ds.Tables[4] != null && ds.Tables[4].Rows.Count > 0)
        //            {
        //                ToAddressLine1 = !string.IsNullOrEmpty(ds.Tables[4].Rows[0]["ToAddressLine1"].ToString()) ? ds.Tables[4].Rows[0]["ToAddressLine1"].ToString() : string.Empty;
        //                ToAddressLine2 = !string.IsNullOrEmpty(ds.Tables[4].Rows[0]["ToAddressLine2"].ToString()) ? ds.Tables[4].Rows[0]["ToAddressLine2"].ToString() : string.Empty;
        //                ToAddressLine3 = !string.IsNullOrEmpty(ds.Tables[4].Rows[0]["ToAddressLine3"].ToString()) ? ds.Tables[4].Rows[0]["ToAddressLine3"].ToString() : string.Empty;
        //                ToCompanyName = !string.IsNullOrEmpty(ds.Tables[4].Rows[0]["ToCompanyName"].ToString()) ? ds.Tables[4].Rows[0]["ToCompanyName"].ToString() : string.Empty;
        //            }

        //            if (ds.Tables[5] != null && ds.Tables[5].Rows.Count > 0)
        //            {
        //                FromAddressLine1 = (ds.Tables[5].Rows[0]["FormAddressline1"].ToString() == "") ? "" : ds.Tables[5].Rows[0]["FormAddressline1"].ToString();
        //                FromAddressLine2 = (ds.Tables[5].Rows[0]["FormAddressline2"].ToString() == "") ? "" : ds.Tables[5].Rows[0]["FormAddressline2"].ToString();
        //                FromAddressLine3 = (ds.Tables[5].Rows[0]["FormAddressline3"].ToString() == "") ? "" : ds.Tables[5].Rows[0]["FormAddressline3"].ToString();
        //                FromCompanyName = (ds.Tables[5].Rows[0]["FromCompanyName"].ToString() == "") ? "" : ds.Tables[5].Rows[0]["FromCompanyName"].ToString();
        //            }

        //            if (ds.Tables[6] != null && ds.Tables[6].Rows.Count > 0)
        //            {
        //                JobDate = !string.IsNullOrEmpty(ds.Tables[6].Rows[0][0].ToString()) ? Convert.ToDateTime(ds.Tables[6].Rows[0][0]).ToString("dd MMMM yyyy") : string.Empty;
        //            }

        //            if (ds.Tables[7] != null && ds.Tables[7].Rows.Count > 0)
        //            {
        //                JobDescription = !string.IsNullOrEmpty(ds.Tables[7].Rows[0][0].ToString()) ? ds.Tables[7].Rows[0][0].ToString() : string.Empty;
        //            }

        //            if (ds.Tables[8] != null && ds.Tables[8].Rows.Count > 0)
        //            {
        //                JobTitle = !string.IsNullOrEmpty(ds.Tables[8].Rows[0][0].ToString()) ? ds.Tables[8].Rows[0][0].ToString() : string.Empty;
        //            }

        //            if (ds.Tables[9] != null && ds.Tables[9].Rows.Count > 0)
        //            {
        //                JobAddress = !string.IsNullOrEmpty(ds.Tables[9].Rows[0][0].ToString()) ? ds.Tables[9].Rows[0][0].ToString() : string.Empty;
        //            }

        //            viewer.LocalReport.ReportPath = AppDomain.CurrentDomain.BaseDirectory + "//Reports//InvoiceSTC.rdlc";
        //            viewer.LocalReport.EnableExternalImages = true;
        //            ReportDataSource rds1 = new ReportDataSource("dt", dt);
        //            viewer.LocalReport.DataSources.Add(rds1);

        //            viewer.LocalReport.SetParameters(new ReportParameter("RefNumber", RefNumber));
        //            viewer.LocalReport.SetParameters(new ReportParameter("CompanyABN", CompanyABN));
        //            viewer.LocalReport.SetParameters(new ReportParameter("CompanyABNReseller", CompanyABNReseller));
        //            viewer.LocalReport.SetParameters(new ReportParameter("InoviceDate", InoviceDate));
        //            viewer.LocalReport.SetParameters(new ReportParameter("InvoiceNumber", InvoiceNumber));
        //            viewer.LocalReport.SetParameters(new ReportParameter("AmountDue", AmountDue));
        //            viewer.LocalReport.SetParameters(new ReportParameter("Total", Total));
        //            viewer.LocalReport.SetParameters(new ReportParameter("DueDate", DueDate));
        //            viewer.LocalReport.SetParameters(new ReportParameter("ToAddressLine1", ToAddressLine1));
        //            viewer.LocalReport.SetParameters(new ReportParameter("ToAddressLine2", ToAddressLine2));
        //            viewer.LocalReport.SetParameters(new ReportParameter("ToAddressLine3", ToAddressLine3));
        //            viewer.LocalReport.SetParameters(new ReportParameter("FromAddressLine1", FromAddressLine1));
        //            viewer.LocalReport.SetParameters(new ReportParameter("FromAddressLine2", FromAddressLine2));
        //            viewer.LocalReport.SetParameters(new ReportParameter("FromAddressLine3", FromAddressLine3));
        //            viewer.LocalReport.SetParameters(new ReportParameter("JobDate", JobDate));
        //            viewer.LocalReport.SetParameters(new ReportParameter("JobDescription", JobDescription));
        //            viewer.LocalReport.SetParameters(new ReportParameter("JobTitle", JobTitle));
        //            viewer.LocalReport.SetParameters(new ReportParameter("JobAddress", JobAddress));
        //            viewer.LocalReport.SetParameters(new ReportParameter("LogoPath", LogoPath));
        //            viewer.LocalReport.SetParameters(new ReportParameter("InvoiceFooter", InvoiceFooter));
        //            viewer.LocalReport.SetParameters(new ReportParameter("ToName", ToName));
        //            viewer.LocalReport.SetParameters(new ReportParameter("FromName", FromName));
        //            viewer.LocalReport.SetParameters(new ReportParameter("IsStcInvoice", IsStcInvoice));
        //            viewer.LocalReport.SetParameters(new ReportParameter("FromCompanyName", FromCompanyName));
        //            viewer.LocalReport.SetParameters(new ReportParameter("ToCompanyName", ToCompanyName));
        //            viewer.LocalReport.SetParameters(new ReportParameter("AccountName", AccountName));
        //            viewer.LocalReport.SetParameters(new ReportParameter("BSB", BSB));
        //            viewer.LocalReport.SetParameters(new ReportParameter("AccountNumber", AccountNumber));

        //            viewer.LocalReport.Refresh();
        //            byte[] bytes = viewer.LocalReport.Render(ExportType, null, out mimeType, out encoding, out extension, out streamIds, out warnings);
        //            path = ByteArrayToFile(InvoiceNo, bytes, jobid);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log("An error occured while generating STCInvioce file for " + ex.Message);
        //    }
        //    return path;
        //}

        //public Settings GetSettingsData(string SolarCompanyId, int ResellerUserId, int UserTypeId, int ResellerID)
        //{
        //    SettingsBAL settingsBAL = new SettingsBAL();
        //    Settings settings = new Settings();
        //    int? solarCompanyId = Convert.ToInt32(SolarCompanyId);

        //    settings = settingsBAL.GetChargesPartsPaymentCodeAndSettings(ResellerUserId, UserTypeId, solarCompanyId, ResellerID);
        //    return settings;
        //}

        //public String ByteArrayToFile(string _FileName, byte[] _ByteArray, string jobID)
        //{
        //    try
        //    {
        //        string physicalPath = Convert.ToString(ConfigurationManager.AppSettings["ProofUploadFolder"]) + "\\" + "JobDocuments" + "\\" + jobID + "\\" + "Invoice\\Report" + "\\" + _FileName;
        //        string filePath = "JobDocuments" + "\\" + jobID + "\\" + "Invoice\\Report" + "\\" + _FileName + ".pdf";

        //        if (!Directory.Exists(Path.GetDirectoryName(physicalPath)))
        //        {
        //            Directory.CreateDirectory(Path.GetDirectoryName(physicalPath));
        //        }

        //        System.IO.File.WriteAllBytes(physicalPath + ".pdf", _ByteArray);
        //        return filePath;
        //    }
        //    catch (Exception _Exception)
        //    {
        //        Log("Exception caught in process: " + _Exception.ToString());
        //    }
        //    return string.Empty;
        //}

        /// <summary>
        /// Updates the priority for jobs.
        /// </summary>
        public void UpdatePriorityForJobs()
        {
            Common.Log("UpdatePriorityForJobs call");
            int UrgentJobDay = Convert.ToInt32(ConfigurationManager.AppSettings["UrgentJobDay"].ToString());
            objCreateJobBAL.UpdatePriorityForJobs(UrgentJobDay);
            Common.Log("UpdatePriorityForJobs completed");
        }

        /// <summary>
        /// Delete REC Folder
        /// </summary>
        public void RECDeleteFolder()
        {
            try
            {
                DataTable dt = _createJob.GetDateTimeTicksFromQueuedRECSubmission();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string recFolderDirectory = ProjectSession.ProofDocuments + "\\UserDocuments\\" + dr["datetimeticks"].ToString();
                        foreach (var filePath in System.IO.Directory.GetFiles(recFolderDirectory, "*.*", System.IO.SearchOption.AllDirectories))
                        {
                            if (System.IO.File.Exists(filePath))
                                System.IO.File.Delete(filePath);
                        }
                        Directory.Delete(recFolderDirectory);
                        string recZipFilepath = ProjectSession.ProofDocuments + "\\UserDocuments\\" + dr["datetimeticks"].ToString() + "_REC.zip";
                        if (File.Exists(recZipFilepath))
                            System.IO.File.Delete(recZipFilepath);
                    }
                }

            }
            catch (Exception ex)
            {
                Common.Log(ex.Message);
            }
        }
        #endregion

        #region Fetch email for every reseller

        /// <summary>
        /// Handles the Elapsed event of the resellerEmailTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        void resellerEmailTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Common.Log("resellerEmailTimer_Elapsed call");
                FetchResellerNewMail();
                ResetTimer_ResellerEmail();
                Common.Log("resellerEmailTimer_Elapsed completed");
            }
            catch (Exception ex)
            {
                Common.Log("ERROR:" + ex.ToString());
            }
            finally
            {
                Common.Log("Timer elapsed completed");
            }
        }

        /// <summary>
        /// Fetches the reseller new mail.
        /// </summary>
        public void FetchResellerNewMail()
        {
            try
            {
                string url = Convert.ToString(ConfigurationManager.AppSettings["FetchResellerNewMailUrl"]);
                System.Net.WebClient webClient = new System.Net.WebClient();
                string request = webClient.DownloadString(url);
            }
            catch (Exception ex)
            {
                Common.Log(ex.Message);
            }
        }

        #endregion

        #region Common Methods

        /// <summary>
        /// Calculates the time interval.
        /// </summary>
        /// <param name="runTime">The run time.</param>
        /// <returns>double object</returns>
        public double CalculateTimeInterval(string runTime)
        {
            DateTime nowTime = DateTime.Now;
            DateTime timeToRun = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, Convert.ToInt32(runTime.Split(':')[0]), Convert.ToInt32(runTime.Split(':')[1].Substring(0, 2)), 0, 0);
            Common.Log(timeToRun.ToString("dd/MM/yyyy HH:mm:ss"));
            if (nowTime > timeToRun)
            {
                timeToRun = timeToRun.AddDays(1);
            }

            Common.Log(runTime + " Service scheduled to run after: " + (double)(timeToRun - DateTime.Now).TotalMilliseconds);
            return (double)(timeToRun - DateTime.Now).TotalMilliseconds;
        }

        public double CalculateTimeIntervalInDays(string runTime)
        {
            DateTime timeToRun = DateTime.Now.AddDays(Convert.ToInt32(runTime));
            Common.Log(timeToRun.ToString("dd/MM/yyyy HH:mm:ss"));
            Common.Log(runTime + " Service scheduled to run in days after: " + (double)(timeToRun - DateTime.Now).TotalMilliseconds);
            return (double)(timeToRun - DateTime.Now).TotalMilliseconds;
        }

        /// <summary>
        /// Reset timer for reseller email and set to next schedule
        /// </summary>
        void ResetTimer_ResellerEmail()
        {
            int TimeInterval = Convert.ToInt32(ConfigurationManager.AppSettings["TimeIntervalInMinutes_ResellerEmail"]);
            DateTime nextSchedule = DateTime.MinValue;
            DateTime dtNow = DateTime.Now;
            nextSchedule = dtNow.AddMinutes(TimeInterval);

            TimeSpan ts = new TimeSpan();
            ts = nextSchedule - dtNow;
            resellerEmailTimer.Interval = ts.TotalMilliseconds;
            resellerEmailTimer.Enabled = true;

            Common.Log("Next Service Scheduled Call at : " + nextSchedule.ToString());
            resellerEmailTimer.Start();

        }

        #endregion

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(string[] args)
        {
            Common.Log("Service start");
            priorityServiceTimer = new Timer();
            priorityServiceTimer.Elapsed += serviceTimer_Elapsed;
            priorityServiceTimer.Interval = CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["TimeToRun"]));
            priorityServiceTimer.Enabled = true;

            RECDataServiceTimer = new Timer();
            RECDataServiceTimer.Elapsed += RECDataServiceTimer_Elapsed;
            RECDataServiceTimer.Interval = CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["RECDataServiceTimeToRun"]));
            RECDataServiceTimer.Enabled = true;

            Common.Log("Service run for REC Auto Upload");
            RECAutoUploadServiceTimer = new Timer();
            RECAutoUploadServiceTimer.Elapsed += RECAutoUploadServiceTimer_Elapsed;
            RECAutoUploadServiceTimer.Interval = CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["RECAutoUploadServiceTimeToRun"]));
            RECAutoUploadServiceTimer.Enabled = true;

            solarCompanyRegisteredWithGST = new Timer();
            solarCompanyRegisteredWithGST.Elapsed += SolarCompanyGSTserviceTimer_Elapsed;
            solarCompanyRegisteredWithGST.Interval = CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["SolarCompanyGSTTimeToRun"]));
            solarCompanyRegisteredWithGST.Enabled = true;

            //jobOwnerRegisteredWithGST = new Timer();
            //jobOwnerRegisteredWithGST.Elapsed += JobOwnerGSTserviceTimer_Elapsed;
            //jobOwnerRegisteredWithGST.Interval = CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["JobOwnerGSTTimeToRun"]));
            //jobOwnerRegisteredWithGST.Enabled = true;

            deleteTempCheckListItem = new Timer();
            deleteTempCheckListItem.Elapsed += DeleteTempCheckListItemserviceTimer_Elapsed;
            deleteTempCheckListItem.Interval = CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["SolarCompanyGSTTimeToRun"]));
            deleteTempCheckListItem.Enabled = true;

            Common.Log("Service run for Reseller Email");
            resellerEmailTimer = new Timer();
            resellerEmailTimer.Elapsed += resellerEmailTimer_Elapsed;
            ResetTimer_ResellerEmail();

            Common.Log("Service run for Email Shedular");
            sentMailTimer = new System.Timers.Timer();
            sentMailTimer.Elapsed += sentMailTimer_Elapsed;
            sentMailTimer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings["TimeIntervalInMilliSecond_SentMail"]);// emailBAL.GetEmailSentSchedulerIntervalTime();//CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["SolarCompanyGSTTimeToRun"]));//emailBAL.GetEmailSentSchedulerIntervalTime();//CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["SolarCompanyGSTTimeToRun"]));
            sentMailTimer.Enabled = true;


            //Common.Log("Service run for send Email to installer Shedular");
            //sentMailTimer = new System.Timers.Timer();
            //sentMailTimer.Elapsed += sentMailToAllInstallerTimer_Elapsed;
            //sentMailTimer.Interval = emailBAL.GetEmailSentSchedulerIntervalTime();//CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["SolarCompanyGSTTimeToRun"]));//emailBAL.GetEmailSentSchedulerIntervalTime();//CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["SolarCompanyGSTTimeToRun"]));
            //sentMailTimer.Enabled = true;

            Common.Log("Service run for Update Cache");
            updateCacheTimer = new Timer();
            updateCacheTimer.Interval = 86400000; //1 DAY Interval
            updateCacheTimer.Elapsed += UpdateCacheTimer_Elapsed;
            //updateCacheTimer.Interval = CalculateTimeIntervalInDays(Convert.ToString(ConfigurationManager.AppSettings["UpdateCacheTimeToRun"]));
            updateCacheTimer.Enabled = true;

            //Log("Service run for ProductVerification SPV");
            //SPVProductVerificationTimer = new Timer();
            //SPVProductVerificationTimer.Elapsed += SPVProductVerificationTimer_Elapsed;
            //SPVProductVerificationTimer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings["SPVProductVerificationTimer"]);//CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["SolarCompanyGSTTimeToRun"]));//emailBAL.GetEmailSentSchedulerIntervalTime();//CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["SolarCompanyGSTTimeToRun"]));
            //SPVProductVerificationTimer.Enabled = true;
            //resetPwdflag = new Timer();
            //resetPwdflag.Elapsed += resetPasswordTimer_Elapsed;
            //resetPwdflag.Interval = CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["TimeIntervalInHour_ResetPassword"]));
            //resetPwdflag.Enabled = true;

            Common.Log("Service run for REC Upload Shedular");
            recUploadTimer = new System.Timers.Timer();
            recUploadTimer.Elapsed += RECUploadTimer_Elapsed;
            recUploadTimer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings["TimeIntervalInSecond_UploadToREC"]);
            recUploadTimer.Enabled = true;

            Common.Log("Service run for Update Urgent Jobs Flag");
            urgentJobsTimer = new Timer();
            urgentJobsTimer.Elapsed += UrgentJobServiceTimer_Elapsed;
            urgentJobsTimer.Interval = CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["UrgentJobsTimeToRun"]));
            urgentJobsTimer.Enabled = true;

            Common.Log("Service run for Update EntityName");
            updateEntityName = new Timer();
            updateEntityName.Elapsed += UpdateEntityName_Elapsed;
            updateEntityName.Interval = CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["UpdateEntityNameTimeToRun"]));
            updateEntityName.Enabled = true;

            Common.Log("Service run for Deleting REC folder");
            recDeleteFolderTimer = new Timer();
            recDeleteFolderTimer.Elapsed += RECDeleteFolderTimer_Elapsed;
            recDeleteFolderTimer.Interval = CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["RECDeleteFolderTimeToRun"]));
            recDeleteFolderTimer.Enabled = true;

            Common.Log("Service Stop");

            //Log("Service run for Sync VEEcScheduleActivityPremise From VEECPortal Start");
            //SyncVEEcScheduleActivityPremiseFromVEECPortal = new System.Timers.Timer();
            //SyncVEEcScheduleActivityPremiseFromVEECPortal.Elapsed += SyncVEEcScheduleActivityPremiseFromVEECPortal_Elapsed;
            //SyncVEEcScheduleActivityPremiseFromVEECPortal.Interval = Convert.ToDouble(ConfigurationManager.AppSettings["TimeIntervalInSecond_SyncVEEcScheduleActivityPremiseFromVEECPortal"]);//CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["SolarCompanyGSTTimeToRun"]));
            //SyncVEEcScheduleActivityPremiseFromVEECPortal.Enabled = true;
            //Log("Service Stop");
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override void OnStop()
        {
            resellerEmailTimer.Enabled = false;
            resellerEmailTimer.Stop();
            Common.Log("Service stop");
        }

        /// <summary>
        /// Logs the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        //private void Log(string text)
        //{
        //	string path = AppDomain.CurrentDomain.BaseDirectory + "ServiceLog.txt";
        //	using (StreamWriter writer = new StreamWriter(path, true))
        //	{
        //		writer.WriteLine(DateTime.Now.ToString() + Environment.NewLine + text + Environment.NewLine + "------------------------------------------------------------------------");
        //		writer.Close();
        //	}
        //}

        /// <summary>
        /// Inserts the record failure job reason.
        /// </summary>
        /// <param name="dtReason">The data table reason.</param>
        private void InsertRECFailureJobReason(DataTable dtReason)
        {
            try
            {
                objCreateJobBAL.InsertRECFailureJobReason(dtReason);
            }
            catch (Exception ex)
            {
                Common.Log("An error has occured while Insert Job Failure Reason: " + ex.Message);
            }
        }

        //public void SendMailOnCERFailed(string stcjobids)
        //{
        //	foreach (string id in stcjobids.Split(','))
        //	{
        //		int STCJobDetailsId = Convert.ToInt32(id);
        //		DataSet ds = objCreateJobBAL.GetDetailsOfCERFailedJobForMail(STCJobDetailsId);
        //		if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
        //		{
        //			DataRow dr = ds.Tables[0].Rows[0];
        //			EmailInfo emailInfo = new EmailInfo();
        //			emailInfo.TemplateID = 33;
        //			emailInfo.ReferenceNumber = Convert.ToString(dr["ReferenceNumber"]);
        //			emailInfo.OwnerName = Convert.ToString(dr["OwnerName"]);
        //			emailInfo.InstallationAddress = Convert.ToString(dr["InstallationAddress"]);
        //			emailInfo.SystemSize = Convert.ToDecimal(dr["SystemSize"]);
        //			emailInfo.STCsValue = Convert.ToString(dr["STCsValue"]);
        //			emailInfo.TotalValue = Convert.ToDecimal(dr["TotalValue"]);
        //			emailInfo.ResellerFullName = Convert.ToString(dr["ResellerFullName"]);
        //			emailInfo.FailureNotice = Convert.ToString(dr["FailureNotice"]);
        //			emailInfo.Date = DateTime.Now;
        //			emailBAL.ComposeAndSendEmail(emailInfo, Convert.ToString(dr["EmailAddresses"]));
        //		}
        //	}
        //}
        //public void TempData()
        //{
        //    DataTable dtReason = new DataTable();
        //    dtReason.Clear();
        //    dtReason.Columns.Add("jobId");
        //    dtReason.Columns.Add("reason");
        //    dtReason.Columns.Add("completedTime");
        //    dtReason.Columns.Add("jobType");

        //    List<string> lstFailureReasons = new List<string>();
        //    string RECUsername = "LAMH53041";
        //    string RecPassword = "Em3rging786";
        //    DateTime CompletedTime = Convert.ToDateTime("2017-04-07");
        //    string STCPVDCode = "SW2647758";
        //    int JobID = 953;
        //    int jobType = 2;

        //    //string STCPVDCode = "PVD2643769";
        //    //int JobID = 1236;
        //    //int jobType = 1;

        //    if (!string.IsNullOrEmpty(STCPVDCode) && !string.IsNullOrEmpty(RECUsername) && !string.IsNullOrEmpty(RecPassword) && JobID > 0 && jobType > 0)
        //    {
        //        // Get Failure reasons from REC using PVDCode 
        //        RECRegistry.AuthenticateUser_UploadFileForREC(ref lstFailureReasons, RECUsername, RecPassword, STCPVDCode, jobType);
        //        Log("Reason for JobID:" + JobID + " Failure reason count: " + lstFailureReasons.Count);
        //        //Insert into Reason and JobReason Table
        //        foreach (var reason in lstFailureReasons)
        //        {
        //            dtReason.Rows.Add(JobID, reason, CompletedTime, jobType);
        //        }
        //    }
        //}

        public void MakeSCARegisteredWithGST()
        {
            try
            {
                Common.Log("Start Get All Company To Make Registered With GST = ");
                List<SolarCompany> lstCompanyABN = objCreateJobBAL.GetAllCompanyToMakeRegisteredWithGST();
                Common.Log("End Get All Company To Make Registered With GST = ");
                DataTable dtSCAWithGST = SCAWithGSTValue();
                if (lstCompanyABN != null && lstCompanyABN.Count > 0)
                {
                    Common.Log("Start search Company ABN ");
                    for (int i = 0; i < lstCompanyABN.Count; i++)
                    {
                        bool isRegistered = false;
                        string GSTText = string.Empty;
                        string abnURL = ConfigurationManager.AppSettings["ABNSearch"].ToString() + lstCompanyABN[i].CompanyABN;
                        Common.Log(abnURL);
                        try
                        {
                            HttpWebRequest wreq = (HttpWebRequest)WebRequest.Create(abnURL);
                            wreq.Method = "GET";
                            wreq.Timeout = -1;
                            wreq.ContentType = "application/json; charset=UTF-8";
                            var myHttpWebResponse = (HttpWebResponse)wreq.GetResponse();
                            string strResult;
                            using (var reader = new StreamReader(myHttpWebResponse.GetResponseStream(), System.Text.Encoding.UTF8))
                            {
                                strResult = reader.ReadToEnd();
                                myHttpWebResponse.Close();
                            }

                            if (strResult != null)
                            {
                                strResult = WebUtility.HtmlDecode(strResult);
                                HtmlDocument resultat = new HtmlDocument();
                                resultat.LoadHtml(strResult);

                                HtmlNode table = resultat.DocumentNode.SelectSingleNode("//table[1]");
                                if (table != null)
                                {
                                    foreach (var cell in table.SelectNodes(".//tr/th"))
                                    {
                                        string someVariable = cell.InnerText;
                                        if (cell.InnerText.ToLower() == "goods & services tax (gst):")
                                        {
                                            var td = cell.ParentNode.SelectNodes("./td");
                                            string tdValue = td[0].InnerText;
                                            GSTText = tdValue;
                                            if (tdValue.ToLower().Contains("registered from"))
                                            {
                                                isRegistered = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Common.Log("MakeSCARegisteredWithGST = " + ex.Message.ToString());
                        }
                        dtSCAWithGST.Rows.Add(new object[] { lstCompanyABN[i].CompanyABN, isRegistered, GSTText.Trim() });
                    }
                    Common.Log("End search Company ABN ");
                }
                if (dtSCAWithGST.Rows.Count > 0)
                {
                    Common.Log("Start Make SCA Registered/Deregistered With GST");
                    try
                    {
                        objCreateJobBAL.MakeSCARegisteredWithGST(dtSCAWithGST);
                    }
                    catch (Exception ex)
                    {
                        Common.Log("SetSCAGSTInDataSet = " + ex.Message.ToString());
                    }
                    Common.Log("End Make SCA Registered/Deregistered With GST");
                }
            }
            catch (Exception ex)
            {
                Common.Log("Error on MakeSCARegisteredWithGST " + ex.Message.ToString());
            }
        }

        public DataTable SCAWithGSTValue()
        {
            DataTable dtSCAWithGST = new DataTable();
            dtSCAWithGST.Columns.Add("CompanyABN", typeof(string));
            dtSCAWithGST.Columns.Add("isRegisteredWithGST", typeof(bool));
            dtSCAWithGST.Columns.Add("GSTText", typeof(string));
            return dtSCAWithGST;
        }

        public void MakeOwnerRegisteredWithGST()
        {
            try
            {
                Common.Log("Start Get All Job Owner To Make Registered With GST = ");
                List<JobOwnerDetails> lstCompanyABN = objCreateJobBAL.GetAllOwnerWithGST();
                Common.Log("End Get All Job Owner To Make Registered With GST = ");
                DataTable dtOwnerWithGST = SCAWithGSTValue();
                if (lstCompanyABN != null && lstCompanyABN.Count > 0)
                {
                    Common.Log("Start search Company ABN ");
                    for (int i = 0; i < lstCompanyABN.Count; i++)
                    {
                        bool isRegistered = false;
                        string GSTText = string.Empty;
                        string abnURL = ConfigurationManager.AppSettings["ABNSearch"].ToString() + lstCompanyABN[i].CompanyABN;
                        Common.Log(abnURL);
                        try
                        {
                            HttpWebRequest wreq = (HttpWebRequest)WebRequest.Create(abnURL);
                            wreq.Method = "GET";
                            wreq.Timeout = -1;
                            wreq.ContentType = "application/json; charset=UTF-8";
                            var myHttpWebResponse = (HttpWebResponse)wreq.GetResponse();
                            string strResult;
                            using (var reader = new StreamReader(myHttpWebResponse.GetResponseStream(), System.Text.Encoding.UTF8))
                            {
                                strResult = reader.ReadToEnd();
                                myHttpWebResponse.Close();
                            }

                            if (strResult != null)
                            {
                                strResult = WebUtility.HtmlDecode(strResult);
                                HtmlDocument resultat = new HtmlDocument();
                                resultat.LoadHtml(strResult);

                                HtmlNode table = resultat.DocumentNode.SelectSingleNode("//table[1]");
                                if (table != null)
                                {
                                    foreach (var cell in table.SelectNodes(".//tr/th"))
                                    {
                                        string someVariable = cell.InnerText;
                                        if (cell.InnerText.ToLower() == "goods & services tax (gst):")
                                        {
                                            var td = cell.ParentNode.SelectNodes("./td");
                                            string tdValue = td[0].InnerText;
                                            GSTText = tdValue;
                                            if (tdValue.ToLower().Contains("registered from"))
                                            {
                                                isRegistered = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Common.Log("MakeJObOwnerRegisteredWithGST = " + ex.Message.ToString());
                        }
                        dtOwnerWithGST.Rows.Add(new object[] { lstCompanyABN[i].CompanyABN, isRegistered, GSTText.Trim() });
                    }
                    Common.Log("End search Company ABN ");
                }
                if (dtOwnerWithGST.Rows.Count > 0)
                {
                    Common.Log("Start Make Job Owner Registered/Deregistered With GST");
                    try
                    {
                        objCreateJobBAL.MakeOwnerRegisteredWithGST(dtOwnerWithGST);
                    }
                    catch (Exception ex)
                    {
                        Common.Log("SetJobOwnerGSTInDataSet = " + ex.Message.ToString());
                    }
                    Common.Log("End Make Job Owner Registered/Deregistered With GST");
                }
            }
            catch (Exception ex)
            {
                Common.Log("Error on MakeOwnerRegisteredWithGST " + ex.Message.ToString());
            }
        }

        public async void UpdateCacheForStcSubmission(int resellerId = 0)
        {
            try
            {
                List<int> lstResellerID = new List<int>();
                if (resellerId == 0)
                {
                    lstResellerID = objResellerBAL.GetData(null, false, true).Select(a => a.ResellerID).ToList();

                }
                else
                {
                    lstResellerID.Add(resellerId);
                }
                //string resellerIds = string.Join(",", lstResellerID.Select(n => n.ToString()).ToArray());
                foreach (var RAId in lstResellerID)
                {
                    Common.Log("enter in for loop ResellerID: " + RAId);
                    string serviceUrl = ConfigurationManager.AppSettings["urlCreateCache"].ToString();
                    string UpdateCacheURL = string.Format(serviceUrl, RAId, true);
                    HttpWebRequest requestUpdateCache = (HttpWebRequest)WebRequest.Create(UpdateCacheURL);
                    requestUpdateCache.AutomaticDecompression = DecompressionMethods.GZip;
                    requestUpdateCache.Timeout = 1200000;
                    using (HttpWebResponse responseUpdateCache = (HttpWebResponse)await requestUpdateCache.GetResponseAsync())

                    {
                        Common.Log("cache updated for resellerID: " + RAId + "andresponsestatus:" + responseUpdateCache.StatusCode);
                    }
                    Common.Log("end in for loop ResellerID: " + RAId);

                }
                Common.Log("end to create cache for all RA.");
            }
            catch (Exception ex)
            {
                Common.Log("Error in UpdateCache " + ex.Message.ToString());
            }
        }

        public void SetCacheDataForStcJobIdFromService(int? StcJobDetailsId, int? JobId = null, SortedList<string, string> data = null)
        {
            string serviceUrl = ConfigurationManager.AppSettings["urlCreateCacheForStcJobId"].ToString();
            string UpdateCacheURL = string.Format(serviceUrl, StcJobDetailsId, JobId);
            HttpWebRequest requestUpdateCache = (HttpWebRequest)WebRequest.Create(UpdateCacheURL);
            foreach (var item in data)
            {
                requestUpdateCache.Headers.Add(item.Key, item.Value);
            }
            requestUpdateCache.AutomaticDecompression = DecompressionMethods.GZip;
            requestUpdateCache.Timeout = 12000000;
            using (HttpWebResponse responseUpdateCache = (HttpWebResponse)requestUpdateCache.GetResponse())
            {
                Common.Log("cache updated for stcjobId: " + StcJobDetailsId + "andresponsestatus:" + responseUpdateCache.StatusCode);
                responseUpdateCache.Dispose();
                responseUpdateCache.Close();
            }
        }
        public static void SetCacheDataForStcJobIds(string StcJobDetailsIds)
        {
            try
            {
                Common.Log(DateTime.Now + "start update cache: " + StcJobDetailsIds.ToString());
                string serviceUrl = ConfigurationManager.AppSettings["urlCreateCacheForStcJobIdForstcIds"].ToString();
                string UpdateCacheURL = string.Format(serviceUrl, StcJobDetailsIds);
                HttpWebRequest requestUpdateCache = (HttpWebRequest)WebRequest.Create(UpdateCacheURL);
                Common.Log(DateTime.Now + "start update cache 1 updatecacheURL: " + UpdateCacheURL);
                requestUpdateCache.AutomaticDecompression = DecompressionMethods.GZip;
                requestUpdateCache.Timeout = 12000000;
                using (HttpWebResponse responseUpdateCache = (HttpWebResponse)requestUpdateCache.GetResponse())
                {
                    Common.Log("cache updated for stcjobId from rec registry: " + StcJobDetailsIds + "andresponsestatus:" + responseUpdateCache.StatusCode);
                    responseUpdateCache.Dispose();
                    responseUpdateCache.Close();
                }


            }
            catch (Exception ex)
            {
                Common.Log(DateTime.Now + " something wrong in cache updating for stcjobId: " + StcJobDetailsIds + "exception: " + ex.InnerException.Message.ToString());
            }

        }

        public void SetCacheDataForPeakPay(string stcJobdetailIds)
        {
            string serviceUrl = ConfigurationManager.AppSettings["urlUpdatePeakPayCacheForStcJobId"].ToString();
            string UpdateCacheURL = string.Format(serviceUrl, stcJobdetailIds);
            HttpWebRequest requestUpdateCache = (HttpWebRequest)WebRequest.Create(UpdateCacheURL);
            requestUpdateCache.AutomaticDecompression = DecompressionMethods.GZip;
            requestUpdateCache.Timeout = 12000000;
            using (HttpWebResponse responseUpdateCache = (HttpWebResponse)requestUpdateCache.GetResponse())
            {
                Common.Log("peak pay cache updated for stcjobId: " + stcJobdetailIds + "andresponsestatus:" + responseUpdateCache.StatusCode);
            }
        }

        public void DeleteTempCheckList()
        {
            try
            {
                Common.Log("Start to delete all temp visitCheckListItem = ");
                _checkListItemBAL.DeleteTempVisitCheckListItem();
                Common.Log("End to delete all temp visitCheckListItem = ");
            }
            catch (Exception ex)
            {
                Common.Log("Error on delete all temp visitCheckListItem " + ex.Message.ToString());
            }
        }

        public void SyncVEEcScheduleActivityPremiseFromVEECPortal_Elapsed(object sender, ElapsedEventArgs e)
        {

            VEECData objVeecData = new VEECData();
            objVeecData.SyncVEECScheduleActivityPremiseFromVEECPortal();
        }

        void resetPasswordTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _login.UpdateFlagResetPassword();

        }
        public void UpdateEntityName_Elapsed(object sender, ElapsedEventArgs e)
        {
            Common.Log("UpdateEntityName_Elapsed call");
            UpdateEntityName();
            updateEntityName.Interval = CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["UpdateEntityNameTimeToRun"]));
            Common.Log("UpdateEntityName_Elapsed completed");
        }
        #region update urgent job stc status 

        /// <summary>
        /// Handles the Elapsed event of the serviceTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        void UrgentJobServiceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Common.Log("UrgentJobServiceTimer_Elapsed call");
            FlagUrgentJobs();
            urgentJobsTimer.Interval = CalculateTimeInterval(Convert.ToString(ConfigurationManager.AppSettings["UrgentJobsTimeToRun"]));
            Common.Log("UrgentJobServiceTimer_Elapsed completed");
        }

        public void FlagUrgentJobs()
        {
            try
            {
                Common.Log("Start Update All Jobs To Flag them urgent");
                _createJob.UpdateUrgentJobs(Convert.ToInt32(ConfigurationManager.AppSettings["SetUrgentSTCStatusJobDay"]));
                Common.Log("End Update All Jobs To Flag them urgent");
            }
            catch (Exception ex)
            {
                Common.Log("Error on FlagUrgentJobs " + ex.Message.ToString());
            }
        }
        public void UpdateEntityName()
        {
            try
            {
                Common.Log("Start Update Entity Name");
                DataSet ds = _userBAL.GetAllUsersWithABN();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string entityName = GetEntityName(Convert.ToString(ds.Tables[0].Rows[i]["CompanyABN"]));
                    _userBAL.UpdateEntityName(Convert.ToInt32(ds.Tables[0].Rows[i]["userid"]), entityName);

                    //string entityName = GetEntityName(Convert.ToString(ds.Tables[0].Rows[i]["CompanyABN"]));
                    //if (!string.IsNullOrEmpty(entityName))
                    //{
                    //    if (entityName.Contains(","))
                    //    {
                    //        string validEntityName = string.Empty;
                    //        validEntityName = entityName.Split(',')[1].ToString() + " " + entityName.Split(',')[0].ToString();
                    //        _userBAL.UpdateEntityName(Convert.ToInt32(ds.Tables[0].Rows[i]["userid"]), validEntityName);
                    //    }
                    //    else
                    //        _userBAL.UpdateEntityName(Convert.ToInt32(ds.Tables[0].Rows[i]["userid"]), entityName);
                    //}

                }
                Common.Log("End Update Entity Name");
            }
            catch (Exception ex)
            {
                Common.Log("error on UpdateEntityName");
            }
        }
        public string GetEntityName(string companyABN)
        {
            string entityName = string.Empty;
            string abnURL = ConfigurationManager.AppSettings["ABNSearch"].ToString() + companyABN;
            try
            {
                HttpWebRequest wreq = (HttpWebRequest)WebRequest.Create(abnURL);
                wreq.Method = "GET";
                wreq.Timeout = -1;
                wreq.ContentType = "application/json; charset=UTF-8";
                var myHttpWebResponse = (HttpWebResponse)wreq.GetResponse();
                string strResult;
                using (var reader = new StreamReader(myHttpWebResponse.GetResponseStream(), System.Text.Encoding.UTF8))
                {
                    strResult = reader.ReadToEnd();
                    myHttpWebResponse.Close();
                }

                if (strResult != null)
                {
                    strResult = WebUtility.HtmlDecode(strResult);
                    HtmlAgilityPack.HtmlDocument resultat = new HtmlAgilityPack.HtmlDocument();
                    resultat.LoadHtml(strResult);

                    HtmlNode table = resultat.DocumentNode.SelectSingleNode("//table[1]");
                    if (table != null)
                    {
                        foreach (var cell in table.SelectNodes(".//tr/th"))
                        {
                            string someVariable = cell.InnerText;
                            if (cell.InnerText.ToLower() == "entity name:")
                            {
                                var td = cell.ParentNode.SelectNodes("./td");
                                string tdValue = td[0].InnerText;
                                entityName = tdValue.Replace("\r\n", "").Trim();
                                if (entityName.Length > 0)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(SystemEnums.Severity.Error, "companyABN = " + companyABN + " " + ex.Message.ToString());
            }
            return entityName;
        }

        #endregion


        #region Email Sent Scheduler
        void sentMailTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Common.Log("sentEmailTimer_Elapsed call");
                SentEmailTask();
                Common.Log("sentEmailTimer_Elapsed completed");
            }
            catch (Exception ex)
            {
                Common.Log("ERROR:" + ex.ToString());
            }
            finally
            {
                Common.Log("Timer sentEmailTimer_Elapsed completed");
            }
        }
        void sentMailToAllInstallerTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Common.Log("sentEmailInstallerTimer_Elapsed call");
                SentEmailTaskAllInstaller();
                Common.Log("sentEmailInstallerTimer_Elapsed completed");
            }
            catch (Exception ex)
            {
                Common.Log("ERROR:" + ex.ToString());
            }
            finally
            {
                Common.Log("Timer sentEmailInstallerTimer_Elapsed completed");
            }
        }
        public void SentEmailTask()
        {
            if (!emailBAL.GetScheduleTaskRunningStatus("Task.Email.Sent"))
            {
                emailBAL.EmailSentSchedulerTimeMaintain(1);
                var queuedEmails = emailBAL.GetAllPendingQueuedEmailList();
                Common.Log("queuedEmails count" + queuedEmails.Count);
                SendEmailsUsingThreadPool(queuedEmails);
                emailBAL.EmailSentSchedulerTimeMaintain(2);
            }
        }
        public void SentEmailTaskAllInstaller()
        {
            var queuedEmails = emailBAL.GetAllPendingQueuedEmailListForNotifyInstaller();
            Common.Log("queuedEmails count" + queuedEmails.Count);
            SendEmailsUsingThreadPoolForAllInstaller(queuedEmails);
        }
        public void SendEmailsUsingThreadPool(List<QueuedEmail> recipients)
        {
            var coreCount = Environment.ProcessorCount;
            var itemCount = recipients.Count;
            var batchSize = itemCount / coreCount;

            var pending = coreCount;
            using (var mre = new System.Threading.ManualResetEvent(false))
            {
                for (int batchCount = 0; batchCount < coreCount; batchCount++)
                {
                    var lower = batchCount * batchSize;
                    var upper = (batchCount == coreCount - 1) ? itemCount : lower + batchSize;
                    System.Threading.ThreadPool.QueueUserWorkItem(st =>
                    {
                        for (int i = lower; i < upper; i++)
                        {
                            try
                            {
                                if (recipients[i].Guid != null)
                                {
                                    recipients[i].lstAttechment = emailBAL.GetEmailAttechmentOverGuid(recipients[i].Guid);
                                }
                                SendEmail(recipients[i]);
                            }
                            catch (Exception ex)
                            {
                                Common.Log("Send EMail Error" + JsonConvert.SerializeObject(ex));
                                continue;
                            }
                        }
                        if (System.Threading.Interlocked.Decrement(ref pending) == 0)
                            mre.Set();
                    });
                }
                mre.WaitOne();
            }
        }
        public void SendEmailsUsingThreadPoolForAllInstaller(List<QueuedEmail> recipients)
        {
            Common.Log("enter into send email thread");
            var coreCount = Environment.ProcessorCount;
            var itemCount = recipients.Count;
            var batchSize = itemCount / coreCount;

            var pending = coreCount;
            using (var mre = new System.Threading.ManualResetEvent(false))
            {
                for (int batchCount = 0; batchCount < coreCount; batchCount++)
                {
                    var lower = batchCount * batchSize;
                    var upper = (batchCount == coreCount - 1) ? itemCount : lower + batchSize;
                    System.Threading.ThreadPool.QueueUserWorkItem(st =>
                    {
                        for (int i = lower; i < upper; i++)
                        {
                            try
                            {
                                if (recipients[i].Guid != null)
                                {
                                    recipients[i].lstAttechment = emailBAL.GetEmailAttechmentOverGuid(recipients[i].Guid);
                                }
                                Common.Log("calling send email..");
                                //SendEmailToAllInstallers(recipients[i]);
                                SendEmail(recipients[i], true);
                            }
                            catch (Exception ex)
                            {
                                continue;
                            }
                        }
                        if (System.Threading.Interlocked.Decrement(ref pending) == 0)
                            mre.Set();
                    });
                }
                mre.WaitOne();
            }
        }
        public void SendEmail(QueuedEmail objQueuedEmail, bool RegularMail = false)
        {
            bool IsRegularMail = RegularMail;
            var guid = Guid.NewGuid().ToString();
            try
            {
                /*MailMessage mail = new MailMessage();
                if (objQueuedEmail.ToEmail.Contains(";"))
                {
                    foreach (var ToEmail in objQueuedEmail.ToEmail.Split(';'))
                        mail.To.Add(ToEmail.Trim());
                }
                else
                {
                    mail.To.Add(objQueuedEmail.ToEmail);
                }
                mail.From = new MailAddress(smtpDetail.MailFrom);
                mail.Subject = objQueuedEmail.Subject;
                mail.Body = objQueuedEmail.Body;
                mail.IsBodyHtml = true;
                foreach (var item in objQueuedEmail.lstAttechment)
                {
                    if (item.FilePath != null)
                    {
                        string attachmentFilename = ConfigurationManager.AppSettings["ProofUploadFolder"].ToString() + "\\" + item.FilePath;
                        Common.Log(attachmentFilename);
                        FileStream fs = new System.IO.FileStream(attachmentFilename, System.IO.FileMode.Open, System.Security.AccessControl.FileSystemRights.FullControl, FileShare.ReadWrite, 4096, FileOptions.None);
                        ContentType ct = new ContentType();
                        ct.MediaType = item.FileMimeType;
                        ct.Name = Path.GetFileName(attachmentFilename);
                        Attachment attachment = new Attachment(fs, ct);
                        mail.Attachments.Add(attachment);
                    }
                } 
                
                using (SmtpClient smtp = new SmtpClient(smtpDetail.SMTPHost, smtpDetail.SMTPPort))
                {
                    smtp.EnableSsl = smtpDetail.IsSMTPEnableSsl;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential(smtpDetail.SMTPUserName, smtpDetail.SMTPPassword);
                    smtp.Send(mail);
                    smtp.Dispose();
                    objQueuedEmail.SentOn = DateTime.Now;
                }
                mail.Dispose();*/

                EmailGraph objEmailGraph = new EmailGraph();

                if (objEmailGraph.SendEMail(objQueuedEmail))
                {
                    objQueuedEmail.SentOn = DateTime.Now;
                }
                //else
                //{
                //    return;
                //}

                if (IsRegularMail) // Log For SendEmailToAllInstallers
                {
                    Common.Log("end sendemail." + objQueuedEmail.ToEmail);
                    //objQueuedEmail.CreatedDate = DateTime.Now;
                    objQueuedEmail.ModifiedDate = DateTime.Now;

                    objQueuedEmail.SentTries = objQueuedEmail.SentTries + 1;
                    Common.Log("QueuedEmailForNotifyInstallerId: " + objQueuedEmail.QueuedEmailForNotifyInstallerId + "FromEmail:" + objQueuedEmail.FromEmail + "Toemail: " + objQueuedEmail.ToEmail + "CC:" + objQueuedEmail.CC + "Bcc:" + objQueuedEmail.Bcc + "Subject:" + objQueuedEmail.Subject + "Guid:" + objQueuedEmail.Guid + "IsSent:" + objQueuedEmail.IsSent + "Senttries:" + objQueuedEmail.SentTries + "SentOn:" + objQueuedEmail.SentOn + "IsUpdate:" + true);
                    emailBAL.InsertUpdateQueuedEmailForAndrodiAppUser(objQueuedEmail, true);
                }
            }
            catch (Exception exc)
            {
                Common.Log(string.Format("Error sending e-mail. {0}", JsonConvert.SerializeObject(exc)));
            }
            finally
            {
                if (!IsRegularMail)
                {
                    //objQueuedEmail.CreatedDate = DateTime.Now;
                    objQueuedEmail.ModifiedDate = DateTime.Now;

                    objQueuedEmail.SentTries = objQueuedEmail.SentTries + 1;
                    emailBAL.InsertUpdateQueuedEmail(objQueuedEmail, true);
                    if (objQueuedEmail.SentOn != null)
                    {
                        //add log for whole email entry in log file.
                        //LogEmailSent("Sent Mail :: " + JsonConvert.SerializeObject(objQueuedEmail));
                        //var attechments = emailBAL.GetEmailAttechmentOverGuid(objQueuedEmail.Guid);
                        //foreach (var item in attechments)
                        //{
                        //    string attachmentFilename = ConfigurationManager.AppSettings["ProofUploadFolder"].ToString() + "\\" + item.FilePath;
                        //    FileInfo fileInfo = new FileInfo(attachmentFilename);
                        //    string directoryFullPath = fileInfo.DirectoryName;
                        //    try
                        //    {
                        //        System.IO.Directory.Delete(directoryFullPath, true);
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        LogEmailSent(guid + " :: attechments ID :: " + item.AttachmentId + "  ex :: " + JsonConvert.SerializeObject(ex));
                        //    }
                        //}
                        //delete email details from queued table.
                        //emailBAL.DeleteQueuedEmailOverQueuedEmailId(objQueuedEmail.QueuedEmailId);
                    }
                }
                else // Log For SendEmailToAllInstallers
                {
                    //objQueuedEmail.CreatedDate = DateTime.Now;
                    objQueuedEmail.ModifiedDate = DateTime.Now;

                    objQueuedEmail.SentTries = objQueuedEmail.SentTries + 1;
                    emailBAL.InsertUpdateQueuedEmailForAndrodiAppUser(objQueuedEmail, true);
                    Log.WriteLog("toemail: " + objQueuedEmail.ToEmail);
                    if (objQueuedEmail.SentOn != null)
                    {
                        //add log for whole email entry in log file.
                        //LogEmailSent("Sent Mail :: " + JsonConvert.SerializeObject(objQueuedEmail));
                        //var attechments = emailBAL.GetEmailAttechmentOverGuid(objQueuedEmail.Guid);
                        //foreach (var item in attechments)
                        //{
                        //    string attachmentFilename = ConfigurationManager.AppSettings["ProofUploadFolder"].ToString() + "\\" + item.FilePath;
                        //    FileInfo fileInfo = new FileInfo(attachmentFilename);
                        //    string directoryFullPath = fileInfo.DirectoryName;
                        //    try
                        //    {
                        //        System.IO.Directory.Delete(directoryFullPath, true);
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        LogEmailSent(guid + " :: attechments ID :: " + item.AttachmentId + "  ex :: " + JsonConvert.SerializeObject(ex));
                        //    }
                        //}
                        //delete email details from queued table.
                        //emailBAL.DeleteQueuedEmailOverQueuedEmailId(objQueuedEmail.QueuedEmailId);
                    }
                }

            }
        }
        public void SendEmailToAllInstallers(QueuedEmail objQueuedEmail)
        {
            var guid = Guid.NewGuid().ToString();
            try
            {
                Common.Log("start send email to installers:" + objQueuedEmail.ToEmail);
                MailMessage mail = new MailMessage();
                if (objQueuedEmail.ToEmail.Contains(";"))
                {
                    foreach (var ToEmail in objQueuedEmail.ToEmail.Split(';'))
                        mail.To.Add(ToEmail.Trim());
                }
                else
                {
                    mail.To.Add(objQueuedEmail.ToEmail);
                }
                mail.From = new MailAddress(smtpDetail.MailFrom);
                mail.Subject = objQueuedEmail.Subject;
                mail.Body = objQueuedEmail.Body;
                mail.IsBodyHtml = true;
                foreach (var item in objQueuedEmail.lstAttechment)
                {
                    if (item.FilePath != null)
                    {
                        string attachmentFilename = ConfigurationManager.AppSettings["ProofUploadFolder"].ToString() + "\\" + item.FilePath;
                        Common.Log(attachmentFilename);
                        FileStream fs = new System.IO.FileStream(attachmentFilename, System.IO.FileMode.Open, System.Security.AccessControl.FileSystemRights.FullControl, FileShare.ReadWrite, 4096, FileOptions.None);
                        ContentType ct = new ContentType();
                        ct.MediaType = item.FileMimeType;
                        ct.Name = Path.GetFileName(attachmentFilename);
                        Attachment attachment = new Attachment(fs, ct);
                        mail.Attachments.Add(attachment);
                    }
                }
                using (SmtpClient smtp = new SmtpClient(smtpDetail.SMTPHost, smtpDetail.SMTPPort))
                {
                    smtp.EnableSsl = smtpDetail.IsSMTPEnableSsl;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential(smtpDetail.SMTPUserName, smtpDetail.SMTPPassword);
                    smtp.Send(mail);
                    smtp.Dispose();
                    objQueuedEmail.SentOn = DateTime.Now;
                }
                mail.Dispose();
                Common.Log("end sendemail." + objQueuedEmail.ToEmail);
                //objQueuedEmail.CreatedDate = DateTime.Now;
                objQueuedEmail.ModifiedDate = DateTime.Now;

                objQueuedEmail.SentTries = objQueuedEmail.SentTries + 1;
                Common.Log("QueuedEmailForNotifyInstallerId: " + objQueuedEmail.QueuedEmailForNotifyInstallerId + "FromEmail:" + objQueuedEmail.FromEmail + "Toemail: " + objQueuedEmail.ToEmail + "CC:" + objQueuedEmail.CC + "Bcc:" + objQueuedEmail.Bcc + "Subject:" + objQueuedEmail.Subject + "Guid:" + objQueuedEmail.Guid + "IsSent:" + objQueuedEmail.IsSent + "Senttries:" + objQueuedEmail.SentTries + "SentOn:" + objQueuedEmail.SentOn + "IsUpdate:" + true);
                emailBAL.InsertUpdateQueuedEmailForAndrodiAppUser(objQueuedEmail, true);
            }
            catch (Exception exc)
            {
                Common.Log(string.Format("Error sending e-mail. {0}", JsonConvert.SerializeObject(exc)));
            }
            finally
            {
                //objQueuedEmail.CreatedDate = DateTime.Now;
                objQueuedEmail.ModifiedDate = DateTime.Now;

                objQueuedEmail.SentTries = objQueuedEmail.SentTries + 1;
                emailBAL.InsertUpdateQueuedEmailForAndrodiAppUser(objQueuedEmail, true);
                Log.WriteLog("toemail: " + objQueuedEmail.ToEmail);
                if (objQueuedEmail.SentOn != null)
                {
                    //add log for whole email entry in log file.
                    //LogEmailSent("Sent Mail :: " + JsonConvert.SerializeObject(objQueuedEmail));
                    //var attechments = emailBAL.GetEmailAttechmentOverGuid(objQueuedEmail.Guid);
                    //foreach (var item in attechments)
                    //{
                    //    string attachmentFilename = ConfigurationManager.AppSettings["ProofUploadFolder"].ToString() + "\\" + item.FilePath;
                    //    FileInfo fileInfo = new FileInfo(attachmentFilename);
                    //    string directoryFullPath = fileInfo.DirectoryName;
                    //    try
                    //    {
                    //        System.IO.Directory.Delete(directoryFullPath, true);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        LogEmailSent(guid + " :: attechments ID :: " + item.AttachmentId + "  ex :: " + JsonConvert.SerializeObject(ex));
                    //    }
                    //}
                    //delete email details from queued table.
                    //emailBAL.DeleteQueuedEmailOverQueuedEmailId(objQueuedEmail.QueuedEmailId);
                }
            }
        }

        private void LogEmailSent(string text)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Logs/Email/EmailServiceLog_" + DateTime.Now.ToString("ddMMyyyy") + ".txt";
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(DateTime.Now.ToString() + " :: " + text + Environment.NewLine);
            }
        }
        private IDictionary<string, string> _mappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {

        #region Big freaking list of mime types
        // combination of values from Windows 7 Registry and 
        // from C:\Windows\System32\inetsrv\config\applicationHost.config
        // some added, including .7z and .dat
        {".323", "text/h323"},
        {".3g2", "video/3gpp2"},
        {".3gp", "video/3gpp"},
        {".3gp2", "video/3gpp2"},
        {".3gpp", "video/3gpp"},
        {".7z", "application/x-7z-compressed"},
        {".aa", "audio/audible"},
        {".AAC", "audio/aac"},
        {".aaf", "application/octet-stream"},
        {".aax", "audio/vnd.audible.aax"},
        {".ac3", "audio/ac3"},
        {".aca", "application/octet-stream"},
        {".accda", "application/msaccess.addin"},
        {".accdb", "application/msaccess"},
        {".accdc", "application/msaccess.cab"},
        {".accde", "application/msaccess"},
        {".accdr", "application/msaccess.runtime"},
        {".accdt", "application/msaccess"},
        {".accdw", "application/msaccess.webapplication"},
        {".accft", "application/msaccess.ftemplate"},
        {".acx", "application/internet-property-stream"},
        {".AddIn", "text/xml"},
        {".ade", "application/msaccess"},
        {".adobebridge", "application/x-bridge-url"},
        {".adp", "application/msaccess"},
        {".ADT", "audio/vnd.dlna.adts"},
        {".ADTS", "audio/aac"},
        {".afm", "application/octet-stream"},
        {".ai", "application/postscript"},
        {".aif", "audio/x-aiff"},
        {".aifc", "audio/aiff"},
        {".aiff", "audio/aiff"},
        {".air", "application/vnd.adobe.air-application-installer-package+zip"},
        {".amc", "application/x-mpeg"},
        {".application", "application/x-ms-application"},
        {".art", "image/x-jg"},
        {".asa", "application/xml"},
        {".asax", "application/xml"},
        {".ascx", "application/xml"},
        {".asd", "application/octet-stream"},
        {".asf", "video/x-ms-asf"},
        {".ashx", "application/xml"},
        {".asi", "application/octet-stream"},
        {".asm", "text/plain"},
        {".asmx", "application/xml"},
        {".aspx", "application/xml"},
        {".asr", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".atom", "application/atom+xml"},
        {".au", "audio/basic"},
        {".avi", "video/x-msvideo"},
        {".axs", "application/olescript"},
        {".bas", "text/plain"},
        {".bcpio", "application/x-bcpio"},
        {".bin", "application/octet-stream"},
        {".bmp", "image/bmp"},
        {".c", "text/plain"},
        {".cab", "application/octet-stream"},
        {".caf", "audio/x-caf"},
        {".calx", "application/vnd.ms-office.calx"},
        {".cat", "application/vnd.ms-pki.seccat"},
        {".cc", "text/plain"},
        {".cd", "text/plain"},
        {".cdda", "audio/aiff"},
        {".cdf", "application/x-cdf"},
        {".cer", "application/x-x509-ca-cert"},
        {".chm", "application/octet-stream"},
        {".class", "application/x-java-applet"},
        {".clp", "application/x-msclip"},
        {".cmx", "image/x-cmx"},
        {".cnf", "text/plain"},
        {".cod", "image/cis-cod"},
        {".config", "application/xml"},
        {".contact", "text/x-ms-contact"},
        {".coverage", "application/xml"},
        {".cpio", "application/x-cpio"},
        {".cpp", "text/plain"},
        {".crd", "application/x-mscardfile"},
        {".crl", "application/pkix-crl"},
        {".crt", "application/x-x509-ca-cert"},
        {".cs", "text/plain"},
        {".csdproj", "text/plain"},
        {".csh", "application/x-csh"},
        {".csproj", "text/plain"},
        {".css", "text/css"},
        {".csv", "text/csv"},
        {".cur", "application/octet-stream"},
        {".cxx", "text/plain"},
        {".dat", "application/octet-stream"},
        {".datasource", "application/xml"},
        {".dbproj", "text/plain"},
        {".dcr", "application/x-director"},
        {".def", "text/plain"},
        {".deploy", "application/octet-stream"},
        {".der", "application/x-x509-ca-cert"},
        {".dgml", "application/xml"},
        {".dib", "image/bmp"},
        {".dif", "video/x-dv"},
        {".dir", "application/x-director"},
        {".disco", "text/xml"},
        {".dll", "application/x-msdownload"},
        {".dll.config", "text/xml"},
        {".dlm", "text/dlm"},
        {".doc", "application/msword"},
        {".docm", "application/vnd.ms-word.document.macroEnabled.12"},
        {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
        {".dot", "application/msword"},
        {".dotm", "application/vnd.ms-word.template.macroEnabled.12"},
        {".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
        {".dsp", "application/octet-stream"},
        {".dsw", "text/plain"},
        {".dtd", "text/xml"},
        {".dtsConfig", "text/xml"},
        {".dv", "video/x-dv"},
        {".dvi", "application/x-dvi"},
        {".dwf", "drawing/x-dwf"},
        {".dwp", "application/octet-stream"},
        {".dxr", "application/x-director"},
        {".eml", "message/rfc822"},
        {".emz", "application/octet-stream"},
        {".eot", "application/octet-stream"},
        {".eps", "application/postscript"},
        {".etl", "application/etl"},
        {".etx", "text/x-setext"},
        {".evy", "application/envoy"},
        {".exe", "application/octet-stream"},
        {".exe.config", "text/xml"},
        {".fdf", "application/vnd.fdf"},
        {".fif", "application/fractals"},
        {".filters", "Application/xml"},
        {".fla", "application/octet-stream"},
        {".flr", "x-world/x-vrml"},
        {".flv", "video/x-flv"},
        {".fsscript", "application/fsharp-script"},
        {".fsx", "application/fsharp-script"},
        {".generictest", "application/xml"},
        {".gif", "image/gif"},
        {".group", "text/x-ms-group"},
        {".gsm", "audio/x-gsm"},
        {".gtar", "application/x-gtar"},
        {".gz", "application/x-gzip"},
        {".h", "text/plain"},
        {".hdf", "application/x-hdf"},
        {".hdml", "text/x-hdml"},
        {".hhc", "application/x-oleobject"},
        {".hhk", "application/octet-stream"},
        {".hhp", "application/octet-stream"},
        {".hlp", "application/winhlp"},
        {".hpp", "text/plain"},
        {".hqx", "application/mac-binhex40"},
        {".hta", "application/hta"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".htt", "text/webviewhtml"},
        {".hxa", "application/xml"},
        {".hxc", "application/xml"},
        {".hxd", "application/octet-stream"},
        {".hxe", "application/xml"},
        {".hxf", "application/xml"},
        {".hxh", "application/octet-stream"},
        {".hxi", "application/octet-stream"},
        {".hxk", "application/xml"},
        {".hxq", "application/octet-stream"},
        {".hxr", "application/octet-stream"},
        {".hxs", "application/octet-stream"},
        {".hxt", "text/html"},
        {".hxv", "application/xml"},
        {".hxw", "application/octet-stream"},
        {".hxx", "text/plain"},
        {".i", "text/plain"},
        {".ico", "image/x-icon"},
        {".ics", "application/octet-stream"},
        {".idl", "text/plain"},
        {".ief", "image/ief"},
        {".iii", "application/x-iphone"},
        {".inc", "text/plain"},
        {".inf", "application/octet-stream"},
        {".inl", "text/plain"},
        {".ins", "application/x-internet-signup"},
        {".ipa", "application/x-itunes-ipa"},
        {".ipg", "application/x-itunes-ipg"},
        {".ipproj", "text/plain"},
        {".ipsw", "application/x-itunes-ipsw"},
        {".iqy", "text/x-ms-iqy"},
        {".isp", "application/x-internet-signup"},
        {".ite", "application/x-itunes-ite"},
        {".itlp", "application/x-itunes-itlp"},
        {".itms", "application/x-itunes-itms"},
        {".itpc", "application/x-itunes-itpc"},
        {".IVF", "video/x-ivf"},
        {".jar", "application/java-archive"},
        {".java", "application/octet-stream"},
        {".jck", "application/liquidmotion"},
        {".jcz", "application/liquidmotion"},
        {".jfif", "image/pjpeg"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpb", "application/octet-stream"},
        {".jpe", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".json", "application/json"},
        {".jsx", "text/jscript"},
        {".jsxbin", "text/plain"},
        {".latex", "application/x-latex"},
        {".library-ms", "application/windows-library+xml"},
        {".lit", "application/x-ms-reader"},
        {".loadtest", "application/xml"},
        {".lpk", "application/octet-stream"},
        {".lsf", "video/x-la-asf"},
        {".lst", "text/plain"},
        {".lsx", "video/x-la-asf"},
        {".lzh", "application/octet-stream"},
        {".m13", "application/x-msmediaview"},
        {".m14", "application/x-msmediaview"},
        {".m1v", "video/mpeg"},
        {".m2t", "video/vnd.dlna.mpeg-tts"},
        {".m2ts", "video/vnd.dlna.mpeg-tts"},
        {".m2v", "video/mpeg"},
        {".m3u", "audio/x-mpegurl"},
        {".m3u8", "audio/x-mpegurl"},
        {".m4a", "audio/m4a"},
        {".m4b", "audio/m4b"},
        {".m4p", "audio/m4p"},
        {".m4r", "audio/x-m4r"},
        {".m4v", "video/x-m4v"},
        {".mac", "image/x-macpaint"},
        {".mak", "text/plain"},
        {".man", "application/x-troff-man"},
        {".manifest", "application/x-ms-manifest"},
        {".map", "text/plain"},
        {".master", "application/xml"},
        {".mda", "application/msaccess"},
        {".mdb", "application/x-msaccess"},
        {".mde", "application/msaccess"},
        {".mdp", "application/octet-stream"},
        {".me", "application/x-troff-me"},
        {".mfp", "application/x-shockwave-flash"},
        {".mht", "message/rfc822"},
        {".mhtml", "message/rfc822"},
        {".mid", "audio/mid"},
        {".midi", "audio/mid"},
        {".mix", "application/octet-stream"},
        {".mk", "text/plain"},
        {".mmf", "application/x-smaf"},
        {".mno", "text/xml"},
        {".mny", "application/x-msmoney"},
        {".mod", "video/mpeg"},
        {".mov", "video/quicktime"},
        {".movie", "video/x-sgi-movie"},
        {".mp2", "video/mpeg"},
        {".mp2v", "video/mpeg"},
        {".mp3", "audio/mpeg"},
        {".mp4", "video/mp4"},
        {".mp4v", "video/mp4"},
        {".mpa", "video/mpeg"},
        {".mpe", "video/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpf", "application/vnd.ms-mediapackage"},
        {".mpg", "video/mpeg"},
        {".mpp", "application/vnd.ms-project"},
        {".mpv2", "video/mpeg"},
        {".mqv", "video/quicktime"},
        {".ms", "application/x-troff-ms"},
        {".msi", "application/octet-stream"},
        {".mso", "application/octet-stream"},
        {".mts", "video/vnd.dlna.mpeg-tts"},
        {".mtx", "application/xml"},
        {".mvb", "application/x-msmediaview"},
        {".mvc", "application/x-miva-compiled"},
        {".mxp", "application/x-mmxp"},
        {".nc", "application/x-netcdf"},
        {".nsc", "video/x-ms-asf"},
        {".nws", "message/rfc822"},
        {".ocx", "application/octet-stream"},
        {".oda", "application/oda"},
        {".odc", "text/x-ms-odc"},
        {".odh", "text/plain"},
        {".odl", "text/plain"},
        {".odp", "application/vnd.oasis.opendocument.presentation"},
        {".ods", "application/oleobject"},
        {".odt", "application/vnd.oasis.opendocument.text"},
        {".one", "application/onenote"},
        {".onea", "application/onenote"},
        {".onepkg", "application/onenote"},
        {".onetmp", "application/onenote"},
        {".onetoc", "application/onenote"},
        {".onetoc2", "application/onenote"},
        {".orderedtest", "application/xml"},
        {".osdx", "application/opensearchdescription+xml"},
        {".p10", "application/pkcs10"},
        {".p12", "application/x-pkcs12"},
        {".p7b", "application/x-pkcs7-certificates"},
        {".p7c", "application/pkcs7-mime"},
        {".p7m", "application/pkcs7-mime"},
        {".p7r", "application/x-pkcs7-certreqresp"},
        {".p7s", "application/pkcs7-signature"},
        {".pbm", "image/x-portable-bitmap"},
        {".pcast", "application/x-podcast"},
        {".pct", "image/pict"},
        {".pcx", "application/octet-stream"},
        {".pcz", "application/octet-stream"},
        {".pdf", "application/pdf"},
        {".pfb", "application/octet-stream"},
        {".pfm", "application/octet-stream"},
        {".pfx", "application/x-pkcs12"},
        {".pgm", "image/x-portable-graymap"},
        {".pic", "image/pict"},
        {".pict", "image/pict"},
        {".pkgdef", "text/plain"},
        {".pkgundef", "text/plain"},
        {".pko", "application/vnd.ms-pki.pko"},
        {".pls", "audio/scpls"},
        {".pma", "application/x-perfmon"},
        {".pmc", "application/x-perfmon"},
        {".pml", "application/x-perfmon"},
        {".pmr", "application/x-perfmon"},
        {".pmw", "application/x-perfmon"},
        {".png", "image/png"},
        {".pnm", "image/x-portable-anymap"},
        {".pnt", "image/x-macpaint"},
        {".pntg", "image/x-macpaint"},
        {".pnz", "image/png"},
        {".pot", "application/vnd.ms-powerpoint"},
        {".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12"},
        {".potx", "application/vnd.openxmlformats-officedocument.presentationml.template"},
        {".ppa", "application/vnd.ms-powerpoint"},
        {".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12"},
        {".ppm", "image/x-portable-pixmap"},
        {".pps", "application/vnd.ms-powerpoint"},
        {".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
        {".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
        {".ppt", "application/vnd.ms-powerpoint"},
        {".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
        {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
        {".prf", "application/pics-rules"},
        {".prm", "application/octet-stream"},
        {".prx", "application/octet-stream"},
        {".ps", "application/postscript"},
        {".psc1", "application/PowerShell"},
        {".psd", "application/octet-stream"},
        {".psess", "application/xml"},
        {".psm", "application/octet-stream"},
        {".psp", "application/octet-stream"},
        {".pub", "application/x-mspublisher"},
        {".pwz", "application/vnd.ms-powerpoint"},
        {".qht", "text/x-html-insertion"},
        {".qhtm", "text/x-html-insertion"},
        {".qt", "video/quicktime"},
        {".qti", "image/x-quicktime"},
        {".qtif", "image/x-quicktime"},
        {".qtl", "application/x-quicktimeplayer"},
        {".qxd", "application/octet-stream"},
        {".ra", "audio/x-pn-realaudio"},
        {".ram", "audio/x-pn-realaudio"},
        {".rar", "application/octet-stream"},
        {".ras", "image/x-cmu-raster"},
        {".rat", "application/rat-file"},
        {".rc", "text/plain"},
        {".rc2", "text/plain"},
        {".rct", "text/plain"},
        {".rdlc", "application/xml"},
        {".resx", "application/xml"},
        {".rf", "image/vnd.rn-realflash"},
        {".rgb", "image/x-rgb"},
        {".rgs", "text/plain"},
        {".rm", "application/vnd.rn-realmedia"},
        {".rmi", "audio/mid"},
        {".rmp", "application/vnd.rn-rn_music_package"},
        {".roff", "application/x-troff"},
        {".rpm", "audio/x-pn-realaudio-plugin"},
        {".rqy", "text/x-ms-rqy"},
        {".rtf", "application/rtf"},
        {".rtx", "text/richtext"},
        {".ruleset", "application/xml"},
        {".s", "text/plain"},
        {".safariextz", "application/x-safari-safariextz"},
        {".scd", "application/x-msschedule"},
        {".sct", "text/scriptlet"},
        {".sd2", "audio/x-sd2"},
        {".sdp", "application/sdp"},
        {".sea", "application/octet-stream"},
        {".searchConnector-ms", "application/windows-search-connector+xml"},
        {".setpay", "application/set-payment-initiation"},
        {".setreg", "application/set-registration-initiation"},
        {".settings", "application/xml"},
        {".sgimb", "application/x-sgimb"},
        {".sgml", "text/sgml"},
        {".sh", "application/x-sh"},
        {".shar", "application/x-shar"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".sitemap", "application/xml"},
        {".skin", "application/xml"},
        {".sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12"},
        {".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide"},
        {".slk", "application/vnd.ms-excel"},
        {".sln", "text/plain"},
        {".slupkg-ms", "application/x-ms-license"},
        {".smd", "audio/x-smd"},
        {".smi", "application/octet-stream"},
        {".smx", "audio/x-smd"},
        {".smz", "audio/x-smd"},
        {".snd", "audio/basic"},
        {".snippet", "application/xml"},
        {".snp", "application/octet-stream"},
        {".sol", "text/plain"},
        {".sor", "text/plain"},
        {".spc", "application/x-pkcs7-certificates"},
        {".spl", "application/futuresplash"},
        {".src", "application/x-wais-source"},
        {".srf", "text/plain"},
        {".SSISDeploymentManifest", "text/xml"},
        {".ssm", "application/streamingmedia"},
        {".sst", "application/vnd.ms-pki.certstore"},
        {".stl", "application/vnd.ms-pki.stl"},
        {".sv4cpio", "application/x-sv4cpio"},
        {".sv4crc", "application/x-sv4crc"},
        {".svc", "application/xml"},
        {".swf", "application/x-shockwave-flash"},
        {".t", "application/x-troff"},
        {".tar", "application/x-tar"},
        {".tcl", "application/x-tcl"},
        {".testrunconfig", "application/xml"},
        {".testsettings", "application/xml"},
        {".tex", "application/x-tex"},
        {".texi", "application/x-texinfo"},
        {".texinfo", "application/x-texinfo"},
        {".tgz", "application/x-compressed"},
        {".thmx", "application/vnd.ms-officetheme"},
        {".thn", "application/octet-stream"},
        {".tif", "image/tiff"},
        {".tiff", "image/tiff"},
        {".tlh", "text/plain"},
        {".tli", "text/plain"},
        {".toc", "application/octet-stream"},
        {".tr", "application/x-troff"},
        {".trm", "application/x-msterminal"},
        {".trx", "application/xml"},
        {".ts", "video/vnd.dlna.mpeg-tts"},
        {".tsv", "text/tab-separated-values"},
        {".ttf", "application/octet-stream"},
        {".tts", "video/vnd.dlna.mpeg-tts"},
        {".txt", "text/plain"},
        {".u32", "application/octet-stream"},
        {".uls", "text/iuls"},
        {".user", "text/plain"},
        {".ustar", "application/x-ustar"},
        {".vb", "text/plain"},
        {".vbdproj", "text/plain"},
        {".vbk", "video/mpeg"},
        {".vbproj", "text/plain"},
        {".vbs", "text/vbscript"},
        {".vcf", "text/x-vcard"},
        {".vcproj", "Application/xml"},
        {".vcs", "text/plain"},
        {".vcxproj", "Application/xml"},
        {".vddproj", "text/plain"},
        {".vdp", "text/plain"},
        {".vdproj", "text/plain"},
        {".vdx", "application/vnd.ms-visio.viewer"},
        {".vml", "text/xml"},
        {".vscontent", "application/xml"},
        {".vsct", "text/xml"},
        {".vsd", "application/vnd.visio"},
        {".vsi", "application/ms-vsi"},
        {".vsix", "application/vsix"},
        {".vsixlangpack", "text/xml"},
        {".vsixmanifest", "text/xml"},
        {".vsmdi", "application/xml"},
        {".vspscc", "text/plain"},
        {".vss", "application/vnd.visio"},
        {".vsscc", "text/plain"},
        {".vssettings", "text/xml"},
        {".vssscc", "text/plain"},
        {".vst", "application/vnd.visio"},
        {".vstemplate", "text/xml"},
        {".vsto", "application/x-ms-vsto"},
        {".vsw", "application/vnd.visio"},
        {".vsx", "application/vnd.visio"},
        {".vtx", "application/vnd.visio"},
        {".wav", "audio/wav"},
        {".wave", "audio/wav"},
        {".wax", "audio/x-ms-wax"},
        {".wbk", "application/msword"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wcm", "application/vnd.ms-works"},
        {".wdb", "application/vnd.ms-works"},
        {".wdp", "image/vnd.ms-photo"},
        {".webarchive", "application/x-safari-webarchive"},
        {".webtest", "application/xml"},
        {".wiq", "application/xml"},
        {".wiz", "application/msword"},
        {".wks", "application/vnd.ms-works"},
        {".WLMP", "application/wlmoviemaker"},
        {".wlpginstall", "application/x-wlpg-detect"},
        {".wlpginstall3", "application/x-wlpg3-detect"},
        {".wm", "video/x-ms-wm"},
        {".wma", "audio/x-ms-wma"},
        {".wmd", "application/x-ms-wmd"},
        {".wmf", "application/x-msmetafile"},
        {".wml", "text/vnd.wap.wml"},
        {".wmlc", "application/vnd.wap.wmlc"},
        {".wmls", "text/vnd.wap.wmlscript"},
        {".wmlsc", "application/vnd.wap.wmlscriptc"},
        {".wmp", "video/x-ms-wmp"},
        {".wmv", "video/x-ms-wmv"},
        {".wmx", "video/x-ms-wmx"},
        {".wmz", "application/x-ms-wmz"},
        {".wpl", "application/vnd.ms-wpl"},
        {".wps", "application/vnd.ms-works"},
        {".wri", "application/x-mswrite"},
        {".wrl", "x-world/x-vrml"},
        {".wrz", "x-world/x-vrml"},
        {".wsc", "text/scriptlet"},
        {".wsdl", "text/xml"},
        {".wvx", "video/x-ms-wvx"},
        {".x", "application/directx"},
        {".xaf", "x-world/x-vrml"},
        {".xaml", "application/xaml+xml"},
        {".xap", "application/x-silverlight-app"},
        {".xbap", "application/x-ms-xbap"},
        {".xbm", "image/x-xbitmap"},
        {".xdr", "text/plain"},
        {".xht", "application/xhtml+xml"},
        {".xhtml", "application/xhtml+xml"},
        {".xla", "application/vnd.ms-excel"},
        {".xlam", "application/vnd.ms-excel.addin.macroEnabled.12"},
        {".xlc", "application/vnd.ms-excel"},
        {".xld", "application/vnd.ms-excel"},
        {".xlk", "application/vnd.ms-excel"},
        {".xll", "application/vnd.ms-excel"},
        {".xlm", "application/vnd.ms-excel"},
        {".xls", "application/vnd.ms-excel"},
        {".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
        {".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12"},
        {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
        {".xlt", "application/vnd.ms-excel"},
        {".xltm", "application/vnd.ms-excel.template.macroEnabled.12"},
        {".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
        {".xlw", "application/vnd.ms-excel"},
        {".xml", "text/xml"},
        {".xmta", "application/xml"},
        {".xof", "x-world/x-vrml"},
        {".XOML", "text/plain"},
        {".xpm", "image/x-xpixmap"},
        {".xps", "application/vnd.ms-xpsdocument"},
        {".xrm-ms", "text/xml"},
        {".xsc", "application/xml"},
        {".xsd", "text/xml"},
        {".xsf", "text/xml"},
        {".xsl", "text/xml"},
        {".xslt", "text/xml"},
        {".xsn", "application/octet-stream"},
        {".xss", "application/xml"},
        {".xtp", "application/octet-stream"},
        {".xwd", "image/x-xwindowdump"},
        {".z", "application/x-compress"},
        {".zip", "application/x-zip-compressed"},
        #endregion

        };
        public string GetMimeType(string extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }

            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            string mime;

            return _mappings.TryGetValue(extension, out mime) ? mime : "application/octet-stream";
        }
        #endregion

        #region ProductVerificationSPV

        //void SPVProductVerificationTimer_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    try
        //    {
        //        Log("SPVProductVerificationTimer_Elapsed call");
        //        SerialNumberProductVerificationSPV();
        //        Log("SPVProductVerificationTimer_Elapsed completed");
        //    }
        //    catch (Exception ex)
        //    {
        //        Log("ERROR:" + ex.ToString());
        //    }
        //    finally
        //    {
        //        Log("Timer SPVProductVerificationTimer_Elapsed completed");
        //    }
        //}

        //public void SerialNumberProductVerificationSPV()
        //{
        //    try
        //    {
        //        DataSet dsSPV = objCreateJobBAL.GetSPVVerificationUrlSerialNumber();
        //        var ProductVerificationXMLPath = ConfigurationManager.AppSettings["ProductVerificationXMLPath"].ToString();
        //        var ServerCertificate = ConfigurationManager.AppSettings["ServerCertificate"].ToString();
        //        SPVVerification objSPVVerification = new SPVVerification(_spvLog);
        //        DataTable VerifiedSerialNumber  = objSPVVerification.SPVProductVerification(dsSPV, ProductVerificationXMLPath, ServerCertificate);
        //        //objCreateJobBAL.UpdateVerifiedSerialNumber(VerifiedSerialNumber);
        //        Log("Sucessfully complete SerialNumberProductVerificationSPV");

        //    }catch(Exception ex)
        //    {
        //        Log("Error in  SerialNumberProductVerificationSPV : " + ex.Message);
        //    }
        //}

        #endregion
    }

    #region Email Sent Scheduler
    public class SMTPDetails
    {
        /// <summary>
        /// Gets or sets the mail from.
        /// </summary>
        /// <value>
        /// The mail from.
        /// </value>
        public string MailFrom { get; set; }
        /// <summary>
        /// Gets or sets the SMTP host.
        /// </summary>
        /// <value>
        /// The SMTP host.
        /// </value>
        public string SMTPHost { get; set; }
        /// <summary>
        /// Gets or sets the name of the SMTP user.
        /// </summary>
        /// <value>
        /// The name of the SMTP user.
        /// </value>
        public string SMTPUserName { get; set; }
        /// <summary>
        /// Gets or sets the SMTP password.
        /// </summary>
        /// <value>
        /// The SMTP password.
        /// </value>
        public string SMTPPassword { get; set; }
        /// <summary>
        /// Gets or sets the SMTP port.
        /// </summary>
        /// <value>
        /// The SMTP port.
        /// </value>
        public int SMTPPort { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is SMTP enable SSL.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is SMTP enable SSL; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsSMTPEnableSsl { get; set; }

    }
    public class AttechmentFilePath
    {
        public string FilePath { get; set; }
    }
    #endregion
}
