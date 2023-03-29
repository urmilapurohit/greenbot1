using Dapper;
using FormBot.BAL.Service.CommonRules;
using FormBot.DAL;
using FormBot.Entity;
using FormBot.Entity.CheckList;
using FormBot.Entity.Documents;
using FormBot.Entity.Email;
using FormBot.Entity.Job;
using FormBot.Entity.KendoGrid;
using FormBot.Entity.Pdf;
using FormBot.Entity.Settings;
using FormBot.Entity.SolarElectrician;
using FormBot.Entity.SPV;
using FormBot.Helper;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FormBot.BAL.Service
{
    public class CreateJobBAL : ICreateJobBAL
    {
        /// <summary>
        /// Gets the se user.
        /// </summary>
        /// <param name="isInstaller">if set to <c>true</c> [is installer].</param>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="existUserId">The exist user identifier.</param>
        /// <returns>Solar Electrician View</returns>
        public List<SolarElectricianView> GetSEUser(bool isInstaller, int companyId, int existUserId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("IsInstaller", SqlDbType.Bit, isInstaller));
            sqlParameters.Add(DBClient.AddParameters("CompanyId", SqlDbType.Int, companyId));
            sqlParameters.Add(DBClient.AddParameters("ExistUserId", SqlDbType.Int, existUserId));
            List<SolarElectricianView> lstSolarElectrician = CommonDAL.ExecuteProcedure<SolarElectricianView>("Job_GetSEUser", sqlParameters.ToArray()).ToList();
            return lstSolarElectrician;
        }
        /// <summary>
        /// Gets the se user With Status.
        /// </summary>
        /// <param name="isInstaller">if set to <c>true</c> [is installer].</param>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="existUserId">The exist user identifier.</param>
        /// <returns>Solar Electrician View</returns>
        public List<SolarElectricianView> GetSEUserWithStatus(bool isInstaller, int companyId, int existUserId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("IsInstaller", SqlDbType.Bit, isInstaller));
            sqlParameters.Add(DBClient.AddParameters("CompanyId", SqlDbType.Int, companyId));
            sqlParameters.Add(DBClient.AddParameters("ExistUserId", SqlDbType.Int, existUserId));
            List<SolarElectricianView> lstSolarElectrician = CommonDAL.ExecuteProcedure<SolarElectricianView>("Job_GetSEUserWithStatus", sqlParameters.ToArray()).ToList();
            return lstSolarElectrician;
        }
        /// <summary>
        /// Gets the job stage.
        /// </summary>
        /// <returns>
        /// list of job stage
        /// </returns>
        public List<JobStage> GetJobStage()
        {
            List<JobStage> lstJobStage = CommonDAL.ExecuteProcedure<JobStage>("Job_GetJobSatge").ToList();
            return lstJobStage;
        }

        /// <summary>
        /// Gets the STC job stage.
        /// </summary>
        /// <returns>List</returns>
        public List<JobStage> GetSTCJobStage()
        {
            List<JobStage> lstJobStage = CommonDAL.ExecuteProcedure<JobStage>("Job_GetSTCJobSatge").ToList();
            return lstJobStage;
        }

        /// <summary>
        /// Gets the panel.
        /// </summary>
        /// <param name="Mode">The mode.</param>
        /// <param name="CertificateHolder">The certificate holder.</param>
        /// <param name="JobType">Type of the job.</param>
        /// <returns>
        /// job panel details
        /// </returns>
        public List<JobPanelDetails> GetPanel(string Mode, string CertificateHolder, string JobType)
        {
            string spName = "[Job_GetPanelBrandModelNumber]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Mode", SqlDbType.NVarChar, Mode));
            sqlParameters.Add(DBClient.AddParameters("CertificateHolder", SqlDbType.NVarChar, CertificateHolder));
            sqlParameters.Add(DBClient.AddParameters("JobType", SqlDbType.Int, Convert.ToInt32(JobType)));
            IList<JobPanelDetails> PanelList = CommonDAL.ExecuteProcedure<JobPanelDetails>(spName, sqlParameters.ToArray());
            return PanelList.ToList();
        }

        /// <summary>
        /// Gets the panel data.
        /// </summary>
        /// <returns></returns>
        public List<PanelModel> GetPanelData()
        {
            string spName = "[Job_GetPanelData]";

            IList<PanelModel> PanelList = CommonDAL.ExecuteProcedure<PanelModel>(spName, null);
            return PanelList.ToList();
        }

        /// <summary>
        /// Gets the inverter data.
        /// </summary>
        /// <returns></returns>
        public List<Inverter> GetInverterData()
        {
            string spName = "[Job_GetInverterData]";

            IList<Inverter> InverterList = CommonDAL.ExecuteProcedure<Inverter>(spName, null);
            return InverterList.ToList();
        }

        /// <summary>
        /// Gets the system brand data.
        /// </summary>
        /// <returns></returns>
        public List<SystemBrandModel> GetSystemBrandData()
        {
            string spName = "[Job_GetSystemBrandData]";

            IList<SystemBrandModel> SystemBrandList = CommonDAL.ExecuteProcedure<SystemBrandModel>(spName, null);
            return SystemBrandList.ToList();
        }

        /// <summary>
        /// Gets the job list vendor API.
        /// </summary>
        /// <param name="CreatedDate">The created date.</param>
        /// <param name="FromDate">From date.</param>
        /// <param name="ToDate">To date.</param>
        /// <param name="RefNumber">The reference number.</param>
        /// <param name="VendorJobId">The vendor job identifier.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <param name="CompanyABN">The company abn.</param>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <returns></returns>
        public List<JobListModel> GetJobList_VendorAPI(string CreatedDate, string FromDate, string ToDate, string RefNumber, string VendorJobId, string SolarCompanyId, string CompanyABN, int? ResellerId)
        {
            string spName = "[GetJobList_VendorAPI]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            if (CreatedDate != null) { sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.ParseExact(CreatedDate, "dd/MM/yyyy", null))); }
            if (FromDate != null) { sqlParameters.Add(DBClient.AddParameters("FromDate", SqlDbType.DateTime, DateTime.ParseExact(FromDate, "dd/MM/yyyy", null))); }
            if (ToDate != null) { sqlParameters.Add(DBClient.AddParameters("ToDate", SqlDbType.DateTime, DateTime.ParseExact(ToDate, "dd/MM/yyyy", null))); }
            if (RefNumber != null) { sqlParameters.Add(DBClient.AddParameters("RefNumber", SqlDbType.NVarChar, RefNumber)); }
            if (SolarCompanyId != null) { sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.NVarChar, SolarCompanyId)); }
            if (ResellerId != null) { sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId)); }
            if (VendorJobId != null) { sqlParameters.Add(DBClient.AddParameters("VendorJobId", SqlDbType.NVarChar, VendorJobId)); }
            if (CompanyABN != null) { sqlParameters.Add(DBClient.AddParameters("CompanyABN", SqlDbType.NVarChar, CompanyABN)); }

            DataSet dataSet = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());

            List<JobListModel> lstJobList = new List<JobListModel>();


            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                foreach (var item in dataSet.Tables[0].Rows)
                {
                    DataRow dr = (DataRow)item;
                    int JobID = Convert.ToInt32(dr["JobID"].ToString());
                    DataTable dtBasicDetails, dtJobElectricians, dtJobInstallationDetails, dtJobSTCDetails, dtJobSystemDetails, dtJobOwnerDetails, dtJobInstallerDetails, dtInstallerView, dtDesignerView, dtJobInverterDetails, dtJobPanelDetails, dtCustomDetails;

                    dtBasicDetails = dataSet.Tables[0].Select("JobID = " + JobID).Count() > 0 ? dataSet.Tables[0].Select("JobID = " + JobID).CopyToDataTable() : new DataTable();
                    dtJobElectricians = dataSet.Tables[1].Select("JobID = " + JobID).Count() > 0 ? dataSet.Tables[1].Select("JobID = " + JobID).CopyToDataTable() : new DataTable();
                    dtJobInstallationDetails = dataSet.Tables[2].Select("JobID = " + JobID).Count() > 0 ? dataSet.Tables[2].Select("JobID = " + JobID).CopyToDataTable() : new DataTable();
                    dtJobSTCDetails = dataSet.Tables[3].Select("JobID = " + JobID).Count() > 0 ? dataSet.Tables[3].Select("JobId = " + JobID).CopyToDataTable() : new DataTable();
                    dtJobSystemDetails = dataSet.Tables[4].Select("JobID = " + JobID).Count() > 0 ? dataSet.Tables[4].Select("JobID = " + JobID).CopyToDataTable() : new DataTable();

                    dtJobInverterDetails = dataSet.Tables[5].Select("JobID = " + JobID).Count() > 0 ? dataSet.Tables[5].Select("JobID = " + JobID).CopyToDataTable() : new DataTable();
                    dtJobPanelDetails = dataSet.Tables[6].Select("JobID = " + JobID).Count() > 0 ? dataSet.Tables[6].Select("JobID = " + JobID).CopyToDataTable() : new DataTable();

                    dtJobOwnerDetails = dataSet.Tables[7].Select("JobID = " + JobID).Count() > 0 ? dataSet.Tables[7].Select("JobID = " + JobID).CopyToDataTable() : new DataTable();
                    dtJobInstallerDetails = dataSet.Tables[8].Select("JobID = " + JobID).Count() > 0 ? dataSet.Tables[8].Select("JobID = " + JobID).CopyToDataTable() : new DataTable();
                    dtInstallerView = dataSet.Tables[9].Select("JobID = " + JobID).Count() > 0 ? dataSet.Tables[9].Select("jobid = " + JobID).CopyToDataTable() : new DataTable();
                    dtDesignerView = dataSet.Tables[10].Select("JobID = " + JobID).Count() > 0 ? dataSet.Tables[10].Select("jobid = " + JobID).CopyToDataTable() : new DataTable();


                    //if(drJobInverterDetails.Count() > 0)
                    //{
                    //    dtJobInverterDetails = new DataTable();
                    //    foreach(var datarow in drJobInverterDetails)
                    //    {
                    //        dtJobInverterDetails.Rows.Add(datarow);
                    //    }
                    //}
                    //if (drJobPanelDetails.Count() > 0)
                    //{
                    //    dtJobPanelDetails = new DataTable();
                    //    foreach (var datarow in drJobPanelDetails)
                    //    {
                    //        dtJobPanelDetails.Rows.Add(datarow);
                    //    }
                    //}

                    JobListModel objJobListModel = new JobListModel();

                    objJobListModel.BasicDetails = dtBasicDetails != null && dtBasicDetails.Rows.Count > 0 ? DBClient.DataTableToList<BasicDetailsVendorAPI>(dtBasicDetails)[0] : new BasicDetailsVendorAPI();
                    objJobListModel.JobElectricians = dtJobElectricians != null && dtJobElectricians.Rows.Count > 0 ? DBClient.DataTableToList<JobElectriciansVendorAPI>(dtJobElectricians)[0] : new JobElectriciansVendorAPI();
                    if (objJobListModel.JobElectricians.IsPostalAddress == false) { objJobListModel.JobElectricians.AddressID = 1; }
                    else if (objJobListModel.JobElectricians.IsPostalAddress == true) { objJobListModel.JobElectricians.AddressID = 2; }

                    objJobListModel.JobInstallationDetails = dtJobInstallationDetails != null && dtJobInstallationDetails.Rows.Count > 0 ? DBClient.DataTableToList<JobInstallationDetails>(dtJobInstallationDetails)[0] : new JobInstallationDetails();
                    if (objJobListModel.JobInstallationDetails.IsPostalAddress == false) { objJobListModel.JobInstallationDetails.AddressID = 1; }
                    else if (objJobListModel.JobInstallationDetails.IsPostalAddress == true) { objJobListModel.JobInstallationDetails.AddressID = 2; }

                    objJobListModel.JobSTCDetails = dtJobSTCDetails != null && dtJobSTCDetails.Rows.Count > 0 ? DBClient.DataTableToList<JobSTCDetailsVendorAPI>(dtJobSTCDetails)[0] : new JobSTCDetailsVendorAPI();
                    objJobListModel.JobSystemDetails = dtJobSystemDetails != null && dtJobSystemDetails.Rows.Count > 0 ? DBClient.DataTableToList<JobSystemDetailsVendorAPI>(dtJobSystemDetails)[0] : new JobSystemDetailsVendorAPI();

                    objJobListModel.lstJobInverterDetails = dtJobInverterDetails != null && dtJobInverterDetails.Rows.Count > 0 ? DBClient.DataTableToList<JobInverterDetails>(dtJobInverterDetails) : new List<JobInverterDetails>();
                    objJobListModel.lstJobPanelDetails = dtJobPanelDetails != null && dtJobPanelDetails.Rows.Count > 0 ? DBClient.DataTableToList<JobPanelDetails>(dtJobPanelDetails) : new List<JobPanelDetails>();

                    objJobListModel.JobOwnerDetails = dtJobOwnerDetails != null && dtJobOwnerDetails.Rows.Count > 0 ? DBClient.DataTableToList<JobOwnerDetailsVendorAPI>(dtJobOwnerDetails)[0] : new JobOwnerDetailsVendorAPI();
                    if (objJobListModel.JobOwnerDetails.IsPostalAddress == false) { objJobListModel.JobOwnerDetails.AddressID = 1; }
                    else if (objJobListModel.JobOwnerDetails.IsPostalAddress == true) { objJobListModel.JobOwnerDetails.AddressID = 2; }

                    objJobListModel.JobInstallerDetails = dtJobInstallerDetails != null && dtJobInstallerDetails.Rows.Count > 0 ? DBClient.DataTableToList<JobInstallerDetails>(dtJobInstallerDetails)[0] : new JobInstallerDetails();
                    if (objJobListModel.JobInstallerDetails.IsPostalAddress == false) { objJobListModel.JobInstallerDetails.AddressID = 1; }
                    else if (objJobListModel.JobInstallerDetails.IsPostalAddress == true) { objJobListModel.JobInstallerDetails.AddressID = 2; }

                    if (objJobListModel.BasicDetails.JobType == 2 && objJobListModel.lstJobPanelDetails.Count > 0)
                    {
                        objJobListModel.JobSystemDetails.SystemBrand = objJobListModel.lstJobPanelDetails[0].Brand;
                        objJobListModel.JobSystemDetails.SystemModel = objJobListModel.lstJobPanelDetails[0].Model;
                        objJobListModel.JobSystemDetails.NoOfPanel = objJobListModel.lstJobPanelDetails[0].NoOfPanel;
                    }


                    //objJobListModel.InstallerView = dtInstallerView != null && dtInstallerView.Rows.Count > 0 ? DBClient.DataTableToList<InstallerDesignerViewVendorAPI>(dtInstallerView)[0] : new InstallerDesignerViewVendorAPI();
                    //if (objJobListModel.InstallerView.IsPostalAddress == false) { objJobListModel.InstallerView.AddressID = 1; }
                    //else if (objJobListModel.InstallerView.IsPostalAddress == true) { objJobListModel.InstallerView.AddressID = 2; }

                    //objJobListModel.DesignerView = dtDesignerView != null && dtDesignerView.Rows.Count > 0 ? DBClient.DataTableToList<InstallerDesignerViewVendorAPI>(dtDesignerView)[0] : new InstallerDesignerViewVendorAPI();
                    //if (objJobListModel.DesignerView.IsPostalAddress == false) { objJobListModel.DesignerView.AddressID = 1; }
                    //else if (objJobListModel.DesignerView.IsPostalAddress == true) { objJobListModel.DesignerView.AddressID = 2; }

                    //STCBasicDetails stcBasicDetails = CommonDAL.ExecuteProcedure<STCBasicDetails>("GetStcBasicDetailsWithStatus", sqlParameters.ToArray()).FirstOrDefault();
                    if (!objJobListModel.BasicDetails.IsClassic)
                    {
                        dtCustomDetails = dataSet.Tables[11].Select("jobId = " + JobID).Count() > 0 ? dataSet.Tables[11].Select("jobId = " + JobID).CopyToDataTable() : new DataTable();
                        List<CustomDetail> lstCustomDetails = dtCustomDetails != null && dtCustomDetails.Rows.Count > 0 ? dtCustomDetails.ToListof<CustomDetail>() : new List<CustomDetail>();

                        objJobListModel.lstCustomDetails = lstCustomDetails;
                    }

                    objJobListModel.STCStatus = dataSet.Tables[12].Select("JobID = " + JobID).Count() > 0 ? dataSet.Tables[12].Select("JobID = " + JobID)[0][0].ToString() : "";

                    lstJobList.Add(objJobListModel);
                }



            }


            return lstJobList;
        }

        /// <summary>
        /// Gets the hw panel.
        /// </summary>
        /// <param name="Mode">The mode.</param>
        /// <param name="CertificateHolder">The certificate holder.</param>
        /// <param name="JobType">Type of the job.</param>
        /// <returns>model list</returns>
        public List<HWBrandModel> GetHWPanel(string Mode, string CertificateHolder, string JobType)
        {
            string spName = "[Job_GetPanelBrandModelNumber]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Mode", SqlDbType.NVarChar, Mode));
            sqlParameters.Add(DBClient.AddParameters("CertificateHolder", SqlDbType.NVarChar, CertificateHolder));
            sqlParameters.Add(DBClient.AddParameters("JobType", SqlDbType.Int, Convert.ToInt32(JobType)));
            IList<HWBrandModel> HWList = CommonDAL.ExecuteProcedure<HWBrandModel>(spName, sqlParameters.ToArray());
            return HWList.ToList();
        }

        /// <summary>
        /// Bind Inverter Dropdowns.
        /// </summary>
        /// <param name="Mode">Mode</param>
        /// <param name="Search">Search</param>
        /// <param name="menufacturer">menufacturer</param>
        /// <returns>Job Inverter Details</returns>
        public List<JobInverterDetails> GetJobInverter(string Mode, string Search, string menufacturer = null)
        {
            string spName = "[Job_GetInverter]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Mode", SqlDbType.NVarChar, Mode));
            sqlParameters.Add(DBClient.AddParameters("Search", SqlDbType.NVarChar, Search));
            sqlParameters.Add(DBClient.AddParameters("Manufacturer", SqlDbType.NVarChar, menufacturer));
            IList<JobInverterDetails> inverterList = CommonDAL.ExecuteProcedure<JobInverterDetails>(spName, sqlParameters.ToArray());
            return inverterList.ToList();
        }

        /// <summary>
        /// Gets the electricity provider.
        /// </summary>
        /// <returns>
        /// list of electricity provider
        /// </returns>
        public List<ElectricityProvider> GetElectricityProvider()
        {
            List<ElectricityProvider> lstElectricityProvider = CommonDAL.ExecuteProcedure<ElectricityProvider>("Job_GetElectricityProvider").ToList();
            return lstElectricityProvider;
        }



        /// <summary>
        /// Inserts the job.
        /// </summary>
        /// <param name="createJob">The create job.</param>
        /// <param name="xmlPanels">The XML panels.</param>
        /// <param name="xmlInverters">The XML inverters.</param>
        /// <returns>
        /// integer job id
        /// </returns>
        public int InsertJob(CreateJob createJob, string xmlPanels, string xmlInverters, int SolarCompanyId, int LoggedInUserId, DataTable dtCustomField = null)
        {
            //try
            //{
            DataSet dataSet = CreateJobNumber(createJob.BasicDetails.JobType, SolarCompanyId);//ProjectSession.SolarCompanyId);
            if (dataSet != null && dataSet.Tables.Count > 2)
            {
                createJob.BasicDetails.CompanyCounter = Convert.ToInt32(dataSet.Tables[1].Rows[0].ItemArray[0].ToString());
                createJob.BasicDetails.CompanyName = dataSet.Tables[2].Rows[0].ItemArray[0].ToString();
                createJob.BasicDetails.ShortCompanyName = dataSet.Tables[2].Rows[0].ItemArray[1].ToString();
            }

            List<SqlParameter> sqlParameters = new List<SqlParameter>();


            //if (createJob.BasicDetails.IsGst == false)
            //    createJob.BasicDetails.GSTDocument = null;
            #region parameters
            sqlParameters.Add(DBClient.AddParameters("IsGst", SqlDbType.Bit, createJob.BasicDetails.IsGst));
            sqlParameters.Add(DBClient.AddParameters("GSTDocument", SqlDbType.NVarChar, createJob.BasicDetails.GSTDocument));
            sqlParameters.Add(DBClient.AddParameters("IsClassic", SqlDbType.Bit, createJob.BasicDetails.IsClassic));
            sqlParameters.Add(DBClient.AddParameters("NewlyAddedSerialNumber", SqlDbType.NVarChar, createJob.NewlyAddedSerialNumber));
            sqlParameters.Add(DBClient.AddParameters("DeletedSerialNumber", SqlDbType.NVarChar, createJob.DeletedSerialNumber));
            sqlParameters.Add(DBClient.AddParameters("BasicJobID", SqlDbType.Int, createJob.BasicDetails.JobID));
            sqlParameters.Add(DBClient.AddParameters("RefNumber", SqlDbType.NVarChar, createJob.BasicDetails.RefNumber));
            sqlParameters.Add(DBClient.AddParameters("Title", SqlDbType.NVarChar, createJob.BasicDetails.Title));
            sqlParameters.Add(DBClient.AddParameters("Description", SqlDbType.NVarChar, createJob.BasicDetails.Description));
            sqlParameters.Add(DBClient.AddParameters("JobNumber", SqlDbType.NVarChar, createJob.BasicDetails.JobNumber));
            sqlParameters.Add(DBClient.AddParameters("JobStage", SqlDbType.Int, createJob.BasicDetails.JobStage));
            sqlParameters.Add(DBClient.AddParameters("JobType", SqlDbType.TinyInt, createJob.BasicDetails.JobType));
            sqlParameters.Add(DBClient.AddParameters("InstallerID", SqlDbType.Int, createJob.BasicDetails.InstallerID));
            sqlParameters.Add(DBClient.AddParameters("DesignerID", SqlDbType.Int, createJob.BasicDetails.DesignerID));
            sqlParameters.Add(DBClient.AddParameters("JobElectricianID", SqlDbType.Int, createJob.BasicDetails.JobElectricianID));
            sqlParameters.Add(DBClient.AddParameters("InstallationDate", SqlDbType.DateTime, createJob.BasicDetails.InstallationDate));
            sqlParameters.Add(DBClient.AddParameters("Priority", SqlDbType.NVarChar, createJob.BasicDetails.Priority));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, LoggedInUserId));//ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.Int, SolarCompanyId));//ProjectSession.SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("CompanyName", SqlDbType.NVarChar, createJob.BasicDetails.CompanyName));
            sqlParameters.Add(DBClient.AddParameters("ShortCompanyName", SqlDbType.NVarChar, createJob.BasicDetails.ShortCompanyName));
            sqlParameters.Add(DBClient.AddParameters("CompanyCounter", SqlDbType.Int, createJob.BasicDetails.CompanyCounter));
            sqlParameters.Add(DBClient.AddParameters("SSCID", SqlDbType.Int, createJob.BasicDetails.SSCID));

            if (ProjectSession.UserTypeId == 8)
            {
                createJob.BasicDetails.ScoID = ProjectSession.LoggedInUserId;
            }

            sqlParameters.Add(DBClient.AddParameters("ScoID", SqlDbType.Int, createJob.BasicDetails.ScoID));
            sqlParameters.Add(DBClient.AddParameters("SoldBy", SqlDbType.NVarChar, createJob.BasicDetails.SoldBy));
            sqlParameters.Add(DBClient.AddParameters("SoldByDate", SqlDbType.DateTime, createJob.BasicDetails.SoldByDate));

            sqlParameters.Add(DBClient.AddParameters("EleCompanyName", SqlDbType.NVarChar, (createJob.JobElectricians != null ? createJob.JobElectricians.CompanyName : null)));
            sqlParameters.Add(DBClient.AddParameters("EleFirstName", SqlDbType.NVarChar, (createJob.JobElectricians != null ? createJob.JobElectricians.FirstName : null)));
            sqlParameters.Add(DBClient.AddParameters("EleLastName", SqlDbType.NVarChar, (createJob.JobElectricians != null ? createJob.JobElectricians.LastName : null)));
            sqlParameters.Add(DBClient.AddParameters("EleUnitTypeID", SqlDbType.Int, (createJob.JobElectricians != null ? createJob.JobElectricians.UnitTypeID : null)));
            sqlParameters.Add(DBClient.AddParameters("EleUnitNumber", SqlDbType.NVarChar, (createJob.JobElectricians != null ? createJob.JobElectricians.UnitNumber : null)));
            sqlParameters.Add(DBClient.AddParameters("EleStreetNumber", SqlDbType.NVarChar, (createJob.JobElectricians != null ? createJob.JobElectricians.StreetNumber : null)));
            sqlParameters.Add(DBClient.AddParameters("EleStreetName ", SqlDbType.NVarChar, (createJob.JobElectricians != null ? createJob.JobElectricians.StreetName : null)));
            sqlParameters.Add(DBClient.AddParameters("EleStreetTypeID", SqlDbType.Int, (createJob.JobElectricians != null ? createJob.JobElectricians.StreetTypeID : null)));
            sqlParameters.Add(DBClient.AddParameters("EleTown", SqlDbType.NVarChar, (createJob.JobElectricians != null ? createJob.JobElectricians.Town : null)));
            sqlParameters.Add(DBClient.AddParameters("EleState", SqlDbType.NVarChar, (createJob.JobElectricians != null ? createJob.JobElectricians.State : null)));
            sqlParameters.Add(DBClient.AddParameters("ElePostCode", SqlDbType.NVarChar, (createJob.JobElectricians != null ? createJob.JobElectricians.PostCode : null)));
            sqlParameters.Add(DBClient.AddParameters("ElePhone", SqlDbType.NVarChar, (createJob.JobElectricians != null ? createJob.JobElectricians.Phone : null)));
            sqlParameters.Add(DBClient.AddParameters("EleMobile", SqlDbType.NVarChar, (createJob.JobElectricians != null ? createJob.JobElectricians.Mobile : null)));
            sqlParameters.Add(DBClient.AddParameters("EleEmail", SqlDbType.NVarChar, (createJob.JobElectricians != null ? createJob.JobElectricians.Email : null)));
            sqlParameters.Add(DBClient.AddParameters("EleLicenseNumber", SqlDbType.NVarChar, (createJob.JobElectricians != null ? createJob.JobElectricians.LicenseNumber : null)));
            if (createJob.JobElectricians != null)
            {
                sqlParameters.Add(DBClient.AddParameters("EleIsPostalAddress", SqlDbType.Bit, createJob.JobElectricians.IsPostalAddress));
                sqlParameters.Add(DBClient.AddParameters("ElePostalAddressID", SqlDbType.NVarChar, createJob.JobElectricians.PostalAddressID));
            }
            else
            {
                sqlParameters.Add(DBClient.AddParameters("EleIsPostalAddress", SqlDbType.Bit, null));
                sqlParameters.Add(DBClient.AddParameters("ElePostalAddressID", SqlDbType.NVarChar, null));
            }
            sqlParameters.Add(DBClient.AddParameters("ElePostalDeliveryNumber", SqlDbType.NVarChar, (createJob.JobElectricians != null ? createJob.JobElectricians.PostalDeliveryNumber : null)));
            sqlParameters.Add(DBClient.AddParameters("EleInstallerID", SqlDbType.Int, (createJob.JobElectricians != null ? createJob.JobElectricians.InstallerID : null)));
            sqlParameters.Add(DBClient.AddParameters("ElectricianID", SqlDbType.Int, (createJob.JobElectricians != null ? createJob.JobElectricians.ElectricianID : null)));

            sqlParameters.Add(DBClient.AddParameters("EleSignature", SqlDbType.NVarChar, createJob.Signature));
            sqlParameters.Add(DBClient.AddParameters("EleCreatedBy", SqlDbType.Int, LoggedInUserId));//ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("EleModifiedBy", SqlDbType.Int, LoggedInUserId));//ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("InsCreatedBy", SqlDbType.NVarChar, LoggedInUserId));//ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("Date", SqlDbType.DateTime, DateTime.Now));

            sqlParameters.Add(DBClient.AddParameters("InsUnitTypeID", SqlDbType.Int, createJob.JobInstallationDetails.UnitTypeID));
            sqlParameters.Add(DBClient.AddParameters("InsUnitNumber", SqlDbType.NVarChar, createJob.JobInstallationDetails.UnitNumber));
            sqlParameters.Add(DBClient.AddParameters("InsStreetNumber", SqlDbType.NVarChar, createJob.JobInstallationDetails.StreetNumber));
            sqlParameters.Add(DBClient.AddParameters("InsStreetName", SqlDbType.NVarChar, createJob.JobInstallationDetails.StreetName));
            sqlParameters.Add(DBClient.AddParameters("InsStreetTypeID", SqlDbType.Int, createJob.JobInstallationDetails.StreetTypeID));
            sqlParameters.Add(DBClient.AddParameters("InsTown", SqlDbType.NVarChar, createJob.JobInstallationDetails.Town));
            sqlParameters.Add(DBClient.AddParameters("InsState", SqlDbType.NVarChar, createJob.JobInstallationDetails.State));
            sqlParameters.Add(DBClient.AddParameters("InsPostCode", SqlDbType.NVarChar, createJob.JobInstallationDetails.PostCode));
            sqlParameters.Add(DBClient.AddParameters("InsNMI", SqlDbType.NVarChar, createJob.JobInstallationDetails.NMI));
            sqlParameters.Add(DBClient.AddParameters("InsDistributorID", SqlDbType.NVarChar, createJob.JobInstallationDetails.DistributorID));
            sqlParameters.Add(DBClient.AddParameters("InsPropertyType", SqlDbType.NVarChar, createJob.JobInstallationDetails.PropertyType));
            sqlParameters.Add(DBClient.AddParameters("InsPropertyName", SqlDbType.NVarChar, createJob.JobInstallationDetails.PropertyName));
            sqlParameters.Add(DBClient.AddParameters("InsSingleMultipleStory", SqlDbType.NVarChar, createJob.JobInstallationDetails.SingleMultipleStory));
            sqlParameters.Add(DBClient.AddParameters("InsInstallingNewPanel", SqlDbType.NVarChar, createJob.JobInstallationDetails.InstallingNewPanel));
            sqlParameters.Add(DBClient.AddParameters("InsMeterNumber", SqlDbType.NVarChar, createJob.JobInstallationDetails.MeterNumber));
            sqlParameters.Add(DBClient.AddParameters("InsPhaseProperty", SqlDbType.NVarChar, createJob.JobInstallationDetails.PhaseProperty));
            sqlParameters.Add(DBClient.AddParameters("InsElectricityProviderID ", SqlDbType.Int, createJob.JobInstallationDetails.ElectricityProviderID));
            sqlParameters.Add(DBClient.AddParameters("InsExistingSystem ", SqlDbType.NVarChar, createJob.JobInstallationDetails.ExistingSystem));
            sqlParameters.Add(DBClient.AddParameters("InsExistingSystemSize", SqlDbType.Decimal, createJob.JobInstallationDetails.ExistingSystemSize));
            sqlParameters.Add(DBClient.AddParameters("InsNoOfPanels", SqlDbType.Int, createJob.JobInstallationDetails.NoOfPanels));
            sqlParameters.Add(DBClient.AddParameters("InsSystemLocation", SqlDbType.NVarChar, createJob.JobInstallationDetails.SystemLocation));
            sqlParameters.Add(DBClient.AddParameters("InsPostalDeliveryNumber ", SqlDbType.NVarChar, createJob.JobInstallationDetails.PostalDeliveryNumber));
            sqlParameters.Add(DBClient.AddParameters("InsIsPostalAddress", SqlDbType.Bit, createJob.JobInstallationDetails.IsPostalAddress));
            sqlParameters.Add(DBClient.AddParameters("InsPostalAddressID", SqlDbType.NVarChar, createJob.JobInstallationDetails.PostalAddressID));
            sqlParameters.Add(DBClient.AddParameters("InsLocation", SqlDbType.NVarChar, createJob.JobInstallationDetails.Location));
            sqlParameters.Add(DBClient.AddParameters("InsAdditionalInstallationInformation", SqlDbType.NVarChar, createJob.JobInstallationDetails.AdditionalInstallationInformation));
            sqlParameters.Add(DBClient.AddParameters("InsIsSameAsOwnerAddress", SqlDbType.Bit, createJob.JobInstallationDetails.IsSameAsOwnerAddress));

            sqlParameters.Add(DBClient.AddParameters("InsLatitude", SqlDbType.NVarChar, (createJob.JobInstallationDetails != null ? createJob.JobInstallationDetails.Latitude : null)));
            sqlParameters.Add(DBClient.AddParameters("InsLongitude", SqlDbType.NVarChar, (createJob.JobInstallationDetails != null ? createJob.JobInstallationDetails.Longitude : null)));

            sqlParameters.Add(DBClient.AddParameters("InstallerFirstname", SqlDbType.NVarChar, (createJob.JobInstallerDetails != null ? createJob.JobInstallerDetails.FirstName : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerSurname", SqlDbType.NVarChar, (createJob.JobInstallerDetails != null ? createJob.JobInstallerDetails.Surname : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerPhone", SqlDbType.NVarChar, (createJob.JobInstallerDetails != null ? createJob.JobInstallerDetails.Phone : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerMobile", SqlDbType.NVarChar, (createJob.JobInstallerDetails != null ? createJob.JobInstallerDetails.Mobile : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerEmail", SqlDbType.NVarChar, (createJob.JobInstallerDetails != null ? createJob.JobInstallerDetails.Email : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerUnitTypeID", SqlDbType.Int, (createJob.JobInstallerDetails != null ? createJob.JobInstallerDetails.UnitTypeID : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerUnitNumber", SqlDbType.NVarChar, (createJob.JobInstallerDetails != null ? createJob.JobInstallerDetails.UnitNumber : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerStreetNumber", SqlDbType.NVarChar, (createJob.JobInstallerDetails != null ? createJob.JobInstallerDetails.StreetNumber : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerStreetName", SqlDbType.NVarChar, (createJob.JobInstallerDetails != null ? createJob.JobInstallerDetails.StreetName : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerStreetTypeID", SqlDbType.Int, (createJob.JobInstallerDetails != null ? createJob.JobInstallerDetails.StreetTypeID : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerTown", SqlDbType.NVarChar, (createJob.JobInstallerDetails != null ? createJob.JobInstallerDetails.Town : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerState", SqlDbType.NVarChar, (createJob.JobInstallerDetails != null ? createJob.JobInstallerDetails.State : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerPostCode", SqlDbType.NVarChar, (createJob.JobInstallerDetails != null ? createJob.JobInstallerDetails.PostCode : null)));

            if (createJob.JobInstallerDetails != null)
            {
                sqlParameters.Add(DBClient.AddParameters("InstallerIsPostalAddress", SqlDbType.Bit, createJob.JobInstallerDetails.IsPostalAddress));
                sqlParameters.Add(DBClient.AddParameters("InstallerPostalAddressID", SqlDbType.NVarChar, createJob.JobInstallerDetails.PostalAddressID));
            }
            else
            {
                sqlParameters.Add(DBClient.AddParameters("InstallerIsPostalAddress", SqlDbType.Bit, null));
                sqlParameters.Add(DBClient.AddParameters("InstallerPostalAddressID", SqlDbType.NVarChar, null));
            }
            sqlParameters.Add(DBClient.AddParameters("InstallerPostalDeliveryNumber", SqlDbType.NVarChar, (createJob.JobInstallerDetails != null ? createJob.JobInstallerDetails.PostalDeliveryNumber : null)));
            sqlParameters.Add(DBClient.AddParameters("SWHInstallerDesignerId", SqlDbType.Int, (createJob.JobInstallerDetails != null ? createJob.JobInstallerDetails.SWHInstallerDesignerId : 0)));
            sqlParameters.Add(DBClient.AddParameters("InstallerLicenseNumber", SqlDbType.NVarChar, (createJob.JobInstallerDetails != null ? createJob.JobInstallerDetails.LicenseNumber : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerSESignature", SqlDbType.NVarChar, (createJob.JobInstallerDetails != null ? createJob.JobInstallerDetails.SESignature : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerSolarCompanyId", SqlDbType.Int, (createJob.BasicDetails != null ? createJob.BasicDetails.SolarCompanyId : 0)));


            sqlParameters.Add(DBClient.AddParameters("OwnerType", SqlDbType.NVarChar, createJob.JobOwnerDetails.OwnerType));
            sqlParameters.Add(DBClient.AddParameters("OwnerCompanyABN", SqlDbType.NVarChar, createJob.JobOwnerDetails.CompanyABN));
            sqlParameters.Add(DBClient.AddParameters("OwnerCompanyName", SqlDbType.NVarChar, createJob.JobOwnerDetails.CompanyName));
            sqlParameters.Add(DBClient.AddParameters("OwnerFirstName", SqlDbType.NVarChar, createJob.JobOwnerDetails.FirstName));
            sqlParameters.Add(DBClient.AddParameters("OwnerLastName", SqlDbType.NVarChar, createJob.JobOwnerDetails.LastName));
            sqlParameters.Add(DBClient.AddParameters("OwnerUnitTypeID", SqlDbType.Int, createJob.JobOwnerDetails.UnitTypeID));
            sqlParameters.Add(DBClient.AddParameters("OwnerUnitNumber", SqlDbType.NVarChar, createJob.JobOwnerDetails.UnitNumber));
            sqlParameters.Add(DBClient.AddParameters("OwnerStreetNumber", SqlDbType.NVarChar, createJob.JobOwnerDetails.StreetNumber));
            sqlParameters.Add(DBClient.AddParameters("OwnerStreetName", SqlDbType.NVarChar, createJob.JobOwnerDetails.StreetName));
            sqlParameters.Add(DBClient.AddParameters("OwnerStreetTypeID", SqlDbType.NVarChar, createJob.JobOwnerDetails.StreetTypeID));
            sqlParameters.Add(DBClient.AddParameters("OwnerTown", SqlDbType.NVarChar, createJob.JobOwnerDetails.Town));
            sqlParameters.Add(DBClient.AddParameters("OwnerState", SqlDbType.NVarChar, createJob.JobOwnerDetails.State));
            sqlParameters.Add(DBClient.AddParameters("OwnerPostCode", SqlDbType.NVarChar, createJob.JobOwnerDetails.PostCode));
            sqlParameters.Add(DBClient.AddParameters("OwnerPhone", SqlDbType.NVarChar, createJob.JobOwnerDetails.Phone));
            sqlParameters.Add(DBClient.AddParameters("OwnerMobile", SqlDbType.NVarChar, createJob.JobOwnerDetails.Mobile));
            sqlParameters.Add(DBClient.AddParameters("OwnerEmail", SqlDbType.NVarChar, createJob.JobOwnerDetails.Email));
            sqlParameters.Add(DBClient.AddParameters("OwnerIsPostalAddress", SqlDbType.Bit, createJob.JobOwnerDetails.IsPostalAddress));
            sqlParameters.Add(DBClient.AddParameters("OwnerPostalAddressID", SqlDbType.NVarChar, createJob.JobOwnerDetails.PostalAddressID));
            sqlParameters.Add(DBClient.AddParameters("OwnerPostalDeliveryNumber", SqlDbType.NVarChar, createJob.JobOwnerDetails.PostalDeliveryNumber));
            sqlParameters.Add(DBClient.AddParameters("IsOwnerRegisteredWithGST", SqlDbType.Bit, createJob.JobOwnerDetails.IsOwnerRegisteredWithGST));
            #endregion add parameters
            sqlParameters = BusinessRuleforJobSTCDetail(sqlParameters, createJob.JobInstallationDetails, createJob.JobSTCDetails, LoggedInUserId, createJob.BasicDetails.JobType);
            #region commented code
            //sqlParameters.Add(DBClient.AddParameters("STCTypeOfConnection", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.TypeOfConnection : null)));
            //sqlParameters.Add(DBClient.AddParameters("STCSystemMountingType", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.SystemMountingType : null)));
            //sqlParameters.Add(DBClient.AddParameters("STCInstallingCompleteUnit", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.InstallingCompleteUnit : null)));
            //sqlParameters.Add(DBClient.AddParameters("STCAdditionalCapacityNotes", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.AdditionalCapacityNotes : null)));
            //sqlParameters.Add(DBClient.AddParameters("STCDeemingPeriod", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.DeemingPeriod : null)));
            //sqlParameters.Add(DBClient.AddParameters("STCCertificateCreated", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.CertificateCreated : null)));
            //sqlParameters.Add(DBClient.AddParameters("STCFailedAccreditationCode", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.FailedAccreditationCode : null)));
            //sqlParameters.Add(DBClient.AddParameters("STCFailedReason", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.FailedReason : null)));
            //sqlParameters.Add(DBClient.AddParameters("STCCECAccreditationStatement", SqlDbType.NVarChar, "Yes"));
            //sqlParameters.Add(DBClient.AddParameters("STCGovernmentSitingApproval", SqlDbType.NVarChar, "Yes"));
            //sqlParameters.Add(DBClient.AddParameters("STCElectricalSafetyDocumentation", SqlDbType.NVarChar, "Yes"));
            //sqlParameters.Add(DBClient.AddParameters("STCAustralianNewZealandStandardStatement", SqlDbType.NVarChar, "Yes"));
            //sqlParameters.Add(DBClient.AddParameters("STCVolumetricCapacity", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.VolumetricCapacity : null)));
            //sqlParameters.Add(DBClient.AddParameters("STCStatutoryDeclarations", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.StatutoryDeclarations : null)));
            //sqlParameters.Add(DBClient.AddParameters("STCSecondhandWaterHeater", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.SecondhandWaterHeater : null)));
            //sqlParameters.Add(DBClient.AddParameters("STCStandAloneGridSelected", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.StandAloneGridSelected : null)));
            //sqlParameters.Add(DBClient.AddParameters("STCMultipleSGUAddress", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.MultipleSGUAddress : null)));
            //sqlParameters.Add(DBClient.AddParameters("STCSGUSystemLocated", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.SGUSystemLocated : null)));
            //sqlParameters.Add(DBClient.AddParameters("STCCreatedBy", SqlDbType.NVarChar, LoggedInUserId));//ProjectSession.LoggedInUserId));
            //sqlParameters.Add(DBClient.AddParameters("STCLocation", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.Location : null)));
            //sqlParameters.Add(DBClient.AddParameters("AdditionalLocationInformation", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.AdditionalLocationInformation : null)));
            //sqlParameters.Add(DBClient.AddParameters("batterySystemPartOfAnAggregatedControl", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.batterySystemPartOfAnAggregatedControl : null)));
            //sqlParameters.Add(DBClient.AddParameters("changedSettingOfBatteryStorageSystem", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.changedSettingOfBatteryStorageSystem : null)));
            //sqlParameters.Add(DBClient.AddParameters("AdditionalSystemInformation", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.AdditionalSystemInformation : null)));
            //sqlParameters.Add(DBClient.AddParameters("Latitude", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.Latitude : null)));
            //sqlParameters.Add(DBClient.AddParameters("Longitude", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.Longitude : null)));
            #endregion commented code

            sqlParameters.Add(DBClient.AddParameters("SystemSize", SqlDbType.Decimal, (createJob.JobSystemDetails != null ? createJob.JobSystemDetails.SystemSize : null)));
            sqlParameters.Add(DBClient.AddParameters("NoOfPanel", SqlDbType.Int, (createJob.JobSystemDetails != null ? createJob.JobSystemDetails.NoOfPanel : null)));
            sqlParameters.Add(DBClient.AddParameters("SerialNumbers", SqlDbType.NVarChar, (createJob.JobSystemDetails != null ? createJob.JobSystemDetails.SerialNumbers : null)));
            //sqlParameters.Add(DBClient.AddParameters("CalculatedSTC", SqlDbType.Decimal, (createJob.JobSystemDetails != null ? createJob.JobSystemDetails.CalculatedSTC : null)));
            sqlParameters.Add(DBClient.AddParameters("ModifiedCalculatedSTC", SqlDbType.Decimal, (createJob.JobSystemDetails != null ? createJob.JobSystemDetails.ModifiedCalculatedSTC : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallationType", SqlDbType.NVarChar, (createJob.JobSystemDetails != null ? createJob.JobSystemDetails.InstallationType : null)));
            sqlParameters.Add(DBClient.AddParameters("InverterSerialNumbers", SqlDbType.NVarChar, (createJob.JobSystemDetails != null ? createJob.JobSystemDetails.InverterSerialNumbers : null)));
            if (createJob.JobSystemDetails != null && createJob.BasicDetails.JobType == 2)
            {
                xmlPanels = "<Panels><panel><Brand>" + HttpUtility.HtmlEncode(createJob.JobSystemDetails.SystemBrand) + "</Brand><Model>" + HttpUtility.HtmlEncode(createJob.JobSystemDetails.SystemModel) + "</Model><NoOfPanel>" + createJob.JobSystemDetails.NoOfPanel + "</NoOfPanel></panel></Panels>";
            }

            sqlParameters.Add(DBClient.AddParameters("xmlPanels", SqlDbType.Xml, xmlPanels));
            sqlParameters.Add(DBClient.AddParameters("xmlInverters", SqlDbType.Xml, xmlInverters));

            DataTable dtBatteryManufacturer = ConvertBatteryStorageToList(createJob.lstJobBatteryManufacturer, LoggedInUserId);
            sqlParameters.Add(DBClient.AddParameters("batteryManufacturerList", SqlDbType.Structured, dtBatteryManufacturer));

            sqlParameters.Add(DBClient.AddParameters("IsVendorApi", SqlDbType.Bit, createJob.IsVendorApi));
            sqlParameters.Add(DBClient.AddParameters("dtCustomField", SqlDbType.Structured, dtCustomField));
            sqlParameters.Add(DBClient.AddParameters("VendorJobId", SqlDbType.NVarChar, createJob.VendorJobId));
            sqlParameters.Add(DBClient.AddParameters("UrgentJobDays", SqlDbType.Int, Convert.ToInt32(ProjectConfiguration.UrgentSTCStatusJobDay)));

            int jobId = 0;
            DataSet dataset = CommonDAL.ExecuteDataSet("Job_Insert", sqlParameters.ToArray()); // Live or Staging 
                                                                                               //DataSet dataset = CommonDAL.ExecuteDataSet("Job_Insert_Bk", sqlParameters.ToArray()); //Local
            if (dataset != null && dataset.Tables.Count > 0)
            {
                jobId = Convert.ToInt32(dataset.Tables[0].Rows[0].ItemArray[0].ToString());
                string IsUrgentSubmission = Convert.ToString(false);
                if (dataset.Tables.Count > 1 && dataset.Tables[1].Rows.Count > 0)
                {
                    IsUrgentSubmission = !string.IsNullOrEmpty(dataset.Tables[1].Rows[0]["IsUrgentSubmission"].ToString()) ? dataset.Tables[1].Rows[0]["IsUrgentSubmission"].ToString() : Convert.ToString(false);
                }
                if (createJob.BasicDetails.JobID > 0)
                {
                    SortedList<string, string> data = new SortedList<string, string>();
                    DataTable dt = GetSTCDetailsAndJobDataForCache(null, createJob.BasicDetails.JobID.ToString());

                    if (dt.Rows.Count > 0)
                    {
                        string Panelbrand = string.Empty;
                        string PanelModel = string.Empty;
                        string InverterBrand = string.Empty;
                        string InverterModel = string.Empty;
                        string InverterSeries = string.Empty;
                        #region getpaneldata
                        if (!string.IsNullOrEmpty(xmlPanels))
                        {
                            List<xmlPanel> lstxmlPanels = new List<xmlPanel>();
                            lstxmlPanels = CommonBAL.getPanelDetailsFromXML(xmlPanels);
                            if (lstxmlPanels != null && lstxmlPanels.Count > 0)
                            {
                                Panelbrand = String.Join(", ", lstxmlPanels.Select(v => v.Brand).Distinct().ToList());
                                PanelModel = String.Join(", ", lstxmlPanels.Select(v => v.Model).Distinct().ToList());
                            }
                        }
                        #endregion
                        #region getinverterdata
                        if (!string.IsNullOrEmpty(xmlInverters))
                        {
                            List<xmlInverter> lstxmlInverter = new List<xmlInverter>();
                            lstxmlInverter = CommonBAL.getInverterDetailsFromXML(xmlInverters);
                            if (lstxmlInverter != null && lstxmlInverter.Count > 0)
                            {
                                InverterBrand = String.Join(", ", lstxmlInverter.Select(v => v.Brand).Distinct().ToList());
                                InverterModel = String.Join(", ", lstxmlInverter.Select(v => v.Model).Distinct().ToList());
                                InverterSeries = String.Join(", ", lstxmlInverter.Select(v => v.Series).Distinct().ToList());
                            }
                        }
                        #endregion

                        string isSPVRequired = dt.Rows[0]["IsSPVRequired"].ToString();
                        string OwnerCompany = createJob.JobOwnerDetails.CompanyName == null ? "" : createJob.JobOwnerDetails.CompanyName;
                        string RefNumberOwnerName = createJob.BasicDetails.RefNumber + ((createJob.JobOwnerDetails.CompanyName) == null ? "" : (" - " + createJob.JobOwnerDetails.CompanyName)) + " - " + createJob.JobOwnerDetails.FirstName + " " + createJob.JobOwnerDetails.LastName;
                        string installationAddress = GetInstallationAddressForCache(createJob.BasicDetails.JobID).Rows[0]["InstallationAddress"].ToString();
                        data.Add("InstallationTown", createJob.JobInstallationDetails.Town);

                        data.Add("InstallationState", createJob.JobInstallationDetails.State);
                        data.Add("JobID", createJob.BasicDetails.JobID.ToString());
                        data.Add("JobTypeId", createJob.BasicDetails.JobType.ToString());
                        data.Add("IsSPVRequired", isSPVRequired);
                        data.Add("OwnerName", createJob.JobOwnerDetails.FirstName + " " + createJob.JobOwnerDetails.LastName);
                        data.Add("OwnerCompany", createJob.JobOwnerDetails.CompanyName == null ? "" : createJob.JobOwnerDetails.CompanyName);
                        data.Add("InstallationDate", createJob.BasicDetails.InstallationDate.ToString());
                        data.Add("InstallationAddress", installationAddress);
                        data.Add("SystemSize", createJob.JobSystemDetails.SystemSize.ToString());
                        data.Add("RefNumberOwnerName", RefNumberOwnerName);
                        data.Add("STC", createJob.JobSystemDetails.ModifiedCalculatedSTC.ToString());
                        data.Add("IsGst", dt.Rows[0]["IsGst"].ToString());
                        data.Add("PanelBrand", Panelbrand);
                        data.Add("PanelModel", PanelModel);
                        data.Add("InverterBrand", InverterBrand);
                        data.Add("InverterModel", InverterModel);
                        data.Add("InverterSeries", InverterSeries);
                        data.Add("IsUrgentSubmission", IsUrgentSubmission);
                        CommonBAL.SetCacheDataForSTCSubmission(null, createJob.BasicDetails.JobID, data).Wait();

                    }

                    //int stcJobDetailsId = Convert.ToInt32(dataset.Tables[0].Rows[0].ItemArray[1].ToString());
                    //if (stcJobDetailsId > 0)
                    //{
                    //    CommonBAL.SetCacheDataForSTCSubmission(stcJobDetailsId, SolarCompanyId);
                    //}
                }
            }

            return jobId;
            //}
            //catch (Exception ex)
            //{
            //    if (createJob.VendorJobId != null)
            //        WriteToLogFile(ex.Message, "InsertJob", "RefNumber:" + createJob.BasicDetails.RefNumber);

            //    Log.WriteError(ex);
            //    return 0;
            //}
        }

        /// <summary>
        /// Yesr wise calculation of Deeming Period.
        /// </summary>
        /// <param name="year">year</param>
        /// <returns>string</returns>
        public List<string> GetDeemingPeriod(int year)
        {
            const string ONE_YEAR = "One year";
            const string FIVE_YEARS = "Five years";
            const string FIFTEEN_YEARS = "Fifteen years";
            List<string> lstYear = new List<string> { ONE_YEAR, FIVE_YEARS, FIFTEEN_YEARS };
            switch (year)
            {
                case 2013:
                    return lstYear;
                case 2014:
                    return lstYear;
                case 2015:
                    return lstYear;
                case 2016:
                    return lstYear;
                case 2017:
                    return new List<string> { ONE_YEAR, FIVE_YEARS, GetYearInWords(year) };
                case 2018:
                    return new List<string> { ONE_YEAR, FIVE_YEARS, GetYearInWords(year) };
                case 2019:
                    return new List<string> { ONE_YEAR, FIVE_YEARS, GetYearInWords(year) };
                case 2020:
                    return new List<string> { ONE_YEAR, FIVE_YEARS, GetYearInWords(year) };
                case 2021:
                    return new List<string> { ONE_YEAR, FIVE_YEARS, GetYearInWords(year) };
                case 2022:
                    return new List<string> { ONE_YEAR, FIVE_YEARS, GetYearInWords(year) };
                case 2023:
                    return new List<string> { ONE_YEAR, FIVE_YEARS, GetYearInWords(year) };
                case 2024:
                    return new List<string> { ONE_YEAR, FIVE_YEARS, GetYearInWords(year) };
                case 2025:
                    return new List<string> { ONE_YEAR, FIVE_YEARS, GetYearInWords(year) };
                case 2026:
                    return new List<string> { ONE_YEAR, FIVE_YEARS };
                case 2027:
                    return new List<string> { ONE_YEAR, FIVE_YEARS };
                case 2028:
                    return new List<string> { ONE_YEAR, GetYearInWords(year) };
                case 2029:
                    return new List<string> { ONE_YEAR, GetYearInWords(year) };
                case 2030:
                    return new List<string> { ONE_YEAR };
                default:
                    break;
                    //case 2017:
                    //    return new List<string> { ONE_YEAR, FIVE_YEARS, "Fourteen years" };
                    //case 2018:
                    //    return new List<string> { ONE_YEAR, FIVE_YEARS, "Thirteen years" };
                    //case 2019:
                    //    return new List<string> { ONE_YEAR, FIVE_YEARS, "Twelve years" };
                    //case 2020:
                    //    return new List<string> { ONE_YEAR, FIVE_YEARS, "Eleven years" };
                    //case 2021:
                    //    return new List<string> { ONE_YEAR, FIVE_YEARS, "Ten years" };
                    //case 2022:
                    //    return new List<string> { ONE_YEAR, FIVE_YEARS, "Nine years" };
                    //case 2023:
                    //    return new List<string> { ONE_YEAR, FIVE_YEARS, "Eight years" };
                    //case 2024:
                    //    return new List<string> { ONE_YEAR, FIVE_YEARS, "Seven years" };
                    //case 2025:
                    //    return new List<string> { ONE_YEAR, FIVE_YEARS, "Six years" };
                    //case 2026:
                    //    return new List<string> { ONE_YEAR, FIVE_YEARS };
                    //case 2027:
                    //    return new List<string> { ONE_YEAR, FIVE_YEARS };
                    //case 2028:
                    //    return new List<string> { ONE_YEAR, "Three years" };
                    //case 2029:
                    //    return new List<string> { ONE_YEAR, "Two years" };
                    //case 2030:
                    //    return new List<string> { ONE_YEAR };
                    //default:
                    //    break;
            }

            return lstYear;
        }

        public string GetYearInWords(int year)
        {
            if (year == 2017)
                return "Fourteen years";
            else if (year == 2018)
                return "Thirteen years";
            else if (year == 2019)
                return "Twelve years";
            else if (year == 2020)
                return "Eleven years";
            else if (year == 2021)
                return "Ten years";
            else if (year == 2022)
                return "Nine years";
            else if (year == 2023)
                return "Eight years";
            else if (year == 2024)
                return "Seven years";
            else if (year == 2025)
                return "Six years";
            else if (year == 2026)
                return string.Empty;
            else if (year == 2027)
                return string.Empty;
            else if (year == 2028)
                return "Three years";
            else if (year == 2029)
                return "Two years";
            else if (year == 2030)
                return string.Empty;
            else
                return string.Empty;
        }

        /// <summary>
        /// Creates the job number.
        /// </summary>
        /// <param name="jobType">Type of the job.</param>
        /// <returns>
        /// data set
        /// </returns>
        public DataSet CreateJobNumber(int jobType, int solarCompanyId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolardCompanyID", SqlDbType.Int, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("jobType", SqlDbType.Int, jobType));
            DataSet dsUsers = CommonDAL.ExecuteDataSet("Job_createJobNumber", sqlParameters.ToArray());
            return dsUsers;
        }

        /// <summary>
        /// Get Job By ID.
        /// </summary>
        /// <param name="jobID">JobID</param>
        /// <returns>
        /// job by id
        /// </returns>
        public CreateJob GetJobByID(int jobID)
        {
            CreateJob createjob = new CreateJob();

            string spSelfieName = "[GetSignatureSelfies]";
            List<SqlParameter> sqlParametersSelfie = new List<SqlParameter>();
            sqlParametersSelfie.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            DataSet dataSetSelfie = CommonDAL.ExecuteDataSet(spSelfieName, sqlParametersSelfie.ToArray());
            if (dataSetSelfie != null && dataSetSelfie.Tables.Count > 0)
            {
                var data = dataSetSelfie.Tables[0].AsEnumerable().FirstOrDefault();
                createjob.OwnerSignatureSelfie = data.Field<string>("OwnerSelfie");
                createjob.DesignerSignatureSelfie = data.Field<string>("DesignerSelfie");
                createjob.InstallerSignatureSelfie = data.Field<string>("InstallerSelfie");
                createjob.ElectritionSignatureSelfie = data.Field<string>("ElectricianSelfie");
            }

            string spName = "[Job_GetJobByJobId]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            DataSet dataSet = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                //Basic  Details
                createjob.BasicDetails = dataSet.Tables[0].AsEnumerable().Select(m => new BasicDetails()
                {
                    JobID = m.Field<int>("JobID")
                        ,
                    RefNumber = m.Field<string>("RefNumber")
                        ,
                    Title = m.Field<string>("Title")
                        ,
                    Description = m.Field<string>("Description")
                        ,
                    JobNumber = m.Field<string>("JobNumber")
                        ,
                    JobStage = m.Field<int>("JobStage")
                        ,
                    JobType = m.Field<byte>("JobType")
                        ,
                    InstallerID = m.Field<int?>("InstallerID")
                        ,
                    DesignerID = m.Field<int?>("DesignerID")
                        ,
                    JobElectricianID = m.Field<int>("JobElectricianID")
                        ,
                    InstallerSignature = m.Field<string>("InstallerSignature")
                        ,
                    DesignerSignature = m.Field<string>("DesignerSignature")
                        ,
                    ElectricianSignature = m.Field<string>("ElectricianSignature")
                        ,
                    OwnerSignature = m.Field<string>("OwnerSignature")
                        ,
                    InstallationDate = m.Field<DateTime?>("InstallationDate")
                        ,
                    Priority = m.Field<byte>("Priority")
                        ,
                    SolarCompanyId = m.Field<int>("SolarCompanyID")
                        ,
                    CompanyName = m.Field<string>("CustomCompanyName")
                        ,
                    GB_SCACode = m.Field<string>("GB_SCACode")
                        ,
                    IsGst = m.Field<bool>("IsGst")
                        ,
                    SSCID = m.Field<int?>("SSCID")
                        ,
                    Notes = m.Field<string>("Notes")
                        ,
                    SoldBy = m.Field<string>("SoldBy")
                        ,
                    SoldByDate = m.Field<DateTime?>("SoldByDate")
                        ,
                    GSTDocument = m.Field<string>("GSTDocument")
                        ,
                    IsClassic = m.Field<bool>("IsClassic")
                        ,
                    CompanyABN = m.Field<string>("CompanyABN")
                        ,
                    IsRegisteredWithGST = m.Field<bool>("IsRegisteredWithGST")
                        ,
                    IsGSTSetByAdminUser = m.Field<int?>("IsGSTSetByAdminUser")
                        ,
                    STCInvoiceCount = m.Field<int>("STCInvoiceCount")
                        ,
                    ContactName = m.Field<string>("ContactName")
                        ,
                    SolarCompanyPhone = m.Field<string>("SolarCompanyPhone")
                        ,
                    SolarCompanyMobile = m.Field<string>("SolarCompanyMobile")
                        ,
                    Email = m.Field<string>("Email")
                        ,
                    Reseller = m.Field<string>("Reseller")
                        ,
                    ResellerABN = m.Field<string>("ResellerABN")
                        ,
                    ResellerName = m.Field<string>("ResellerName")
                        ,
                    InstallerLatLong = m.Field<string>("InstallerLatLong")
                        ,
                    DesignerLatLong = m.Field<string>("DesignerLatLong")
                        ,
                    ElectricianLatLong = m.Field<string>("ElectricianLatLong")
                        ,
                    OwnerLatLong = m.Field<string>("OwnerLatLong")
                        ,
                    IsAllowTrade = m.Field<bool>("IsAllowTrade")
                        ,
                    IsWholeSaler = m.Field<bool>("IsWholeSaler")
                        ,
                    IsSPVRequired = m.Field<bool>("IsSPVRequired"),
                    IsGenerateRecZip = m.Field<bool?>("IsGenerateRecZip"),
                    IsLockedSerialNumber = m.Field<bool>("IsLockedSerialNumber"),
                    ResellerId = m.Field<int>("ResellerId"),
                    CreatedBy = m.Field<int>("CreatedBy")
                    //    ,
                    //ApproachingExpiryDate = m.Field<DateTime?>("ApproachingExpiryDate")
                }).FirstOrDefault();
                //createjob.JobElectricians = dataSet.Tables[1].Rows.Count > 0 ? DBClient.DataTableToList<JobElectricians>(dataSet.Tables[1])[0] : new JobElectricians();

                createjob.JobElectricians = JobElectriciansEntity(dataSet.Tables[1]);

                //createjob.JobInstallationDetails = dataSet.Tables[2].Rows.Count > 0 ? DBClient.DataTableToList<JobInstallationDetails>(dataSet.Tables[2])[0] : new JobInstallationDetails();
                createjob.JobInstallationDetails = JobInstallationDetailsEntity(dataSet.Tables[2]);

                createjob.JobSTCDetails = dataSet.Tables[3].Rows.Count > 0 ? DBClient.DataTableToList<JobSTCDetails>(dataSet.Tables[3])[0] : new JobSTCDetails();

                //createjob.JobSystemDetails = dataSet.Tables[4].Rows.Count > 0 ? DBClient.DataTableToList<JobSystemDetails>(dataSet.Tables[4])[0] : new JobSystemDetails();
                createjob.JobSystemDetails = JobSystemDetailsEntity(dataSet.Tables[4]);

                createjob.lstJobInverterDetails = dataSet.Tables[5].Rows.Count > 0 ? DBClient.DataTableToList<JobInverterDetails>(dataSet.Tables[5]) : new List<JobInverterDetails>();
                createjob.JobSystemDetails.NoOfInverter = createjob.lstJobInverterDetails.Count > 0 ? createjob.lstJobInverterDetails.Sum(x => x.NoOfInverter) : 0;
                createjob.lstJobPanelDetails = dataSet.Tables[6].Rows.Count > 0 ? DBClient.DataTableToList<JobPanelDetails>(dataSet.Tables[6]) : new List<JobPanelDetails>();
                if (createjob.lstJobPanelDetails.Count > 0)
                {
                    foreach (var item in createjob.lstJobPanelDetails)
                    {
                        item.CECApprovedDate = !string.IsNullOrEmpty(item.CECApprovedDate) ? Convert.ToDateTime(item.CECApprovedDate).ToString("dd/MM/yyyy") : "";
                        item.ExpiryDate = !string.IsNullOrEmpty(item.ExpiryDate) ? Convert.ToDateTime(item.ExpiryDate).ToString("dd/MM/yyyy") : "";
                    }

                }
                if (createjob.lstJobInverterDetails.Count > 0)
                {
                    foreach (var item in createjob.lstJobInverterDetails)
                    {
                        item.CECApprovedDate = !string.IsNullOrEmpty(item.CECApprovedDate) ? Convert.ToDateTime(item.CECApprovedDate).ToString("dd/MM/yyyy") : "";
                        item.ExpiryDate = !string.IsNullOrEmpty(item.ExpiryDate) ? Convert.ToDateTime(item.ExpiryDate).ToString("dd/MM/yyyy") : "";
                    }

                }
                //createjob.JobOwnerDetails = dataSet.Tables[7].Rows.Count > 0 ? DBClient.DataTableToList<JobOwnerDetails>(dataSet.Tables[7])[0] : new JobOwnerDetails();
                createjob.JobOwnerDetails = JobOwnerDetailsEntity(dataSet.Tables[7]);
                //createjob.JobInstallerDetails = dataSet.Tables[8].Rows.Count > 0 ? DBClient.DataTableToList<JobInstallerDetails>(dataSet.Tables[8])[0] : new JobInstallerDetails();
                createjob.JobInstallerDetails = JobInstallerEntity(dataSet.Tables[8]);

                if (createjob.BasicDetails != null && createjob.BasicDetails.JobType == 2 && createjob.lstJobPanelDetails.Count > 0)
                {
                    createjob.JobSystemDetails.SystemBrand = createjob.lstJobPanelDetails[0].Brand;
                    createjob.JobSystemDetails.SystemModel = createjob.lstJobPanelDetails[0].Model;
                    createjob.JobSystemDetails.NoOfPanel = createjob.lstJobPanelDetails[0].NoOfPanel;


                    createjob.JobSystemDetails.CECApprovedDate = !string.IsNullOrEmpty(createjob.lstJobPanelDetails[0].CECApprovedDate) ? Convert.ToDateTime(createjob.lstJobPanelDetails[0].CECApprovedDate).ToString("dd/MM/yyyy") : "";
                    createjob.JobSystemDetails.ExpiryDate = !string.IsNullOrEmpty(createjob.lstJobPanelDetails[0].ExpiryDate) ? Convert.ToDateTime(createjob.lstJobPanelDetails[0].ExpiryDate).ToString("dd/MM/yyyy") : "";

                }

                createjob.JobPanelDetails = new JobPanelDetails();
                createjob.JobInverterDetails = new JobInverterDetails();
                createjob.lstPVModules = dataSet.Tables[9].Rows.Count > 0 ? DBClient.DataTableToList<PVModules>(dataSet.Tables[9]) : new List<PVModules>();
                createjob.lstInverters = dataSet.Tables[10].Rows.Count > 0 ? DBClient.DataTableToList<Inverters>(dataSet.Tables[10]) : new List<Inverters>();

                if (dataSet.Tables.Count > 13 && dataSet.Tables[13].Rows.Count > 0)
                {
                    createjob.InstallerSignature = dataSet.Tables[13].Rows[0]["InstallerSignature"].ToString();
                    createjob.DesignerSignature = dataSet.Tables[13].Rows[0]["DesignerSignature"].ToString();
                    createjob.ElectricianSignature = dataSet.Tables[13].Rows[0]["ElectricianSignature"].ToString();
                    createjob.OwnerSignature = dataSet.Tables[13].Rows[0]["OwnerSignature"].ToString();
                }
                if (createjob.BasicDetails != null)
                {
                    createjob.IsOwnerSignUpload = createjob.BasicDetails.OwnerSignature != null ? true : false;
                    createjob.IsInstallerSignUpload = createjob.BasicDetails.InstallerSignature != null ? true : false;
                    createjob.IsElectricianSignUpload = createjob.BasicDetails.ElectricianSignature != null ? true : false;
                    createjob.IsDesigerSignUpload = createjob.BasicDetails.DesignerSignature != null ? true : false;
                }
                else
                {
                    createjob.IsOwnerSignUpload = false;
                    createjob.IsInstallerSignUpload = false;
                    createjob.IsElectricianSignUpload = false;
                    createjob.IsDesigerSignUpload = false;
                }
                createjob.InstallerView = GetInstallerDesignerEntity(dataSet.Tables[11], true);
                //createjob.InstallerView = dataSet.Tables[11].Rows.Count > 0 ? DBClient.DataTableToList<InstallerDesignerView>(dataSet.Tables[11])[0] : new InstallerDesignerView();
                //createjob.DesignerView = dataSet.Tables[12].Rows.Count > 0 ? DBClient.DataTableToList<InstallerDesignerView>(dataSet.Tables[12])[0] : new InstallerDesignerView();
                createjob.DesignerView = GetInstallerDesignerEntity(dataSet.Tables[12], false);
                if (createjob.BasicDetails != null && !createjob.BasicDetails.IsClassic)
                {
                    List<CustomDetail> lstCustomDetails = dataSet.Tables[14].Rows.Count > 0 ? dataSet.Tables[14].ToListof<CustomDetail>() : new List<CustomDetail>();
                    createjob.lstCustomDetails = lstCustomDetails;
                }

                if (createjob.BasicDetails != null && createjob.BasicDetails.JobType == 2 && createjob.JobInstallerDetails != null)
                    createjob.JobInstallerDetails.SESignature = dataSet.Tables[15].Rows.Count > 0 ? dataSet.Tables[15].Rows[0]["SESignature"].ToString() : string.Empty;

                createjob.lstJobBatteryManufacturer = dataSet.Tables[16].Rows.Count > 0 ? DBClient.DataTableToList<JobBatteryManufacturer>(dataSet.Tables[16]) : new List<JobBatteryManufacturer>();
                createjob.InstallerDesignerView = new InstallerDesignerView();

                createjob.JobSerialNumbers = dataSet.Tables[17].Rows.Count > 0 ? DBClient.DataTableToList<JobSerialNumbers>(dataSet.Tables[17]) : new List<JobSerialNumbers>();
                if (createjob.JobSerialNumbers != null)
                {
                    createjob.JobSerialNumbers.ForEach(a => a.SerialNumber = HttpUtility.JavaScriptStringEncode(a.SerialNumber));
                }

                createjob.ElectricianData = dataSet.Tables[18].Rows.Count > 0 ? dataSet.Tables[18] : new DataTable();
                createjob.RetailerAutoSettingForSignature = dataSet.Tables[19].Rows.Count > 0 ? dataSet.Tables[19] : new DataTable();

                GetJobDetails(jobID, createjob);
            }
            return createjob;
        }

        public CreateJob GetJobByIDTabWise(int jobID, int DATAOPMODE)
        {
            CreateJob createjob = new CreateJob();

            if (DATAOPMODE == 1 || DATAOPMODE == 6)
            {
                string spSelfieName = "[GetSignatureSelfies]";
                List<SqlParameter> sqlParametersSelfie = new List<SqlParameter>();
                sqlParametersSelfie.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
                DataSet dataSetSelfie = CommonDAL.ExecuteDataSet(spSelfieName, sqlParametersSelfie.ToArray());
                if (dataSetSelfie != null && dataSetSelfie.Tables.Count > 0)
                {
                    var data = dataSetSelfie.Tables[0].AsEnumerable().FirstOrDefault();
                    createjob.OwnerSignatureSelfie = data.Field<string>("OwnerSelfie");
                    createjob.DesignerSignatureSelfie = data.Field<string>("DesignerSelfie");
                    createjob.InstallerSignatureSelfie = data.Field<string>("InstallerSelfie");
                    createjob.ElectritionSignatureSelfie = data.Field<string>("ElectricianSelfie");
                }
            }

            string spName = "[Job_GetJobByJobIdTabularNew]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("DATAOPMODE", SqlDbType.Int, DATAOPMODE));
            DataSet dataSet = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                //Basic  Details
                //if (DATAOPMODE == 1)
                //{
                createjob.BasicDetails = dataSet.Tables[0].AsEnumerable().Select(m => new BasicDetails()
                {
                    JobID = m.Field<int>("JobID")
                        ,
                    RefNumber = m.Field<string>("RefNumber")
                        ,
                    Title = m.Field<string>("Title")
                        ,
                    Description = m.Field<string>("Description")
                        ,
                    JobNumber = m.Field<string>("JobNumber")
                        ,
                    JobStage = m.Field<int>("JobStage")
                        ,
                    JobType = m.Field<byte>("JobType")
                        ,
                    InstallerID = m.Field<int?>("InstallerID")
                        ,
                    DesignerID = m.Field<int?>("DesignerID")
                        ,
                    JobElectricianID = m.Field<int>("JobElectricianID")
                        ,
                    InstallerSignature = m.Field<string>("InstallerSignature")
                        ,
                    DesignerSignature = m.Field<string>("DesignerSignature")
                        ,
                    ElectricianSignature = m.Field<string>("ElectricianSignature")
                        ,
                    OwnerSignature = m.Field<string>("OwnerSignature")
                        ,
                    InstallationDate = m.Field<DateTime?>("InstallationDate")
                        ,
                    Priority = m.Field<byte>("Priority")
                        ,
                    SolarCompanyId = m.Field<int>("SolarCompanyID")
                        ,
                    CompanyName = m.Field<string>("CustomCompanyName")
                        ,
                    GB_SCACode = m.Field<string>("GB_SCACode")
                        ,
                    IsGst = m.Field<bool>("IsGst")
                        ,
                    SSCID = m.Field<int?>("SSCID")
                        ,
                    Notes = m.Field<string>("Notes")
                        ,
                    SoldBy = m.Field<string>("SoldBy")
                        ,
                    SoldByDate = m.Field<DateTime?>("SoldByDate")
                        ,
                    GSTDocument = m.Field<string>("GSTDocument")
                        ,
                    IsClassic = m.Field<bool>("IsClassic")
                        ,
                    CompanyABN = m.Field<string>("CompanyABN")
                        ,
                    IsRegisteredWithGST = m.Field<bool>("IsRegisteredWithGST")
                        ,
                    IsGSTSetByAdminUser = m.Field<int?>("IsGSTSetByAdminUser")
                        ,
                    STCInvoiceCount = m.Field<int>("STCInvoiceCount")
                        ,
                    ContactName = m.Field<string>("ContactName")
                        ,
                    SolarCompanyPhone = m.Field<string>("SolarCompanyPhone")
                        ,
                    SolarCompanyMobile = m.Field<string>("SolarCompanyMobile")
                        ,
                    Email = m.Field<string>("Email")
                        ,
                    Reseller = m.Field<string>("Reseller")
                        ,
                    ResellerABN = m.Field<string>("ResellerABN")
                        ,
                    ResellerName = m.Field<string>("ResellerName")
                        ,
                    InstallerLatLong = m.Field<string>("InstallerLatLong")
                        ,
                    DesignerLatLong = m.Field<string>("DesignerLatLong")
                        ,
                    ElectricianLatLong = m.Field<string>("ElectricianLatLong")
                        ,
                    OwnerLatLong = m.Field<string>("OwnerLatLong")
                        ,
                    IsAllowTrade = m.Field<bool>("IsAllowTrade")
                        ,
                    IsWholeSaler = m.Field<bool>("IsWholeSaler")
                        ,
                    IsSPVRequired = m.Field<bool>("IsSPVRequired"),
                    IsGenerateRecZip = m.Field<bool?>("IsGenerateRecZip"),
                    IsLockedSerialNumber = m.Field<bool>("IsLockedSerialNumber"),
                    ResellerId = m.Field<int>("ResellerId"),
                    CreatedBy = m.Field<int>("CreatedBy")
                    //    ,
                    //ApproachingExpiryDate = m.Field<DateTime?>("ApproachingExpiryDate")
                }).FirstOrDefault();
                //}
                //else
                //{
                //    createjob.BasicDetails = new BasicDetails();
                //}

                Int32.TryParse(dataSet.Tables[0].Rows[0]["STCStatus"].ToString(), out int stcStatus);
                Int32.TryParse(dataSet.Tables[0].Rows[0]["STCSettlementTerm"].ToString(), out int stcSettlementTerm);
                createjob.STCStatus =  stcStatus;
                createjob.STCSettlementTerm = stcSettlementTerm;
                createjob.IsPartialValidForSTCInvoice = dataSet.Tables[0].Rows[0].Field<bool>("IsPartialValidForSTCInvoice");
                createjob.IsInvoiced = dataSet.Tables[0].Rows[0].Field<bool>("IsInvoiced");

                if (dataSet.Tables[7].Rows.Count > 0)
                {
                    createjob.JobUserComplianceID = Convert.ToInt32(dataSet.Tables[7].Rows[0]["UserId"]);
                }
                if (DATAOPMODE == 3 || DATAOPMODE == 4 || DATAOPMODE == 1 || DATAOPMODE == 13 || DATAOPMODE == 14)
                {
                    createjob.JobInstallationDetails = JobInstallationDetailsEntity(dataSet.Tables[2]);
                    createjob.OwnerSignature = dataSet.Tables[0].Rows[0]["OwnerSignature"].ToString();
                    if (DATAOPMODE == 3)
                    {
                        createjob.JobInstallationDetails = JobInstallationDetailsEntity(dataSet.Tables[8]);
                        createjob.JobOwnerDetails = JobOwnerDetailsEntity(dataSet.Tables[1]);
                    }

                    if (DATAOPMODE != 13 && DATAOPMODE != 14)
                    {
                        if (createjob.BasicDetails != null && !createjob.BasicDetails.IsClassic)
                        {
                            if (dataSet.Tables[6].Rows.Count > 0)
                            {
                                List<CustomDetail> lstCustomDetails = dataSet.Tables[6].Rows.Count > 0 ? dataSet.Tables[6].ToListof<CustomDetail>() : new List<CustomDetail>();
                                createjob.lstCustomDetails = lstCustomDetails;
                            }
                            else
                            {
                                createjob.lstCustomDetails = new List<CustomDetail>();
                            }
                        }
                    }
                }
                else
                {
                    createjob.JobInstallationDetails = new JobInstallationDetails();
                }

                if (DATAOPMODE == 4)
                {
                    createjob.JobSTCDetails = dataSet.Tables[9].Rows.Count > 0 ? DBClient.DataTableToList<JobSTCDetails>(dataSet.Tables[9])[0] : new JobSTCDetails();
                    createjob.JobSystemDetails = JobSystemDetailsEntity(dataSet.Tables[10]);
                }
                else
                {
                    createjob.JobSTCDetails = new JobSTCDetails();
                    createjob.JobSystemDetails = new JobSystemDetails();
                }
                if (DATAOPMODE == 8)
                {
                    createjob.JobSystemDetails = JobSystemDetailsEntity(dataSet.Tables[9]);
                    createjob.JobSerialNumbers = dataSet.Tables[13].Rows.Count > 0 ? DBClient.DataTableToList<JobSerialNumbers>(dataSet.Tables[13]) : new List<JobSerialNumbers>();
                    if (createjob.JobSerialNumbers != null)
                    {
                        createjob.JobSerialNumbers.ForEach(a => a.SerialNumber = HttpUtility.JavaScriptStringEncode(a.SerialNumber));
                    }
                    //createjob.JobInstallationDetails = JobInstallationDetailsEntity(dataSet.Tables[13]);
                    //createjob.JobSystemDetails.NoOfInverter = createjob.lstJobInverterDetails.Count > 0 ? createjob.lstJobInverterDetails.Sum(x => x.NoOfInverter) : 0;
                }
                if (DATAOPMODE == 5)
                {
                    createjob.JobSTCDetails = dataSet.Tables[8].Rows.Count > 0 ? DBClient.DataTableToList<JobSTCDetails>(dataSet.Tables[8])[0] : new JobSTCDetails();

                    createjob.JobSystemDetails = JobSystemDetailsEntity(dataSet.Tables[9]);

                    createjob.lstJobPanelDetails = dataSet.Tables[10].Rows.Count > 0 ? DBClient.DataTableToList<JobPanelDetails>(dataSet.Tables[10]) : new List<JobPanelDetails>();
                    if (createjob.lstJobPanelDetails.Count > 0)
                    {
                        foreach (var item in createjob.lstJobPanelDetails)
                        {
                            item.CECApprovedDate = !string.IsNullOrEmpty(item.CECApprovedDate) ? Convert.ToDateTime(item.CECApprovedDate).ToString("dd/MM/yyyy") : "";
                            item.ExpiryDate = !string.IsNullOrEmpty(item.ExpiryDate) ? Convert.ToDateTime(item.ExpiryDate).ToString("dd/MM/yyyy") : "";
                        }
                    }

                    createjob.lstJobInverterDetails = dataSet.Tables[11].Rows.Count > 0 ? DBClient.DataTableToList<JobInverterDetails>(dataSet.Tables[11]) : new List<JobInverterDetails>();
                    if (createjob.lstJobInverterDetails.Count > 0)
                    {
                        foreach (var item in createjob.lstJobInverterDetails)
                        {
                            item.CECApprovedDate = !string.IsNullOrEmpty(item.CECApprovedDate) ? Convert.ToDateTime(item.CECApprovedDate).ToString("dd/MM/yyyy") : "";
                            item.ExpiryDate = !string.IsNullOrEmpty(item.ExpiryDate) ? Convert.ToDateTime(item.ExpiryDate).ToString("dd/MM/yyyy") : "";
                        }

                    }

                    createjob.lstJobBatteryManufacturer = dataSet.Tables[12].Rows.Count > 0 ? DBClient.DataTableToList<JobBatteryManufacturer>(dataSet.Tables[12]) : new List<JobBatteryManufacturer>();

                    if (createjob.BasicDetails != null && createjob.BasicDetails.JobType == 2 && createjob.lstJobPanelDetails.Count > 0)
                    {
                        createjob.JobSystemDetails.SystemBrand = createjob.lstJobPanelDetails[0].Brand;
                        createjob.JobSystemDetails.SystemModel = createjob.lstJobPanelDetails[0].Model;
                        createjob.JobSystemDetails.NoOfPanel = createjob.lstJobPanelDetails[0].NoOfPanel;

                        createjob.JobSystemDetails.CECApprovedDate = !string.IsNullOrEmpty(createjob.lstJobPanelDetails[0].CECApprovedDate) ? Convert.ToDateTime(createjob.lstJobPanelDetails[0].CECApprovedDate).ToString("dd/MM/yyyy") : "";
                        createjob.JobSystemDetails.ExpiryDate = !string.IsNullOrEmpty(createjob.lstJobPanelDetails[0].ExpiryDate) ? Convert.ToDateTime(createjob.lstJobPanelDetails[0].ExpiryDate).ToString("dd/MM/yyyy") : "";
                    }

                    //createjob.JobSerialNumbers = dataSet.Tables[12].Rows.Count > 0 ? DBClient.DataTableToList<JobSerialNumbers>(dataSet.Tables[12]) : new List<JobSerialNumbers>();
                    //if (createjob.JobSerialNumbers != null)
                    //{
                    //    createjob.JobSerialNumbers.ForEach(a => a.SerialNumber = HttpUtility.JavaScriptStringEncode(a.SerialNumber));
                    //}

                    createjob.JobInstallationDetails = JobInstallationDetailsEntity(dataSet.Tables[14]);
                    createjob.JobSystemDetails.NoOfInverter = createjob.lstJobInverterDetails.Count > 0 ? createjob.lstJobInverterDetails.Sum(x => x.NoOfInverter) : 0;
                }
                else
                {
                    if (DATAOPMODE != 4)
                    {
                        createjob.JobSTCDetails = new JobSTCDetails();
                    }
                    if (DATAOPMODE != 8)
                    {
                        createjob.JobSystemDetails = new JobSystemDetails();
                        createjob.JobSerialNumbers = new List<JobSerialNumbers>();
                    }

                    createjob.lstJobPanelDetails = new List<JobPanelDetails>();
                    createjob.lstJobInverterDetails = new List<JobInverterDetails>();
                    createjob.lstJobBatteryManufacturer = new List<JobBatteryManufacturer>();

                    if (DATAOPMODE != 3 && DATAOPMODE != 1 && DATAOPMODE != 4 && DATAOPMODE != 13 && DATAOPMODE != 14)
                    {
                        createjob.JobInstallationDetails = new JobInstallationDetails();
                    }
                }


                if (DATAOPMODE == 1 || DATAOPMODE == 4)
                {
                    if (DATAOPMODE == 1)
                    {
                        createjob.JobOwnerDetails = JobOwnerDetailsEntity(dataSet.Tables[1]);
                        createjob.JobInstallationDetails = JobInstallationDetailsEntity(dataSet.Tables[2]);

                        createjob.OwnerSignature = dataSet.Tables[1].Rows[0]["OwnerSignature"].ToString();

                        createjob.DesignerView = GetInstallerDesignerEntity(dataSet.Tables[3], false);
                        createjob.InstallerView = GetInstallerDesignerEntity(dataSet.Tables[4], true);
                        createjob.JobElectricians = JobElectriciansEntity(dataSet.Tables[5]);
                    }
                    else
                    {
                        createjob.JobOwnerDetails = JobOwnerDetailsEntity(dataSet.Tables[1]);
                    }
                }
                else
                {
                    if (DATAOPMODE != 3)
                    {
                        createjob.JobOwnerDetails = new JobOwnerDetails();
                    }
                }

                createjob.lstPVModules = new List<PVModules>();
                createjob.lstInverters = new List<Inverters>();
                //createjob.lstPVModules = dataSet.Tables[9].Rows.Count > 0 ? DBClient.DataTableToList<PVModules>(dataSet.Tables[9]) : new List<PVModules>();
                //createjob.lstInverters = dataSet.Tables[10].Rows.Count > 0 ? DBClient.DataTableToList<Inverters>(dataSet.Tables[10]) : new List<Inverters>();



                if (createjob.BasicDetails != null)
                {
                    createjob.IsOwnerSignUpload = createjob.BasicDetails.OwnerSignature != null ? true : false;
                    createjob.IsInstallerSignUpload = createjob.BasicDetails.InstallerSignature != null ? true : false;
                    createjob.IsElectricianSignUpload = createjob.BasicDetails.ElectricianSignature != null ? true : false;
                    createjob.IsDesigerSignUpload = createjob.BasicDetails.DesignerSignature != null ? true : false;
                }
                else
                {
                    createjob.IsOwnerSignUpload = false;
                    createjob.IsInstallerSignUpload = false;
                    createjob.IsElectricianSignUpload = false;
                    createjob.IsDesigerSignUpload = false;
                }

                if (DATAOPMODE == 6)
                {
                    createjob.DesignerView = GetInstallerDesignerEntity(dataSet.Tables[8], false);
                    createjob.InstallerView = GetInstallerDesignerEntity(dataSet.Tables[9], true);
                    createjob.JobElectricians = JobElectriciansEntity(dataSet.Tables[10]);

                    if (createjob.BasicDetails != null && createjob.BasicDetails.JobType == 2 && createjob.JobInstallerDetails != null)
                    {
                        createjob.JobInstallerDetails.SESignature = dataSet.Tables[4].Rows.Count > 0 ? dataSet.Tables[4].Rows[0]["SESignature"].ToString() : string.Empty;
                    }
                    else
                    {
                        createjob.JobInstallerDetails = new JobInstallerDetails();
                    }

                    createjob.JobInstallerDetails = JobInstallerEntity(dataSet.Tables[12]);

                    if (dataSet.Tables.Count > 5 && dataSet.Tables[13].Rows.Count > 0)
                    {
                        createjob.InstallerSignature = dataSet.Tables[13].Rows[0]["InstallerSignature"].ToString();
                        createjob.DesignerSignature = dataSet.Tables[13].Rows[0]["DesignerSignature"].ToString();
                        createjob.ElectricianSignature = dataSet.Tables[13].Rows[0]["ElectricianSignature"].ToString();
                        createjob.OwnerSignature = dataSet.Tables[13].Rows[0]["OwnerSignature"].ToString();
                    }
                }
                else
                {
                    if (DATAOPMODE != 1)
                    {
                        createjob.DesignerView = new InstallerDesignerView();
                        createjob.InstallerView = new InstallerDesignerView();
                        if (DATAOPMODE != 8 || createjob.JobElectricians == null)
                        {
                            createjob.JobElectricians = new JobElectricians();
                        }

                    }
                    createjob.JobInstallerDetails = new JobInstallerDetails();
                }

                createjob.InstallerDesignerView = new InstallerDesignerView();

                //createjob.ElectricianData = dataSet.Tables[18].Rows.Count > 0 ? dataSet.Tables[18] : new DataTable();
                //createjob.RetailerAutoSettingForSignature = dataSet.Tables[19].Rows.Count > 0 ? dataSet.Tables[19] : new DataTable();

                GetJobDetails(jobID, createjob);
            }
            return createjob;
        }

        //Get Documents Count
        public CreateJob GetDocumentsandPhotosTabCount(int jobID)
        {
            CreateJob createjob = new CreateJob();

            string spName = "[GetDocumentsandPhotosTabCount]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            DataSet dataSet = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                createjob.DocumentsAndPhotosCount  = new DocumentsAndPhotosCount();
                createjob.DocumentsAndPhotosCount.DocumentsCount = !String.IsNullOrEmpty(dataSet.Tables[0].Rows[0]["TotalDocumentCount"].ToString()) ? Convert.ToInt32(dataSet.Tables[0].Rows[0]["TotalDocumentCount"].ToString()) : 0;
                createjob.DocumentsAndPhotosCount.PhotosTotalCount = !String.IsNullOrEmpty(dataSet.Tables[1].Rows[0]["TotalPhotoCheckListItemCount"].ToString()) ? Convert.ToInt32(dataSet.Tables[1].Rows[0]["TotalPhotoCheckListItemCount"].ToString()) : 0;
                createjob.DocumentsAndPhotosCount.PhotosCount = !String.IsNullOrEmpty(dataSet.Tables[1].Rows[0]["TotalVisitedCount"].ToString()) ? Convert.ToInt32(dataSet.Tables[1].Rows[0]["TotalVisitedCount"].ToString()) : 0;
                createjob.DocumentsAndPhotosCount.SerialNumberCount = !String.IsNullOrEmpty(dataSet.Tables[2].Rows[0]["SerialNumberCount"].ToString()) ? Convert.ToInt32(dataSet.Tables[2].Rows[0]["SerialNumberCount"].ToString()) : 0;
                createjob.DocumentsAndPhotosCount.SerialNumberTotalCount = !String.IsNullOrEmpty(dataSet.Tables[3].Rows[0]["TotalSerialNumberCount"].ToString()) ? Convert.ToInt32(dataSet.Tables[3].Rows[0]["TotalSerialNumberCount"].ToString()) : 0;
            }
            return createjob;
        }

        //Job Summary Page
        public CreateJob GetJobSummaryTabular(int jobID)
        {
            CreateJob createjob = new CreateJob();

            string spName = "[Job_GetJobSummaryByJobIdTabular]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            DataSet dataSet = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                createjob.JobOwnerDetails = dataSet.Tables[0].AsEnumerable().Select(m => new JobOwnerDetails()
                {
                    DisplayOwnerFullAddress = m.Field<string>("OwnerDetails"),
                    OwnerType = m.Field<string>("OwnerType")
                }).FirstOrDefault();

                createjob.JobInstallationDetails = dataSet.Tables[1].AsEnumerable().Select(m => new JobInstallationDetails()
                {
                    AddressDisplay = m.Field<string>("Installationdetails"),
                    PropertyType = m.Field<string>("PropertyType"),
                    SingleMultipleStory = m.Field<string>("SingleMultipleStory"),
                    InstallingNewPanel = m.Field<string>("InstallingNewPanel")


                }).FirstOrDefault();

                createjob.JobSTCDetails = dataSet.Tables[2].AsEnumerable().Select(m => new JobSTCDetails()
                {
                    AdditionalCapacityNotes = m.Field<string>("AdditionalCapacityNotes"),
                    TypeOfConnection = m.Field<string>("TypeOfConnection"),
                    MultipleSGUAddress = m.Field<string>("MultipleSGUAddress"),
                    AdditionalLocationInformation = m.Field<string>("AdditionalLocationInformation")
                }).FirstOrDefault();

                createjob.JobInstallerDetails = dataSet.Tables[3].AsEnumerable().Select(m => new JobInstallerDetails()
                {
                    FirstName = m.Field<string>("FirstName"),
                    Surname = m.Field<string>("LastName"),
                    AccreditationNumber= m.Field<string>("CECAccreditationNumber")

                }).FirstOrDefault();

                createjob.InstallerDesignerView = dataSet.Tables[4].AsEnumerable().Select(m => new InstallerDesignerView()
                {
                    FirstName = m.Field<string>("FirstName"),
                    LastName = m.Field<string>("LastName"),
                    CECAccreditationNumber = m.Field<string>("CECAccreditationNumber")

                }).FirstOrDefault();

                createjob.JobElectricians = dataSet.Tables[5].AsEnumerable().Select(m => new JobElectricians()
                {
                    FirstName = m.Field<string>("FirstName"),
                    LastName = m.Field<string>("LastName"),
                    LicenseNumber = m.Field<string>("LicenseNumber")

                }).FirstOrDefault();

                createjob.lstJobPanelDetails = dataSet.Tables[6].Rows.Count > 0 ? DBClient.DataTableToList<JobPanelDetails>(dataSet.Tables[6]) : new List<JobPanelDetails>();
                if (createjob.lstJobPanelDetails.Count > 0)
                {
                    foreach (var item in createjob.lstJobPanelDetails)
                    {
                        item.CECApprovedDate = !string.IsNullOrEmpty(item.CECApprovedDate) ? Convert.ToDateTime(item.CECApprovedDate).ToString("dd/MM/yyyy") : "";
                        item.ExpiryDate = !string.IsNullOrEmpty(item.ExpiryDate) ? Convert.ToDateTime(item.ExpiryDate).ToString("dd/MM/yyyy") : "";
                    }
                }

                createjob.lstJobInverterDetails = dataSet.Tables[7].Rows.Count > 0 ? DBClient.DataTableToList<JobInverterDetails>(dataSet.Tables[7]) : new List<JobInverterDetails>();
                if (createjob.lstJobInverterDetails.Count > 0)
                {
                    foreach (var item in createjob.lstJobInverterDetails)
                    {
                        item.CECApprovedDate = !string.IsNullOrEmpty(item.CECApprovedDate) ? Convert.ToDateTime(item.CECApprovedDate).ToString("dd/MM/yyyy") : "";
                        item.ExpiryDate = !string.IsNullOrEmpty(item.ExpiryDate) ? Convert.ToDateTime(item.ExpiryDate).ToString("dd/MM/yyyy") : "";
                    }

                }

                createjob.lstJobBatteryManufacturer = dataSet.Tables[8].Rows.Count > 0 ? DBClient.DataTableToList<JobBatteryManufacturer>(dataSet.Tables[8]) : new List<JobBatteryManufacturer>();


            }
            return createjob;
        }


        public DataSet GetCommonSerialByID(int jobID, int UserTypeId)
        {
            CreateJob createjob = new CreateJob();
            string spName = "[GetCommonSerialNumber]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            DataSet dataSet = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dataSet;
        }
        public DataSet GetCommonInverterSerialByID(int jobID, int UserTypeId)
        {
            CreateJob createjob = new CreateJob();
            string spName = "[GetCommonInverterSerialNumber]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            DataSet dataSet = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dataSet;
        }
        /// <summary>
        /// Gets the job list.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="PageNumber">The page number.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <param name="SortCol">The sort col.</param>
        /// <param name="SortDir">The sort dir.</param>
        /// <param name="UrgentJobDay">The urgent job day.</param>
        /// <param name="StageId">The stage identifier.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <param name="IsArchive">if set to <c>true</c> [is archive].</param>
        /// <param name="ScheduleType">Type of the schedule.</param>
        /// <param name="JobType">Type of the job.</param>
        /// <param name="JobPriority">The job priority.</param>
        /// <param name="searchtext">The searchtext.</param>
        /// <param name="FromDate">From date.</param>
        /// <param name="ToDate">To date.</param>
        /// <param name="jobref">if set to <c>true</c> [jobref].</param>
        /// <param name="jobdescription">if set to <c>true</c> [jobdescription].</param>
        /// <param name="jobaddress">if set to <c>true</c> [jobaddress].</param>
        /// <param name="jobclient">if set to <c>true</c> [jobclient].</param>
        /// <param name="jobstaff">if set to <c>true</c> [jobstaff].</param>
        /// <param name="Invoiced">if set to <c>true</c> [invoiced].</param>
        /// <param name="NotInvoiced">if set to <c>true</c> [not invoiced].</param>
        /// <param name="ReadyToTrade">if set to <c>true</c> [ready to trade].</param>
        /// <param name="NotReadyToTrade">if set to <c>true</c> [not ready to trade].</param>
        /// <param name="traded">if set to <c>true</c> [traded].</param>
        /// <param name="nottraded">if set to <c>true</c> [nottraded].</param>
        /// <param name="preapprovalnotapproved">if set to <c>true</c> [preapprovalnotapproved].</param>
        /// <param name="preapprovalapproved">if set to <c>true</c> [preapprovalapproved].</param>
        /// <param name="connectioncompleted">if set to <c>true</c> [connectioncompleted].</param>
        /// <param name="connectionnotcompleted">if set to <c>true</c> [connectionnotcompleted].</param>
        /// <param name="ACT">if set to <c>true</c> [act].</param>
        /// <param name="NSW">if set to <c>true</c> [NSW].</param>
        /// <param name="NT">if set to <c>true</c> [nt].</param>
        /// <param name="QLD">if set to <c>true</c> [QLD].</param>
        /// <param name="SA">if set to <c>true</c> [sa].</param>
        /// <param name="TAS">if set to <c>true</c> [tas].</param>
        /// <param name="WA">if set to <c>true</c> [wa].</param>
        /// <param name="VIC">if set to <c>true</c> [vic].</param>
        /// <param name="PreApprovalStatusId">The pre approval status identifier.</param>
        /// <param name="ConnectionStatusId">The connection status identifier.</param>
        /// <returns>list of job</returns>
        public List<JobList> GetJobList(int UserId, int UserTypeId, int PageNumber, int PageSize, string SortCol, string SortDir, int UrgentJobDay, int StageId, string SolarCompanyId, bool IsArchive, int ScheduleType, int JobType, int JobPriority, string searchtext, DateTime? FromDate, DateTime? ToDate, bool IsGst, bool jobref, bool jobdescription, bool jobaddress, bool jobclient, bool jobstaff, bool Invoiced, bool NotInvoiced, bool ReadyToTrade, bool NotReadyToTrade, bool traded, bool nottraded, bool preapprovalnotapproved, bool preapprovalapproved, bool connectioncompleted, bool connectionnotcompleted, bool ACT, bool NSW, bool NT, bool QLD, bool SA, bool TAS, bool WA, bool VIC, int PreApprovalStatusId, int ConnectionStatusId, DateTime? FromDateInstalling, DateTime? ToDateInstalling)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, PageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, PageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, SortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, SortDir));
            sqlParameters.Add(DBClient.AddParameters("UrgentJobDay", SqlDbType.Int, UrgentJobDay));
            sqlParameters.Add(DBClient.AddParameters("StageId", SqlDbType.Int, StageId));
            //sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.NVarChar, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("IsArchive", SqlDbType.Bit, IsArchive));
            sqlParameters.Add(DBClient.AddParameters("ScheduleType", SqlDbType.Int, ScheduleType));
            sqlParameters.Add(DBClient.AddParameters("JobType", SqlDbType.Int, JobType));
            sqlParameters.Add(DBClient.AddParameters("IsGst", SqlDbType.Bit, IsGst));
            sqlParameters.Add(DBClient.AddParameters("JobPriority", SqlDbType.Int, JobPriority));
            sqlParameters.Add(DBClient.AddParameters("searchtext", SqlDbType.NVarChar, searchtext));
            sqlParameters.Add(DBClient.AddParameters("FromDate", SqlDbType.DateTime, FromDate != null ? FromDate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("ToDate", SqlDbType.DateTime, ToDate != null ? ToDate : (object)DBNull.Value));

            sqlParameters.Add(DBClient.AddParameters("Invoiced", SqlDbType.Bit, Invoiced));
            sqlParameters.Add(DBClient.AddParameters("NotInvoiced", SqlDbType.Bit, NotInvoiced));
            sqlParameters.Add(DBClient.AddParameters("ReadyToTrade", SqlDbType.Bit, ReadyToTrade));
            sqlParameters.Add(DBClient.AddParameters("NotReadyToTrade", SqlDbType.Bit, NotReadyToTrade));

            sqlParameters.Add(DBClient.AddParameters("IsJobRef", SqlDbType.Bit, jobref));
            sqlParameters.Add(DBClient.AddParameters("IsJobDescription", SqlDbType.Bit, jobdescription));
            sqlParameters.Add(DBClient.AddParameters("IsJobAddress", SqlDbType.Bit, jobaddress));
            sqlParameters.Add(DBClient.AddParameters("IsJobClient", SqlDbType.Bit, jobclient));
            sqlParameters.Add(DBClient.AddParameters("IsJobStaff", SqlDbType.Bit, jobstaff));

            sqlParameters.Add(DBClient.AddParameters("IsTraded", SqlDbType.Bit, traded));
            sqlParameters.Add(DBClient.AddParameters("IsNotTraded", SqlDbType.Bit, nottraded));
            sqlParameters.Add(DBClient.AddParameters("IsPreApprovalNotApproved", SqlDbType.Bit, preapprovalnotapproved));
            sqlParameters.Add(DBClient.AddParameters("IsPreApprovalApproved", SqlDbType.Bit, preapprovalapproved));
            sqlParameters.Add(DBClient.AddParameters("IsConnectionCompleted", SqlDbType.Bit, connectioncompleted));
            sqlParameters.Add(DBClient.AddParameters("IsConnectionNotCompleted", SqlDbType.Bit, connectionnotcompleted));

            sqlParameters.Add(DBClient.AddParameters("IsACT", SqlDbType.Bit, ACT));
            sqlParameters.Add(DBClient.AddParameters("IsNSW", SqlDbType.Bit, NSW));
            sqlParameters.Add(DBClient.AddParameters("IsNT", SqlDbType.Bit, NT));
            sqlParameters.Add(DBClient.AddParameters("IsQLD", SqlDbType.Bit, QLD));
            sqlParameters.Add(DBClient.AddParameters("IsSA", SqlDbType.Bit, SA));
            sqlParameters.Add(DBClient.AddParameters("IsTAS", SqlDbType.Bit, TAS));
            sqlParameters.Add(DBClient.AddParameters("IsWA", SqlDbType.Bit, WA));
            sqlParameters.Add(DBClient.AddParameters("IsVIC", SqlDbType.Bit, VIC));

            sqlParameters.Add(DBClient.AddParameters("PreApprovalStatusId", SqlDbType.Int, PreApprovalStatusId));
            sqlParameters.Add(DBClient.AddParameters("ConnectionStatusId", SqlDbType.Int, ConnectionStatusId));

            sqlParameters.Add(DBClient.AddParameters("FromDateInstalling", SqlDbType.DateTime, FromDateInstalling != null ? FromDateInstalling : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("ToDateInstalling", SqlDbType.DateTime, ToDateInstalling != null ? ToDateInstalling : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("CurrentDateTime", SqlDbType.DateTime, DateTime.Now));
            List<JobList> lstJobs = CommonDAL.ExecuteProcedure<JobList>("Job_GetJobList", sqlParameters.ToArray()).ToList();
            return lstJobs;
        }

        /// <summary>
        /// Deletes the selected jobs.
        /// </summary>
        /// <param name="lstJobs">The LST jobs.</param>
        /// <param name="ModifiedDate">The modified date.</param>
        /// <param name="LogginUserID">The login user identifier.</param>
        /// <returns>
        /// delete job
        /// </returns>
        public List<DeleteJob_Failed> DeleteSelectedJobs(List<int> lstJobs, DateTime ModifiedDate, int LogginUserID, int UserTypeId)
        {
            List<DeleteJob_Failed> lstDeletedJobs_Reason = new List<DeleteJob_Failed>();
            if (lstJobs != null && lstJobs.Count > 0)
            {
                DataSet dsUsers = new DataSet();
                var iDs = lstJobs.Select(i => i.ToString(CultureInfo.InvariantCulture)).Aggregate((s1, s2) => s1 + ", " + s2);
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("JobIDs", SqlDbType.NVarChar, iDs));
                sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, ModifiedDate));
                sqlParameters.Add(DBClient.AddParameters("LogginUserID", SqlDbType.Int, LogginUserID));
                sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
                lstDeletedJobs_Reason = CommonDAL.ExecuteProcedure<DeleteJob_Failed>("Jobs_DeleteSelectedJobs", sqlParameters.ToArray()).ToList();
                return lstDeletedJobs_Reason;
            }

            return lstDeletedJobs_Reason;
        }

        /// <summary>
        /// Open deleted jobs.
        /// </summary>
        /// <param name="lstJobs">The LST jobs.</param>
        /// <param name="ModifiedDate">The modified date.</param>
        /// <param name="LogginUserID">The login user identifier.</param>
        /// <returns>
        /// delete job
        /// </returns>
        public void OpenDeletedJobs(List<int> lstJobs, DateTime ModifiedDate, int LogginUserID)
        {
            if (lstJobs != null && lstJobs.Count > 0)
            {
                DataSet dsUsers = new DataSet();
                var iDs = lstJobs.Select(i => i.ToString(CultureInfo.InvariantCulture)).Aggregate((s1, s2) => s1 + ", " + s2);
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("JobIDs", SqlDbType.NVarChar, iDs));
                sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, ModifiedDate));
                sqlParameters.Add(DBClient.AddParameters("LogginUserID", SqlDbType.Int, LogginUserID));
                CommonDAL.Crud("Jobs_OpenDeletedJobs", sqlParameters.ToArray());
            }
        }

        /// <summary>
        /// Gets the sco user.
        /// </summary>
        /// <returns>
        /// reutn assign sco list
        /// </returns>
        public List<AssignSCO> GetSCOUser(int solarcompanyId)
        {
            string spName = "[User_GetSCOUser]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            //sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, FormBot.Helper.ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("solarcompanyId", SqlDbType.Int, solarcompanyId));
            IList<AssignSCO> userTypeList = CommonDAL.ExecuteProcedure<AssignSCO>(spName, sqlParameters.ToArray());
            return userTypeList.ToList();
        }

        /// <summary>
        /// Gets the assign job to list.
        /// </summary>
        /// <param name="logedinUserID">The logedin user identifier.</param>
        /// <param name="scoID">identifier.</param>
        /// <returns>
        /// list of user
        /// </returns>
        public List<SelectListItem> GetAssignJobToSCOList(int logedinUserID, int scoID)
        {
            string spName = "[JobSCOMapping_GetAssignedJobToSCO]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, logedinUserID));
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, scoID));
            var jobList = CommonDAL.ExecuteProcedure<AssignSCO>(spName, sqlParameters.ToArray())
            .Select(d => new SelectListItem()
            {
                Value = Convert.ToString(d.JobID),
                Text = d.Title
            }).ToList();
            return jobList;
        }

        /// <summary>
        /// Assigns the job.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>Get List of job</returns>
        public IEnumerable<SelectListItem> AssignJobToSCO(int userID)
        {
            string spName = "[JobDetails_GetAllJob]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.NVarChar, userID));
            IList<AssignSCO> jobList = CommonDAL.ExecuteProcedure<AssignSCO>(spName, sqlParameters.ToArray());
            return jobList.Select(d => new SelectListItem()
            {
                Value = Convert.ToString(d.JobID),
                Text = d.Title
            }).ToList();
        }

        /// <summary>
        /// Creates the job sco mapping.
        /// </summary>
        /// <param name="rAMID">The r amid.</param>
        /// <param name="solarCompanyIDs">The solar company i ds.</param>
        /// <returns>object</returns>
        public object CreateJobSCOMapping(int rAMID, string solarCompanyIDs)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.NVarChar, rAMID));
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.NVarChar, solarCompanyIDs));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("IsDeleted", SqlDbType.Bit, 0));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.Crud("JobSCOMapping_Insert", sqlParameters.ToArray());
            return null;
        }

        /// <summary>
        /// Gets the sco by user identifier.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>
        /// integer id
        /// </returns>
        public int GetSCOByUserId(int userID)
        {
            string spName = "[JobSCOMapping_GetSCOByUserId]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userID));
            object scoID = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            return Convert.ToInt32(scoID);
        }

        /// <summary>
        /// Gets all job to sco.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>select list</returns>
        public IEnumerable<SelectListItem> GetAllJobToSCO(int userID)
        {
            string spName = "[JobDetails_GetAllJobToSCO]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.NVarChar, userID));
            IList<AssignSCO> jobList = CommonDAL.ExecuteProcedure<AssignSCO>(spName, sqlParameters.ToArray());
            return jobList.Select(d => new SelectListItem()
            {
                Value = Convert.ToString(d.JobID),
                Text = d.Title
            }).ToList();
        }

        /// <summary>
        /// Get all selected jobs to assign SCO
        /// </summary>
        /// <param name="jobIds"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetAllJobsToAssignSCO(string jobIds)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobIds", SqlDbType.VarChar, jobIds));
            IList<AssignSCO> jobList = CommonDAL.ExecuteProcedure<AssignSCO>("GetAllJobsToAssignSCO", sqlParameters.ToArray());
            return jobList.Select(d => new SelectListItem()
            {
                Value = Convert.ToString(d.JobID),
                Text = d.FullJobDetails
            }).ToList();
        }

        /// <summary>
        /// Creates the job notes.
        /// </summary>
        /// <param name="notes">notes</param>
        /// <param name="jobID">job ID</param>
        /// <param name="createdBy">created By</param>
        public void CreateJobNotes(string notes, int jobID, int createdBy)
        {
            string spName = "[JobNotes_InsertJobNotes]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("Notes", SqlDbType.NVarChar, notes));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, createdBy));
            CommonDAL.Crud(spName, sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the job notes list.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="jobID">job ID</param>
        /// <returns>data set</returns>
        public DataSet GetJobNotesList(int pageIndex, int jobID)
        {
            string spName = "[JobNotes_GetJobList]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, 10));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageIndex));
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            DataSet dsUsers = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dsUsers;
        }

        /// <summary>
        /// Deletes the job notes.
        /// </summary>
        /// <param name="jobNotesId">The job notes identifier.</param>
        public void DeleteJobNotes(int jobNotesId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobNotesID", SqlDbType.Int, jobNotesId));
            CommonDAL.Crud("JobNotes_DeleteJobNotes", sqlParameters.ToArray());
        }

        /// <summary>
        /// Inserts the photo.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="status">The status.</param>
        public void InsertPhoto(int jobID, string filename, int status)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("Name", SqlDbType.NVarChar, filename));
            sqlParameters.Add(DBClient.AddParameters("Status", SqlDbType.Int, status));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.Crud("JobPhoto_Insert", sqlParameters.ToArray());
        }


        /// <summary>
        /// Inserts the photo.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="status">The status.</param>
        public DataSet InsertReferencePhoto(int jobID, string filename, int UserId, string jobscId, string cId, bool isDefault, string ClassType, string VendorJobPhotoId, string Status, string Latitude, string Longitude, string ImageTakenDate, string Manufaturer, string Model, bool isFromApp = false, string Altitude = "", string Accuracy = "")
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("Path", SqlDbType.NVarChar, filename));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));

            if (!string.IsNullOrEmpty(cId))
                sqlParameters.Add(DBClient.AddParameters("VisitCheckListItemId", SqlDbType.Int, Convert.ToInt32(cId)));

            if (!string.IsNullOrEmpty(jobscId))
                sqlParameters.Add(DBClient.AddParameters("JobSchedulingId", SqlDbType.Int, Convert.ToInt32(jobscId)));

            if (string.IsNullOrEmpty(jobscId) && string.IsNullOrEmpty(cId) && !isDefault)
                sqlParameters.Add(DBClient.AddParameters("IsReference", SqlDbType.Bit, true));


            if (isDefault)
            {
                sqlParameters.Add(DBClient.AddParameters("ClassType", SqlDbType.VarChar, ClassType));
                sqlParameters.Add(DBClient.AddParameters("IsDefault", SqlDbType.Bit, isDefault));
            }

            if (!string.IsNullOrEmpty(Status))
            {
                sqlParameters.Add(DBClient.AddParameters("Status", SqlDbType.Int, Convert.ToInt32(Status)));
            }
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("Latitude", SqlDbType.VarChar, Latitude));
            sqlParameters.Add(DBClient.AddParameters("Longitude", SqlDbType.VarChar, Longitude));
            sqlParameters.Add(DBClient.AddParameters("VendorJobPhotoId", SqlDbType.NVarChar, VendorJobPhotoId));

            if (!string.IsNullOrEmpty(ImageTakenDate))
                sqlParameters.Add(DBClient.AddParameters("ImageTakenDate", SqlDbType.NVarChar, ImageTakenDate));
            sqlParameters.Add(DBClient.AddParameters("Model", SqlDbType.NVarChar, Model));
            sqlParameters.Add(DBClient.AddParameters("Manufacturer", SqlDbType.NVarChar, Manufaturer));
            sqlParameters.Add(DBClient.AddParameters("IsFromApp", SqlDbType.Bit, isFromApp));
            sqlParameters.Add(DBClient.AddParameters("Altitude", SqlDbType.NVarChar, Altitude));
            sqlParameters.Add(DBClient.AddParameters("Accuracy", SqlDbType.NVarChar, Accuracy));

            DataSet jobPhoto = CommonDAL.ExecuteDataSet("InsertPreferencePhotos", sqlParameters.ToArray());
            return jobPhoto;

            //return Convert.ToInt32(CommonDAL.ExecuteScalar("InsertPreferencePhotos", sqlParameters.ToArray()));
        }

        /// <summary>
        /// Inserts the photo for API.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="status">The status.</param>
        /// <param name="UserID">The user identifier.</param>
        public void InsertPhotoForAPI(int jobID, string filename, int status, int UserID, string VendorJobPhotoId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("Name", SqlDbType.NVarChar, filename));
            sqlParameters.Add(DBClient.AddParameters("Status", SqlDbType.Int, status));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, UserID));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("VendorJobPhotoId", SqlDbType.NVarChar, VendorJobPhotoId));
            CommonDAL.Crud("JobPhoto_Insert", sqlParameters.ToArray());
        }

        /// <summary>
        /// Deletes the photo.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="jobID">The job identifier.</param>
        public void DeletePhoto(string filename, int jobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Name", SqlDbType.NVarChar, filename));
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            CommonDAL.Crud("JobPhoto_Delete", sqlParameters.ToArray());
        }

        /// <summary>
        /// Deletes the photo API.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="jobID">The job identifier.</param>
        public void DeletePhotoApi(string filename, int jobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Name", SqlDbType.NVarChar, filename));
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            CommonDAL.Crud("JobPhoto_Delete_Api", sqlParameters.ToArray());
        }

        /// <summary>
        /// Deletes the visit check list photos by path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="jobId">The job identifier.</param>
        public void DeleteVisitCheckListPhotosByPath(string path, int jobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Path", SqlDbType.NVarChar, path));
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobId));
            CommonDAL.Crud("DeleteVisitCheckListPhotosByPath", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the job installation photo by job identifier.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>user document</returns>
        public List<UserDocument> GetJobInstallationPhotoByJobID(int jobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            List<UserDocument> lstUserDocument = CommonDAL.ExecuteProcedure<UserDocument>("JobPhoto_GetJobInstallationPhotoByJobID", sqlParameters.ToArray()).ToList();
            if (lstUserDocument != null && lstUserDocument.Count > 0)
            {
                for (int i = 0; i < lstUserDocument.Count; i++)
                {
                    if (lstUserDocument[i].DocumentPath.Length > 20)
                    {
                        lstUserDocument[i].strDocumentPath = lstUserDocument[i].DocumentPath.Substring(0, 20) + "...";
                    }
                    else
                    {
                        lstUserDocument[i].strDocumentPath = lstUserDocument[i].DocumentPath;
                    }

                    if (System.Web.MimeMapping.GetMimeMapping(lstUserDocument[i].DocumentPath).ToLower().StartsWith("image"))
                    {
                        lstUserDocument[i].MimeType = "image";
                    }
                    lstUserDocument[i].index = i + 1;
                }

            }

            return lstUserDocument;
        }

        /// <summary>
        /// Gets the job installation photo by job identifier.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>user document</returns>
        public void GetJobDetails(int jobID, CreateJob job)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, ProjectSession.UserTypeId));
            //List<UserDocument> lstUserDocument = CommonDAL.ExecuteProcedure<UserDocument>("[Job_Details]", sqlParameters.ToArray()).ToList();
            DataSet dsJobDetails = CommonDAL.ExecuteDataSet("Job_Details", sqlParameters.ToArray());

            if (dsJobDetails != null && dsJobDetails.Tables.Count > 0 && dsJobDetails.Tables[0] != null && dsJobDetails.Tables[0].Rows.Count > 0)
                job.lstUserDocument = GetJobInstallationPhotoByJobID(dsJobDetails.Tables[0]);

            if (dsJobDetails != null && dsJobDetails.Tables.Count > 0 && dsJobDetails.Tables[1] != null && dsJobDetails.Tables[1].Rows.Count > 0)
                job.lstSerialDocument = GetJobInstallationSerialByJobID(dsJobDetails.Tables[1]);

            if (dsJobDetails != null && dsJobDetails.Tables.Count > 0 && dsJobDetails.Tables[2] != null && dsJobDetails.Tables[2].Rows.Count > 0)
                job.lstJobOtherDocument = GetJobOtherDocumentByJobID(dsJobDetails.Tables[2]);

            if (dsJobDetails != null && dsJobDetails.Tables.Count > 0 && dsJobDetails.Tables[3] != null && dsJobDetails.Tables[3].Rows.Count > 0)
                job.BasicDetails.ScoID = Convert.ToInt16(dsJobDetails.Tables[3].Rows[0]["Userid"].ToString());

            if (dsJobDetails != null && dsJobDetails.Tables.Count > 0 && dsJobDetails.Tables[4] != null && dsJobDetails.Tables[4].Rows.Count > 0)
                job.CommonSerialNumbers = dsJobDetails.Tables[4].Rows.Count > 0 ? dsJobDetails.Tables[4].ToListof<CommonSerialNumber>() : new List<CommonSerialNumber>();

            if (dsJobDetails != null && dsJobDetails.Tables.Count > 0 && dsJobDetails.Tables[5] != null && dsJobDetails.Tables[5].Rows.Count > 0)
                job.lstJobSchedule = GetJobschedulingByJobID(dsJobDetails.Tables[5]);

            if (dsJobDetails != null && dsJobDetails.Tables.Count > 0 && dsJobDetails.Tables[6] != null && dsJobDetails.Tables[6].Rows.Count > 0)
                job.CommonInverterSerialNumbers = dsJobDetails.Tables[6].Rows.Count > 0 ? dsJobDetails.Tables[6].ToListof<CommonInverterSerialNumber>() : new List<CommonInverterSerialNumber>();
        }

        /// <summary>
        /// Gets the job installation photo by job identifier using datatable.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>user document</returns>
        public List<UserDocument> GetJobInstallationPhotoByJobID(DataTable dt)
        {
            List<UserDocument> lstUserDocument = CommonDAL.DataTableToList<UserDocument>(dt);
            if (lstUserDocument != null && lstUserDocument.Count > 0)
            {
                for (int i = 0; i < lstUserDocument.Count; i++)
                {
                    if (lstUserDocument[i].DocumentPath.Length > 20)
                    {
                        lstUserDocument[i].strDocumentPath = lstUserDocument[i].DocumentPath.Substring(0, 20) + "...";
                    }
                    else
                    {
                        lstUserDocument[i].strDocumentPath = lstUserDocument[i].DocumentPath;
                    }

                    if (System.Web.MimeMapping.GetMimeMapping(lstUserDocument[i].DocumentPath).ToLower().StartsWith("image"))
                    {
                        lstUserDocument[i].MimeType = "image";
                    }
                    lstUserDocument[i].index = i + 1;
                }

            }

            return lstUserDocument;
        }

        /// <summary>
        /// Gets the job installation serial by job identifier.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>user document</returns>
        public List<UserDocument> GetJobInstallationSerialByJobID(int jobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            List<UserDocument> lstUserDocument = CommonDAL.ExecuteProcedure<UserDocument>("JobPhoto_GetJobInstallationSerialByJobID", sqlParameters.ToArray()).ToList();
            if (lstUserDocument != null && lstUserDocument.Count > 0)
            {
                for (int i = 0; i < lstUserDocument.Count; i++)
                {
                    if (lstUserDocument[i].DocumentPath.Length > 20)
                    {
                        lstUserDocument[i].strDocumentPath = lstUserDocument[i].DocumentPath.Substring(0, 20) + "...";
                    }
                    else
                    {
                        lstUserDocument[i].strDocumentPath = lstUserDocument[i].DocumentPath;
                    }

                    if (System.Web.MimeMapping.GetMimeMapping(lstUserDocument[i].DocumentPath).ToLower().StartsWith("image"))
                    {
                        lstUserDocument[i].MimeType = "image";
                    }

                    lstUserDocument[i].index = i + 1;
                }

            }

            return lstUserDocument;
        }

        /// <summary>
        /// Gets the job installation serial by job identifier using datatable.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>user document</returns>
        public List<UserDocument> GetJobInstallationSerialByJobID(DataTable dt)
        {
            //List<SqlParameter> sqlParameters = new List<SqlParameter>();
            //sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            //List<UserDocument> lstUserDocument = CommonDAL.ExecuteProcedure<UserDocument>("JobPhoto_GetJobInstallationSerialByJobID", sqlParameters.ToArray()).ToList();
            List<UserDocument> lstUserDocument = CommonDAL.DataTableToList<UserDocument>(dt);
            if (lstUserDocument != null && lstUserDocument.Count > 0)
            {
                for (int i = 0; i < lstUserDocument.Count; i++)
                {
                    if (lstUserDocument[i].DocumentPath.Length > 20)
                    {
                        lstUserDocument[i].strDocumentPath = lstUserDocument[i].DocumentPath.Substring(0, 20) + "...";
                    }
                    else
                    {
                        lstUserDocument[i].strDocumentPath = lstUserDocument[i].DocumentPath;
                    }

                    if (System.Web.MimeMapping.GetMimeMapping(lstUserDocument[i].DocumentPath).ToLower().StartsWith("image"))
                    {
                        lstUserDocument[i].MimeType = "image";
                    }

                    lstUserDocument[i].index = i + 1;
                }

            }

            return lstUserDocument;
        }

        /// <summary>
        /// Gets the job scheduling photos by job identifier.
        /// </summary>
        /// <param name="JobSchedulingId">The job scheduling identifier.</param>
        /// <returns></returns>
        public List<UserDocument> GetJobSchedulingPhotosByJobID(int JobSchedulingId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobSchedulingId", SqlDbType.Int, JobSchedulingId));
            List<UserDocument> lstUserDocument = CommonDAL.ExecuteProcedure<UserDocument>("GetVisitCheckListPhotosAPI", sqlParameters.ToArray()).ToList();

            return lstUserDocument;
        }

        /// <summary>
        /// Gets the ssc user.
        /// </summary>
        /// <returns>Basic Details</returns>
        public List<BasicDetails> GetSSCUser()
        {
            string spName = "[User_GetSSCUser]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, FormBot.Helper.ProjectSession.LoggedInUserId));
            IList<BasicDetails> userTypeList = CommonDAL.ExecuteProcedure<BasicDetails>(spName, sqlParameters.ToArray());
            return userTypeList.ToList();
        }

        /// <summary>
        /// Creates the job mapping.
        /// </summary>
        /// <param name="ramID">rAM ID</param>
        /// <param name="jobID">job ID</param>
        public void CreateJobSSCMapping(int ramID, int jobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, ramID));
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            CommonDAL.Crud("JobSSCMapping_Insert", sqlParameters.ToArray());
        }


        public DataSet InsertUserSignature(string jobDocId, string CaptureUserSignId, string signstring, bool IsImage, string fieldName, string mobileNo, string Firstname, string Lastname, int SignatureType, string Email)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(DBClient.AddParameters("JobDocID", SqlDbType.Int, Convert.ToInt32(jobDocId)));
            sqlParameters.Add(DBClient.AddParameters("CaptureUserSignId", SqlDbType.Int, Convert.ToInt32(CaptureUserSignId)));
            sqlParameters.Add(DBClient.AddParameters("SignString", SqlDbType.NVarChar, signstring));
            sqlParameters.Add(DBClient.AddParameters("IsImage", SqlDbType.Bit, IsImage));
            sqlParameters.Add(DBClient.AddParameters("FieldName", SqlDbType.NVarChar, fieldName));
            sqlParameters.Add(DBClient.AddParameters("MobileNumber", SqlDbType.NVarChar, mobileNo));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("Firstname", SqlDbType.NVarChar, Firstname));
            sqlParameters.Add(DBClient.AddParameters("Lastname", SqlDbType.NVarChar, Lastname));
            sqlParameters.Add(DBClient.AddParameters("SignatureType", SqlDbType.Int, SignatureType));
            sqlParameters.Add(DBClient.AddParameters("Email", SqlDbType.NVarChar, Email));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            DataSet ds = CommonDAL.ExecuteDataSet("Insert_UserSignature", sqlParameters.ToArray());
            return ds;
        }
        public DataSet InsertUserSignatureApi(string jobDocId, string signstring, bool IsImage, string fieldName, string mobileNo, string Firstname, string Lastname, int SignatureType, string Email)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobDocID", SqlDbType.Int, Convert.ToInt32(jobDocId)));
            sqlParameters.Add(DBClient.AddParameters("SignString", SqlDbType.NVarChar, signstring));
            sqlParameters.Add(DBClient.AddParameters("IsImage", SqlDbType.Bit, IsImage));
            sqlParameters.Add(DBClient.AddParameters("FieldName", SqlDbType.NVarChar, fieldName));
            sqlParameters.Add(DBClient.AddParameters("MobileNumber", SqlDbType.NVarChar, mobileNo));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("Firstname", SqlDbType.NVarChar, Firstname));
            sqlParameters.Add(DBClient.AddParameters("Lastname", SqlDbType.NVarChar, Lastname));
            sqlParameters.Add(DBClient.AddParameters("SignatureType", SqlDbType.Int, SignatureType));
            sqlParameters.Add(DBClient.AddParameters("Email", SqlDbType.NVarChar, Email));
            DataSet ds = CommonDAL.ExecuteDataSet("Insert_UserSignature_Api", sqlParameters.ToArray());
            return ds;
        }


        /// <summary>
        /// Gets the job scheduling  by job Id.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>job list</returns>
        public List<JobScheduling> GetJobschedulingByJobID(int jobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            List<JobScheduling> lstJobSchedule = CommonDAL.ExecuteProcedure<JobScheduling>("job_GetSchedulingByJobID", sqlParameters.ToArray()).ToList();
            if (lstJobSchedule != null && lstJobSchedule.Count > 0)
            {
                for (int i = 0; i < lstJobSchedule.Count; i++)
                {
                    lstJobSchedule[i].UserName = lstJobSchedule[i].FirstName + "  " + lstJobSchedule[i].LastName;
                    int month = lstJobSchedule[i].CreatedDate.Month;
                    lstJobSchedule[i].Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month).Substring(0, 3);
                    lstJobSchedule[i].Year = lstJobSchedule[i].CreatedDate.Year;
                    lstJobSchedule[i].Date = lstJobSchedule[i].CreatedDate.Day;

                    lstJobSchedule[i].StatusName = ((SystemEnums.UserStatus)lstJobSchedule[i].Status).ToString();
                    if (lstJobSchedule[i].CreatedDate.Minute < 10)
                    {
                        lstJobSchedule[i].time = lstJobSchedule[i].CreatedDate.Hour + ":" + "0" + lstJobSchedule[i].CreatedDate.Minute;
                    }
                    else
                    {
                        lstJobSchedule[i].time = lstJobSchedule[i].CreatedDate.Hour + ":" + lstJobSchedule[i].CreatedDate.Minute;
                    }

                    //string visitNum = string.Empty;
                    //if (lstJobSchedule[i].VisitNum.ToString().Length == 1)
                    //    visitNum = "00" + lstJobSchedule[i].VisitNum.ToString();
                    //else if (lstJobSchedule[i].VisitNum.ToString().Length == 2)
                    //    visitNum = "0" + lstJobSchedule[i].VisitNum.ToString();
                    //else
                    //    visitNum = lstJobSchedule[i].VisitNum.ToString();

                    //lstJobSchedule[i].UniqueVisitID = lstJobSchedule[i].CreatedDate.Year.ToString().Substring(2) + (lstJobSchedule[i].CreatedDate.Month.ToString().Length == 1 ? "0" + lstJobSchedule[i].CreatedDate.Month.ToString() : lstJobSchedule[i].CreatedDate.Month.ToString()) + lstJobSchedule[i].CreatedDate.Day.ToString() + visitNum;
                    lstJobSchedule[i].UniqueVisitID = lstJobSchedule[i].VisitUniqueId;
                    lstJobSchedule[i].strVisitStartDate = Convert.ToDateTime(lstJobSchedule[i].strVisitStartDate).ToString("dd/MM/yyyy hh:mm tt");
                    lstJobSchedule[i].strCompletedDate = Convert.ToDateTime(lstJobSchedule[i].CompletedDate).ToString("dd/MM/yyyy hh:mm tt");
                }
            }

            return lstJobSchedule;
        }

        /// <summary>
        /// Gets the job scheduling  by job Id.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>job list</returns>
        public List<JobScheduling> GetJobschedulingByJobID(DataTable dt)
        {
            //List<SqlParameter> sqlParameters = new List<SqlParameter>();
            //sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            //List<JobScheduling> lstJobSchedule = CommonDAL.ExecuteProcedure<JobScheduling>("job_GetSchedulingByJobID", sqlParameters.ToArray()).ToList();
            //List<JobScheduling> lstJobSchedule = CommonDAL.DataTableToList<JobScheduling>(dt);
            List<JobScheduling> lstJobSchedule = JobSchedulingEntity(dt);
            if (lstJobSchedule != null && lstJobSchedule.Count > 0)
            {
                for (int i = 0; i < lstJobSchedule.Count; i++)
                {
                    lstJobSchedule[i].UserName = lstJobSchedule[i].FirstName + "  " + lstJobSchedule[i].LastName;
                    int month = lstJobSchedule[i].CreatedDate.Month;
                    lstJobSchedule[i].Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month).Substring(0, 3);
                    lstJobSchedule[i].Year = lstJobSchedule[i].CreatedDate.Year;
                    lstJobSchedule[i].Date = lstJobSchedule[i].CreatedDate.Day;

                    lstJobSchedule[i].StatusName = ((SystemEnums.UserStatus)lstJobSchedule[i].Status).ToString();
                    if (lstJobSchedule[i].CreatedDate.Minute < 10)
                    {
                        lstJobSchedule[i].time = lstJobSchedule[i].CreatedDate.Hour + ":" + "0" + lstJobSchedule[i].CreatedDate.Minute;
                    }
                    else
                    {
                        lstJobSchedule[i].time = lstJobSchedule[i].CreatedDate.Hour + ":" + lstJobSchedule[i].CreatedDate.Minute;
                    }

                    //string visitNum = string.Empty;
                    //if (lstJobSchedule[i].VisitNum.ToString().Length == 1)
                    //    visitNum = "00" + lstJobSchedule[i].VisitNum.ToString();
                    //else if (lstJobSchedule[i].VisitNum.ToString().Length == 2)
                    //    visitNum = "0" + lstJobSchedule[i].VisitNum.ToString();
                    //else
                    //    visitNum = lstJobSchedule[i].VisitNum.ToString();

                    //lstJobSchedule[i].UniqueVisitID = lstJobSchedule[i].CreatedDate.Year.ToString().Substring(2) + (lstJobSchedule[i].CreatedDate.Month.ToString().Length == 1 ? "0" + lstJobSchedule[i].CreatedDate.Month.ToString() : lstJobSchedule[i].CreatedDate.Month.ToString()) + lstJobSchedule[i].CreatedDate.Day.ToString() + visitNum;
                    lstJobSchedule[i].UniqueVisitID = lstJobSchedule[i].VisitUniqueId;
                    lstJobSchedule[i].strVisitStartDate = Convert.ToDateTime(lstJobSchedule[i].strVisitStartDate).ToString("dd/MM/yyyy hh:mm tt");
                    lstJobSchedule[i].strCompletedDate = Convert.ToDateTime(lstJobSchedule[i].CompletedDate).ToString("dd/MM/yyyy hh:mm tt");
                }
            }

            return lstJobSchedule;
        }

        /// <summary>
        /// Creates the job documents.
        /// </summary>
        /// <param name="documentsView">The documents view.</param>
        /// <param name="isClassic"></param>
        /// <returns>
        /// integer id
        /// </returns>
        public int CreateJobDocuments(Entity.Documents.DocumentsView documentsView, bool isClassic)
        {
            string spName = "[JobDocument_InsertUpdate]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, documentsView.JobId));
            sqlParameters.Add(DBClient.AddParameters("DocumentId", SqlDbType.Int, documentsView.DocumentId == 0 ? null : documentsView.DocumentId));
            sqlParameters.Add(DBClient.AddParameters("isUpload", SqlDbType.Bit, documentsView.IsUpload));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, documentsView.CreatedDate));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, documentsView.CreatedBy));
            sqlParameters.Add(DBClient.AddParameters("JsonData", SqlDbType.NVarChar, documentsView.JsonData));
            sqlParameters.Add(DBClient.AddParameters("Path", SqlDbType.NVarChar, documentsView.Path));
            sqlParameters.Add(DBClient.AddParameters("isClassic", SqlDbType.Bit, isClassic));
            sqlParameters.Add(DBClient.AddParameters("VendorJobDocumentId", SqlDbType.NVarChar, documentsView.VendorJobDocumentId));
            if (!isClassic)
            {
                sqlParameters.Add(DBClient.AddParameters("JobDocumentId", SqlDbType.Int, documentsView.JobDocumentId));
            }

            if (!string.IsNullOrEmpty(documentsView.Type))
            {
                sqlParameters.Add(DBClient.AddParameters("Type", SqlDbType.NVarChar, documentsView.Type));
            }
            object userID = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            return Convert.ToInt32(userID);
        }

        /// <summary>
        /// Gets the SSC user by jb identifier.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns>Basic Details</returns>
        public List<BasicDetails> GetSSCUserByJbID(int jobID, int solarCompanyId)
        {
            string spName = "[User_GetSSCUserByJbID]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("SolarID", SqlDbType.Int, solarCompanyId));
            IList<BasicDetails> userTypeList = CommonDAL.ExecuteProcedure<BasicDetails>(spName, sqlParameters.ToArray());
            return userTypeList.ToList();
        }

        /// <summary>
        /// Gets the sscid.
        /// </summary>
        /// <param name="sscJOBID">The SSC jobid.</param>
        /// <returns>
        /// ineteger id
        /// </returns>
        public List<SolarSubContractor> GetSSCID(int sscJOBID)
        {
            string spName = "[JobDetails_GetSSCID]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, sscJOBID));
            IList<SolarSubContractor> sscID = CommonDAL.ExecuteProcedure<SolarSubContractor>(spName, sqlParameters.ToArray());
            return sscID.ToList();
            ////if (Convert.ToString(sscID) == string.Empty)
            ////{
            ////    return 0;
            ////}
            ////else
            ////{
            ////   return Convert.ToInt32(sscID);
            ////}
        }

        /// <summary>
        /// Gets the job stages with count.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <returns>job stage list</returns>
        public List<JobStage> GetJobStagesWithCount(int userId, int userTypeId, int SolarCompanyId)
        {
            string spName = "[Job_GetJobSatgesWithCount]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            List<JobStage> lstJobStage = CommonDAL.ExecuteProcedure<JobStage>(spName, sqlParameters.ToArray()).ToList();
            return lstJobStage;
        }

        /// <summary>
        ///  Delete Job Documents from database it will mark isDeleted to true.
        /// </summary>
        /// <param name="documentsView">documents View</param>
        /// <returns>integer id</returns>
        public int DeleteJobDocument(Entity.Documents.DocumentsView documentsView, string Path)
        {
            string spName = "[JobDocument_Delete]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, documentsView.JobId));
            sqlParameters.Add(DBClient.AddParameters("DocumentId", SqlDbType.Int, documentsView.DocumentId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, documentsView.CreatedDate));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, documentsView.CreatedBy));
            sqlParameters.Add(DBClient.AddParameters("JobDocumentId", SqlDbType.Int, documentsView.JobDocumentId));
            sqlParameters.Add(DBClient.AddParameters("Path", SqlDbType.VarChar, Path));
            object userID = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            return Convert.ToInt32(userID);
        }

        /// <summary>
        /// Deletes the job document new.
        /// </summary>
        /// <param name="JobDocumentId">The job document identifier.</param>
        /// <param name="Path">The path.</param>
        public void DeleteJobDocumentNew(int JobDocumentId, string Path)
        {
            string spName = "[JobDocument_DeleteNew]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobDocumentId", SqlDbType.Int, JobDocumentId));
            sqlParameters.Add(DBClient.AddParameters("Path", SqlDbType.VarChar, Path));
            CommonDAL.Crud(spName, sqlParameters.ToArray());

        }

        /// <summary>
        /// Inserts the other documents.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="logedInUserID">The login user identifier.</param>
        public void InsertOtherDocuments(int jobID, string filename, int logedInUserID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("FileName", SqlDbType.NVarChar, filename));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, logedInUserID));
            CommonDAL.Crud("JobDocument_InsertOtherDocument", sqlParameters.ToArray());
        }

        /// <summary>
        /// Inserts the other documents.
        /// </summary>
        /// <param name="JobId"></param>
        /// <param name="Path"></param>
        /// <param name="UserID"></param>
        /// <param name="Type"></param>
        /// <param name="JsonData"></param>
        /// <returns></returns>
        public int InsertCESDocuments(int JobId, string Path, int UserID, string Type, string JsonData, bool IsSPVXml, bool IsWrittenStatement)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserID));
            sqlParameters.Add(DBClient.AddParameters("Path", SqlDbType.VarChar, Path));
            sqlParameters.Add(DBClient.AddParameters("Type", SqlDbType.VarChar, Type));
            sqlParameters.Add(DBClient.AddParameters("JsonData", SqlDbType.VarChar, JsonData));
            sqlParameters.Add(DBClient.AddParameters("Date", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("IsSPVXml", SqlDbType.Bit, IsSPVXml));
            sqlParameters.Add(DBClient.AddParameters("IsWrittenStatement", SqlDbType.Bit, IsWrittenStatement));
            return Convert.ToInt32(CommonDAL.ExecuteScalar("Insert_CES_Document", sqlParameters.ToArray()));
            //Crud("Insert_CES_Document", sqlParameters.ToArray());
        }


        public DocumentsView GetJobDocumentByJobDocumentId(int jobDocumentId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobDocumentId", SqlDbType.BigInt, jobDocumentId));
            return CommonDAL.SelectObject<DocumentsView>("GetJobDocumentByJobDocumentId", sqlParameters.ToArray());
        }


        /// <summary>
        /// Gets the job other document by job identifier.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>User Document</returns>
        public List<UserDocument> GetJobOtherDocumentByJobID(int jobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            List<UserDocument> lstUserDocument = CommonDAL.ExecuteProcedure<UserDocument>("JobDocument_GetJobDocumentByJobID", sqlParameters.ToArray()).ToList();

            if (lstUserDocument != null && lstUserDocument.Count > 0)
            {
                for (int i = 0; i < lstUserDocument.Count; i++)
                {
                    if (lstUserDocument[i].DocumentPath.Length > 20)
                    {
                        lstUserDocument[i].strDocumentPath = lstUserDocument[i].DocumentPath.Substring(0, 20) + "...";
                    }

                    else
                    {
                        lstUserDocument[i].strDocumentPath = lstUserDocument[i].DocumentPath;
                    }

                    if (System.Web.MimeMapping.GetMimeMapping(lstUserDocument[i].DocumentPath).ToLower().StartsWith("image"))
                    {
                        lstUserDocument[i].MimeType = "image";
                    }

                    lstUserDocument[i].index = i + 1;
                }
            }

            return lstUserDocument;
        }

        /// <summary>
        /// Gets the job other document by job identifier using datatable.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>User Document</returns>
        public List<UserDocument> GetJobOtherDocumentByJobID(DataTable dt)
        {
            //List<SqlParameter> sqlParameters = new List<SqlParameter>();
            //sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            //List<UserDocument> lstUserDocument = CommonDAL.ExecuteProcedure<UserDocument>("JobDocument_GetJobDocumentByJobID", sqlParameters.ToArray()).ToList();
            //List<UserDocument> lstUserDocument = CommonDAL.DataTableToList<UserDocument>(dt);
            List<UserDocument> lstUserDocument = GetUserDocumentEntity(dt);
            if (lstUserDocument != null && lstUserDocument.Count > 0)
            {
                for (int i = 0; i < lstUserDocument.Count; i++)
                {
                    if (lstUserDocument[i].DocumentPath.Length > 20)
                    {
                        lstUserDocument[i].strDocumentPath = lstUserDocument[i].DocumentPath.Substring(0, 20) + "...";
                    }

                    else
                    {
                        lstUserDocument[i].strDocumentPath = lstUserDocument[i].DocumentPath;
                    }

                    if (System.Web.MimeMapping.GetMimeMapping(lstUserDocument[i].DocumentPath).ToLower().StartsWith("image"))
                    {
                        lstUserDocument[i].MimeType = "image";
                    }

                    lstUserDocument[i].index = i + 1;
                }
            }

            return lstUserDocument;
        }

        /// <summary>
        /// Get job document by job id
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        public List<DocumentsView> GetJobDocumentByJobID(int jobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            List<DocumentsView> lstUserDocument = CommonDAL.ExecuteProcedure<DocumentsView>("Document_GetJobDocumentByJobID", sqlParameters.ToArray()).ToList();
            return lstUserDocument;
        }



        /// <summary>
        /// Deletes the other document.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="JobID">The job identifier.</param>
        public void DeleteOtherDocument(string filename, int JobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Name", SqlDbType.NVarChar, filename));
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobID));
            CommonDAL.Crud("JobOtherDocument_Delete", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets all calendar data.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns>data set</returns>
        public DataSet GetAllCalendarData(int userId, int userTypeId, int solarCompanyId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.BigInt, userId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.BigInt, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.BigInt, solarCompanyId));
            DataSet dsJobSchedulingDetail = CommonDAL.ExecuteDataSet("Calendar_GetAllData", sqlParameters.ToArray());
            return dsJobSchedulingDetail;
        }


        /// <summary>
        /// Gets the type of the documents by.
        /// </summary>
        /// <param name="jobid">The jobid.</param>
        /// <returns>
        /// data set
        /// </returns>
        //public DataSet GetDocumentsByType(int jobid, string type)
        public DataSet GetDocumentsByType(int jobid)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobid));
            // sqlParameters.Add(DBClient.AddParameters("Type", SqlDbType.VarChar, type));
            return CommonDAL.ExecuteDataSet("GetDocumentsByType", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the identifier by job identifier.
        /// </summary>
        /// <param name="JobID">The job identifier.</param>
        /// <returns>
        /// integer id
        /// </returns>
        public int GetSCOIdByJobId(int JobID)
        {
            string spName = "[Job_GetSCOByJobId]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobID));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            object scoID = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            if (Convert.ToString(scoID) == string.Empty)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(scoID);
            }

        }


        /// <summary>
        /// Gets the job by identifier for PDF.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="solarCompanyID">The solar company identifier.</param>
        /// <param name="installerSignPath">The installer sign path.</param>
        /// <param name="ownerSignPath">The owner sign path.</param>
        /// <param name="sCASignPath">The path.</param>
        /// <param name="electricianSignPath">The electrician sign path.</param>
        /// <returns>
        /// data set
        /// </returns>
        public DataSet GetJobByIDForPDF(int jobID, int solarCompanyID, string installerSignPath, string ownerSignPath, string sCASignPath, string electricianSignPath, DateTime createdDate)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.Int, solarCompanyID));
            sqlParameters.Add(DBClient.AddParameters("InstallerSignPath", SqlDbType.NVarChar, installerSignPath));
            sqlParameters.Add(DBClient.AddParameters("OwnerSignPath", SqlDbType.NVarChar, ownerSignPath));
            sqlParameters.Add(DBClient.AddParameters("SCASignPath", SqlDbType.NVarChar, sCASignPath));
            sqlParameters.Add(DBClient.AddParameters("ElectricianSignPath", SqlDbType.NVarChar, electricianSignPath));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, createdDate));
            sqlParameters.Add(DBClient.AddParameters("CurrentDateTime", SqlDbType.DateTime, DateTime.Now));
            DataSet ds = CommonDAL.ExecuteDataSet("Job_GetJobByIDForPDF_New", sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Gets the header details.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>data set</returns>
        public DataSet GetHeaderDetails(int jobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            DataSet ds = CommonDAL.ExecuteDataSet("JobOwnerDetails_Header", sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Gets the job owner signature.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="ownerSignature">The owner signature.</param>
        /// <param name="Latitude">The latitude.</param>
        /// <param name="Longitude">The longitude.</param>
        /// <param name="IpAddress">The address.</param>
        /// <param name="Location">The location.</param>
        /// <param name="SignatureDate">The signature date.</param>
        public void GetJobOwnerSignature(int jobId, string ownerSignature, string Latitude, string Longitude, string IpAddress, string Location, DateTime SignatureDate, string path)
        {
            string spName = "[Job_GetOwnerSignature]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("OwnerSignature", SqlDbType.NVarChar, ownerSignature));
            sqlParameters.Add(DBClient.AddParameters("Latitude", SqlDbType.NVarChar, Latitude));
            sqlParameters.Add(DBClient.AddParameters("Longitude", SqlDbType.NVarChar, Longitude));
            sqlParameters.Add(DBClient.AddParameters("IpAddress", SqlDbType.NVarChar, IpAddress));
            sqlParameters.Add(DBClient.AddParameters("Location", SqlDbType.NVarChar, Location));
            sqlParameters.Add(DBClient.AddParameters("SignatureDate", SqlDbType.DateTime, SignatureDate));
            sqlParameters.Add(DBClient.AddParameters("Path", SqlDbType.NVarChar, path));
            CommonDAL.Crud(spName, sqlParameters.ToArray());
        }

        /// <summary>
        /// Checks the status and installation date.
        /// </summary>
        /// <param name="InstallerId">The installer identifier.</param>
        /// <param name="DesignerId">The designer identifier.</param>
        /// <returns>
        /// data set
        /// </returns>
        public DataSet CheckStatusAndInstallationDate(int InstallerId, int DesignerId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("InstallerUserId", SqlDbType.Int, InstallerId));
            sqlParameters.Add(DBClient.AddParameters("DesignerUserId ", SqlDbType.Int, DesignerId));
            DataSet ds = CommonDAL.ExecuteDataSet("Job_CheckStatusAndInstallationDate", sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Updates the priority for jobs.
        /// </summary>
        /// <param name="urgentJobDay">The urgent job day.</param>
        public void UpdatePriorityForJobs(int urgentJobDay)
        {
            string spName = "[Jobs_UpdatePriorityForJobs]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UrgentJobDay", SqlDbType.Int, urgentJobDay));
            sqlParameters.Add(DBClient.AddParameters("CurrentDateTime", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.Crud(spName, sqlParameters.ToArray());
        }

        /// <summary>
        /// Inserts the record data.
        /// </summary>
        /// <param name="dtRECData">The dt record data.</param>
        /// <param name="createdDate">The created date.</param>
        /// <returns>DataSet</returns>
        public DataSet InsertRECData(DataTable dtRECData, DateTime createdDate)
        {
            string spName = "[InsertRECData]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("TableRECData", SqlDbType.Structured, dtRECData));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.Date, createdDate));
            DataSet ds = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Inserts the record failure job reason.
        /// </summary>
        /// <param name="dtReason">The date reason.</param>
        public void InsertRECFailureJobReason(DataTable dtReason)
        {
            string spName = "[InsertRECFailureJobReason]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("TableRECFailureJobReason", SqlDbType.Structured, dtReason));
            CommonDAL.Crud(spName, sqlParameters.ToArray());
        }

        /// <summary>
        /// Checks the record data exist for date.
        /// </summary>
        /// <param name="createdDate">The created date.</param>
        /// <returns>bool</returns>
        public bool CheckRECDataExistForDate(DateTime createdDate)
        {
            string spName = "[CheckRECDataExistForDate]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.Date, createdDate));
            ////CommonDAL.Crud(spName, sqlParameters.ToArray());
            return Convert.ToBoolean(CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray()));
        }

        /// <summary>
        /// Gets the Theme and other Detail by jobId.
        /// </summary>
        /// <param name="JobID">The job identifier.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet GetThemeByJobId(int JobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobID));
            DataSet ds = CommonDAL.ExecuteDataSet("GetThemeByJobId", sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Gets the drop down values by job identifier.
        /// </summary>
        /// <param name="UnitTypeID">The unit type identifier.</param>
        /// <param name="StreetTypeId">The street type identifier.</param>
        /// <param name="PostalAddressID">The postal address identifier.</param>
        /// <param name="Model">The model.</param>
        /// <param name="Brand">The brand.</param>
        /// <returns>DataSet</returns>
        public DataSet GetDropDownValuesByJobId(int UnitTypeID, int StreetTypeId, int PostalAddressID, int Model, int Brand)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UnitTypeID", SqlDbType.Int, UnitTypeID));
            sqlParameters.Add(DBClient.AddParameters("StreetTypeId", SqlDbType.Int, StreetTypeId));
            sqlParameters.Add(DBClient.AddParameters("PostalAddressID", SqlDbType.Int, PostalAddressID));
            sqlParameters.Add(DBClient.AddParameters("Model", SqlDbType.Int, Model));
            sqlParameters.Add(DBClient.AddParameters("Brand", SqlDbType.Int, Brand));
            DataSet ds = CommonDAL.ExecuteDataSet("GetDropDownValuesByJobId", sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Gets the jobs for custom pricing.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <param name="SystemSize">Size of the system.</param>
        /// <param name="OwnerName">Name of the owner.</param>
        /// <param name="OwnerAddress">The owner address.</param>
        /// <param name="RefNumber">The reference number.</param>
        /// <returns>job list</returns>
        public List<JobList> GetJobsForCustomPricing(int UserId, int UserTypeId, int ResellerId, int SolarCompanyId, int SystemSize, string OwnerName, string OwnerAddress, string RefNumber)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("OwnerName", SqlDbType.NVarChar, OwnerName));
            sqlParameters.Add(DBClient.AddParameters("OwnerAddress", SqlDbType.NVarChar, OwnerAddress));
            sqlParameters.Add(DBClient.AddParameters("RefNumber", SqlDbType.NVarChar, RefNumber));
            sqlParameters.Add(DBClient.AddParameters("SystemSize", SqlDbType.Int, SystemSize));
            List<JobList> lstJobs = CommonDAL.ExecuteProcedure<JobList>("GetJobsForCustomPricing", sqlParameters.ToArray()).ToList();
            return lstJobs;
        }

        /// <summary>
        /// Removes the SSC request.
        /// </summary>
        /// <param name="notes">The notes.</param>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="requestedBy">The requested by.</param>
        public void RemoveSSCRequest(string notes, int jobID, int requestedBy)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("Notes", SqlDbType.NVarChar, notes));
            sqlParameters.Add(DBClient.AddParameters("SSCRequestedBy", SqlDbType.Int, requestedBy));
            sqlParameters.Add(DBClient.AddParameters("SSCRemoveDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.Crud("JobDetails_RemoveSSCRequest", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the job history list.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="order">The order.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <param name="categoryID">The category identifier.</param>
        /// <returns>
        /// data set
        /// </returns>
        public DataSet GetJobHistoryList(int jobID, string order, DateTime? fromDate, DateTime? toDate, int? categoryID, int pageIndex = 1)
        {
            string spName = "[JobHistory_Select]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("Order", SqlDbType.VarChar, order));
            sqlParameters.Add(DBClient.AddParameters("CategoryID", SqlDbType.Int, categoryID));
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("FromDate", SqlDbType.DateTime, fromDate));
            sqlParameters.Add(DBClient.AddParameters("ToDate", SqlDbType.DateTime, toDate));
            sqlParameters.Add(DBClient.AddParameters("PageIndex", SqlDbType.Int, pageIndex));
            DataSet dsUsers = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dsUsers;
        }

        public DataSet GetJobHistory(int jobID, string order, int? categoryID)
        {
            string spName = "[JobHistory_Select_new]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("Order", SqlDbType.VarChar, order));
            sqlParameters.Add(DBClient.AddParameters("CategoryID", SqlDbType.Int, categoryID));
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, ProjectSession.LoggedInUserId));


            DataSet dsUsers = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dsUsers;
        }

        public DataSet GetJobHistoryForJobDetail(int jobID, string order, int? categoryID)
        {
            string spName = "[JobHistory_Select_JobDetail]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("Order", SqlDbType.VarChar, order));
            sqlParameters.Add(DBClient.AddParameters("CategoryID", SqlDbType.Int, categoryID));
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, ProjectSession.LoggedInUserId));


            DataSet ds = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Gets the job STC submission.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="PageNumber">The page number.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <param name="SortCol">The sort col.</param>
        /// <param name="SortDir">The sort dir.</param>
        /// <param name="StageId">The stage identifier.</param>
        /// <param name="ComplianceOfficcerId">The compliance officcer identifier.</param>
        /// <param name="ResellerId">ResellerId.</param>
        /// <param name="RamId">RamId.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <param name="pvdswhcode">The pvdswhcode.</param>
        /// <param name="RefJobId">The reference job identifier.</param>
        /// <param name="ownername">The ownername.</param>
        /// <param name="installationaddress">The installationaddress.</param>
        /// <param name="SubmissionFromDate">The submission from date.</param>
        /// <param name="SubmissionToDate">The submission to date.</param>
        /// <param name="SettlementFromDate">The settlement from date.</param>
        /// <param name="SettlementToDate">The settlement to date.</param>
        /// <param name="Invoiced">The invoiced.</param>
        /// <returns>job list</returns>
        public DataSet GetJobSTCSubmissionKendo(string SolarCompanyId, int ResellerId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.NVarChar, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("CurrentDateTime", SqlDbType.DateTime, DateTime.Now));
            return CommonDAL.ExecuteDataSet("Job_GetJobSTCSubmission_Kendo", sqlParameters.ToArray());

        }

        public DataSet GetJobSTCSubmissionKendoByYear(string SolarCompanyId, int ResellerId, int Year)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.NVarChar, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("CurrentDateTime", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("Year", SqlDbType.Int, Year));
            return CommonDAL.ExecuteDataSet("Job_GetJobSTCSubmission_KendoByYear", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the job STC submission old page without kendo .
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="PageNumber">The page number.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <param name="SortCol">The sort col.</param>
        /// <param name="SortDir">The sort dir.</param>
        /// <param name="StageId">The stage identifier.</param>
        /// <param name="ComplianceOfficcerId">The compliance officcer identifier.</param>
        /// <param name="ResellerId">ResellerId.</param>
        /// <param name="RamId">RamId.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <param name="pvdswhcode">The pvdswhcode.</param>
        /// <param name="RefJobId">The reference job identifier.</param>
        /// <param name="ownername">The ownername.</param>
        /// <param name="installationaddress">The installationaddress.</param>
        /// <param name="SubmissionFromDate">The submission from date.</param>
        /// <param name="SubmissionToDate">The submission to date.</param>
        /// <param name="SettlementFromDate">The settlement from date.</param>
        /// <param name="SettlementToDate">The settlement to date.</param>
        /// <param name="Invoiced">The invoiced.</param>
        /// <returns>job list</returns>
        public List<JobList> GetJobSTCSubmission(int UserId, int UserTypeId, int PageNumber, int PageSize, string SortCol, string SortDir, int StageId, int ComplianceOfficcerId, int ResellerId, int RamId, int SolarCompanyId, string pvdswhcode, string RefJobId, string ownername, string installationaddress, DateTime? SubmissionFromDate, DateTime? SubmissionToDate, DateTime? SettlementFromDate, DateTime? SettlementToDate, int Invoiced, int SettlementTermId, string isAllScaJobView = "", string isShowOnlyAssignJobsSCO = "", string JobID = "", int? isSPVRequired = null, int? isSPVInstallationVerified = null)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, PageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, PageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, SortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, SortDir));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("RamId", SqlDbType.Int, RamId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("StageId", SqlDbType.Int, StageId));
            sqlParameters.Add(DBClient.AddParameters("ComplianceOfficcerId", SqlDbType.Int, ComplianceOfficcerId));
            sqlParameters.Add(DBClient.AddParameters("PVDSWHCode", SqlDbType.NVarChar, pvdswhcode));
            sqlParameters.Add(DBClient.AddParameters("RefJobId", SqlDbType.NVarChar, RefJobId));
            sqlParameters.Add(DBClient.AddParameters("OwnerName", SqlDbType.NVarChar, ownername));
            sqlParameters.Add(DBClient.AddParameters("InstallationAddress", SqlDbType.NVarChar, installationaddress.Trim()));
            sqlParameters.Add(DBClient.AddParameters("SubmissionFromDate", SqlDbType.DateTime, SubmissionFromDate != null ? SubmissionFromDate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("SubmissionToDate", SqlDbType.DateTime, SubmissionToDate != null ? SubmissionToDate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("SettlementFromDate", SqlDbType.DateTime, SettlementFromDate != null ? SettlementFromDate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("SettlementToDate", SqlDbType.DateTime, SettlementToDate != null ? SettlementToDate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("Invoiced", SqlDbType.Int, Invoiced));
            sqlParameters.Add(DBClient.AddParameters("SettlementTermId", SqlDbType.Int, SettlementTermId));
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.NVarChar, JobID));
            sqlParameters.Add(DBClient.AddParameters("IsAllScaJobView", SqlDbType.Bit, string.IsNullOrEmpty(isAllScaJobView) ? false : Convert.ToBoolean(isAllScaJobView)));
            sqlParameters.Add(DBClient.AddParameters("IsShowOnlyAssignJobsSCO", SqlDbType.Bit, string.IsNullOrEmpty(isShowOnlyAssignJobsSCO) ? false : Convert.ToBoolean(isShowOnlyAssignJobsSCO)));
            ////sqlParameters.Add(DBClient.AddParameters("PanelInverterDetails", SqlDbType.NVarChar, panelinverterdetails));
            sqlParameters.Add(DBClient.AddParameters("IsSPVRequired", SqlDbType.Bit, isSPVRequired));
            sqlParameters.Add(DBClient.AddParameters("IsSPVInstallationVerified", SqlDbType.Int, isSPVInstallationVerified));
            List<JobList> lstJobsSTCSubmission = CommonDAL.ExecuteProcedure<JobList>("Job_GetJobSTCSubmission", sqlParameters.ToArray()).ToList();
            return lstJobsSTCSubmission;
        }

        /// <summary>
        /// Updates the settlement date.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <param name="SettlementDate">The settlement date.</param>
        public void UpdateSettlementDate(int JobId, DateTime SettlementDate)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCJobDetailsId", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("SettlementDate", SqlDbType.DateTime, SettlementDate));
            CommonDAL.Crud("Jobs_UpdateSettlementDate", sqlParameters.ToArray());
        }

        /// <summary>
        /// Updates the PVDSWH code.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <param name="PVDSWHCode">The PVDSWH code.</param>
        public void UpdatePVDSWHCode(int JobId, string PVDSWHCode)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCJobDetailsId", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("PVDSWHCode", SqlDbType.VarChar, PVDSWHCode));
            CommonDAL.Crud("Jobs_UpdatePVDSWHCode", sqlParameters.ToArray());
        }


        /// <summary>
        /// Update Compliance Note
        /// </summary>
        /// <param name="JobId"></param>
        /// <param name="PVDSWHCode"></param>
        public void UpdateComplianceNote(int stcJobdetailsId, string CompalinceNote)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCJobDetailsId", SqlDbType.Int, stcJobdetailsId));
            sqlParameters.Add(DBClient.AddParameters("ComplianceNote", SqlDbType.VarChar, CompalinceNote));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, ProjectSession.UserTypeId));
            CommonDAL.Crud("Jobs_UpdateComplianceNote", sqlParameters.ToArray());
        }

        /// <summary>
        /// Update Compliance Note
        /// </summary>
        /// <param name="JobId"></param>
        /// <param name="PVDSWHCode"></param>
        public void UpdateComplianceNoteById(string stcJobdetailsId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCJobDetailsId", SqlDbType.NVarChar, stcJobdetailsId));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.Crud("Jobs_UpdateComplianceNoteById", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the STC job history.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <returns>history list</returns>
        public List<STCJobHistory> GetSTCJobHistory(int userId, int jobId, int userTypeId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, userId));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userTypeId));
            List<STCJobHistory> lstSTCJobHistory = CommonDAL.ExecuteProcedure<STCJobHistory>("STCJobHistory_GetJobHistory", sqlParameters.ToArray()).ToList();
            return lstSTCJobHistory;
        }

        /// <summary>
        /// Gets the STC submission count.
        /// </summary>
        /// <param name="jobid">The job identifier.</param>
        /// <returns>
        /// general class
        /// </returns>
        public List<GeneralClass> GetStcSubmissionCount(int jobid)
        {
            string spName = "[GetStcSubmissionCount]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("jobid", SqlDbType.Int, jobid));
            DataSet dsStc = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            List<GeneralClass> lstSubmission = new List<GeneralClass>();
            lstSubmission = dsStc.Tables[0].ToListof<GeneralClass>();
            return lstSubmission;
        }

        /// <summary>
        /// Updates the pv module waltage.
        /// </summary>
        /// <param name="PVModuleId">The pv module identifier.</param>
        /// <param name="Wattage">The wattage.</param>
        public void UpdatePVModuleWaltage(int PVModuleId, int Wattage)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("PVModuleId", SqlDbType.Int, PVModuleId));
            sqlParameters.Add(DBClient.AddParameters("Wattage", SqlDbType.Int, Wattage));
            CommonDAL.Crud("Jobs_UpdatePVModuleWaltage", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the STC job stages with count.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="ResellerId">ResellerId.</param>
        /// <param name="RamId">RamId.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <returns>stage list</returns>
        public List<JobStage> GetSTCJobStagesWithCount(int UserId, int UserTypeId, int ResellerId, int RamId, int SolarCompanyId, string isAllScaJobView, string isShowOnlyAssignJobsSCO = "")
        {
            string spName = "[Job_GetSTCJobSatgesWithCount]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("RamId", SqlDbType.Int, RamId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("IsAllScaJobView", SqlDbType.Bit, string.IsNullOrEmpty(isAllScaJobView) ? false : Convert.ToBoolean(isAllScaJobView)));
            sqlParameters.Add(DBClient.AddParameters("isShowOnlyAssignJobsSCO", SqlDbType.Bit, string.IsNullOrEmpty(isShowOnlyAssignJobsSCO) ? false : Convert.ToBoolean(isShowOnlyAssignJobsSCO)));
            List<JobStage> lstSTCJobStage = CommonDAL.ExecuteProcedure<JobStage>(spName, sqlParameters.ToArray()).ToList();
            return lstSTCJobStage;
        }

        /// <summary>
        /// Gets the STC sub records for job.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <returns>job list</returns>
        public List<JobList> GetSTCSubRecordsForJob(int JobId)
        {
            string spName = "[Job_GetSTCSubRecordsForJob]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            List<JobList> lstSTCSubRecords = CommonDAL.ExecuteProcedure<JobList>(spName, sqlParameters.ToArray()).ToList();
            return lstSTCSubRecords;
        }

        /// <summary>
        /// Checks the business rules.
        /// </summary>
        /// <param name="createJob">The create job.</param>
        /// <param name="xmlPanels">The XML panels.</param>
        /// <param name="xmlInverters">The XML inverters.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <returns>
        /// data set
        /// </returns>
        public DataSet CheckBusinessRules(CreateJob createJob, string xmlPanels, string xmlInverters, int? jobId = 0)
        {
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("JobType", SqlDbType.TinyInt, createJob.BasicDetails.JobType));
                sqlParameters.Add(DBClient.AddParameters("InstallerID", SqlDbType.Int, createJob.BasicDetails.InstallerID));
                sqlParameters.Add(DBClient.AddParameters("DesignerID", SqlDbType.Int, createJob.BasicDetails.DesignerID));
                sqlParameters.Add(DBClient.AddParameters("InstallationDate", SqlDbType.DateTime, createJob.BasicDetails.InstallationDate));
                sqlParameters.Add(DBClient.AddParameters("STCTypeOfConnection", SqlDbType.NVarChar, createJob.JobSTCDetails.TypeOfConnection));
                sqlParameters.Add(DBClient.AddParameters("SerialNumbers", SqlDbType.NVarChar, createJob.JobSystemDetails.SerialNumbers));
                //sqlParameters.Add(DBClient.AddParameters("CalculatedSTC", SqlDbType.NVarChar, createJob.JobSystemDetails.CalculatedSTC));
                sqlParameters.Add(DBClient.AddParameters("ModifiedCalculatedSTC", SqlDbType.NVarChar, createJob.JobSystemDetails.ModifiedCalculatedSTC));
                sqlParameters.Add(DBClient.AddParameters("xmlPanels", SqlDbType.Xml, xmlPanels));
                sqlParameters.Add(DBClient.AddParameters("xmlInverters", SqlDbType.Xml, xmlInverters));
                sqlParameters.Add(DBClient.AddParameters("NoOfPanel", SqlDbType.Int, createJob.JobSystemDetails.NoOfPanel));
                sqlParameters.Add(DBClient.AddParameters("InsUnitTypeID", SqlDbType.Int, createJob.JobInstallationDetails.UnitTypeID));
                sqlParameters.Add(DBClient.AddParameters("InsUnitNumber", SqlDbType.NVarChar, createJob.JobInstallationDetails.UnitNumber));
                sqlParameters.Add(DBClient.AddParameters("InsStreetNumber", SqlDbType.NVarChar, createJob.JobInstallationDetails.StreetNumber));
                sqlParameters.Add(DBClient.AddParameters("InsStreetName", SqlDbType.NVarChar, createJob.JobInstallationDetails.StreetName));
                sqlParameters.Add(DBClient.AddParameters("InsStreetTypeID", SqlDbType.Int, createJob.JobInstallationDetails.StreetTypeID));
                sqlParameters.Add(DBClient.AddParameters("InsTown", SqlDbType.NVarChar, createJob.JobInstallationDetails.Town));
                sqlParameters.Add(DBClient.AddParameters("InsState", SqlDbType.NVarChar, createJob.JobInstallationDetails.State));
                sqlParameters.Add(DBClient.AddParameters("InsPostCode", SqlDbType.NVarChar, createJob.JobInstallationDetails.PostCode));
                sqlParameters.Add(DBClient.AddParameters("InsPostalDeliveryNumber ", SqlDbType.NVarChar, createJob.JobInstallationDetails.PostalDeliveryNumber));
                sqlParameters.Add(DBClient.AddParameters("InsIsPostalAddress", SqlDbType.Bit, createJob.JobInstallationDetails.IsPostalAddress));
                sqlParameters.Add(DBClient.AddParameters("InsPostalAddressID", SqlDbType.NVarChar, createJob.JobInstallationDetails.PostalAddressID));
                sqlParameters.Add(DBClient.AddParameters("InsInstallingNewPanel", SqlDbType.NVarChar, createJob.JobInstallationDetails.InstallingNewPanel));
                sqlParameters.Add(DBClient.AddParameters("NMI", SqlDbType.NVarChar, createJob.JobInstallationDetails.NMI));
                sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.Int, ProjectSession.SolarCompanyId));
                sqlParameters.Add(DBClient.AddParameters("IsTabular", SqlDbType.Bit, createJob.isTabular));
                sqlParameters.Add(DBClient.AddParameters("SystemSize", SqlDbType.Decimal, createJob.JobSystemDetails.SystemSize));
                sqlParameters.Add(DBClient.AddParameters("Latitude", SqlDbType.NVarChar, createJob.JobSTCDetails.Latitude));
                sqlParameters.Add(DBClient.AddParameters("Longitude", SqlDbType.NVarChar, createJob.JobSTCDetails.Longitude));
                sqlParameters.Add(DBClient.AddParameters("AdditionalCapacityNotes", SqlDbType.NVarChar, createJob.JobSTCDetails.AdditionalCapacityNotes));
                sqlParameters.Add(DBClient.AddParameters("MultipleSGUAddress", SqlDbType.NVarChar, createJob.JobSTCDetails.MultipleSGUAddress));
                sqlParameters.Add(DBClient.AddParameters("AdditionalLocationInformation", SqlDbType.NVarChar, createJob.JobSTCDetails.AdditionalLocationInformation));
                sqlParameters.Add(DBClient.AddParameters("AdditionalSystemInformation", SqlDbType.NVarChar, createJob.JobSTCDetails.AdditionalSystemInformation));

                if (jobId != null && jobId > 0)
                {
                    sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
                }
                sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, ProjectSession.UserTypeId));
                sqlParameters.Add(DBClient.AddParameters("CurrentDateTime", SqlDbType.DateTime, DateTime.Now));
                sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, ProjectSession.LoggedInUserId));
                sqlParameters.Add(DBClient.AddParameters("NoOfInverters", SqlDbType.Int, createJob.JobSystemDetails.NoOfInverter));
                sqlParameters.Add(DBClient.AddParameters("InverterSerialNumber", SqlDbType.NVarChar, createJob.JobSystemDetails.InverterSerialNumbers));
                sqlParameters.Add(DBClient.AddParameters("IsPanelInvoice", SqlDbType.Int, Convert.ToInt32(createJob.IsPanelInvoice)));
                sqlParameters.Add(DBClient.AddParameters("IsElectricityBill", SqlDbType.Int, Convert.ToInt32(createJob.IsElectricityBill)));
                DataSet dataset = CommonDAL.ExecuteDataSet("Job_CheckBusinessRules", sqlParameters.ToArray()); // live or staging
                                                                                                               //DataSet dataset = CommonDAL.ExecuteDataSet("Job_CheckBusinessRules_Bk", sqlParameters.ToArray()); // local
                return dataset;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Inserts the STC job compliance.
        /// </summary>
        /// <param name="StcComplianceCheck">The STC compliance check.</param>
        /// <returns>job compliance</returns>
        public DataSet InsertSTCJobCompliance(StcComplianceCheck StcComplianceCheck)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCJobDetailsID", SqlDbType.Int, StcComplianceCheck.STCJobDetailsID));
            sqlParameters.Add(DBClient.AddParameters("IsNameCorrect", SqlDbType.NVarChar, StcComplianceCheck.IsNameCorrect));
            sqlParameters.Add(DBClient.AddParameters("IsAddressCorrect", SqlDbType.NVarChar, StcComplianceCheck.IsAddressCorrect));
            sqlParameters.Add(DBClient.AddParameters("IsInstallerSignatureVisible", SqlDbType.NVarChar, StcComplianceCheck.IsInstallerSignatureVisible));
            sqlParameters.Add(DBClient.AddParameters("IsDesignerSignatureVisible", SqlDbType.NVarChar, StcComplianceCheck.IsDesignerSignatureVisible));
            sqlParameters.Add(DBClient.AddParameters("IsElectriciandetailsvisible", SqlDbType.NVarChar, StcComplianceCheck.IsElectriciandetailsvisible));
            sqlParameters.Add(DBClient.AddParameters("IsSerialNumbersMatch", SqlDbType.NVarChar, StcComplianceCheck.IsSerialNumbersMatch));
            sqlParameters.Add(DBClient.AddParameters("IsSTCAmountMatch", SqlDbType.NVarChar, StcComplianceCheck.IsSTCAmountMatch));
            sqlParameters.Add(DBClient.AddParameters("IsOwnerDetailsMatch", SqlDbType.NVarChar, StcComplianceCheck.IsOwnerDetailsMatch));
            sqlParameters.Add(DBClient.AddParameters("IsDescriptionCES", SqlDbType.NVarChar, StcComplianceCheck.IsDescriptionCES));
            sqlParameters.Add(DBClient.AddParameters("FirstName", SqlDbType.NVarChar, StcComplianceCheck.FirstName));
            sqlParameters.Add(DBClient.AddParameters("LastName", SqlDbType.NVarChar, StcComplianceCheck.LastName));
            sqlParameters.Add(DBClient.AddParameters("Address", SqlDbType.NVarChar, StcComplianceCheck.Address));
            sqlParameters.Add(DBClient.AddParameters("NoOfPanel", SqlDbType.Int, StcComplianceCheck.NoOfPanel));
            sqlParameters.Add(DBClient.AddParameters("Notes", SqlDbType.NVarChar, StcComplianceCheck.Notes));
            sqlParameters.Add(DBClient.AddParameters("CallDateTime", SqlDbType.DateTime, StcComplianceCheck.CallDateTime));
            sqlParameters.Add(DBClient.AddParameters("Status", SqlDbType.TinyInt, StcComplianceCheck.Status));
            sqlParameters.Add(DBClient.AddParameters("IsRequestedAuthorize", SqlDbType.Bit, StcComplianceCheck.IsRequestedAuthorize));
            sqlParameters.Add(DBClient.AddParameters("CallMadeBy", SqlDbType.NVarChar, StcComplianceCheck.CallMadeBy));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("FileUploadJson", SqlDbType.NVarChar, StcComplianceCheck.FileName));
            sqlParameters.Add(DBClient.AddParameters("Description", SqlDbType.NVarChar, StcComplianceCheck.Description));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, StcComplianceCheck.JobId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, ProjectSession.UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("STCLastUpdatedDate", SqlDbType.DateTime, StcComplianceCheck.STCLastUpdatedDate));
            sqlParameters.Add(DBClient.AddParameters("StcComplianceJobId", SqlDbType.Int, StcComplianceCheck.STCJobComplianceID));

            sqlParameters.Add(DBClient.AddParameters("InstallationType", SqlDbType.Int, StcComplianceCheck.InstallationType));
            sqlParameters.Add(DBClient.AddParameters("ConnectionType", SqlDbType.Int, StcComplianceCheck.ConnectionType));
            sqlParameters.Add(DBClient.AddParameters("MountingType", SqlDbType.Int, StcComplianceCheck.MountingType));
            sqlParameters.Add(DBClient.AddParameters("AdditionalInformation", SqlDbType.NVarChar, StcComplianceCheck.AdditionalInformation));
            sqlParameters.Add(DBClient.AddParameters("ExplanatoryNotes", SqlDbType.NVarChar, StcComplianceCheck.ExplanatoryNotes));

            sqlParameters.Add(DBClient.AddParameters("IsOrganisationNameCorrect", SqlDbType.NVarChar, StcComplianceCheck.IsOrganisationNameCorrect));
            sqlParameters.Add(DBClient.AddParameters("OrganisationName", SqlDbType.NVarChar, StcComplianceCheck.OrganisationName));
            DataSet ds = CommonDAL.ExecuteDataSet("STCJobCompliance_InsertSTCJobCompliance", sqlParameters.ToArray());
            //object complianceCheckId = CommonDAL.ExecuteScalar("STCJobCompliance_InsertSTCJobCompliance", sqlParameters.ToArray());
            //return Convert.ToInt32(complianceCheckId);
            return ds;
        }

        /// <summary>
        /// Gets the STC job compliance.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="STCJobDetailsID">The STC job details identifier.</param>
        /// <returns>Compliance object</returns>
        public StcComplianceCheck GetSTCJobCompliance(int jobId, int STCJobDetailsID)
        {
            DataTable dt = new DataTable();
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("STCJobDetailsID", SqlDbType.Int, STCJobDetailsID));
            DataSet dsComplianceCheck = CommonDAL.ExecuteDataSet("STCJobCompliance_GetSTCJobCompliance", sqlParameters.ToArray());
            StcComplianceCheck StcComplianceCheck = new StcComplianceCheck();
            if (dsComplianceCheck != null && dsComplianceCheck.Tables.Count > 0)
            {
                if (dsComplianceCheck.Tables[0] != null && dsComplianceCheck.Tables[0].Rows.Count > 0)
                {
                    StcComplianceCheck = dsComplianceCheck.Tables[0].ToListof<StcComplianceCheck>().FirstOrDefault();
                }

                if (dsComplianceCheck.Tables[1] != null && dsComplianceCheck.Tables[1].Rows.Count > 0)
                {
                    StcComplianceCheck.CallMadeBy = dsComplianceCheck.Tables[1].Rows[0][0].ToString();
                    StcComplianceCheck.CallMadeUserId = dsComplianceCheck.Tables[1].Rows[0][1].ToString();
                }

            }
            return StcComplianceCheck;
        }

        /// <summary>
        /// Gets the STC document by STC job details identifier.
        /// </summary>
        /// <param name="STCJobDetailsID">The STC job details identifier.</param>
        /// <returns>doc list</returns>
        public List<UserDocument> GetSTCDocumentBySTCJobDetailsID(int STCJobDetailsID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCJobDetailsID", SqlDbType.Int, STCJobDetailsID));
            List<UserDocument> lstUserDocument = CommonDAL.ExecuteProcedure<UserDocument>("STC_GetStcDocument", sqlParameters.ToArray()).ToList();
            if (lstUserDocument != null && lstUserDocument.Count > 0)
            {
                for (int i = 0; i < lstUserDocument.Count; i++)
                {
                    if (System.Web.MimeMapping.GetMimeMapping(lstUserDocument[i].DocumentPath).ToLower().StartsWith("image"))
                    {
                        lstUserDocument[i].MimeType = "image";
                    }
                    lstUserDocument[i].index = i + 1;
                }

            }

            return lstUserDocument;
        }

        /// <summary>
        /// Gets the STC compliance details.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="JobDetailsId">The job details identifier.</param>
        /// <param name="changedBy">The changed by.</param>
        /// <returns>DataSet</returns>
        public DataSet GetStcComplianceDetails(int jobId, int JobDetailsId, int changedBy)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("JobDetailsId", SqlDbType.Int, JobDetailsId));
            sqlParameters.Add(DBClient.AddParameters("changedBy", SqlDbType.Int, changedBy));
            DataSet dsUsers = CommonDAL.ExecuteDataSet("GetStcComplianceDetails", sqlParameters.ToArray());
            return dsUsers;
        }

        /// <summary>
        /// Accepts the reject job to SSC.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="role">The role.</param>
        public void AcceptRejectJobToSSC(int jobId, string role)
        {
            string spName = "[JobDetails_SSCJobAccept]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("Role", SqlDbType.NVarChar, role));
            CommonDAL.Crud(spName, sqlParameters.ToArray());
        }

        /// <summary>
        /// Cancels the removal request.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        public void CancelRemovalRequest(int jobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            CommonDAL.Crud("JobDetails_CancelRemovalRequest", sqlParameters.ToArray());
        }

        /// <summary>
        /// Assigns the job to fco.
        /// </summary>
        /// <param name="ComplianceOfficerId">The compliance officer identifier.</param>
        /// <param name="jobs">The jobs.</param>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="createdDate">The created date.</param>
        public DataSet AssignJobToFCO(int ComplianceOfficerId, string jobs, int UserId, DateTime createdDate)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ComplianceOfficerId", SqlDbType.Int, ComplianceOfficerId));
            sqlParameters.Add(DBClient.AddParameters("STCJobIDs", SqlDbType.NVarChar, jobs));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("createdDate", SqlDbType.DateTime, createdDate));
            DataSet ds = CommonDAL.ExecuteDataSet("[STCJobDetails_AssignJobToFCO]", sqlParameters.ToArray());
            return ds;
            //CommonDAL.Crud("STCJobDetails_AssignJobToFCO", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the bult upload for job.
        /// </summary>
        /// <param name="JobID">The job identifier.</param>
        /// <returns>DataSet</returns>
        public DataSet GetBultUploadForJob(string JobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.NVarChar, JobID));
            return CommonDAL.ExecuteDataSet("[Job_BulkUploadCSV]", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the nmi by job identifier.
        /// </summary>
        /// <param name="JobID">The job identifier.</param>
        /// <returns>string nmi</returns>
        public string GetNMIByJobID(int JobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.NVarChar, JobID));
            DataSet dataset = CommonDAL.ExecuteDataSet("Job_GetNMIByJobID", sqlParameters.ToArray());
            string nmi = string.Empty;
            if (dataset != null && dataset.Tables.Count > 0)
            {
                if (dataset.Tables[0].Rows.Count > 0)
                {
                    if (dataset.Tables[0].Rows[0].ItemArray.Count() > 0)
                    {
                        nmi = dataset.Tables[0].Rows[0].ItemArray[0].ToString();
                    }
                }
            }

            return nmi;
        }
        /// <summary>
        /// Gets the jobs to assign message.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns>job list</returns>
        public List<JobList> GetJobsToAssignMessage(int UserId, int UserTypeId, string searchText)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("searchtext", SqlDbType.NVarChar, searchText));
            List<JobList> lstJobs = CommonDAL.ExecuteProcedure<JobList>("GetJobsToAssignMessage", sqlParameters.ToArray()).ToList();
            return lstJobs;
        }

        /// <summary>
        /// Gets the bulk upload for job.
        /// </summary>
        /// <param name="JobID">The job identifier.</param>
        /// <returns>data set</returns>
        public DataSet GetBulkUploadForJob(string JobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.NVarChar, JobID));
            return CommonDAL.ExecuteDataSet("[Job_BulkUploadCSV]", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the bulk upload for job.
        /// </summary>
        /// <param name="JobID">The job identifier.</param>
        /// <returns>data set</returns>
        public DataSet GetSWHBulkUploadForJob(string JobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.NVarChar, JobID));
            return CommonDAL.ExecuteDataSet("[Job_BulkUploadSWHCSV]", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the job notes list for API.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>
        /// data set
        /// </returns>
        public DataSet GetJobNotesListForAPI(int jobID, int jobSchedulingId)
        {
            string spName = "[JobNotes_GetJobList]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, Int16.MaxValue));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, 1));
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("JobSchedulingId", SqlDbType.Int, jobSchedulingId));
            DataSet dsUsers = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dsUsers;
        }

        /// <summary>
        /// Gets the electrician by solar company identifier.
        /// </summary>
        /// <param name="solarCompanyID">The solar company identifier.</param>
        /// <param name="isFullDetail">if set to <c>true</c> [is full detail].</param>
        /// <param name="electricianID">The electrician identifier.</param>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet GetElectricianBySolarCompanyID(int solarCompanyID, bool isFullDetail, int electricianID, int jobID, int jobType)
        {
            string spName = "[GetElectrician]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.Int, solarCompanyID));
            sqlParameters.Add(DBClient.AddParameters("IsFullDetail", SqlDbType.Bit, isFullDetail));
            sqlParameters.Add(DBClient.AddParameters("ElectricianID", SqlDbType.Int, electricianID));
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("JobType", SqlDbType.Int, jobType));
            DataSet dsElectrician = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dsElectrician;
        }
        /// <summary>
        /// Gets the Sold by solar company identifier.
        /// </summary>
        /// <param name="solarCompanyID">The solar company identifier.</param>
        /// <returns>DataSet</returns>
        public DataSet GetSoldBySolarCompanyID(int solarCompanyID)
        {
            string spName = "[GetSoldBy]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.Int, solarCompanyID));
            DataSet dsElectrician = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dsElectrician;
        }

        /// <summary>
        /// Gets the distributor.
        /// </summary>
        /// <returns>DataSet</returns>
        public DataSet GetDistributor()
        {
            string spName = "[GetDistributor]";
            DataSet dsDistributor = CommonDAL.ExecuteDataSet(spName);
            return dsDistributor;
        }

        /// <summary>
        /// Updates the serial number.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="SerialNumbers">The serial numbers.</param>
        public void UpdateSerialNumber(int jobID, string SerialNumbers)
        {
            string spName = "[UpdateSerialNumberForAPI]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("SerialNumbers", SqlDbType.NVarChar, SerialNumbers));
            CommonDAL.Crud(spName, sqlParameters.ToArray());
        }

        public string UpdateCheckListSerialNumber(int VisitCheckListItemId, string CheckListSerialNumbers, int UserId, DataTable dtSPVSerialNumber)
        {
            string spName = "[UpdateCheckListSerialNumberApi]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VisitCheckListItemId", SqlDbType.Int, VisitCheckListItemId));
            sqlParameters.Add(DBClient.AddParameters("CheckListSerialNumbers", SqlDbType.NVarChar, CheckListSerialNumbers));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("Date", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("dtSPVSerialNumber", SqlDbType.Structured, dtSPVSerialNumber));
            object visitUniqueId = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            return Convert.ToString(visitUniqueId);
        }


        /// <summary>
        /// Updates the signature.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="signatureName">Name of the signature.</param>
        public void UpdateSignature(int jobID, string signatureName)
        {
            string spName = "[UpdateSignatureForAPI]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("SignatureName", SqlDbType.NVarChar, signatureName));
            CommonDAL.Crud(spName, sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the emailfor PVS sign.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <returns>email string</returns>
        public DataSet GetEmailforPvsSign(int UserId)
        {
            string spName = "[GetEmailforPvsSign]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            DataSet ds = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Gets the job owner signature for API.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="ownerSignature">The owner signature.</param>
        /// <param name="Latitude">The latitude.</param>
        /// <param name="Longitude">The longitude.</param>
        /// <param name="IpAddress">The ip address.</param>
        /// <param name="Location">The location.</param>
        /// <param name="SignatureDate">The signature date.</param>
        /// <returns>signature string</returns>
        public string GetJobOwnerSignatureForAPI(int jobId, string ownerSignature, string Latitude, string Longitude, string IpAddress, string Location, DateTime SignatureDate)
        {
            string spName = "[Job_GetOwnerSignatureForAPI]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("OwnerSignature", SqlDbType.NVarChar, ownerSignature));
            sqlParameters.Add(DBClient.AddParameters("Latitude", SqlDbType.NVarChar, Latitude));
            sqlParameters.Add(DBClient.AddParameters("Longitude", SqlDbType.NVarChar, Longitude));
            sqlParameters.Add(DBClient.AddParameters("IpAddress", SqlDbType.NVarChar, IpAddress));
            sqlParameters.Add(DBClient.AddParameters("Location", SqlDbType.NVarChar, Location));
            sqlParameters.Add(DBClient.AddParameters("SignatureDate", SqlDbType.DateTime, SignatureDate));
            object signature = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            return Convert.ToString(signature);

        }

        /// <summary>
        /// Checks the serial nnumbers.
        /// </summary>
        /// <param name="serialNumbers">The serial numbers.</param>
        /// <param name="solarCompanyID">The solar company identifier.</param>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>DataSet</returns>
        public DataSet CheckSerialNnumbers(string serialNumbers, int solarCompanyID, int jobID)
        {
            string spName = "[Job_CheckSerialNumbers]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.Int, solarCompanyID));
            sqlParameters.Add(DBClient.AddParameters("SerialNumbers", SqlDbType.NVarChar, serialNumbers));
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            DataSet dataset = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dataset;
        }

        /// <summary>
        /// Deletes the check list photos.
        /// </summary>
        /// <param name="chkIds">The CHK ids.</param>
        /// <param name="sigIds">The sig ids.</param>
        /// <param name="JobSchedulingIds">The job scheduling ids.</param>
        /// <returns></returns>
        public DataSet DeleteCheckListPhotos(string chkIds, string sigIds, string JobSchedulingIds, int jobId = 0)
        {
            string spName = "[DeleteCheckListPhotos]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CheckListIds", SqlDbType.VarChar, chkIds));
            sqlParameters.Add(DBClient.AddParameters("SignatureIds", SqlDbType.VarChar, sigIds));
            sqlParameters.Add(DBClient.AddParameters("JobSchedulingIds", SqlDbType.VarChar, JobSchedulingIds));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            return CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the job list for trade STC pop up.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="SortCol">The sort col.</param>
        /// <param name="SortDir">The sort dir.</param>
        /// <param name="jobIDS">The job ids.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns>
        /// price list
        /// </returns>
        public List<FormBot.Entity.PricingManager> GetJobListForTradeSTCPopUp(int UserId, int UserTypeId, string SortCol, string SortDir, string jobIDS, int solarCompanyId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeID", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("jobIDS", SqlDbType.NVarChar, jobIDS));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, SortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, SortDir));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.Int, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("CurrentDateTime", SqlDbType.DateTime, DateTime.Now));
            List<FormBot.Entity.PricingManager> lstJobs = CommonDAL.ExecuteProcedure<FormBot.Entity.PricingManager>("Job_GetJobListForTradeSTCPopUp", sqlParameters.ToArray()).ToList();
            // AddDays(325) 325 = 10 months + 3 weeks  (A per disucssion with Hus) when we change its value its also need to change in js file ViewAndEditNewJob.js
            lstJobs.Select(c => { c.IsApproachingExpiryDate = (Convert.ToDateTime(c.InstallationDate).AddDays(325) <= DateTime.Now.Date ? true : false); return c; }).ToList();
            return lstJobs;
        }

        /// <summary>
        /// Gets the pre approval status.
        /// </summary>
        /// <returns>DataSet</returns>
        public DataSet GetPreApprovalStatus()
        {
            string spName = "[JobStatus_GetPreApprovalStatus]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            DataSet dsPreApprovalStatus = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dsPreApprovalStatus;
        }

        /// <summary>
        /// Gets the connection status.
        /// </summary>
        /// <returns>DataSet</returns>
        public DataSet GetConnectionStatus()
        {
            string spName = "[JobStatus_GetConnectionStatus]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            DataSet dsConnectionStatus = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dsConnectionStatus;
        }

        /// <summary>
        /// Gets Job status common details.
        /// </summary>
        /// <returns>DataSet</returns>
        public DataSet GetJobStatusCommonDetails(int userID, bool defaultGrid = false)
        {
            string spName = "[JobStatus_CommonDetails]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("userId", SqlDbType.Int, userID));
            sqlParameters.Add(DBClient.AddParameters("defaultGrid", SqlDbType.Bit, defaultGrid));
            DataSet dsPreApprovalStatus = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dsPreApprovalStatus;
        }

        /// <summary>
        /// Gets the failure reason by job identifier.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet GetFailureReasonByJobId(int JobId)
        {
            string spName = "[GetFailureReasonByJobId]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            DataSet dsFailureReason = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dsFailureReason;
        }

        /// <summary>
        /// Gets the job stages.
        /// </summary>
        /// <returns>Job List</returns>
        public List<JobList> GetJobStages()
        {
            string spName = "[JobStages_BindDropDown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<JobList> lstJobStages = CommonDAL.ExecuteProcedure<JobList>(spName, sqlParameters.ToArray());
            return lstJobStages.ToList();
        }

        /// <summary>
        /// Bulks the change job stage.
        /// </summary>
        /// <param name="JobStageID">The job stage identifier.</param>
        /// <param name="JobIDs">The job i ds.</param>
        public void BulkChangeJobStage(int JobStageID, string JobIDs)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobStageID", SqlDbType.Int, JobStageID));
            sqlParameters.Add(DBClient.AddParameters("JobIDs", SqlDbType.NVarChar, JobIDs));
            CommonDAL.Crud("Job_BulkChangeJobStage", sqlParameters.ToArray());
        }

        /// <summary>
        /// Getjobs the identifier bystc invoice number.
        /// </summary>
        /// <param name="stcInvoiceNumber">The STC invoice number.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="resellerId">The reseller identifier.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet GetjobIdBystcInvoiceNumber(string stcInvoiceNumber, int UserTypeId, int resellerId)
        {
            string spName = "[GetjobIdBystcInvoiceNumber]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("stcInvoiceNumber", SqlDbType.NVarChar, stcInvoiceNumber));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("resellerId", SqlDbType.Int, resellerId));
            DataSet dsInvoiceDetail = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dsInvoiceDetail;
        }

        /// <summary>
        /// Gets the delete email details by job identifier.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>data set</returns>
        public DataSet GetDeleteEmailDetailsByJobId(int jobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            DataSet dsUsers = CommonDAL.ExecuteDataSet("Users_GetDeleteEmailDetailsByJobId", sqlParameters.ToArray());
            return dsUsers;
        }

        /// <summary>
        /// Gets the email details by job identifier.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>data set</returns>
        public DataSet GetEmailDetailsByJobID(int jobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            DataSet dsUsers = CommonDAL.ExecuteDataSet("Users_GetDeleteEmailDetailsByJobId", sqlParameters.ToArray());
            return dsUsers;
        }

        /// <summary>
        /// Bulks the change STC job stage.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="STCJobStageID">The STC job stage identifier.</param>
        /// <param name="STCJobIDs">The STC job i ds.</param>
        /// <param name="CreatedDate">The created date.</param>
        /// <returns>DataSet</returns>
        public DataSet BulkChangeSTCJobStage(int UserId, int UserTypeId, int STCJobStageID, string STCJobIDs, DateTime CreatedDate)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("STCJobStageID", SqlDbType.Int, STCJobStageID));
            sqlParameters.Add(DBClient.AddParameters("STCJobIDs", SqlDbType.NVarChar, STCJobIDs));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, CreatedDate));
            DataSet dsGeneratedSTCInvoices = CommonDAL.ExecuteDataSet("Job_BulkChangeSTCJobStage", sqlParameters.ToArray());
            return dsGeneratedSTCInvoices;
        }

        /// <summary>
        /// Bulks the change STC job stage.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="STCJobStageID">The STC job stage identifier.</param>
        /// <param name="STCJobIDs">The STC job i ds.</param>
        /// <param name="CreatedDate">The created date.</param>
        /// <returns>DataSet</returns>
        public DataSet AutoChangeSTCJobStageRTC(int UserId, int UserTypeId, string STCJobIDs, DateTime CreatedDate)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("STCJobIDs", SqlDbType.NVarChar, STCJobIDs));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, CreatedDate));
            DataSet dsGeneratedSTCInvoices = CommonDAL.ExecuteDataSet("Job_AutoChangeSTCJobStageRTC", sqlParameters.ToArray());
            return dsGeneratedSTCInvoices;
        }

        /// <summary>
        /// Gets the details of compliance issue job for mail.
        /// </summary>
        /// <param name="STCJobDetailsId">The STC job details identifier.</param>
        /// <returns></returns>
        public DataSet GetDetailsOfComplianceIssueJobForMail(int STCJobDetailsId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCJobDetailsId", SqlDbType.Int, STCJobDetailsId));
            DataSet complianceDetail = CommonDAL.ExecuteDataSet("Job_GetDetailsOfComplianceIssueJobForMail", sqlParameters.ToArray());
            return complianceDetail;
        }

        /// <summary>
        /// Gets the details of cer failed job for mail.
        /// </summary>
        /// <param name="STCJobDetailsId">The STC job details identifier.</param>
        /// <returns></returns>
        public DataSet GetDetailsOfCERFailedJobForMail(int STCJobDetailsId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCJobDetailsId", SqlDbType.Int, STCJobDetailsId));
            DataSet cerFailedDetail = CommonDAL.ExecuteDataSet("Job_GetDetailsOfCERFailedJobForMail", sqlParameters.ToArray());
            return cerFailedDetail;
        }

        /// <summary>
        /// job Print
        /// </summary>
        /// <param name="jobId">jobId</param>
        /// <returns>DataSet</returns>
        public DataSet JobPrint(int jobId, int userTypeId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userTypeId));
            DataSet jobPrintDetail = CommonDAL.ExecuteDataSet("Job_Print", sqlParameters.ToArray());
            return jobPrintDetail;
        }

        /// <summary>
        /// Get AllCompany To Make Registered With GST
        /// </summary>
        /// <returns>list of string</returns>
        public List<SolarCompany> GetAllCompanyToMakeRegisteredWithGST()
        {
            string spName = "[GetAllCompanyToMakeRegisteredWithGST]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<SolarCompany> solarCompanyList = CommonDAL.ExecuteProcedure<SolarCompany>(spName);
            return solarCompanyList.ToList();
        }

        /// <summary>
        /// Get AllCompany To Make Registered With GST
        /// </summary>
        /// <returns>list of string</returns>
        public List<JobOwnerDetails> GetAllOwnerWithGST()
        {
            string spName = "[GetAllJobOwnerRegisterWithGST]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<JobOwnerDetails> JobOwnerList = CommonDAL.ExecuteProcedure<JobOwnerDetails>(spName);
            return JobOwnerList.ToList();
        }

        /// <summary>
        /// Make SCA Registered With GST
        /// </summary>
        /// <param name="dtSCAWithGST"></param>
        public void MakeSCARegisteredWithGST(DataTable dtSCAWithGST)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("dtSCAWithGST", SqlDbType.Structured, dtSCAWithGST));
            CommonDAL.Crud("SolarCompany_MakeSCARegisteredWithGST", sqlParameters.ToArray());
        }

        /// <summary>
        /// Make SCA Registered With GST
        /// </summary>
        /// <param name="dtOwnerWithGST"></param>
        public void MakeOwnerRegisteredWithGST(DataTable dtOwnerWithGST)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("dtSCAWithGST", SqlDbType.Structured, dtOwnerWithGST));
            CommonDAL.Crud("JobOwner_MakeSCARegisteredWithGST", sqlParameters.ToArray());
        }

        /// <summary>
        /// Get Job  Field Data
        /// </summary>
        /// <returns>JobField</returns>
        public List<JobField> GetJobFieldData()
        {
            string spName = "[JobField_BindDropdown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<JobField> jobField = CommonDAL.ExecuteProcedure<JobField>(spName, sqlParameters.ToArray());
            return jobField.ToList();
        }

        /// <summary>
        /// Gets the job notes list.
        /// </summary>        
        /// <param name="jobID">job ID</param>
        /// <returns>list</returns>
        public List<JobNotes> GetJobNotesListOnVisit(int jobID)
        {
            List<JobNotes> lstJobNotes = new List<JobNotes>();
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
                lstJobNotes = CommonDAL.ExecuteProcedure<JobNotes>("JobNotes_GetJobNoteList", sqlParameters.ToArray()).ToList();
                if (lstJobNotes != null && lstJobNotes.Count > 0)
                {
                    for (int i = 0; i < lstJobNotes.Count; i++)
                    {
                        lstJobNotes[i].strCreatedDate = lstJobNotes[i].CreatedDate.ToString("dd/MM/yyyy");
                    }
                }
                return lstJobNotes;
            }
            catch (Exception ex)
            {
                return lstJobNotes;
            }

        }

        public void JobNotesMarkAsSeen(string jobNoteIds)
        {
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("JobNotesId", SqlDbType.NVarChar, jobNoteIds));
                CommonDAL.Crud("JobNotes_MarkAsSeen", sqlParameters.ToArray());
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Inserts the photo for API.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="status">The status.</param>
        /// <param name="UserID">The user identifier.</param>
        public void InsertVisitSignatureAPI(int VisitSignatureId, int VisitCheckListItemId, int JobSchedulingId, int JobId, string Path, int SignatureTypeId, int UserID, string Latitude, string Longitude, string IpAddress, string Location, string Image)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VisitSignatureId", SqlDbType.Int, VisitSignatureId));
            if (VisitCheckListItemId != 0)
                sqlParameters.Add(DBClient.AddParameters("VisitCheckListItemId", SqlDbType.Int, VisitCheckListItemId));
            sqlParameters.Add(DBClient.AddParameters("JobSchedulingId", SqlDbType.Int, JobSchedulingId));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("Path", SqlDbType.NVarChar, Path));
            sqlParameters.Add(DBClient.AddParameters("SignatureTypeId", SqlDbType.Int, SignatureTypeId));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, UserID));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("Latitude", SqlDbType.NVarChar, Latitude));
            sqlParameters.Add(DBClient.AddParameters("Longitude", SqlDbType.NVarChar, Longitude));
            sqlParameters.Add(DBClient.AddParameters("IpAddress", SqlDbType.NVarChar, IpAddress));
            sqlParameters.Add(DBClient.AddParameters("Location", SqlDbType.NVarChar, Location));
            sqlParameters.Add(DBClient.AddParameters("Image", SqlDbType.NVarChar, Image));
            //sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.Crud("VisitSignature_Insert", sqlParameters.ToArray());
        }

        /// <summary>
        /// Get FolderName
        /// </summary>
        /// <param name="CheckListItemId"></param>
        /// <returns></returns>
        public string GetFolderName(int CheckListItemId)
        {
            DataSet ds = new DataSet();
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CheckListItemId", SqlDbType.Int, CheckListItemId));
            ds = CommonDAL.ExecuteDataSet("GetFolderNameBy_CheckListItemId", sqlParameters.ToArray());
            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
            return "";
        }

        /// <summary>
        /// Inserts the photo for API.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="status">The status.</param>
        /// <param name="UserID">The user identifier.</param>
        public void InsertErrorLogAPI(string Error, int UserID, string DeviceToken, DateTime DateTime)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Error", SqlDbType.NVarChar, Error));
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserID));
            sqlParameters.Add(DBClient.AddParameters("DeviceToken", SqlDbType.NVarChar, DeviceToken));
            sqlParameters.Add(DBClient.AddParameters("DateTime", SqlDbType.DateTime, DateTime));
            sqlParameters.Add(DBClient.AddParameters("InsertedDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.Crud("ErrorLog_Insert", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the job list for cer approved jobs.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="SortCol">The sort col.</param>
        /// <param name="SortDir">The sort dir.</param>
        /// <param name="jobIDS">The job ids.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <param name="isApproved">The is approved.</param>
        /// <returns></returns>
        public List<FormBot.Entity.PricingManager> GetJobListForCERApprovedJobs(int UserId, int UserTypeId, string SortCol, string SortDir, string jobIDS, int solarCompanyId, int isApproved)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeID", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("jobIDS", SqlDbType.NVarChar, jobIDS));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, SortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, SortDir));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.Int, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("IsApproved", SqlDbType.Int, isApproved));
            sqlParameters.Add(DBClient.AddParameters("CurrentDate", SqlDbType.DateTime, DateTime.Now));

            List<FormBot.Entity.PricingManager> lstJobs = CommonDAL.ExecuteProcedure<FormBot.Entity.PricingManager>("Job_GetJobListForCERApprovedJobs", sqlParameters.ToArray()).ToList();
            return lstJobs;
        }

        /// <summary>
        /// Updates the job signature.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="signPath">The sign path.</param>
        /// <param name="typeOfSignature">The type of signature.</param>
        public void UpdateJobSignature(int jobId, string signPath, int typeOfSignature)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("SignPath", SqlDbType.NVarChar, signPath));
            sqlParameters.Add(DBClient.AddParameters("TypeOfSignature", SqlDbType.Int, typeOfSignature));
            sqlParameters.Add(DBClient.AddParameters("SignatureDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.Crud("UpdateJobSignature", sqlParameters.ToArray());
        }

        /// <summary>
        /// Get FolderName
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public DataSet GetVisitSignature(int Id)
        {
            DataSet ds = new DataSet();
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobSchedulingId", SqlDbType.Int, Id));

            return CommonDAL.ExecuteDataSet("GetVisitSignatures", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the visit signature API.
        /// </summary>
        /// <param name="JobSchedulingId">The job scheduling identifier.</param>
        /// <param name="VisitCheckListItemId">The visit check list item identifier.</param>
        /// <returns></returns>
        public DataSet GetVisitSignatureApi(int JobSchedulingId, int VisitCheckListItemId)
        {
            DataSet ds = new DataSet();
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobSchedulingId", SqlDbType.Int, JobSchedulingId));

            if (VisitCheckListItemId != 0)
                sqlParameters.Add(DBClient.AddParameters("VisitCheckListItemId", SqlDbType.Int, VisitCheckListItemId));

            return CommonDAL.ExecuteDataSet("GetVisitSignatures_Api", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the visit signature path.
        /// </summary>
        /// <param name="JobSchedulingId">The job scheduling identifier.</param>
        /// <param name="VisitCheckListItemId">The visit check list item identifier.</param>
        /// <param name="SignatureTypeId">The signature type identifier.</param>
        /// <returns></returns>
        public string GetVisitSignaturePath(int JobSchedulingId, int VisitCheckListItemId, int SignatureTypeId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobSchedulingId", SqlDbType.Int, JobSchedulingId));
            if (VisitCheckListItemId != 0)
                sqlParameters.Add(DBClient.AddParameters("VisitCheckListItemId", SqlDbType.Int, VisitCheckListItemId));
            sqlParameters.Add(DBClient.AddParameters("SignatureTypeId", SqlDbType.Int, SignatureTypeId));
            DataSet ds = CommonDAL.ExecuteDataSet("GetVisitSignaturePath", sqlParameters.ToArray());
            return ds.Tables[0].Rows.Count > 0 ? ds.Tables[0].Rows[0][0].ToString() : "";
        }

        /// <summary>
        /// Jobs the installer designer electricians insert update.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="profileType">Type of the profile.</param>
        /// <param name="signPath">The sign path.</param>
        /// <param name="jobElectricians">The job electricians.</param>
        /// <param name="jobInstallerDetails">The job installer details.</param>
        /// <param name="LoggedInUserId">The logged in user identifier.</param>
        /// <returns></returns>
        public int JobInstallerDesignerElectricians_InsertUpdate(int jobId, int profileType, string signPath, JobElectricians jobElectricians, JobInstallerDetails jobInstallerDetails, int LoggedInUserId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            if (jobElectricians != null)
            {
                sqlParameters.Add(DBClient.AddParameters("IsElectrician", SqlDbType.Bit, true));
            }
            else
            {
                sqlParameters.Add(DBClient.AddParameters("IsElectrician", SqlDbType.Bit, false));
            }


            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, LoggedInUserId));//ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, LoggedInUserId));//ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("BasicJobID", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("ProfileType", SqlDbType.Int, profileType));
            sqlParameters.Add(DBClient.AddParameters("JobSignPath", SqlDbType.NVarChar, signPath));

            //Electrician
            sqlParameters.Add(DBClient.AddParameters("EleCompanyName", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.CompanyName : null)));
            sqlParameters.Add(DBClient.AddParameters("EleFirstName", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.FirstName : null)));
            sqlParameters.Add(DBClient.AddParameters("EleLastName", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.LastName : null)));
            sqlParameters.Add(DBClient.AddParameters("EleUnitTypeID", SqlDbType.Int, (jobElectricians != null ? jobElectricians.UnitTypeID : null)));
            sqlParameters.Add(DBClient.AddParameters("EleUnitNumber", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.UnitNumber : null)));
            sqlParameters.Add(DBClient.AddParameters("EleStreetNumber", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.StreetNumber : null)));
            sqlParameters.Add(DBClient.AddParameters("EleStreetName ", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.StreetName : null)));
            sqlParameters.Add(DBClient.AddParameters("EleStreetTypeID", SqlDbType.Int, (jobElectricians != null ? jobElectricians.StreetTypeID : null)));
            sqlParameters.Add(DBClient.AddParameters("EleTown", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.Town : null)));
            sqlParameters.Add(DBClient.AddParameters("EleState", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.State : null)));
            sqlParameters.Add(DBClient.AddParameters("ElePostCode", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.PostCode : null)));
            sqlParameters.Add(DBClient.AddParameters("ElePhone", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.Phone : null)));
            sqlParameters.Add(DBClient.AddParameters("EleMobile", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.Mobile : null)));
            sqlParameters.Add(DBClient.AddParameters("EleEmail", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.Email : null)));
            sqlParameters.Add(DBClient.AddParameters("EleLicenseNumber", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.LicenseNumber : null)));
            sqlParameters.Add(DBClient.AddParameters("EleSignature", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.Signature : null)));
            sqlParameters.Add(DBClient.AddParameters("EleIsPostalAddress", SqlDbType.Bit, (jobElectricians != null ? jobElectricians.IsPostalAddress : false)));
            sqlParameters.Add(DBClient.AddParameters("ElePostalAddressID", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.PostalAddressID : 0)));
            sqlParameters.Add(DBClient.AddParameters("ElePostalDeliveryNumber", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.PostalDeliveryNumber : null)));
            sqlParameters.Add(DBClient.AddParameters("EleInstallerID", SqlDbType.Int, (jobElectricians != null ? jobElectricians.InstallerID : null)));
            sqlParameters.Add(DBClient.AddParameters("ElectricianID", SqlDbType.Int, (jobElectricians != null ? jobElectricians.ElectricianID : null)));

            //SWH Installer
            sqlParameters.Add(DBClient.AddParameters("InstallerFirstname", SqlDbType.NVarChar, (jobInstallerDetails != null ? jobInstallerDetails.FirstName : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerSurname", SqlDbType.NVarChar, (jobInstallerDetails != null ? jobInstallerDetails.Surname : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerPhone", SqlDbType.NVarChar, (jobInstallerDetails != null ? jobInstallerDetails.Phone : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerMobile", SqlDbType.NVarChar, (jobInstallerDetails != null ? jobInstallerDetails.Mobile : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerEmail", SqlDbType.NVarChar, (jobInstallerDetails != null ? jobInstallerDetails.Email : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerUnitTypeID", SqlDbType.Int, (jobInstallerDetails != null ? jobInstallerDetails.UnitTypeID : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerUnitNumber", SqlDbType.NVarChar, (jobInstallerDetails != null ? jobInstallerDetails.UnitNumber : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerStreetNumber", SqlDbType.NVarChar, (jobInstallerDetails != null ? jobInstallerDetails.StreetNumber : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerStreetName", SqlDbType.NVarChar, (jobInstallerDetails != null ? jobInstallerDetails.StreetName : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerStreetTypeID", SqlDbType.Int, (jobInstallerDetails != null ? jobInstallerDetails.StreetTypeID : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerTown", SqlDbType.NVarChar, (jobInstallerDetails != null ? jobInstallerDetails.Town : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerState", SqlDbType.NVarChar, (jobInstallerDetails != null ? jobInstallerDetails.State : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerPostCode", SqlDbType.NVarChar, (jobInstallerDetails != null ? jobInstallerDetails.PostCode : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerIsPostalAddress", SqlDbType.Bit, (jobInstallerDetails != null ? jobInstallerDetails.IsPostalAddress : false)));
            sqlParameters.Add(DBClient.AddParameters("InstallerPostalAddressID", SqlDbType.NVarChar, (jobInstallerDetails != null ? jobInstallerDetails.PostalAddressID : 0)));
            sqlParameters.Add(DBClient.AddParameters("InstallerPostalDeliveryNumber", SqlDbType.NVarChar, (jobInstallerDetails != null ? jobInstallerDetails.PostalDeliveryNumber : null)));
            sqlParameters.Add(DBClient.AddParameters("SWHInstallerDesignerId", SqlDbType.Int, (jobInstallerDetails != null ? jobInstallerDetails.SWHInstallerDesignerId : 0)));
            sqlParameters.Add(DBClient.AddParameters("InstallerLicenseNumber", SqlDbType.NVarChar, (jobInstallerDetails != null ? jobInstallerDetails.LicenseNumber : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerSESignature", SqlDbType.NVarChar, (jobInstallerDetails != null ? jobInstallerDetails.SESignature : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallerSolarCompanyId", SqlDbType.Int, (jobInstallerDetails != null ? jobInstallerDetails.SolarCompanyId : 0)));


            object savedProfileId = CommonDAL.ExecuteScalar("JobInstallerDesignerElectricians_InsertUpdate", sqlParameters.ToArray());
            return Convert.ToInt32(savedProfileId);
        }

        /// <summary>
        /// Gets the default submission signature by job identifier.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="typeOfSignature">The type of signature.</param>
        /// <returns></returns>
        public string GetDefaultSubmissionSignatureByJobId(int jobId, int typeOfSignature)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("TypeOfSignature", SqlDbType.Int, typeOfSignature));
            sqlParameters.Add(DBClient.AddParameters("SignatureDate", SqlDbType.DateTime, DateTime.Now));

            object signPath = CommonDAL.ExecuteScalar("GetDefaultSubmissionSignatureByJobId", sqlParameters.ToArray());
            return Convert.ToString(signPath);
        }

        /// <summary>
        /// Inserts the job notes.
        /// </summary>
        /// <param name="notes">The notes.</param>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="createdBy">The created by.</param>
        /// <param name="jobSchedulingId">The job scheduling identifier.</param>
        /// <returns></returns>
        public int InsertJobNotes(string notes, int jobID, int createdBy, int jobSchedulingId)
        {
            string spName = "[JobNotes_InsertJobNotes]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobID));
            sqlParameters.Add(DBClient.AddParameters("Notes", SqlDbType.NVarChar, notes));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, createdBy));
            sqlParameters.Add(DBClient.AddParameters("JobSchedulingId", SqlDbType.Int, jobSchedulingId));
            DataSet ds = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
        }

        /// <summary>
        /// Gets the serial number.
        /// </summary>
        /// <param name="VisitCheckListItemId">The visit check list item identifier.</param>
        /// <returns></returns>
        public string GetSerialNumber(int VisitCheckListItemId)
        {
            string spName = "[GetSerialNumber]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VisitCheckListItemId", SqlDbType.Int, VisitCheckListItemId));

            DataSet ds = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0][0] != null)
                return Convert.ToString(ds.Tables[0].Rows[0][0]);
            else
                return "";
        }

        /// <summary>
        /// Gets the user signature path.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="Signature">The signature.</param>
        /// <returns></returns>
        public string GetUserSignaturePath(int UserId, string Signature = "")
        {
            try
            {

                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
                if (!string.IsNullOrEmpty(Signature))
                    sqlParameters.Add(DBClient.AddParameters("Signature", SqlDbType.VarChar, Signature));
                DataSet ds = CommonDAL.ExecuteDataSet("GetUserSignature", sqlParameters.ToArray());
                if (ds.Tables[0].Rows.Count > 0)
                    return Convert.ToString(ds.Tables[0].Rows[0][0] == System.DBNull.Value ? "" : ds.Tables[0].Rows[0][0] == null ? "" : ds.Tables[0].Rows[0][0]);
                else
                    return string.Empty;

            }


            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// Gets the checklist photos.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <returns></returns>
        public DataSet GetChecklistPhotos(int JobId)
        {
            try
            {

                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
                DataSet ds = CommonDAL.ExecuteDataSet("GetVisitCheckListPhotos", sqlParameters.ToArray());
                return ds;
            }


            catch (Exception ex)
            {
                return new DataSet();
            }
        }

        public DataSet GetChecklistItems(int JobId, int jobSchedulingId)
        {
            try
            {

                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
                sqlParameters.Add(DBClient.AddParameters("JobSchedulingId", SqlDbType.Int, jobSchedulingId));
                DataSet ds = CommonDAL.ExecuteDataSet("GetVisitChecklistItems", sqlParameters.ToArray());
                return ds;
            }


            catch (Exception ex)
            {
                return new DataSet();
            }
        }
        public DataSet DeletedCheckListItem(int JobId)
        {
            try
            {

                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
                DataSet ds = CommonDAL.ExecuteDataSet("GetDeletedVisitCheckListPhotos", sqlParameters.ToArray());
                return ds;
            }


            catch (Exception ex)
            {
                return new DataSet();
            }
        }
        public DataSet GetVisitData(string jobscId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("jobScId", SqlDbType.Int, jobscId));
            DataSet ds = CommonDAL.ExecuteDataSet("GetVisitData", sqlParameters.ToArray());
            return ds;
        }
        public DataSet RestoreCheckListItem(int JobscId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("jobScId", SqlDbType.VarChar, JobscId));
            DataSet ds = CommonDAL.ExecuteDataSet("GetChecklistForRestoreData", sqlParameters.ToArray());
            return ds;
        }

        public void RestoreData(int vclId, string path, int jobscId, int vcphotoId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("jobScId", SqlDbType.Int, jobscId));
            sqlParameters.Add(DBClient.AddParameters("vclId", SqlDbType.Int, vclId));
            sqlParameters.Add(DBClient.AddParameters("path", SqlDbType.NVarChar, path));
            sqlParameters.Add(DBClient.AddParameters("visitchecklistphotoId", SqlDbType.Int, vcphotoId));
            CommonDAL.ExecuteScalar("RestoreData", sqlParameters.ToArray());
        }
        public DataSet GetphotosbyMultipleIds(int JobId, string vcphotoId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("vclphotoId", SqlDbType.VarChar, vcphotoId));
            DataSet ds = CommonDAL.ExecuteDataSet("GetphotosbyMultipleIds", sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Gets the serial number of job.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <returns></returns>
        public string GetSerialNumberOfJob(int jobId)
        {
            string spName = "[GetSerialNumberOfJob]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobId));
            object serialNumber = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            return Convert.ToString(serialNumber);
        }

        /// <summary>
        /// Makes the visit check list item completed.
        /// </summary>
        /// <param name="visitCheckListItemId">The visit check list item identifier.</param>
        /// <param name="isCompleted">if set to <c>true</c> [is completed].</param>
        public void MakeVisitCheckListItemCompleted(int visitCheckListItemId, bool isCompleted)
        {
            string spName = "[MakeVisitCheckListItemCompleted]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VisitCheckListItemId", SqlDbType.Int, visitCheckListItemId));
            sqlParameters.Add(DBClient.AddParameters("IsCompleted", SqlDbType.Bit, isCompleted));
            CommonDAL.Crud(spName, sqlParameters.ToArray());
        }

        /// <summary>
        /// Updates the custom job field.
        /// </summary>
        /// <param name="dtCustomField">The dt custom field.</param>
        public void UpdateCustomJobField(DataTable dtCustomField)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("dtCustomField", SqlDbType.Structured, dtCustomField));
            CommonDAL.Crud("UpdateCustomJobField", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the job custom fields.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <param name="isPopup">if set to <c>true</c> [is popup].</param>
        /// <returns></returns>
        public List<CustomField> GetJobCustomFields(int JobId, bool isPopup)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("IsPopup", SqlDbType.Bit, isPopup));
            List<CustomField> lstCustomField = CommonDAL.ExecuteProcedure<CustomField>("GetJobCustomFields", sqlParameters.ToArray()).ToList();
            return lstCustomField;
        }

        /// <summary>
        /// Updates the installer designer ele signature.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="signature">The signature.</param>
        /// <param name="Latitude">The latitude.</param>
        /// <param name="Longitude">The longitude.</param>
        /// <param name="IpAddress">The ip address.</param>
        /// <param name="Location">The location.</param>
        /// <param name="SignatureDate">The signature date.</param>
        /// <param name="path">The path.</param>
        /// <param name="typeOfSignature">The type of signature.</param>
        public void UpdateInstallerDesignerEleSignature(int jobId, string signature, string Latitude, string Longitude, string IpAddress, string Location, DateTime SignatureDate, string path, int typeOfSignature)
        {
            string spName = "[Job_UpdateInstallerDesignerEleSignature]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("Signature", SqlDbType.NVarChar, signature));
            sqlParameters.Add(DBClient.AddParameters("Latitude", SqlDbType.NVarChar, Latitude));
            sqlParameters.Add(DBClient.AddParameters("Longitude", SqlDbType.NVarChar, Longitude));
            sqlParameters.Add(DBClient.AddParameters("IpAddress", SqlDbType.NVarChar, IpAddress));
            sqlParameters.Add(DBClient.AddParameters("Location", SqlDbType.NVarChar, Location));
            sqlParameters.Add(DBClient.AddParameters("SignatureDate", SqlDbType.DateTime, SignatureDate));
            sqlParameters.Add(DBClient.AddParameters("Path", SqlDbType.NVarChar, path));
            sqlParameters.Add(DBClient.AddParameters("typeOfSignature", SqlDbType.Int, typeOfSignature));
            CommonDAL.Crud(spName, sqlParameters.ToArray());
        }

        /// <summary>
        /// Updates the basic detail.
        /// </summary>
        /// <param name="obj">The object.</param>
        public void UpdateBasicDetail(BasicDetails obj)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, Convert.ToInt32(obj.JobID)));
            sqlParameters.Add(DBClient.AddParameters("RefNumber", SqlDbType.NVarChar, obj.RefNumber));
            sqlParameters.Add(DBClient.AddParameters("Title", SqlDbType.NVarChar, obj.Title));
            sqlParameters.Add(DBClient.AddParameters("Description", SqlDbType.NVarChar, obj.Description));
            sqlParameters.Add(DBClient.AddParameters("JobStage", SqlDbType.Int, Convert.ToInt32(obj.JobStage)));
            sqlParameters.Add(DBClient.AddParameters("Priority", SqlDbType.Int, Convert.ToInt32(obj.Priority)));

            if (obj.strInstallationDate != null)
            {
                obj.InstallationDate = Convert.ToDateTime(obj.strInstallationDate);
                sqlParameters.Add(DBClient.AddParameters("InstallationDate", SqlDbType.DateTime, obj.InstallationDate));
            }
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            if (obj.strSoldByDate != null)
                obj.SoldByDate = Convert.ToDateTime(obj.strSoldByDate);
            else
                obj.SoldByDate = null;

            sqlParameters.Add(DBClient.AddParameters("SoldBy", SqlDbType.NVarChar, obj.SoldBy));
            sqlParameters.Add(DBClient.AddParameters("SoldByDate", SqlDbType.DateTime, obj.SoldByDate));
            sqlParameters.Add(DBClient.AddParameters("SSCID", SqlDbType.Int, obj.SSCID));
            if (ProjectSession.UserTypeId == 8)
            {
                obj.ScoID = ProjectSession.LoggedInUserId;
            }
            sqlParameters.Add(DBClient.AddParameters("ScoID", SqlDbType.Int, obj.ScoID));
            CommonDAL.Crud("UpdateJobDetail", sqlParameters.ToArray());
        }

        ///// <summary>
        ///// Updates the owner detail.
        ///// </summary>
        ///// <param name="obj">The object.</param>
        //public void UpdateOwnerDetail(JobOwnerDetails obj)
        //{
        //    if (obj.AddressID == 2)
        //        obj.IsPostalAddress = true;
        //    else
        //        obj.IsPostalAddress = false;

        //    List<SqlParameter> sqlParameters = new List<SqlParameter>();
        //    sqlParameters.Add(DBClient.AddParameters("OwnerType", SqlDbType.NVarChar, obj.OwnerType));
        //    sqlParameters.Add(DBClient.AddParameters("OwnerCompanyName", SqlDbType.NVarChar, obj.CompanyName));
        //    sqlParameters.Add(DBClient.AddParameters("OwnerFirstName", SqlDbType.NVarChar, obj.FirstName));
        //    sqlParameters.Add(DBClient.AddParameters("OwnerLastName", SqlDbType.NVarChar, obj.LastName));
        //    sqlParameters.Add(DBClient.AddParameters("OwnerUnitTypeID", SqlDbType.Int, Convert.ToInt32(obj.UnitTypeID)));
        //    sqlParameters.Add(DBClient.AddParameters("OwnerUnitNumber", SqlDbType.NVarChar, Convert.ToString(obj.UnitNumber)));
        //    sqlParameters.Add(DBClient.AddParameters("OwnerStreetNumber", SqlDbType.NVarChar, Convert.ToString(obj.StreetNumber)));
        //    sqlParameters.Add(DBClient.AddParameters("OwnerStreetName", SqlDbType.NVarChar, Convert.ToString(obj.StreetName)));
        //    sqlParameters.Add(DBClient.AddParameters("OwnerStreetTypeID", SqlDbType.Int, Convert.ToInt32(obj.StreetTypeID)));
        //    sqlParameters.Add(DBClient.AddParameters("OwnerTown", SqlDbType.NVarChar, Convert.ToString(obj.Town)));
        //    sqlParameters.Add(DBClient.AddParameters("OwnerState", SqlDbType.NVarChar, Convert.ToString(obj.State)));
        //    sqlParameters.Add(DBClient.AddParameters("OwnerPostCode", SqlDbType.NVarChar, Convert.ToString(obj.PostCode)));
        //    sqlParameters.Add(DBClient.AddParameters("OwnerPhone", SqlDbType.NVarChar, Convert.ToString(obj.Phone)));
        //    sqlParameters.Add(DBClient.AddParameters("OwnerMobile", SqlDbType.NVarChar, Convert.ToString(obj.Mobile)));
        //    sqlParameters.Add(DBClient.AddParameters("OwnerEmail", SqlDbType.NVarChar, Convert.ToString(obj.Email)));
        //    sqlParameters.Add(DBClient.AddParameters("OwnerIsPostalAddress", SqlDbType.Bit, Convert.ToBoolean(obj.IsPostalAddress)));
        //    sqlParameters.Add(DBClient.AddParameters("OwnerPostalAddressID", SqlDbType.NVarChar, Convert.ToString(obj.PostalAddressID)));
        //    sqlParameters.Add(DBClient.AddParameters("OwnerPostalDeliveryNumber", SqlDbType.NVarChar, Convert.ToString(obj.PostalDeliveryNumber)));
        //    sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, Convert.ToInt32(obj.JobID)));        

        //    CommonDAL.Crud("UpdateJobOwnerDetails", sqlParameters.ToArray());
        //}

        /// <summary>
        /// Updates the installation detail.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="dtCustomField">The dt custom field.</param>
        public void UpdateInstallationDetail(JobInstallationDetails obj, DataTable dtCustomField = null)
        {
            if (obj.AddressID == 2)
                obj.IsPostalAddress = true;
            else
                obj.IsPostalAddress = false;

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("InsUnitTypeID", SqlDbType.Int, obj.UnitTypeID));
            sqlParameters.Add(DBClient.AddParameters("InsUnitNumber", SqlDbType.NVarChar, obj.UnitNumber));
            sqlParameters.Add(DBClient.AddParameters("InsStreetNumber", SqlDbType.NVarChar, obj.StreetNumber));
            sqlParameters.Add(DBClient.AddParameters("InsStreetName", SqlDbType.NVarChar, obj.StreetName));
            sqlParameters.Add(DBClient.AddParameters("InsStreetTypeID", SqlDbType.Int, obj.StreetTypeID));
            sqlParameters.Add(DBClient.AddParameters("InsTown", SqlDbType.NVarChar, obj.Town));
            sqlParameters.Add(DBClient.AddParameters("InsState", SqlDbType.NVarChar, obj.State));
            sqlParameters.Add(DBClient.AddParameters("InsPostCode", SqlDbType.NVarChar, obj.PostCode));
            sqlParameters.Add(DBClient.AddParameters("InsNMI", SqlDbType.NVarChar, obj.NMI));
            sqlParameters.Add(DBClient.AddParameters("InsDistributorID", SqlDbType.NVarChar, obj.DistributorID));
            sqlParameters.Add(DBClient.AddParameters("InsPropertyType", SqlDbType.NVarChar, obj.PropertyType));
            sqlParameters.Add(DBClient.AddParameters("InsPropertyName", SqlDbType.NVarChar, obj.PropertyName));
            sqlParameters.Add(DBClient.AddParameters("InsSingleMultipleStory", SqlDbType.NVarChar, obj.SingleMultipleStory));
            sqlParameters.Add(DBClient.AddParameters("InsInstallingNewPanel", SqlDbType.NVarChar, obj.InstallingNewPanel));
            sqlParameters.Add(DBClient.AddParameters("InsMeterNumber", SqlDbType.NVarChar, obj.MeterNumber));
            sqlParameters.Add(DBClient.AddParameters("InsPhaseProperty", SqlDbType.NVarChar, obj.PhaseProperty));
            sqlParameters.Add(DBClient.AddParameters("InsElectricityProviderID ", SqlDbType.Int, obj.ElectricityProviderID));
            sqlParameters.Add(DBClient.AddParameters("InsExistingSystem ", SqlDbType.NVarChar, obj.ExistingSystem));
            sqlParameters.Add(DBClient.AddParameters("InsExistingSystemSize", SqlDbType.Decimal, obj.ExistingSystemSize));
            sqlParameters.Add(DBClient.AddParameters("InsNoOfPanels", SqlDbType.Int, obj.NoOfPanels));
            sqlParameters.Add(DBClient.AddParameters("InsSystemLocation", SqlDbType.NVarChar, obj.SystemLocation));
            sqlParameters.Add(DBClient.AddParameters("InsPostalDeliveryNumber ", SqlDbType.NVarChar, obj.PostalDeliveryNumber));
            sqlParameters.Add(DBClient.AddParameters("InsIsPostalAddress", SqlDbType.Bit, obj.IsPostalAddress));
            sqlParameters.Add(DBClient.AddParameters("InsPostalAddressID", SqlDbType.NVarChar, obj.PostalAddressID));
            sqlParameters.Add(DBClient.AddParameters("InsLocation", SqlDbType.NVarChar, obj.Location));
            sqlParameters.Add(DBClient.AddParameters("InsLatitude", SqlDbType.NVarChar, obj.Latitude));
            sqlParameters.Add(DBClient.AddParameters("InsLongitude", SqlDbType.NVarChar, obj.Longitude));
            sqlParameters.Add(DBClient.AddParameters("InsAdditionalInstallationInformation", SqlDbType.NVarChar, obj.AdditionalInstallationInformation));
            sqlParameters.Add(DBClient.AddParameters("InsIsSameAsOwnerAddress", SqlDbType.Bit, obj.IsSameAsOwnerAddress));
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, Convert.ToInt32(obj.JobID)));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));

            sqlParameters.Add(DBClient.AddParameters("dtCustomField", SqlDbType.Structured, dtCustomField));

            CommonDAL.Crud("UpdateJobInstallationDetails", sqlParameters.ToArray());
        }


        /// <summary>
        /// Updates the installation detail.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="dtCustomField">The dt custom field.</param>
        public void UpdateInstallationDetailTabular(JobInstallationDetails obj)
        {
            if (obj.AddressID == 2)
                obj.IsPostalAddress = true;
            else
                obj.IsPostalAddress = false;

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("InsUnitTypeID", SqlDbType.Int, obj.UnitTypeID));
            sqlParameters.Add(DBClient.AddParameters("InsUnitNumber", SqlDbType.NVarChar, obj.UnitNumber));
            sqlParameters.Add(DBClient.AddParameters("InsStreetNumber", SqlDbType.NVarChar, obj.StreetNumber));
            sqlParameters.Add(DBClient.AddParameters("InsStreetName", SqlDbType.NVarChar, obj.StreetName));
            sqlParameters.Add(DBClient.AddParameters("InsStreetTypeID", SqlDbType.Int, obj.StreetTypeID));
            sqlParameters.Add(DBClient.AddParameters("InsTown", SqlDbType.NVarChar, obj.Town));
            sqlParameters.Add(DBClient.AddParameters("InsState", SqlDbType.NVarChar, obj.State));
            sqlParameters.Add(DBClient.AddParameters("InsPostCode", SqlDbType.NVarChar, obj.PostCode));
            sqlParameters.Add(DBClient.AddParameters("InsNMI", SqlDbType.NVarChar, obj.NMI));
            sqlParameters.Add(DBClient.AddParameters("InsDistributorID", SqlDbType.NVarChar, obj.DistributorID));
            //sqlParameters.Add(DBClient.AddParameters("InsPropertyType", SqlDbType.NVarChar, obj.PropertyType));
            sqlParameters.Add(DBClient.AddParameters("InsPropertyName", SqlDbType.NVarChar, obj.PropertyName));
            //sqlParameters.Add(DBClient.AddParameters("InsSingleMultipleStory", SqlDbType.NVarChar, obj.SingleMultipleStory));
            //sqlParameters.Add(DBClient.AddParameters("InsInstallingNewPanel", SqlDbType.NVarChar, obj.InstallingNewPanel));
            sqlParameters.Add(DBClient.AddParameters("InsMeterNumber", SqlDbType.NVarChar, obj.MeterNumber));
            sqlParameters.Add(DBClient.AddParameters("InsPhaseProperty", SqlDbType.NVarChar, obj.PhaseProperty));
            sqlParameters.Add(DBClient.AddParameters("InsElectricityProviderID ", SqlDbType.Int, obj.ElectricityProviderID));
            sqlParameters.Add(DBClient.AddParameters("InsExistingSystem ", SqlDbType.NVarChar, obj.ExistingSystem));
            sqlParameters.Add(DBClient.AddParameters("InsExistingSystemSize", SqlDbType.Decimal, obj.ExistingSystemSize));
            sqlParameters.Add(DBClient.AddParameters("InsNoOfPanels", SqlDbType.Int, obj.NoOfPanels));
            sqlParameters.Add(DBClient.AddParameters("InsSystemLocation", SqlDbType.NVarChar, obj.SystemLocation));
            sqlParameters.Add(DBClient.AddParameters("InsPostalDeliveryNumber ", SqlDbType.NVarChar, obj.PostalDeliveryNumber));
            sqlParameters.Add(DBClient.AddParameters("InsIsPostalAddress", SqlDbType.Bit, obj.IsPostalAddress));
            sqlParameters.Add(DBClient.AddParameters("InsPostalAddressID", SqlDbType.NVarChar, obj.PostalAddressID));
            //sqlParameters.Add(DBClient.AddParameters("InsLocation", SqlDbType.NVarChar, obj.Location));
            sqlParameters.Add(DBClient.AddParameters("InsLatitude", SqlDbType.NVarChar, obj.Latitude));
            sqlParameters.Add(DBClient.AddParameters("InsLongitude", SqlDbType.NVarChar, obj.Longitude));
            sqlParameters.Add(DBClient.AddParameters("InsAdditionalInstallationInformation", SqlDbType.NVarChar, obj.AdditionalInstallationInformation));
            sqlParameters.Add(DBClient.AddParameters("InsIsSameAsOwnerAddress", SqlDbType.Bit, obj.IsSameAsOwnerAddress));
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, Convert.ToInt32(obj.JobID)));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));

            //sqlParameters.Add(DBClient.AddParameters("dtCustomField", SqlDbType.Structured, dtCustomField));

            CommonDAL.Crud("UpdateJobInstallationDetailsTabular", sqlParameters.ToArray());
        }

        public void UpdateCustomDetail(String JobId, DataTable dtCustomField = null)
        {


            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, Convert.ToInt32(JobId)));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));

            sqlParameters.Add(DBClient.AddParameters("dtCustomField", SqlDbType.Structured, dtCustomField));

            CommonDAL.Crud("UpdateJobCustomDetails", sqlParameters.ToArray());
        }
        /// <summary>
        /// Businesses the rulefor job STC detail.
        /// </summary>
        /// <param name="sqlParameters">The SQL parameters.</param>
        /// <param name="JobInstallationDetails">The job installation details.</param>
        /// <param name="JobSTCDetails">The job STC details.</param>
        /// <param name="LoggedInUserId">The logged in user identifier.</param>
        /// <returns></returns>
        public List<SqlParameter> BusinessRuleforJobSTCDetail(List<SqlParameter> sqlParameters, JobInstallationDetails JobInstallationDetails, JobSTCDetails JobSTCDetails, int LoggedInUserId, int JobType)
        {
            CreateJob createJob = new CreateJob();

            createJob.JobSTCDetails = JobSTCDetailsBusinessRules(JobInstallationDetails.InstallingNewPanel, JobSTCDetails);

            //if (!string.IsNullOrEmpty(JobInstallationDetails.InstallingNewPanel))
            //{
            //    if (JobSTCDetails != null)
            //    {
            //        if (JobInstallationDetails.InstallingNewPanel.ToLower() == "new" || JobInstallationDetails.InstallingNewPanel.ToLower() == "replacing")
            //        {
            //            JobSTCDetails.InstallingCompleteUnit = "Yes";
            //            if (JobInstallationDetails.InstallingNewPanel.ToLower() == "new")
            //            {
            //                JobSTCDetails.AdditionalCapacityNotes = null;
            //            }
            //        }
            //        else
            //        {
            //            JobSTCDetails.InstallingCompleteUnit = "No";
            //        }
            //    }
            //}
            //else if (JobSTCDetails != null)
            //{
            //    JobSTCDetails.AdditionalCapacityNotes = null;
            //}

            //if (JobSTCDetails != null)
            //{
            //    if (!string.IsNullOrEmpty(JobSTCDetails.MultipleSGUAddress))
            //    {
            //        if (JobSTCDetails.MultipleSGUAddress.ToLower() != "yes")
            //        {
            //            JobSTCDetails.Location = null;
            //        }
            //    }
            //    else
            //    {
            //        JobSTCDetails.Location = null;
            //    }

            //    if (JobSTCDetails.TypeOfConnection == "Stand-alone (not connected to an electricity grid)")
            //    {
            //        JobSTCDetails.StandAloneGridSelected = "Yes";
            //    }
            //    else
            //    {
            //        JobSTCDetails.StandAloneGridSelected = "No";
            //    }
            //}

            //createJob.JobSTCDetails = JobSTCDetails;
            createJob.JobInstallationDetails = JobInstallationDetails;

            sqlParameters.Add(DBClient.AddParameters("STCTypeOfConnection", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.TypeOfConnection : null)));
            sqlParameters.Add(DBClient.AddParameters("STCSystemMountingType", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.SystemMountingType : null)));
            sqlParameters.Add(DBClient.AddParameters("STCInstallingCompleteUnit", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.InstallingCompleteUnit : null)));
            sqlParameters.Add(DBClient.AddParameters("STCAdditionalCapacityNotes", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.AdditionalCapacityNotes : null)));
            sqlParameters.Add(DBClient.AddParameters("STCDeemingPeriod", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.DeemingPeriod : null)));
            sqlParameters.Add(DBClient.AddParameters("STCCertificateCreated", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.CertificateCreated : null)));
            sqlParameters.Add(DBClient.AddParameters("STCFailedAccreditationCode", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.FailedAccreditationCode : null)));
            sqlParameters.Add(DBClient.AddParameters("STCFailedReason", SqlDbType.NVarChar, (createJob.JobSTCDetails != null && !string.IsNullOrWhiteSpace(createJob.JobSTCDetails.FailedReason) ? Regex.Replace(HttpUtility.HtmlDecode(createJob.JobSTCDetails.FailedReason), @"\\r\\n", "<br>") : null)));
            sqlParameters.Add(DBClient.AddParameters("STCCECAccreditationStatement", SqlDbType.NVarChar, createJob.JobSTCDetails.CECAccreditationStatement));
            sqlParameters.Add(DBClient.AddParameters("STCGovernmentSitingApproval", SqlDbType.NVarChar, createJob.JobSTCDetails.GovernmentSitingApproval));
            sqlParameters.Add(DBClient.AddParameters("STCElectricalSafetyDocumentation", SqlDbType.NVarChar, createJob.JobSTCDetails.ElectricalSafetyDocumentation));
            sqlParameters.Add(DBClient.AddParameters("STCAustralianNewZealandStandardStatement", SqlDbType.NVarChar, createJob.JobSTCDetails.AustralianNewZealandStandardStatement));
            sqlParameters.Add(DBClient.AddParameters("STCVolumetricCapacity", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.VolumetricCapacity : null)));
            sqlParameters.Add(DBClient.AddParameters("STCStatutoryDeclarations", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.StatutoryDeclarations : null)));
            sqlParameters.Add(DBClient.AddParameters("STCSecondhandWaterHeater", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.SecondhandWaterHeater : null)));
            sqlParameters.Add(DBClient.AddParameters("STCStandAloneGridSelected", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.StandAloneGridSelected : null)));
            sqlParameters.Add(DBClient.AddParameters("STCMultipleSGUAddress", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.MultipleSGUAddress : null)));
            sqlParameters.Add(DBClient.AddParameters("STCSGUSystemLocated", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.SGUSystemLocated : null)));
            sqlParameters.Add(DBClient.AddParameters("STCCreatedBy", SqlDbType.NVarChar, LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("STCLocation", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.Location : null)));
            sqlParameters.Add(DBClient.AddParameters("AdditionalLocationInformation", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.AdditionalLocationInformation : null)));
            //sqlParameters.Add(DBClient.AddParameters("STCLocation", SqlDbType.NVarChar, (JobType == 1 ? createJob.JobSTCDetails != null ? createJob.JobSTCDetails.Location : null : null)));
            //sqlParameters.Add(DBClient.AddParameters("AdditionalLocationInformation", SqlDbType.NVarChar, (JobType == 1 ? createJob.JobSTCDetails != null ? createJob.JobSTCDetails.AdditionalLocationInformation : null : null)));
            sqlParameters.Add(DBClient.AddParameters("batterySystemPartOfAnAggregatedControl", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.batterySystemPartOfAnAggregatedControl : null)));
            sqlParameters.Add(DBClient.AddParameters("changedSettingOfBatteryStorageSystem", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.changedSettingOfBatteryStorageSystem : null)));
            sqlParameters.Add(DBClient.AddParameters("AdditionalSystemInformation", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.AdditionalSystemInformation : null)));
            sqlParameters.Add(DBClient.AddParameters("Latitude", SqlDbType.NVarChar, (createJob.JobSTCDetails != null && createJob.JobSTCDetails.Latitude != null ? String.Format("{0:0.000000000}", Convert.ToDecimal(createJob.JobSTCDetails.Latitude)) : null)));
            sqlParameters.Add(DBClient.AddParameters("Longitude", SqlDbType.NVarChar, (createJob.JobSTCDetails != null && createJob.JobSTCDetails.Longitude != null ? String.Format("{0:0.000000000}", Convert.ToDecimal(createJob.JobSTCDetails.Longitude)) : null)));
            //sqlParameters.Add(DBClient.AddParameters("Latitude", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.Latitude : null)));
            //sqlParameters.Add(DBClient.AddParameters("Longitude", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.Longitude : null)));

            return sqlParameters;
        }


        ///// <summary>
        ///// Businesses the rulefor job STC detail.
        ///// </summary>
        ///// <param name="sqlParameters">The SQL parameters.</param>
        ///// <param name="JobInstallationDetails">The job installation details.</param>
        ///// <param name="JobSTCDetails">The job STC details.</param>
        ///// <param name="LoggedInUserId">The logged in user identifier.</param>
        ///// <returns></returns>
        //public List<SqlParameter> BusinessRuleforJobSTCDetail(List<SqlParameter> sqlParameters, JobInstallationDetails JobInstallationDetails, JobSTCDetails JobSTCDetails, int LoggedInUserId)
        //{
        //    CreateJob createJob = new CreateJob();

        //    if (!string.IsNullOrEmpty(JobInstallationDetails.InstallingNewPanel))
        //    {
        //        if (JobSTCDetails != null)
        //        {
        //            if (JobInstallationDetails.InstallingNewPanel.ToLower() == "new" || JobInstallationDetails.InstallingNewPanel.ToLower() == "replacing")
        //            {
        //                JobSTCDetails.InstallingCompleteUnit = "Yes";
        //                if (JobInstallationDetails.InstallingNewPanel.ToLower() == "new")
        //                {
        //                    JobSTCDetails.AdditionalCapacityNotes = null;
        //                }
        //            }
        //            else
        //            {
        //                JobSTCDetails.InstallingCompleteUnit = "No";
        //            }
        //        }
        //    }
        //    else if (JobSTCDetails != null)
        //    {
        //        JobSTCDetails.AdditionalCapacityNotes = null;
        //    }

        //    if (JobSTCDetails != null)
        //    {
        //        if (!string.IsNullOrEmpty(JobSTCDetails.MultipleSGUAddress))
        //        {
        //            if (JobSTCDetails.MultipleSGUAddress.ToLower() != "yes")
        //            {
        //                JobSTCDetails.Location = null;
        //            }
        //        }
        //        else
        //        {
        //            JobSTCDetails.Location = null;
        //        }

        //        if (JobSTCDetails.TypeOfConnection == "Stand-alone (not connected to an electricity grid)")
        //        {
        //            JobSTCDetails.StandAloneGridSelected = "Yes";
        //        }
        //        else
        //        {
        //            JobSTCDetails.StandAloneGridSelected = "No";
        //        }
        //    }

        //    createJob.JobSTCDetails = JobSTCDetails;
        //    createJob.JobInstallationDetails = JobInstallationDetails;

        //    sqlParameters.Add(DBClient.AddParameters("STCTypeOfConnection", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.TypeOfConnection : null)));
        //    sqlParameters.Add(DBClient.AddParameters("STCSystemMountingType", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.SystemMountingType : null)));
        //    sqlParameters.Add(DBClient.AddParameters("STCInstallingCompleteUnit", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.InstallingCompleteUnit : null)));
        //    sqlParameters.Add(DBClient.AddParameters("STCAdditionalCapacityNotes", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.AdditionalCapacityNotes : null)));
        //    sqlParameters.Add(DBClient.AddParameters("STCDeemingPeriod", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.DeemingPeriod : null)));
        //    sqlParameters.Add(DBClient.AddParameters("STCCertificateCreated", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.CertificateCreated : null)));
        //    sqlParameters.Add(DBClient.AddParameters("STCFailedAccreditationCode", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.FailedAccreditationCode : null)));
        //    sqlParameters.Add(DBClient.AddParameters("STCFailedReason", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.FailedReason : null)));
        //    sqlParameters.Add(DBClient.AddParameters("STCCECAccreditationStatement", SqlDbType.NVarChar, "Yes"));
        //    sqlParameters.Add(DBClient.AddParameters("STCGovernmentSitingApproval", SqlDbType.NVarChar, "Yes"));
        //    sqlParameters.Add(DBClient.AddParameters("STCElectricalSafetyDocumentation", SqlDbType.NVarChar, "Yes"));
        //    sqlParameters.Add(DBClient.AddParameters("STCAustralianNewZealandStandardStatement", SqlDbType.NVarChar, "Yes"));
        //    sqlParameters.Add(DBClient.AddParameters("STCVolumetricCapacity", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.VolumetricCapacity : null)));
        //    sqlParameters.Add(DBClient.AddParameters("STCStatutoryDeclarations", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.StatutoryDeclarations : null)));
        //    sqlParameters.Add(DBClient.AddParameters("STCSecondhandWaterHeater", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.SecondhandWaterHeater : null)));
        //    sqlParameters.Add(DBClient.AddParameters("STCStandAloneGridSelected", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.StandAloneGridSelected : null)));
        //    sqlParameters.Add(DBClient.AddParameters("STCMultipleSGUAddress", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.MultipleSGUAddress : null)));
        //    sqlParameters.Add(DBClient.AddParameters("STCSGUSystemLocated", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.SGUSystemLocated : null)));
        //    sqlParameters.Add(DBClient.AddParameters("STCCreatedBy", SqlDbType.NVarChar, LoggedInUserId));
        //    sqlParameters.Add(DBClient.AddParameters("STCLocation", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.Location : null)));
        //    sqlParameters.Add(DBClient.AddParameters("AdditionalLocationInformation", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.AdditionalLocationInformation : null)));
        //    sqlParameters.Add(DBClient.AddParameters("batterySystemPartOfAnAggregatedControl", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.batterySystemPartOfAnAggregatedControl : null)));
        //    sqlParameters.Add(DBClient.AddParameters("changedSettingOfBatteryStorageSystem", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.changedSettingOfBatteryStorageSystem : null)));
        //    sqlParameters.Add(DBClient.AddParameters("AdditionalSystemInformation", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.AdditionalSystemInformation : null)));
        //    sqlParameters.Add(DBClient.AddParameters("Latitude", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.Latitude : null)));
        //    sqlParameters.Add(DBClient.AddParameters("Longitude", SqlDbType.NVarChar, (createJob.JobSTCDetails != null ? createJob.JobSTCDetails.Longitude : null)));

        //    return sqlParameters;
        //}

        /// <summary>
        /// Updates the STC detail.
        /// </summary>
        /// <param name="createJob">The create job.</param>
        public void UpdateStcDetail(StcObject createJob, int JobType)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            sqlParameters = BusinessRuleforJobSTCDetail(sqlParameters, createJob.JobInstallationDetails, createJob.JobSTCDetails, ProjectSession.LoggedInUserId, JobType);

            sqlParameters.Add(DBClient.AddParameters("BasicJobID", SqlDbType.Int, createJob.JobInstallationDetails.JobID));
            sqlParameters.Add(DBClient.AddParameters("PropertyType", SqlDbType.NVarChar, (createJob.JobInstallationDetails != null ? createJob.JobInstallationDetails.PropertyType : null)));
            sqlParameters.Add(DBClient.AddParameters("SingleMultipleStory", SqlDbType.NVarChar, (createJob.JobInstallationDetails != null ? createJob.JobInstallationDetails.SingleMultipleStory : null)));
            sqlParameters.Add(DBClient.AddParameters("InstallingNewPanel", SqlDbType.NVarChar, (createJob.JobInstallationDetails != null ? createJob.JobInstallationDetails.InstallingNewPanel : null)));
            sqlParameters.Add(DBClient.AddParameters("Location", SqlDbType.NVarChar, (createJob.JobInstallationDetails != null ? createJob.JobInstallationDetails.Location : null)));
            sqlParameters.Add(DBClient.AddParameters("Date", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("InstallationType", SqlDbType.NVarChar, (createJob.JobSystemDetails != null ? createJob.JobSystemDetails.InstallationType : null)));

            CommonDAL.Crud("UpdateJobSTCDetail", sqlParameters.ToArray());
        }

        /// <summary>
        /// Updates the system detail.
        /// </summary>
        /// <param name="jobSystemDetails">The job system details.</param>
        public DataSet UpdateSystemDetail(JobSystemDetails jobSystemDetails)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SystemSize", SqlDbType.Decimal, (jobSystemDetails != null ? jobSystemDetails.SystemSize : null)));
            sqlParameters.Add(DBClient.AddParameters("NoOfPanel", SqlDbType.Int, (jobSystemDetails != null ? jobSystemDetails.NoOfPanel : null)));
            sqlParameters.Add(DBClient.AddParameters("SerialNumbers", SqlDbType.NVarChar, (jobSystemDetails != null ? jobSystemDetails.SerialNumbers : null)));
            //sqlParameters.Add(DBClient.AddParameters("CalculatedSTC", SqlDbType.Decimal, (jobSystemDetails != null ? jobSystemDetails.CalculatedSTC : null)));
            sqlParameters.Add(DBClient.AddParameters("ModifiedCalculatedSTC", SqlDbType.Decimal, (jobSystemDetails != null ? jobSystemDetails.ModifiedCalculatedSTC : null)));
            if (jobSystemDetails != null && jobSystemDetails.jobTypeTab == 2)
            {
                jobSystemDetails.panelXmlTabular = "<Panels><panel><Brand>" + jobSystemDetails.SystemBrand + "</Brand><Model>" + jobSystemDetails.SystemModel + "</Model><NoOfPanel>" + jobSystemDetails.NoOfPanel + "</NoOfPanel></panel></Panels>";
            }
            sqlParameters.Add(DBClient.AddParameters("xmlPanels", SqlDbType.Xml, jobSystemDetails.panelXmlTabular));
            sqlParameters.Add(DBClient.AddParameters("xmlInverters", SqlDbType.Xml, jobSystemDetails.inverterXmlTabular));
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, Convert.ToInt32(jobSystemDetails.JobID)));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));

            sqlParameters.Add(DBClient.AddParameters("batterySystemPartOfAnAggregatedControl", SqlDbType.NVarChar, jobSystemDetails.batterySystemPartOfAnAggregatedControl));
            sqlParameters.Add(DBClient.AddParameters("changedSettingOfBatteryStorageSystem", SqlDbType.NVarChar, jobSystemDetails.changedSettingOfBatteryStorageSystem));

            DataTable dtBatteryManufacturer = ConvertBatteryStorageToList(jobSystemDetails.lstJobBatteryManufacturer, ProjectSession.LoggedInUserId);
            sqlParameters.Add(DBClient.AddParameters("batteryManufacturerList", SqlDbType.Structured, dtBatteryManufacturer));

            DataSet ds = CommonDAL.ExecuteDataSet("UpdateJobSystemDetail", sqlParameters.ToArray()); // Live or Staging
            //CommonDAL.Crud("UpdateJobSystemDetail_Bk", sqlParameters.ToArray()); //Local
            return ds;
        }

        public DataSet UpdateSerialNoDetail(JobSystemDetails jobSystemDetails)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, Convert.ToInt32(jobSystemDetails.JobID)));
            sqlParameters.Add(DBClient.AddParameters("SerialNumbers", SqlDbType.NVarChar, (jobSystemDetails != null ? jobSystemDetails.SerialNumbers : null)));
            sqlParameters.Add(DBClient.AddParameters("InverterSerialNumbers", SqlDbType.NVarChar, (jobSystemDetails != null ? jobSystemDetails.InverterSerialNumbers : null)));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            
            DataSet ds = CommonDAL.ExecuteDataSet("UpdateSerialNoDetail", sqlParameters.ToArray()); // Live or Staging
            //CommonDAL.Crud("UpdateJobSystemDetail_Bk", sqlParameters.ToArray()); //Local
            return ds;
        }

        /// <summary>
        /// Updates the GST detail.
        /// </summary>
        /// <param name="isGST">if set to <c>true</c> [is GST].</param>
        /// <param name="FileName">Name of the file.</param>
        /// <param name="jobId">The job identifier.</param>
        public void UpdateGstDetail(bool isGST, string FileName, int jobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("isGST", SqlDbType.Bit, isGST));
            sqlParameters.Add(DBClient.AddParameters("FileName", SqlDbType.NVarChar, FileName));
            //if (isGST)
            //{
            //    sqlParameters.Add(DBClient.AddParameters("FileName", SqlDbType.NVarChar, FileName));
            //}
            sqlParameters.Add(DBClient.AddParameters("jobId", SqlDbType.Int, jobId));


            CommonDAL.Crud("UpdateGstDetails", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the ces photos by visit check list identifier.
        /// </summary>
        /// <param name="VisitCheckListItemId">The visit check list item identifier.</param>
        /// <returns></returns>
        public DataSet GetCesPhotosByVisitCheckListId(int VisitCheckListItemId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VisitCheckListItemId", SqlDbType.Int, VisitCheckListItemId));
            return CommonDAL.ExecuteDataSet("GetCesPhotosByVisitCheckListId", sqlParameters.ToArray());
        }

        public DataSet GetAllPhotosByJobSchedulingId(int JobSchedulingId, string VisitChecklistItemIds, bool IsReference, bool IsDefault, string ClassType, int JobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobSchedulingId", SqlDbType.Int, JobSchedulingId));
            sqlParameters.Add(DBClient.AddParameters("VisitChecklistItemIds", SqlDbType.NVarChar, VisitChecklistItemIds));
            sqlParameters.Add(DBClient.AddParameters("IsReference", SqlDbType.Bit, IsReference));
            sqlParameters.Add(DBClient.AddParameters("IsDefault", SqlDbType.Bit, IsDefault));
            sqlParameters.Add(DBClient.AddParameters("ClassType", SqlDbType.NVarChar, ClassType));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            return CommonDAL.ExecuteDataSet("GetAllPhotosByJobSchedulingId", sqlParameters.ToArray());
        }
        /// <summary>
        /// Deletes the ces PDF.
        /// </summary>
        /// <param name="VisitCheckListItemId">The visit check list item identifier.</param>
        /// <returns></returns>
        public DataSet DeleteCesPdf(int VisitCheckListItemId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VisitCheckListItemId", SqlDbType.Int, VisitCheckListItemId));
            return CommonDAL.ExecuteDataSet("DeleteCesPdf", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the serial no.
        /// </summary>
        /// <param name="VisitCheckListItemId">The visit check list item identifier.</param>
        /// <returns></returns>
        public DataSet GetSerialNo(int VisitCheckListItemId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VisitCheckListItemId", SqlDbType.Int, VisitCheckListItemId));
            DataSet ds = CommonDAL.ExecuteDataSet("GetSerialNo", sqlParameters.ToArray());
            return ds;
            //return ds.Tables.Count > 0 ? (ds.Tables[0].Rows.Count > 0 ? ds.Tables[0].Rows[0][0].ToString() : "") : "";
        }

        /// <summary>
        /// Gets the check list item for trade.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <returns></returns>
        public List<CheckListItem> GetCheckListItemForTrade(int jobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            IList<CheckListItem> checkListItem = CommonDAL.ExecuteProcedure<CheckListItem>("GetCheckListItemForTrade", sqlParameters.ToArray());
            return checkListItem.ToList();
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="cData">The c data.</param>
        /// <returns></returns>
        public DataSet GetData(List<CommonData> cData)
        {
            DataSet ds = new DataSet();
            foreach (CommonData obj in cData)
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                if (obj.parameters != null && obj.parameters.Length > 0)
                {
                    foreach (Dictionary<string, string> obj1 in obj.parameters)
                    {
                        sqlParameters.Add(DBClient.AddParameters((obj1.Keys).First(), SqlDbType.Int, obj1[(obj1.Keys).First()]));
                    }

                    // sqlParameters.Add(DBClient.AddParameters("VisitCheckListItemId", SqlDbType.Int, VisitCheckListItemId));
                }

                //if (obj.id == "ScoID")
                //{
                //    sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, FormBot.Helper.ProjectSession.LoggedInUserId));
                //}
                if (obj.id == "SSCID")
                {
                    sqlParameters.Add(DBClient.AddParameters("SolarID", SqlDbType.Int, FormBot.Helper.ProjectSession.SolarCompanyId));
                }

                DataTable dt = new DataTable();
                if (obj.id == "BasicDetails_Priority")
                {
                    Type enumType = typeof(Helper.SystemEnums.JobPriority);
                    dt.Columns.Add("Text", typeof(string));
                    dt.Columns.Add("Value", Enum.GetUnderlyingType(enumType));
                    foreach (string name in Enum.GetNames(enumType))
                    {
                        dt.Rows.Add(name.Replace('_', ' '), Enum.Parse(enumType, name));
                    }
                }
                else if (obj.id == "DeemingPeriod")
                {
                    int jobYear = DateTime.Now.Year;
                    if (!string.IsNullOrEmpty(obj.parameters[0].Values.First()))
                    {
                        int.TryParse(obj.parameters[0].Values.First(), out jobYear);
                    }

                    dt.Columns.Add("Text", typeof(string));
                    dt.Columns.Add("Value", typeof(string));

                    List<SelectListItem> Items = GetDeemingPeriod(jobYear).Select(a => new SelectListItem { Text = a, Value = a }).ToList();
                    for (int i = 0; i < Items.Count; i++)
                    {
                        dt.Rows.Add(Items[i].Text, Items[i].Value);
                    }
                }
                else if (obj.proc == "InstallerDesignerEle")
                {
                    string isInstaller = "";
                    string solarCompanyId = "";
                    string jobId = "";

                    obj.parameters[0].TryGetValue("isInstaller", out isInstaller);
                    obj.parameters[0].TryGetValue("solarCompanyId", out solarCompanyId);
                    obj.parameters[0].TryGetValue("jobId", out jobId);

                    int companyId = 0;

                    if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 3 || ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 5)
                        companyId = string.IsNullOrEmpty(solarCompanyId) ? 0 : Convert.ToInt32(solarCompanyId);
                    else
                        companyId = ProjectSession.SolarCompanyId;

                    dt.Columns.Add("Text", typeof(string));
                    dt.Columns.Add("Value", typeof(string));

                    UserBAL objUserbal = new UserBAL();
                    List<SelectListItem> Items = objUserbal.GetInstallerDesignerWithStatus(Convert.ToBoolean(isInstaller), companyId, Convert.ToInt32(jobId), ProjectSession.IsSubContractor).Select(a => new SelectListItem { Text = a.Name, Value = a.InstallerDesignerId.ToString() }).ToList();
                    for (int i = 0; i < Items.Count; i++)
                    {
                        dt.Rows.Add(Items[i].Text, Items[i].Value);
                    }

                }
                else if (obj.id == "JobElectricians_ElectricianID")
                {
                    string solarCompanyId = "";
                    string jobId = "";
                    obj.parameters[0].TryGetValue("solarCompanyId", out solarCompanyId);
                    obj.parameters[0].TryGetValue("JobID", out jobId);

                    int companyId = 0;

                    if (ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 3)
                        companyId = string.IsNullOrEmpty(solarCompanyId) ? 0 : Convert.ToInt32(solarCompanyId);
                    else
                        companyId = ProjectSession.SolarCompanyId;

                    DataSet dataset = GetElectricianBySolarCompanyID(companyId, false, 0, Convert.ToInt32(jobId), 1);

                    string[] strColumn = new string[] { "JobElectricianID", "Name" };

                    dt = dataset.Tables[0].DefaultView.ToTable(false, strColumn);

                }

                else
                {
                    dt = CommonDAL.ExecuteDataSet(obj.proc, sqlParameters.ToArray()).Tables[0].Copy();
                }

                if (dt != null && obj.proc == "Job_GetJobSatge")
                {
                    if (ProjectSession.UserTypeId == 7 || ProjectSession.UserTypeId == 9)
                    {
                        DataTable dtJobStage = dt.AsEnumerable().Where(a => a.Field<Int32>("JobStageId") == 3 || a.Field<Int32>("JobStageId") == 4 || a.Field<Int32>("JobStageId") == 9).CopyToDataTable();
                        dt = dtJobStage;
                    }
                }

                dt.TableName = obj.id;
                ds.Tables.Add(dt);
            }
            return ds;
        }

        /// <summary>
        /// Gets the job custom details.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <returns></returns>
        public List<CustomDetail> GetJobCustomDetails(int JobId, int SolarCompanyId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            List<CustomDetail> lstCustomDetail = CommonDAL.ExecuteProcedure<CustomDetail>("GetJobCustomDetails", sqlParameters.ToArray()).ToList();
            return lstCustomDetail;

        }

        /// <summary>
        /// Fills the PDF and save.
        /// </summary>
        /// <param name="lstPdfItems">The LST PDF items.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="isFill">if set to <c>true</c> [is fill].</param>
        /// <param name="lstSignature">The LST signature.</param>
        public void FillPDFAndSave(List<PdfItems> lstPdfItems, string fileName, bool isFill = false, List<KeyValuePair<int, string>> lstSignature = null)
        {
            //List<PdfItems> lstGetPDFItem = new List<PdfItems>();
            //string jsonPDFData = string.Empty;

            MemoryStream memStream = new MemoryStream();
            using (FileStream fileStream = System.IO.File.OpenRead(fileName))
            {

                memStream.SetLength(fileStream.Length);
                fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
            }
            PdfReader pdfReader = null;
            PdfStamper pdfStamper = null;
            try
            {
                pdfReader = new PdfReader(memStream);

                Type type = typeof(PdfReader);
                FieldInfo info = type.GetField("unethicalreading", BindingFlags.Public | BindingFlags.Static);
                info.SetValue(pdfReader, true);

                pdfStamper = new PdfStamper(pdfReader, new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite));
                AcroFields pdfFormFields = pdfStamper.AcroFields;
                foreach (PdfItems item in lstPdfItems)
                {
                    FillSignature(item, lstSignature, isFill, pdfFormFields, pdfStamper);
                }
                // It will fill date in all fields
                //if (isFill)
                //{
                //    string key = string.Empty;
                //    string value = string.Empty;
                //    foreach (var field in pdfFormFields.Fields)
                //    {
                //        key = Convert.ToString(field.Key);
                //        if (Convert.ToString(field.Key).ToLower().Contains("date"))
                //        {
                //            value = DateTime.Now.ToString(ProjectConfiguration.GetDateFormat.Replace("mm", "MM"));
                //            pdfFormFields.SetField(Convert.ToString(field.Key), value);
                //        }
                //        else if (Convert.ToString(field.Key).ToLower().Contains("_dd") || Convert.ToString(field.Key).ToLower().Contains("_dd1"))
                //        {
                //            value = DateTime.Now.Day.ToString();
                //            pdfFormFields.SetField(Convert.ToString(field.Key), value);
                //        }
                //        else if (Convert.ToString(field.Key).ToLower().Contains("_mm") || Convert.ToString(field.Key).ToLower().Contains("_mm1"))
                //        {
                //            value = DateTime.Now.Month.ToString();
                //            pdfFormFields.SetField(Convert.ToString(field.Key), value);
                //        }
                //        else if (Convert.ToString(field.Key).ToLower().Contains("_yy") || Convert.ToString(field.Key).ToLower().Contains("_yy1"))
                //        {
                //            value = DateTime.Now.Year.ToString();
                //            pdfFormFields.SetField(Convert.ToString(field.Key), value);
                //        }
                //        //lstGetPDFItem.Add(new PdfItems { FieldName = key, Value = value });
                //    }
                //}
                pdfStamper.FormFlattening = false;
                //jsonPDFData = Newtonsoft.Json.JsonConvert.SerializeObject(lstGetPDFItem);
                //return jsonPDFData;
            }
            catch (Exception ex)
            {
                Helper.Log.WriteError(ex);
                //return jsonPDFData;
            }
            finally
            {
                // close the pdf
                if (pdfStamper != null)
                    pdfStamper.Close();
                if (pdfReader != null)
                    pdfReader.Close();
                memStream.Close();
                memStream.Dispose();
            }
        }


        public void FillSignature(PdfItems item, List<KeyValuePair<int, string>> lstSignature = null, bool isFill = false, AcroFields pdfFormFields = null, PdfStamper pdfStamper = null)
        {
            List<KeyValuePair<string, AcroFields.Item>> fieldsName;
            if (!Convert.ToString(item.FieldName).Contains("undefined"))
            {
                if (isFill)
                {
                    fieldsName = pdfFormFields.Fields.Where(m => m.Key.Contains(Convert.ToString(item.FieldName) + "_")).ToList();
                    if (fieldsName.Count == 0)
                    {
                        fieldsName = pdfFormFields.Fields.Where(m => m.Key.Equals(Convert.ToString(item.FieldName))).ToList();
                    }
                }
                else
                {
                    fieldsName = pdfFormFields.Fields.Where(m => m.Key.Contains(Convert.ToString(item.FieldName))).ToList();
                }
            }
            else
            {
                fieldsName = pdfFormFields.Fields.Where(m => m.Key.Equals(Convert.ToString(item.FieldName))).ToList();
            }
            if (isFill && Convert.ToString(item.FieldName).ToLower().Contains("signature"))
            {
                try
                {
                    //float[] fieldPosition = null;

                    foreach (var name in fieldsName)
                    {
                        IList<AcroFields.FieldPosition> fieldPositions = pdfFormFields.GetFieldPositions(name.Key);
                        if (fieldPositions != null)
                        {
                            for (int i = 0; i < fieldPositions.Count; i++)
                            {
                                List<string> lstValues = new List<string>();
                                string signPath = string.Empty;
                                AcroFields.FieldPosition fieldPosition = fieldPositions[i];
                                if (lstSignature != null)
                                {
                                    if (Convert.ToString(item.FieldName).ToLower().Contains("installer_signature"))
                                    {
                                        lstValues = lstSignature.Where(a => a.Key == Convert.ToInt32(SystemEnums.TypeOfSignature.Installer)).Select(a => a.Value).ToList();
                                    }
                                    else if (Convert.ToString(item.FieldName).ToLower().Contains("owner_signature"))
                                    {
                                        lstValues = lstSignature.Where(a => a.Key == Convert.ToInt32(SystemEnums.TypeOfSignature.Home_Owner)).Select(a => a.Value).ToList();
                                    }
                                    else if (Convert.ToString(item.FieldName).ToLower().Contains("electrician_signature"))
                                    {
                                        lstValues = lstSignature.Where(a => a.Key == Convert.ToInt32(SystemEnums.TypeOfSignature.Electrician)).Select(a => a.Value).ToList();
                                    }
                                    else if (Convert.ToString(item.FieldName).ToLower().Contains("designer_signature"))
                                    {
                                        lstValues = lstSignature.Where(a => a.Key == Convert.ToInt32(SystemEnums.TypeOfSignature.Designer)).Select(a => a.Value).ToList();
                                    }
                                    else if (Convert.ToString(item.FieldName).ToLower().Contains("other_signature"))
                                    {
                                        lstValues = lstSignature.Where(a => a.Key == Convert.ToInt32(SystemEnums.TypeOfSignature.Other)).Select(a => a.Value).ToList();
                                    }
                                    else if (Convert.ToString(item.FieldName).ToLower().Contains("Retailer_signature"))
                                    {
                                        lstValues = lstSignature.Where(a => a.Key == Convert.ToInt32(SystemEnums.TypeOfSignature.Retailer)).Select(a => a.Value).ToList();
                                    }

                                }

                                if (lstValues.Count > 0)
                                    signPath = Path.Combine(ProjectSession.ProofDocuments + lstValues[0]);
                                else
                                {
                                    if (string.IsNullOrEmpty(item.ImageName))
                                    {
                                        signPath = Path.Combine(ProjectSession.ProofDocuments + "\\" + item.Value);
                                        item.ImageName = item.Value;
                                    }
                                    else
                                        signPath = Path.Combine(ProjectSession.ProofDocuments + "\\" + item.ImageName);
                                }


                                string signFileName = string.Empty;
                                if (lstValues.Count > 0)
                                    signFileName = Path.GetFileName(lstValues[0]);
                                else
                                {
                                    if (string.IsNullOrEmpty(item.ImageName))
                                        signFileName = item.Value;
                                    else
                                        signFileName = item.ImageName;
                                }
                                if (!string.IsNullOrEmpty(signFileName) && System.IO.File.Exists(signPath) && fieldPosition != null)
                                {
                                    Bitmap original;
                                    Bitmap thumbnailImg;
                                    using (FileStream fs = new System.IO.FileStream(signPath, System.IO.FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                    {
                                        original = new Bitmap(fs);
                                    }
                                    long len = new FileInfo(signPath).Length;
                                    if ((len / 1024) > 1024)
                                    {
                                        using (var image = Image.FromFile(signPath))
                                        {

                                            var newWidth = (int)(image.Width * 0.1);
                                            var newHeight = (int)(image.Height * 0.1);
                                            thumbnailImg = new Bitmap(newWidth, newHeight);
                                            var thumbGraph = Graphics.FromImage(thumbnailImg);
                                            thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                                            thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                                            thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                            var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                                            thumbGraph.DrawImage(image, imageRectangle);
                                        }
                                    }
                                    else
                                    {
                                        thumbnailImg = new Bitmap(signPath);
                                    }
                                    //iTextSharp.text.Image image1 = iTextSharp.text.Image.GetInstance(bitMapSignPath, iTextSharp.text.BaseColor.WHITE);
                                    System.IO.MemoryStream ms = new MemoryStream();
                                    thumbnailImg.Save(ms, ImageFormat.Png);
                                    byte[] byteImage = ms.ToArray();
                                    var SigBase64 = Convert.ToBase64String(byteImage);
                                    iTextSharp.text.Image image1 = iTextSharp.text.Image.GetInstance(thumbnailImg, ImageFormat.Png);
                                    if (item.FieldName.Equals("Reseller_Signature"))
                                    {
                                        image1.ScaleAbsolute(fieldPosition.position);
                                        image1.SetAbsolutePosition(fieldPosition.position.Left, fieldPosition.position.Bottom);
                                        PdfContentByte overContent = pdfStamper.GetOverContent(fieldPosition.page);
                                        overContent.AddImage(image1);
                                        item.ReadOnly = true;
                                    }
                                    else
                                    {
                                        PushbuttonField tx = pdfFormFields.GetNewPushbuttonFromField(name.Key, i);
                                        tx.Box = fieldPosition.position;
                                        //pdfFormFields.DecodeGenericDictionary(pdfFormFields.GetFieldItem(Convert.ToString(item.FieldName)).GetMerged(0), tx);
                                        item.Value = "data:image/png;base64," + SigBase64;
                                        item.Base64 = "data:image/png;base64," + SigBase64;
                                        tx.Text = item.Value;
                                        byte[] bytes = Convert.FromBase64String(item.Base64.Split(',')[1]);
                                        tx.Image = image1;
                                        if (tx.Options == 1)
                                        {
                                            item.ReadOnly = true;
                                        }
                                        pdfFormFields.ReplacePushbuttonField(name.Key, tx.Field, i);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Helper.Log.WriteError(ex);
                }
            }
            else if (item.Type == (int)FormBot.Helper.SystemEnums.InputTypes.BUTTON)
            {
                foreach (var name in fieldsName)
                {
                    PushbuttonField tx = pdfFormFields.GetNewPushbuttonFromField(name.Key);
                    tx.FieldName = name.Key;
                    tx.Text = item.Value;
                    if (item.Base64.Split(',').ToList().Count > 1 && !string.IsNullOrEmpty(item.Base64.Split(',')[1]))
                    {
                        byte[] bytes = Convert.FromBase64String(item.Base64.Split(',')[1]);
                        tx.Image = iTextSharp.text.Image.GetInstance(bytes);
                    }
                    if (tx.Options == 1)
                    {
                        item.ReadOnly = true;
                    }
                    pdfFormFields.ReplacePushbuttonField(Convert.ToString(item.FieldName), tx.Field);
                }


            }
            else if (item.Type == (int)FormBot.Helper.SystemEnums.InputTypes.CHECK_BOX)
            {
                AcroFields.FieldPosition fieldPosition = pdfFormFields.GetFieldPositions(item.FieldName.ToString())[0];
                pdfStamper.AcroFields.RemoveField(item.FieldName.ToString());
                string value = Convert.ToString(item.Value);
                RadioCheckField checkbox = new RadioCheckField(pdfStamper.Writer, fieldPosition.position, item.FieldName.ToString(), value);
                checkbox.CheckType = RadioCheckField.TYPE_CHECK;
                checkbox.Checked = item.Value == "on" ? true : false;// Convert.ToBoolean(obj["check"]);
                PdfFormField fieldC = checkbox.CheckField;
                pdfStamper.AddAnnotation(fieldC, fieldPosition.page);
                pdfFormFields.SetField(item.FieldName.ToString(), value);
            }
            else
            {
                foreach (var name in fieldsName)
                {
                    pdfFormFields.SetField(name.Key, item.Value);
                }
            }
        }

        /// <summary>
        /// Gets the photos path.
        /// </summary>
        /// <param name="VisitCheckListPhotoIds">The visit check list photo ids.</param>
        /// <param name="VisitSignatureIds">The visit signature ids.</param>
        /// <returns></returns>
        public DataSet GetPhotosPath(string VisitCheckListPhotoIds, string VisitSignatureIds)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VisitCheckListPhotoIds", SqlDbType.NVarChar, VisitCheckListPhotoIds));
            sqlParameters.Add(DBClient.AddParameters("VisitSignatureIds", SqlDbType.NVarChar, VisitSignatureIds));
            DataSet ds = CommonDAL.ExecuteDataSet("Get_VisitCheckListPhotos_VisitSignature", sqlParameters.ToArray());
            return ds;
        }

        //get all photos for tabular view
        public DataSet GetPhotosForAllTabular(int JobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            DataSet ds = CommonDAL.ExecuteDataSet("GetAllPhotosForDownload", sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Gets the required detail to set STC value.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <returns></returns>
        public DataSet GetRequiredDetailToSetSTCValue(int jobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            DataSet dsSTCParam = CommonDAL.ExecuteDataSet("GetRequiredDetailToSetSTCValue", sqlParameters.ToArray());
            return dsSTCParam;
        }

        /// <summary>
        /// Deletes the job images vendor API.
        /// </summary>
        /// <param name="VendorJobPhotoId">The vendor job photo identifier.</param>
        /// <param name="IsClassic">if set to <c>true</c> [is classic].</param>
        /// <returns></returns>
        public DataSet DeleteJobImages_VendorApi(string VendorJobPhotoId, bool IsClassic)
        {
            string spName = "[DeleteJobImages_VendorApi]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VendorJobPhotoId", SqlDbType.NVarChar, VendorJobPhotoId));
            sqlParameters.Add(DBClient.AddParameters("IsClassic", SqlDbType.Bit, IsClassic));
            DataSet ds = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return ds;
        }

        public DataSet GetCaptureUserSignDetail(string jobDocId, string Fieldname)
        {
            string spName = "[GetCaptureUserSignDetail]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobDocumentId", SqlDbType.Int, Convert.ToInt32(jobDocId)));
            sqlParameters.Add(DBClient.AddParameters("Fiedlname", SqlDbType.NVarChar, Fieldname));
            DataSet ds = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return ds;
        }
        public int CheckPreviousSmsTime(string MobileNo)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("MobileNo", SqlDbType.NVarChar, MobileNo));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            int result = Convert.ToInt32(CommonDAL.ExecuteScalar("CheckPreviousSmsTime", sqlParameters.ToArray()));
            return result;
        }
        /// <summary>
        /// Deletes the documents vendor API.
        /// </summary>
        /// <param name="VendorJobDocumentId">The vendor job document identifier.</param>
        /// <param name="IsClassic">if set to <c>true</c> [is classic].</param>
        /// <returns></returns>
        public DataSet DeleteDocuments_VendorApi(string VendorJobDocumentId, bool IsClassic)
        {
            string spName = "[DeleteDocuments_VendorApi]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VendorJobDocumentId", SqlDbType.NVarChar, VendorJobDocumentId));
            sqlParameters.Add(DBClient.AddParameters("IsClassic", SqlDbType.Bit, IsClassic));
            DataSet ds = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Moves the deleted documents vendor API.
        /// </summary>
        /// <param name="VendorJobDocumentId">The vendor job document identifier.</param>
        /// <param name="Path">The path.</param>
        public void MoveDeletedDocuments_VendorApi(string VendorJobDocumentId, string Path)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VendorJobDocumentId", SqlDbType.NVarChar, VendorJobDocumentId));
            sqlParameters.Add(DBClient.AddParameters("Path", SqlDbType.NVarChar, Path));
            CommonDAL.Crud("MoveDeletedDocuments_VendorApi", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the job identifier by vendorjob identifier.
        /// </summary>
        /// <param name="VendorJobId">The vendor job identifier.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <returns></returns>
        public DataTable GetJobIdByVendorjobId(string VendorJobId, int SolarCompanyId)
        {
            string spName = "[GetJobId_VendorApi]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VendorJobId", SqlDbType.NVarChar, VendorJobId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            DataTable dt = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray()).Tables[0];
            return dt;
        }

        /// <summary>
        /// Checks the panel inverter system brand vendor API.
        /// </summary>
        /// <param name="JobType">Type of the job.</param>
        /// <param name="dtPanelSystemBrand">The dt panel system brand.</param>
        /// <param name="dtInverterDetail">The dt inverter detail.</param>
        /// <param name="JobId">JobId.</param>
        /// <returns></returns>
        public DataSet CheckPanelInverterSystemBrand_VendorApi(int JobType, DataTable dtPanelSystemBrand, DataTable dtInverterDetail, int JobId = 0)
        {
            string spName = "[CheckPanelInverterSystemBrand_VendorApi]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobType", SqlDbType.Int, JobType));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("dtPanelSystemBrand", SqlDbType.Structured, dtPanelSystemBrand));
            sqlParameters.Add(DBClient.AddParameters("dtInverterDetail", SqlDbType.Structured, dtInverterDetail));
            DataSet ds = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Writes to log file.
        /// </summary>
        /// <param name="errorMsg">The error MSG.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="ID">The identifier.</param>
        private void WriteToLogFile(string errorMsg, string methodName, string ID)
        {
            string sError = "Date:" + DateTime.Now + " MethodName:" + methodName + " " + ID + " Error:" + errorMsg;
            StreamWriter sw = new StreamWriter(ProjectSession.VendorAPIErrorLogs + "ErrorLogs.txt", append: true);
            sw.WriteLine(sError + "\n");
            sw.Flush();
            sw.Close();
        }

        /// <summary>
        /// Checks the vendor job photo identifier vendor API.
        /// </summary>
        /// <param name="VendorJobPhotoId">The vendor job photo identifier.</param>
        /// <param name="JobId">The job identifier.</param>
        /// <param name="IsClassic">if set to <c>true</c> [is classic].</param>
        /// <returns></returns>
        public string CheckVendorJobPhotoId_VendorApi(string VendorJobPhotoId, int JobId, bool IsClassic)
        {
            string spName = "[CheckVendorJobPhotoId_VendorApi]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VendorJobPhotoId", SqlDbType.NVarChar, VendorJobPhotoId));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("IsClassic", SqlDbType.Bit, IsClassic));
            object vendorJobPhotoId = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            return Convert.ToString(vendorJobPhotoId);
        }

        /// <summary>
        /// Checks the vendor job document identifier vendor API.
        /// </summary>
        /// <param name="VendorJobDocumentId">The vendor job document identifier.</param>
        /// <param name="JobId">The job identifier.</param>
        /// <param name="IsClassic">if set to <c>true</c> [is classic].</param>
        /// <returns></returns>
        public string CheckVendorJobDocumentId_VendorApi(string VendorJobDocumentId, int JobId, bool IsClassic)
        {
            string spName = "[CheckVendorJobDocumentId_VendorApi]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VendorJobDocumentId", SqlDbType.NVarChar, VendorJobDocumentId));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("IsClassic", SqlDbType.Bit, IsClassic));
            object vendorJobDocumentId = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            return Convert.ToString(vendorJobDocumentId);
        }

        /// <summary>
        /// Gets the custom field details.
        /// </summary>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <returns></returns>
        public List<VendorCustomField> GetCustomFieldDetails(int SolarCompanyId)
        {
            string spName = "[GetCustomField_VendorAPI]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));

            IList<VendorCustomField> CustomFieldList = CommonDAL.ExecuteProcedure<VendorCustomField>(spName, sqlParameters.ToArray());
            return CustomFieldList.ToList();
        }

        /// <summary>
        /// Converts the battery storage to list.
        /// </summary>
        /// <param name="batteryManufacturerList">The battery manufacturer list.</param>
        /// <param name="LoggedInUserId">The logged in user identifier.</param>
        /// <returns></returns>
        public DataTable ConvertBatteryStorageToList(List<JobBatteryManufacturer> batteryManufacturerList, int LoggedInUserId)
        {
            DataTable dtBatteryStorage = CreateBatteryStorageTable();
            if (batteryManufacturerList != null)
            {
                for (int i = 0; i < batteryManufacturerList.Count; i++)
                {
                    DataRow dr = dtBatteryStorage.NewRow();
                    dr["ManufacturerCertificateHolder"] = batteryManufacturerList[i].Manufacturer;
                    dr["ModelNumber"] = batteryManufacturerList[i].ModelNumber;
                    dr["CreatedDate"] = DateTime.Now;
                    dr["CreatedBy"] = LoggedInUserId;
                    dtBatteryStorage.Rows.Add(dr);
                }
            }
            return dtBatteryStorage;
        }

        /// <summary>
        /// Creates the battery storage table.
        /// </summary>
        /// <returns></returns>
        public DataTable CreateBatteryStorageTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("EquipmentCategory", typeof(string));
            dt.Columns.Add("CompliancePathway", typeof(string));
            dt.Columns.Add("ManufacturerCertificateHolder", typeof(string));
            dt.Columns.Add("BrandName", typeof(string));
            dt.Columns.Add("Series", typeof(string));
            dt.Columns.Add("ModelNumber", typeof(string));
            dt.Columns.Add("RatedApparentACPowerkVA", typeof(decimal));
            dt.Columns.Add("NominalBatteryCapacitykWh", typeof(decimal));
            dt.Columns.Add("DepthOfDischarge", typeof(int));
            dt.Columns.Add("UsableCapacitykWh", typeof(decimal));
            dt.Columns.Add("MinOperatingTemp", typeof(int));
            dt.Columns.Add("MaxOperatingTemp", typeof(int));
            dt.Columns.Add("OutdoorUsage", typeof(string));
            dt.Columns.Add("CECApprovalDate", typeof(DateTime));
            dt.Columns.Add("CECExpiryDate", typeof(DateTime));
            dt.Columns.Add("CreatedDate", typeof(DateTime));
            dt.Columns.Add("CreatedBy", typeof(int));
            return dt;
        }

        /// <summary>
        /// Gets the job list user wise columns.
        /// </summary>
        /// <param name="JobViewMenuId">The job view menu identifier.</param>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="PageNumber">The page number.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <param name="SortCol">The sort col.</param>
        /// <param name="SortDir">The sort dir.</param>
        /// <param name="UrgentJobDay">The urgent job day.</param>
        /// <param name="StageId">The stage identifier.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <param name="IsArchive">if set to <c>true</c> [is archive].</param>
        /// <param name="ScheduleType">Type of the schedule.</param>
        /// <param name="JobType">Type of the job.</param>
        /// <param name="JobPriority">The job priority.</param>
        /// <param name="searchtext">The searchtext.</param>
        /// <param name="FromDate">From date.</param>
        /// <param name="ToDate">To date.</param>
        /// <param name="IsGst">if set to <c>true</c> [is GST].</param>
        /// <param name="jobref">if set to <c>true</c> [jobref].</param>
        /// <param name="jobdescription">if set to <c>true</c> [jobdescription].</param>
        /// <param name="jobaddress">if set to <c>true</c> [jobaddress].</param>
        /// <param name="jobclient">if set to <c>true</c> [jobclient].</param>
        /// <param name="jobstaff">if set to <c>true</c> [jobstaff].</param>
        /// <param name="Invoiced">if set to <c>true</c> [invoiced].</param>
        /// <param name="NotInvoiced">if set to <c>true</c> [not invoiced].</param>
        /// <param name="ReadyToTrade">if set to <c>true</c> [ready to trade].</param>
        /// <param name="NotReadyToTrade">if set to <c>true</c> [not ready to trade].</param>
        /// <param name="traded">if set to <c>true</c> [traded].</param>
        /// <param name="nottraded">if set to <c>true</c> [nottraded].</param>
        /// <param name="preapprovalnotapproved">if set to <c>true</c> [preapprovalnotapproved].</param>
        /// <param name="preapprovalapproved">if set to <c>true</c> [preapprovalapproved].</param>
        /// <param name="connectioncompleted">if set to <c>true</c> [connectioncompleted].</param>
        /// <param name="connectionnotcompleted">if set to <c>true</c> [connectionnotcompleted].</param>
        /// <param name="ACT">if set to <c>true</c> [act].</param>
        /// <param name="NSW">if set to <c>true</c> [NSW].</param>
        /// <param name="NT">if set to <c>true</c> [nt].</param>
        /// <param name="QLD">if set to <c>true</c> [QLD].</param>
        /// <param name="SA">if set to <c>true</c> [sa].</param>
        /// <param name="TAS">if set to <c>true</c> [tas].</param>
        /// <param name="WA">if set to <c>true</c> [wa].</param>
        /// <param name="VIC">if set to <c>true</c> [vic].</param>
        /// <param name="PreApprovalStatusId">The pre approval status identifier.</param>
        /// <param name="ConnectionStatusId">The connection status identifier.</param>
        /// <returns></returns>
        public DataSet GetJobList_UserWiseColumns(int JobViewMenuId, int UserId, int UserTypeId, int PageNumber, int PageSize, string SortCol, string SortDir, int UrgentJobDay, int StageId, int SolarCompanyId, bool IsArchive, int ScheduleType, int JobType, int JobPriority, string searchtext, DateTime? FromDate, DateTime? ToDate, bool IsGst, bool jobref, bool jobdescription, bool jobaddress, bool jobclient, bool jobstaff, bool Invoiced, bool NotInvoiced, bool ReadyToTrade, bool NotReadyToTrade, bool traded, bool nottraded, bool preapprovalnotapproved, bool preapprovalapproved, bool connectioncompleted, bool connectionnotcompleted, bool ACT, bool NSW, bool NT, bool QLD, bool SA, bool TAS, bool WA, bool VIC, int PreApprovalStatusId, int ConnectionStatusId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobViewMenuId", SqlDbType.Int, JobViewMenuId));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("UrgentJobDay", SqlDbType.Int, UrgentJobDay));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("IsArchive", SqlDbType.Bit, IsArchive));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, PageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, PageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, SortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, SortDir));
            sqlParameters.Add(DBClient.AddParameters("StageId", SqlDbType.Int, StageId));
            sqlParameters.Add(DBClient.AddParameters("ScheduleType", SqlDbType.Int, ScheduleType));
            sqlParameters.Add(DBClient.AddParameters("JobType", SqlDbType.Int, JobType));
            sqlParameters.Add(DBClient.AddParameters("IsGst", SqlDbType.Bit, IsGst));
            sqlParameters.Add(DBClient.AddParameters("JobPriority", SqlDbType.Int, JobPriority));
            sqlParameters.Add(DBClient.AddParameters("searchtext", SqlDbType.NVarChar, searchtext));
            sqlParameters.Add(DBClient.AddParameters("FromDate", SqlDbType.DateTime, FromDate != null ? FromDate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("ToDate", SqlDbType.DateTime, ToDate != null ? ToDate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("Invoiced", SqlDbType.Bit, Invoiced));
            sqlParameters.Add(DBClient.AddParameters("NotInvoiced", SqlDbType.Bit, NotInvoiced));
            sqlParameters.Add(DBClient.AddParameters("ReadyToTrade", SqlDbType.Bit, ReadyToTrade));
            sqlParameters.Add(DBClient.AddParameters("NotReadyToTrade", SqlDbType.Bit, NotReadyToTrade));
            sqlParameters.Add(DBClient.AddParameters("IsJobRef", SqlDbType.Bit, jobref));
            sqlParameters.Add(DBClient.AddParameters("IsJobDescription", SqlDbType.Bit, jobdescription));
            sqlParameters.Add(DBClient.AddParameters("IsJobAddress", SqlDbType.Bit, jobaddress));
            sqlParameters.Add(DBClient.AddParameters("IsJobClient", SqlDbType.Bit, jobclient));
            sqlParameters.Add(DBClient.AddParameters("IsJobStaff", SqlDbType.Bit, jobstaff));
            sqlParameters.Add(DBClient.AddParameters("IsTraded", SqlDbType.Bit, traded));
            sqlParameters.Add(DBClient.AddParameters("IsNotTraded", SqlDbType.Bit, nottraded));
            sqlParameters.Add(DBClient.AddParameters("IsPreApprovalNotApproved", SqlDbType.Bit, preapprovalnotapproved));
            sqlParameters.Add(DBClient.AddParameters("IsPreApprovalApproved", SqlDbType.Bit, preapprovalapproved));
            sqlParameters.Add(DBClient.AddParameters("IsConnectionCompleted", SqlDbType.Bit, connectioncompleted));
            sqlParameters.Add(DBClient.AddParameters("IsConnectionNotCompleted", SqlDbType.Bit, connectionnotcompleted));
            sqlParameters.Add(DBClient.AddParameters("IsACT", SqlDbType.Bit, ACT));
            sqlParameters.Add(DBClient.AddParameters("IsNSW", SqlDbType.Bit, NSW));
            sqlParameters.Add(DBClient.AddParameters("IsNT", SqlDbType.Bit, NT));
            sqlParameters.Add(DBClient.AddParameters("IsQLD", SqlDbType.Bit, QLD));
            sqlParameters.Add(DBClient.AddParameters("IsSA", SqlDbType.Bit, SA));
            sqlParameters.Add(DBClient.AddParameters("IsTAS", SqlDbType.Bit, TAS));
            sqlParameters.Add(DBClient.AddParameters("IsWA", SqlDbType.Bit, WA));
            sqlParameters.Add(DBClient.AddParameters("IsVIC", SqlDbType.Bit, VIC));
            sqlParameters.Add(DBClient.AddParameters("PreApprovalStatusId", SqlDbType.Int, PreApprovalStatusId));
            sqlParameters.Add(DBClient.AddParameters("ConnectionStatusId", SqlDbType.Int, ConnectionStatusId));
            sqlParameters.Add(DBClient.AddParameters("CurrentDateTime", SqlDbType.DateTime, DateTime.Now));

            DataSet dsJobsPlusColumns = CommonDAL.ExecuteDataSet("Job_GetJobList_UserWiseColumns", sqlParameters.ToArray());
            return dsJobsPlusColumns;
        }

        /// <summary>
        /// Gets the job list for caching data.
        /// </summary>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="JobViewMenuId">The job view menu identifier.</param>
        /// <returns></returns>
        public DataSet GetJobList_ForCachingData(string SolarCompanyId, int UserId, int JobViewMenuId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.NVarChar, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("JobViewMenuId", SqlDbType.Int, JobViewMenuId));
            sqlParameters.Add(DBClient.AddParameters("CurrentDateTime", SqlDbType.DateTime, DateTime.Now));
            DataSet dsJobsPlusColumns = CommonDAL.ExecuteDataSet("Job_GetJobList_ForCachingData", sqlParameters.ToArray());
            return dsJobsPlusColumns;
        }
        /// <summary>
        /// Gets Solar Jobs without Cache
        /// </summary>
        /// <param name="SolarCompanyId"></param>
        /// <param name="UserId"></param>
        /// <param name="JobViewMenuId"></param>
        /// <param name="year"></param>
        /// <param name="isDefault"></param>
        /// <param name="strYear"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="dtFilter"></param>
        /// <returns></returns>

        public JobViewLists GetJobList_ForCachingDataKendoByYearWithoutCacheDapper(string SolarCompanyId, int UserId, int JobViewMenuId, int year, string strYear = "", int page = 1, int pageSize = 10, DataTable dtFilter = null, DataTable dtSort = null)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();

            dynamicParameters.Add("SolarCompanyID", SolarCompanyId, DbType.String);
            dynamicParameters.Add("UserId", UserId, DbType.Int32);
            dynamicParameters.Add("JobViewMenuId", JobViewMenuId, DbType.Int32);
            dynamicParameters.Add("CurrentDateTime", DateTime.Now, DbType.DateTime);
            dynamicParameters.Add("Year", year, DbType.Int32);
            dynamicParameters.Add("strYear", strYear, DbType.String);
            dynamicParameters.Add("dtFilter", dtFilter, DbType.Object);
            dynamicParameters.Add("dtSort", dtSort, DbType.Object);
            dynamicParameters.Add("page", page, DbType.Int32);
            dynamicParameters.Add("pagesize", pageSize, DbType.Int32);
            return CommonDAL.ExecuteProcedureJobViewDapper("Job_GetJobList_ForCachingData_KendoByYearNew", dynamicParameters);
        }

        /// <summary>
        /// Gets the column master.
        /// </summary>
        /// <param name="JobViewMenuId">The job view menu identifier.</param>
        /// <returns></returns>
        public List<ColumnMaster> GetColumnMaster(int JobViewMenuId)
        {
            List<ColumnMaster> listColumnMaster = new List<ColumnMaster>();
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobViewMenuId", SqlDbType.Int, JobViewMenuId));

            DataSet dsColumnMaster = CommonDAL.ExecuteDataSet("GetColumnMaster", sqlParameters.ToArray());
            if (dsColumnMaster != null && dsColumnMaster.Tables.Count > 0)
            {
                if (dsColumnMaster.Tables[0] != null && dsColumnMaster.Tables[0].Rows.Count > 0)
                {
                    listColumnMaster = dsColumnMaster.Tables[0].ToListof<ColumnMaster>();
                }
            }
            return listColumnMaster;
        }

        /// <summary>
        /// Gets the user wise columns.
        /// </summary>
        /// <param name="UserID">The user identifier.</param>
        /// <param name="JobViewMenuId">The job view menu identifier.</param>
        /// <returns></returns>
        public List<UserWiseColumns> GetUserWiseColumns(int UserID, int JobViewMenuId)
        {
            List<UserWiseColumns> listUserWiseColumns = new List<UserWiseColumns>();
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserID));
            sqlParameters.Add(DBClient.AddParameters("JobViewMenuId", SqlDbType.Int, JobViewMenuId));

            return CommonDAL.ExecuteProcedure<UserWiseColumns>("GetUserWiseColumn", sqlParameters.ToArray()).ToList();
            //DataSet dsColumnMaster = CommonDAL.ExecuteDataSet("GetUserWiseColumn", sqlParameters.ToArray());
            //if (dsColumnMaster != null && dsColumnMaster.Tables.Count > 0)
            //{
            //    if (dsColumnMaster.Tables[0] != null && dsColumnMaster.Tables[0].Rows.Count > 0)
            //    {
            //        //  listUserWiseColumns = dsColumnMaster.Tables[0].ToListof<UserWiseColumns>();
            //        listUserWiseColumns = DBClient.DataTableToList<UserWiseColumns>(dsColumnMaster.Tables[0]);
            //    }
            //}
            //return listUserWiseColumns;
        }

        /// <summary>
        /// Saves the user wise columns.
        /// </summary>
        /// <param name="dtColumns">The dt columns.</param>
        /// <param name="UserID">The user identifier.</param>
        /// <param name="JobViewMenuId">The job view menu identifier.</param>
        public void SaveUserWiseColumns(DataTable dtColumns, int UserID, int JobViewMenuId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("dtColumns", SqlDbType.Structured, dtColumns));
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserID));
            sqlParameters.Add(DBClient.AddParameters("JobViewMenuId", SqlDbType.Int, JobViewMenuId));
            CommonDAL.Crud("UserWiseColumns_Insert", sqlParameters.ToArray());
        }

        /// <summary>
        /// Resets the default columns.
        /// </summary>
        /// <param name="UserID">The user identifier.</param>
        /// <param name="JobViewMenuId">The job view menu identifier.</param>
        /// <returns></returns>
        public List<UserWiseColumns> ResetDefaultColumns(int UserID, int JobViewMenuId)
        {
            List<UserWiseColumns> listUserWiseColumns = new List<UserWiseColumns>();
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserID));
            sqlParameters.Add(DBClient.AddParameters("JobViewMenuId", SqlDbType.Int, JobViewMenuId));

            DataSet dsColumnMaster = CommonDAL.ExecuteDataSet("UserWiseColumns_Delete", sqlParameters.ToArray());
            if (dsColumnMaster != null && dsColumnMaster.Tables.Count > 0)
            {
                if (dsColumnMaster.Tables[0] != null && dsColumnMaster.Tables[0].Rows.Count > 0)
                {
                    listUserWiseColumns = DBClient.DataTableToList<UserWiseColumns>(dsColumnMaster.Tables[0]);
                }
            }
            return listUserWiseColumns;
        }


        //public DataSet GetAdvanceSearchCategory()
        //{
        //    List<SqlParameter> sqlParameters = new List<SqlParameter>();
        //    DataSet dsAdvanceSearchCategory = CommonDAL.ExecuteDataSet("GetAdvanceSearchCategory", sqlParameters.ToArray());
        //    return dsAdvanceSearchCategory;
        //}

        /// <summary>
        /// Gets the advance search category.
        /// </summary>
        /// <returns></returns>
        public DataSet GetAdvanceSearchCategory()
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            DataSet dsAdvanceSearchCategory = CommonDAL.ExecuteDataSet("GetAdvanceSearchCategory", sqlParameters.ToArray());
            return dsAdvanceSearchCategory;
        }
        /// <summary>
        /// Gets the job identifier wise caching data.
        /// </summary>
        /// <param name="JobID">The job identifier.</param>
        /// <returns></returns>
        public DataRow GetJobIDWiseCachingData(string JobID)
        {
            DataRow dr = null;
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.NVarChar, JobID));
            sqlParameters.Add(DBClient.AddParameters("CurrentDateTime", SqlDbType.DateTime, DateTime.Now));
            DataSet dsJobIDWiseCachingData = CommonDAL.ExecuteDataSet("GetJobIDWiseCachingData", sqlParameters.ToArray());
            if (dsJobIDWiseCachingData != null && dsJobIDWiseCachingData.Tables.Count > 0 && dsJobIDWiseCachingData.Tables[0].Rows.Count > 0)
            {
                dr = dsJobIDWiseCachingData.Tables[0].Rows[0];
            }
            return dr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="STCJobDetailsId"></param>
        /// <returns></returns>
        public DataRow GetSTCJobDetailsIDWiseCachingData(int STCJobDetailsId)
        {
            DataRow dr = null;
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCJobDetailsId", SqlDbType.Int, STCJobDetailsId));
            DataSet dsSTCJobDetailsIDWiseCachingData = CommonDAL.ExecuteDataSet("GetSTCJobDetailsIDWiseCachingData", sqlParameters.ToArray());
            if (dsSTCJobDetailsIDWiseCachingData != null && dsSTCJobDetailsIDWiseCachingData.Tables.Count > 0 && dsSTCJobDetailsIDWiseCachingData.Tables[0].Rows.Count > 0)
            {
                dr = dsSTCJobDetailsIDWiseCachingData.Tables[0].Rows[0];
            }
            return dr;
        }

        /// <summary>
        /// Get STc Invoice Id Wise Caching Data
        /// </summary>
        /// <param name="STCJobDetailsId"></param>
        /// <returns></returns>
        public DataRow GetSTCInvoiceIDWiseCachingData(int STCInvoiceId)
        {
            DataRow dr = null;
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCInvoiceId", SqlDbType.Int, STCInvoiceId));
            DataSet dsSTCInvoiceIDWiseCachingData = CommonDAL.ExecuteDataSet("GetSTCInvoiceIDWiseCachingData", sqlParameters.ToArray());
            if (dsSTCInvoiceIDWiseCachingData != null && dsSTCInvoiceIDWiseCachingData.Tables.Count > 0 && dsSTCInvoiceIDWiseCachingData.Tables[0].Rows.Count > 0)
            {
                dr = dsSTCInvoiceIDWiseCachingData.Tables[0].Rows[0];
            }
            return dr;
        }

        /// <summary>
        /// Gets the STC submission list vendor API.
        /// </summary>
        /// <param name="VendorJobId">The vendor job identifier.</param>
        /// <returns></returns>
        public List<FormBot.Entity.Job.STCBasicDetails.STCSubmissionModel> GetStcSubmissionList_VendorAPI(string VendorJobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VendorJobId", SqlDbType.NVarChar, VendorJobId));

            string spName = "[GetStcSubmissionList_VendorAPI]";

            IList<FormBot.Entity.Job.STCBasicDetails.STCSubmissionModel> StcSubmission = CommonDAL.ExecuteProcedure<FormBot.Entity.Job.STCBasicDetails.STCSubmissionModel>(spName, sqlParameters.ToArray());
            return StcSubmission.ToList();
        }

        /// <summary>
        /// Gets the job photo vendor API.
        /// </summary>
        /// <param name="VendorJobId">The vendor job identifier.</param>
        /// <returns></returns>
        public List<VendorJobPhotoList> GetJobPhoto_VendorAPI(string VendorJobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VendorJobId", SqlDbType.Int, VendorJobId));

            string spName = "[GetJobPhotos_VendorAPI]";

            IList<VendorJobPhotoList> jobPhotoList = CommonDAL.ExecuteProcedure<VendorJobPhotoList>(spName, sqlParameters.ToArray());
            return jobPhotoList.ToList();
        }

        /// <summary>
        /// Checks the existing custom electrician.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns></returns>
        public bool CheckExistingCustomElectrician(string fullName, int solarCompanyId, int jobId)
        {
            string spName = "[CheckExistingCustomElectrician]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("FullName", SqlDbType.NVarChar, fullName));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobId));
            return Convert.ToBoolean(CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray()));
        }

        /// <summary>
        /// JobElectricians_InsertUpdate
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="profileType"></param>
        /// <param name="jobElectricians"></param>
        /// <param name="LoggedInUserId"></param>
        /// <param name="isCreateNew"></param>
        /// <returns></returns>
        public int JobElectricians_InsertUpdate(int jobId, int profileType, JobElectricians jobElectricians, int LoggedInUserId, bool isCreateNew)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(DBClient.AddParameters("BasicJobID", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("ProfileType", SqlDbType.Int, profileType));
            sqlParameters.Add(DBClient.AddParameters("EleCompanyName", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.CompanyName : null)));
            sqlParameters.Add(DBClient.AddParameters("EleFirstName", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.FirstName : null)));
            sqlParameters.Add(DBClient.AddParameters("EleLastName", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.LastName : null)));
            sqlParameters.Add(DBClient.AddParameters("EleUnitTypeID", SqlDbType.Int, (jobElectricians != null ? jobElectricians.UnitTypeID : null)));
            sqlParameters.Add(DBClient.AddParameters("EleUnitNumber", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.UnitNumber : null)));
            sqlParameters.Add(DBClient.AddParameters("EleStreetNumber", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.StreetNumber : null)));
            sqlParameters.Add(DBClient.AddParameters("EleStreetName ", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.StreetName : null)));
            sqlParameters.Add(DBClient.AddParameters("EleStreetTypeID", SqlDbType.Int, (jobElectricians != null ? jobElectricians.StreetTypeID : null)));
            sqlParameters.Add(DBClient.AddParameters("EleTown", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.Town : null)));
            sqlParameters.Add(DBClient.AddParameters("EleState", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.State : null)));
            sqlParameters.Add(DBClient.AddParameters("ElePostCode", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.PostCode : null)));
            sqlParameters.Add(DBClient.AddParameters("ElePhone", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.Phone : null)));
            sqlParameters.Add(DBClient.AddParameters("EleMobile", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.Mobile : null)));
            sqlParameters.Add(DBClient.AddParameters("EleEmail", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.Email : null)));
            sqlParameters.Add(DBClient.AddParameters("EleLicenseNumber", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.LicenseNumber : null)));
            sqlParameters.Add(DBClient.AddParameters("EleSignature", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.Signature : null)));
            sqlParameters.Add(DBClient.AddParameters("EleIsPostalAddress", SqlDbType.Bit, (jobElectricians != null ? jobElectricians.IsPostalAddress : false)));
            sqlParameters.Add(DBClient.AddParameters("ElePostalAddressID", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.PostalAddressID : 0)));
            sqlParameters.Add(DBClient.AddParameters("ElePostalDeliveryNumber", SqlDbType.NVarChar, (jobElectricians != null ? jobElectricians.PostalDeliveryNumber : null)));
            sqlParameters.Add(DBClient.AddParameters("EleInstallerID", SqlDbType.Int, (jobElectricians != null ? jobElectricians.InstallerID : null)));
            sqlParameters.Add(DBClient.AddParameters("ElectricianID", SqlDbType.Int, (jobElectricians != null ? jobElectricians.ElectricianID : null)));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, LoggedInUserId));//ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, LoggedInUserId));//ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, jobElectricians.SolarCompanyID));
            sqlParameters.Add(DBClient.AddParameters("IsCreateNew", SqlDbType.Bit, isCreateNew));

            object jobElectricianId = CommonDAL.ExecuteScalar("JobElectricians_InsertUpdate", sqlParameters.ToArray());
            return Convert.ToInt32(jobElectricianId);
        }

        /// <summary>
        /// Update JobStatus FromREC
        /// </summary>
        /// <param name="resellerId"></param>
        /// <param name="isSwitch"></param>
        /// <param name="isAutoUpdateCEROn"></param>
        /// <returns></returns>
        public DataTable UpdateJobStatusFromREC(int resellerId, bool isSwitch, bool isAutoUpdateCEROn)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, resellerId));
            sqlParameters.Add(DBClient.AddParameters("IsSwitch", SqlDbType.Bit, isSwitch));
            sqlParameters.Add(DBClient.AddParameters("IsAutoUpdateCEROn", SqlDbType.Bit, isAutoUpdateCEROn));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            DataSet ds = CommonDAL.ExecuteDataSet("UpdateJobStatusAsPerRECbyReseller", sqlParameters.ToArray());
            DataTable dt = ds.Tables[0];
            return dt;
        }

        /// <summary>
        /// Update Details After RECInsertion
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="resellerId"></param>
        /// <returns></returns>
        public DataSet UpdateDetailsAfterRECInsertion(DataTable dt, int resellerId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("tableRECData", SqlDbType.Structured, dt));
            sqlParameters.Add(DBClient.AddParameters("createdDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("rId", SqlDbType.Int, resellerId));
            DataSet ds = CommonDAL.ExecuteDataSet("UpdateDetailsAfterRECInsertion", sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Get UpdateJobStatusFromRec By ResellerId
        /// </summary>
        /// <param name="resellerId"></param>
        /// <returns></returns>
        public bool GetUpdateJobStatusFromRecByResellerId(int resellerId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, resellerId));
            return Convert.ToBoolean(CommonDAL.ExecuteScalar("GetUpdateJobStatusFromRecByResellerId", sqlParameters.ToArray()));
        }

        /// <summary>
        /// Update Job Owner Details
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="jobOwnerDetails"></param>
        public void UpdateJobOwnerDetails(int jobId, JobOwnerDetails jobOwnerDetails, bool isGST)
        {
            string spName = "[UpdateJobOwnerDetails]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("OwnerType", SqlDbType.NVarChar, jobOwnerDetails.OwnerType));
            sqlParameters.Add(DBClient.AddParameters("OwnerCompanyABN", SqlDbType.NVarChar, jobOwnerDetails.CompanyABN));
            sqlParameters.Add(DBClient.AddParameters("OwnerCompanyName", SqlDbType.NVarChar, jobOwnerDetails.CompanyName));
            sqlParameters.Add(DBClient.AddParameters("OwnerFirstName", SqlDbType.NVarChar, jobOwnerDetails.FirstName));
            sqlParameters.Add(DBClient.AddParameters("OwnerLastName", SqlDbType.NVarChar, jobOwnerDetails.LastName));
            sqlParameters.Add(DBClient.AddParameters("OwnerUnitTypeID", SqlDbType.Int, jobOwnerDetails.UnitTypeID));
            sqlParameters.Add(DBClient.AddParameters("OwnerUnitNumber", SqlDbType.NVarChar, jobOwnerDetails.UnitNumber));
            sqlParameters.Add(DBClient.AddParameters("OwnerStreetNumber", SqlDbType.NVarChar, jobOwnerDetails.StreetNumber));
            sqlParameters.Add(DBClient.AddParameters("OwnerStreetName", SqlDbType.NVarChar, jobOwnerDetails.StreetName));
            sqlParameters.Add(DBClient.AddParameters("OwnerStreetTypeID", SqlDbType.NVarChar, jobOwnerDetails.StreetTypeID));
            sqlParameters.Add(DBClient.AddParameters("OwnerTown", SqlDbType.NVarChar, jobOwnerDetails.Town));
            sqlParameters.Add(DBClient.AddParameters("OwnerState", SqlDbType.NVarChar, jobOwnerDetails.State));
            sqlParameters.Add(DBClient.AddParameters("OwnerPostCode", SqlDbType.NVarChar, jobOwnerDetails.PostCode));
            sqlParameters.Add(DBClient.AddParameters("OwnerPhone", SqlDbType.NVarChar, jobOwnerDetails.Phone));
            sqlParameters.Add(DBClient.AddParameters("OwnerMobile", SqlDbType.NVarChar, jobOwnerDetails.Mobile));
            sqlParameters.Add(DBClient.AddParameters("OwnerEmail", SqlDbType.NVarChar, jobOwnerDetails.Email));
            sqlParameters.Add(DBClient.AddParameters("OwnerIsPostalAddress", SqlDbType.Bit, jobOwnerDetails.IsPostalAddress));
            sqlParameters.Add(DBClient.AddParameters("OwnerPostalAddressID", SqlDbType.NVarChar, jobOwnerDetails.PostalAddressID));
            sqlParameters.Add(DBClient.AddParameters("OwnerPostalDeliveryNumber", SqlDbType.NVarChar, jobOwnerDetails.PostalDeliveryNumber));
            sqlParameters.Add(DBClient.AddParameters("IsGST", SqlDbType.Bit, isGST));
            sqlParameters.Add(DBClient.AddParameters("IsOwnerRegisteredWithGST", SqlDbType.Bit, jobOwnerDetails.IsOwnerRegisteredWithGST));
            CommonDAL.Crud(spName, sqlParameters.ToArray());
        }

        /// <summary>
        /// Update Job Installation PropertyType
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="propertyType"></param>
        public void UpdateJobInstallationPropertyType(int jobId, string propertyType, bool isGST)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("PropertyType", SqlDbType.NVarChar, propertyType));
            sqlParameters.Add(DBClient.AddParameters("ISGST", SqlDbType.Bit, isGST));
            CommonDAL.Crud("UpdateJobInstallationPropertyType", sqlParameters.ToArray());
        }

        /// <summary>
        /// Upload GstDocument For Job
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="gstDocument"></param>
        public void UploadGstDocumentForJob(int jobId, string gstDocument)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("GstDocument", SqlDbType.NVarChar, gstDocument));
            CommonDAL.Crud("UploadGstDocumentForJob", sqlParameters.ToArray());
        }

        /// <summary>
        /// Delete GstDocument For Job
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="gstDocument"></param>
        public void DeleteGstDocumentForJob(int jobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            CommonDAL.Crud("DeleteGstDocumentForJob", sqlParameters.ToArray());
        }

        public string CheckSpecialCharInSerialNumbers(string JobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.NVarChar, JobId));
            string jobIds = Convert.ToString(CommonDAL.ExecuteScalar("CheckSpecialCharInSerialNumbers", sqlParameters.ToArray()));
            return jobIds;
        }
        public DataSet Job_InsertBulkUploadSolarJobs(DataTable dtBulkUploadPVDSolarJobs, DataTable dtBulkUploadSWHSolarJobs, int solarCompanyId, string JobScheduleHistoryMsg)
        {
            int colIndex = 1;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("dtBulkUploadPVDSolarJobs", SqlDbType.Structured, dtBulkUploadPVDSolarJobs));
                sqlParameters.Add(DBClient.AddParameters("dtBulkUploadSWHSolarJobs", SqlDbType.Structured, dtBulkUploadSWHSolarJobs));
                sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.Int, solarCompanyId));
                sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
                sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
                sqlParameters.Add(DBClient.AddParameters("JobScheduleHistoryMsg", SqlDbType.NVarChar, JobScheduleHistoryMsg));
                sqlParameters.Add(DBClient.AddParameters("FromEmail", SqlDbType.NVarChar, ProjectSession.MailFrom));
                DataSet dsResult = new DataSet();
                dsResult = CommonDAL.ExecuteDataSet("Job_InsertBulkUploadSolarJobs", sqlParameters.ToArray());
                if (dsResult.Tables.Count > 0)
                {
                    foreach (DataTable dt1 in dsResult.Tables)
                    {
                        if (dt1.Columns.Contains("ErrorMsg"))
                        {
                            ds.Tables.Add(dt1.Copy());
                        }
                        if (dt1.Columns.Contains("CacheJobId"))
                        {
                            ds.Tables.Add(dt1.Copy());
                        }
                    }
                }
                //dt = CommonDAL.ExecuteDataSet("Job_InsertBulkUploadSolarJobs", sqlParameters.ToArray()).Tables[0];
                return ds;
            }
            catch (Exception ex)
            {
                dt.Columns.Add("ErrorMsg", typeof(string));
                DataRow dr = dt.NewRow();
                dr["ErrorMsg"] = dt != null ? ex.Message + " at Row: " + (dt.Rows.Count + 2) + " and Column: " + colIndex : ex.Message;
                dt.Rows.Add(ex.Message);
                ds.Tables.Add(dt);
                return ds;
            }
        }

        public JobSTCDetails JobSTCDetailsBusinessRules(string installingNewPanel, JobSTCDetails jobSTCDetails)
        {
            JobSTCDetails objJobSTCDetails = new JobSTCDetails();

            if (jobSTCDetails != null)
            {
                objJobSTCDetails = jobSTCDetails;

                if (!string.IsNullOrEmpty(installingNewPanel))
                {
                    if (objJobSTCDetails != null)
                    {
                        if (installingNewPanel.ToLower() == "new" || installingNewPanel.ToLower() == "replacing")
                        {
                            objJobSTCDetails.InstallingCompleteUnit = "Yes";
                            if (installingNewPanel.ToLower() == "new")
                            {
                                objJobSTCDetails.AdditionalCapacityNotes = null;
                            }
                        }
                        else
                        {
                            objJobSTCDetails.InstallingCompleteUnit = "No";
                        }
                    }
                }
                else if (objJobSTCDetails != null)
                {
                    objJobSTCDetails.AdditionalCapacityNotes = null;
                }

                if (objJobSTCDetails != null)
                {
                    if (!string.IsNullOrEmpty(objJobSTCDetails.MultipleSGUAddress))
                    {
                        if (objJobSTCDetails.MultipleSGUAddress.ToLower() != "yes")
                        {
                            objJobSTCDetails.Location = null;
                        }
                    }
                    else
                    {
                        objJobSTCDetails.Location = null;
                    }

                    if (objJobSTCDetails.TypeOfConnection == "Stand-alone (not connected to an electricity grid)")
                    {
                        objJobSTCDetails.StandAloneGridSelected = "Yes";
                    }
                    else
                    {
                        objJobSTCDetails.StandAloneGridSelected = "No";
                    }
                }
            }
            objJobSTCDetails.CECAccreditationStatement = "Yes";
            objJobSTCDetails.GovernmentSitingApproval = "Yes";
            objJobSTCDetails.ElectricalSafetyDocumentation = "Yes";
            objJobSTCDetails.AustralianNewZealandStandardStatement = "Yes";

            return objJobSTCDetails;
        }


        /// <summary>
        /// Get the required data for trade the select job  
        /// </summary>
        /// <param name="lstJobs"></param>
        /// <returns></returns>
        public List<JobList> GetDataForTradeJob(List<int> lstJobs)
        {
            List<JobList> lstJobList = new List<JobList>();
            if (lstJobs != null && lstJobs.Count > 0)
            {
                DataSet dsUsers = new DataSet();
                var iDs = lstJobs.Select(i => i.ToString(CultureInfo.InvariantCulture)).Aggregate((s1, s2) => s1 + ", " + s2);
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("JobIDs", SqlDbType.NVarChar, iDs));
                sqlParameters.Add(DBClient.AddParameters("CurrentDateTime", SqlDbType.DateTime, DateTime.Now));
                lstJobList = CommonDAL.ExecuteProcedure<JobList>("Jobs_GetDataForTradeJob", sqlParameters.ToArray()).ToList();
            }
            return lstJobList;
        }

        public DataSet GetJobList_ForCachingDataKendo(string SolarCompanyId, int UserId, int JobViewMenuId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.NVarChar, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("JobViewMenuId", SqlDbType.Int, JobViewMenuId));
            sqlParameters.Add(DBClient.AddParameters("CurrentDateTime", SqlDbType.DateTime, DateTime.Now));
            DataSet dsJobsPlusColumns = CommonDAL.ExecuteDataSet("Job_GetJobList_ForCachingData_Kendo", sqlParameters.ToArray());
            return dsJobsPlusColumns;
        }

        public List<string> GetStaffNameFromResellerOrRam(int ResellerId, int UserTypeId, string isAllScaJobView)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            var listStaffName = new List<string>();
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("RAMId", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("IsAllScaJobView", SqlDbType.Bit, string.IsNullOrEmpty(isAllScaJobView) ? false : Convert.ToBoolean(isAllScaJobView)));
            DataSet ds = CommonDAL.ExecuteDataSet("GetStaffNameFromReseller", sqlParameters.ToArray());
            listStaffName = ds.Tables[0].AsEnumerable().Select(x => x.Field<string>("StaffName")).ToList();
            return listStaffName;
        }

        /// <summary>
        /// Get StcJobDetailsId List ForCachingData
        /// </summary>
        /// <param name="ResellerId"></param>
        /// <param name="SolarCompanyId"></param>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public DataSet GetStcJobDetailsIdList_ForCachingData(int ResellerId, int SolarCompanyId, int jobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            DataSet ds = CommonDAL.ExecuteDataSet("GetStcJobDetailsIdList_ForCachingData", sqlParameters.ToArray());
            return ds;
        }
        public DataSet JobDocument_GetJobDocumentByDocuementPathJobId(int JobId, string JobDocumentPath)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("JobDocumentPath", SqlDbType.NVarChar, JobDocumentPath));
            DataSet ds = CommonDAL.ExecuteDataSet("JobDocument_GetJobDocumentByDocuementPathJobId", sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Load Common Jobs With Same Installation Date And Installer
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="installerId"></param>
        /// <param name="installationDate"></param>
        /// <returns>list of jobs</returns>
        //public List<CommonJobsWithSameInstallDateAndInstaller> LoadCommonJobs_SameInstallDateAndInstaller(int jobId, int installerId, string installationDate)
        //{
        //    List<SqlParameter> sqlParameters = new List<SqlParameter>();
        //    sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
        //    sqlParameters.Add(DBClient.AddParameters("InstallerId", SqlDbType.Int, installerId));
        //    sqlParameters.Add(DBClient.AddParameters("InstallerDate", SqlDbType.DateTime, installationDate));
        //    DataSet dataSet = CommonDAL.ExecuteDataSet("LoadCommonJobs_SameInstallDateAndInstaller", sqlParameters.ToArray());
        //    List<CommonJobsWithSameInstallDateAndInstaller> lstJobs = dataSet.Tables[0].Rows.Count > 0 ? dataSet.Tables[0].ToListof<CommonJobsWithSameInstallDateAndInstaller>() : new List<CommonJobsWithSameInstallDateAndInstaller>();

        //    return lstJobs;
        //}

        public DataSet LoadCommonJobs_SameInstallDateAndInstaller(int jobId, int installerId, string installationDate)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("InstallerId", SqlDbType.Int, installerId));
            sqlParameters.Add(DBClient.AddParameters("InstallerDate", SqlDbType.DateTime, !string.IsNullOrEmpty(installationDate) ? Convert.ToDateTime(installationDate) : (object)DBNull.Value));
            DataSet dataSet = CommonDAL.ExecuteDataSet("LoadCommonJobs_SameInstallDateAndInstaller", sqlParameters.ToArray());
            //List<CommonJobsWithSameInstallDateAndInstaller> lstJobs = dataSet.Tables[0].Rows.Count > 0 ? dataSet.Tables[0].ToListof<CommonJobsWithSameInstallDateAndInstaller>() : new List<CommonJobsWithSameInstallDateAndInstaller>();

            return dataSet;
        }
        /// <summary>
        /// Load Common Jobs With Same Installation Address
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="usertypeid"></param>
        /// <returns>list of jobs</returns>
        public DataSet CommonJobsWithSameInstallationAddress(int jobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, ProjectSession.UserTypeId));
            DataSet dataSet = CommonDAL.ExecuteDataSet("LoadCommonJobs_SameInstallationAddress", sqlParameters.ToArray());

            return dataSet;
        }
        /// <summary>
        /// Get AllCompany To Make Registered With GST From ResellerId
        /// </summary>
        /// <returns>list of string</returns>
        public List<SolarCompany> GetAllCompanyToMakeRegisteredWithGSTFromResellerId(int resellerId)
        {
            string spName = "[GetAllCompanyToMakeRegisteredWithGSTFromResellerId]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, resellerId));
            IList<SolarCompany> solarCompanyList = CommonDAL.ExecuteProcedure<SolarCompany>(spName, sqlParameters.ToArray());
            return solarCompanyList.ToList();
        }


        public string UploadPVDCode(DataTable dt)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("BulkUploadPVDCodeData", SqlDbType.Structured, dt));
            return Convert.ToString(CommonDAL.ExecuteScalar("BulkUploadPVDCodeDataUpdate", sqlParameters.ToArray()));
        }

        public DataSet GetJobsForRecInsertOrUpdate(string JobIds)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobIds", SqlDbType.NVarChar, JobIds));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            return CommonDAL.ExecuteDataSet("GetJobsForRecInsertOrUpdate", sqlParameters.ToArray());
        }

        public DataSet GetJobsForRecInsertOrUpdateNew(string JobIds)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobIds", SqlDbType.NVarChar, JobIds));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            return CommonDAL.ExecuteDataSet("GetJobsForRecInsertOrUpdateNew", sqlParameters.ToArray());
        }

        public DataSet GetJobsForRecSubmission()
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            return CommonDAL.ExecuteDataSet("GETRECDataForSubmission", sqlParameters.ToArray());
        }

        public DataSet InsertGBBatchRECUploadId(DataTable dt, int resellerId, string dateTimeTicks = "", decimal TotalSTC = 0, int CreatedBy = 0, string CERLoginId = "", string CERPassword = "", string RECAccName = "", string LoginType = "", string RECCompanyName = "")
        {
            string spName = "[InsertGBBatchRECUploadId]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("dtJobIds", SqlDbType.Structured, dt));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, resellerId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("datetimeticks", SqlDbType.NVarChar, dateTimeTicks));
            sqlParameters.Add(DBClient.AddParameters("TotalSTC", SqlDbType.Decimal, TotalSTC));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, CreatedBy));
            if (!string.IsNullOrWhiteSpace(CERLoginId))
                sqlParameters.Add(DBClient.AddParameters("CERLoginId", SqlDbType.NVarChar, CERLoginId));
            if (!string.IsNullOrWhiteSpace(CERPassword))
                sqlParameters.Add(DBClient.AddParameters("CERPassword", SqlDbType.NVarChar, CERPassword));
            if (!string.IsNullOrWhiteSpace(RECAccName))
                sqlParameters.Add(DBClient.AddParameters("RECAccName", SqlDbType.NVarChar, RECAccName));
            if (!string.IsNullOrWhiteSpace(RECCompanyName))
                sqlParameters.Add(DBClient.AddParameters("RECCompanyName", SqlDbType.NVarChar, RECCompanyName));
            if (!string.IsNullOrWhiteSpace(LoginType))
                sqlParameters.Add(DBClient.AddParameters("LoginType", SqlDbType.NVarChar, LoginType));
            return CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
        }


        public void RemoveJobFromBatch(string StcJobdetailsId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("StcJobdetailsId", SqlDbType.NVarChar, StcJobdetailsId));
            CommonDAL.ExecuteScalar("RemoveJobFromBatch", sqlParameters.ToArray());
        }

        /// <summary>
        /// Get All the job serial numbers and verification url for spv
        /// </summary>
        /// <param name="JobPanelId"></param>
        /// <returns></returns>
        public DataSet GetSPVVerificationUrlSerialNumber(int JobId = 0, bool reProductVerification = false)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("reProductVerification", SqlDbType.Bit, reProductVerification));
            return CommonDAL.ExecuteDataSet("GetSPVVerificationUrlSerialNumber", sqlParameters.ToArray());
        }


        /// <summary>
        /// Get All the job serial numbers and verification url for spv
        /// </summary>
        /// <param name="JobPanelId"></param>
        /// <returns></returns>
        public DataSet GetSPVVerificationUrlSerialNumberAPI(string Manufacturer, string ModelNumber, string commaSerialnumber, int JobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CommaSerialnumber", SqlDbType.VarChar, commaSerialnumber));
            sqlParameters.Add(DBClient.AddParameters("Manufacturer", SqlDbType.VarChar, Manufacturer));
            sqlParameters.Add(DBClient.AddParameters("ModelNumber", SqlDbType.VarChar, ModelNumber));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            return CommonDAL.ExecuteDataSet("GetSPVVerificationUrlSerialNumberAPI", sqlParameters.ToArray());
        }


        /// <summary>
        /// Get all the data for installation verification
        /// </summary>
        /// <param name="STCJobDetailsId"></param>
        /// <returns></returns>
        public DataSet GetSPVInstallationVerificationUrlSerialNumber(int STCJobDetailsId, bool ReVerify = false)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCJobDetailsId", SqlDbType.Int, STCJobDetailsId));
            sqlParameters.Add(DBClient.AddParameters("ReVerify", SqlDbType.Bit, ReVerify));
            return CommonDAL.ExecuteDataSet("GetSPVInstallationVerificationUrlSerialNumber", sqlParameters.ToArray());
        }

        /// <summary>
        /// Update serialnumber is verified or not in product verification SPV
        /// </summary>
        /// <param name="VerifiedSerialNumber"></param>
        /// <param name="JobId"></param>
        public List<JobSerialNumbers> UpdateVerifiedSerialNumber(DataTable VerifiedSerialNumber, int JobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VerifiedSerialNumber", SqlDbType.Structured, VerifiedSerialNumber));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            return CommonDAL.ExecuteProcedure<JobSerialNumbers>("UpdateVerifiedSerialNumber", sqlParameters.ToArray()).ToList();
        }

        /// <summary>
        /// Update Installtion verification failed or pass in stc submission
        /// </summary>
        /// <param name="StcJobDetailsId"></param>
        /// <param name="Status"></param>
        public void UpdateInstallationVerificationStatus(int StcJobDetailsId, bool? Status)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("StcJobDetailsId", SqlDbType.Int, StcJobDetailsId));
            sqlParameters.Add(DBClient.AddParameters("IsInstallationVerificationPass", SqlDbType.Bit, Status));
            CommonDAL.ExecuteScalar("UpdateInstallationVerificationStatus", sqlParameters.ToArray());
        }


        public DataSet GetJobPanelDetails(int JobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            return CommonDAL.ExecuteDataSet("Get_JobPanelDetailsForAPI", sqlParameters.ToArray());

        }
        public DataSet GetBulkJobPanelDetails(string JobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.NVarChar, JobId));
            return CommonDAL.ExecuteDataSet("Get_BulkJobPanelDetailsForAPI", sqlParameters.ToArray());
        }
        public void UpdateJobSerialNumber(int Jobid, string SerialNumber)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, Jobid));
            sqlParameters.Add(DBClient.AddParameters("SerialNumber", SqlDbType.VarChar, SerialNumber));
            CommonDAL.ExecuteScalar("UpdateJobSerialNumber", sqlParameters.ToArray());
        }

        /// <summary>
        /// Get job basic detail for send email on installation verifid
        /// </summary>
        /// <param name="JobId"></param>
        /// <returns></returns>
        public JobBasicDetail GetJobBasicDetail(int JobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            JobBasicDetail lstCustomDetail = CommonDAL.ExecuteProcedure<JobBasicDetail>("GetJob_BasicDetail_ByJobId", sqlParameters.ToArray()).FirstOrDefault();
            return lstCustomDetail;

        }


        /// <summary>
        /// Release Spv
        /// </summary>
        /// <param name="JobId"></param>
        public void SpvRelease(int JobID, int STCJobDetailsID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("jobID", SqlDbType.Int, JobID));
            sqlParameters.Add(DBClient.AddParameters("STCJobDetailsID", SqlDbType.Int, STCJobDetailsID));
            CommonDAL.ExecuteScalar("SpvRelease", sqlParameters.ToArray());
        }
        public DataSet GetDocumentsForGeneratingRecZip(int jobId)
        {
            string spName = "[GetDocumentsForGeneratingRecZip]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            return CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
        }

        public DataSet InsertDelete_RecZip_Document(int jobId, int userId, string path, bool isGenerateRecZip)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("Path", SqlDbType.NVarChar, path));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, userId));
            sqlParameters.Add(DBClient.AddParameters("Date", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("IsGenerateRecZip", SqlDbType.Bit, isGenerateRecZip));
            return CommonDAL.ExecuteDataSet("InsertDelete_RecZip_Document", sqlParameters.ToArray());
        }

        /// <summary>
        /// Reset Spv
        /// </summary>
        /// <param name="JobId"></param>
        public void SpvReset(int JobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("jobID", SqlDbType.Int, JobID));
            CommonDAL.ExecuteScalar("SpvReset", sqlParameters.ToArray());
        }
        public CheckSPVRequiredByJobId CheckSPVRequiredByJobId(int JobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("jobID", SqlDbType.Int, JobID));
            CheckSPVRequiredByJobId lstcheckSPVRequiredByJobId = CommonDAL.ExecuteProcedure<CheckSPVRequiredByJobId>("CheckSPVRequiredByJobId", sqlParameters.ToArray()).FirstOrDefault(); ;
            return lstcheckSPVRequiredByJobId;
        }
        //public List<SpvRequiredSolarCompanyWise> GetSPVRequiredOrNotOnSCOrGlobalLevel(string SolarCompanyIds = null, string JobIds = null, string StcJobIds = null)
        //{
        //    List<SqlParameter> sqlParameters = new List<SqlParameter>();
        //    sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.NVarChar, SolarCompanyIds));
        //    sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.NVarChar, JobIds));
        //    sqlParameters.Add(DBClient.AddParameters("StcJobIds", SqlDbType.NVarChar, StcJobIds));
        //    List<SpvRequiredSolarCompanyWise> lstSpvRequiredSolarCompanyWise = CommonDAL.ExecuteProcedure<SpvRequiredSolarCompanyWise>("GetSPVRequiredOrNotOnSCOrGlobalLevel", sqlParameters.ToArray()).ToList();
        //    return lstSpvRequiredSolarCompanyWise;
        //}
        public List<CheckSPVrequired> GetSPVRequiredOrNotOnSCAOrGlobalLevelOrManufacturer(string JobIds = null, string Manufacturer = null, string Model = null)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobIds", SqlDbType.NVarChar, JobIds));
            sqlParameters.Add(DBClient.AddParameters("Manufacturer", SqlDbType.VarChar, Manufacturer));
            sqlParameters.Add(DBClient.AddParameters("Model", SqlDbType.VarChar, Model));
            List<CheckSPVrequired> lstCheckSPVrequired = CommonDAL.ExecuteProcedure<CheckSPVrequired>("GetSPVRequiredOrNotOnSCAOrGlobalLevelOrManufacturer", sqlParameters.ToArray()).ToList();
            return lstCheckSPVrequired;
        }
        /// <summary>
        /// Update IsSpvVerified flag in job when lat-long getting null from app for upload job photo
        /// </summary>
        /// <param name="JobId"></param>
        /// <returns></returns>
        public DataSet UpdateIsSpvVerifiedWhenLatLongNullForUploadPhoto(int JobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("jobID", SqlDbType.Int, JobId));
            DataSet ds = CommonDAL.ExecuteDataSet("UpdateIsSpvVerifiedWhenLatLongNullForUploadPhoto", sqlParameters.ToArray());
            return ds;
        }
        public List<GetPhotoFromPortalWithNullLatLongIsdeletedFlagApiRequest> GetPhotoFromPortalWithNullLatLongIsdeletedFlag(string VisitChecklistPhotoIds)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VisitChecklistPhotoIds", SqlDbType.NVarChar, VisitChecklistPhotoIds));
            List<GetPhotoFromPortalWithNullLatLongIsdeletedFlagApiRequest> lstvisitPhotoids = CommonDAL.ExecuteProcedure<GetPhotoFromPortalWithNullLatLongIsdeletedFlagApiRequest>("GetPhotoFromPortalWithNullLatLongIsdeletedFlag", sqlParameters.ToArray()).ToList();
            return lstvisitPhotoids;
        }
        public List<SpvLog> GetSPVProductVerificationLogByJobId(int jobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            List<SpvLog> SpvLog = CommonDAL.ExecuteProcedure<SpvLog>("GetSPVProductVerificationLogByJobId", sqlParameters.ToArray()).ToList(); ;
            return SpvLog;
        }

        /// <summary>
        /// Set SPV on lock serialnumber
        /// </summary>
        /// <param name="jobId"></param>
        public bool SetSPVOnLockSerialNumber(int jobId)
        {
            bool isSPVRequired = false;
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("jobId", SqlDbType.Int, jobId));
            DataSet dataSet = CommonDAL.ExecuteDataSet("SetSPVOnLockSerialNumber", sqlParameters.ToArray());
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0] != null && dataSet.Tables[0].Rows.Count > 0)
            {
                isSPVRequired = Convert.ToBoolean(dataSet.Tables[0].Rows[0]["IsSPVRequired"]);
            }
            return isSPVRequired;
        }

        /// <summary>
        /// Remove SPV on unlock serialnumber
        /// </summary>
        /// <param name="jobId"></param>
        public void RemoveSPVOnUnlockSerialnumber(int jobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("jobId", SqlDbType.Int, jobId));
            CommonDAL.ExecuteScalar("RemoveSPVOnUnlockSerialnumber", sqlParameters.ToArray());
        }
        public bool CheckDocumentRightsFromJobStcStatus(int jobId)
        {
            bool isAllowForDocumentOperaion = false;
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            DataSet dataSet = CommonDAL.ExecuteDataSet("CheckDocumentRightsFromJobStcStatus", sqlParameters.ToArray());
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0] != null && dataSet.Tables[0].Rows.Count > 0)
            {
                isAllowForDocumentOperaion = Convert.ToBoolean(dataSet.Tables[0].Rows[0]["IsAllowForDocumentOperaion"]);
            }
            return isAllowForDocumentOperaion;
        }
        /// <summary>
        /// get serial number with latest spv verify status
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public List<JobSerialNumbers> GetSerialnumberWithSpvStatus(int JobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            //sqlParameters.Add(DBClient.AddParameters("VerifiedSerialNumber", SqlDbType.Structured, VerifiedSerialNumber));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            return CommonDAL.ExecuteProcedure<JobSerialNumbers>("GetSerialnumberWithSpvStatus", sqlParameters.ToArray()).ToList();
        }
        /// <summary>
        /// get reseller id solar company id from jobid or stcjobdetailid
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="stcJobDetailId"></param>
        public DataTable GetResellerSolarCompnayFromJobIdOrStcJobdetailId(int? jobid, int? stcJobDetailId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("jobid", SqlDbType.Int, jobid));
            sqlParameters.Add(DBClient.AddParameters("stcJobdetailId", SqlDbType.Int, stcJobDetailId));
            DataTable dt = CommonDAL.ExecuteDataSet("GetResellerSolarCompnayFromJobIdOrStcJobdetailId", sqlParameters.ToArray()).Tables[0];
            return dt;
        }
        /// <summary>
        /// get User name from user id
        /// </summary>
        /// <param name="userId"></param>
        public DataSet GetUserNameSolarCompanyForCache(int userId, int usertypeId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, userId));
            sqlParameters.Add(DBClient.AddParameters("userTypeId", SqlDbType.Int, usertypeId));
            DataSet ds = CommonDAL.ExecuteDataSet("GetUserNameSolarCompanyForCache", sqlParameters.ToArray());
            return ds;
        }
        /// <summary>
        /// get invoice count from stcjobdetailsids
        /// </summary>
        /// <param name="stcJobdetailId"></param>
        public DataTable GetInvoiceCount(string stcJobdetailIds)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("stcjobdetailIds", SqlDbType.VarChar, stcJobdetailIds));
            DataTable dt = CommonDAL.ExecuteDataSet("GetInvoiceCount", sqlParameters.ToArray()).Tables[0];
            return dt;
        }
        /// <summary>
        /// get STCJObdetailData and job data 
        /// </summary>
        /// <param name="stcJobdetailIds"></param>
        /// <param name="jobids"></param>
        public DataTable GetSTCDetailsAndJobDataForCache(string stcJobdetailIds, string jobIds)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("stcJobdetailIds", SqlDbType.VarChar, stcJobdetailIds));
            sqlParameters.Add(DBClient.AddParameters("jobIds", SqlDbType.VarChar, jobIds));
            DataTable dt = CommonDAL.ExecuteDataSet("GetSTCDetailsAndJobDataForCache", sqlParameters.ToArray()).Tables[0];
            return dt;
        }
        /// <summary>
        /// get installation address  
        /// </summary>
        /// <param name="jobid"></param>
        public DataTable GetInstallationAddressForCache(int jobid)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobid));
            DataTable dt = CommonDAL.ExecuteDataSet("GetInstallationAddressForCache", sqlParameters.ToArray()).Tables[0];
            return dt;
        }
        /// <summary>
        /// get ram solar company mapping detail  
        /// </summary>
        /// <param name="ramId"></param>
        public DataTable GetRAMSolarCompanyMappingForCache(int ramId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RamId", SqlDbType.Int, ramId));
            DataTable dt = CommonDAL.ExecuteDataSet("GetRAMSolarCompanyMappingForCache", sqlParameters.ToArray()).Tables[0];
            return dt;
        }
        /// <summary>
        /// Get job data fro template
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public EmailInfo GetJobDataForTemplate(int jobId)
        {
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("jobId", SqlDbType.Int, jobId));
                EmailInfo jobData = CommonDAL.SelectObject<EmailInfo>("GetJobDataForTemplate", sqlParameters.ToArray());
                return jobData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// Get Installation Verification Status
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>       
        public DataSet GetInstallationVerificationStatus(int JobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            DataSet ds = CommonDAL.ExecuteDataSet("GetInstallationVerificationStatus", sqlParameters.ToArray());
            return ds;
        }
        /// <summary>
        /// Get Reseller name and solar company by job Id
        /// </summary>
        /// <param name="JobID"></param>
        /// <returns>Resturns Reseller name and solar company name </returns>
        public BasicDetails GetResellerSolarCompany(int JobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobID));
            BasicDetails jobData = CommonDAL.SelectObject<BasicDetails>("GetResellerSolarCompanyByJobId", sqlParameters.ToArray());
            return jobData;
        }
        /// <summary>
        /// ignore 200 mtr validation rule in photos section
        /// </summary>
        /// <param name="JobID"></param>
        public DataSet SPVIgnore200mtrValidation(int jobid)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobid));
            DataSet ds = CommonDAL.ExecuteDataSet("Ignore200mtrValidation", sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Get JobFullpack by job id
        /// </summary>
        /// <param name="JobId"></param>
        /// <returns>Dataset </returns>
        public DataSet GetFullJobPack(int JobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobId));
            DataSet ds = CommonDAL.ExecuteDataSet("GetFullJobPack", sqlParameters.ToArray());
            return ds;
        }
        /// <summary>
        /// installation address validation flag change
        /// </summary>
        /// <param name="isValid"></param>
        public void installationAddValidChange(bool isValid, int JobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("Isvalid", SqlDbType.Bit, isValid));
            CommonDAL.Crud("InstallationAddValidationChange", sqlParameters.ToArray());

        }
        /// <summary>
        /// owner address validation flag change
        /// </summary>
        /// <param name="isValid"></param>
        public void OwnerAddValidChange(bool isValid, int JobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("Isvalid", SqlDbType.Bit, isValid));
            CommonDAL.Crud("OwnerAddValidationChange", sqlParameters.ToArray());

        }
        /// <summary>
        /// Restore deleted visit photo 
        /// </summary>
        /// <param name="vclpID"></param>
        /// <param name="vciId"></param>
        /// <param name="jobscId"></param>
        /// <param name="path"></param>
        /// <param name="isReference"></param>
        /// <param name="isDefault"></param>
        /// <param name="type"></param>
        public void RestoreDataWithAllFolder(int vclpID, int? vciId, int? jobscId, string path, bool isReference, bool isDefault, int type)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("visitchecklistphotoId", SqlDbType.Int, vclpID));
            sqlParameters.Add(DBClient.AddParameters("vclId", SqlDbType.Int, vciId));
            sqlParameters.Add(DBClient.AddParameters("jobScId", SqlDbType.Int, jobscId));
            sqlParameters.Add(DBClient.AddParameters("type", SqlDbType.Int, type));
            sqlParameters.Add(DBClient.AddParameters("path", SqlDbType.NVarChar, path));
            sqlParameters.Add(DBClient.AddParameters("isReference", SqlDbType.Bit, isReference));
            sqlParameters.Add(DBClient.AddParameters("isDefault", SqlDbType.Bit, isDefault));
            CommonDAL.Crud("RestoreDataWithAllFolder", sqlParameters.ToArray());
        }
        /// <summary>
        /// get serial number photos and serial number from jobid 
        /// </summary>
        /// <param name="jobid"></param>
        public DataSet GetSerialNumberAndPhotos(int jobid)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobid));
            DataSet ds = CommonDAL.ExecuteDataSet("GetSerialNumberPhotosAndSerialNumber", sqlParameters.ToArray());
            return ds;
        }
        /// <summary>
        /// get rec status from jobids
        /// </summary>
        /// <param name="JobIDs"></param>
        /// <returns></returns>
        public DataSet CheckStatusOfSubmissionInRec(string JobIDs)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobIds", SqlDbType.NVarChar, JobIDs));
            DataSet ds = CommonDAL.ExecuteDataSet("CheckStatusOfSubmissionInRec", sqlParameters.ToArray());
            return ds;
        }

        public DataSet GetDataForXMLVerification(int StreetTypeID, string ModelNo, int JobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobID));
            sqlParameters.Add(DBClient.AddParameters("StreetTypeID", SqlDbType.Int, StreetTypeID));
            sqlParameters.Add(DBClient.AddParameters("ModelNo", SqlDbType.NVarChar, ModelNo));
            DataSet ds = CommonDAL.ExecuteDataSet("GetDataForXMLVerification", sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Remove spv when XMl verification faild
        /// </summary>
        /// <param name="JobID"></param>
        public void RemoveSPVByXMlVerification(int JobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobID));
            CommonDAL.Crud("RemoveSPVByXMlVerification", sqlParameters.ToArray());
        }
        /// <summary>
        /// update spv XMl verification flag
        /// </summary>
        /// <param name="JobID"></param>
        public void UpdateIsSPVXmlVerificationFlag(int JobID, bool isSPVValidXML)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobID));
            sqlParameters.Add(DBClient.AddParameters("IsValidSPVXml", SqlDbType.Bit, isSPVValidXML));
            CommonDAL.Crud("UpdateIsSPVXmlVerificationFlag", sqlParameters.ToArray());
        }
        /// <summary>
        /// get spv XMl verification flag
        /// </summary>
        /// <param name="JobID"></param>
        public DataTable GetIsSPVXMlVerificationFlag(int JobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobID));
            DataTable dt = CommonDAL.ExecuteDataSet("GetIsSPVXMlVerificationFlag", sqlParameters.ToArray()).Tables[0];
            return dt;
        }
        /// <summary>
        /// get serial number by jobid 
        /// </summary>
        /// <param name="JobID"></param>
        /// <returns></returns>
        public DataSet GetSerialNobyJobID(int JobID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobID));
            DataSet ds = CommonDAL.ExecuteDataSet("GetSerialNumberByJobID", sqlParameters.ToArray());
            return ds;
        }
        public decimal GetPostCodeZoneRating(int postCode)
        {
            string spName = "[GetPostCodeZoneRating]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("postCode", SqlDbType.Int, postCode));
            object rating = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            return Convert.ToDecimal(rating);
        }
        public DataSet GetUserList(string name, int UserId, int UserTypeId, int JobId)
        {

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Name", SqlDbType.VarChar, name));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            DataSet ds = CommonDAL.ExecuteDataSet("GetUserNameNew", sqlParameters.ToArray());
            return ds;
        }

        public InstallerDesignerView GetInstallerDesignerEntity(DataTable dtInstaller, bool isInstaller)
        {
            InstallerDesignerView installerDesignerView = new InstallerDesignerView();
            try
            {
                if (dtInstaller.Rows.Count > 0)
                {
                    installerDesignerView = dtInstaller.AsEnumerable().Select(m => new InstallerDesignerView
                    {
                        InstallerDesignerId = m.Field<int>("InstallerDesignerId")
                        ,
                        FirstName = m.Field<string>("FirstName")
                        ,
                        LastName = m.Field<string>("LastName")
                        ,
                        UnitTypeID = m.Field<byte?>("UnitTypeID") == null ? 0 : m.Field<byte>("UnitTypeID")
                        ,
                        UnitNumber = m.Field<string>("UnitNumber")
                        ,
                        StreetNumber = m.Field<string>("StreetNumber")
                        ,
                        StreetName = m.Field<string>("StreetName")
                        ,
                        StreetTypeID = m.Field<byte?>("StreetTypeID") == null ? 0 : m.Field<byte>("StreetTypeID")
                        ,
                        Town = m.Field<string>("Town")
                        ,
                        State = m.Field<string>("State")
                        ,
                        PostCode = m.Field<string>("PostCode")
                        ,
                        CECAccreditationNumber = m.Field<string>("CECAccreditationNumber")
                        ,
                        CECDesignerNumber = m.Field<string>("CECDesignerNumber")
                        ,
                        CreatedBy = m.Field<int>("CreatedBy")
                        ,
                        ModifiedBy = m.Field<int?>("ModifiedBy") == null ? 0 : m.Field<int>("ModifiedBy")
                        ,
                        IsDeleted = m.Field<bool>("IsDeleted")
                        ,
                        SolarCompanyId = m.Field<int>("SolarCompanyId")
                        ,
                        IsPostalAddress = m.Field<bool>("IsPostalAddress")
                        ,
                        PostalAddressID = m.Field<int?>("PostalAddressID") == null ? 0 : m.Field<int>("PostalAddressID")
                        ,
                        PostalDeliveryNumber = m.Field<string>("PostalDeliveryNumber")
                        ,
                        Email = m.Field<string>("Email")
                        ,
                        SERole = m.Field<byte?>("SERole") == null ? 0 : m.Field<byte>("SERole")
                        ,
                        ElectricalContractorsLicenseNumber = m.Field<string>("ElectricalContractorsLicenseNumber")
                        ,
                        SESignature = m.Field<string>("SESignature")
                        ,
                        Phone = m.Field<string>("Phone")
                        ,
                        Mobile = m.Field<string>("Mobile")
                        ,
                        Longitude = m.Field<string>("Longitude")
                        ,
                        Latitude = m.Field<string>("Latitude")
                        ,
                        Location = m.Field<string>("Location")
                        ,
                        IpAddress = m.Field<string>("IpAddress")
                        ,
                        SignatureDate = m.Field<DateTime?>("SignatureDate") == null ? DateTime.Now : m.Field<DateTime>("SignatureDate")
                        ,
                        IsSWHUser = m.Field<bool?>("IsSWHUser") == null ? false : m.Field<bool>("IsSWHUser")
                        ,
                        IsPVDUser = m.Field<bool?>("IsPVDUser") == null ? false : m.Field<bool>("IsPVDUser")
                        ,
                        IsSystemUser = m.Field<bool?>("IsSystemUser") == null ? false : m.Field<bool>("IsSystemUser")
                        ,
                        IsVisitScheduled = (isInstaller == false || m.Field<bool?>("IsVisitScheduled") == null) ? false : m.Field<bool>("IsVisitScheduled")
                        ,
                        SEStatus = (isInstaller == false || m.Field<byte?>("SEStatus") == null) ? 0 : m.Field<byte>("SEStatus")
                        ,
                        GridType = m.Field<string>("GridType")
                        ,
                        SPS = m.Field<string>("SPS")
                    }).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                Helper.Log.WriteError(ex, "Error for InstallerDesignerId : " + dtInstaller.Rows[0]["InstallerDesignerId"].ToString() + " " + DateTime.Now.ToString("dd/MM/yyyy") + " " + ex.Message);
                installerDesignerView = dtInstaller.Rows.Count > 0 ? DBClient.DataTableToList<InstallerDesignerView>(dtInstaller)[0] : new InstallerDesignerView();
            }
            return installerDesignerView;
        }

        public List<UserDocument> GetUserDocumentEntity(DataTable dtDocument)
        {
            List<UserDocument> userDocument = new List<UserDocument>();
            try
            {
                if (dtDocument.Rows.Count > 0)
                {
                    userDocument = dtDocument.AsEnumerable().Select(m => new UserDocument
                    {
                        DocumentPath = m.Field<string>("DocumentPath")
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                Helper.Log.WriteError(ex, "Error for GetUserDocumentEntity JobId : " + dtDocument.Rows[0]["JobID"].ToString() + " " + DateTime.Now.ToString("dd/MM/yyyy") + " " + ex.Message);
                userDocument = CommonDAL.DataTableToList<UserDocument>(dtDocument);
            }
            return userDocument;
        }

        public List<JobScheduling> JobSchedulingEntity(DataTable dtJobScheduling)
        {
            List<JobScheduling> jobSchedulings = new List<JobScheduling>();
            try
            {
                if (dtJobScheduling.Rows.Count > 0)
                {
                    jobSchedulings = dtJobScheduling.AsEnumerable().Select(m => new JobScheduling
                    {
                        VisitNum = m.Field<Int64>("VisitNum")
                        ,
                        strVisitStartDate = m.Field<string>("strVisitStartDate")
                        ,
                        FirstName = m.Field<string>("FirstName")
                        ,
                        LastName = m.Field<string>("LastName")
                        ,
                        TotalItemCount = m.Field<int>("TotalItemCount")
                        ,
                        TotalCompletedItemCount = m.Field<int>("TotalCompletedItemCount")
                        ,
                        JobSchedulingID = m.Field<int>("JobSchedulingID")
                        ,
                        JobID = m.Field<int>("JobID")
                        ,
                        UserId = m.Field<int>("UserId")
                        ,
                        Label = m.Field<string>("Label")
                        ,
                        Detail = m.Field<string>("Detail")
                        ,
                        StartDate = m.Field<DateTime>("StartDate")
                        ,
                        StartTime = m.Field<TimeSpan>("StartTime")
                        ,
                        CreatedDate = m.Field<DateTime>("CreatedDate")
                        ,
                        CreatedBy = m.Field<int>("CreatedBy")
                        ,
                        ModifiedDate = m.Field<DateTime?>("ModifiedDate") == null ? DateTime.Now : m.Field<DateTime>("ModifiedDate")
                        ,
                        ModifiedBy = m.Field<int?>("ModifiedBy") == null ? 0 : m.Field<int>("ModifiedBy")
                        ,
                        IsDeleted = m.Field<bool?>("IsDeleted") == null ? false : m.Field<bool>("IsDeleted")
                        ,
                        Status = m.Field<byte>("Status")
                        ,
                        SolarCompanyId = m.Field<int>("SolarCompanyId")
                        ,
                        VisitStatus = m.Field<int>("VisitStatus")
                        ,
                        CompletedDate = m.Field<DateTime?>("CompletedDate") == null ? DateTime.Now : m.Field<DateTime>("CompletedDate")
                        ,
                        IsDefaultSubmission = m.Field<bool?>("IsDefaultSubmission") == null ? false : m.Field<bool>("IsDefaultSubmission")
                        ,
                        VisitUniqueId = m.Field<string>("VisitUniqueId")
                    }).ToList();

                }
            }
            catch (Exception ex)
            {
                Helper.Log.WriteError(ex, "Error for JobSchedulingEntity JobId : " + dtJobScheduling.Rows[0]["JobID"].ToString() + " " + DateTime.Now.ToString("dd/MM/yyyy") + " " + ex.Message);
                jobSchedulings = CommonDAL.DataTableToList<JobScheduling>(dtJobScheduling);
            }
            return jobSchedulings;
        }

        public JobInstallerDetails JobInstallerEntity(DataTable dtInstaller)
        {
            JobInstallerDetails jobInstallerDetails = new JobInstallerDetails();
            try
            {
                if (dtInstaller.Rows.Count > 0)
                {
                    jobInstallerDetails = dtInstaller.AsEnumerable().Select(m => new JobInstallerDetails
                    {
                        IsVisitScheduled = m.Field<bool?>("IsVisitScheduled") == null ? false : m.Field<bool>("IsVisitScheduled")
                        ,
                        IsSystemUser = m.Field<bool?>("IsSystemUser") == null ? false : m.Field<bool>("IsSystemUser")
                        ,
                        SEStatus = m.Field<byte?>("SEStatus") == null ? 0 : m.Field<byte>("SEStatus")
                        ,
                        JobInstallerId = m.Field<int>("JobInstallerId")
                        ,
                        JobID = m.Field<int>("JobID")
                        ,
                        FirstName = m.Field<string>("Firstname")
                        ,
                        Surname = m.Field<string>("Surname")
                        ,
                        Phone = m.Field<string>("Phone")
                        ,
                        Mobile = m.Field<string>("Mobile")
                        ,
                        Email = m.Field<string>("Email")
                        ,
                        UnitTypeID = m.Field<int?>("UnitTypeID") == null ? 0 : m.Field<int>("UnitTypeID")
                        ,
                        UnitNumber = m.Field<string>("UnitNumber")
                        ,
                        StreetNumber = m.Field<string>("StreetNumber")
                        ,
                        StreetName = m.Field<string>("StreetName")
                        ,
                        StreetTypeID = m.Field<int?>("StreetTypeID") == null ? 0 : m.Field<int>("StreetTypeID")
                        ,
                        Town = m.Field<string>("Town")
                        ,
                        State = m.Field<string>("State")
                        ,
                        PostCode = m.Field<string>("PostCode")
                        ,
                        IsPostalAddress = m.Field<bool?>("IsPostalAddress") == null ? false : m.Field<bool>("IsPostalAddress")
                        ,
                        PostalAddressID = m.Field<string>("PostalAddressID") == null ? 0 : Convert.ToInt32(m.Field<string>("PostalAddressID"))
                        ,
                        PostalDeliveryNumber = m.Field<string>("PostalDeliveryNumber")
                        ,
                        LicenseNumber = m.Field<string>("LicenseNumber")
                        ,
                        SWHInstallerDesignerId = m.Field<int?>("SWHInstallerDesignerId") == null ? 0 : m.Field<int>("SWHInstallerDesignerId")
                    }).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                Helper.Log.WriteError(ex, "Error for JobInstallerEntity JobInstallerId : " + dtInstaller.Rows[0]["JobInstallerId"].ToString() + " " + DateTime.Now.ToString("dd/MM/yyyy") + " " + ex.Message);
                jobInstallerDetails = dtInstaller.Rows.Count > 0 ? DBClient.DataTableToList<JobInstallerDetails>(dtInstaller)[0] : new JobInstallerDetails();
            }
            return jobInstallerDetails;
        }

        public JobSystemDetails JobSystemDetailsEntity(DataTable dtJobSystemDetails)
        {
            JobSystemDetails jobSystemDetails = new JobSystemDetails();
            try
            {
                if (dtJobSystemDetails.Rows.Count > 0)
                {
                    jobSystemDetails = dtJobSystemDetails.AsEnumerable().Select(m => new JobSystemDetails
                    {
                        JobSystemDetailID = m.Field<int>("JobSystemDetailID")
                        ,
                        JobID = m.Field<int>("JobID")
                        ,
                        SystemSize = m.Field<decimal?>("SystemSize")
                        ,
                        SerialNumbers = m.Field<string>("SerialNumbers") == null ? "" : m.Field<string>("SerialNumbers")
                        ,
                        InverterSerialNumbers = m.Field<string>("InverterSerialNumbers") == null ? "" : m.Field<string>("InverterSerialNumbers")
                        ,
                        CalculatedSTC = m.Field<decimal?>("CalculatedSTC")
                        ,
                        InstallationType = m.Field<string>("InstallationType")
                        ,
                        NoOfPanel = m.Field<int?>("NoOfPanel")
                        ,
                        ModifiedCalculatedSTC = m.Field<decimal?>("ModifiedCalculatedSTC") == null ? 0 : m.Field<decimal>("ModifiedCalculatedSTC")
                    }).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Helper.Log.WriteError(ex, "Error for JobSystemDetailsEntity JobId : " + dtJobSystemDetails.Rows[0]["JobID"].ToString() + " " + DateTime.Now.ToString("dd/MM/yyyy") + " " + ex.Message);
                jobSystemDetails = dtJobSystemDetails.Rows.Count > 0 ? DBClient.DataTableToList<JobSystemDetails>(dtJobSystemDetails)[0] : new JobSystemDetails();
            }
            return jobSystemDetails;
        }

        public JobInstallationDetails JobInstallationDetailsEntity(DataTable dtJobInstallationDetails)
        {
            JobInstallationDetails jobInstallationDetails = new JobInstallationDetails();
            try
            {
                if (dtJobInstallationDetails.Rows.Count > 0)
                {
                    jobInstallationDetails = dtJobInstallationDetails.AsEnumerable().Select(m => new JobInstallationDetails
                    {
                        JobInstallationID = m.Field<int>("JobInstallationID")
                        ,
                        JobID = m.Field<int>("JobID")
                        ,
                        UnitTypeID = m.Field<int?>("UnitTypeID") == null ? 0 : m.Field<int>("UnitTypeID")
                        ,
                        UnitNumber = m.Field<string>("UnitNumber")
                        ,
                        StreetNumber = m.Field<string>("StreetNumber")
                        ,
                        StreetName = m.Field<string>("StreetName")
                        ,
                        StreetTypeID = m.Field<int?>("StreetTypeID") == null ? 0 : m.Field<int>("StreetTypeID")
                        ,
                        Town = m.Field<string>("Town")
                        ,
                        State = m.Field<string>("State")
                        ,
                        PostCode = m.Field<string>("PostCode")
                        ,
                        DistributorID = m.Field<int?>("DistributorID") == null ? 0 : m.Field<int>("DistributorID")
                        ,
                        NMI = m.Field<string>("NMI")
                        ,
                        PropertyType = m.Field<string>("PropertyType")
                        ,
                        PropertyName = m.Field<string>("PropertyName")
                        ,
                        SingleMultipleStory = m.Field<string>("SingleMultipleStory")
                        ,
                        InstallingNewPanel = m.Field<string>("InstallingNewPanel")
                        ,
                        MeterNumber = m.Field<string>("MeterNumber")
                        ,
                        PhaseProperty = m.Field<string>("PhaseProperty")
                        ,
                        ElectricityProviderID = m.Field<int?>("ElectricityProviderID") == null ? 0 : m.Field<int>("ElectricityProviderID")
                        ,
                        AdditionalNotes = m.Field<string>("AdditionalNotes")
                        ,
                        ExistingSystem = m.Field<bool?>("ExistingSystem") == null ? false : m.Field<bool>("ExistingSystem")
                        ,
                        ExistingSystemSize = m.Field<decimal?>("ExistingSystemSize") == null ? 0 : m.Field<decimal>("ExistingSystemSize")
                        ,
                        NoOfPanels = m.Field<int?>("NoOfPanels") == null ? 0 : m.Field<int>("NoOfPanels")
                        ,
                        SystemLocation = m.Field<string>("SystemLocation")
                        ,
                        PostalDeliveryNumber = m.Field<string>("PostalDeliveryNumber")
                        ,
                        IsPostalAddress = m.Field<bool?>("IsPostalAddress") == null ? false : m.Field<bool>("IsPostalAddress")
                        ,
                        PostalAddressID = m.Field<string>("PostalAddressID") == null ? 0 : Convert.ToInt32(m.Field<string>("PostalAddressID"))
                        ,
                        Location = m.Field<string>("Location")
                        ,
                        AdditionalInstallationInformation = m.Field<string>("AdditionalInstallationInformation")
                        ,
                        IsSameAsOwnerAddress = m.Field<bool?>("IsSameAsOwnerAddress") == null ? false : m.Field<bool>("IsSameAsOwnerAddress")
                        ,
                        Latitude = m.Field<string>("Latitude")
                        ,
                        Longitude = m.Field<string>("Longitude")
                        ,
                        IsInstallationAddressValid = m.Field<bool?>("IsInstallationAddressValid") == null ? false : m.Field<bool>("IsInstallationAddressValid")
                    }).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                Helper.Log.WriteError(ex, "Error for JobInstallationDetailsEntity JobId : " + dtJobInstallationDetails.Rows[0]["JobID"].ToString() + " " + DateTime.Now.ToString("dd/MM/yyyy") + " " + ex.Message);
                jobInstallationDetails = dtJobInstallationDetails.Rows.Count > 0 ? DBClient.DataTableToList<JobInstallationDetails>(dtJobInstallationDetails)[0] : new JobInstallationDetails();
            }
            return jobInstallationDetails;
        }

        public JobElectricians JobElectriciansEntity(DataTable dtJobElectrician)
        {
            JobElectricians jobElectricians = new JobElectricians();
            try
            {
                if (dtJobElectrician.Rows.Count > 0)
                {
                    jobElectricians = dtJobElectrician.AsEnumerable().Select(m => new JobElectricians
                    {
                        JobElectricianID = m.Field<int>("JobElectricianID")
                    ,
                        CompanyName = m.Field<string>("CompanyName")
                    ,
                        FirstName = m.Field<string>("FirstName")
                    ,
                        LastName = m.Field<string>("LastName")
                    ,
                        UnitTypeID = m.Field<byte?>("UnitTypeID") == null ? 0 : m.Field<byte>("UnitTypeID")
                    ,
                        UnitNumber = m.Field<string>("UnitNumber")
                    ,
                        StreetNumber = m.Field<string>("StreetNumber")
                    ,
                        StreetName = m.Field<string>("StreetName")
                    ,
                        StreetTypeID = m.Field<byte?>("StreetTypeID") == null ? 0 : m.Field<byte>("StreetTypeID")
                    ,
                        Town = m.Field<string>("Town")
                    ,
                        State = m.Field<string>("State")
                    ,
                        PostCode = m.Field<string>("PostCode")
                    ,
                        Phone = m.Field<string>("Phone")
                    ,
                        Mobile = m.Field<string>("Mobile")
                    ,
                        Email = m.Field<string>("Email")
                    ,
                        LicenseNumber = m.Field<string>("LicenseNumber")
                    ,
                        IsPostalAddress = m.Field<bool?>("IsPostalAddress") == null ? false : m.Field<bool>("IsPostalAddress")
                    ,
                        PostalAddressID = m.Field<int?>("PostalAddressID") == null ? 0 : m.Field<int>("PostalAddressID")
                    ,
                        PostalDeliveryNumber = m.Field<string>("PostalDeliveryNumber")
                    ,
                        Signature = m.Field<string>("Signature")
                    ,
                        InstallerID = m.Field<int?>("InstallerID") == null ? 0 : m.Field<int>("InstallerID")
                    ,
                        ElectricianID = m.Field<int?>("ElectricianID") == null ? 0 : m.Field<int>("ElectricianID")
                    ,
                        Latitude = m.Field<string>("Latitude")
                    ,
                        Longitude = m.Field<string>("Longitude")
                    ,
                        IpAddress = m.Field<string>("IpAddress")
                    ,
                        Location = m.Field<string>("Location")
                    ,
                        SignatureDate = m.Field<DateTime?>("SignatureDate") == null ? DateTime.Now : m.Field<DateTime>("SignatureDate")
                    }).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Helper.Log.WriteError(ex, "Error for JobElectriciansEntity JobElectricianID : " + dtJobElectrician.Rows[0]["JobElectricianID"].ToString() + " " + DateTime.Now.ToString("dd/MM/yyyy") + " " + ex.Message);
                jobElectricians = dtJobElectrician.Rows.Count > 0 ? DBClient.DataTableToList<JobElectricians>(dtJobElectrician)[0] : new JobElectricians();
            }
            return jobElectricians;
        }

        public JobOwnerDetails JobOwnerDetailsEntity(DataTable dtOwnerDetails)
        {
            JobOwnerDetails jobOwnerDetails = new JobOwnerDetails();
            try
            {
                if (dtOwnerDetails.Rows.Count > 0)
                {
                    jobOwnerDetails = dtOwnerDetails.AsEnumerable().Select(m => new JobOwnerDetails
                    {
                        JobOwnerID = m.Field<int>("JobOwnerID")
                    ,
                        JobID = m.Field<int>("JobID")
                    ,
                        CompanyName = m.Field<string>("CompanyName")
                    ,
                        FirstName = m.Field<string>("FirstName")
                    ,
                        LastName = m.Field<string>("LastName")
                    ,
                        UnitTypeID = m.Field<int?>("UnitTypeID") == null ? 0 : m.Field<int>("UnitTypeID")
                    ,
                        UnitNumber = m.Field<string>("UnitNumber")
                    ,
                        StreetNumber = m.Field<string>("StreetNumber")
                    ,
                        StreetName = m.Field<string>("StreetName")
                    ,
                        StreetTypeID = m.Field<int?>("StreetTypeID") == null ? 0 : m.Field<int>("StreetTypeID")
                    ,
                        Town = m.Field<string>("Town")
                    ,
                        State = m.Field<string>("State")
                    ,
                        PostCode = m.Field<string>("PostCode")
                    ,
                        Phone = m.Field<string>("Phone")
                    ,
                        Mobile = m.Field<string>("Mobile")
                    ,
                        Email = m.Field<string>("Email")
                    ,
                        IsPostalAddress = m.Field<bool?>("IsPostalAddress") == null ? false : m.Field<bool>("IsPostalAddress")
                    ,
                        PostalAddressID = m.Field<string>("PostalAddressID")
                    ,
                        PostalDeliveryNumber = m.Field<string>("PostalDeliveryNumber")
                    ,
                        OwnerSignature = m.Field<string>("OwnerSignature")
                    ,
                        SignatureDate = m.Field<DateTime?>("SignatureDate") == null ? DateTime.Now : m.Field<DateTime>("SignatureDate")
                    ,
                        Latitude = m.Field<string>("Latitude")
                    ,
                        Longitude = m.Field<string>("Longitude")
                    ,
                        IpAddress = m.Field<string>("IpAddress")
                    ,
                        Location = m.Field<string>("Location")
                    ,
                        OwnerType = m.Field<string>("OwnerType")
                    ,
                        CompanyABN = m.Field<string>("CompanyABN")
                    ,
                        IsOwnerRegisteredWithGST = m.Field<bool?>("IsOwnerRegisteredWithGST") == null ? false : m.Field<bool>("IsOwnerRegisteredWithGST")
                    ,
                        IsOwnerAddressValid = m.Field<bool?>("IsOwnerAddressValid") == null ? false : m.Field<bool>("IsOwnerAddressValid")
                    }).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Helper.Log.WriteError(ex, "Error for JobOwnerDetailsEntity JobID : " + dtOwnerDetails.Rows[0]["JobID"].ToString() + " " + DateTime.Now.ToString("dd/MM/yyyy") + " " + ex.Message);
                jobOwnerDetails = dtOwnerDetails.Rows.Count > 0 ? DBClient.DataTableToList<JobOwnerDetails>(dtOwnerDetails)[0] : new JobOwnerDetails();
            }
            return jobOwnerDetails;
        }
        public object InsertJobRequestData(int JobId, int JobSchedulingId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("JobSchedulingId", SqlDbType.Int, JobSchedulingId));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("IsUploaded", SqlDbType.Bit, 0));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.Crud("JobRequestData_Insert", sqlParameters.ToArray());
            return null;
        }


        public List<Username> GetAllUserList(int UserId, int UserTypeId, int jobid)
        {
            string spName = "[GetAllUserList]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeID", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobid));
            IList<Username> solarCompanyList = CommonDAL.ExecuteProcedure<Username>(spName, sqlParameters.ToArray());
            return solarCompanyList.ToList();
        }

        public List<JobNotes> GetComplianceNotes(int JobID)
        {
            List<JobNotes> lstJobNotes = new List<JobNotes>();
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobID));
                lstJobNotes = CommonDAL.ExecuteProcedure<JobNotes>("JobNotes_GetComplianceJobNoteList", sqlParameters.ToArray()).ToList();
                if (lstJobNotes != null && lstJobNotes.Count > 0)
                {
                    for (int i = 0; i < lstJobNotes.Count; i++)
                    {
                        lstJobNotes[i].strCreatedDate = lstJobNotes[i].CreatedDate.ToString("dd/MM/yyyy");
                    }
                }
                return lstJobNotes;
            }
            catch (Exception ex)
            {
                return lstJobNotes;
            }
        }

        public string GetUsernameByUserID(int UserID)
        {
            string spName = "[GetUsernameByUserID]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserID));
            object username = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            return Convert.ToString(username);
        }

        public string GetSTCStausNameBySTCStatusID(int STCStatusID)
        {
            string spName = "[GetSTCStausNameBySTCStatusID]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCStatusID", SqlDbType.Int, STCStatusID));
            object STCStatusName = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            return Convert.ToString(STCStatusName);
        }
        /// <summary>
        /// get ref number by job id
        /// </summary>
        /// <param name="JobId"></param>
        /// <returns></returns>
        public string GetRefNumberByJobId(int JobId)
        {
            string spName = "GetRefNumberByJobId";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            object RefNumber = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            return Convert.ToString(RefNumber);
        }
        public bool CheckAccessForInstallerNotes(int UserId)
        {
            string spName = "CheckAccessForInstallerNotes";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            object IsAccessToInstallerNotes = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            return Convert.ToBoolean(IsAccessToInstallerNotes);
        }
        public DataSet GetCallLogsBySTCJobDetailsID(int STCJobDetailsID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCJobDetailsID", SqlDbType.Int, STCJobDetailsID));
            DataSet dsCallLogs = CommonDAL.ExecuteDataSet("GetCallLogsbySTCJobDetailsID", sqlParameters.ToArray());
            return dsCallLogs;
        }
        /// <summary>
        /// Get UserId of Solarcompany and solarelectrician by JobID
        /// </summary>
        /// <param name="JobID"></param> 
        /// <returns></returns>
        public DataSet GetUserIDofSolarCompanyandSolarElectricianByJobID(int JobID)
        {
            string spName = "GetUserIDofSolarCompanyandSolarElectricianByJobID";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobID));
            DataSet ds = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return ds;
        }
        /// <summary>
        /// Insert User note to db
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="Notes"></param>
        /// <returns></returns>
        public void InsertUserNote(int UserID, string Notes)
        {
            string spName = "[InsertUserNotes]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserID));
            sqlParameters.Add(DBClient.AddParameters("Notes", SqlDbType.NVarChar, Notes));
            CommonDAL.Crud(spName, sqlParameters.ToArray());
        }
        /// <summary>
        /// Save Signature Log
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="signatureTypeId"></param>
        /// <param name="signatureMethodId"></param>
        /// <param name="signatureSource"></param>
        public void SaveSignatureLog(int jobId, int signatureTypeId, int signatureMethodId, string signatureSource)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("SignatureTypeId", SqlDbType.Int, signatureTypeId));
            sqlParameters.Add(DBClient.AddParameters("SignatureMethodId", SqlDbType.Int, signatureMethodId));
            sqlParameters.Add(DBClient.AddParameters("SignatureSource", SqlDbType.VarChar, signatureSource));
            CommonDAL.Crud("SaveSignatureLog", sqlParameters.ToArray());
        }

        public List<SearchResults> GetSearchResults(string search)
        {
            string spName = "[SearchInGreenbotSystem_Header]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Search", SqlDbType.VarChar, search));
            IList<SearchResults> Resultlist = CommonDAL.ExecuteProcedure<SearchResults>(spName, sqlParameters.ToArray());
            return Resultlist.ToList();
        }

        public void SaveSTCJobHistory(int STCJobDetailID, int STCStatusID, int UserID, string Description, DateTime CreatedDate, int CreatedBy)
        {
            string spName = "[SaveSTCJobHistory]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCJobDetailID", SqlDbType.Int, STCJobDetailID));
            sqlParameters.Add(DBClient.AddParameters("STCStatusID", SqlDbType.Int, STCStatusID));
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserID));
            sqlParameters.Add(DBClient.AddParameters("Description", SqlDbType.VarChar, Description));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, CreatedDate));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, CreatedBy));
            CommonDAL.Crud(spName, sqlParameters.ToArray());
        }

        public DataSet GetRECBulkuploadIDandSTCStatusbySTCJobDetailID(string STCJobDetailID)
        {
            string spName = "[GetRECBulkIDandSTCStatusbySTCJobDetailID]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCJobDetailsID", SqlDbType.VarChar, STCJobDetailID));
            DataSet dataSet = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dataSet;
        }
        /// <summary>
        /// get data for logs of upload photo
        /// </summary>
        /// <param name="VisitCheckListItemId"></param>
        /// <returns></returns>
        public DataSet GetCheckListItemDetailsFromIdForLog(int VisitCheckListItemId)
        {
            string spName = "[GetCheckListItemDetailsFromIdForLog]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VisitCheckListItemId", SqlDbType.Int, VisitCheckListItemId));
            DataSet dataSet = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dataSet;
        }
        /// <summary>
        /// get data for owner,installation for job
        /// </summary>
        /// <param name="JobId"></param>
        /// <returns></returns>
        public DataSet GetDataForEmailOfReplyNotes(int JobId, int noteCreatedBy = 0)
        {
            string spName = "[GetOwnerInstallationDataForEmail]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("jobid", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("loggedInuserId", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("usertypeId", SqlDbType.Int, ProjectSession.UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("noteCreatedById", SqlDbType.Int, noteCreatedBy));
            DataSet dataSet = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dataSet;
        }
        /// <summary>
        /// Updates the urgent job.
        /// </summary>
        /// <param name="UrgentJobDays">The number of days for job to be Urgent.</param>
        public void UpdateUrgentJobs(int UrgentJobDays)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UrgentJobDays", SqlDbType.Int, UrgentJobDays));
            sqlParameters.Add(DBClient.AddParameters("CurrentDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.Crud("UpdateUrgentJobIds", sqlParameters.ToArray());
        }
        /// <summary>
        /// update urgent review stc status flag 
        /// </summary>
        /// <param name="stcjobids"></param>
        /// <returns></returns>
        public DataSet UpdateUrgentStatusFlagForSTCIds(string stcjobids)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("stcjobids", SqlDbType.VarChar, stcjobids));
            sqlParameters.Add(DBClient.AddParameters("UrgentJobDays", SqlDbType.Int, Convert.ToInt32(ProjectConfiguration.UrgentSTCStatusJobDay)));
            sqlParameters.Add(DBClient.AddParameters("CurrentDate", SqlDbType.DateTime, DateTime.Now));
            DataSet ds = CommonDAL.ExecuteDataSet("UpdateUrgentStatusFlagForSTCIds", sqlParameters.ToArray());
            return ds;
        }
        public DataSet GetJobRetailerSettingData(int JobID, int SolarCompanyId, int userid = 0)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobID));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, userid));
            DataSet ds = CommonDAL.ExecuteDataSet("GetJobRetailerSettingData", sqlParameters.ToArray());
            return ds;
        }
        public DataSet GetJobRetailerSettingDataByJobId(int JobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobId));
            DataSet ds = CommonDAL.ExecuteDataSet("GetJobRetailerSettingDataByJobId", sqlParameters.ToArray());
            return ds;
        }
        public DataSet GetAutoSignSettingsDataForJob(int JobID, int SolarCompanyId, int UserID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobID));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserID));
            DataSet ds = CommonDAL.ExecuteDataSet("GetAutoSignSettingsDataForJob", sqlParameters.ToArray());
            return ds;
        }
        public bool CheckInstallationDate(string jobIds)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("jobIds", SqlDbType.VarChar, jobIds));
            object checkInstallationDate = CommonDAL.ExecuteScalar("CheckInstallationDate", sqlParameters.ToArray());
            return Convert.ToBoolean(checkInstallationDate);
        }
        public DataSet GetJobRetailerSettingDataRetailerIdWise(int JobID, int SolarCompanyId, int userid = 0)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobID));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, userid));
            DataSet ds = CommonDAL.ExecuteDataSet("GetJobRetailerSettingDataRetailerIdWise", sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// get spv product verification status
        /// </summary>
        /// <param name="JobId"></param>
        /// <returns></returns>
        public bool? GetProductVerificationStatusByJobId(int JobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobId));
            ////CommonDAL.Crud(spName, sqlParameters.ToArray());
            return Convert.ToBoolean(CommonDAL.ExecuteScalar("GetProductVerificationStatusByJobId", sqlParameters.ToArray()));
        }
        /// <summary>
        /// update supplier in job panel details
        /// </summary>
        /// <param name="JobId"></param>
        /// <param name="Brand"></param>
        /// <param name="Model"></param>
        public void UpdateSupplierForJob(int JobId, string Brand, string Model, string supplier)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("Brand", SqlDbType.NVarChar, Brand));
            sqlParameters.Add(DBClient.AddParameters("Model", SqlDbType.NVarChar, Model));
            sqlParameters.Add(DBClient.AddParameters("Supplier", SqlDbType.NVarChar, supplier));
            CommonDAL.Crud("UpdateSupplierForJob", sqlParameters.ToArray());

        }

        /// <summary>
        /// Gets the STC job stages with count by year.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="ResellerId">ResellerId.</param>
        /// <param name="RamId">RamId.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <returns>stage list</returns>
        public List<JobStage> GetSTCJobStagesWithCountByYear(int UserId, int UserTypeId, int ResellerId, int RamId, int SolarCompanyId, string isAllScaJobView, string isShowOnlyAssignJobsSCO = "", int year = 0)
        {
            string spName = "[Job_GetSTCJobSatgesWithCountByYear]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("RamId", SqlDbType.Int, RamId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("IsAllScaJobView", SqlDbType.Bit, string.IsNullOrEmpty(isAllScaJobView) ? false : Convert.ToBoolean(isAllScaJobView)));
            sqlParameters.Add(DBClient.AddParameters("isShowOnlyAssignJobsSCO", SqlDbType.Bit, string.IsNullOrEmpty(isShowOnlyAssignJobsSCO) ? false : Convert.ToBoolean(isShowOnlyAssignJobsSCO)));
            sqlParameters.Add(DBClient.AddParameters("Year", SqlDbType.Int, year));
            List<JobStage> lstSTCJobStage = CommonDAL.ExecuteProcedure<JobStage>(spName, sqlParameters.ToArray()).ToList();
            return lstSTCJobStage;
        }

        /// <summary>
        /// Gets the STC job stages with count for non cer approved stc submission.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="ResellerId">ResellerId.</param>
        /// <param name="RamId">RamId.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <returns>stage list</returns>
        public List<JobStage> GetSTCJobStagesWithCountForCERNotApproved(int UserId, int UserTypeId, int ResellerId, int RamId, int SolarCompanyId, string isAllScaJobView, string isShowOnlyAssignJobsSCO = "")
        {
            string spName = "[Job_GetSTCJobSatgesWithCountForCERNotApproved]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("RamId", SqlDbType.Int, RamId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("IsAllScaJobView", SqlDbType.Bit, string.IsNullOrEmpty(isAllScaJobView) ? false : Convert.ToBoolean(isAllScaJobView)));
            sqlParameters.Add(DBClient.AddParameters("isShowOnlyAssignJobsSCO", SqlDbType.Bit, string.IsNullOrEmpty(isShowOnlyAssignJobsSCO) ? false : Convert.ToBoolean(isShowOnlyAssignJobsSCO)));
            List<JobStage> lstSTCJobStage = CommonDAL.ExecuteProcedure<JobStage>(spName, sqlParameters.ToArray()).ToList();
            return lstSTCJobStage;
        }

        /// <summary>
        /// Gets the STC job stages with count by year for cer approved and isinvoiced true.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="ResellerId">ResellerId.</param>
        /// <param name="RamId">RamId.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <returns>stage list</returns>
        public List<JobStage> GetSTCJobStagesWithCountByYearForCERApproved(int UserId, int UserTypeId, int ResellerId, int RamId, int SolarCompanyId, string isAllScaJobView, string isShowOnlyAssignJobsSCO = "", int year = 0)
        {
            string spName = "[Job_GetSTCJobSatgesWithCountByYearForCERApproved]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("RamId", SqlDbType.Int, RamId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("IsAllScaJobView", SqlDbType.Bit, string.IsNullOrEmpty(isAllScaJobView) ? false : Convert.ToBoolean(isAllScaJobView)));
            sqlParameters.Add(DBClient.AddParameters("isShowOnlyAssignJobsSCO", SqlDbType.Bit, string.IsNullOrEmpty(isShowOnlyAssignJobsSCO) ? false : Convert.ToBoolean(isShowOnlyAssignJobsSCO)));
            sqlParameters.Add(DBClient.AddParameters("Year", SqlDbType.Int, year));
            List<JobStage> lstSTCJobStage = CommonDAL.ExecuteProcedure<JobStage>(spName, sqlParameters.ToArray()).ToList();
            return lstSTCJobStage;
        }

        public DataSet GetJobList_UserWiseColumnsByYear(int JobViewMenuId, int UserId, int UserTypeId, int PageNumber, int PageSize, string SortCol, string SortDir, int UrgentJobDay, int StageId, int SolarCompanyId, bool IsArchive, int ScheduleType, int JobType, int JobPriority, string searchtext, DateTime? FromDate, DateTime? ToDate, bool IsGst, bool jobref, bool jobdescription, bool jobaddress, bool jobclient, bool jobstaff, bool Invoiced, bool NotInvoiced, bool ReadyToTrade, bool NotReadyToTrade, bool traded, bool nottraded, bool preapprovalnotapproved, bool preapprovalapproved, bool connectioncompleted, bool connectionnotcompleted, bool ACT, bool NSW, bool NT, bool QLD, bool SA, bool TAS, bool WA, bool VIC, int PreApprovalStatusId, int ConnectionStatusId, int year)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobViewMenuId", SqlDbType.Int, JobViewMenuId));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("UrgentJobDay", SqlDbType.Int, UrgentJobDay));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("IsArchive", SqlDbType.Bit, IsArchive));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, PageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, PageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, SortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, SortDir));
            sqlParameters.Add(DBClient.AddParameters("StageId", SqlDbType.Int, StageId));
            sqlParameters.Add(DBClient.AddParameters("ScheduleType", SqlDbType.Int, ScheduleType));
            sqlParameters.Add(DBClient.AddParameters("JobType", SqlDbType.Int, JobType));
            sqlParameters.Add(DBClient.AddParameters("IsGst", SqlDbType.Bit, IsGst));
            sqlParameters.Add(DBClient.AddParameters("JobPriority", SqlDbType.Int, JobPriority));
            sqlParameters.Add(DBClient.AddParameters("searchtext", SqlDbType.NVarChar, searchtext));
            sqlParameters.Add(DBClient.AddParameters("FromDate", SqlDbType.DateTime, FromDate != null ? FromDate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("ToDate", SqlDbType.DateTime, ToDate != null ? ToDate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("Invoiced", SqlDbType.Bit, Invoiced));
            sqlParameters.Add(DBClient.AddParameters("NotInvoiced", SqlDbType.Bit, NotInvoiced));
            sqlParameters.Add(DBClient.AddParameters("ReadyToTrade", SqlDbType.Bit, ReadyToTrade));
            sqlParameters.Add(DBClient.AddParameters("NotReadyToTrade", SqlDbType.Bit, NotReadyToTrade));
            sqlParameters.Add(DBClient.AddParameters("IsJobRef", SqlDbType.Bit, jobref));
            sqlParameters.Add(DBClient.AddParameters("IsJobDescription", SqlDbType.Bit, jobdescription));
            sqlParameters.Add(DBClient.AddParameters("IsJobAddress", SqlDbType.Bit, jobaddress));
            sqlParameters.Add(DBClient.AddParameters("IsJobClient", SqlDbType.Bit, jobclient));
            sqlParameters.Add(DBClient.AddParameters("IsJobStaff", SqlDbType.Bit, jobstaff));
            sqlParameters.Add(DBClient.AddParameters("IsTraded", SqlDbType.Bit, traded));
            sqlParameters.Add(DBClient.AddParameters("IsNotTraded", SqlDbType.Bit, nottraded));
            sqlParameters.Add(DBClient.AddParameters("IsPreApprovalNotApproved", SqlDbType.Bit, preapprovalnotapproved));
            sqlParameters.Add(DBClient.AddParameters("IsPreApprovalApproved", SqlDbType.Bit, preapprovalapproved));
            sqlParameters.Add(DBClient.AddParameters("IsConnectionCompleted", SqlDbType.Bit, connectioncompleted));
            sqlParameters.Add(DBClient.AddParameters("IsConnectionNotCompleted", SqlDbType.Bit, connectionnotcompleted));
            sqlParameters.Add(DBClient.AddParameters("IsACT", SqlDbType.Bit, ACT));
            sqlParameters.Add(DBClient.AddParameters("IsNSW", SqlDbType.Bit, NSW));
            sqlParameters.Add(DBClient.AddParameters("IsNT", SqlDbType.Bit, NT));
            sqlParameters.Add(DBClient.AddParameters("IsQLD", SqlDbType.Bit, QLD));
            sqlParameters.Add(DBClient.AddParameters("IsSA", SqlDbType.Bit, SA));
            sqlParameters.Add(DBClient.AddParameters("IsTAS", SqlDbType.Bit, TAS));
            sqlParameters.Add(DBClient.AddParameters("IsWA", SqlDbType.Bit, WA));
            sqlParameters.Add(DBClient.AddParameters("IsVIC", SqlDbType.Bit, VIC));
            sqlParameters.Add(DBClient.AddParameters("PreApprovalStatusId", SqlDbType.Int, PreApprovalStatusId));
            sqlParameters.Add(DBClient.AddParameters("ConnectionStatusId", SqlDbType.Int, ConnectionStatusId));
            sqlParameters.Add(DBClient.AddParameters("CurrentDateTime", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("Year", SqlDbType.Int, year));

            DataSet dsJobsPlusColumns = CommonDAL.ExecuteDataSet("Job_GetJobList_UserWiseColumnsByYear", sqlParameters.ToArray());
            return dsJobsPlusColumns;
        }

        public DataSet GetJobList_ForCachingDataKendoByYear(string SolarCompanyId, int UserId, int JobViewMenuId, int year)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.NVarChar, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("JobViewMenuId", SqlDbType.Int, JobViewMenuId));
            sqlParameters.Add(DBClient.AddParameters("CurrentDateTime", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("Year", SqlDbType.Int, year));

            return CommonDAL.ExecuteDataSet("Job_GetJobList_ForCachingData_KendoByYear", sqlParameters.ToArray());
        }

        public DataTable GetSTCInvoiceAndCreditNoteCount(string stcJobdetailIds)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("stcjobdetailIds", SqlDbType.VarChar, stcJobdetailIds));
            DataTable dt = CommonDAL.ExecuteDataSet("GetSTCInvoiceAndCreditNoteCount", sqlParameters.ToArray()).Tables[0];
            return dt;
        }

        public DataTable GetSTCInvoiceData(int StcJobDetailId, string STCInvoiceNumber)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("StcJobDetailId", SqlDbType.Int, StcJobDetailId));
            sqlParameters.Add(DBClient.AddParameters("STCInvoiceNumber", SqlDbType.VarChar, STCInvoiceNumber));
            DataTable dt = CommonDAL.ExecuteDataSet("GetSTCInvoiceData", sqlParameters.ToArray()).Tables[0];
            return dt;
        }

        public DataSet GetJobsForSEAndSC(int UserId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            DataSet ds = CommonDAL.ExecuteDataSet("GetJobsForSEAndSC", sqlParameters.ToArray());
            return ds;
        }
        public DataSet GetStartEndDateForPanel(string manufacturer, string model)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Manufacturer", SqlDbType.VarChar, manufacturer));
            sqlParameters.Add(DBClient.AddParameters("Model", SqlDbType.VarChar, model));
            DataSet ds = CommonDAL.ExecuteDataSet("GetStartEndDateForPanel", sqlParameters.ToArray());
            return ds;
        }
        public DataSet GetStartEndDateForInverter(string brand, string model, string series)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Manufacturer", SqlDbType.VarChar, brand));
            sqlParameters.Add(DBClient.AddParameters("Model", SqlDbType.VarChar, model));
            sqlParameters.Add(DBClient.AddParameters("Series", SqlDbType.VarChar, series));
            DataSet ds = CommonDAL.ExecuteDataSet("GetStartEndDateForInverter", sqlParameters.ToArray());
            return ds;
        }
        public DataSet GetStartEndDateForSWHBrandModel(string manufacturer, string model)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Manufacturer", SqlDbType.VarChar, manufacturer));
            sqlParameters.Add(DBClient.AddParameters("Model", SqlDbType.VarChar, model));
            DataSet ds = CommonDAL.ExecuteDataSet("GetStartEndDateForSWHBrandModel", sqlParameters.ToArray());
            return ds;
        }
        /// <summary>
        /// change sca and ra in job
        /// </summary>
        /// <param name="ResellerID"></param>
        /// <param name="SolarCompanyID"></param>
        /// <param name="JobIDs"></param>
        public void ChangeSCARAInJob(int ResellerID, int SolarCompanyID, string JobIDs)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerID));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyID));
            sqlParameters.Add(DBClient.AddParameters("jobid", SqlDbType.VarChar, JobIDs));
            CommonDAL.Crud("ChangeRA_SCA_InJob", sqlParameters.ToArray());
        }

        public bool CheckJobexists(int JobID)
        {
            string spName = "[CheckJobExist]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobID));
            ////CommonDAL.Crud(spName, sqlParameters.ToArray());
            return Convert.ToBoolean(CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray()));
        }
        public void SendToSAASInvoiceBuilder(int stcJobDetailsId, int jobId, int resellerId, int userId, DateTime stcSubmissionDate)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("STCJobDetailsId", SqlDbType.Int, stcJobDetailsId));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, resellerId));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, userId));
            sqlParameters.Add(DBClient.AddParameters("STCSubmissionDate", SqlDbType.DateTime, stcSubmissionDate));
            CommonDAL.Crud("SendToSAASInvoiceBuilder", sqlParameters.ToArray());
        }
        /// <summary>
        /// Get DateTimeTicks From QueuedRECSubmission for deleting rec folders and zip files
        /// </summary>
        /// <returns></returns>
        public DataTable GetDateTimeTicksFromQueuedRECSubmission()
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("RECBulkUploadTime", SqlDbType.Date, DateTime.Now.Date.AddDays(-1)));
            return CommonDAL.ExecuteDataSet("GetDateTimeTicksFromQueuedRECSubmission", sqlParameters.ToArray()).Tables[0];
        }

        public List<UserWiseColumns> GetUserWiseColumnsStatic(int UserID, int JobViewMenuId)
        {
            List<UserWiseColumns> listUserWiseColumns = new List<UserWiseColumns>();
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, UserID));
            sqlParameters.Add(DBClient.AddParameters("JobViewMenuId", SqlDbType.Int, JobViewMenuId));

            return CommonDAL.ExecuteProcedure<UserWiseColumns>("GetUserWiseColumnStatic", sqlParameters.ToArray()).ToList();
        }

        public List<STCSubmissionView> GetJobSTCSubmissionKendoByYearWithoutCacheDapper(string SolarCompanyId, int ResellerId, int Year, int page, int pageSize, DataTable dtFilter, DataTable dtSort, int StageId = 0, string sStageId = "")
        {
            DynamicParameters dynamicParameters = new DynamicParameters();

            dynamicParameters.Add("SolarCompanyId", SolarCompanyId, DbType.String);
            dynamicParameters.Add("ResellerId", ResellerId, DbType.Int32);
            dynamicParameters.Add("CurrentDateTime", DateTime.Now, DbType.DateTime);
            dynamicParameters.Add("Year", Year, DbType.Int32);
            dynamicParameters.Add("page", page, DbType.Int32);
            dynamicParameters.Add("pagesize", pageSize, DbType.Int32);
            dynamicParameters.Add("dtFilter", dtFilter, DbType.Object);
            dynamicParameters.Add("dtSort", dtSort, DbType.Object);
            dynamicParameters.Add("StageId", StageId, DbType.Int32);
            dynamicParameters.Add("strStageId", sStageId, DbType.String);
            return CommonDAL.ExecuteProcedureDapper<STCSubmissionView>("Job_GetJobSTCSubmission_KendoByYearNew", dynamicParameters);
        }

        public DataSet GetJobSTCSubmissionForCacheService(string SolarCompanyId, int ResellerId, int Year)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.NVarChar, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("CurrentDateTime", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("Year", SqlDbType.Int, Year));
            return CommonDAL.ExecuteDataSet("Job_GetJobSTCSubmission_CacheService", sqlParameters.ToArray());
        }

        /// <summary>
        /// Returns number of days or date
        /// </summary>
        /// <returns></returns>
        public DataSet GetSettlementDaysToAddForUpFrontSettlementterm(string JobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.NVarChar, JobId));
            DataSet ds = CommonDAL.ExecuteDataSet("GetSettlementDaysForUpFrontSettlementterm", sqlParameters.ToArray());
            return ds;
        }

        public DataSet ChangeSTCJobStage(int UserId, int UserTypeId, int STCJobStageID, int JobID, DateTime CreatedDate)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("STCJobStageID", SqlDbType.Int, STCJobStageID));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobID));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, CreatedDate));
            DataSet dsGeneratedSTCInvoices = CommonDAL.ExecuteDataSet("Job_ChangeSTCJobStage", sqlParameters.ToArray());
            return dsGeneratedSTCInvoices;
        }

        public DataSet UpdateUrgentStatusFlagForJobIds(int jobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("UrgentJobDays", SqlDbType.Int, Convert.ToInt32(ProjectConfiguration.UrgentSTCStatusJobDay)));
            sqlParameters.Add(DBClient.AddParameters("CurrentDate", SqlDbType.DateTime, DateTime.Now));
            DataSet ds = CommonDAL.ExecuteDataSet("UpdateUrgentStatusFlagForJobIds", sqlParameters.ToArray());
            return ds;
        }
    }
}


