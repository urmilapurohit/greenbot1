using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using FormBot.Entity;
using System.Data;
using FormBot.Entity.Job;
using FormBot.Entity.Settings;
using FormBot.Entity.SolarElectrician;
using FormBot.Entity.CheckList;
using FormBot.Entity.Pdf;
using iTextSharp.text.pdf;
using FormBot.Entity.Documents;
using FormBot.Entity.SPV;
using FormBot.Entity.Email;
using FormBot.Entity.KendoGrid;

namespace FormBot.BAL.Service
{
    public interface ICreateJobBAL
    {
        /// <summary>
        /// Gets the se user.
        /// </summary>
        /// <param name="isInstaller">if set to <c>true</c> [is installer].</param>
        /// <param name="CompanyId">The company identifier.</param>
        /// <param name="existUserId">The exist user identifier.</param>
        /// <returns>list of electrician</returns>
        List<SolarElectricianView> GetSEUser(bool isInstaller, int CompanyId, int existUserId);

        /// <summary>
        /// Get Staff Name From Reseller
        /// </summary>
        /// <param name="ResellerId"></param>
        /// <returns></returns>

        List<string> GetStaffNameFromResellerOrRam(int ResellerId, int UserTypeId, string isAllScaJobView);

        /// <summary>
        /// Gets the se user With Status.
        /// </summary>
        /// <param name="isInstaller">if set to <c>true</c> [is installer].</param>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="existUserId">The exist user identifier.</param>
        /// <returns>solar view</returns>
        List<SolarElectricianView> GetSEUserWithStatus(bool isInstaller, int companyId, int existUserId);

        /// <summary>
        /// Gets the job stage.
        /// </summary>
        /// <returns>list of job stage</returns>
        List<JobStage> GetJobStage();

        /// <summary>
        /// Gets the STC job stage.
        /// </summary>
        /// <returns></returns>
        List<JobStage> GetSTCJobStage();

        /// <summary>
        /// Gets the electricity provider.
        /// </summary>
        /// <returns>list of electricity provider</returns>
        List<ElectricityProvider> GetElectricityProvider();

        /// <summary>
        /// Gets the deeming period.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns>list of year</returns>
        List<string> GetDeemingPeriod(int year);

        /// <summary>
        /// Get Year In Words
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        string GetYearInWords(int year);

        /// <summary>
        /// Inserts the job.
        /// </summary>
        /// <param name="createJob">The create job.</param>
        /// <param name="xmlPanels">The XML panels.</param>
        /// <param name="xmlInverters">The XML inverters.</param>
        /// <returns>integer job id</returns>
        Int32 InsertJob(CreateJob createJob, string xmlPanels, string xmlInverters, int SolarCompanyId, int LoggedInUserId, DataTable dtCustomField = null);

        /// <summary>
        /// Get Job By ID.
        /// </summary>
        /// <param name="JobID">Job identifier</param>
        /// <returns>job by id</returns>
        CreateJob GetJobByID(int JobID);

        CreateJob GetJobByIDTabWise(int JobID, int DATAOPMODE);

        //Get Documents Count for Documents Tab
        CreateJob GetDocumentsandPhotosTabCount(int JobID);

        //Job Summary Tabular 
        CreateJob GetJobSummaryTabular(int JobID);

        /// <summary>
        /// Gets the common serial by identifier.
        /// </summary>
        /// <param name="JobID">The job identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <returns></returns>
        DataSet GetCommonSerialByID(int JobID, int UserTypeId);

        /// <summary>
        /// Gets the common inverter serial by identifier.
        /// </summary>
        /// <param name="JobID">The job identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <returns></returns>
        DataSet GetCommonInverterSerialByID(int jobID, int UserTypeId);

        /// <summary>
        /// Gets the panel.
        /// </summary>
        /// <param name="Mode">The mode.</param>
        /// <param name="CertificateHolder">The certificate holder.</param>
        /// <param name="JobType">Type of the job.</param>
        /// <returns>job panel details</returns>
        List<JobPanelDetails> GetPanel(string Mode, string CertificateHolder, string JobType);

        /// <summary>
        /// Gets the panel data.
        /// </summary>
        /// <returns></returns>
        List<PanelModel> GetPanelData();

        /// <summary>
        /// Gets the inverter data.
        /// </summary>
        /// <returns></returns>
        List<Inverter> GetInverterData();

        /// <summary>
        /// Gets the system brand data.
        /// </summary>
        /// <returns></returns>
        List<SystemBrandModel> GetSystemBrandData();

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
        List<JobListModel> GetJobList_VendorAPI(string CreatedDate, string FromDate, string ToDate, string RefNumber, string VendorJobId, string SolarCompanyId, string CompanyABN, int? ResellerId);

        /// <summary>
        /// Gets the job inverter.
        /// </summary>
        /// <param name="Mode">The mode.</param>
        /// <param name="Search">The search.</param>
        /// <param name="manufacturer">The manufacturer.</param>
        /// <returns>job inverter</returns>
        List<JobInverterDetails> GetJobInverter(string Mode, string Search, string manufacturer = null);

        /// <summary>
        /// Deletes the selected jobs.
        /// </summary>
        /// <param name="lstJobs">The LST jobs.</param>
        /// <param name="ModifiedDate">The modified date.</param>
        /// <param name="LogginUserID">The login user identifier.</param>
        /// <returns>delete job</returns>
        List<DeleteJob_Failed> DeleteSelectedJobs(List<int> lstJobs, DateTime ModifiedDate, int LogginUserID, int UserTypeId);

        /// <summary>
        /// Open deleted jobs.
        /// </summary>
        /// <param name="lstJobs">The LST jobs.</param>
        /// <param name="ModifiedDate">The modified date.</param>
        /// <param name="LogginUserID">The login user identifier.</param>
        /// <returns>delete job</returns>
        void OpenDeletedJobs(List<int> lstJobs, DateTime ModifiedDate, int LogginUserID);

        /// <summary>
        /// Creates the job number.
        /// </summary>
        /// <param name="jobType">Type of the job.</param>
        /// <returns>data set</returns>
        DataSet CreateJobNumber(int jobType, int SolarCompanyId);



        /// <summary>
        /// Get the required data for trade the select job
        /// </summary>
        /// <param name="lstJobs"></param>
        /// <returns></returns>
        List<JobList> GetDataForTradeJob(List<int> lstJobs);

        /// <summary>
        /// Gets the job list.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="PageNumber">The page number.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <param name="SortCol">The sort col.</param>
        /// <param name="SortDirection">The sort Direction.</param>
        /// <param name="UrgentJobDay">The urgent job day.</param>
        /// <param name="StageId">The stage identifier.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <param name="IsArchive">if set to <c>true</c> [is archive].</param>
        /// <param name="ScheduleType">Type of the schedule.</param>
        /// <param name="JobType">Type of the job.</param>
        /// <param name="JobPriority">The job priority.</param>
        /// <param name="searchtext">The search text.</param>
        /// <param name="FromDate">From date.</param>
        /// <param name="ToDate">To date.</param>
        /// <param name="jobref">if set to <c>true</c> [job reference].</param>
        /// <param name="jobdescription">if set to <c>true</c> [job description].</param>
        /// <param name="jobaddress">if set to <c>true</c> [job address].</param>
        /// <param name="jobclient">if set to <c>true</c> [job client].</param>
        /// <param name="jobstaff">if set to <c>true</c> [job staff].</param>
        /// <param name="Invoiced">if set to <c>true</c> [invoiced].</param>
        /// <param name="NotInvoiced">if set to <c>true</c> [not invoiced].</param>
        /// <param name="ReadyToTrade">if set to <c>true</c> [ready to trade].</param>
        /// <param name="NotReadyToTrade">if set to <c>true</c> [not ready to trade].</param>
        /// <param name="traded">if set to <c>true</c> [traded].</param>
        /// <param name="nottraded">if set to <c>true</c> [not traded].</param>
        /// <param name="preapprovalnotapproved">if set to <c>true</c> [preapproval not approved].</param>
        /// <param name="preapprovalapproved">if set to <c>true</c> [preapproval approved].</param>
        /// <param name="connectioncompleted">if set to <c>true</c> [connection completed].</param>
        /// <param name="connectionnotcompleted">if set to <c>true</c> [connection not completed].</param>
        /// <param name="ACT">if set to <c>true</c>.</param>
        /// <param name="NSW">if set to <c>true</c>.</param>
        /// <param name="NT">if set to <c>true</c>.</param>
        /// <param name="QLD">if set to <c>true</c>.</param>
        /// <param name="SA">if set to <c>true</c>.</param>
        /// <param name="TAS">if set to <c>true</c>.</param>
        /// <param name="WA">if set to <c>true</c>.</param>
        /// <param name="VIC">if set to <c>true</c>.</param>
        /// <param name="PreApprovalStatusId">The pre approval status identifier.</param>
        /// <param name="ConnectionStatusId">The connection status identifier.</param>
        /// <returns>job list</returns>
        List<JobList> GetJobList(int UserId, int UserTypeId, int PageNumber, int PageSize, string SortCol, string SortDirection, int UrgentJobDay, int StageId, string SolarCompanyId, bool IsArchive, int ScheduleType, int JobType, int JobPriority, string searchtext, DateTime? FromDate, DateTime? ToDate, bool IsGst, bool jobref, bool jobdescription, bool jobaddress, bool jobclient, bool jobstaff, bool Invoiced, bool NotInvoiced, bool ReadyToTrade, bool NotReadyToTrade, bool traded, bool nottraded, bool preapprovalnotapproved, bool preapprovalapproved, bool connectioncompleted, bool connectionnotcompleted, bool ACT, bool NSW, bool NT, bool QLD, bool SA, bool TAS, bool WA, bool VIC, int PreApprovalStatusId, int ConnectionStatusId, DateTime? FromDateInstalling, DateTime? ToDateInstalling);

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <returns>assign list</returns>
        List<AssignSCO> GetSCOUser(int solarcompanyId);

        /// <summary>
        /// Assigns the job.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>select assign job</returns>
        IEnumerable<SelectListItem> AssignJobToSCO(int userID);

        /// <summary>
        /// Creates the job mapping.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="assignedsc">The assigned.</param>
        /// <returns>create job mapping</returns>
        object CreateJobSCOMapping(int id, string assignedsc);

        /// <summary>
        /// Gets all job.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>select list</returns>
        IEnumerable<SelectListItem> GetAllJobToSCO(int userID);

        /// <summary>
        /// Get all selected jobs to assign SCO
        /// </summary>
        /// <param name="jobIds"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetAllJobsToAssignSCO(string jobIds);

        /// <summary>
        /// Gets the user by user identifier.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>integer id</returns>
        int GetSCOByUserId(int userID);

        /// <summary>
        /// Gets the assign job.
        /// </summary>
        /// <param name="logedinUserID">The login user identifier.</param>
        /// <param name="scoID">The user identifier.</param>
        /// <returns>list of user</returns>
        List<SelectListItem> GetAssignJobToSCOList(int logedinUserID, int scoID);

        /// <summary>
        /// Creates the job documents.
        /// </summary>
        /// <param name="documentsView">The documents view.</param>
        /// <returns>integer id</returns>
        int CreateJobDocuments(Entity.Documents.DocumentsView documentsView, bool isClassic);

        /// <summary>
        /// Creates the job notes.
        /// </summary>
        /// <param name="notes">The notes.</param>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="createdBy">The created by.</param>
        void CreateJobNotes(string notes, int jobID, int createdBy);

        /// <summary>
        /// Gets the job notes list.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>data set</returns>
        System.Data.DataSet GetJobNotesList(int pageIndex, int jobID);

        /// <summary>
        /// Deletes the job notes.
        /// </summary>
        /// <param name="jobNotesId">The job notes identifier.</param>
        void DeleteJobNotes(int jobNotesId);

        /// <summary>
        /// Inserts the photo.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="status">The status.</param>
        void InsertPhoto(int jobID, string filename, int status);

        /// <summary>
        /// Inserts the photo.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="status">The status.</param>
        DataSet InsertReferencePhoto(int jobID, string filename, int UserId, string jobscId, string cId, bool isDefault, string ClassType, string VendorJobPhotoId, string Status = "", string Latitude = "", string Longitude = "", string ImageTakenDate = "", string Brand = "", string Manufacturer = "", bool isFromAPP = false, string Altitude = "", string Accuracy = "");

        /// <summary>
        /// Deletes the photo.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="FolderName">Name of the folder.</param>
        void DeletePhoto(string fileName, int FolderName);

        /// <summary>
        /// Deletes the photo API.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="FolderName">Name of the folder.</param>
        void DeletePhotoApi(string fileName, int FolderName);

        /// <summary>
        /// Deletes the visit check list photos by path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="jobId">The job identifier.</param>
        void DeleteVisitCheckListPhotosByPath(string path, int jobId);

        /// <summary>
        /// Gets the job installation photo by job identifier.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>list of document</returns>
        List<UserDocument> GetJobInstallationPhotoByJobID(int jobID);

        /// <summary>
        /// Gets the job scheduling photos by job identifier.
        /// </summary>
        /// <param name="JobSchedulingId">The job scheduling identifier.</param>
        /// <returns></returns>
        List<UserDocument> GetJobSchedulingPhotosByJobID(int JobSchedulingId);

        /// <summary>
        /// Gets the job installation serial by job identifier.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>list of document</returns>
        List<UserDocument> GetJobInstallationSerialByJobID(int jobID);

        /// <summary>
        /// Gets the SSC user.
        /// </summary>
        /// <returns>list of basic details</returns>
        List<BasicDetails> GetSSCUser();

        /// <summary>
        /// Creates the job SSC mapping.
        /// </summary>
        /// <param name="rAMID">The r amid.</param>
        /// <param name="jobID">The job identifier.</param>
        void CreateJobSSCMapping(int rAMID, int jobID);

        /// <summary>
        /// Gets the SSC user by job identifier.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <returns>list of basic detail</returns>
        List<BasicDetails> GetSSCUserByJbID(int jobID, int SolarCompanyId);

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <param name="sscJOBID">The SSC job identifier.</param>
        /// <returns>integer identifier</returns>
        List<SolarSubContractor> GetSSCID(int sscJOBID);

        /// <summary>
        /// Get Schedule by job identifier
        /// </summary>
        /// <param name="jobID">job identifier</param>
        /// <returns>job schedule</returns>
        List<JobScheduling> GetJobschedulingByJobID(int jobID);

        /// <summary>
        /// Gets the job stages with count.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <returns>job stage</returns>
        List<JobStage> GetJobStagesWithCount(int UserId, int UserTypeId, int SolarCompanyId);

        /// <summary>
        /// Deletes the job document.
        /// </summary>
        /// <param name="documentsView">The documents view.</param>
        /// <returns>integer identifier</returns>
        int DeleteJobDocument(Entity.Documents.DocumentsView documentsView, string Path);

        /// <summary>
        /// Deletes the job document new.
        /// </summary>
        /// <param name="JobDocumentId">The job document identifier.</param>
        /// <param name="Path">The path.</param>
        void DeleteJobDocumentNew(int JobDocumentId, string Path);

        /// <summary>
        /// Inserts the other documents.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="logedInUserID">The login user identifier.</param>
        void InsertOtherDocuments(int jobID, string filename, int logedInUserID);

        /// <summary>
        /// Inserts the other documents.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="logedInUserID">The login user identifier.</param>
        int InsertCESDocuments(int JobId, string Path, int UserID, string Type, string JsonData, bool IsSPVxml = false, bool IsWrittenStatement = false);

        /// <summary>
        /// Gets the job other document by job identifier.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>list of user document</returns>
        List<UserDocument> GetJobOtherDocumentByJobID(int jobID);

        /// <summary>
        /// Gets the job document by job id
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        List<DocumentsView> GetJobDocumentByJobID(int jobID);

        /// <summary>
        /// Deletes the other document.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="folderName">Name of the folder.</param>
        void DeleteOtherDocument(string fileName, int folderName);

        /// <summary>
        /// Gets all calendar data.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns>data set</returns>
        DataSet GetAllCalendarData(int userId, int userTypeId, int solarCompanyId);

        //DataSet GetDocumentsByType(int jobId, string type);
        /// <summary>
        /// Gets the type of the documents by.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <returns></returns>
        DataSet GetDocumentsByType(int jobId);

        /// <summary>
        /// Gets the identifier by job identifier.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>integer id</returns>
        int GetSCOIdByJobId(int jobID);

        /// <summary>
        /// Gets the job by identifier for PDF.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="solarCompanyID">The solar company identifier.</param>
        /// <param name="installerSignPath">The installer sign path.</param>
        /// <param name="ownerSignPath">The owner sign path.</param>
        /// <param name="sCASignPath">The path.</param>
        /// <param name="electricianSignPath">The electrician sign path.</param>
        /// <returns>data set</returns>
        DataSet GetJobByIDForPDF(int jobID, int solarCompanyID, string installerSignPath, string ownerSignPath, string sCASignPath, string electricianSignPath, DateTime createdDate);

        /// <summary>
        /// Gets the header details.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <returns>data table for header</returns>
        DataSet GetHeaderDetails(int jobId);

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
        void GetJobOwnerSignature(int jobId, string ownerSignature, string Latitude, string Longitude, string IpAddress, string Location, DateTime SignatureDate, string path);

        /// <summary>
        /// Checks the status and installation date.
        /// </summary>
        /// <param name="InstallerId">The installer identifier.</param>
        /// <param name="DesignerId">The designer identifier.</param>
        /// <returns>data set</returns>
        DataSet CheckStatusAndInstallationDate(int InstallerId, int DesignerId);

        /// <summary>
        /// Updates the priority for jobs.
        /// </summary>
        /// <param name="UrgentJobDay">The urgent job day.</param>
        void UpdatePriorityForJobs(int UrgentJobDay);

        /// <summary>
        /// Inserts the record data.
        /// </summary>
        /// <param name="dtRECData">The date record data.</param>
        /// <param name="createdDate">The created date.</param>
        /// <returns>data set</returns>
        DataSet InsertRECData(DataTable dtRECData, DateTime createdDate);

        /// <summary>
        /// Inserts the record failure job reason.
        /// </summary>
        /// <param name="dtReason">The date reason.</param>
        void InsertRECFailureJobReason(DataTable dtReason);

        /// <summary>
        /// Checks the record data exist for date.
        /// </summary>
        /// <param name="createdDate">The created date.</param>
        /// <returns>return boolean </returns>
        bool CheckRECDataExistForDate(DateTime createdDate);

        /// <summary>
        /// Gets the theme by job identifier.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>data set</returns>
        DataSet GetThemeByJobId(int jobID);

        /// <summary>
        /// Gets the drop down values by job identifier.
        /// </summary>
        /// <param name="unitTypeID">The unit type identifier.</param>
        /// <param name="streetTypeId">The street type identifier.</param>
        /// <param name="postalAddressID">The postal address identifier.</param>
        /// <param name="model">The model.</param>
        /// <param name="brand">The brand.</param>
        /// <returns>data set</returns>
        DataSet GetDropDownValuesByJobId(int unitTypeID, int streetTypeId, int postalAddressID, int model, int brand);

        /// <summary>
        /// Gets the jobs for custom pricing.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="resellerId">The reseller identifier.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <param name="systemSize">Size of the system.</param>
        /// <param name="ownerName">Name of the owner.</param>
        /// <param name="ownerAddress">The owner address.</param>
        /// <param name="refNumber">The reference number.</param>
        /// <returns>job list</returns>
        List<JobList> GetJobsForCustomPricing(int userId, int userTypeId, int resellerId, int solarCompanyId, int systemSize, string ownerName, string ownerAddress, string refNumber);

        /// <summary>
        /// Removes the SSC request.
        /// </summary>
        /// <param name="notes">The notes.</param>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="requestedBy">The requested by.</param>
        void RemoveSSCRequest(string notes, int jobID, int requestedBy);

        /// <summary>
        /// Gets the job history list.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="order">The order.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <param name="categoryID">The category identifier.</param>
        /// <returns>data set</returns>
        DataSet GetJobHistoryList(int jobID, string order, DateTime? fromDate, DateTime? toDate, int? categoryID, int pageIndex = 1);
        DataSet GetJobHistory(int jobID, string order, int? categoryID);
        DataSet GetJobHistoryForJobDetail(int jobID, string order, int? categoryID);
        /// <summary>
        /// Gets the job submission.
        /// </summary>
        /// <param name="serId">The identifier.</param>
        /// <param name="serTypeId">The type identifier.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="stageId">The stage identifier.</param>
        /// <param name="complianceOfficcerId">The compliance officer identifier.</param>
        /// <param name="ResellerId">The reseller identifier.</param>
        /// <param name="RamId">The ram identifier.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <param name="pvdswhcode">The code.</param>
        /// <param name="refJobId">The reference job identifier.</param>
        /// <param name="ownername">The owner name.</param>
        /// <param name="installationaddress">The installation address.</param>
        /// <param name="submissionFromDate">The submission from date.</param>
        /// <param name="submissionToDate">The submission to date.</param>
        /// <param name="settlementFromDate">The settlement from date.</param>
        /// <param name="settlementToDate">The settlement to date.</param>
        /// <param name="invoiced">The invoiced.</param>
        /// <returns>
        /// job list
        /// </returns>
        DataSet GetJobSTCSubmissionKendo(string solarCompanyId, int ResellerId);

        DataSet GetJobSTCSubmissionKendoByYear(string SolarCompanyId, int ResellerId, int Year);

        /// <summary>
        /// Get STC submission old page 
        /// </summary>
        /// <param name="serId"></param>
        /// <param name="serTypeId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortDirection"></param>
        /// <param name="stageId"></param>
        /// <param name="complianceOfficcerId"></param>
        /// <param name="ResellerId"></param>
        /// <param name="RamId"></param>
        /// <param name="solarCompanyId"></param>
        /// <param name="pvdswhcode"></param>
        /// <param name="refJobId"></param>
        /// <param name="ownername"></param>
        /// <param name="installationaddress"></param>
        /// <param name="submissionFromDate"></param>
        /// <param name="submissionToDate"></param>
        /// <param name="settlementFromDate"></param>
        /// <param name="settlementToDate"></param>
        /// <param name="invoiced"></param>
        /// <param name="SettlementTermId"></param>
        /// <param name="JobID"></param>
        /// <returns></returns>
        List<JobList> GetJobSTCSubmission(int serId, int serTypeId, int pageNumber, int pageSize, string sortColumn, string sortDirection, int stageId, int complianceOfficcerId, int ResellerId, int RamId, int solarCompanyId, string pvdswhcode, string refJobId, string ownername, string installationaddress, DateTime? submissionFromDate, DateTime? submissionToDate, DateTime? settlementFromDate, DateTime? settlementToDate, int invoiced, int SettlementTermId, string isAllScaJobView = "", string isShowOnlyAssignJobsSCO = "", string JobID = "", int? isSPVRequired = null, int? isSPVInstallationVerified = null);

        /// <summary>
        /// Updates the settlement date.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="settlementDate">The settlement date.</param>
        void UpdateSettlementDate(int jobId, DateTime settlementDate);

        /// <summary>
        /// Updates the PVDSWH code.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="pvdSWHCode">The PVDSWH code.</param>
        void UpdatePVDSWHCode(int jobId, string pvdSWHCode);

        /// <summary>
        /// Update Compliance Note
        /// </summary>
        /// <param name="stcJobdetailsId"></param>
        /// <param name="CompalinceNote"></param>
        void UpdateComplianceNote(int stcJobdetailsId, string CompalinceNote);

        /// <summary>
        /// Update Compliance Note
        /// </summary>
        /// <param name="stcJobdetailsId"></param>
        void UpdateComplianceNoteById(string stcJobdetailsId);


        /// <summary>
        /// Gets the STC job history.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <returns>job history</returns>
        List<STCJobHistory> GetSTCJobHistory(int userId, int jobId, int userTypeId);

        /// <summary>
        /// Gets the STC submission count.
        /// </summary>
        /// <param name="jobid">The job identifier.</param>
        /// <returns>general class</returns>
        List<GeneralClass> GetStcSubmissionCount(int jobid);

        /// <summary>
        /// Updates the module wattage.
        /// </summary>
        /// <param name="pvModuleId">The module identifier.</param>
        /// <param name="wattage">The wattage.</param>
        void UpdatePVModuleWaltage(int pvModuleId, int wattage);

        /// <summary>
        /// Gets the job stages with count.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="ResellerId">ResellerId.</param>
        /// <param name="RamId">RamId.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns>job stage</returns>
        List<JobStage> GetSTCJobStagesWithCount(int userId, int userTypeId, int ResellerId, int RamId, int solarCompanyId, string isAllScaJobView = "", string isShowOnlyAssignJobsSCO = "");

        /// <summary>
        /// Checks the business rules.
        /// </summary>
        /// <param name="createJob">The create job.</param>
        /// <param name="xmlPanels">The XML panels.</param>
        /// <param name="xmlInverters">The XML inverters.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <returns>data set</returns>
        DataSet CheckBusinessRules(CreateJob createJob, string xmlPanels, string xmlInverters, int? jobId = 0);

        /// <summary>
        /// Inserts the job compliance.
        /// </summary>
        /// <param name="stcComplianceCheck">The STC compliance check.</param>
        /// <returns>integer value</returns>
        DataSet InsertSTCJobCompliance(StcComplianceCheck stcComplianceCheck);

        /// <summary>
        /// Gets the job compliance.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="stcJobDetailsID">The job details identifier.</param>
        /// <returns>complaince check</returns>
        StcComplianceCheck GetSTCJobCompliance(int jobId, int stcJobDetailsID);

        /// <summary>
        /// Gets the STC compliance details.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="jobDetailsId">The job details identifier.</param>
        /// <param name="changedBy">The changed by.</param>
        /// <returns>data set</returns>
        DataSet GetStcComplianceDetails(int jobId, int jobDetailsId, int changedBy);

        /// <summary>
        /// Gets the STC sub records for job.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <returns>job list</returns>
        List<JobList> GetSTCSubRecordsForJob(int jobId);

        /// <summary>
        /// Accepts the reject job to SSC.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="role">The role.</param>
        void AcceptRejectJobToSSC(int jobId, string role);

        /// <summary>
        /// Gets the STC document by STC job details identifier.
        /// </summary>
        /// <param name="stcJobDetailsID">The STC job details identifier.</param>
        /// <returns>user list</returns>
        List<UserDocument> GetSTCDocumentBySTCJobDetailsID(int stcJobDetailsID);

        /// <summary>
        /// Cancels the removal request.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        void CancelRemovalRequest(int jobID);

        /// <summary>
        /// Assigns the job to fco.
        /// </summary>
        /// <param name="complianceOfficerId">The compliance officer identifier.</param>
        /// <param name="jobs">The jobs.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="createdDate">The created date.</param>
        DataSet AssignJobToFCO(int complianceOfficerId, string jobs, int userId, DateTime createdDate);

        /// <summary>
        /// Gets the bult upload for job.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>data set</returns>
        DataSet GetBultUploadForJob(string jobID);

        /// <summary>
        /// Gets the hw panel.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="certificateHolder">The certificate holder.</param>
        /// <param name="jobType">Type of the job.</param>
        /// <returns>brand model</returns>
        List<HWBrandModel> GetHWPanel(string mode, string certificateHolder, string jobType);

        /// <summary>
        /// Inserts the photo for API.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="status">The status.</param>
        /// <param name="userID">The user identifier.</param>
        void InsertPhotoForAPI(int jobID, string filename, int status, int userID, string VendorJobPhotoId);

        /// <summary>
        /// Gets the nmi by job identifier.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>nmi</returns>
        string GetNMIByJobID(int jobID);

        /// <summary>
        /// Gets the jobs to assign message.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userTypeId">The user type identifier.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns>job list</returns>
        List<JobList> GetJobsToAssignMessage(int userId, int userTypeId, string searchText);

        /// <summary>
        /// Gets the bulk upload for job.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>data set</returns>
        DataSet GetBulkUploadForJob(string jobID);

        /// <summary>
        /// Gets the SWH bulk upload for job.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>data set</returns>
        DataSet GetSWHBulkUploadForJob(string jobID);

        /// <summary>
        /// Gets the job notes list for API.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>data set</returns>
        DataSet GetJobNotesListForAPI(int jobID, int jobSchedulingId);

        /// <summary>
        /// Gets the electrician by solar company identifier.
        /// </summary>
        /// <param name="solarCompanyID">The solar company identifier.</param>
        /// <param name="isFullDetail">if set to <c>true</c> [is full detail].</param>
        /// <param name="electricianID">The electrician identifier.</param>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>DataSet</returns>
        DataSet GetElectricianBySolarCompanyID(int solarCompanyID, bool isFullDetail, int electricianID, int jobID, int jobType = 1);

        /// <summary>
        /// Gets the sold by solar company identifier.
        /// </summary>
        /// <param name="solarCompanyID">The solar company identifier.</param>
        /// <returns>DataSet</returns>
        DataSet GetSoldBySolarCompanyID(int solarCompanyID);

        /// <summary>
        /// Gets the distributor.
        /// </summary>
        /// <returns>dataset</returns>
        DataSet GetDistributor();

        /// <summary>
        /// Updates the serial number.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="serialNumbers">The serial numbers.</param>
        void UpdateSerialNumber(int jobID, string serialNumbers);

        /// <summary>
        /// Updates the check list serial number.
        /// </summary>
        /// <param name="VisitCheckListItemId">The visit check list item identifier.</param>
        /// <param name="CheckListSerialNumbers">The check list serial numbers.</param>
        /// <param name="UserId">The user identifier.</param>
        /// <returns></returns>
        string UpdateCheckListSerialNumber(int VisitCheckListItemId, string CheckListSerialNumbers, int UserId, DataTable dtSPVSerialNumber);

        /// <summary>
        /// Updates the signature.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="signatureName">Name of the signature.</param>
        void UpdateSignature(int jobID, string signatureName);

        /// <summary>
        /// Get Email for PvsSign;
        /// </summary>
        /// <param name="userId">user identifier.</param>
        /// <returns>string</returns>
        DataSet GetEmailforPvsSign(int userId);

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
        /// <returns>string</returns>
        string GetJobOwnerSignatureForAPI(int jobId, string ownerSignature, string Latitude, string Longitude, string IpAddress, string Location, DateTime SignatureDate);

        /// <summary>
        /// Checks the serial nnumbers.
        /// </summary>
        /// <param name="serialNumbers">The serial numbers.</param>
        /// <param name="solarCOmpanyID">The solar c ompany identifier.</param>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>DataSet</returns>
        DataSet CheckSerialNnumbers(string serialNumbers, int solarCOmpanyID, int jobID);

        /// <summary>
        /// Deletes the check list photos.
        /// </summary>
        /// <param name="chkIds">The CHK ids.</param>
        /// <param name="sigIds">The sig ids.</param>
        /// <param name="JobSchedulingIds">The job scheduling ids.</param>
        /// <returns></returns>
        DataSet DeleteCheckListPhotos(string chkIds, string sigIds, string JobSchedulingIds, int jobId = 0);

        /// <summary>
        /// Gets the job list for trade STC pop up.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="SortCol">The sort col.</param>
        /// <param name="SortDir">The sort dir.</param>
        /// <param name="jobIDS">The job ids.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns>price list</returns>
        List<FormBot.Entity.PricingManager> GetJobListForTradeSTCPopUp(int UserId, int UserTypeId, string SortCol, string SortDir, string jobIDS, int solarCompanyId);

        /// <summary>
        /// Gets the pre approval status.
        /// </summary>
        /// <returns>dataset</returns>
        DataSet GetPreApprovalStatus();

        /// <summary>
        /// Gets the connection status.
        /// </summary>
        /// <returns>dataset</returns>
        DataSet GetConnectionStatus();


        /// <summary>
        /// Get JobStatus Common Details.
        /// </summary>
        /// <returns>dataset</returns>
        DataSet GetJobStatusCommonDetails(int userID, bool defaultGrid = false);

        /// <summary>
        /// Gets the failure reason by job identifier.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <returns>DataSet</returns>
        DataSet GetFailureReasonByJobId(int JobId);

        /// <summary>
        /// Gets the job stages.
        /// </summary>
        /// <returns>job list</returns>
        List<JobList> GetJobStages();

        /// <summary>
        /// Bulks the change job stage.
        /// </summary>
        /// <param name="JobStageID">The job stage identifier.</param>
        /// <param name="JobIDs">The job i ds.</param>
        void BulkChangeJobStage(int JobStageID, string JobIDs);

        /// <summary>
        /// Getjobs the identifier bystc invoice number.
        /// </summary>
        /// <param name="stcInvoiceNumber">The STC invoice number.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="resellerId">The reseller identifier.</param>
        /// <returns>DataSet</returns>
        DataSet GetjobIdBystcInvoiceNumber(string stcInvoiceNumber, int UserTypeId, int resellerId);

        /// <summary>
        /// Gets the delete email details by job identifier.
        /// </summary>
        /// <param name="JobID">The job identifier.</param>
        /// <returns>dataset</returns>
        DataSet GetDeleteEmailDetailsByJobId(int JobID);

        /// <summary>
        /// Gets the email details by job identifier.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <returns>dataset</returns>
        DataSet GetEmailDetailsByJobID(int jobID);

        /// <summary>
        /// Bulks the change STC job stage.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="STCJobStageID">The STC job stage identifier.</param>
        /// <param name="STCJobIDs">The STC job i ds.</param>
        /// <param name="CreatedDate">The created date.</param>
        /// <returns></returns>
        DataSet BulkChangeSTCJobStage(int UserId, int UserTypeId, int STCJobStageID, string STCJobIDs, DateTime CreatedDate);

        /// <summary>
        /// Gets the details of compliance issue job for mail.
        /// </summary>
        /// <param name="STCJobDetailsId">The STC job details identifier.</param>
        /// <returns></returns>
        DataSet GetDetailsOfComplianceIssueJobForMail(int STCJobDetailsId);

        /// <summary>
        /// Gets the details of cer failed job for mail.
        /// </summary>
        /// <param name="STCJobDetailsId">The STC job details identifier.</param>
        /// <returns></returns>
        DataSet GetDetailsOfCERFailedJobForMail(int STCJobDetailsId);

        /// <summary>
        /// job Print
        /// </summary>
        /// <param name="jobId">jobId</param>
        /// <returns>DataSet</returns>
        DataSet JobPrint(int jobId, int userTypeId);

        /// <summary>
        /// Get AllCompany To Make Registered With GST
        /// </summary>
        /// <returns>list of string</returns>
        List<SolarCompany> GetAllCompanyToMakeRegisteredWithGST();

        /// <summary>
        /// Make SCA Registered With GST
        /// </summary>
        /// <param name="dtSCAWithGST"></param>
        void MakeSCARegisteredWithGST(DataTable dtSCAWithGST);

        /// <summary>
        /// Get Job Field Data
        /// </summary>
        /// <returns>JobField</returns>
        List<JobField> GetJobFieldData();

        /// <summary>
        /// Gets the job notes list
        /// </summary>
        /// <param name="jobID">jobID</param>
        /// <returns>DataSet</returns>
        List<JobNotes> GetJobNotesListOnVisit(int jobID);

        /// <summary>
        /// Jobs the notes mark as seen.
        /// </summary>
        /// <param name="jobNoteIds">The job note ids.</param>
        void JobNotesMarkAsSeen(string jobNoteIds);

        /// <summary>
        /// Insert VisitSignature
        /// </summary>
        /// <param name="dtSCAWithGST"></param>
        void InsertVisitSignatureAPI(int VisitSignatureId, int VisitCheckListItemId, int JobSchedulingId, int JobId, string Path, int SignatureTypeId, int UserID, string Latitude, string Longitude, string IpAddress, string Location, string Image);

        /// <summary>
        /// Insert ErrorLog
        /// </summary>
        /// <param name="dtSCAWithGST"></param>
        void InsertErrorLogAPI(string Error, int UserId, string DeviceToken, DateTime DateTime);

        /// <summary>
        /// Get FolderName
        /// </summary>
        /// <param name="CheckListItemId"></param>
        string GetFolderName(int CheckListItemId);

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
        List<FormBot.Entity.PricingManager> GetJobListForCERApprovedJobs(int UserId, int UserTypeId, string SortCol, string SortDir, string jobIDS, int solarCompanyId, int isApproved);

        /// <summary>
        /// Get FolderName
        /// </summary>
        /// <param name="id"></param>
        DataSet GetVisitSignature(int Id);

        /// <summary>
        /// Gets the visit signature API.
        /// </summary>
        /// <param name="JobSchedulingId">The job scheduling identifier.</param>
        /// <param name="VisitCheckListItemId">The visit check list item identifier.</param>
        /// <returns></returns>
        DataSet GetVisitSignatureApi(int JobSchedulingId, int VisitCheckListItemId);

        /// <summary>
        /// Get VisitSignature Path
        /// </summary>
        /// <param name="Id"></param>
        string GetVisitSignaturePath(int JobSchedulingId, int CheckListItemId, int SignatureTypeId);

        //int JobInstallerDesignerElectricians_InsertUpdate(int jobId, int profileType, string signPath, JobElectricians jobElectricians, JobInstallerDetails jobInstallerDetails);
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
        int JobInstallerDesignerElectricians_InsertUpdate(int jobId, int profileType, string signPath, JobElectricians jobElectricians, JobInstallerDetails jobInstallerDetails, int LoggedInUserId);

        /// <summary>
        /// Updates the job signature.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="signPath">The sign path.</param>
        /// <param name="typeOfSignature">The type of signature.</param>
        void UpdateJobSignature(int jobId, string signPath, int typeOfSignature);

        /// <summary>
        /// Gets the default submission signature by job identifier.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="typeOfSignature">The type of signature.</param>
        /// <returns></returns>
        string GetDefaultSubmissionSignatureByJobId(int jobId, int typeOfSignature);

        /// <summary>
        /// Inserts the job notes.
        /// </summary>
        /// <param name="notes">The notes.</param>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="createdBy">The created by.</param>
        /// <param name="jobSchedulingId">The job scheduling identifier.</param>
        /// <returns></returns>
        int InsertJobNotes(string notes, int jobID, int createdBy, int jobSchedulingId);

        /// <summary>
        /// Gets the serial number.
        /// </summary>
        /// <param name="VisitCheckListItemId">The visit check list item identifier.</param>
        /// <returns></returns>
        string GetSerialNumber(int VisitCheckListItemId);

        /// <summary>
        /// Gets the user signature path.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="FileName">Name of the file.</param>
        /// <returns></returns>
        string GetUserSignaturePath(int UserId, string FileName = "");

        /// <summary>
        /// Gets the checklist photos.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <returns></returns>
        DataSet GetChecklistPhotos(int JobId);

        /// <summary>
        /// Gets the checklist items.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <param name="jobSchedulingId">The job scheduling identifier.</param>
        /// <returns></returns>
        DataSet GetChecklistItems(int JobId, int jobSchedulingId);

        /// <summary>
        /// Gets the serial number of job.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <returns></returns>
        string GetSerialNumberOfJob(int jobId);

        /// <summary>
        /// Makes the visit check list item completed.
        /// </summary>
        /// <param name="visitCheckListItemId">The visit check list item identifier.</param>
        /// <param name="isCompleted">if set to <c>true</c> [is completed].</param>
        void MakeVisitCheckListItemCompleted(int visitCheckListItemId, bool isCompleted);

        /// <summary>
        /// Updates the custom job field.
        /// </summary>
        /// <param name="dtCustomField">The dt custom field.</param>
        void UpdateCustomJobField(DataTable dtCustomField);

        /// <summary>
        /// Gets the job custom fields.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <param name="isPopup">if set to <c>true</c> [is popup].</param>
        /// <returns></returns>
        List<CustomField> GetJobCustomFields(int JobId, bool isPopup);

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
        void UpdateInstallerDesignerEleSignature(int jobId, string signature, string Latitude, string Longitude, string IpAddress, string Location, DateTime SignatureDate, string path, int typeOfSignature);

        /// <summary>
        /// Updates the basic detail.
        /// </summary>
        /// <param name="obj">The object.</param>
        void UpdateBasicDetail(BasicDetails obj);

        ///// <summary>
        ///// Updates the owner detail.
        ///// </summary>
        ///// <param name="obj">The object.</param>
        //void UpdateOwnerDetail(JobOwnerDetails obj);

        /// <summary>
        /// Updates the installation detail tabular.
        /// </summary>
        /// <param name="obj">The object.</param>
        void UpdateInstallationDetailTabular(JobInstallationDetails obj);
        /// <summary>
        /// Updates the installation detail.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="dtCustomField">The dt custom field.</param>
        void UpdateInstallationDetail(JobInstallationDetails obj, DataTable dtCustomField = null);
        /// <summary>
        /// Updates the custom detail.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <param name="dtCustomField">The dt custom field.</param>
        void UpdateCustomDetail(string JobId, DataTable dtCustomField = null);

        /// <summary>
        /// Updates the STC detail.
        /// </summary>
        /// <param name="obj">The object.</param>
        void UpdateStcDetail(StcObject obj, int JobType);

        /// <summary>
        /// Updates the system detail.
        /// </summary>
        /// <param name="jobSystemDetails">The job system details.</param>
        DataSet UpdateSystemDetail(JobSystemDetails jobSystemDetails);

        /// <summary>
        /// Updates the serial no detail.
        /// </summary>
        /// <param name="jobSystemDetails">The job system details.</param>
        /// <returns></returns>
        DataSet UpdateSerialNoDetail(JobSystemDetails jobSystemDetails);

        /// <summary>
        /// Updates the GST detail.
        /// </summary>
        /// <param name="isGST">if set to <c>true</c> [is GST].</param>
        /// <param name="FileName">Name of the file.</param>
        /// <param name="jobId">The job identifier.</param>
        void UpdateGstDetail(bool isGST, string FileName, int jobId);

        /// <summary>
        /// Gets the ces photos by visit check list identifier.
        /// </summary>
        /// <param name="VisitCheckListItemId">The visit check list item identifier.</param>
        /// <returns></returns>
        DataSet GetCesPhotosByVisitCheckListId(int VisitCheckListItemId);
        DataSet GetAllPhotosByJobSchedulingId(int JobSchedulingId, string VisitChecklistItemIds, bool IsReference, bool IsDefault, string ClassType, int JobId);

        /// <summary>
        /// Deletes the ces PDF.
        /// </summary>
        /// <param name="VisitCheckListItemId">The visit check list item identifier.</param>
        /// <returns></returns>
        DataSet DeleteCesPdf(int VisitCheckListItemId);

        /// <summary>
        /// Gets the serial no.
        /// </summary>
        /// <param name="VisitCheckListItemId">The visit check list item identifier.</param>
        /// <returns></returns>
        DataSet GetSerialNo(int VisitCheckListItemId);

        /// <summary>
        /// Gets the check list item for trade.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <returns></returns>
        List<CheckListItem> GetCheckListItemForTrade(int jobId);

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="cdata">The cdata.</param>
        /// <returns></returns>
        DataSet GetData(List<CommonData> cdata);

        /// <summary>
        /// Gets the job custom details.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <returns></returns>
        List<CustomDetail> GetJobCustomDetails(int JobId, int SolarCompanyId);

        /// <summary>
        /// Fills the PDF and save.
        /// </summary>
        /// <param name="lstPdfItems">The LST PDF items.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="isFill">if set to <c>true</c> [is fill].</param>
        /// <param name="lstSignature">The LST signature.</param>
        void FillPDFAndSave(List<PdfItems> lstPdfItems, string fileName, bool isFill = false, List<KeyValuePair<int, string>> lstSignature = null);

        /// <summary>
        /// Gets the photos path.
        /// </summary>
        /// <param name="VisitCheckListPhotoIds">The visit check list photo ids.</param>
        /// <param name="VisitSignatureIds">The visit signature ids.</param>
        /// <returns></returns>
        DataSet GetPhotosPath(string VisitCheckListPhotoIds, string VisitSignatureIds);

        //get all photos for tabular view
        DataSet GetPhotosForAllTabular(int JobId);

        /// <summary>
        /// Gets the required detail to set STC value.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <returns></returns>
        DataSet GetRequiredDetailToSetSTCValue(int jobId);

        /// <summary>
        /// Deletes the job images vendor API.
        /// </summary>
        /// <param name="VendorJobPhotoId">The vendor job photo identifier.</param>
        /// <param name="IsClassic">if set to <c>true</c> [is classic].</param>
        /// <returns></returns>
        DataSet DeleteJobImages_VendorApi(string VendorJobPhotoId, bool IsClassic);

        /// <summary>
        /// Deletes the documents vendor API.
        /// </summary>
        /// <param name="VendorJobDocumentId">The vendor job document identifier.</param>
        /// <param name="IsClassic">if set to <c>true</c> [is classic].</param>
        /// <returns></returns>
        DataSet DeleteDocuments_VendorApi(string VendorJobDocumentId, bool IsClassic);

        /// <summary>
        /// Moves the deleted documents vendor API.
        /// </summary>
        /// <param name="VendorJobDocumentId">The vendor job document identifier.</param>
        /// <param name="Path">The path.</param>
        void MoveDeletedDocuments_VendorApi(string VendorJobDocumentId, string Path);

        /// <summary>
        /// Gets the job identifier by vendorjob identifier.
        /// </summary>
        /// <param name="VendorJobId">The vendor job identifier.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <returns></returns>
        DataTable GetJobIdByVendorjobId(string VendorJobId, int SolarCompanyId);

        /// <summary>
        /// Checks the panel inverter system brand vendor API.
        /// </summary>
        /// <param name="JobType">Type of the job.</param>
        /// <param name="dtPanelSystemBrand">The dt panel system brand.</param>
        /// <param name="dtInverterDetail">The dt inverter detail.</param>
        /// <param name="JobId">JobId.</param>
        /// <returns></returns>
        DataSet CheckPanelInverterSystemBrand_VendorApi(int JobType, DataTable dtPanelSystemBrand, DataTable dtInverterDetail, int JobId = 0);

        /// <summary>
        /// Checks the vendor job photo identifier vendor API.
        /// </summary>
        /// <param name="VendorJobPhotoId">The vendor job photo identifier.</param>
        /// <param name="JobId">The job identifier.</param>
        /// <param name="IsClassic">if set to <c>true</c> [is classic].</param>
        /// <returns></returns>
        string CheckVendorJobPhotoId_VendorApi(string VendorJobPhotoId, int JobId, bool IsClassic);

        /// <summary>
        /// Checks the vendor job document identifier vendor API.
        /// </summary>
        /// <param name="VendorJobDocumentId">The vendor job document identifier.</param>
        /// <param name="JobId">The job identifier.</param>
        /// <param name="IsClassic">if set to <c>true</c> [is classic].</param>
        /// <returns></returns>
        string CheckVendorJobDocumentId_VendorApi(string VendorJobDocumentId, int JobId, bool IsClassic);

        /// <summary>
        /// Gets the custom field details.
        /// </summary>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <returns></returns>
        List<VendorCustomField> GetCustomFieldDetails(int SolarCompanyId);

        /// <summary>
        /// Gets the job list user wise columns.
        /// </summary>
        /// <param name="JobViewMenuId">The job view menu identifier.</param>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="PageNumber">The page number.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <param name="SortCol">The sort col.</param>
        /// <param name="SortDirection">The sort direction.</param>
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
        DataSet GetJobList_UserWiseColumns(int JobViewMenuId, int UserId, int UserTypeId, int PageNumber, int PageSize, string SortCol, string SortDirection, int UrgentJobDay, int StageId, int SolarCompanyId, bool IsArchive, int ScheduleType, int JobType, int JobPriority, string searchtext, DateTime? FromDate, DateTime? ToDate, bool IsGst, bool jobref, bool jobdescription, bool jobaddress, bool jobclient, bool jobstaff, bool Invoiced, bool NotInvoiced, bool ReadyToTrade, bool NotReadyToTrade, bool traded, bool nottraded, bool preapprovalnotapproved, bool preapprovalapproved, bool connectioncompleted, bool connectionnotcompleted, bool ACT, bool NSW, bool NT, bool QLD, bool SA, bool TAS, bool WA, bool VIC, int PreApprovalStatusId, int ConnectionStatusId);

        /// <summary>
        /// Gets the job list for caching data.
        /// </summary>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="JobViewMenuId">The job view menu identifier.</param>
        /// <returns></returns>
        DataSet GetJobList_ForCachingData(string SolarCompanyId, int UserId, int JobViewMenuId);

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
        /// <param name="filter"></param>
        /// <returns></returns>
        JobViewLists GetJobList_ForCachingDataKendoByYearWithoutCacheDapper(string SolarCompanyId, int UserId, int JobViewMenuId, int year, string strYear = "", int page = 1, int pageSize = 10, DataTable filter = null, DataTable dtSort = null);


        /// <summary>
        /// Gets the column master.
        /// </summary>
        /// <param name="JobViewMenuId">The job view menu identifier.</param>
        /// <returns></returns>
        List<ColumnMaster> GetColumnMaster(int JobViewMenuId);

        /// <summary>
        /// Gets the user wise columns.
        /// </summary>
        /// <param name="UserID">The user identifier.</param>
        /// <param name="JobViewMenuId">The job view menu identifier.</param>
        /// <returns></returns>
        List<UserWiseColumns> GetUserWiseColumns(int UserID, int JobViewMenuId);

        /// <summary>
        /// Saves the user wise columns.
        /// </summary>
        /// <param name="dtColumns">The dt columns.</param>
        /// <param name="UserID">The user identifier.</param>
        /// <param name="JobViewMenuId">The job view menu identifier.</param>
        void SaveUserWiseColumns(DataTable dtColumns, int UserID, int JobViewMenuId);

        /// <summary>
        /// Resets the default columns.
        /// </summary>
        /// <param name="UserID">The user identifier.</param>
        /// <param name="JobViewMenuId">The job view menu identifier.</param>
        /// <returns></returns>
        List<UserWiseColumns> ResetDefaultColumns(int UserID, int JobViewMenuId);

        /// <summary>
        /// Gets the advance search category.
        /// </summary>
        /// <returns></returns>
        DataSet GetAdvanceSearchCategory();




        /// <summary>
        /// Gets the job identifier wise caching data.
        /// </summary>
        /// <param name="JobID">The job identifier.</param>
        /// <returns></returns>
        DataRow GetJobIDWiseCachingData(string JobID);


        /// <summary>
        /// Get STC Sumbmission Data From STCJobDetailsId 
        /// </summary>
        /// <param name="STCJobDetailsId"></param>
        /// <returns></returns>
        DataRow GetSTCJobDetailsIDWiseCachingData(int STCJobDetailsId);


        /// <summary>
        /// Get STC Invoice wise caching data
        /// </summary>
        /// <param name="STCJobDetailsId"></param>
        /// <returns></returns>
        DataRow GetSTCInvoiceIDWiseCachingData(int STCInvoiceId);
        /// <summary>
        /// Gets the STC submission list vendor API.
        /// </summary>
        /// <param name="VendorJobId">The vendor job identifier.</param>
        /// <returns></returns>
        List<FormBot.Entity.Job.STCBasicDetails.STCSubmissionModel> GetStcSubmissionList_VendorAPI(string VendorJobId);

        /// <summary>
        /// Gets the job photo vendor API.
        /// </summary>
        /// <param name="VendorJobId">The vendor job identifier.</param>
        /// <returns></returns>
        List<VendorJobPhotoList> GetJobPhoto_VendorAPI(string VendorJobId);

        DataSet InsertUserSignature(string jobDocId, string CaptureUserSignId, string signstring, bool IsImage, string fieldName = "", string mobileNo = "", string Firstname = null, string Lastname = null, int SignatureType = 0, string Email = "");

        DataSet InsertUserSignatureApi(string jobDocId, string signstring, bool IsImage, string fieldName, string mobileNo, string Firstname, string Lastname, int SignatureType, string Email);

        void FillSignature(PdfItems item, List<KeyValuePair<int, string>> lstSignature, bool isFill, AcroFields pdfFormFields, PdfStamper pdfStamper);

        int CheckPreviousSmsTime(string mobileNo);

        DataSet GetCaptureUserSignDetail(string jobDocId, string fieldname);

        /// <summary>
        /// Checks the existing custom electrician.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        /// <returns></returns>
        bool CheckExistingCustomElectrician(string fullName, int solarCompanyId, int jobId);

        /// <summary>
        /// JobElectricians_InsertUpdate
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="profileType"></param>
        /// <param name="jobElectricians"></param>
        /// <param name="LoggedInUserId"></param>
        /// <param name="isCreateNew"></param>
        /// <returns></returns>
        int JobElectricians_InsertUpdate(int jobId, int profileType, JobElectricians jobElectricians, int LoggedInUserId, bool isCreateNew);

        /// <summary>
        /// Update JobStatus FromREC
        /// </summary>
        /// <param name="resellerId"></param>
        /// <param name="isSwitch"></param>
        /// <param name="isAutoUpdateCEROn"></param>
        /// <returns></returns>
        DataTable UpdateJobStatusFromREC(int resellerId, bool isSwitch, bool isAutoUpdateCEROn);

        /// <summary>
        /// Update Details After RECInsertion
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="resellerId"></param>
        /// <returns></returns>
        DataSet UpdateDetailsAfterRECInsertion(DataTable dt, int resellerId);

        /// <summary>
        /// Get UpdateJobStatusFromRec By ResellerId
        /// </summary>
        /// <param name="resellerId"></param>
        /// <returns></returns>
        bool GetUpdateJobStatusFromRecByResellerId(int resellerId);

        /// <summary>
        /// Get JobDocument By JobDocumentId
        /// </summary>
        /// <param name="jobDocumentId"></param>
        /// <returns></returns>
        DocumentsView GetJobDocumentByJobDocumentId(int jobDocumentId);

        /// <summary>
        /// Update Job Owner Details
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="jobOwnerDetails"></param>
        void UpdateJobOwnerDetails(int jobId, JobOwnerDetails jobOwnerDetails, bool isGST);

        /// <summary>
        /// Update JobInstallationPropertyType
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="propertyType"></param>
        void UpdateJobInstallationPropertyType(int jobId, string propertyType, bool isGST);

        /// <summary>
        /// Upload GstDocument For Job
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="gstDocument"></param>
        void UploadGstDocumentForJob(int jobId, string gstDocument);

        /// <summary>
        /// Delete GstDocument For Job
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="gstDocument"></param>
        void DeleteGstDocumentForJob(int jobId);



        /// <summary>
        /// Check special char in serial numbers
        /// </summary>
        /// <param name="JobId"></param>
        /// <returns></returns>
        string CheckSpecialCharInSerialNumbers(string JobId);

        DataSet Job_InsertBulkUploadSolarJobs(DataTable dtBulkUploadPVDSolarJobs, DataTable dtBulkUploadSWHSolarJobs, int solarCompanyId, string JobScheduleHistoryMsg);

        JobSTCDetails JobSTCDetailsBusinessRules(string installingNewPanel, JobSTCDetails jobSTCDetails);

        DataSet GetJobList_ForCachingDataKendo(string SolarCompanyId, int UserId, int JobViewMenuId);

        /// <summary>
        /// Get StcJobDetailsId List ForCachingData
        /// </summary>
        /// <param name="ResellerId"></param>
        /// <param name="SolarCompanyId"></param>
        /// <param name="jobId"></param>
        /// <returns></returns>
        DataSet GetStcJobDetailsIdList_ForCachingData(int ResellerId, int SolarCompanyId, int jobId);

        DataSet JobDocument_GetJobDocumentByDocuementPathJobId(int JobId, string JobDocumentPath);

        /// <summary>
        /// Load Common Jobs With Same Installation Date And Installer
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="installerId"></param>
        /// <param name="installationDate"></param>
        /// <returns>list of jobs</returns>
        //List<CommonJobsWithSameInstallDateAndInstaller> LoadCommonJobs_SameInstallDateAndInstaller(int jobId, int installerId, string installationDate);
        DataSet LoadCommonJobs_SameInstallDateAndInstaller(int jobId, int installerId, string installationDate);
        /// <summary>
        /// Load Common Jobs With Same Installation Address
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="usertypeid"></param>
        /// <returns>list of jobs</returns>
        DataSet CommonJobsWithSameInstallationAddress(int jobId);
        /// <summary>
        /// Get AllCompany To Make Registered With GST From ResellerId
        /// </summary>
        /// <returns>list of string</returns>
        List<SolarCompany> GetAllCompanyToMakeRegisteredWithGSTFromResellerId(int resellerId);


        string UploadPVDCode(DataTable dt);

        DataSet GetJobsForRecInsertOrUpdate(string JobIds);

        DataSet GetJobsForRecInsertOrUpdateNew(string JobIds);

        DataSet InsertGBBatchRECUploadId(DataTable dt, int resellerId, string datetimeticks = "", decimal TotalSTC = 0, int CreatedBy = 0, string CERLoginId = "", string CERPassword = "", string RECAccName = "", string LoginType = "", string RECCompanyName = "");

        /// <summary>
        /// remove batchid from selected jobs
        /// </summary>
        /// <param name="StcJobdetailsId"></param>
        void RemoveJobFromBatch(string StcJobdetailsId);

        /// <summary>
        /// Get All the job serial numbers and verification url for spv
        /// </summary>
        /// <param name="JobPanelId"></param>
        /// <returns></returns>
        DataSet GetSPVVerificationUrlSerialNumber(int JobId = 0, bool reProductVerification = false);

        /// <summary>
        /// Get All the job serial numbers and verification url for spv API
        /// </summary>
        /// <param name="JobPanelId"></param>
        /// <returns></returns>
        DataSet GetSPVVerificationUrlSerialNumberAPI(string Manufacturer, string ModelNumber, string commaSerialnumber, int JobId);


        /// <summary>
        /// Get all the data for installation verification
        /// </summary>
        /// <param name="STCJobDetailsId"></param>
        /// <returns></returns>
        DataSet GetSPVInstallationVerificationUrlSerialNumber(int STCJobDetailsId, bool ReVerify = false);

        /// <summary>
        /// Update Installtion verification failed or pass in stc submission
        /// </summary>
        /// <param name="StcJobDetailsId"></param>
        /// <param name="Status"></param>
        void UpdateInstallationVerificationStatus(int StcJobDetailsId, bool? Status);


        /// <summary>
        /// Update serialnumber is verified or not in product verification SPV
        /// </summary>
        /// <param name="VerifiedSerialNumber"></param>
        /// <param name="JobId"></param>
        List<JobSerialNumbers> UpdateVerifiedSerialNumber(DataTable VerifiedSerialNumber, int JobId);
        DataSet GetJobPanelDetails(int JobId);
        DataSet GetBulkJobPanelDetails(string JobId);
        /// <summary>
        /// Update serialnumber portal side
        /// </summary>
        /// <param name="Jobid"></param>
        /// <param name="SerialNumber"></param>
        void UpdateJobSerialNumber(int Jobid, string SerialNumber);

        /// <summary>
        /// Get job basic detail
        /// </summary>
        /// <param name="JobId"></param>
        /// <returns></returns>
        JobBasicDetail GetJobBasicDetail(int JobId);

        /// <summary>
        /// Release SPV
        /// </summary>
        /// <param name="JobID"></param>
        void SpvRelease(int JobID, int STCJobDetailsID);
        DataSet GetDocumentsForGeneratingRecZip(int jobId);

        DataSet InsertDelete_RecZip_Document(int jobId, int userId, string path, bool isGenerateRecZip);

        /// <summary>
        /// Reset SPV
        /// </summary>
        /// <param name="JobID"></param>
        void SpvReset(int JobID);
        CheckSPVRequiredByJobId CheckSPVRequiredByJobId(int JobId);
        /// <summary>
        /// This service will provide you that, solar company is given to allow SPV flag on or off by SolarCompanyIds or JobIds.
        /// </summary>
        /// <param name="SolarCompanyIds"></param>
        /// <param name="JobIds"></param>
        /// <returns></returns>
        //List<SpvRequiredSolarCompanyWise> GetSPVRequiredOrNotOnSCOrGlobalLevel(string SolarCompanyIds = null, string JobIds = null,string StcJobIds = null);
        List<CheckSPVrequired> GetSPVRequiredOrNotOnSCAOrGlobalLevelOrManufacturer(string JobIds = null, string Manufacturer = null, string Model = null);

        /// <summary>
        /// Update IsSpvVerified flag in job when lat-long getting null from app for upload job photo
        /// </summary>
        /// <param name="JobId"></param>
        /// <returns></returns>
        DataSet UpdateIsSpvVerifiedWhenLatLongNullForUploadPhoto(int JobId);

        List<GetPhotoFromPortalWithNullLatLongIsdeletedFlagApiRequest> GetPhotoFromPortalWithNullLatLongIsdeletedFlag(string VisitChecklistPhotoIds);
        List<SpvLog> GetSPVProductVerificationLogByJobId(int JobId);

        /// <summary>
        /// Set SPV on lock serialnumber
        /// </summary>
        /// <param name="jobId"></param>
        bool SetSPVOnLockSerialNumber(int jobId);

        /// <summary>
        /// Remove SPV on unlock serialnumber
        /// </summary>
        /// <param name="jobId"></param>
        void RemoveSPVOnUnlockSerialnumber(int jobId);
        bool CheckDocumentRightsFromJobStcStatus(int JobId);
        /// <summary>
        /// get serial number with latest spv verify status
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        List<JobSerialNumbers> GetSerialnumberWithSpvStatus(int JobId);
        ///// <summary>
        ///// get latest stcjobdetails Id from jobid
        ///// </summary>
        ///// <param name="jobId"></param>
        //DataTable GetStcJobDetailsDataFromJobId(int jobId);

        /// <summary>
        /// get reseller id solar company id from jobid or stcjobdetailid
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="stcJobDetailId"></param>
        DataTable GetResellerSolarCompnayFromJobIdOrStcJobdetailId(int? jobid, int? stcJobDetailId);
        /// <summary>
        /// get User name from user id
        /// </summary>
        /// <param name="userId"></param>
        DataSet GetUserNameSolarCompanyForCache(int userId, int usertypeId);

        /// <summary>
        /// get invoice count from stcjobdetailsids
        /// </summary>
        /// <param name="stcJobdetailId"></param>
        DataTable GetInvoiceCount(string stcJobdetailIds);
        /// <summary>
        /// get STCJObdetailData and job data 
        /// </summary>
        /// <param name="stcJobdetailIds"></param>
        /// <param name="jobids"></param>
        DataTable GetSTCDetailsAndJobDataForCache(string stcJobdetailIds, string jobIds);
        /// <summary>
        /// get installation address  
        /// </summary>
        /// <param name="jobid"></param>
        DataTable GetInstallationAddressForCache(int jobid);
        /// <summary>
        /// get ram solar company mapping detail  
        /// </summary>
        ///<param name="ramId"></param>
        DataTable GetRAMSolarCompanyMappingForCache(int ramId);
        /// <summary>
        /// Get job data fro template
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        EmailInfo GetJobDataForTemplate(int jobId);
        /// <summary>
        /// Get Installation Verification Status
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>       
        DataSet GetInstallationVerificationStatus(int JobId);

        /// <summary>
        /// Get Reseller name and solar company by job Id
        /// </summary>
        /// <param name="JobID"></param>
        /// <returns>Resturns Reseller name and solar company name </returns>
        BasicDetails GetResellerSolarCompany(int JobID);
        /// <summary>
        /// ignore 200 mtr validation rule in photos section
        /// </summary>
        /// <param name="JobID"></param>
        DataSet SPVIgnore200mtrValidation(int jobid);
        DataSet GetFullJobPack(int JobId);
        DataSet DeletedCheckListItem(int JobId);
        DataSet RestoreCheckListItem(int JobscId);
        void RestoreData(int vclId, string path, int jobscId, int vcphotoId);
        DataSet GetphotosbyMultipleIds(int JobId, string vcphotoId);
        DataSet GetVisitData(string jobscId);

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
        void RestoreDataWithAllFolder(int vclpID, int? vciId, int? jobscId, string path, bool isReference, bool isDefault, int type);
        /// <summary>
        /// get serial number photos and serial number from jobid 
        /// </summary>
        /// <param name="jobid"></param>
        DataSet GetSerialNumberAndPhotos(int jobid);

        /// <summary>
        /// installation address validation flag change
        /// </summary>
        /// <param name="isValid"></param>
        void installationAddValidChange(bool isValid, int JobId);

        /// <summary>
        /// owner address validation flag change
        /// </summary>
        /// <param name="isValid"></param>
        void OwnerAddValidChange(bool isValid, int JobId);
        /// <summary>
        /// get rec status from jobids
        /// </summary>
        /// <param name="JobIDs"></param>
        /// <returns></returns>
        DataSet CheckStatusOfSubmissionInRec(string JobIDs);

        /// <summary>
        /// Get Data for xml verification 
        /// </summary>
        /// <param name="JobID"></param>
        /// <returns>Dataset</returns>
        DataSet GetDataForXMLVerification(int StreetTypeID, string ModelNo, int JobID);

        /// <summary>
        /// Remove spv when XMl verification faild
        /// </summary>
        /// <param name="JobID"></param>
        void RemoveSPVByXMlVerification(int JobID);
        /// <summary>
        /// update spv flag for XMl verification faild
        /// </summary>
        /// <param name="JobID"></param>
        void UpdateIsSPVXmlVerificationFlag(int JobID, bool isSPVValidXML);
        /// <summary>
        /// get spv XMl verification flag
        /// </summary>
        /// <param name="JobID"></param>

        DataTable GetIsSPVXMlVerificationFlag(int JobID);
        /// <summary>
        /// get serial number by jobid 
        /// </summary>
        /// <param name="JobID"></param>
        /// <returns></returns>
        DataSet GetSerialNobyJobID(int JobID);
        decimal GetPostCodeZoneRating(int postCode);
        DataSet GetUserList(string name, int UserId, int UserTypeId, int JobId);
        DataSet AutoChangeSTCJobStageRTC(int UserId, int UserTypeId, string STCJobIDs, DateTime CreatedDate);

        List<Username> GetAllUserList(int UserId, int UserTypeId, int JobId);
        List<JobNotes> GetComplianceNotes(int JobID);
        string GetUsernameByUserID(int UserID);
        string GetSTCStausNameBySTCStatusID(int STCStatusID);
        /// <summary>
        /// get ref number by job id
        /// </summary>
        /// <param name="JobId"></param>
        /// <returns></returns>
        string GetRefNumberByJobId(int JobId);
        bool CheckAccessForInstallerNotes(int UserId);
        DataSet GetCallLogsBySTCJobDetailsID(int STCJobDetailsID);
        /// <summary>
        /// Get UserId of Solarcompany and solarelectrician by JobID
        /// </summary>
        /// <param name="JobID"></param> 
        /// <returns></returns>
        DataSet GetUserIDofSolarCompanyandSolarElectricianByJobID(int JobID);
        /// <summary>
        /// Insert User notes to db
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="Notes"></param>
        /// <returns></returns>
        void InsertUserNote(int UserID, string Notes);

        /// <summary>
        /// Save Signature Log
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="signatureTypeId"></param>
        /// <param name="signatureMethodId"></param>
        /// <param name="signatureSource"></param>
        void SaveSignatureLog(int jobId, int signatureTypeId, int signatureMethodId, string signatureSource);
        /// <summary>
        /// Get Search Results
        /// </summary>
        /// <param name="search"></param>
        List<SearchResults> GetSearchResults(string search);

        void SaveSTCJobHistory(int STCJobDetailID, int STCStatusID, int UserID, string Description, DateTime CreatedDate, int CreatedBy);

        DataSet GetRECBulkuploadIDandSTCStatusbySTCJobDetailID(string STCJobDetailID);
        /// <summary>
        /// get data for logs of upload photo
        /// </summary>
        /// <param name="VisitCheckListItemId"></param>
        /// <returns></returns>
        DataSet GetCheckListItemDetailsFromIdForLog(int VisitCheckListItemId);
        /// <summary>
        /// get data for owner,installation for job
        /// </summary>
        /// <param name="JobId"></param>
        /// <returns></returns>
        DataSet GetDataForEmailOfReplyNotes(int JobId, int noteCreatedBy = 0);
        /// <summary>
        /// Updates the urgent job.
        /// </summary>
        /// <param name="UrgentJobDays">The number of days for job to be Urgent.</param>
        void UpdateUrgentJobs(int UrgentJobDays);
        /// <summary>
        /// update urgent review stc status flag 
        /// </summary>
        /// <param name="stcjobids"></param>
        /// <returns></returns>
        DataSet UpdateUrgentStatusFlagForSTCIds(string stcjobids);
        DataSet GetJobRetailerSettingData(int JobID, int SolarCompanyId, int userid = 0);
        DataSet GetJobRetailerSettingDataByJobId(int JobId);
        object InsertJobRequestData(int JobId, int JobSchedulingId);
        DataSet GetAutoSignSettingsDataForJob(int JobID, int SolarCompanyId, int UserID);
        bool CheckInstallationDate(string jobIds);
        DataSet GetJobRetailerSettingDataRetailerIdWise(int JobID, int SolarCompanyId, int userid = 0);
        /// <summary>
        /// get spv product verfication status
        /// </summary>
        /// <param name="JobId"></param>
        /// <returns></returns>
        bool? GetProductVerificationStatusByJobId(int JobId);
        /// <summary>
        /// update supplier in job panel details
        /// </summary>
        /// <param name="JobId"></param>
        /// <param name="Brand"></param>
        /// <param name="Model"></param>
        void UpdateSupplierForJob(int JobId, string Brand, string Model, string supplier);
        /// <summary>
        /// Gets the STC job stages with count by year.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <param name="ResellerId">ResellerId.</param>
        /// <param name="RamId">RamId.</param>
        /// <param name="SolarCompanyId">The solar company identifier.</param>
        /// <returns>stage list</returns>
        List<JobStage> GetSTCJobStagesWithCountByYear(int UserId, int UserTypeId, int ResellerId, int RamId, int SolarCompanyId, string isAllScaJobView, string isShowOnlyAssignJobsSCO = "", int year = 0);

        List<JobStage> GetSTCJobStagesWithCountForCERNotApproved(int UserId, int UserTypeId, int ResellerId, int RamId, int SolarCompanyId, string isAllScaJobView, string isShowOnlyAssignJobsSCO = "");
        List<JobStage> GetSTCJobStagesWithCountByYearForCERApproved(int UserId, int UserTypeId, int ResellerId, int RamId, int SolarCompanyId, string isAllScaJobView, string isShowOnlyAssignJobsSCO = "", int year = 0);

        DataSet GetJobList_UserWiseColumnsByYear(int JobViewMenuId, int UserId, int UserTypeId, int PageNumber, int PageSize, string SortCol, string SortDir, int UrgentJobDay, int StageId, int SolarCompanyId, bool IsArchive, int ScheduleType, int JobType, int JobPriority, string searchtext, DateTime? FromDate, DateTime? ToDate, bool IsGst, bool jobref, bool jobdescription, bool jobaddress, bool jobclient, bool jobstaff, bool Invoiced, bool NotInvoiced, bool ReadyToTrade, bool NotReadyToTrade, bool traded, bool nottraded, bool preapprovalnotapproved, bool preapprovalapproved, bool connectioncompleted, bool connectionnotcompleted, bool ACT, bool NSW, bool NT, bool QLD, bool SA, bool TAS, bool WA, bool VIC, int PreApprovalStatusId, int ConnectionStatusId, int year);

        DataSet GetJobList_ForCachingDataKendoByYear(string SolarCompanyId, int UserId, int JobViewMenuId, int year);

        DataTable GetSTCInvoiceAndCreditNoteCount(string stcJobdetailIds);

        DataTable GetSTCInvoiceData(int StcJobDetailId, string STCInvoiceNumber);

        DataSet GetJobsForSEAndSC(int UserId);
        DataSet GetStartEndDateForPanel(string manufacturer, string model);
        DataSet GetStartEndDateForInverter(string brand, string model, string series);
        DataSet GetStartEndDateForSWHBrandModel(string manufacturer, string model);
        /// <summary>
        /// change sca and ra in job
        /// </summary>
        /// <param name="ResellerID"></param>
        /// <param name="SolarCompanyID"></param>
        /// <param name="JobIDs"></param>
        void ChangeSCARAInJob(int ResellerID, int SolarCompanyID, string JobIDs);
        bool CheckJobexists(int JobID);

        /// <summary>
        /// Get DateTimeTicks From QueuedRECSubmission for deleting rec folders and zip files
        /// </summary>
        /// <returns></returns>
        DataTable GetDateTimeTicksFromQueuedRECSubmission();
        void SendToSAASInvoiceBuilder(int stcJobDetailsId, int jobId, int resellerId, int userId, DateTime stcSubmissionDate);

        /// <summary>
        /// Get STC Submission
        /// </summary>
        /// <param name="SolarCompanyId"></param>
        /// <param name="ResellerId"></param>
        /// <param name="Year"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="dtFilter"></param>
        /// <param name="dtSort"></param>
        /// <returns></returns>
        List<STCSubmissionView> GetJobSTCSubmissionKendoByYearWithoutCacheDapper(string SolarCompanyId, int ResellerId, int Year, int page, int pageSize, DataTable dtFilter, DataTable dtSort, int StageId = 0, string sStageId = "");

        List<UserWiseColumns> GetUserWiseColumnsStatic(int loggedInUserId, int JobViewMenuId);

        DataSet GetJobSTCSubmissionForCacheService(string SolarCompanyId, int ResellerId, int Year);

        DataSet GetSettlementDaysToAddForUpFrontSettlementterm(string JobId);
        DataSet ChangeSTCJobStage(int UserId, int UserTypeId, int STCJobStageID, int JobID, DateTime CreatedDate);
        DataSet UpdateUrgentStatusFlagForJobIds(int jobId);
    }
}
