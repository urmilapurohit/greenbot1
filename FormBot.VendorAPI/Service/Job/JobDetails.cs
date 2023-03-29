using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FormBot.VendorAPI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using FormBot.BAL.Service;
using System.Net;
using Newtonsoft.Json.Linq;
using FormBot.Entity;
using FormBot.Entity.Job;
using FormBot.Helper;
using System.IO;
using FormBot.BAL;
using FormBot.Entity.Email;
using System.Data;
using FormBot.BAL.Service.CommonRules;
using System.Xml.Serialization;
using FormBot.Helper.Helper;
using System.Data.SqlClient;
using FormBot.DAL;
using System.Net.Http;
using System.Web.Http;
using System.Drawing;
using System.Configuration;
using System.Drawing.Imaging;
using FormBot.Entity.Documents;
using Newtonsoft.Json;
using FormBot.Entity.SolarElectrician;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace FormBot.VendorAPI.Service.Job
{
    public class JobDetails : IJobDetails
    {

        private readonly IUserBAL _user;
        private readonly ICreateJobHistoryBAL _jobHistory;
        private readonly ICreateJobBAL _job;
        private readonly IEmailBAL _emailService;
        private readonly IJobRulesBAL _jobRules;
        private readonly IGenerateStcReportBAL _generateStcReportBAL;

        public UserManager<ApplicationUser> UserManager { get; private set; }

        public JobDetails(IUserBAL user, ICreateJobHistoryBAL jobHistory, ICreateJobBAL job, IEmailBAL emailService, IJobRulesBAL jobRules, IGenerateStcReportBAL generateStcReportBAL)
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            this._user = user;
            this._jobHistory = jobHistory;
            this._job = job;
            this._emailService = emailService;
            this._jobRules = jobRules;
            this._generateStcReportBAL = generateStcReportBAL;
        }


        /// <summary>
        /// Creates/Edit the job.
        /// </summary>
        /// <param name="createJob">The create job.</param>
        /// <param name="userdata"></param>
        /// <returns></returns>
        public OutputJobResponse CreateJob(CreateJob createJob, User userdata)
        {
            Int32 jobID = 0;
            //CreateJobResponse objCreateJobResponse = new CreateJobResponse();
            JobResponse objJobResponse = new JobResponse();

            string Brand = string.Empty;
            string Model = string.Empty;
            string ErrorMsg = string.Empty;

            DataTable dtPanelSystemBrand = GetPanelSystemBrand();
            if ((createJob.panel != null && createJob.panel.Count() > 0 && createJob.BasicDetails.JobType == 1))
            {
                foreach (var item in createJob.panel)
                {
                    DataRow dr = dtPanelSystemBrand.NewRow();
                    dr["Brand"] = item.Brand;
                    dr["Model"] = item.Model;
                    dtPanelSystemBrand.Rows.Add(dr);
                }
            }
            if (createJob.JobSystemDetails != null && createJob.BasicDetails.JobType == 2 && createJob.JobSystemDetails.SystemBrand != null && createJob.JobSystemDetails.SystemModel != null)
            {
                DataRow dr = dtPanelSystemBrand.NewRow();
                dr["Brand"] = createJob.JobSystemDetails.SystemBrand;
                dr["Model"] = createJob.JobSystemDetails.SystemModel;
                dtPanelSystemBrand.Rows.Add(dr);
            }

            DataTable dtInverter = GetInverterDetails();
            if (createJob.inverter != null && createJob.inverter.Count() > 0 && createJob.BasicDetails.JobType == 1)
            {
                foreach (var item in createJob.inverter)
                {
                    DataRow dr = dtInverter.NewRow();
                    dr["Brand"] = item.Brand;
                    dr["Model"] = item.Model;
                    dr["Series"] = item.Series;
                    dtInverter.Rows.Add(dr);
                }
            }

            DataSet ds = _job.CheckPanelInverterSystemBrand_VendorApi(createJob.BasicDetails.JobType, dtPanelSystemBrand, dtInverter,createJob.BasicDetails.JobID);
            DataTable panelDataTable = new DataTable();
            if (createJob.BasicDetails.JobID > 0)
            {
                panelDataTable = ds.Tables[ds.Tables.Count - 1].Copy();
                ds.Tables.Remove(ds.Tables[ds.Tables.Count - 1]);
            }

            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (Convert.ToInt32(ds.Tables[0].Rows[i]["IsExist"]) == 0)
                        {
                            Brand = Convert.ToString(ds.Tables[0].Rows[i]["Brand"]);
                            Model = Convert.ToString(ds.Tables[0].Rows[i]["Model"]);

                            if (createJob.BasicDetails.JobType == 1)
                            {
                                ErrorMsg += "PanelBrand: " + Brand + " PanelModel: " + Model + " doesn't exist." + Environment.NewLine;
                            }
                            else
                            {
                                ErrorMsg += "SystemBrand: " + Brand + " SystemModel: " + Model + " doesn't exist." + Environment.NewLine;
                            }
                        }
                    }
                }
                if (ds.Tables.Count > 1 && ds.Tables[1] != null)
                {
                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                    {
                        if (Convert.ToInt32(ds.Tables[1].Rows[i]["IsExist"]) == 0)
                        {
                            if (ds.Tables[1] != null)
                            {
                                ErrorMsg += "Inverter Brand: " + Convert.ToString(ds.Tables[1].Rows[i]["Brand"]) + " Inverter Model: " + Convert.ToString(ds.Tables[1].Rows[i]["Model"]) + " Inverter Series: " + Convert.ToString(ds.Tables[1].Rows[i]["Series"]) + " doesn't exist." + Environment.NewLine;
                            }
                        }
                    }
                }
            }

            if (createJob.lstJobNotes != null && createJob.lstJobNotes.Count() > 0)
            {
                for (int i = 0; i < createJob.lstJobNotes.Count; i++)
                {
                    if (createJob.lstJobNotes[i].Notes == null || string.IsNullOrEmpty(createJob.lstJobNotes[i].VendorJobNoteId))
                    {
                        ErrorMsg += " Vendor Job note id not valid";
                    }
                }
            }

            if (!string.IsNullOrEmpty(ErrorMsg))
            {
                objJobResponse = JobResponse(null, ErrorMsg);
                return new OutputJobResponse() { obj = objJobResponse };
            }
            List<AccreditedInstallers> accreditedInstallersList = new List<AccreditedInstallers>();
            List<AccreditedInstallers> accreditedDesignersList = new List<AccreditedInstallers>();
            int installerIdExist = 0, designerIdExist = 0;

            if (createJob.InstallerView != null & createJob.InstallerView.CECAccreditationNumber != null)
            {
                //createJob.InstallerView = new InstallerDesignerView();
                accreditedInstallersList = _user.CheckAccreditationNumberExistsInAccreditedInstallers(createJob.InstallerView.CECAccreditationNumber, null, true, false);
                //accreditedInstallersList = _user.CheckExistAccreditationNumber_VendorApi(createJob.InstallerView.CECAccreditationNumber, createJob.InstallerView.FirstName, createJob.InstallerView.LastName);
            }
            if (createJob.DesignerView != null && createJob.DesignerView.CECAccreditationNumber != null)
            {
                //createJob.DesignerView = new InstallerDesignerView();
                accreditedDesignersList = _user.CheckAccreditationNumberExistsInAccreditedInstallers(createJob.DesignerView.CECAccreditationNumber, null, true, false);
                //accreditedDesignersList = _user.CheckExistAccreditationNumber_VendorApi(createJob.DesignerView.CECAccreditationNumber, createJob.DesignerView.FirstName, createJob.DesignerView.LastName);                                                    
            }

            if ((createJob.InstallerView == null || (createJob.InstallerView.CECAccreditationNumber != null && accreditedInstallersList.Count > 0)) && (createJob.DesignerView == null || (createJob.DesignerView.CECAccreditationNumber != null && accreditedDesignersList.Count > 0)))
            {
                createJob.IsVendorApi = true;
                createJob.BasicDetails.UserTypeID = userdata.UserTypeID;

                //string jobNumber = string.Empty;

                //DataSet dataSet = _job.CreateJobNumber(createJob.BasicDetails.JobType, (int)userdata.SolarCompanyId);
                //if (dataSet != null && dataSet.Tables.Count > 0)
                //{
                //    jobNumber = dataSet.Tables[0].Rows[0].ItemArray[0].ToString();
                //}
                //createJob.BasicDetails.JobNumber = jobNumber;

                string panelXml = "";
                string inverterXml = "";
                if (createJob.BasicDetails.JobType == 1)
                {
                    if (createJob.panel != null && createJob.panel.Count() > 0)
                    {
                        if (createJob.JobSystemDetails != null)
                            createJob.JobSystemDetails.NoOfPanel = 0;

                        panelXml = "<Panels>";
                        foreach (var item in createJob.panel)
                        {
                            panelXml = panelXml + "<panel><Brand>" + HttpUtility.HtmlEncode(item.Brand) + "</Brand><Model>" + HttpUtility.HtmlEncode(item.Model) + "</Model><NoOfPanel>" + item.NoOfPanel + "</NoOfPanel></panel>";
                            if (createJob.JobSystemDetails != null)
                            {
                                createJob.JobSystemDetails.NoOfPanel += item.NoOfPanel;
                            }
                        }
                        panelXml = panelXml + "</Panels>";
                    }
                    if (createJob.inverter != null && createJob.inverter.Count() > 0)
                    {
                        inverterXml = "<Inverters>";
                        foreach (var item in createJob.inverter)
                        {
                            inverterXml = inverterXml + "<inverter><Brand>" + HttpUtility.HtmlEncode(item.Brand) + "</Brand><Model>" + HttpUtility.HtmlEncode(item.Model) + "</Model><Series>" + HttpUtility.HtmlEncode(item.Series) + "</Series></inverter>";
                        }
                        inverterXml = inverterXml + "</Inverters>";
                    }
                }

                if (createJob.BasicDetails.JobID == 0)
                {
                    createJob.Guid = Guid.NewGuid().ToString();
                }

                int isFileUpload = 0;
                if (createJob.JobInstallationDetails.PropertyType == "Commercial" && createJob.BasicDetails.IsGst == true && createJob.BasicDetails.GstFileBase64 != null && createJob.BasicDetails.GSTDocument != null)
                {
                    isFileUpload = GstFileUpload(createJob);
                }
                else
                {
                    createJob.BasicDetails.IsGst = false;
                    createJob.BasicDetails.GstFileBase64 = null;
                    createJob.BasicDetails.GSTDocument = null;
                }

                if (createJob.JobSystemDetails != null)
                {
                    if (createJob.JobSystemDetails.SerialNumbers != null && createJob.JobSystemDetails.SerialNumbers != "")     //change
                    {
                        createJob.JobSystemDetails.SerialNumbers = createJob.JobSystemDetails.SerialNumbers.Replace(",", "\r\n");
                    }
                }

                string calculatSTCUrl = string.Empty;
                if (createJob.BasicDetails.JobType == 1)
                    calculatSTCUrl = ConfigurationManager.AppSettings["CalculateSTCUrl"].ToString();
                else
                    calculatSTCUrl = ConfigurationManager.AppSettings["CalculateSWHSTCUrl"].ToString();
                // Setting by default highest value of Deeming period 
                List<SelectListItem> items = _jobRules.GetDeemingPeriod(Convert.ToDateTime(createJob.BasicDetails.strInstallationDate).Year.ToString());
                createJob.JobSTCDetails.DeemingPeriod = items[items.Count - 1].Value;

                //jobID = _jobRules.InsertCreateJobData(ref createJob, panelXml, inverterXml, (int)userdata.SolarCompanyId, userdata.UserId, calculatSTCUrl);
                KeyValuePair<bool, Int32> keyValue = _jobRules.InsertCreateJobData(ref createJob, panelXml, inverterXml, (int)userdata.SolarCompanyId, userdata.UserId, calculatSTCUrl);
                jobID = keyValue.Value;
                
                // Maintain panel history
                if (panelDataTable != null && panelDataTable.Rows.Count > 0)
                {
                    foreach (var item in createJob.panel)
                    {
                        var listRow = panelDataTable.Select("Brand LIKE '" + item.Brand+"'");
                        if(listRow.Length > 0)
                        {
                            var result = listRow.CopyToDataTable();
                            var resultRow = result.Select("Model LIKE '" + item.Model + "'");
                            if(resultRow.Length > 0)
                            {
                                result = listRow.CopyToDataTable();
                                listRow = result.Select("NoOfPanel = " + item.NoOfPanel);
                                if (listRow.Length == 0)
                                    PanelHistory(item.Brand + "$" + result.Rows[0]["Brand"], item.Model + "$" + result.Rows[0]["Model"], item.NoOfPanel + "$" + result.Rows[0]["NoOfPanel"], jobID, HistoryCategory.PanelUpdated,userdata.UserId);
                            }
                            else
                            {
                                PanelHistory(item.Brand + "$"+listRow[0]["Brand"], item.Model + "$" + listRow[0]["Model"], item.NoOfPanel + "$" + listRow[0]["NoOfPanel"], jobID, HistoryCategory.PanelUpdated, userdata.UserId);
                            }
                            panelDataTable.Select("Brand LIKE '" + item.Brand + "'")[0].Delete();
                            panelDataTable.AcceptChanges();
                        }
                        else
                        {
                            PanelHistory(item.Brand, item.Model, item.NoOfPanel.ToString(), jobID, HistoryCategory.PanelAdded, userdata.UserId);
                        }
                    }
                }
                else
                {
                    foreach (var item in createJob.panel)
                    {
                        PanelHistory(item.Brand, item.Model, item.NoOfPanel.ToString(), jobID, HistoryCategory.PanelAdded, userdata.UserId);
                    }
                }
                foreach(DataRow dr in panelDataTable.Rows)
                {
                    PanelHistory(Convert.ToString(dr["Brand"]), Convert.ToString(dr["Model"]), Convert.ToString(dr["NoOfPanel"]), jobID, HistoryCategory.PanelRemoved, userdata.UserId);
                }
                // Maintain panel history
                if (accreditedInstallersList.Count > 0)
                {
                    installerIdExist = _user.CheckExistAccreditationNumberForInstallerDesigner(createJob.InstallerView.CECAccreditationNumber, (int)userdata.SolarCompanyId);
                    if (installerIdExist > 0)
                    {
                        _user.UpdateJob_InstallerDesignerId(installerIdExist, jobID, 2, false, accreditedInstallersList[0].UserId);
                    }
                    else
                    {
                        createJob.InstallerView = GetInstallerDesignerView(accreditedInstallersList, (int)userdata.SolarCompanyId, userdata.UserId, createJob.BasicDetails.JobType);
                        _user.AddEditInstallerDesigner(createJob.InstallerView, jobID, 2, null, 0, 0);
                    }
                }

                if (accreditedDesignersList.Count > 0)
                {
                    designerIdExist = _user.CheckExistAccreditationNumberForInstallerDesigner(createJob.DesignerView.CECAccreditationNumber, (int)userdata.SolarCompanyId);
                    if (designerIdExist > 0)
                    {
                        _user.UpdateJob_InstallerDesignerId(designerIdExist, jobID, 4, false, accreditedDesignersList[0].UserId);
                    }
                    else
                    {
                        createJob.DesignerView = GetInstallerDesignerView(accreditedDesignersList, (int)userdata.SolarCompanyId, userdata.UserId, createJob.BasicDetails.JobType);
                        _user.AddEditInstallerDesigner(createJob.DesignerView, jobID, 4, null, 0, 0);
                    }
                }

                if (createJob.JobElectricians != null)
                {
                    if (createJob.JobElectricians.JobElectricianID > 0)
                    {
                        _user.UpdateJob_JobElectricianId(jobID, (int)userdata.SolarCompanyId, createJob.JobElectricians.JobElectricianID, createJob.JobElectricians.IsCustomElectrician,userdata.UserId);
                    }
                    else
                    {
                        createJob.JobElectricians.SolarCompanyID = (int)userdata.SolarCompanyId;
                       int jobElectricianId = _job.JobElectricians_InsertUpdate(jobID, 3, createJob.JobElectricians, userdata.UserId, true);
                        _user.UpdateJob_JobElectricianId(jobID, (int)userdata.SolarCompanyId, jobElectricianId, true, userdata.UserId);
                    }
                }

                //AddAccreditedInstallerDesigner(createJob.InstallerView, userdata, createJob.BasicDetails.JobType, jobID, 2);
                //AddAccreditedInstallerDesigner(createJob.DesignerView, userdata, createJob.BasicDetails.JobType, jobID, 4);

                //if (createJob.JobElectricians != null && !createJob.BasicDetails.IsClassic)
                //    _job.JobInstallerDesignerElectricians_InsertUpdate(jobID, 3, createJob.JobElectricians.Signature, createJob.JobElectricians, null, userdata.UserId);

                //if (createJob.InstallerView != null && !string.IsNullOrEmpty(createJob.InstallerView.CECAccreditationNumber))
                //{
                //    createJob.InstallerView.CECAccreditationNumber = createJob.InstallerView.CECAccreditationNumber.Trim();
                //    createJob.InstallerView.SolarCompanyId = (int)userdata.SolarCompanyId;
                //    createJob.InstallerView.CreatedBy = userdata.UserId;
                //    createJob.InstallerView.ModifiedBy = userdata.UserId;

                //    if (createJob.BasicDetails.JobType == 1)
                //    {
                //        int accreditationNumberExist = _user.CheckExistAccreditationNumberForInstallerDesigner(createJob.InstallerView.CECAccreditationNumber, createJob.InstallerView.SolarCompanyId);
                //        string signDocumentsFolder = ProjectSession.ProofDocuments + "\\" + "UserDocuments" + "\\" + userdata.UserId;
                //        string ImagePath = "";
                //        if (!string.IsNullOrEmpty(createJob.InstallerView.SESignature))
                //        {
                //            if (!Directory.Exists(signDocumentsFolder))
                //            {
                //                Directory.CreateDirectory(signDocumentsFolder);
                //            }

                //            ImagePath = Path.Combine(signDocumentsFolder + "\\" + createJob.InstallerView.SESignature.Replace("%", "$"));
                //            if (System.IO.File.Exists(ImagePath))
                //            {
                //                System.IO.File.Delete(ImagePath);
                //            }

                //            byte[] sign = Convert.FromBase64String(createJob.InstallerView.SignBase64);
                //            File.WriteAllBytes(ImagePath, sign);
                //        }

                //        string signPath = !string.IsNullOrEmpty(ImagePath) ? ImagePath : null;
                //        if (!accreditationNumberExist.Equals(0))
                //        {
                //            createJob.InstallerView.IsCECAccreditationNumberExist = true;
                //        }
                //        int InstallerDesignerId = _user.AddEditInstallerDesigner(createJob.InstallerView, jobID, 2, signPath);
                //    }
                //    createJob.InstallerView.CreatedBy = createJob.InstallerView.CreatedBy;
                //}

                if (jobID > 0)
                {
                    InsertJobNotes(createJob, jobID, userdata);
                    if (isFileUpload == 2)
                    {
                        string sourceGSTDir = ProjectSession.ProofDocuments + "\\JobDocuments" + "\\" + createJob.Guid;
                        string destinationGSTDir = ProjectSession.ProofDocuments + "\\JobDocuments" + "\\" + jobID;
                        if (sourceGSTDir != destinationGSTDir)
                        {
                            try
                            {
                                Directory.Move(sourceGSTDir, destinationGSTDir);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }
                    if (createJob.BasicDetails.JobID > 0)
                    {
                        objJobResponse = JobResponse(Convert.ToString(jobID), "Job Updated Successfully");
                    }
                    else
                    {
                        objJobResponse = JobResponse(Convert.ToString(jobID), "Job Created Successfully");
                    }
                }
                else
                {
                    objJobResponse = JobResponse(Convert.ToString(jobID), "Job not created");
                }
            }
            else
            {
                objJobResponse = JobResponse(null, "Installer/Designer with given accreditation number doesn't exist");
            }
            return new OutputJobResponse() { JobId = jobID, obj = objJobResponse };
        }

        /// <summary>
        /// Inserts the job notes.
        /// </summary>
        /// <param name="createJob">The create job.</param>
        /// <param name="JobId">The job identifier.</param>
        /// <param name="userdata">The userdata.</param>
        public void InsertJobNotes(CreateJob createJob, int JobId, User userdata)
        {
            try
            {
                DataTable dtJobNotesField = Common.GetJobNotesDetail();
                if (createJob.lstJobNotes != null && createJob.lstJobNotes.Count() > 0)
                {

                    for (int i = 0; i < createJob.lstJobNotes.Count; i++)
                    {
                        //dtJobNotesField.Rows.Add(new object[] { createJob.BasicDetails.JobID, createJob.lstCustomDetails[i].JobCustomFieldId, createJob.lstCustomDetails[i].FieldValue, userId, DateTime.Now, userId, DateTime.Now, 0 });
                        if (createJob.lstJobNotes[i].Notes != null && !string.IsNullOrEmpty(createJob.lstJobNotes[i].VendorJobNoteId))
                        {
                            dtJobNotesField.Rows.Add(new object[] { JobId, createJob.lstJobNotes[i].Notes, userdata.UserId, DateTime.Now, null, null, createJob.lstJobNotes[i].VendorJobNoteId });
                        }
                    }
                }
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("dtJobNotesField", SqlDbType.Structured, dtJobNotesField));
                sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));

                DataSet dataset = CommonDAL.ExecuteDataSet("[Insert_JobNotes]", sqlParameters.ToArray());
                DataTable dtInsertedJobNotes;
                if (dataset != null && dataset.Tables.Count > 0)
                {
                    dtInsertedJobNotes = dataset.Tables[0];
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void PanelHistory(string Brand,string Model,string NoOfPanel,int JobId, HistoryCategory HistoryId,int UserId)
        {
            PanelCompare objPanelCompare = new PanelCompare();
            objPanelCompare.Brand = Brand;
            objPanelCompare.Model = Model;
            objPanelCompare.Count = NoOfPanel;
            objPanelCompare.JobID = JobId;
            _jobHistory.LogJobHistory<PanelCompare>(objPanelCompare, HistoryId, UserId);
        }

        /// <summary>
        /// Upload Gst file
        /// </summary>
        /// <param name="createJob">The create job.</param>
        /// <returns></returns>
        public int GstFileUpload(CreateJob createJob)
        {
            int IsFileUploaded = 0;
            try
            {
                string proofDocumentsFolder = UtilityValues.ProofDocuments;
                if (createJob.BasicDetails.JobID > 0)
                {
                    proofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\" + "JobDocuments" + "\\" + createJob.BasicDetails.JobID + "\\" + "GST" + "\\");
                }
                else
                {
                    proofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\" + "JobDocuments" + "\\" + createJob.Guid + "\\" + "GST" + "\\");
                }

                if (!Directory.Exists(proofDocumentsFolder))
                {
                    Directory.CreateDirectory(proofDocumentsFolder);
                }
                string path = Path.Combine(proofDocumentsFolder + "\\" + createJob.BasicDetails.GSTDocument.Replace("%", "$"));

                if (System.IO.File.Exists(path))
                {
                    //string orignalFileName = Path.GetFileNameWithoutExtension(path);
                    //string fileExtension = Path.GetExtension(path);
                    //string fileDirectory = Path.GetDirectoryName(path);
                    //int i = 1;
                    //while (true)
                    //{
                    //    string renameFileName = fileDirectory + @"\" + orignalFileName + "(" + i + ")" + fileExtension;
                    //    if (System.IO.File.Exists(renameFileName))
                    //        i++;
                    //    else
                    //    {
                    //        path = renameFileName;
                    //        break;
                    //    }
                    //}
                    File.Delete(path);

                }
                //if (!System.IO.File.Exists(path))
                //{
                Byte[] bytes = Convert.FromBase64String(createJob.BasicDetails.GstFileBase64);
                File.WriteAllBytes(path, bytes);
                createJob.BasicDetails.GSTDocument = Path.GetFileName(path);

                if (createJob.BasicDetails.JobID > 0)
                    IsFileUploaded = 1;
                else
                    IsFileUploaded = 2;
                //IsFileUploaded = 1;
                //}
            }
            catch (Exception ex)
            {

            }
            return IsFileUploaded;
        }

        /// <summary>
        /// Sends the mail when job create.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        public void SendMailWhenJobCreate(int jobID)
        {
            var dsreceiverUsers = _job.GetEmailDetailsByJobID(jobID);
            List<FormBot.Entity.User> rusers = DBClient.DataTableToList<FormBot.Entity.User>(dsreceiverUsers.Tables[0]);
            FormBot.Entity.User receiverUser = rusers.FirstOrDefault();
            if (receiverUser != null)
            {
                EmailInfo emailInfo = new EmailInfo();
                emailInfo.TemplateID = 14;
                emailInfo.FirstName = receiverUser.FirstName;
                emailInfo.LastName = receiverUser.LastName;
                emailInfo.JobName = receiverUser.JobName;
                //_emailService.ComposeAndSendEmail(emailInfo, receiverUser.Email);
            }
        }

        public GetJobsModel GetJobs(string CreatedDate, string FromDate, string ToDate, string RefNumber, User userdata, string VendorJobId, string CompanyABN)
        {
            GetJobsModel objGetJobsModel = new GetJobsModel();
            objGetJobsModel.lstJobData = new List<JobListModel>();

            List<JobListModel> lstJobListModel = new List<JobListModel>();

            if (CompanyABN != null && userdata.UserTypeID != 2)
            {
                objGetJobsModel.Status = false;
                objGetJobsModel.StatusCode = HttpStatusCode.NoContent.ToString();
                objGetJobsModel.Message = "You are not authorized to accesss this method";
                return objGetJobsModel;
            }

            if (CreatedDate != null || FromDate != null || ToDate != null || RefNumber != null || VendorJobId != null || CompanyABN != null)
            {
                if (CreatedDate != null && (FromDate != null || ToDate != null))
                {
                    objGetJobsModel.Status = false;
                    objGetJobsModel.StatusCode = HttpStatusCode.NoContent.ToString();
                    objGetJobsModel.Message = "Please enter valid date";
                }
                if (VendorJobId != null && (CreatedDate != null || FromDate != null || ToDate != null || RefNumber != null || CompanyABN != null))
                {
                    objGetJobsModel.Status = false;
                    objGetJobsModel.StatusCode = HttpStatusCode.NoContent.ToString();
                    objGetJobsModel.Message = "Please enter only VendorJobId.";
                }
                if (CompanyABN != null && (VendorJobId != null || CreatedDate != null || FromDate != null || ToDate != null || RefNumber != null))
                {
                    objGetJobsModel.Status = false;
                    objGetJobsModel.StatusCode = HttpStatusCode.NoContent.ToString();
                    objGetJobsModel.Message = "Please enter only CompanyABN.";
                }
                else
                {
                    lstJobListModel = _job.GetJobList_VendorAPI(CreatedDate, FromDate, ToDate, RefNumber, VendorJobId, Convert.ToString(userdata.SolarCompanyId), CompanyABN, userdata.ResellerID);
                    if (lstJobListModel.Count() > 0)
                    {
                        objGetJobsModel.lstJobData = lstJobListModel;
                        objGetJobsModel.Status = true;
                        objGetJobsModel.StatusCode = HttpStatusCode.OK.ToString();
                        objGetJobsModel.Message = "Jobs found";
                    }
                    else
                    {
                        objGetJobsModel.Status = false;
                        objGetJobsModel.StatusCode = HttpStatusCode.NoContent.ToString();
                        objGetJobsModel.Message = "Jobs not found";
                    }
                }

            }
            else
            {
                objGetJobsModel.Status = false;
                objGetJobsModel.StatusCode = HttpStatusCode.NoContent.ToString();
                objGetJobsModel.Message = "Required parameter not found";
            }

            return objGetJobsModel;

        }

        /// <summary>
        /// Gets the panel.
        /// </summary>
        /// <returns></returns>
        public PanelResponseModel GetPanel()
        {
            PanelResponseModel objPanelResponseModel = new PanelResponseModel();

            List<PanelModel> panels;

            panels = _job.GetPanelData();

            if (panels != null && panels.Count() > 0)
            {
                objPanelResponseModel.panel = panels;
                objPanelResponseModel.Status = true;
                objPanelResponseModel.StatusCode = HttpStatusCode.OK.ToString();
                objPanelResponseModel.Message = "Panel data found";
            }
            else
            {
                objPanelResponseModel.Status = false;
                objPanelResponseModel.StatusCode = HttpStatusCode.NoContent.ToString();
                objPanelResponseModel.Message = "Panel data not found";
            }
            return objPanelResponseModel;
        }

        /// <summary>
        /// Gets the inverter.
        /// </summary>
        /// <returns></returns>
        public InverterResponseModel GetInverter()
        {
            InverterResponseModel objInverterResponseModel = new InverterResponseModel();

            List<Inverter> inverters;

            inverters = _job.GetInverterData();

            if (inverters != null && inverters.Count() > 0)
            {
                objInverterResponseModel.inverter = inverters;
                objInverterResponseModel.Status = true;
                objInverterResponseModel.StatusCode = HttpStatusCode.OK.ToString();
                objInverterResponseModel.Message = "Inverter data found";
            }
            else
            {
                objInverterResponseModel.Status = false;
                objInverterResponseModel.StatusCode = HttpStatusCode.NoContent.ToString();
                objInverterResponseModel.Message = "Inverter data not found";
            }
            return objInverterResponseModel;
        }

        /// <summary>
        /// Gets the system brand.
        /// </summary>
        /// <returns></returns>
        public SystemBrandResponseModel GetSystemBrand()
        {
            SystemBrandResponseModel objSystemBrandResponseModel = new SystemBrandResponseModel();

            List<SystemBrandModel> systemBrand;

            systemBrand = _job.GetSystemBrandData();

            if (systemBrand != null && systemBrand.Count() > 0)
            {
                objSystemBrandResponseModel.systemBrand = systemBrand;
                objSystemBrandResponseModel.Status = true;
                objSystemBrandResponseModel.StatusCode = HttpStatusCode.OK.ToString();
                objSystemBrandResponseModel.Message = "System Brand data found";
            }
            else
            {
                objSystemBrandResponseModel.Status = false;
                objSystemBrandResponseModel.StatusCode = HttpStatusCode.NoContent.ToString();
                objSystemBrandResponseModel.Message = "System Brand data not found";
            }
            return objSystemBrandResponseModel;
        }

        /// <summary>
        /// Uploading photos in job
        /// </summary>
        /// <param name="jobPhotoRequest">The job photo request.</param>
        /// <param name="userdata"></param>
        /// <returns></returns>
        public OutputJobResponse JobPhoto(JobPhotoRequest jobPhotoRequest, User userdata)
        {
            JobResponse objJobPhotoResponse = new JobResponse();
            string VendorJobPhotoId = _job.CheckVendorJobPhotoId_VendorApi(jobPhotoRequest.VendorJobPhotoId, jobPhotoRequest.JobId, jobPhotoRequest.IsClassic);
            if (VendorJobPhotoId != jobPhotoRequest.VendorJobPhotoId)
            {
                byte[] imageByte = Convert.FromBase64String(jobPhotoRequest.ImageBase64);
                MemoryStream ms = new MemoryStream(imageByte);
                Image image = Image.FromStream(ms);
                string ImagePath = "";

                if (FileUploadForJob(image, Convert.ToString(jobPhotoRequest.JobId), jobPhotoRequest.Filename, ref ImagePath, jobPhotoRequest.IsClassic, jobPhotoRequest.Latitude, jobPhotoRequest.Longitude))
                {
                    if (jobPhotoRequest.IsClassic)
                    {
                        _job.InsertPhotoForAPI(jobPhotoRequest.JobId, jobPhotoRequest.Filename, Convert.ToInt32(jobPhotoRequest.PhotoType), Convert.ToInt32(userdata.UserId), jobPhotoRequest.VendorJobPhotoId);
                    }
                    else
                    {
                        if (jobPhotoRequest.PhotoType == 1)
                        {
                            jobPhotoRequest.PhotoType = 2;
                        }
                        else
                        {
                            jobPhotoRequest.PhotoType = 1;
                        }
                        _job.InsertReferencePhoto(jobPhotoRequest.JobId, ImagePath, Convert.ToInt32(userdata.UserId), null, null, jobPhotoRequest.IsDefault, Convert.ToString(jobPhotoRequest.PhotoType), jobPhotoRequest.VendorJobPhotoId, null, jobPhotoRequest.Latitude, jobPhotoRequest.Longitude);
                    }
                    objJobPhotoResponse = JobResponse(Convert.ToString(jobPhotoRequest.JobId), "Photo uploaded successfully");
                }
                else
                {
                    objJobPhotoResponse = JobResponse(null, "Photo not uploaded");
                }
            }
            else
            {
                objJobPhotoResponse = JobResponse(null, "VendorJobPhotoId already exists.");
            }
            return new OutputJobResponse() { JobId = jobPhotoRequest.JobId, obj = objJobPhotoResponse };
        }


        /// <summary>
        /// Uploading document in job
        /// </summary>
        /// <param name="jobDocumentRequest">The job document request.</param>
        /// <returns></returns>
        public OutputJobResponse JobDocument(JobDocumentRequest jobDocumentRequest, User userdata)
        {
            JobResponse objJobDocumentResponse = new JobResponse();

            string VendorJobDocumentId = _job.CheckVendorJobDocumentId_VendorApi(jobDocumentRequest.VendorJobDocumentId, jobDocumentRequest.JobId, jobDocumentRequest.IsClassic);
            if (VendorJobDocumentId != jobDocumentRequest.VendorJobDocumentId)
            {
                string filename = string.Empty;
                int documentId = 0;
                string stage = string.Empty;
                string path = string.Empty;
                string destinationFile = string.Empty;
                string proofDocumentsFolder = Path.Combine(ProjectSession.ProofDocuments + "\\" + "JobDocuments" + "\\" + jobDocumentRequest.JobId + "\\");

                if (jobDocumentRequest.IsClassic)
                {
                    if (jobDocumentRequest.DocumentType == Convert.ToInt32(SystemEnums.DocumentType.OTHER))
                    {
                        objJobDocumentResponse = JobResponse(Convert.ToString(jobDocumentRequest.JobId), "Document type not supported for classic job.");
                        return new OutputJobResponse() { obj = objJobDocumentResponse };
                    }

                    string fileExtension = Path.GetExtension(jobDocumentRequest.DocumentName);
                    if (fileExtension.ToLower() == ".pdf")
                    {
                        stage = "STC";
                        if (jobDocumentRequest.JobType == Convert.ToInt32(SystemEnums.JobType.PVD))
                        {
                            if (jobDocumentRequest.DocumentType == Convert.ToInt32(SystemEnums.DocumentType.CES))
                            {
                                documentId = Convert.ToInt32(SystemEnums.DocumentNameId.cespvd);
                                filename = Convert.ToString((SystemEnums.DocumentNameId)documentId) + ".pdf";
                            }
                            else
                            {
                                documentId = Convert.ToInt32(SystemEnums.DocumentNameId.STC_Assignment_Form);
                                filename = Convert.ToString((SystemEnums.DocumentNameId)documentId) + ".pdf";
                            }
                        }
                        else
                        {
                            if (jobDocumentRequest.DocumentType == Convert.ToInt32(SystemEnums.DocumentType.CES))
                            {
                                documentId = Convert.ToInt32(SystemEnums.DocumentNameId.cessw);
                                filename = Convert.ToString((SystemEnums.DocumentNameId)documentId) + ".pdf";
                            }
                            else
                            {
                                documentId = Convert.ToInt32(SystemEnums.DocumentNameId.EESG_SWH_Form);
                                filename = Convert.ToString((SystemEnums.DocumentNameId)documentId) + ".pdf";
                            }
                        }
                    }
                    else
                    {
                        objJobDocumentResponse = JobResponse(Convert.ToString(jobDocumentRequest.JobId), "Please upload a document with .pdf extension for classic job.");
                        return new OutputJobResponse() { obj = objJobDocumentResponse };
                    }
                }
                else
                {
                    documentId = 0;
                    filename = jobDocumentRequest.DocumentName;
                    string fileExtension = Path.GetExtension(filename);
                    if (jobDocumentRequest.DocumentType == 1)
                    {
                        stage = "CES";
                    }
                    else if (jobDocumentRequest.DocumentType == 2)
                    {
                        if ((fileExtension.ToLower() == ".pdf") || MimeMapping.GetMimeMapping(filename).Contains("image"))
                        {
                            stage = "STC";
                            //filename = "STC_Assignment_Form";
                            //int i = 0;
                            //while (true)
                            //{
                            //    string renameFileName = string.Empty;
                            //    if (i == 0)
                            //    {
                            //        renameFileName = proofDocumentsFolder + "/" + stage + "/" + filename + fileExtension;
                            //    }
                            //    else
                            //    {
                            //        renameFileName = proofDocumentsFolder + "/" + stage + "/" + filename + "(" + i + ")" + fileExtension;
                            //    }
                            //    if (System.IO.File.Exists(renameFileName))
                            //        i++;
                            //    else
                            //    {
                            //        destinationFile = renameFileName;
                            //        break;
                            //    }
                            //}
                            //filename = Path.GetFileName(destinationFile);
                        }
                        else
                        {
                            objJobDocumentResponse = JobResponse(Convert.ToString(jobDocumentRequest.JobId), "Please upload a document with .pdf extension or image file.");
                            return new OutputJobResponse() { obj = objJobDocumentResponse };
                        }
                    }
                    else
                    {
                        stage = "other";
                    }
                }
                path = "JobDocuments\\" + jobDocumentRequest.JobId + "\\" + stage + "\\" + filename.Replace("%", "$");
                if (jobDocumentRequest.JobId > 0)
                {
                    proofDocumentsFolder = proofDocumentsFolder + "\\" + stage + "\\";
                    if (!Directory.Exists(proofDocumentsFolder))
                    {
                        Directory.CreateDirectory(proofDocumentsFolder);
                    }

                    string docPath = Path.Combine(proofDocumentsFolder + "\\" + filename.Replace("%", "$"));

                    if (System.IO.File.Exists(docPath))
                    {
                        System.IO.File.Delete(docPath);
                    }

                    Byte[] bytes = Convert.FromBase64String(jobDocumentRequest.DocumentBase64);
                    File.WriteAllBytes(docPath, bytes);

                    DocumentsView documentsView = new DocumentsView();
                    documentsView.CreatedBy = userdata.UserId;//jobDocumentRequest.UserId;
                    documentsView.CreatedDate = DateTime.Now;
                    documentsView.DocumentId = documentId;
                    documentsView.IsUpload = true;
                    documentsView.JobId = jobDocumentRequest.JobId;
                    documentsView.Path = path;
                    documentsView.Type = Convert.ToString((SystemEnums.DocumentType)jobDocumentRequest.DocumentType);
                    documentsView.VendorJobDocumentId = jobDocumentRequest.VendorJobDocumentId;

                    _job.CreateJobDocuments(documentsView, jobDocumentRequest.IsClassic);

                    objJobDocumentResponse = JobResponse(Convert.ToString(jobDocumentRequest.JobId), "Document uploaded successfully");
                }
                else
                {
                    objJobDocumentResponse = JobResponse(null, "Document not uploaded");
                }
            }
            else
            {
                objJobDocumentResponse = JobResponse(null, "VendorJobDocumentId already exists.");
            }
            return new OutputJobResponse() { JobId = jobDocumentRequest.JobId, obj = objJobDocumentResponse };
        }

        /// <summary>
        /// Delete photo from job
        /// </summary>
        /// <param name="jobPhotoDeleteRequest">The job photo delete request.</param>
        /// <returns></returns>
        public JobResponse JobPhotoDelete(JobPhotoDeleteRequest jobPhotoDeleteRequest)
        {
            JobResponse objJobPhotoDeleteResponse = new JobResponse();

            if (!string.IsNullOrEmpty(jobPhotoDeleteRequest.VendorJobPhotoId))
            {
                _job.DeleteJobImages_VendorApi(jobPhotoDeleteRequest.VendorJobPhotoId, jobPhotoDeleteRequest.IsClassic);
                objJobPhotoDeleteResponse = JobResponse(jobPhotoDeleteRequest.VendorJobPhotoId, "Photo deleted successfully");
            }
            else
            {
                objJobPhotoDeleteResponse = JobResponse(null, "Photo not deleted");
            }

            return objJobPhotoDeleteResponse;
        }

        /// <summary>
        /// Delete document from job.
        /// </summary>
        /// <param name="jobDocumentDeleteRequest">The job document delete request.</param>
        /// <returns></returns>
        public JobResponse JobDocumentDelete(JobDocumentDeleteRequest jobDocumentDeleteRequest)
        {
            JobResponse objJobDocumentDeleteResponse = new JobResponse();
            string destPath = string.Empty;

            if (!string.IsNullOrEmpty(jobDocumentDeleteRequest.VendorJobDocumentId))
            {

                DataSet ds = _job.DeleteDocuments_VendorApi(jobDocumentDeleteRequest.VendorJobDocumentId, jobDocumentDeleteRequest.IsClassic);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = new DataTable();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var filePath = dr["Path"].ToString();
                        string vendorJobDocumentID = dr["VendorJobDocumentId"].ToString();
                        string sourcePath = Path.Combine(ProjectSession.ProofDocumentsURL, filePath);
                        string pathInfo = sourcePath.Split(new string[] { "JobDocuments\\" }, StringSplitOptions.None)[1].ToString();
                        string JobId = Convert.ToString(Directory.GetParent(pathInfo).Parent);
                        //destPath = "JobDocuments\\" + MoveDeletedDocuments(sourcePath);
                        destPath = "JobDocuments\\" + _generateStcReportBAL.MoveDeletedDocuments(sourcePath, JobId);

                        _job.MoveDeletedDocuments_VendorApi(vendorJobDocumentID, destPath);
                    }
                }
                objJobDocumentDeleteResponse = JobResponse(jobDocumentDeleteRequest.VendorJobDocumentId, "Document deleted successfully");
            }
            else
            {
                objJobDocumentDeleteResponse = JobResponse(null, "Document not deleted");
            }

            return objJobDocumentDeleteResponse;
        }

        /// <summary>
        /// Upload file for job.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="ImagePath">The image path.</param>
        /// <param name="isClassic">if set to <c>true</c> [is classic].</param>
        /// <param name="lat">The lat.</param>
        /// <param name="lon">The lon.</param>
        /// <returns></returns>
        public bool FileUploadForJob(Image image, string jobID, string fileName, ref string ImagePath, bool isClassic = true, string lat = "", string lon = "")
        {
            try
            {
                string proofDocumentsFolder = ProjectSession.ProofDocuments;
                string proofDocumentsFolderURL = ProjectSession.ProofDocumentsURL;


                if (!isClassic)
                {
                    proofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\" + "JobDocuments" + "\\" + jobID + "\\DefaultFolder");
                    proofDocumentsFolderURL = proofDocumentsFolderURL + "\\" + "JobDocuments" + "\\" + jobID + "\\DefaultFolder";
                }
                else
                {
                    if (!string.IsNullOrEmpty(jobID))
                    {
                        proofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\" + "JobDocuments" + "\\" + jobID + "\\");
                        proofDocumentsFolderURL = proofDocumentsFolderURL + "\\" + "JobDocuments" + "\\" + jobID + "\\";
                    }
                }

                if (!Directory.Exists(proofDocumentsFolder))
                {
                    Directory.CreateDirectory(proofDocumentsFolder);
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                string path = Path.Combine(proofDocumentsFolder + "\\" + fileName.Replace("%", "$"));
                if (System.IO.File.Exists(path))
                {
                    string orignalFileName = Path.GetFileNameWithoutExtension(path);
                    string fileExtension = Path.GetExtension(path);
                    string fileDirectory = Path.GetDirectoryName(path);
                    int i = 1;
                    while (true)
                    {
                        string renameFileName = fileDirectory + @"\" + orignalFileName + "(" + i + ")" + fileExtension;
                        if (System.IO.File.Exists(renameFileName))
                        {
                            i++;
                        }
                        else
                        {
                            path = renameFileName;
                            break;
                        }

                    }

                    fileName = Path.GetFileName(path);

                }

                Geotag(new Bitmap(image), Double.Parse(string.IsNullOrEmpty(lat) ? "0" : lat), Double.Parse(string.IsNullOrEmpty(lon) ? "0" : lon)).Save(path);

                if (!isClassic)
                {
                    ImagePath = "JobDocuments\\" + jobID + "\\DefaultFolder\\" + fileName.Replace("%", "$");
                }
                else
                {
                    ImagePath = fileName.Replace("%", "$");
                }
                //image.Save(path);
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return false;
            }

        }

        /// <summary>
        /// Geotags the specified original.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <param name="lat">The lat.</param>
        /// <param name="lng">The LNG.</param>
        /// <returns></returns>
        static Image Geotag(Image original, double lat, double lng)
        {
            // These constants come from the CIPA DC-008 standard for EXIF 2.3
            const short ExifTypeByte = 1;
            const short ExifTypeAscii = 2;
            const short ExifTypeRational = 5;

            const int ExifTagGPSVersionID = 0x0000;
            const int ExifTagGPSLatitudeRef = 0x0001;
            const int ExifTagGPSLatitude = 0x0002;
            const int ExifTagGPSLongitudeRef = 0x0003;
            const int ExifTagGPSLongitude = 0x0004;

            char latHemisphere = 'N';
            if (lat < 0)
            {
                latHemisphere = 'S';
                lat = -lat;
            }
            char lngHemisphere = 'E';
            if (lng < 0)
            {
                lngHemisphere = 'W';
                lng = -lng;
            }

            MemoryStream ms = new MemoryStream();
            original.Save(ms, ImageFormat.Jpeg);
            ms.Seek(0, SeekOrigin.Begin);

            Image img = Image.FromStream(ms);
            AddProperty(img, ExifTagGPSVersionID, ExifTypeByte, new byte[] { 2, 3, 0, 0 });
            AddProperty(img, ExifTagGPSLatitudeRef, ExifTypeAscii, new byte[] { (byte)latHemisphere, 0 });
            AddProperty(img, ExifTagGPSLatitude, ExifTypeRational, ConvertToRationalTriplet(lat));
            AddProperty(img, ExifTagGPSLongitudeRef, ExifTypeAscii, new byte[] { (byte)lngHemisphere, 0 });
            AddProperty(img, ExifTagGPSLongitude, ExifTypeRational, ConvertToRationalTriplet(lng));

            return img;
        }

        static void AddProperty(Image img, int id, short type, byte[] value)
        {
            PropertyItem pi = img.PropertyItems[0];
            pi.Id = id;
            pi.Type = type;
            pi.Len = value.Length;
            pi.Value = value;
            img.SetPropertyItem(pi);
        }

        static byte[] ConvertToRationalTriplet(double value)
        {
            int degrees = (int)Math.Floor(value);
            value = (value - degrees) * 60;
            int minutes = (int)Math.Floor(value);
            value = (value - minutes) * 60 * 100;
            int seconds = (int)Math.Round(value);
            byte[] bytes = new byte[3 * 2 * 4]; // Degrees, minutes, and seconds, each with a numerator and a denominator, each composed of 4 bytes
            int i = 0;
            Array.Copy(BitConverter.GetBytes(degrees), 0, bytes, i, 4); i += 4;
            Array.Copy(BitConverter.GetBytes(1), 0, bytes, i, 4); i += 4;
            Array.Copy(BitConverter.GetBytes(minutes), 0, bytes, i, 4); i += 4;
            Array.Copy(BitConverter.GetBytes(1), 0, bytes, i, 4); i += 4;
            Array.Copy(BitConverter.GetBytes(seconds), 0, bytes, i, 4); i += 4;
            Array.Copy(BitConverter.GetBytes(100), 0, bytes, i, 4);
            return bytes;
        }

        /// <summary>
        /// Deletes the directory.
        /// </summary>
        /// <param name="path">The path.</param>
        private void DeleteDirectory(string path)
        {

            try
            {
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                ////Delete all files from the Directory
                System.IO.File.Delete(path);
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
            }

        }

        /// <summary>
        /// Jobs the response.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="msg">The MSG.</param>
        /// <returns></returns>
        public JobResponse JobResponse(string id, string msg)
        {
            JobResponse objJobResponse = new JobResponse();
            if (!string.IsNullOrEmpty(id))
            {
                //objJobResponse.JobId = Convert.ToInt32(id);
                objJobResponse.Status = true;
                objJobResponse.StatusCode = HttpStatusCode.OK.ToString();
            }
            else
            {
                objJobResponse.Status = false;
                objJobResponse.StatusCode = HttpStatusCode.NoContent.ToString();
            }
            objJobResponse.Message = msg;

            return objJobResponse;
        }

        public DataTable GetPanelSystemBrand()
        {
            DataTable dtPanelSystemBrand = new DataTable();
            dtPanelSystemBrand.Columns.Add("Brand", typeof(string));
            dtPanelSystemBrand.Columns.Add("Model", typeof(string));

            return dtPanelSystemBrand;
        }

        public DataTable GetInverterDetails()
        {
            DataTable dtInverter = new DataTable();
            dtInverter.Columns.Add("Brand", typeof(string));
            dtInverter.Columns.Add("Model", typeof(string));
            dtInverter.Columns.Add("Series", typeof(string));

            return dtInverter;
        }

        ///// <summary>
        ///// Moves the deleted documents.
        ///// </summary>
        ///// <param name="path">The path.</param>
        //private string MoveDeletedDocuments(string sourcePath)
        //{
        //    string fileName = Path.GetFileName(sourcePath);
        //    string pathInfo = sourcePath.Split(new string[] { "JobDocuments\\" }, StringSplitOptions.None)[1].ToString();
        //    string JobId = Convert.ToString(Directory.GetParent(pathInfo).Parent);
        //    string destPath = string.Empty;
        //    if (System.IO.File.Exists(sourcePath))
        //    {
        //        string destinationDirectory = ProjectSession.ProofDocumentsURL + "JobDocuments" + "\\" + JobId + "\\" + "DeletedDocuments";

        //        GC.Collect();
        //        GC.WaitForPendingFinalizers();
        //        if (!Directory.Exists(destinationDirectory))
        //        {
        //            Directory.CreateDirectory(destinationDirectory);
        //        }

        //        destPath = System.IO.Path.Combine(destinationDirectory, fileName);
        //        if (System.IO.File.Exists(destPath))
        //        {
        //            string orignalFileName = Path.GetFileNameWithoutExtension(destPath);
        //            string fileExtension = Path.GetExtension(destPath);
        //            string fileDirectory = Path.GetDirectoryName(destPath);
        //            int i = 1;
        //            while (true)
        //            {
        //                string renameFileName = fileDirectory + @"\" + orignalFileName + "(" + i + ")" + fileExtension;
        //                if (System.IO.File.Exists(renameFileName))
        //                    i++;
        //                else
        //                {
        //                    destPath = renameFileName;
        //                    break;
        //                }
        //            }
        //        }
        //        System.IO.File.Copy(sourcePath, destPath, true);
        //        System.IO.File.Delete(sourcePath);
        //    }
        //    if (!string.IsNullOrEmpty(destPath))
        //    {
        //        return (destPath.Split(new string[] { "JobDocuments\\" }, StringSplitOptions.None)[1].ToString());
        //    }
        //    return destPath;
        //}

        public CustomFieldResponseModel GetCustomField(User userdata)
        {
            CustomFieldResponseModel objCustomFieldResponseModel = new CustomFieldResponseModel();

            if (userdata.UserTypeID == 4)
            {
                List<VendorCustomField> customField;
                customField = _job.GetCustomFieldDetails(Convert.ToInt32(userdata.SolarCompanyId));

                if (customField != null && customField.Count() > 0)
                {
                    objCustomFieldResponseModel.CustomField = customField;
                    objCustomFieldResponseModel.Status = true;
                    objCustomFieldResponseModel.StatusCode = HttpStatusCode.OK.ToString();
                    objCustomFieldResponseModel.Message = "Custom Field's found";
                }
                else
                {
                    objCustomFieldResponseModel.Status = false;
                    objCustomFieldResponseModel.StatusCode = HttpStatusCode.NoContent.ToString();
                    objCustomFieldResponseModel.Message = "No Custom Field found";
                }
            }
            else
            {
                objCustomFieldResponseModel.Status = false;
                objCustomFieldResponseModel.StatusCode = HttpStatusCode.NoContent.ToString();
                objCustomFieldResponseModel.Message = "You are not authorized to access this method.";
            }
            return objCustomFieldResponseModel;
        }

        //public void AddAccreditedInstallerDesigner(InstallerDesignerView installerDesignerView, User userdata, int JobType, int jobID, int profileType)
        //{
        //    if (installerDesignerView != null && !string.IsNullOrEmpty(installerDesignerView.CECAccreditationNumber))
        //    {
        //        installerDesignerView.CECAccreditationNumber = installerDesignerView.CECAccreditationNumber.Trim();
        //        installerDesignerView.SolarCompanyId = (int)userdata.SolarCompanyId;
        //        installerDesignerView.CreatedBy = userdata.UserId;
        //        installerDesignerView.ModifiedBy = userdata.UserId;

        //        if (JobType == 1)
        //        {
        //            int accreditationNumberExist = _user.CheckExistAccreditationNumberForInstallerDesigner(installerDesignerView.CECAccreditationNumber, installerDesignerView.SolarCompanyId);
        //            string signDocumentsFolder = ProjectSession.ProofDocuments + "\\" + "UserDocuments" + "\\" + userdata.UserId;
        //            string ImagePath = "";
        //            if (!string.IsNullOrEmpty(installerDesignerView.SESignature))
        //            {
        //                if (!Directory.Exists(signDocumentsFolder))
        //                {
        //                    Directory.CreateDirectory(signDocumentsFolder);
        //                }

        //                ImagePath = Path.Combine(signDocumentsFolder + "\\" + installerDesignerView.SESignature.Replace("%", "$"));
        //                if (System.IO.File.Exists(ImagePath))
        //                {
        //                    System.IO.File.Delete(ImagePath);
        //                }

        //                byte[] sign = Convert.FromBase64String(installerDesignerView.SignBase64);
        //                File.WriteAllBytes(ImagePath, sign);
        //            }

        //            string signPath = !string.IsNullOrEmpty(ImagePath) ? ImagePath : null;
        //            if (!accreditationNumberExist.Equals(0))
        //            {
        //                installerDesignerView.IsCECAccreditationNumberExist = true;
        //            }
        //            int InstallerDesignerId = _user.AddEditInstallerDesigner(installerDesignerView, jobID, profileType, signPath);
        //        }
        //        installerDesignerView.CreatedBy = installerDesignerView.CreatedBy;
        //    }
        //}

        public InstallerDesignerView GetInstallerDesignerView(List<AccreditedInstallers> accreditedInstallerDesignerList, int solarCompanyId, int userId, int jobType)
        {
            InstallerDesignerView installerDesignerView = new InstallerDesignerView();

            int roleId = 0;
            if (!string.IsNullOrEmpty(accreditedInstallerDesignerList[0].GridType))
            {
                if (accreditedInstallerDesignerList[0].GridType.ToLower() == "install only")
                    roleId = Convert.ToInt32(FormBot.Helper.SystemEnums.SEDesignRole.Install);
                else if (accreditedInstallerDesignerList[0].GridType.ToLower() == "design only")
                    roleId = Convert.ToInt32(FormBot.Helper.SystemEnums.SEDesignRole.Design);
                else if (accreditedInstallerDesignerList[0].GridType.ToLower() == "design & supervise" || accreditedInstallerDesignerList[0].GridType.ToLower() == "design & install")
                    roleId = Convert.ToInt32(FormBot.Helper.SystemEnums.SEDesignRole.Design_Install);
                //else if (accreditedInstallerDesignerList[0].GridType.ToLower() == "design & install")
                //    roleId = Convert.ToInt32(FormBot.Helper.SystemEnums.SEDesignRole.Design_Install);
                else
                    roleId = 0;
            }
            accreditedInstallerDesignerList[0].RoleId = roleId;
            accreditedInstallerDesignerList[0].Phone = Regex.Replace(accreditedInstallerDesignerList[0].Phone, "[^.0-9]", "");
            accreditedInstallerDesignerList[0].Inst_Phone = Regex.Replace(accreditedInstallerDesignerList[0].Inst_Phone, "[^.0-9]", "");

            installerDesignerView.InstallerDesignerId = 0;
            installerDesignerView.FirstName = accreditedInstallerDesignerList[0].FirstName;
            installerDesignerView.LastName = accreditedInstallerDesignerList[0].LastName;
            installerDesignerView.Email = accreditedInstallerDesignerList[0].Email;
            installerDesignerView.Phone = accreditedInstallerDesignerList[0].Inst_Phone;
            installerDesignerView.Mobile = accreditedInstallerDesignerList[0].Inst_Mobile;
            installerDesignerView.CECAccreditationNumber = accreditedInstallerDesignerList[0].AccreditationNumber;
            installerDesignerView.ElectricalContractorsLicenseNumber = accreditedInstallerDesignerList[0].LicensedElectricianNumber;
            installerDesignerView.SEDesignRoleId = accreditedInstallerDesignerList[0].RoleId;
            installerDesignerView.UnitTypeID = accreditedInstallerDesignerList[0].Inst_UnitTypeID;
            installerDesignerView.UnitNumber = accreditedInstallerDesignerList[0].MailingAddressUnitNumber;
            installerDesignerView.StreetNumber = accreditedInstallerDesignerList[0].MailingAddressStreetNumber;
            installerDesignerView.StreetName = accreditedInstallerDesignerList[0].MailingAddressStreetName;
            installerDesignerView.StreetTypeID = accreditedInstallerDesignerList[0].StreetTypeID;
            installerDesignerView.PostalAddressID = 0;
            installerDesignerView.Town = accreditedInstallerDesignerList[0].MailingCity;
            installerDesignerView.State = accreditedInstallerDesignerList[0].Abbreviation;
            installerDesignerView.PostCode = accreditedInstallerDesignerList[0].PostalCode;
            installerDesignerView.IsPVDUser = jobType == 1 ? true : false;
            installerDesignerView.SolarCompanyId = solarCompanyId;
            //installerDesignerView.UserId = accreditedInstallerDesignerList[0].UserId;
            installerDesignerView.CreatedBy = userId;

            return installerDesignerView;
        }

        public GetStcSubmissionModel GetStcSubmission(string VendorJobId)
        {
            GetStcSubmissionModel objGetStcSubmissionModel = new GetStcSubmissionModel();
            objGetStcSubmissionModel.lstStcSubmissionData = new List<FormBot.Entity.Job.STCBasicDetails.STCSubmissionModel>();

            List<FormBot.Entity.Job.STCBasicDetails.STCSubmissionModel> lstStcSubmissionListModel = new List<FormBot.Entity.Job.STCBasicDetails.STCSubmissionModel>();

            if (VendorJobId != null)
            {
                lstStcSubmissionListModel = _job.GetStcSubmissionList_VendorAPI(VendorJobId);
                if (lstStcSubmissionListModel.Count() > 0)
                {
                    objGetStcSubmissionModel.lstStcSubmissionData = lstStcSubmissionListModel;
                    objGetStcSubmissionModel.Status = true;
                    objGetStcSubmissionModel.StatusCode = HttpStatusCode.OK.ToString();
                    objGetStcSubmissionModel.Message = "Stc data found";
                }
                else
                {
                    objGetStcSubmissionModel.Status = false;
                    objGetStcSubmissionModel.StatusCode = HttpStatusCode.NoContent.ToString();
                    objGetStcSubmissionModel.Message = "Stc data not found";
                }
            }
            else
            {
                objGetStcSubmissionModel.Status = false;
                objGetStcSubmissionModel.StatusCode = HttpStatusCode.NoContent.ToString();
                objGetStcSubmissionModel.Message = "Required parameter not found";
            }

            return objGetStcSubmissionModel;

        }

        //public GetJobPhotosModel GetJobPhotos(string VendorJobId)
        //{
        //    GetJobPhotosModel objGetJobPhotosModel = new GetJobPhotosModel();
        //    objGetJobPhotosModel.lstJobPhoto = new List<VendorJobPhotoList>();

        //    List<VendorJobPhotoList> lstJobPhotosModel = new List<VendorJobPhotoList>();

        //    if (VendorJobId != null)
        //    {
        //        lstJobPhotosModel = _job.GetJobPhoto_VendorAPI(VendorJobId);
        //        if (lstJobPhotosModel.Count() > 0)
        //        {
        //            foreach (var item in lstJobPhotosModel)
        //            {
        //                string path = Path.Combine(ProjectSession.ProofDocuments, item.Filename);
        //                using (Image image = Image.FromFile(path))
        //                {
        //                    using (MemoryStream m = new MemoryStream())
        //                    {
        //                        image.Save(m, image.RawFormat);
        //                        byte[] imageBytes = m.ToArray();

        //                        // Convert byte[] to Base64 String
        //                        string base64String = Convert.ToBase64String(imageBytes);
        //                        item.ImageBase64 = base64String;
        //                    }
        //                }

        //                if (item.Filename.Contains("\\"))
        //                {
        //                    item.Filename = item.Filename.Split('\\')[3];
        //                }
        //            }

        //            objGetJobPhotosModel.lstJobPhoto = lstJobPhotosModel;
        //            objGetJobPhotosModel.Status = true;
        //            objGetJobPhotosModel.StatusCode = HttpStatusCode.OK.ToString();
        //            objGetJobPhotosModel.Message = "Job photo's found";
        //        }
        //        else
        //        {
        //            objGetJobPhotosModel.Status = false;
        //            objGetJobPhotosModel.StatusCode = HttpStatusCode.NoContent.ToString();
        //            objGetJobPhotosModel.Message = "Job photo's not found";
        //        }
        //    }
        //    else
        //    {
        //        objGetJobPhotosModel.Status = false;
        //        objGetJobPhotosModel.StatusCode = HttpStatusCode.NoContent.ToString();
        //        objGetJobPhotosModel.Message = "Required parameter not found";
        //    }

        //    return objGetJobPhotosModel;

        //}
    }
}