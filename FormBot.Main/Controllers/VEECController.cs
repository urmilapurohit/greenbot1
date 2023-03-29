using FormBot.BAL;
using FormBot.BAL.Service;
using FormBot.BAL.Service.VEEC;
using FormBot.Entity;
using FormBot.Entity.Job;
using FormBot.Entity.VEEC;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Ionic.Zip;
using FormBot.BAL.Service.CommonRules;
using System.Drawing;
using System.Drawing.Imaging;
using System.Configuration;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using System.Collections.Specialized;
using Ionic.Zlib;
using OfficeOpenXml;
using System.Text.RegularExpressions;

namespace FormBot.Main.Controllers
{
    public class VEECController : Controller
    {
        #region Properties
        private readonly ICreateVeecBAL _createVeecBAL;
        private readonly ICreateJobBAL _job;
        private readonly ICreateVeecBAL _veec;
        private readonly IVEECSchedulingBAL _veecSchedule;
        #endregion

        #region Constructor
        public VEECController(ICreateVeecBAL createVeecBAL, ICreateJobBAL job, IVEECSchedulingBAL veecSchedule, ICreateVeecBAL veec)
        {
            this._createVeecBAL = createVeecBAL;
            this._job = job;
            this._veecSchedule = veecSchedule;
            this._veec = veec;
        }
        #endregion

        #region Method

        [HttpGet]
        public ActionResult CreateVEECPopup()
        {
            ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
            return PartialView("_CreateVEECPopup", new FormBot.Entity.CreateVEEC());
        }

        [HttpPost]
        public JsonResult CreateVEECPopup(CreateVEECPopup createVeecPopup)
        {
            CreateVEEC objCreateVEEC = createVeecPopup.createVEEC;
            try
            {
                if (ProjectSession.LoggedInUserId == 0)
                {
                    return Json(new { error = true, isLogout = 0 }, JsonRequestBehavior.AllowGet); //return RedirectToAction("Logout", "Account");
                }

                if (!ModelState.IsValid)
                {
                    objCreateVEEC.VEECInstaller = new VEECInstaller();
                    Int32 veecID = _createVeecBAL.InsertVeec(objCreateVEEC);
                    return Json(new { error = false, veecId = QueryString.QueryStringEncode("id=" + Convert.ToString(veecID)), veecName = objCreateVEEC.VEECDetail.RefNumber }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string msg = string.Empty;
                    ModelState.Values.AsEnumerable().ToList().ForEach(d =>
                    {
                        if (d.Errors.Count > 0)
                            msg += d.Errors[0].ErrorMessage;
                    });
                    return Json(new { error = true, errorMessage = msg }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [UserAuthorization]
        public ActionResult Index(string id = null)
        {
            if (ProjectSession.LoggedInUserId > 0)
            {
                //List All Veec
                if (string.IsNullOrEmpty(id))
                {
                    FormBot.Entity.VEECList veec = new FormBot.Entity.VEECList();
                    veec.UserTypeID = ProjectSession.UserTypeId;
                    if (ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 8)
                    {
                        veec.lstJobStages = _job.GetJobStagesWithCount(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, ProjectSession.SolarCompanyId);
                    }
                    else
                    {
                        veec.lstJobStages = _job.GetJobStagesWithCount(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, 0);
                    }

                    var jobSearchType = from SystemEnums.JobScheduleType j in Enum.GetValues(typeof(SystemEnums.JobScheduleType))
                                        select new { ID = j, Name = j.ToString() };
                    ViewData["JobScheduleType"] = new SelectList(jobSearchType, "ID", "Name");
                    var jobType = from SystemEnums.JobType j in Enum.GetValues(typeof(SystemEnums.JobType))
                                  select new { ID = j, Name = j.ToString() };
                    ViewData["JobType"] = new SelectList(jobType, "ID", "Name");

                    var jobPriority = from SystemEnums.JobPriority j in Enum.GetValues(typeof(SystemEnums.JobPriority))
                                      select new { ID = j, Name = j.ToString() };
                    ViewData["JobPriority"] = new SelectList(jobPriority, "ID", "Name");

                    ViewData["JobStage"] = new SelectList(GetJobAllStage(ProjectSession.UserTypeId), "Value", "Text");

                    ViewData["colPriority"] = Newtonsoft.Json.JsonConvert.SerializeObject(jobPriority);
                    ViewData["colJobType"] = Newtonsoft.Json.JsonConvert.SerializeObject(jobType);
                    var tradeStatus = from SystemEnums.TradeStatus j in Enum.GetValues(typeof(SystemEnums.TradeStatus))
                                      select new { ID = j, Name = j.ToString() };
                    ViewData["colTradeStatus"] = Newtonsoft.Json.JsonConvert.SerializeObject(tradeStatus);

                    DataSet dsPreapproval = _job.GetPreApprovalStatus();
                    DataSet dsConnection = _job.GetConnectionStatus();
                    if (dsPreapproval != null && dsPreapproval.Tables.Count > 0 && dsPreapproval.Tables[0] != null && dsPreapproval.Tables[0].Rows.Count > 0)
                    {
                        veec.lstPreApproval = dsPreapproval.Tables[0].ToListof<PreConStatus>();
                    }

                    if (dsConnection != null && dsConnection.Tables.Count > 0 && dsConnection.Tables[0] != null && dsConnection.Tables[0].Rows.Count > 0)
                    {
                        veec.lstConnection = dsConnection.Tables[0].ToListof<PreConStatus>();
                    }

                    if (TempData["PreApprovalsDashboardStatus"] != null && Convert.ToString(TempData["PreApprovalsDashboardStatus"]) != "")
                    {
                        veec.PreApprovalStatusId = Convert.ToInt32(TempData["PreApprovalsDashboardStatus"]);
                    }

                    if (TempData["ConnectionsDashboardStatus"] != null && Convert.ToString(TempData["ConnectionsDashboardStatus"]) != "")
                    {
                        veec.ConnectionStatusId = Convert.ToInt32(TempData["ConnectionsDashboardStatus"]);
                    }

                    List<UserWiseColumns> listUserWiseColumns = _job.GetUserWiseColumns(ProjectSession.LoggedInUserId, SystemEnums.MenuId.JobView.GetHashCode());
                    ViewBag.JSUserColumnList = listUserWiseColumns;
                    ViewBag.ListColumnName = string.Join(",", listUserWiseColumns.Select(X => X.Name));
                    ViewBag.ListColumnWidth = string.Join(",", listUserWiseColumns.Select(X => X.Width));

                    DataSet dsAdvanceSearchCategory = _job.GetAdvanceSearchCategory();
                    if (dsAdvanceSearchCategory != null && dsAdvanceSearchCategory.Tables.Count > 0)
                    {
                        List<AdvanceSearchCategory> lstSearchCategory = new List<AdvanceSearchCategory>();

                        if (dsAdvanceSearchCategory.Tables[0].Rows.Count > 0)
                        {
                            Int32[] arrSearchCategoryId = dsAdvanceSearchCategory.Tables[0].AsEnumerable().ToList().Select(s => s.Field<Int32>("SearchCategoryId")).Distinct().ToArray<Int32>();
                            foreach (int i in arrSearchCategoryId)
                            {
                                List<DataRow> drSearchCategory = dsAdvanceSearchCategory.Tables[0].AsEnumerable().ToList().Where(s => s.Field<Int32>("SearchCategoryId") == i).ToList();
                                if (drSearchCategory.Count > 0)
                                {
                                    AdvanceSearchCategory tmp = new AdvanceSearchCategory();
                                    tmp.SearchCategoryId = Convert.ToInt32(drSearchCategory[0]["SearchCategoryId"]);
                                    tmp.SearchCategoryName = Convert.ToString(drSearchCategory[0]["SearchCategoryName"]);
                                    List<AdvanceSearchSubCategory> lstAdvanceSearchSubCategory = new List<AdvanceSearchSubCategory>();
                                    foreach (DataRow dr in drSearchCategory)
                                    {
                                        AdvanceSearchSubCategory tmpsub = new AdvanceSearchSubCategory();
                                        tmpsub.ColumnID = Convert.ToInt32(dr["ColumnID"]);
                                        tmpsub.Name = Convert.ToString(dr["Name"]);
                                        tmpsub.DisplayName = Convert.ToString(dr["DisplayName"]);
                                        lstAdvanceSearchSubCategory.Add(tmpsub);
                                    }

                                    if (lstAdvanceSearchSubCategory.Count > 0)
                                    {
                                        tmp.lstAdvanceSearchSubCategory = lstAdvanceSearchSubCategory;
                                        tmp.hdnColName = lstAdvanceSearchSubCategory[0].Name;
                                    }

                                    if (tmp.SearchCategoryId == 3 && dsAdvanceSearchCategory.Tables[1].Rows.Count > 0)
                                    {
                                        tmp.AllFilters = tmp.hdnAllFilters = string.Join(",", dsAdvanceSearchCategory.Tables[1].AsEnumerable().ToList().Select(s => s.Field<string>("Abbreviation")).Distinct().ToArray<string>());
                                    }
                                    else if (tmp.SearchCategoryId == 4)
                                    {
                                        tmp.AllFilters = tmp.hdnAllFilters = string.Join(",", jobPriority.Select(x => x.Name).ToList());
                                    }
                                    else if (tmp.SearchCategoryId == 5)
                                    {
                                        tmp.AllFilters = tmp.hdnAllFilters = string.Join(",", tradeStatus.Select(x => x.Name).ToList());
                                    }
                                    else if (tmp.SearchCategoryId == 6)
                                    {
                                        tmp.AllFilters = tmp.hdnAllFilters = "Pending,Approved";
                                    }
                                    else if (tmp.SearchCategoryId == 7)
                                    {
                                        tmp.AllFilters = tmp.hdnAllFilters = "Pending,Completed";
                                    }
                                    else if (tmp.SearchCategoryId == 8)
                                    {
                                        tmp.AllFilters = tmp.hdnAllFilters = string.Join(",", jobType.Select(x => x.Name).ToList());
                                    }
                                    else
                                    {
                                        tmp.AllFilters = string.Join(",", string.Join(",", lstAdvanceSearchSubCategory.Select(X => X.DisplayName).ToList()));
                                        tmp.hdnAllFilters = string.Join(",", string.Join(",", lstAdvanceSearchSubCategory.Select(X => X.Name).ToList()));
                                        tmp.hdnColName = string.Empty;
                                    }

                                    lstSearchCategory.Add(tmp);
                                }
                            }
                        }

                        //ViewBag.jsListSearchCategory = Newtonsoft.Json.JsonConvert.SerializeObject(lstSearchCategory);
                        veec.lstAdvanceSearchCategory = lstSearchCategory;
                    }
                    return View("Index", veec);
                }
                else
                {
                    //Get Veec Detail by Id
                    int veecId = 0;
                    if (!string.IsNullOrEmpty(id))
                    {
                        int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out veecId);
                    }
                    CreateVEEC createVEEC = _createVeecBAL.GetVeecByID(veecId);
                    //var lamptype= from SystemEnums.LampType l in Enum.GetValues(typeof(SystemEnums.LampType))
                    //              select new { ID = l, Name = l.ToString() };
                    //ViewData["Lamptype"] = new SelectList(lamptype, "ID", "Name");
                    createVEEC.chkPhotosAll = LoadAllPhotosOfNewVeec(veecId);
                    createVEEC.lstArea = LoadAllFolderPhotoVeec(veecId);
                    createVEEC.Guid = Convert.ToString(veecId);
                    ViewBag.PostalAddressType = new SelectList(EnumExtensions.GetPostalAddressEnumList(), "ID", "Name");
                    BasicDetailsVEEC basicDetail = new BasicDetailsVEEC();
                    basicDetail.ScoIDVEEC = _veec.GetSCOIdByVeecId(createVEEC.VEECId);
                    createVEEC.BasicDetailsVeec = basicDetail;
                    createVEEC.VeecScheduling = _veecSchedule.GetAllSchedulingDataOfVEEC(id, true, false, _veec);
                    createVEEC.UserType = ProjectSession.UserTypeId;

                    return View("ViewAndEditVEEC", createVEEC);
                }
            }
            else
                return RedirectToAction("Logout", "Account");

        }

        /// <summary>
        /// This method is used for load Veec photos based on veecId and area 
        /// </summary>
        /// <param name="veecId"></param>
        /// <returns></returns>
        public List<VEECArea> LoadAllFolderPhotoVeec(int veecId)
        {
            DataSet lstVeecPhotos = _createVeecBAL.GetVeecsPhotos(veecId);
            List<VEECArea> lstArea = new List<VEECArea>();


            if (lstVeecPhotos.Tables.Count > 0)
            {
                DataTable veecArea = lstVeecPhotos.Tables[0];
                DataTable veecPhotos = lstVeecPhotos.Tables[1];
                DataTable veecFolder = lstVeecPhotos.Tables[2];

                foreach (DataRow dr in veecArea.Rows)
                {
                    List<VEECFolder> lstFolder = new List<VEECFolder>();
                    VEECArea objveecArea = new VEECArea();
                    objveecArea.VEECAreaId = Convert.ToInt32(dr["VEECAreaId"]);
                    objveecArea.Name = Convert.ToString(dr["Name"]);
                    objveecArea.IsDeleted = Convert.ToBoolean(dr["IsDeleted"]);
                    foreach (DataRow dtrow in veecFolder.Rows)
                    {

                        if (dtrow["ParentId"] == DBNull.Value)
                        {
                            List<VEECSubFolder> lstSubFolder = new List<VEECSubFolder>();
                            VEECFolder folder = new VEECFolder();
                            folder.FolderName = Convert.ToString(dtrow["FolderName"]);
                            folder.Id = Convert.ToInt32(dtrow["VeecFolderId"]);
                            if (folder.FolderName.Equals("Decommisioned Equipment"))
                            {
                                continue;
                            }
                            DataRow[] subFolder = veecFolder.Select("ParentId=" + folder.Id + "");
                            if (subFolder.Length == 0)
                            {
                                List<VeecPhotos> lstVeecPhoto = new List<VeecPhotos>();
                                DataRow[] veecPhotoRow = veecPhotos.Select("FolderId = " + folder.Id + " and VeecAreaId = " + objveecArea.VEECAreaId);
                                foreach (DataRow photoRow in veecPhotoRow)
                                {
                                    if (!Convert.ToBoolean(photoRow["IsDeleted"]))
                                    {
                                        VeecPhotos objVeecPhotos = new VeecPhotos();

                                        objVeecPhotos.FullPath = Convert.ToString(photoRow["FullPath"]);
                                        objVeecPhotos.Name = Path.GetFileName(objVeecPhotos.FullPath);
                                        objVeecPhotos.veecPhotosId = Convert.ToInt32(photoRow["VeecPhotoId"]);
                                        lstVeecPhoto.Add(objVeecPhotos);
                                    }
                                }
                                folder.lstPhotos = lstVeecPhoto;
                            }
                            foreach (DataRow subFolderRow in subFolder)
                            {
                                List<VeecPhotos> lstVeecPhoto = new List<VeecPhotos>();
                                VEECSubFolder objVeecSubFolder = new VEECSubFolder();
                                objVeecSubFolder.FolderName = Convert.ToString(subFolderRow["FolderName"]);
                                objVeecSubFolder.Id = Convert.ToInt32(subFolderRow["VeecFolderId"]);
                                DataRow[] veecPhotoRow = veecPhotos.Select("FolderId = " + objVeecSubFolder.Id + " and VeecAreaId = " + objveecArea.VEECAreaId);
                                foreach (DataRow photoRow in veecPhotoRow)
                                {
                                    if (!Convert.ToBoolean(photoRow["IsDeleted"]))
                                    {
                                        VeecPhotos objVeecPhotos = new VeecPhotos();

                                        objVeecPhotos.FullPath = Convert.ToString(photoRow["FullPath"]);
                                        objVeecPhotos.Name = Path.GetFileName(objVeecPhotos.FullPath);
                                        objVeecPhotos.veecPhotosId = Convert.ToInt32(photoRow["VeecPhotoId"]);
                                        lstVeecPhoto.Add(objVeecPhotos);
                                    }

                                }
                                objVeecSubFolder.lstPhotos = lstVeecPhoto;
                                lstSubFolder.Add(objVeecSubFolder);

                            }
                            folder.subFolder = lstSubFolder;
                            lstFolder.Add(folder);
                        }
                    }
                    objveecArea.veecFolder = lstFolder;

                    lstArea.Add(objveecArea);
                }
            }
            return lstArea;
        }

        /// <summary>
        /// This method is used to load checklistitem photo of veec visit based on veecId
        /// </summary>
        /// <param name="veecId"></param>
        /// <returns></returns>
        public chkPhotosVeec LoadAllPhotosOfNewVeec(int veecId)
        {
            List<VeecSchedulingPhotos> objLst = new List<VeecSchedulingPhotos>();
            //Karan
            DataSet dsCheckListPhotos = _veec.GetChecklistPhotosVeec(veecId);
            chkPhotosVeec objChk = new chkPhotosVeec();

            if (dsCheckListPhotos.Tables.Count > 0)
            {
                DataTable dtSchedulingIds = dsCheckListPhotos.Tables[0];
                DataTable dtData = dsCheckListPhotos.Tables[1];
                DataTable dtCheckListCount = dsCheckListPhotos.Tables[2];
                DataTable dtName = dsCheckListPhotos.Tables[3];
                DataRow[] ReferencePhotos = dtData.Select("IsReference=1");
                DataRow[] DefaultPhotos = dtData.Select("IsDefault=1");
                foreach (DataRow row in dtSchedulingIds.Rows)
                {
                    VeecSchedulingPhotos obj = new VeecSchedulingPhotos();
                    obj.UniqueVisitID = Convert.ToString(row["VisitUniqueId"]);
                    obj.veecSchedulingId = Convert.ToInt32(row["VeecSchedulingID"]);
                    obj.veecId = veecId;
                    obj.IsDefaultSubmission = Convert.ToBoolean(row["IsDefaultSubmissionofVeec"]);
                    obj.IsDeleted = Convert.ToBoolean(row["IsDeleted"]);
                    DataRow[] result = dtData.Select("VeecSchedulingId = " + obj.veecSchedulingId.ToString());
                    List<VEECVisitCheckListItems> lstVisitCheckListItem = new List<VEECVisitCheckListItems>();
                    int serialNumTotalCount = dtCheckListCount.AsEnumerable().Where(a => a.Field<int?>("VeecSchedulingId") == Convert.ToInt32(obj.veecSchedulingId) && a.Field<int>("VeecCheckListClassTypeId") == 1).Select(a => a.Field<int>("TotalCheckListItemCount")).FirstOrDefault();
                    int capturePhotoTotalCount = dtCheckListCount.AsEnumerable().Where(a => a.Field<int?>("VeecSchedulingId") == Convert.ToInt32(obj.veecSchedulingId) && a.Field<int>("VeecCheckListClassTypeId") == 2).Select(a => a.Field<int>("TotalCheckListItemCount")).FirstOrDefault();
                    int signatureTotalCount = dtCheckListCount.AsEnumerable().Where(a => a.Field<int?>("VeecSchedulingId") == Convert.ToInt32(obj.veecSchedulingId) && a.Field<int>("VeecCheckListClassTypeId") == 3).Select(a => a.Field<int>("TotalCheckListItemCount")).FirstOrDefault();
                    DataRow[] checklistitems = dtName.Select("VeecSchedulingId = " + obj.veecSchedulingId.ToString());
                    foreach (DataRow dr in checklistitems)
                    {
                        int count = dtCheckListCount.AsEnumerable().Where(a => a.Field<int>("VeecVisitCheckListItemId") == Convert.ToInt32(dr["VeecVisitCheckListItemId"])).Select(a => a.Field<int>("TotalCheckListItemCount")).FirstOrDefault();
                        int visitedCount = dtCheckListCount.AsEnumerable().Where(a => a.Field<int>("VeecVisitCheckListItemId") == Convert.ToInt32(dr["VeecVisitCheckListItemId"])).Select(a => a.Field<int>("VisitedCount")).FirstOrDefault();
                        VEECVisitCheckListItems visitCheckListItem = new VEECVisitCheckListItems();
                        visitCheckListItem.VeecVisitCheckListItemId = Convert.ToString(dr["VeecVisitCheckListItemId"]);
                        visitCheckListItem.FolderName = Convert.ToString(dr["FolderName"]);
                        visitCheckListItem.TotalCount = count;
                        visitCheckListItem.VisitedCount = visitedCount;
                        visitCheckListItem.CheckListClassTypeId = Convert.ToInt32(dr["VeecCheckListClassTypeId"]); // dtCheckListCount.AsEnumerable().Where(a => a.Field<int>("VisitCheckListItemId") == Convert.ToInt32(dr["VisitCheckListItemId"])).Select(a => a.Field<int>("CheckListClassTypeId")).FirstOrDefault();
                        visitCheckListItem.PDFLocationId = Convert.ToString(dr["PDFLocationId"]);
                        visitCheckListItem.CaptureUploadImagePDFName = Convert.ToString(dr["CaptureUploadImagePDFName"]);
                        DataRow[] checkListPhotos = dtData.Select("VeecVisitCheckListItemId = " + visitCheckListItem.VeecVisitCheckListItemId);
                        List<Photo> objList = new List<Photo>();
                        foreach (DataRow drPhotos in checkListPhotos)
                        {
                            Photo p = new Photo();
                            p.Name = Path.GetFileName(Convert.ToString(drPhotos["Path"]));
                            p.Path = Convert.ToString(drPhotos["Path"]);
                            p.VisitCheckListPhotoId = Convert.ToString(drPhotos["VeecVisitCheckListPhotoId"]);
                            p.VisitSignatureId = Convert.ToString(drPhotos["VeecVisitSignatureId"]);
                            p.Latitude = Convert.ToString(drPhotos["Latitude"]);
                            p.Longitude = Convert.ToString(drPhotos["Longitude"]);
                            p.CreatedDate = Convert.ToString(drPhotos["CreatedDate"]);
                            objList.Add(p);
                        }
                        visitCheckListItem.lstCheckListPhoto = objList;
                        if (visitCheckListItem.CheckListClassTypeId != 5)
                        {
                            lstVisitCheckListItem.Add(visitCheckListItem);
                        }
                    }
                    obj.serialNumTotalCount = serialNumTotalCount;
                    obj.capturePhotoTotalCount = capturePhotoTotalCount;
                    obj.signatureTotalCount = signatureTotalCount;
                    obj.lstVeecVisitCheckListItem = lstVisitCheckListItem;
                    if (obj.lstVeecVisitCheckListItem.Count > 0)
                    {
                        objLst.Add(obj);
                    }
                }

                List<Photo> chkReference = new List<Photo>();
                foreach (DataRow drRef in ReferencePhotos)
                {
                    string path = drRef["Path"].ToString();
                    Photo objPhoto = new Photo();
                    objPhoto.Path = path;
                    objPhoto.Name = Path.GetFileName(path);
                    objPhoto.VisitCheckListPhotoId = Convert.ToString(drRef["VeecVisitCheckListPhotoId"]);
                    objPhoto.VisitSignatureId = Convert.ToString(drRef["VeecVisitSignatureId"]);
                    chkReference.Add(objPhoto);
                }


                List<Photo> objInstall = new List<Photo>();
                List<Photo> objSerial = new List<Photo>();

                foreach (DataRow dRow in DefaultPhotos)
                {
                    int ClassType = Convert.ToInt32(dRow["ClassType"]);
                    Photo p = new Photo();
                    p.VisitCheckListPhotoId = Convert.ToString(dRow["VeecVisitCheckListPhotoId"]);
                    p.VisitSignatureId = Convert.ToString(dRow["VeecVisitSignatureId"]);
                    p.Path = dRow["Path"].ToString();
                    p.Name = Path.GetFileName(p.Path);
                    p.Latitude = Convert.ToString(dRow["Latitude"]);
                    p.Longitude = Convert.ToString(dRow["Longitude"]);
                    p.CreatedDate = Convert.ToString(dRow["CreatedDate"]);

                    if (ClassType == 1)
                        objSerial.Add(p);
                    else if (ClassType == 2)
                        objInstall.Add(p);

                }
                objChk.InstallationPhotos = objInstall;

                objChk.SerialPhotos = objSerial;
                objChk.veecId = veecId;
                objChk.chkVeecPhotos = objLst;
                objChk.ReferencePhotos = chkReference;
            }

            return objChk;
        }

        public void RemoveModelStateOfAddressFields(bool IsPostalAddress, string ObjectName)
        {
            if (IsPostalAddress)
            {
                ModelState.Remove(ObjectName + ".PostalDeliveryNumber");
                ModelState.Remove(ObjectName + ".PostalAddressID");
            }
            else
            {
                ModelState.Remove(ObjectName + ".StreetName");
                ModelState.Remove(ObjectName + ".StreetNumber");
                ModelState.Remove(ObjectName + ".StreetTypeID");
            }
        }
        
        /// <summary>
        /// Upload file to Veec Portal
        /// </summary>
        /// <param name="createVEEC"></param>
        /// <returns></returns>
        [HttpPost]
        [UserAuthorization]
        public JsonResult Create(CreateVEEC createVEEC)
        {
            //RemoveModelStateOfAddressFields(createVEEC.VEECOwnerDetail.IsPostalAddress, "VEECOwnerDetail");
            RemoveModelStateOfAddressFields(createVEEC.VEECInstallationDetail.IsPostalAddress, "VEECInstallationDetail");
            RemoveRequiredFieldOf1680Ligthing(createVEEC.VEECDetail);
            //RemoveModelStateOfAddressFields(createVEEC.VEECInstaller.IsPostalAddress, "VEECInstaller");
            ModelState.Remove("VEECOwnerDetail.CompanyName");
            ModelState.Remove("VEECOwnerDetail.OwnerType");


            if (ModelState.IsValid)
            //ModelState.IsValidField("VEECDetail.ScheduleActivityType") &&
            //ModelState.IsValidField("VEECDetail.RefNumber") &&
            //ModelState.IsValidField("VEECDetail.Title") &&
            //ModelState.IsValidField("VEECDetail.Description") &&
            //ModelState.IsValidField("VEECDetail.CommencementDate") &&
            //ModelState.IsValidField("VEECDetail.ActivityDate")
            {
                Int32 veecID = _createVeecBAL.InsertVeec(createVEEC);
                return Json(new { error = false, veecId = QueryString.QueryStringEncode("id=" + Convert.ToString(veecID)), veecName = createVEEC.VEECDetail.RefNumber }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string msg = string.Empty;
                ModelState.Values.AsEnumerable().ToList().ForEach(d =>
                {
                    if (d.Errors.Count > 0)
                        msg += d.Errors[0].ErrorMessage + "</br>";
                });
                return Json(new { error = true, errorMessage = msg }, JsonRequestBehavior.AllowGet);
            }

        }

        public void RemoveRequiredFieldOf1680Ligthing(VEECDetail veecDetail)
        {
            if (veecDetail.ContractualArrangementId != 5)
            {
                ModelState.Remove("VEECDetail.ContractualDetails");
            }
            if(veecDetail.LightingDesignMethodId == 1 )
            {
                if(veecDetail.LightLevelVerificationId == 1  || veecDetail.LightLevelVerificationId == 0)
                {
                    ModelState.Remove("VEECDetail.QualificationOfLightLevelVerifierId");
                    ModelState.Remove("VEECDetail.VerifierQualificationDetails");
                }
            }
            else if(veecDetail.LightingDesignMethodId == 2)
            {
                if(veecDetail.LightLevelVerificationId == 1)
                {
                    ModelState.Remove("VEECDetail.QualificationOfLightLevelVerifierId");
                    ModelState.Remove("VEECDetail.VerifierQualificationDetails");
                }
                ModelState.Remove("VEECDetail.QualificationsOfLightingDesignerId");
                ModelState.Remove("VEECDetail.DesignerQualificationDetails");
            }
            else
            {
                ModelState.Remove("VEECDetail.QualificationOfLightLevelVerifierId");
                ModelState.Remove("VEECDetail.VerifierQualificationDetails");
                ModelState.Remove("VEECDetail.QualificationsOfLightingDesignerId");
                ModelState.Remove("VEECDetail.DesignerQualificationDetails");
            }
        }

        public CookieContainer VEECLogin()
        {
            CookieContainer ck = new CookieContainer();

            string postUrl = ProjectConfiguration.VEECUrl + "Account/Login.aspx";

            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;
            request.Method = "GET";

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            ck.Add(response.Cookies);

            System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream());
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();

            HtmlDocument do2c = new HtmlDocument();
            do2c.LoadHtml(responseFromServer);

            Dictionary<string, string> postParameters = new Dictionary<string, string>();

            postParameters.Add("__EVENTTARGET", "");
            postParameters.Add("__EVENTARGUMENT", "");
            postParameters.Add("__VIEWSTATE", do2c.GetElementbyId("__VIEWSTATE").GetAttributeValue("value", ""));
            postParameters.Add("__VIEWSTATEGENERATOR", do2c.GetElementbyId("__VIEWSTATEGENERATOR").GetAttributeValue("value", ""));
            postParameters.Add("__EVENTVALIDATION", do2c.GetElementbyId("__EVENTVALIDATION").GetAttributeValue("value", ""));
            postParameters.Add("ctl00$MenuHead", "{&quot;selectedItemIndexPath&quot;:&quot;&quot;,&quot;checkedState&quot;:&quot;&quot;}");
            postParameters.Add("ctl00$ContentPlaceHolder1$LoginUser$UserName", "sKhan");
            postParameters.Add("ctl00$ContentPlaceHolder1$LoginUser$Password", "3m3rging786");
            //Mouse coordinates
            postParameters.Add("ctl00$ContentPlaceHolder1$LoginUser$LoginButton.x", "27");
            postParameters.Add("ctl00$ContentPlaceHolder1$LoginUser$LoginButton.y", "10");

            string strPost = string.Empty;
            foreach (var item in postParameters)
            {
                strPost += HttpUtility.UrlEncode(item.Key) + "=" + HttpUtility.UrlEncode(item.Value) + "&";
            }
            strPost = strPost.TrimEnd(new char[] { '&' });

            string postData = string.Empty;
            byte[] byteArray;

            request = (HttpWebRequest)WebRequest.Create(postUrl);
            request.Method = "POST";
            request.Host = ProjectConfiguration.VEECHost;
            request.KeepAlive = true;
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Referer = ProjectConfiguration.VEECUrl + "Account/Login.aspx";
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.8");
            request.CookieContainer = ck;
            request.AllowAutoRedirect = false;

            // Create POST data and convert it to a byte array.
            postData = strPost;
            byteArray = Encoding.UTF8.GetBytes(postData);

            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;

            // To establish trust relationship for the SSL/TLS.
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();

            response = request.GetResponse() as HttpWebResponse;
            ck.Add(response.Cookies);
            //ck.Add(ck.GetCookies(response.ResponseUri));

            Session["VeecSessionCookie"] = ck;
            Session["VeecSessionCookieTimeout"] = DateTime.Now.AddHours(3);

            reader = new System.IO.StreamReader(response.GetResponseStream());
            responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();

            return (CookieContainer)Session["VeecSessionCookie"];
        }

        public JsonResult UploadVEEC(int veecId)
        {
            if (Session["VeecVeecSessionCookie"] != null && (DateTime)Session["VeecSessionCookieTimeout"] <= DateTime.Now)
            {
                Session.Remove("VeecSessionCookie");
            }
            var VeecSessionCookie = (CookieContainer)Session["VeecSessionCookie"];
            if (VeecSessionCookie == null)
            {
                VeecSessionCookie = VEECLogin();
            }

            string ErrorMsg = string.Empty;
            string RefNumber = string.Empty;

            string FilePath = Server.MapPath("~/VEECDocuments/" + DateTime.Now.Ticks + ".xlsx");
            var file = new FileInfo(FilePath);

            StringBuilder csv = new StringBuilder();

            csv.Append(@"Own Reference,Scheduled activity premises?,Brief Description of the Upgrade,Upgrade Commencement Date,Activity Date,Business/Company Name,ABN / ACN,Industry/Business Type,Number of Levels,Floor Space (m2),Floor Space Upgraded Area (m2),Unit Type,Unit Number,Level Type,Level Number,Street Number,Street Name,Street Type,Street Type Suffix,Town / Suburb,State,Postcode,Authorised Signatory First Name,Authorised Signatory Last Name,Authorised Signatory Phone Number,Contractual Arrangements (who undertook work),Contractual details (if other),1680 - Lighting design method,1680 - Qualifications of lighting designer,1680 - Designer qualification details,1680 - Light level verification,1680 - Qualifications of light level verifier,1680 - Verifier qualification details,Electrician (Installer) Company Name,Electrician First Name,Electrician Last Name,Certificate of Electrical Compliance Number,Electrician Licence Number,Upgrade Manager Company Name,Upgrade Manager First Name,Upgrade Manager Last Name,Upgrade Manager Phone Number");
            for (int i = 1; i <= 50; i++)
            {
                csv.Append(@",Area Name " + i);
                csv.Append(@",Space Type " + i);
                csv.Append(@",Space Type Unlisted " + i);
                csv.Append(@",BCA classification " + i);
                csv.Append(@",Baseline/Upgrade " + i);
                csv.Append(@",Lamp Ballast Combination " + i);
                csv.Append(@",Lamp Category " + i);
                csv.Append(@",Quantity " + i);
                csv.Append(@",BASELINE Asset Lifetime Reference " + i);
                csv.Append(@",UPGRADE Asset Lifetime Reference " + i);
                csv.Append(@",Product Brand " + i);
                csv.Append(@",Product Model " + i);
                csv.Append(@",Rated Lifetime Hours " + i);
                csv.Append(@",Nominal Lamp Power " + i);
                csv.Append(@",First Controller Type " + i);
                csv.Append(@",Second Controller Type " + i);
                csv.Append(@",VRU1 Product Brand " + i);
                csv.Append(@",VRU1 Product Model " + i);
                csv.Append(@", HVAC A/C " + i);     // Space Inserted as per VEEC portal Format
            }
            csv.Append(@",Unrecognised Address Justification,Internal Duplicate Justification,External Duplicate Justification");

            string strHeader = csv.ToString();
            using (ExcelPackage pckExport = new ExcelPackage(file))
            {
                ExcelWorksheet worksheetExport = pckExport.Workbook.Worksheets.Add("UploadVeec");

                List<string> lstHeader = strHeader.Split(new string[] { "," }, StringSplitOptions.None).ToList();

                for (int i = 0; i < lstHeader.Count(); i++)
                {
                    worksheetExport.Cells[1, i + 1].Value = lstHeader[i];
                }

                int lastColInserted = 1;
                int lastRowsInserted = 2;

                DataSet dsVEEC = _veec.UploadVeec_GetVeecByID(veecId);
                RefNumber = dsVEEC.Tables[0].Rows[0].ItemArray[0].ToString();

                foreach (DataTable dt in dsVEEC.Tables)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        for (int i = 0; i < dr.ItemArray.Length; i++)
                        {
                            worksheetExport.Cells[lastRowsInserted, lastColInserted].Value = dr.ItemArray[i];
                            lastColInserted++;
                        }
                    }
                }
                pckExport.Save();
            }
            
            string postUrl = ProjectConfiguration.VEECUrl + "Accounts/Activities/Upload.aspx";

            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;
            request.Method = "GET";
            request.CookieContainer = VeecSessionCookie;

            //WebProxy myProxy = new WebProxy();
            //Uri newUri = new Uri("http://192.168.0.251:8080");
            //myProxy.Address = newUri;
            //request.Proxy = myProxy;

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //cookie = response.Headers[HttpResponseHeader.SetCookie];

            Stream dataStream = response.GetResponseStream();

            System.IO.StreamReader reader = new StreamReader(dataStream);

            string responseFromServer = reader.ReadToEnd();

            reader.Close();
            dataStream.Close();
            response.Close();

            HtmlDocument do2c = new HtmlDocument();
            do2c.LoadHtml(responseFromServer);

            Dictionary<string, string> postParameters = new Dictionary<string, string>();

            postParameters.Add("__EVENTTARGET", "ctl00$ctl00$ContentPlaceHolder1$Content$ScheduleFormCMB");
            postParameters.Add("__EVENTARGUMENT", "");
            postParameters.Add("__VIEWSTATE", do2c.GetElementbyId("__VIEWSTATE").GetAttributeValue("value", ""));
            postParameters.Add("__VIEWSTATEGENERATOR", do2c.GetElementbyId("__VIEWSTATEGENERATOR").GetAttributeValue("value", ""));
            postParameters.Add("__EVENTVALIDATION", do2c.GetElementbyId("__EVENTVALIDATION").GetAttributeValue("value", ""));
            postParameters.Add("ctl00$ctl00$MenuHead", "{&quot;selectedItemIndexPath&quot;:&quot;&quot;,&quot;checkedState&quot;:&quot;&quot;}");
            postParameters.Add("ContentPlaceHolder1_Content_ScheduleFormCMB_VI", "1543");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ScheduleFormCMB", "34 NonJ6 - Lighting Upgrade - Business");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ScheduleFormCMB$DDD$L", "1543");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ScheduleFormCMB$DDDState", "{&quot;windowsState&quot;:&quot;1:1:12000:247:205:1:513:142:1:0:0:0&quot;}");
            postParameters.Add("ctl00$ctl00$MenuFoot", "{&quot;selectedItemIndexPath&quot;:&quot;&quot;,&quot;checkedState&quot;:&quot;&quot;}");
            postParameters.Add("DXScript", "1_16,1_17,1_28,1_66,1_19,1_20,1_22,1_29,1_36,1_225,1_226,1_231,1_228,1_234,1_44");
            postParameters.Add("DXCss", "0_1144,1_69,1_71,0_1148,1_251,0_1002,1_250,0_1006,../../App_Themes/Glass/Public170801.css,../../App_Themes/Glass/Chart/styles.css,../../App_Themes/Glass/Editors/sprite.css,../../App_Themes/Glass/Editors/styles.css,../../App_Themes/Glass/GridView/sprite.css,../../App_Themes/Glass/GridView/styles.css,../../App_Themes/Glass/HtmlEditor/sprite.css,../../App_Themes/Glass/HtmlEditor/styles.css,../../App_Themes/Glass/Portlets.css,../../App_Themes/Glass/Scheduler/sprite.css,../../App_Themes/Glass/Scheduler/styles.css,../../App_Themes/Glass/SpellChecker/styles.css,../../App_Themes/Glass/TreeList/sprite.css,../../App_Themes/Glass/TreeList/styles.css,../../App_Themes/Glass/Web/sprite.css,../../App_Themes/Glass/Web/styles.css");

            string strPost = string.Empty;
            foreach (var item in postParameters)
            {
                strPost += item.Key + "=" + HttpUtility.UrlEncode(item.Value) + "&";
            }
            strPost = strPost.TrimEnd(new char[] { '&' });

            request = WebRequest.Create(postUrl) as HttpWebRequest;
            request.Method = "POST";
            request.Host = ProjectConfiguration.VEECHost;
            request.KeepAlive = true;
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Referer = postUrl;
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.8");
            request.CookieContainer = VeecSessionCookie;
            request.AllowAutoRedirect = false;

            //request.Headers.Add("X-MicrosoftAjax", "Delta=true");
            //request.Headers.Add("X-Requested-With", "XMLHttpRequest");

            string postData = strPost;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();

            // Get the response.
            response = request.GetResponse() as HttpWebResponse;

            dataStream = response.GetResponseStream();
            reader = new StreamReader(dataStream);
            responseFromServer = reader.ReadToEnd();

            reader.Close();
            dataStream.Close();
            response.Close();

            do2c = new HtmlDocument();
            do2c.LoadHtml(responseFromServer);
            string boundary = "----WebKitFormBoundary" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(postUrl);
            wr.Method = "POST";
            wr.Host = ProjectConfiguration.VEECHost;
            wr.Accept = "application/json, text/javascript, */*; q=0.01";
            wr.Headers.Add("X-Requested-With", "XMLHttpRequest");
            wr.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/48.0.2564.109 Safari/537.36";
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Headers.Add("Accept-Encoding", "gzip, deflate");
            wr.Headers.Add("Accept-Language", "en-US,en;q=0.8");
            wr.CookieContainer = VeecSessionCookie;
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;
            wr.Timeout = -1;
            wr.AllowAutoRedirect = false;

            //wr.Proxy = myProxy;

            Stream rs = wr.GetRequestStream();
            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            rs.Write(boundarybytes, 0, boundarybytes.Length);
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("__EVENTTARGET", "ctl00$ctl00$ContentPlaceHolder1$ExtraButton1");
            nvc.Add("__EVENTARGUMENT", "Click");
            nvc.Add("__VIEWSTATE", do2c.GetElementbyId("__VIEWSTATE").GetAttributeValue("value", ""));
            nvc.Add("__VIEWSTATEGENERATOR", do2c.GetElementbyId("__VIEWSTATEGENERATOR").GetAttributeValue("value", ""));
            nvc.Add("__EVENTVALIDATION", do2c.GetElementbyId("__EVENTVALIDATION").GetAttributeValue("value", ""));
            nvc.Add("ctl00$ctl00$ContentPlaceHolder1$Content$UploadControl1", "{&quot;inputCount&quot;:1}");
            nvc.Add("ContentPlaceHolder1_Content_UploadControl1_TextBoxT_Input", "");
            nvc.Add("ctl00$ctl00$MenuHead", "{&quot;selectedItemIndexPath&quot;:&quot;&quot;,&quot;checkedState&quot;:&quot;&quot;}");
            nvc.Add("ContentPlaceHolder1_Content_ScheduleFormCMB_VI", "1543");
            nvc.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ScheduleFormCMB", "34 NonJ6 - Lighting Upgrade - Business");
            nvc.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ScheduleFormCMB$DDD$L", "1543");
            nvc.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ScheduleFormCMB$DDDState", "{&quot;windowsState&quot;:&quot;1:1:12000:247:205:1:513:142:1:0:0:0&quot;}");
            nvc.Add("ctl00$ctl00$MenuFoot", "{&quot;selectedItemIndexPath&quot;:&quot;&quot;,&quot;checkedState&quot;:&quot;&quot;}");
            nvc.Add("DXScript", "1_16,1_17,1_28,1_66,1_19,1_20,1_22,1_29,1_36,1_225,1_226,1_231,1_228,1_234,1_44");
            nvc.Add("DXCss", "0_1144,1_69,1_71,0_1148,1_251,0_1002,1_250,0_1006,../../App_Themes/Glass/Public170801.css,../../App_Themes/Glass/Chart/styles.css,../../App_Themes/Glass/Editors/sprite.css,../../App_Themes/Glass/Editors/styles.css,../../App_Themes/Glass/GridView/sprite.css,../../App_Themes/Glass/GridView/styles.css,../../App_Themes/Glass/HtmlEditor/sprite.css,../../App_Themes/Glass/HtmlEditor/styles.css,../../App_Themes/Glass/Portlets.css,../../App_Themes/Glass/Scheduler/sprite.css,../../App_Themes/Glass/Scheduler/styles.css,../../App_Themes/Glass/SpellChecker/styles.css,../../App_Themes/Glass/TreeList/sprite.css,../../App_Themes/Glass/TreeList/styles.css,../../App_Themes/Glass/Web/sprite.css,../../App_Themes/Glass/Web/styles.css");

            foreach (string key in nvc.Keys)
            {

                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, "ContentPlaceHolder1_Content_UploadControl1_TextBoxT_Input", "", "application/octet-stream");
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);
            rs.Write(boundarybytes, 0, boundarybytes.Length);
            //header = string.Format(headerTemplate, "ContentPlaceHolder1_Content_UploadControl1_TextBox0_Input", Path.GetFileName(@"C:\Users\pct113\Desktop\12407_34_CommercialLightingUpgrade.xlsx"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            //header = string.Format(headerTemplate, "ContentPlaceHolder1_Content_UploadControl1_TextBox0_Input", Path.GetFileName(@"D:\Projects\FormBot\SourceCode\FormBot01082017\FormBot.Main\VeecDocuments\12407_34_CommercialLightingUpgrade.xlsx"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            //header = string.Format(headerTemplate, "ContentPlaceHolder1_Content_UploadControl1_TextBox0_Input", Path.GetFileName(@"C:\Users\pct113\Desktop\RAM user roles.txt"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            header = string.Format(headerTemplate, "ContentPlaceHolder1_Content_UploadControl1_TextBox0_Input", Path.GetFileName(FilePath), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);
            //FileStream fileStream = new FileStream(@"C:\Users\pct113\Desktop\12407_34_CommercialLightingUpgrade.xlsx", FileMode.Open, FileAccess.Read);
            //FileStream fileStream = new FileStream(@"D:\Projects\FormBot\SourceCode\FormBot01082017\FormBot.Main\VeecDocuments\12407_34_CommercialLightingUpgrade.xlsx", FileMode.Open, FileAccess.Read);
            //FileStream fileStream = new FileStream(@"C:\Users\pct113\Desktop\RAM user roles.txt", FileMode.Open, FileAccess.Read);
            FileStream fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();
            rs.Write(boundarybytes, 0, boundarybytes.Length);
            StreamReader Reader = new StreamReader(Request.InputStream, Encoding.Default);
            string bodyString = Reader.ReadToEnd();

            int length = bodyString.Length; // (If you still need this.)
            rs.Close();
            HttpWebResponse wresp = null;
            try
            {

                wresp = (HttpWebResponse)wr.GetResponse();
                Stream responseStream = responseStream = wresp.GetResponseStream();

                if (wresp.ContentEncoding.ToLower().Contains("gzip"))
                    responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                else if (wresp.ContentEncoding.ToLower().Contains("deflate"))
                    responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);

                Reader = new StreamReader(responseStream, Encoding.Default);
                string json = Reader.ReadToEnd();

                do2c = new HtmlDocument();
                do2c.LoadHtml(json);

                if (do2c.GetElementbyId("ContentPlaceHolder1_UserMessage_Panel1").GetAttributeValue("class", string.Empty).ToLower() == "content-um-warning" || do2c.GetElementbyId("ContentPlaceHolder1_UserMessage_Panel1").GetAttributeValue("class", string.Empty).ToLower() == "content-um-error")
                {
                    string msg = do2c.GetElementbyId("ContentPlaceHolder1_UserMessage_Labelmessge").InnerText;
                    string[] splitMsg = msg.Split(new string[] { "  " }, StringSplitOptions.None);
                    if (splitMsg.Count() > 1)
                    {
                        for (int i = 2; i < splitMsg.Count(); i++)
                        {
                            ErrorMsg = ErrorMsg + splitMsg[i];
                        }
                    }
                    else
                        ErrorMsg = msg;
                }
                if (String.IsNullOrEmpty(ErrorMsg))
                {
                    GetUploadDetailsFromVeecPortal(veecId,RefNumber);
                    return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false, errorMsg = ErrorMsg }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, errorMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public List<SelectListItem> GetJobAllStage(int userTypeId)
        {
            List<SelectListItem> items = null;
            if (userTypeId == 7 || userTypeId == 9)
            {
                items = _job.GetJobStages().Where(t => t.JobStageID == 3 || t.JobStageID == 4 || t.JobStageID == 9)
                                           .Select(a => new SelectListItem { Text = a.JobStageName, Value = a.JobStageID.ToString() }).ToList();
            }
            else
            {
                items = _job.GetJobStages().Select(a => new SelectListItem { Text = a.JobStageName, Value = a.JobStageID.ToString() }).ToList();
            }
            return items;
        }

        /// <summary>
        /// This method is used to get VeecList in Grid
        /// </summary>
        public void GetVEECList(string solarcompanyid, string FromDate, string ToDate, string FromDateCommencement, string ToDateCommencement, string FromDateActivity, string ToDateActivity, string Searchtext)
        {

            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            DateTime? fromDate = null, toDate = null, fromDateCommencement = null, toDateCommencement = null, fromDateActivity = null, toDateActivity = null;
            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                fromDate = Convert.ToDateTime(FromDate);
                toDate = Convert.ToDateTime(ToDate);
            }
            if (!string.IsNullOrEmpty(FromDateCommencement) && !string.IsNullOrEmpty(ToDateCommencement))
            {
                fromDateCommencement = Convert.ToDateTime(FromDate);
                toDateCommencement = Convert.ToDateTime(ToDate);
            }
            if (!string.IsNullOrEmpty(FromDateActivity) && !string.IsNullOrEmpty(ToDateActivity))
            {
                fromDateActivity = Convert.ToDateTime(FromDate);
                toDateActivity = Convert.ToDateTime(ToDate);
            }
            IList<FormBot.Entity.VEECList> lstVEEC = _createVeecBAL.GetVEECList(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, solarcompanyid, fromDate, toDate, fromDateCommencement, toDateCommencement, fromDateActivity, toDateActivity, Searchtext);
            if (lstVEEC.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstVEEC.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstVEEC.FirstOrDefault().TotalRecords;
                lstVEEC.ToList().ForEach(a => a.UserID = ProjectSession.LoggedInUserId);
            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstVEEC, gridParam));

        }

        [HttpPost]
        public JsonResult AddEditArea(string areaname, int VEECId, int veecareaid, string veecpreareaName)
        {
            CreateVEEC createVeec = new CreateVEEC();
            VEECArea VEECArea = new VEECArea();
            VEECArea.Name = areaname;
            VEECArea.VEECId = VEECId;
            VEECArea.VEECAreaId = veecareaid;
            if (ProjectSession.LoggedInUserId == 0)
            {
                return Json(new { status = false, isLogout = 0 }, JsonRequestBehavior.AllowGet); //return RedirectToAction("Logout", "Account");
            }
            Int32 VEECAreaId = _createVeecBAL.InsertVeecArea(VEECArea);
            //Create VEEC Folder in Physical Path 
            CreateAreaFolder(areaname, VEECId, veecpreareaName);
            //Get List Of Area
            createVeec.lstArea = LoadAllFolderPhotoVeec(VEECId);
            //Get VisitList and ChecklistItem of VEEC

            VEECDetail veecDetail = new VEECDetail();
            veecDetail.VEECId = VEECId;
            createVeec.VEECDetail = veecDetail;
            var veecPhotoView = ControlToString("~/Views/Veec/_VeecPhotosNew.cshtml", createVeec);
            JsonResult json = new JsonResult();
            json.Data = new { veecPhotoView };
            json.MaxJsonLength = Int32.MaxValue;
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return json;
        }

        public void CreateAreaFolder(string areaName, int VEECId, string veecAreaPreName)
        {

            string areaFolder = ProjectSession.ProofDocuments;
            string areaFolderURL = ProjectSession.ProofDocumentsURL;
            string preareaFolder = Path.Combine(areaFolder + "\\VeecDocuments\\" + VEECId + "\\VeecsPhotos" + "\\" + veecAreaPreName);
            areaFolder = Path.Combine(areaFolder + "\\VeecDocuments\\" + VEECId + "\\VeecsPhotos" + "\\" + areaName);
            //Area Edit then change Folder name 
            if (veecAreaPreName != " " && veecAreaPreName != null)
            {
                Directory.Move(preareaFolder, areaFolder);
                return;
            }
            //Area add then create Folder 
            DataSet folderDs = _createVeecBAL.GetVeecFolderStructure();
            DataTable folderTable = folderDs.Tables[0];
            foreach (DataRow dr in folderTable.Rows)
            {
                if (Convert.ToString(dr["FolderName"]).Equals("Decommisioned Equipment"))
                {
                    string Decommisioned_EquipmentFolder = Path.Combine(ProjectSession.ProofDocuments + "\\VeecDocuments\\" + VEECId + "\\VeecsPhotos" + "\\Decommisioned Equipment");
                    if (!Directory.Exists(Decommisioned_EquipmentFolder))
                    {
                        Directory.CreateDirectory(Decommisioned_EquipmentFolder);
                    }
                    continue;
                }
                string areaFolderstrcture = "";
                if (dr["ParentId"] == null)
                {
                    areaFolderstrcture = Path.Combine(areaFolder + "\\" + Convert.ToString(dr["FolderName"]));
                }
                else
                {
                    areaFolderstrcture = Path.Combine(areaFolder + "\\" + Convert.ToString(dr["FolderName"]) + "\\" + Convert.ToString(dr["ChildFolderName"]));
                }
                if (!Directory.Exists(areaFolderstrcture))
                {
                    Directory.CreateDirectory(areaFolderstrcture);
                }
            }

        }

       
        /// <summary>
        /// This Method is used for Get Area Name and Show in grid
        /// </summary>
        /// <param name="VEECId"></param>
        public void GetAreaNameRecords(string VEECId)
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            Int64 VEECIds = !string.IsNullOrEmpty(VEECId) ? Convert.ToInt64(VEECId) : 0;
            IList<VEECArea> lstAreaName = _createVeecBAL.GetVEECAreaNameRecords(VEECIds, gridParam.SortCol, gridParam.SortDir);

            HttpContext.Response.Write(Grid.PrepareDataSet(lstAreaName, gridParam));
        }

        [HttpPost]
        public ActionResult DeleteVEECAreaName(string id, string veecId)
        {
            CreateVEEC createVeec = new CreateVEEC();
            Int64 VEECAreaId = !string.IsNullOrEmpty(id) ? Convert.ToInt64(id) : 0;
            _createVeecBAL.DeleteVEECAreaName(VEECAreaId);
            //Load Veecphoto Partial view 
            createVeec.lstArea = LoadAllFolderPhotoVeec(Convert.ToInt32(veecId));
            var veecPhotoView = ControlToString("~/Views/Veec/_VeecPhotosNew.cshtml", createVeec);
            return Json(new { success = true, veecPhotoView });
        }

        public bool CheckIfAreaNameExists(string name)
        {
            IList<VEECArea> lstNameExists = _createVeecBAL.CheckIfNameExists(name);
            if (lstNameExists.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpGet]
        public JsonResult GetAreaNameList(string VEECId)
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            Int64 VEECIds = !string.IsNullOrEmpty(VEECId) ? Convert.ToInt64(VEECId) : 0;
            List<SelectListItem> items = _createVeecBAL.GetVEECAreaNameRecords(VEECIds, gridParam.SortCol, gridParam.SortDir).Select(a => new SelectListItem { Text = a.Name, Value = a.VEECAreaId.ToString() }).ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This Method is used for reload Partial view after visit save 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isCheckListView"></param>
        /// <param name="isReloadGridView"></param>
        /// <param name="solarCompanyId"></param>
        /// <returns></returns>
        public JsonResult ReloadSectionOnVisitSave(string id = null, bool isCheckListView = false, bool isReloadGridView = false, int solarCompanyId = 0)
        {
            //WriteToLogFile("StartTime (Get ReloadSectionOnVisitSave) :" + DateTime.Now);

            int veecId = 0;
            if (!string.IsNullOrEmpty(id))
            {
                int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out veecId);
            }


            VEECScheduling veecScheduling = new VEECScheduling();

            //load Visit Partial View
            veecScheduling = _veecSchedule.GetAllSchedulingDataOfVEEC(id, isCheckListView, isReloadGridView, _veec);
            var visitPartialView = ControlToString("~/Views/VEECScheduling/_VisitGridView.cshtml", veecScheduling);

            //WriteToLogFile("EndTime (Get VisitGridView) :" + DateTime.Now);

            List<CustomDetail> lstCustomField = _veec.GetVeecCustomDetails(veecId, solarCompanyId);
            CreateVEEC createVeec = new CreateVEEC();
            VEECDetail veecDetail = new VEECDetail();
            veecDetail.VEECId = veecId;
            createVeec.VEECDetail = veecDetail;
            //createJob.lstCustomDetails = lstCustomField;
            createVeec.lstCustomDetails = ReplaceSeperatorValue(lstCustomField);
            //var customFieldView = ControlToString("~/Views/VEEC/_CustomJobField.cshtml", createVeec);

            //WriteToLogFile("EndTime (Get ReloadCustomFields) :" + DateTime.Now);

            //Load Document and Photo Partial view 
            createVeec.chkPhotosAll = LoadAllPhotosOfNewVeec(veecId);
            var photoView = ControlToString("~/Views/Veec/_VeecCheckListPhotos.cshtml", createVeec);

            //WriteToLogFile("EndTime (Get ReloadJobPhoto) :" + DateTime.Now);

            //createVeec.STCDetailsModel = new STCDetailsModel();
            //createVeec.STCDetailsModel.lstCheckListItem = _job.GetCheckListItemForTrade(jobId);
            //var checkListView = ControlToString("~/Views/Job/_CheckListItemForTrade.cshtml", createVeec.STCDetailsModel);

            //WriteToLogFile("EndTime (Get _CheckListItemForTrade) :" + DateTime.Now);

            //return Json(new { visitPartialView, customFieldView, photoView, checkListView }, JsonRequestBehavior.AllowGet);
            JsonResult json = new JsonResult();
            json.Data = new { visitPartialView, photoView /*checkListView*/ };
            json.MaxJsonLength = Int32.MaxValue;
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return json;
        }

        private string ControlToString(string controlPath, object model)
        {
            RazorView control = new RazorView(this.ControllerContext, controlPath, null, false, null);

            this.ViewData.Model = model;

            HtmlTextWriter writer = new HtmlTextWriter(new System.IO.StringWriter());
            control.Render(new ViewContext(this.ControllerContext, control, this.ViewData, this.TempData, writer), writer);

            string value = ((StringWriter)writer.InnerWriter).ToString();

            return value;
        }

        private List<CustomDetail> ReplaceSeperatorValue(List<CustomDetail> lstCustomDetails)
        {
            if (lstCustomDetails != null && lstCustomDetails.Count() > 0)
            {
                foreach (var item in lstCustomDetails)
                {
                    if (item.FieldValue != null && item.SeparatorId > 0)
                    {
                        if (item.SeparatorId == Convert.ToInt32(SystemEnums.SerialNumberSeparatorId.Comma))
                        {
                            item.FieldValue = item.FieldValue.Replace("\r\n", ",");
                        }
                        if (item.SeparatorId == Convert.ToInt32(SystemEnums.SerialNumberSeparatorId.NewLine))
                        {
                            item.FieldValue = item.FieldValue.Replace("\r\n", Environment.NewLine);
                        }
                        if (item.SeparatorId == Convert.ToInt32(SystemEnums.SerialNumberSeparatorId.Colon))
                        {
                            item.FieldValue = item.FieldValue.Replace("\r\n", ":");
                        }
                        if (item.SeparatorId == Convert.ToInt32(SystemEnums.SerialNumberSeparatorId.SemiColon))
                        {
                            item.FieldValue = item.FieldValue.Replace("\r\n", ";");
                        }
                    }
                }
            }

            return lstCustomDetails;
        }

        public JsonResult AddVEECInstaller(VEECInstaller VEECInstaller)
        {
            try
            {
                int id = 0;
                if (VEECInstaller.UserId != 0)
                {
                    id = _veec.AddVEECInstaller(VEECInstaller.SolarCompanyId, VEECInstaller.UserId, "", "", "", VEECInstaller.ElectricalComplienceNumber);
                }
                else
                {
                    id = _veec.AddVEECInstaller(VEECInstaller.SolarCompanyId, VEECInstaller.UserId, VEECInstaller.FirstName, VEECInstaller.LastName, VEECInstaller.CompanyName, VEECInstaller.ElectricalComplienceNumber, VEECInstaller.ElectricalContractorsLicenseNumber, VEECInstaller.VEECInstallerId);
                }
                return Json(new { VEECInstallerId = id, status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, status = false }, JsonRequestBehavior.AllowGet);
            }

        }
        
        /// <summary>
        /// Update VEEC Installer Detail
        /// </summary>
        /// <param name="veecInstallerId"></param>
        /// <param name="veecId"></param>
        /// <returns></returns>
        public JsonResult UpdateVEECInstallerDetail(string veecInstallerId, string veecId)
        {
            try
            {
                _veec.UpdateVEECInstallerDetail(Convert.ToInt32(veecInstallerId), Convert.ToInt32(veecId));
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetVEECInstaller(string solarCompanyId)
        {
            DataSet ds = _veec.GetVEECIntaller(Convert.ToInt32(solarCompanyId));
            List<VEECInstaller> lstVeecInstaller = ds.Tables[0].DataTableToList<VEECInstaller>();
            return Json(lstVeecInstaller, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult AddVEECUpgradeManager(VEECUpgradeManagerDetail veecUpgradeManagerDetail)
        {
            try
            {
                int veecUpgradeManagerDetailId = _veec.AddVEECUpgradeManager(veecUpgradeManagerDetail);
                return Json(new { status = true, veecUpgradeManagerDetailId = veecUpgradeManagerDetailId }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, veecUpgradeManagerDetailId = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdateVEECUpgradeManager(string veecUpgradeManagerId, string VeecId, bool IsSysUser)
        {
            try
            {
                _veec.UpdateVEECUpgradeManager(Convert.ToInt32(veecUpgradeManagerId), Convert.ToInt32(VeecId), IsSysUser);
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetVEECUpgradeManager(string solarCompanyId)
        {
            DataSet ds = _veec.GetUpgradeManager(Convert.ToInt32(solarCompanyId));
            List<VEECUpgradeManagerDetail> lstVeecUpgradeManager = ds.Tables[0].DataTableToList<VEECUpgradeManagerDetail>();
            return Json(lstVeecUpgradeManager, JsonRequestBehavior.AllowGet);
        }

        public void RequiredValidationField(List<string> Requireddata)
        {
            foreach (var item in Requireddata)
            {
                ModelState.Remove(item);
            }
            //ModelState.Values.AsEnumerable().ToList().ForEach(d =>
            //{
            //    if(Requireddata.Contains(d.Value.))
            //});
            //for (int i=0; i < Requireddata.Count; i++)
            //{

            //}
        }

        /// <summary>
        /// This Method Is Used For Rearrange CalcZone Of Baseline and Upgrade Equipment
        /// </summary>
        /// <param name="VEECId"></param>
        /// <returns></returns>
        public JsonResult RearrangeCalcZone(int VEECId)
        {
            int lastUpdateCalcZoneId = _createVeecBAL.RearrangeCalcZone(VEECId);
            if (lastUpdateCalcZoneId > 0)
            {
                return Json(new { status = true, successMsg = "ReArrange CalcZone Successfully" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, errorMsg = "ReArrange CalcZone Fail " }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This Method is Used for add Baseline Upgrade Equipment
        /// </summary>
        /// <param name="baselineequipment"></param>
        /// <param name="VEECId"></param>
        /// <param name="VEECAreaId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddBaselineequipment(BaselineEquipment baselineequipment, List<string> requiredData, int VEECId, int VEECAreaId)
        {
            baselineequipment.VEECId = VEECId;
            baselineequipment.VEECAreaId = VEECAreaId;

            RequiredValidationField(requiredData);
            if (ModelState.IsValid)
            {
                if (baselineequipment.BaselineUpgrade == 1)
                {
                    Int32 BaselineEquipmentId = _createVeecBAL.InsertBaseLineEquipment(baselineequipment);
                    return Json(new { status = true, save = "Baseline" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Int32 BaselineEquipmentId = _createVeecBAL.InsertBaseLineEquipment(baselineequipment);
                    return Json(new { status = true, save = "Upgrade" }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                string msg = string.Empty;
                ModelState.Values.AsEnumerable().ToList().ForEach(d =>
                {
                    if (d.Errors.Count > 0)
                        msg += d.Errors[0].ErrorMessage;
                });
                return Json(new { status = false, errorMessage = msg }, JsonRequestBehavior.AllowGet);

                //return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        public ActionResult DeleteBaselineUpgradeEquipment(string id)
        {
            Int64 BaselineEquipmentId = !string.IsNullOrEmpty(id) ? Convert.ToInt64(id) : 0;
            _createVeecBAL.DeleteBaselineUpgradeEquipment(BaselineEquipmentId);


            return Json(new { success = true });
        }

        /// <summary>
        /// This Method Is Used For Reload Baseline and Upgrade Grid
        /// </summary>
        /// <param name="id"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ReloadBaselineUpgradeEquipment(string id = null, string areaId = null)
        {
            CreateVEEC createveec = new CreateVEEC();
            if (areaId != "")
            {
                DataSet ds = _createVeecBAL.Lstbaseline(Convert.ToInt32(id), Convert.ToInt32(areaId));
                createveec.lstBaselineEquipment = ds.Tables[0].DataTableToList<BaselineEquipment>();
                List<VEECNonJ6Scenario> lstVeecNonJ6Scenario = ds.Tables[1].DataTableToList<VEECNonJ6Scenario>();
                for (int i = 0; i < createveec.lstBaselineEquipment.Count; i++)
                {
                    createveec.lstBaselineEquipment[i].lstVEECNonJ6Scenario = lstVeecNonJ6Scenario.Where(m => m.BaselineAssetLifetimeReferenceId == createveec.lstBaselineEquipment[i].BaselineAssetLifetimeReference).ToList();
                }
                DataSet ds1 = _createVeecBAL.Lstupgradeline(Convert.ToInt32(id), Convert.ToInt32(areaId));
                createveec.lstUpgradeEquipment = ds1.Tables[0].DataTableToList<BaselineEquipment>();
                for (int i = 0; i < createveec.lstUpgradeEquipment.Count; i++)
                {
                    createveec.lstUpgradeEquipment[i].lstVEECNonJ6Scenario = lstVeecNonJ6Scenario.Where(m => m.UpgradeAssetLifetimeReferenceId == createveec.lstUpgradeEquipment[i].UpgradeAssetLifetimeReference).ToList();
                }
                var baselinePartialView = ControlToString("~/Views/VEEC/_BaselineUpgradeView.cshtml", createveec);
                return Json(new { baselinePartialView }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                var baselinePartialView = "";
                return Json(new { baselinePartialView }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult GetData(List<CommonData> cData)
        {
            DataSet Data = _createVeecBAL.GetData(cData);
            return Json(new { success = true, data = Newtonsoft.Json.JsonConvert.SerializeObject(Data) }, JsonRequestBehavior.AllowGet);
        }

        public int showHideBCAClassificationOnSpaceTypeChange(string spaceTypeId = "")
        {
            int result = _veec.showHideBCAClassificationOnSpaceTypeChange(Convert.ToInt32(spaceTypeId));
            return result;
        }

        public void DownloadVeecPhotos(string veecId, string photos, string refno = null, string isall = "")
        {
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<MainObject>(photos);
            //  Rootobject[] results = (Rootobject[])Newtonsoft.Json.JsonConvert.DeserializeObject(photos);
            // JobPhotosList
            DataSet ds = _veec.GetPhotosPath(obj.vclid, obj.vsid);
            string n2 = "";
            int count = obj.vp.Count;
            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;

                foreach (Vp vp in obj.vp)
                {
                    string VisitId = vp.id.ToString();
                    n2 += "_" + VisitId;
                    zip.AddDirectoryByName(VisitId);
                    foreach (Pl p in vp.pl)
                    {
                        string folderName = p.fn;

                        string path = Path.Combine(VisitId, folderName).Replace("\\", "/");
                        string finalPath = path + "/";
                        int i = 1;
                        while (zip.EntryFileNames.Contains(finalPath))
                        {
                            string renamePath = path + "(" + i + ")" + "/";
                            if (zip.EntryFileNames.Contains(renamePath))
                            {
                                i++;
                            }
                            else
                            {
                                finalPath = renamePath;
                                break;
                            }
                        }
                        zip.AddDirectoryByName(finalPath);
                        //zip.AddDirectoryByName(Path.Combine(VisitId, folderName));

                        if (ds != null && ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0 && !string.IsNullOrEmpty(p.p))
                            {
                                DataRow[] foundRows = ds.Tables[0].Select("VeecVisitCheckListPhotoId in (" + p.p + ")");
                                for (int j = 0; j < foundRows.Length; j++)
                                {

                                    var documentFullPath = Path.Combine(ProjectSession.ProofDocuments, foundRows[j]["Path"].ToString());
                                    if (System.IO.File.Exists(documentFullPath))
                                    {
                                        try
                                        {
                                            //zip.AddFile(documentFullPath, Path.Combine(VisitId, folderName));
                                            zip.AddFile(documentFullPath, finalPath);
                                        }
                                        catch { }
                                    }
                                }
                            }
                            if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0 && !string.IsNullOrEmpty(p.s))
                            {
                                DataRow[] signRows = ds.Tables[1].Select("VisitSignatureId in (" + p.s + ")");
                                for (int j = 0; j < signRows.Length; j++)
                                {

                                    var documentFullPath = Path.Combine(ProjectSession.ProofDocuments, signRows[j]["Path"].ToString());
                                    if (System.IO.File.Exists(documentFullPath))
                                    {
                                        try
                                        {
                                            //zip.AddFile(documentFullPath, Path.Combine(VisitId, folderName));
                                            zip.AddFile(documentFullPath, finalPath);
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    }
                }


                if (obj.isDownloadAll)
                {
                    zip.AddDirectoryByName("Reference Photos");
                    n2 += "_Reference Photos";
                }

                if (!string.IsNullOrEmpty(obj.rp))
                {
                    if (!obj.isDownloadAll)
                    {
                        zip.AddDirectoryByName("Reference Photos");
                        n2 += "_Reference Photos";
                    }

                    DataRow[] signRows1 = ds.Tables[0].Select("VisitCheckListPhotoId in (" + obj.rp + ")");
                    for (int k = 0; k < signRows1.Length; k++)
                    {

                        var documentFullPath = Path.Combine(ProjectSession.ProofDocuments, signRows1[k]["Path"].ToString());
                        if (System.IO.File.Exists(documentFullPath))
                        {
                            try
                            {
                                zip.AddFile(documentFullPath, "Reference Photos");
                            }
                            catch { }
                        }
                    }
                }

                Response.Clear();
                Response.BufferOutput = true;
                Response.ContentType = "application/zip";
                string val = veecId.ToString();

                CreateJob createJob = new CreateJob();
                createJob.BasicDetails = new BasicDetails();
                var name = createJob.BasicDetails.RefNumber;
                string n1 = refno;
                string filename = "";
                if (isall == "true")
                {
                    filename = "" + val + "_JobPhotos.zip";
                    Response.AddHeader("content-disposition", "attachment; filename=\"" + filename + "\"");
                }
                else
                {
                    filename = "" + n1 + '(' + val + ')' + n2 + "_JobPhotos.zip";
                    Response.AddHeader("content-disposition", "attachment; filename=\"" + filename + "\"");
                }
                zip.Save(Response.OutputStream);
                Response.End();
            }
            //return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteCheckListPhotos(string checkListIds, string sigIds, string pdelete, int jobId = 0)
        {
            DataSet dsData = _veec.DeleteCheckListPhotos(checkListIds, sigIds, pdelete);
            return Json("true", JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteVeecPhotos(string veecPhotoId, string veecAreaId, string veecId)
        {
            DataSet dsData = _veec.DeleteVeecPhotos(veecPhotoId, veecAreaId, veecId);
            return Json("true", JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveImage(ImageDetails imgDetails)
        {
            string p = @imgDetails.Src.Split(new string[] { "VeecDocuments\\" }, StringSplitOptions.None)[1].ToString();
            p = @p.Split(new string[] { "?v=" }, StringSplitOptions.None)[0].ToString();
            string OriginalPath = Path.Combine(ProjectSession.ProofDocuments, "VeecDocuments", p);

            string message = "";
            try
            {

                Image img = Image.FromFile(OriginalPath);
                Bitmap t = RotateImage(new Bitmap(img), imgDetails.Angle);
                img.Dispose();
                System.IO.File.Delete(OriginalPath);
                t.Save(OriginalPath, t.RawFormat.Guid.Equals(ImageFormat.Png.Guid) ? ImageFormat.Png : t.RawFormat.Guid.Equals(ImageFormat.Jpeg.Guid) ? ImageFormat.Jpeg : t.RawFormat.Guid.Equals(ImageFormat.Gif.Guid) ? ImageFormat.Gif : ImageFormat.Jpeg);
                return Json(new { result = true });
                //return Json(new { status = true, Data = businessRuleStatus }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message + ".OriginalPath : " + OriginalPath + "error from : " + message }, JsonRequestBehavior.AllowGet);
            }


        }

        private Bitmap RotateImage(Bitmap b, float Angle)
        {
            // The original bitmap needs to be drawn onto a new bitmap which will probably be bigger 
            // because the corners of the original will move outside the original rectangle.
            // An easy way (OK slightly 'brute force') is to calculate the new bounding box is to calculate the positions of the 
            // corners after rotation and get the difference between the maximum and minimum x and y coordinates.
            float wOver2 = b.Width / 2.0f;
            float hOver2 = b.Height / 2.0f;
            float radians = -(float)(Angle / 180.0 * Math.PI);
            // Get the coordinates of the corners, taking the origin to be the centre of the bitmap.
            PointF[] corners = new PointF[]{
            new PointF(-wOver2, -hOver2),
            new PointF(+wOver2, -hOver2),
            new PointF(+wOver2, +hOver2),
            new PointF(-wOver2, +hOver2)
        };

            for (int i = 0; i < 4; i++)
            {
                PointF p = corners[i];
                PointF newP = new PointF((float)(p.X * Math.Cos(radians) - p.Y * Math.Sin(radians)), (float)(p.X * Math.Sin(radians) + p.Y * Math.Cos(radians)));
                corners[i] = newP;
            }

            // Find the min and max x and y coordinates.
            float minX = corners[0].X;
            float maxX = minX;
            float minY = corners[0].Y;
            float maxY = minY;
            for (int i = 1; i < 4; i++)
            {
                PointF p = corners[i];
                minX = Math.Min(minX, p.X);
                maxX = Math.Max(maxX, p.X);
                minY = Math.Min(minY, p.Y);
                maxY = Math.Max(maxY, p.Y);
            }

            // Get the size of the new bitmap.
            SizeF newSize = new SizeF(maxX - minX, maxY - minY);
            // ...and create it.
            Bitmap returnBitmap = new Bitmap((int)Math.Ceiling(newSize.Width), (int)Math.Ceiling(newSize.Height));
            // Now draw the old bitmap on it.
            using (Graphics g = Graphics.FromImage(returnBitmap))
            {
                g.TranslateTransform(newSize.Width / 2.0f, newSize.Height / 2.0f);
                g.RotateTransform(Angle);
                g.TranslateTransform(-b.Width / 2.0f, -b.Height / 2.0f);

                g.DrawImage(b, 0, 0);
            }

            return returnBitmap;
        }

        /// <summary>
        /// This Method Is Used For Upload CheckList Photo
        /// </summary>
        /// <param name="VeecId"></param>
        /// <param name="UserId"></param>
        /// <param name="veecScId"></param>
        /// <param name="folder"></param>
        /// <param name="isRef"></param>
        /// <param name="IsDefault"></param>
        /// <param name="ClassType"></param>
        /// <param name="PdfLocationId"></param>
        /// <param name="ClassTypeId"></param>
        /// <param name="PdfName"></param>
        /// <returns></returns>
        public JsonResult UploadReferencePhoto(string VeecId, string UserId, string veecScId = "", string folder = "", bool isRef = false, bool IsDefault = false, string ClassType = "", string PdfLocationId = "", string ClassTypeId = "", string PdfName = "")
        {
            List<HelperClasses.UploadStatus> uploadStatus = new List<HelperClasses.UploadStatus>();
            if (Request.Files != null && Request.Files.Count != 0)
            {
                int length = Request.Files.Count;
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    string RefPath = string.Empty;
                    uploadStatus.Add(GetReferenceFileUploadPhoto(Request.Files[i], VeecId, isRef, folder, IsDefault, ref RefPath));
                    if (uploadStatus[i].Status == true)
                    {
                        uploadStatus[i].AttachmentID = _veec.InsertReferencePhoto(Convert.ToInt32(VeecId), RefPath, Convert.ToInt32(UserId), isRef ? "" : VeecId, isRef ? "" : folder, IsDefault, ClassType, null);
                    }

                    if (ClassTypeId == "4" && length == (i + 1) && !string.IsNullOrEmpty(PdfName))
                    {
                        UploadPdfAndSave(PdfLocationId, VeecId, folder, PdfName, RefPath, UserId);
                    }
                }
                CommonBAL.SetCacheDataForJobID(ProjectSession.SolarCompanyId, Convert.ToInt32(VeecId));
            }

            return Json(uploadStatus);
        }

        public HelperClasses.UploadStatus GetReferenceFileUploadPhoto(HttpPostedFileBase fileUpload, string VeecId, bool isRef, string cId, bool isDefault, ref string RefPath)
        {
            HelperClasses.UploadStatus uploadStatus = new HelperClasses.UploadStatus();
            uploadStatus.FileName = Request.Files[0].FileName;
            string uploadMimeType = fileUpload.ContentType.Split('/')[0];
            if (fileUpload != null)
            {
                if (fileUpload.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(fileUpload.FileName);
                    string proofDocumentsFolder = ProjectSession.ProofDocuments;
                    string proofDocumentsFolderURL = ProjectSession.ProofDocumentsURL;
                    if (VeecId != null)
                    {
                        if (isRef)
                        {
                            proofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\VeecDocuments\\" + VeecId + "\\ReferencePhotos");
                            proofDocumentsFolderURL = proofDocumentsFolderURL + "\\VeecDocuments\\" + VeecId + "\\ReferencePhotos";
                        }
                        else if (isDefault)
                        {
                            proofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\VeecDocuments\\" + VeecId + "\\DefaultFolder");
                            proofDocumentsFolderURL = proofDocumentsFolderURL + "\\VeecDocuments\\" + VeecId + "\\DefaultFolder";
                        }
                        else
                        {
                            proofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\VeecDocuments\\" + VeecId + "\\ChecklistPhotos\\" + cId);
                            proofDocumentsFolderURL = proofDocumentsFolderURL + "\\VeecDocuments\\" + VeecId + "\\ChecklistPhotos\\" + cId;
                        }

                    }

                    if (!Directory.Exists(proofDocumentsFolder))
                    {
                        Directory.CreateDirectory(proofDocumentsFolder);
                    }

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    try
                    {
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

                        if (isRef)
                        {
                            RefPath = "VeecDocuments" + "\\" + VeecId + "\\ReferencePhotos\\" + fileName.Replace("%", "$");
                        }
                        else if (isDefault)
                        {
                            RefPath = "VeecDocuments" + "\\" + VeecId + "\\DefaultFolder\\" + fileName.Replace("%", "$");
                        }
                        else
                        {
                            RefPath = "VeecDocuments" + "\\" + VeecId + "\\ChecklistPhotos\\" + cId + "\\" + fileName.Replace("%", "$");
                        }


                        string mimeType = MimeMapping.GetMimeMapping(fileName);
                        if (uploadMimeType != "image")
                        {
                            uploadStatus.Status = false;
                            uploadStatus.Message = "NotImage";

                        }
                        else if (fileUpload.FileName.Length > 50)
                        {
                            uploadStatus.Status = false;
                            uploadStatus.Message = "BigName";

                        }
                        else
                        {
                            fileUpload.SaveAs(path);
                            uploadStatus.Status = true;
                            uploadStatus.Message = "File Uploaded Successfully.";
                            uploadStatus.FileName = fileName;
                            uploadStatus.MimeType = mimeType;
                            uploadStatus.Path = RefPath;
                        }



                    }
                    catch (Exception)
                    {
                        uploadStatus.Status = false;
                        uploadStatus.Message = "An error occured while uploading. Please try again later.";
                    }

                }
                else
                {
                    uploadStatus.Status = false;
                    uploadStatus.Message = "No data received";
                }

            }
            else
            {
                uploadStatus.Status = false;
                uploadStatus.Message = "No data received";
            }

            return uploadStatus;
        }

        public void UploadPdfAndSave(string type, string VeecId, string VeecVisitChecklistItemId, string PDFName, string ImagePath, string UserId)
        {
            string Type = type == "2" ? "CES" : "OTHER";
            string DocPath = Path.Combine("VeecDocuments", VeecId, Type, VeecVisitChecklistItemId, PDFName.ToLower().Contains(".pdf") ? PDFName : (PDFName + ".pdf"));

            string JsonData = string.Empty;

            string Source = Path.Combine(ConfigurationManager.AppSettings["ProofUploadFolder"].ToString(), ImagePath);
            string Destination = Path.Combine(ConfigurationManager.AppSettings["ProofUploadFolder"].ToString(), DocPath);

            //string FolderPath = Path.Combine("JobDocuments", JobId, Type, VisitChecklistItemId);
            //string DestinationFolder = Path.Combine(ConfigurationManager.AppSettings["ProofUploadFolder"].ToString(), FolderPath);
            //bool isFolderExists = Directory.Exists(DestinationFolder);

            bool isFileExists = System.IO.File.Exists(Destination);
            DataSet dsFiles = _veec.GetCesPhotosByVEECVisitCheckListId(Convert.ToInt32(VeecVisitChecklistItemId));
            Helper.Helper.Common.generatePDFfromImage(dsFiles.Tables[0], Destination);

            //delete pdf file
            if (dsFiles.Tables[0].Rows.Count < 1)
            {

            }

            if (!isFileExists)
            {
                //if (!isFolderExists)
                {
                    // _veec.InsertCESDocuments(Convert.ToInt32(VeecId), DocPath, Convert.ToInt32(UserId), Type, JsonData);
                }
            }
        }

        public ActionResult DownloadVeecFolderPhotos(string veecId, string photos, string isall = "")
        {

            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<MainObject>(photos);
            //check if Download All Photo or not
            if (obj.isDownloadAll)
            {
                //string path = @"D:\Projects\FormBot\SourceCode\FormBot01082017\FormBot.Main\VEECDocuments\" + veecId + @"\VeecsPhotos";

                using (ZipFile zip = new ZipFile())
                {
                    DataSet lstVeecPhotos = _createVeecBAL.GetVeecsPhotos(Convert.ToInt32(veecId));
                    List<VEECArea> lstArea = new List<VEECArea>();


                    if (lstVeecPhotos.Tables.Count > 0)
                    {
                        DataTable veecArea = lstVeecPhotos.Tables[0];
                        DataTable veecPhotos = lstVeecPhotos.Tables[1];
                        DataTable veecFolder = lstVeecPhotos.Tables[2];

                        foreach (DataRow dr in veecArea.Rows)
                        {
                            if (!Convert.ToBoolean(dr["IsDeleted"]))
                            {

                                int VEECAreaId = Convert.ToInt32(dr["VEECAreaId"]);
                                string areaName = Convert.ToString(dr["Name"]);
                                foreach (DataRow dtrow in veecFolder.Rows)
                                {

                                    if (dtrow["ParentId"] == DBNull.Value)
                                    {
                                        string FolderName = Convert.ToString(dtrow["FolderName"]);
                                        int folderId = Convert.ToInt32(dtrow["VeecFolderId"]);
                                        DataRow[] subFolder = veecFolder.Select("ParentId=" + folderId + "");
                                        if (subFolder.Length == 0)
                                        {
                                            if (!zip.ContainsEntry(areaName + "\\" + FolderName))
                                            {
                                                zip.AddDirectoryByName(areaName + "\\" + FolderName);
                                            }

                                            DataRow[] veecPhotoRow = veecPhotos.Select("FolderId = " + folderId + " and VeecAreaId = " + VEECAreaId);
                                            foreach (DataRow photoRow in veecPhotoRow)
                                            {
                                                if (!Convert.ToBoolean(photoRow["IsDeleted"]))
                                                {
                                                    zip.AddFile(Path.Combine(ProjectSession.ProofDocuments, Convert.ToString(photoRow["FullPath"])), areaName + "\\" + FolderName);
                                                }

                                            }
                                        }
                                        foreach (DataRow subFolderRow in subFolder)
                                        {
                                            string subFolderName = Convert.ToString(subFolderRow["FolderName"]);
                                            if (!zip.ContainsEntry(areaName + "\\" + FolderName + "\\" + subFolderName))
                                            {
                                                zip.AddDirectoryByName(areaName + "\\" + FolderName + "\\" + subFolderName);
                                            }
                                            int subFolderId = Convert.ToInt32(subFolderRow["VeecFolderId"]);
                                            DataRow[] veecPhotoRow = veecPhotos.Select("FolderId = " + subFolderId + " and VeecAreaId = " + VEECAreaId);
                                            foreach (DataRow photoRow in veecPhotoRow)
                                            {
                                                if (!Convert.ToBoolean(photoRow["IsDeleted"]))
                                                {
                                                    zip.AddFile(Path.Combine(ProjectSession.ProofDocuments, Convert.ToString(photoRow["FullPath"])), areaName + "\\" + FolderName + "\\" + subFolderName);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    Response.Clear();
                    Response.BufferOutput = false;
                    string zipName = String.Format("Zip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
                    Response.ContentType = "application/zip";
                    Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
                    zip.Save(Response.OutputStream);
                    Response.End();
                }
            }
            else
            {
                DataSet ds = _veec.GetVeecPhotosPath(obj.vclid);

                if (ds.Tables.Count > 0)
                {
                    DataTable veecPathDatatable = ds.Tables[0];
                    DataTable veecFolderDataTable = ds.Tables[1];
                    //check if only one photo download
                    if (veecPathDatatable.Rows.Count == 1)
                    {

                        byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(ProjectSession.ProofDocuments, Convert.ToString(veecPathDatatable.Rows[0]["FullPath"])));
                        string fileName = Convert.ToString(veecPathDatatable.Rows[0]["FullPath"]);
                        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

                    }
                    else
                    {
                        using (ZipFile zip = new ZipFile())
                        {
                            foreach (DataRow dr in veecPathDatatable.Rows)
                            {
                                int folderId = Convert.ToInt32(dr["FolderId"]);
                                DataRow[] folderRow = veecFolderDataTable.Select("VeecFolderId = " + folderId);
                                foreach (DataRow row in folderRow)
                                {
                                    //check folder is parentFolder or Subfolder
                                    if (row["Parentid"].Equals(DBNull.Value))
                                    {
                                        string dirName = new DirectoryInfo(Path.GetDirectoryName(Convert.ToString(dr["FullPath"]))).Name;
                                        string areaName = new DirectoryInfo(Path.GetDirectoryName(Path.GetDirectoryName(Convert.ToString(dr["FullPath"])))).Name;
                                        if (!zip.ContainsEntry(areaName + "\\" + dirName))
                                        {
                                            zip.AddDirectoryByName(areaName + "\\" + dirName);
                                        }
                                        zip.AddFile(Path.Combine(ProjectSession.ProofDocuments, (Convert.ToString(dr["FullPath"]))), areaName + "\\" + dirName);
                                    }
                                    else
                                    {
                                        string subdirName = new DirectoryInfo(Path.GetDirectoryName(Convert.ToString(dr["FullPath"]))).Name;
                                        string dirName = new DirectoryInfo(Path.GetDirectoryName(Path.GetDirectoryName(Convert.ToString(dr["FullPath"])))).Name;
                                        string areaName = new DirectoryInfo(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Convert.ToString(dr["FullPath"]))))).Name;
                                        if (!zip.ContainsEntry(areaName + "\\" + dirName + "\\" + subdirName))
                                        {
                                            zip.AddDirectoryByName(areaName + "\\" + dirName + "\\" + subdirName);
                                        }
                                        zip.AddFile(Path.Combine(ProjectSession.ProofDocuments, (Convert.ToString(dr["FullPath"]))), areaName + "\\" + dirName + "\\" + subdirName);
                                    }
                                }

                            }
                            Response.Clear();
                            Response.BufferOutput = false;
                            string zipName = String.Format("Zip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
                            Response.ContentType = "application/zip";
                            Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
                            zip.Save(Response.OutputStream);
                            Response.End();
                        }
                    }

                }

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult UploadVeecPhoto(string VeecId, string UserId, string veecAreaId = "", string folder = "", string veecAreaName = "", string folderName = "", string parentFolder = "")
        {
            CreateVEEC createVeec = new CreateVEEC();
            List<HelperClasses.UploadStatus> uploadStatus = new List<HelperClasses.UploadStatus>();
            if (Request.Files != null && Request.Files.Count != 0)
            {
                int length = Request.Files.Count;
                for (var i = 0; i < Request.Files.Count; i++)
                {

                    string RefPath = string.Empty;
                    uploadStatus.Add(GetVeecUploadPhoto(Request.Files[i], VeecId, folderName, ref RefPath, veecAreaName, parentFolder));
                    if (uploadStatus[i].Status == true)
                    {
                        uploadStatus[i].AttachmentID = _veec.InsertVeecPhoto(Convert.ToInt32(VeecId), RefPath, Convert.ToInt32(UserId), Convert.ToInt32(veecAreaId), Convert.ToInt32(folder), null);
                    }
                }

            }
            return Json(uploadStatus);
        }

        public HelperClasses.UploadStatus GetVeecUploadPhoto(HttpPostedFileBase fileUpload, string VeecId, string folder, ref string RefPath, string veecAreaName, string parentFolder)
        {
            HelperClasses.UploadStatus uploadStatus = new HelperClasses.UploadStatus();
            uploadStatus.FileName = Request.Files[0].FileName;
            string uploadMimeType = fileUpload.ContentType.Split('/')[0];
            if (fileUpload != null)
            {
                if (fileUpload.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(fileUpload.FileName);
                    string proofDocumentsFolder = ProjectSession.ProofDocuments;
                    string proofDocumentsFolderURL = ProjectSession.ProofDocumentsURL;
                    if (VeecId != null && parentFolder == "")
                    {
                        proofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\VeecDocuments\\" + VeecId + "\\VeecsPhotos\\" + veecAreaName + "\\" + folder);
                        proofDocumentsFolderURL = proofDocumentsFolderURL + "\\VeecDocuments\\" + VeecId + "\\VeecsPhotos\\" + veecAreaName + "\\" + folder;

                    }
                    else if (VeecId != null && parentFolder != "")
                    {
                        proofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\VeecDocuments\\" + VeecId + "\\VeecsPhotos\\" + veecAreaName + "\\" + parentFolder + "\\" + folder);
                        proofDocumentsFolderURL = proofDocumentsFolderURL + "\\VeecDocuments\\" + VeecId + "\\VeecsPhotos\\" + veecAreaName + "\\" + parentFolder + "\\" + folder;
                    }

                    if (!Directory.Exists(proofDocumentsFolder))
                    {
                        Directory.CreateDirectory(proofDocumentsFolder);
                    }

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    try
                    {
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


                        if (VeecId != null && parentFolder == "")
                        {
                            RefPath = "VeecDocuments" + "\\" + VeecId + "\\VeecsPhotos\\" + veecAreaName + "\\" + folder + "\\" + fileName.Replace("%", "$");

                        }
                        else if (VeecId != null && parentFolder != "")
                        {
                            RefPath = "VeecDocuments" + "\\" + VeecId + "\\VeecsPhotos\\" + veecAreaName + "\\" + parentFolder + "\\" + folder + "\\" + fileName.Replace("%", "$");
                        }




                        string mimeType = MimeMapping.GetMimeMapping(fileName);
                        if (uploadMimeType != "image")
                        {
                            uploadStatus.Status = false;
                            uploadStatus.Message = "NotImage";

                        }
                        else if (fileUpload.FileName.Length > 50)
                        {
                            uploadStatus.Status = false;
                            uploadStatus.Message = "BigName";

                        }
                        else
                        {
                            fileUpload.SaveAs(path);
                            uploadStatus.Status = true;
                            uploadStatus.Message = "File Uploaded Successfully.";
                            uploadStatus.FileName = fileName;
                            uploadStatus.MimeType = mimeType;
                            uploadStatus.Path = RefPath;
                        }



                    }
                    catch (Exception)
                    {
                        uploadStatus.Status = false;
                        uploadStatus.Message = "An error occured while uploading. Please try again later.";
                    }

                }
                else
                {
                    uploadStatus.Status = false;
                    uploadStatus.Message = "No data received";
                }

            }
            else
            {
                uploadStatus.Status = false;
                uploadStatus.Message = "No data received";
            }

            return uploadStatus;
        }

        public JsonResult GetProductModel(string ProductBrand)
        {
            List<string> items = _veec.GetProductmodel(ProductBrand);
            List<SelectListItem> productItems = items.Select(x => new SelectListItem()
                                  {
                                      Text = x.ToString(),
                                      Value = x.ToString()
                                  }).ToList();
            return Json(productItems, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [GZipOrDeflate]
        public JsonResult GetVEECListUserWiseColumns(string solarcompanyid = "", string sResellerId = "", bool isarchive = false, string stageid = "", string scheduletype = "", string jobtype = "", string jobpriority = "", string searchtext = "", string fromdate = "", string todate = "", bool IsGst = false, bool jobref = true, bool jobdescription = true, bool jobaddress = true, bool jobclient = true, bool jobstaff = false, bool invoiced = true, bool notinvoiced = true, bool readytotrade = true, bool notreadytotrade = true, bool traded = true, bool nottraded = true, bool preapprovalnotapproved = true, bool preapprovalapproved = true, bool connectioncompleted = true, bool connectionnotcompleted = true, bool ACT = true, bool NSW = true, bool NT = true, bool QLD = true, bool SA = true, bool TAS = true, bool WA = true, bool VIC = true, string preapprovalstatusid = "", string connectionstatusid = "")
        {
            GridParam gridParam = new GridParam();
            gridParam.PageStart = 1;
            gridParam.PageSize = 10;
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);

            int SolarCompanyId = 0;
            if (ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 8)
                SolarCompanyId = ProjectSession.SolarCompanyId;
            else
                SolarCompanyId = !string.IsNullOrEmpty(solarcompanyid) ? Convert.ToInt32(solarcompanyid) : 0;

            int PreApprovalStatusId = !string.IsNullOrEmpty(preapprovalstatusid) ? Convert.ToInt32(preapprovalstatusid) : 0;
            int ConnectionStatusId = !string.IsNullOrEmpty(connectionstatusid) ? Convert.ToInt32(connectionstatusid) : 0;
            int ScheduleType = !string.IsNullOrEmpty(scheduletype) ? Convert.ToInt32((SystemEnums.JobScheduleType)Enum.Parse(typeof(SystemEnums.JobScheduleType), scheduletype).GetHashCode()) : 0;
            int JobType = !string.IsNullOrEmpty(jobtype) ? Convert.ToInt32((SystemEnums.JobType)Enum.Parse(typeof(SystemEnums.JobType), jobtype).GetHashCode()) : 0;
            int JobPriority = !string.IsNullOrEmpty(jobpriority) ? Convert.ToInt32((SystemEnums.JobPriority)Enum.Parse(typeof(SystemEnums.JobPriority), jobpriority).GetHashCode()) : 0;
            int StageId = !string.IsNullOrEmpty(stageid) ? Convert.ToInt32(stageid) : 0;
            DateTime? FromDate = null, ToDate = null;
            if (!string.IsNullOrEmpty(fromdate) && !string.IsNullOrEmpty(todate))
            {
                FromDate = Convert.ToDateTime(fromdate);
                ToDate = Convert.ToDateTime(todate);
            }

            DataSet dsJobsPlusColumns = new DataSet(); ;
            if (ProjectSession.UserTypeId == 7 || ProjectSession.UserTypeId == 9)
            {
                dsJobsPlusColumns = _job.GetJobList_UserWiseColumns(SystemEnums.MenuId.JobView.GetHashCode(), ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, Convert.ToInt32(ConfigurationManager.AppSettings["UrgentJobDay"].ToString()), StageId, SolarCompanyId, isarchive, ScheduleType, JobType, JobPriority, searchtext, FromDate, ToDate, IsGst, jobref, jobdescription, jobaddress, jobclient, jobstaff, invoiced, notinvoiced, readytotrade, notreadytotrade, traded, nottraded, preapprovalnotapproved, preapprovalapproved, connectioncompleted, connectionnotcompleted, ACT, NSW, NT, QLD, SA, TAS, WA, VIC, PreApprovalStatusId, ConnectionStatusId);
            }
            else
            {
                List<UserWiseColumns> listUserWiseColumns = new List<UserWiseColumns>();
                DataSet dsAllColumnsData = new DataSet();

                if (SolarCompanyId == 0)
                    SolarCompanyId = ProjectSession.SolarCompanyId;

                if (SolarCompanyId == -1)
                {
                    #region SolarCompanyID value is "All"
                    int ResellerId = !string.IsNullOrEmpty(sResellerId) ? Convert.ToInt32(sResellerId) : 0;

                    if ((ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 5) && ResellerId == 0)
                        ResellerId = ProjectSession.ResellerId;

                    ISolarCompanyBAL _solarCompanyService = new SolarCompanyBAL();
                    List<int> lstSolarCompanyId = _solarCompanyService.GetSolarCompanyByResellerID(ResellerId).Select(X => X.SolarCompanyId).ToList();
                    if (lstSolarCompanyId != null && lstSolarCompanyId.Count > 0)
                    {
                        #region Call SP for those SolarCompanyId which are not found in CacheData
                        List<int> lstSolarCompanyIdForCachingData = lstSolarCompanyId.Where(X => !CacheConfiguration.IsContainsKey(RedisCacheConfiguration.dsJobIndex + "_" + X)).Select(X => X).ToList();
                        if (lstSolarCompanyIdForCachingData != null && lstSolarCompanyIdForCachingData.Count > 0)
                        {
                            dsAllColumnsData = _job.GetJobList_ForCachingData(string.Join(",", lstSolarCompanyIdForCachingData), ProjectSession.LoggedInUserId, SystemEnums.MenuId.JobView.GetHashCode());
                            if (dsAllColumnsData.Tables.Count > 1 && dsAllColumnsData.Tables[1] != null && dsAllColumnsData.Tables[1].Rows.Count > 0)
                                listUserWiseColumns = DBClient.DataTableToList<UserWiseColumns>(dsAllColumnsData.Tables[1]);

                            if (dsAllColumnsData.Tables[0] != null && dsAllColumnsData.Tables[0].Rows.Count > 0)
                            {
                                List<int> dsSolarCompanyIds = dsAllColumnsData.Tables[0].AsEnumerable().Select(dr => dr.Field<int>("SolarCompanyId")).Distinct().ToList();
                                foreach (int solarCompId in dsSolarCompanyIds)
                                {
                                    DataTable dtSolarCompData = dsAllColumnsData.Tables[0].AsEnumerable().Where(dr => dr.Field<int>("SolarCompanyId") == solarCompId).Select(dr => dr).CopyToDataTable();
                                    CacheConfiguration.Set(RedisCacheConfiguration.dsJobIndex + "_" + solarCompId, dtSolarCompData);
                                }
                            }
                            else
                            {
                                foreach (int iSolarID in lstSolarCompanyIdForCachingData)
                                {
                                    CacheConfiguration.Set(RedisCacheConfiguration.dsJobIndex + "_" + iSolarID, new DataTable());
                                }
                            }
                        }
                        #endregion

                        #region Fetch SolarCompanyWise JobList Data From Cache

                        DataTable dtAllSolarCompanyJobList = new DataTable();
                        foreach (int solarCompId in lstSolarCompanyId)
                        {
                            if (CacheConfiguration.IsContainsKey(RedisCacheConfiguration.dsJobIndex + "_" + solarCompId))
                                dtAllSolarCompanyJobList.Merge(CacheConfiguration.Get<DataTable>(RedisCacheConfiguration.dsJobIndex + "_" + solarCompId));
                        }

                        if (dtAllSolarCompanyJobList != null && dtAllSolarCompanyJobList.Rows.Count > 0)
                        {
                            dsAllColumnsData = new DataSet();
                            dsAllColumnsData.Tables.Add(dtAllSolarCompanyJobList.Copy());
                        }
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    #region Selected specific SolarCompanyId
                    if (CacheConfiguration.IsContainsKey(RedisCacheConfiguration.dsJobIndex + "_" + SolarCompanyId))
                    {
                        dsAllColumnsData.Tables.Add(CacheConfiguration.Get<DataTable>(RedisCacheConfiguration. dsJobIndex + "_" + SolarCompanyId).Copy());
                        //listUserWiseColumns = _job.GetUserWiseColumns(ProjectSession.LoggedInUserId,SystemEnums.MenuId.JobView.GetHashCode());
                    }
                    else
                    {
                        dsAllColumnsData = _job.GetJobList_ForCachingData(Convert.ToString(SolarCompanyId), ProjectSession.LoggedInUserId, SystemEnums.MenuId.JobView.GetHashCode());
                        CacheConfiguration.Set(RedisCacheConfiguration.dsJobIndex + "_" + SolarCompanyId, dsAllColumnsData.Tables[0]);
                        listUserWiseColumns = DBClient.DataTableToList<UserWiseColumns>(dsAllColumnsData.Tables[1]);
                    }
                    #endregion
                }

                #region Fetch UserWiseColumns From Database
                if (listUserWiseColumns.Count == 0)
                    listUserWiseColumns = _job.GetUserWiseColumns(ProjectSession.LoggedInUserId, SystemEnums.MenuId.JobView.GetHashCode());
                #endregion

                ViewBag.JSUserColumnList = listUserWiseColumns;
                ViewBag.ListColumnName = string.Join(",", listUserWiseColumns.Select(X => X.Name));
                ViewBag.ListColumnWidth = string.Join(",", listUserWiseColumns.Select(X => X.Width));

                if (dsAllColumnsData.Tables.Count > 0 && dsAllColumnsData.Tables[0] != null && dsAllColumnsData.Tables[0].Rows.Count > 0)
                {
                    List<string> colNames = new List<string>() { "JobID", "ColorCode", "IsCustomPrice", "IsReadyToTrade", "PriceDay1", "UpFront", "PriceDay3", "PriceDay7", "PriceOnApproval", "PartialPayment", "RapidPay", "OptiPay", "Commercial", "Custom", "InvoiceStc", "TradeStatus", "SystemSize", "JobTypeId", "RefNumber", "JobAddress", "JobTitle", "JobDescription", "FullOwnerCompanyDetails", "StaffName", "InstallerFullName", "DesignerFullName", "ElectricianFullName", "InstallationState", "Priority", "IsPreApprovaApproved", "IsConnectionCompleted", "IsDeleted", "JobStageChangeDate", "CreatedDate", "InstallationDate", "JobNumber", "SSCID", "IsAccept", "CreatedBy" };
                    colNames.AddRange(listUserWiseColumns.Where(X => !colNames.Contains(X.Name)).Select(X => X.Name).ToList());
                    dsJobsPlusColumns.Tables.Add(dsAllColumnsData.Tables[0].DefaultView.ToTable(false, colNames.Distinct().ToArray()));
                }
            }

            List<dynamic> dynamicTable = new List<dynamic>();
            if (dsJobsPlusColumns != null && dsJobsPlusColumns.Tables.Count > 0)
            {
                if (dsJobsPlusColumns.Tables[0] != null && dsJobsPlusColumns.Tables[0].Rows.Count > 0)
                {
                    DataTable dtJobList = new DataTable();

                    if (ProjectSession.UserTypeId == 6)
                        dtJobList = dsJobsPlusColumns.Tables[0].Select("SSCID =" + ProjectSession.LoggedInUserId + " or CreatedBy = " + ProjectSession.LoggedInUserId).CopyToDataTable();
                    else
                        dtJobList = dsJobsPlusColumns.Tables[0];

                    dtJobList.Columns.Add("Id", typeof(System.String));
                    dtJobList.Columns.Add("strInstallationDate", typeof(System.String));
                    dtJobList.Columns.Add("strCreatedDate", typeof(System.String));
                    dtJobList.Columns.Add("strSignatureDate", typeof(System.String));
                    dtJobList.Columns.Add("strInstallerSignatureDate", typeof(System.String));
                    dtJobList.Columns.Add("strDesignerSignatureDate", typeof(System.String));
                    dtJobList.Columns.Add("strElectricianSignatureDate", typeof(System.String));
                    if (!dtJobList.Columns.Contains("Urgent"))
                        dtJobList.Columns.Add("Urgent", typeof(System.Boolean));

                    foreach (DataRow row in dtJobList.Rows)
                    {
                        row["Id"] = QueryString.QueryStringEncode("id=" + Convert.ToString(row["JobID"]));

                        if (dtJobList.Columns.Contains("InstallationDate"))
                            row["strInstallationDate"] = !string.IsNullOrEmpty(Convert.ToString(row["InstallationDate"])) ? Convert.ToDateTime(row["InstallationDate"]).ToString("dd/MM/yyyy") : null;

                        if (dtJobList.Columns.Contains("CreatedDate"))
                            row["strCreatedDate"] = !string.IsNullOrEmpty(Convert.ToString(row["CreatedDate"])) ? Convert.ToDateTime(row["CreatedDate"]).ToString("dd/MM/yyyy") : null;

                        if (dtJobList.Columns.Contains("SignatureDate"))
                            row["strSignatureDate"] = !string.IsNullOrEmpty(Convert.ToString(row["SignatureDate"])) ? Convert.ToDateTime(row["SignatureDate"]).ToString("dd/MM/yyyy") : null;

                        if (dtJobList.Columns.Contains("InstallerSignatureDate"))
                            row["strInstallerSignatureDate"] = !string.IsNullOrEmpty(Convert.ToString(row["InstallerSignatureDate"])) ? Convert.ToDateTime(row["InstallerSignatureDate"]).ToString("dd/MM/yyyy") : null;

                        if (dtJobList.Columns.Contains("DesignerSignatureDate"))
                            row["strDesignerSignatureDate"] = !string.IsNullOrEmpty(Convert.ToString(row["DesignerSignatureDate"])) ? Convert.ToDateTime(row["DesignerSignatureDate"]).ToString("dd/MM/yyyy") : null;

                        if (dtJobList.Columns.Contains("ElectricianSignatureDate"))
                            row["strElectricianSignatureDate"] = !string.IsNullOrEmpty(Convert.ToString(row["ElectricianSignatureDate"])) ? Convert.ToDateTime(row["ElectricianSignatureDate"]).ToString("dd/MM/yyyy") : null;

                        if (!dtJobList.Columns.Contains("Urgent") && dtJobList.Columns.Contains("JobStageChangeDate"))
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(row["JobStageChangeDate"])))
                            {
                                if ((System.DateTime.Now - Convert.ToDateTime(row["JobStageChangeDate"])).TotalDays >= Convert.ToInt32(ConfigurationManager.AppSettings["UrgentJobDay"].ToString()))
                                    row["Urgent"] = 1;
                                else
                                    row["Urgent"] = 0;
                            }
                            else
                                row["Urgent"] = 0;
                        }

                    }

                    dynamicTable = DataTableExtension.ToDynamicList(dtJobList, "JobList");
                    gridParam.TotalDisplayRecords = dsJobsPlusColumns.Tables[0].Rows.Count;
                    gridParam.TotalRecords = dsJobsPlusColumns.Tables[0].Rows.Count;

                }
                if (dsJobsPlusColumns.Tables.Count > 1 && dsJobsPlusColumns.Tables[1] != null && dsJobsPlusColumns.Tables[1].Rows.Count > 0)
                {
                    List<UserWiseColumns> listUserWiseColumns = DBClient.DataTableToList<UserWiseColumns>(dsJobsPlusColumns.Tables[1]);
                    ViewBag.JSUserColumnList = listUserWiseColumns;
                    ViewBag.ListColumnName = string.Join(",", listUserWiseColumns.Select(X => X.Name));
                    ViewBag.ListColumnWidth = string.Join(",", listUserWiseColumns.Select(X => X.Width));
                }
            }

            JsonResult json = new JsonResult();
            json.Data = dynamicTable.OrderByDescending(X => X.JobID);
            json.MaxJsonLength = Int32.MaxValue;
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return json;
        }

        public JsonResult GetUploadDetailsFromVeecPortal(int veecId,string refNumber)
        {
            if (Session["VeecVeecSessionCookie"] != null && (DateTime)Session["VeecSessionCookieTimeout"] <= DateTime.Now)
            {
                Session.Remove("VeecSessionCookie");
            }
            var VeecSessionCookie = (CookieContainer)Session["VeecSessionCookie"];
            if (VeecSessionCookie == null)
            {
                VeecSessionCookie = VEECLogin();
            }

            string postUrl = ProjectConfiguration.VEECUrl + "Accounts/Activities/Uploaded.aspx";

            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;

            request.Method = "GET";
            request.CookieContainer = VeecSessionCookie;

            string[] key = Request.Form.AllKeys;

            CookieContainer ck = VeecSessionCookie;

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream());
            ck.Add(response.Cookies);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();

            HtmlDocument do2c = new HtmlDocument();
            do2c.LoadHtml(responseFromServer);

            Dictionary<string, string> postParameters = new Dictionary<string, string>();

            postParameters.Add("__EVENTTARGET", "ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$PerPageQuantity");
            postParameters.Add("__EVENTARGUMENT", "");
            postParameters.Add("__LASTFOCUS", "");
            postParameters.Add("__VIEWSTATE", do2c.GetElementbyId("__VIEWSTATE").GetAttributeValue("value", ""));
            postParameters.Add("__VIEWSTATEGENERATOR", do2c.GetElementbyId("__VIEWSTATEGENERATOR").GetAttributeValue("value", ""));
            postParameters.Add("__EVENTVALIDATION", do2c.GetElementbyId("__EVENTVALIDATION").GetAttributeValue("value", ""));

            postParameters.Add("ctl00$ctl00$MenuHead", "{&quot;selectedItemIndexPath&quot;:&quot;&quot;,&quot;checkedState&quot;:&quot;&quot;}");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$FilterList", "0");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$FilterList$RB0", "C");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$FilterList$RB1", "U");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$FilterList$RB2", "U");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$PerPageQuantity", "20");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$ActivitiesList", "");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$ActivitiesList$DXFREditorcol2", "");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$ActivitiesList$DXFREditorcol3", "");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$ActivitiesList$DXFREditorcol12", "");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$ActivitiesList$DXFREditorcol13", "");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$ActivitiesList$DXFREditorcol14$State", "{&quot;rawValue&quot;:&quot;N&quot;}");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$ActivitiesList$DXFREditorcol14", "");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$ActivitiesList$DXFREditorcol14$DDDState", "{&quot;windowsState&quot;:&quot;0:0:-1:0:0:0:-10000:-10000:1:0:0:0&quot;}");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$ActivitiesList$DXFREditorcol14$DDD$C", "{&quot;visibleDate&quot;:&quot;10/18/2018&quot;,&quot;initialVisibleDate&quot;:&quot;10/18/2018&quot;,&quot;selectedDates&quot;:[]}");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$ActivitiesList$DXFREditorcol14$DDD$C$FNPState", "{&quot;windowsState&quot;:&quot;0:0:-1:0:0:0:-10000:-10000:1:0:0:0&quot;}");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$ActivitiesList$DXFREditorcol15", "");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$ActivitiesList$DXFREditorcol16", "");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$ActivitiesList$DXFREditorcol17$State", "{&quot;rawValue&quot;:&quot;N&quot;}");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$ActivitiesList$DXFREditorcol17", "");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$ActivitiesList$DXFREditorcol17$DDDState", "{&quot;windowsState&quot;:&quot;0:0:-1:0:0:0:-10000:-10000:1:0:0:0&quot;}");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$ActivitiesList$DXFREditorcol17$DDD$C", "{&quot;visibleDate&quot;:&quot;10/18/2018&quot;,&quot;initialVisibleDate&quot;:&quot;10/18/2018&quot;,&quot;selectedDates&quot;:[]}");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$ActivitiesList$DXFREditorcol17$DDD$C$FNPState", "{&quot;windowsState&quot;:&quot;0:0:-1:0:0:0:-10000:-10000:1:0:0:0&quot;}");
            postParameters.Add("ctl00_ctl00_ContentPlaceHolder1_Content_ActivityList_ActivitiesList_DXFREditorcol19_VI", "");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$ActivitiesList$DXFREditorcol19", "");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$ActivitiesList$DXFREditorcol19$DDDState", "{&quot;windowsState&quot;:&quot;0:0:-1:0:0:0:-10000:-10000:1:0:0:0&quot;}");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$ActivitiesList$DXFREditorcol19$DDD$L$State", "{&quot;CustomCallback&quot;:&quot;&quot;}");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$ActivitiesList$DXFREditorcol19$DDD$L", "");
            postParameters.Add("ctl00$ctl00$ContentPlaceHolder1$Content$ActivityList$ActivitiesList$DXFREditorcol20", "");
            postParameters.Add("ctl00$MenuFoot", "{&quot;selectedItemIndexPath&quot;:&quot;&quot;,&quot;checkedState&quot;:&quot;&quot;}");
            //postParameters.Add("DXCss", "0_1116,1_50,1_51,0_1120,1_16,0_978,1_17,0_974,0_983,0_987,../App_Themes/Glass/Public170801.css,../App_Themes/Glass/Chart/styles.css,../App_Themes/Glass/Editors/sprite.css,../App_Themes/Glass/Editors/styles.css,../App_Themes/Glass/GridView/sprite.css,../App_Themes/Glass/GridView/styles.css,../App_Themes/Glass/HtmlEditor/sprite.css,../App_Themes/Glass/HtmlEditor/styles.css,../App_Themes/Glass/Portlets.css,../App_Themes/Glass/Scheduler/sprite.css,../App_Themes/Glass/Scheduler/styles.css,../App_Themes/Glass/SpellChecker/styles.css,../App_Themes/Glass/TreeList/sprite.css,../App_Themes/Glass/TreeList/styles.css,../App_Themes/Glass/Web/sprite.css,../App_Themes/Glass/Web/styles.css");
            //postParameters.Add("DXScript", "1_304,1_185,1_298,1_211,1_188,1_182,1_290,1_296,1_279,1_209,1_217,1_208,1_206,1_288,1_212,1_198,1_196,1_254,1_256,1_263,1_235,1_248,1_244,1_242,1_251,1_239,1_247,1_201,1_190,1_223,1_207,1_199,1_286,1_270");

            string strPost = string.Empty;
            foreach (var item in postParameters)
            {
                strPost += HttpUtility.UrlEncode(item.Key) + "=" + HttpUtility.UrlEncode(item.Value) + "&";
            }
            strPost = strPost.TrimEnd(new char[] { '&' });

            string postData = string.Empty;
            byte[] byteArray;

            request = (HttpWebRequest)WebRequest.Create(postUrl);
            request.Method = "POST";
            request.Host = ProjectConfiguration.VEECHost;
            request.KeepAlive = true;
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36";
            request.Accept = "*/*";
            request.Referer = postUrl;
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.8");
            request.CookieContainer = VeecSessionCookie;

            // Create POST data and convert it to a byte array.
            postData = strPost;
            byteArray = Encoding.UTF8.GetBytes(postData);

            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);

            HttpWebResponse wresp = null;
            try
            {

                wresp = (HttpWebResponse)request.GetResponse();
                Stream responseStream = responseStream = wresp.GetResponseStream();

                if (wresp.ContentEncoding.ToLower().Contains("gzip"))
                    responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                else if (wresp.ContentEncoding.ToLower().Contains("deflate"))
                    responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);

                StreamReader Reader1 = new StreamReader(responseStream, Encoding.Default);
                string json = Reader1.ReadToEnd().ToString();

                do2c = new HtmlDocument();
                json = System.Text.RegularExpressions.Regex.Unescape(json);
                do2c.LoadHtml(json);

                //List<List<string>> lstData = do2c.DocumentNode.SelectSingleNode("//table[1]/tr[3]/td[2]/div[1]/div[1]/div[2]/table[1]").Descendants("tr").Skip(1).Where(tr => tr.Elements("td").Count() > 1).Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList()).Where(i => i.Count == 14).ToList();
                List<List<string>> lstData = do2c.DocumentNode.SelectSingleNode("//table[@id = 'ctl00_ctl00_ContentPlaceHolder1_Content_ActivityList_ActivitiesList']").Descendants("tr").Skip(1).Where(tr => tr.Elements("td").Count() > 1).Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList()).Where(i => i.Count == 14).ToList();

                DataTable dt = new DataTable("VeecUploadTable");

                foreach (var item in lstData[0])
                {
                    dt.Columns.Add(Regex.Replace(item, @"<[^>]+>|&nbsp;", "").Trim());
                }

                int colCount = dt.Columns.Count;

                int lastRowsInserted = 1;

                for (int i = 1; i < lstData.Count; i++)
                {
                    DataRow dr = dt.NewRow();
                    int lastColInserted = 0;
                    foreach (var item in lstData[i])
                    {
                        dr[lastColInserted] = Regex.Replace(item, @"<[^>]+>|&nbsp;", "").Trim();
                        lastColInserted++;
                    }
                    dt.Rows.Add(dr);
                    lastRowsInserted++;
                }

                DataRow foundRows = dt.AsEnumerable().Where(a => a.Field<string>("Own Reference") == refNumber).FirstOrDefault();

                int veecPortalId = Convert.ToInt32(foundRows[1]);
                int veecUploadId = Convert.ToInt32(foundRows[2]);
                int noOfVEECs = Convert.ToInt32(foundRows[13]);

                _createVeecBAL.VEECUpdateDetails_InsertUpdate(veecPortalId, veecUploadId, noOfVEECs, veecId, false);
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, errorMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult UpdateVeec(int veecPortalId)
        {
            if (Session["VeecVeecSessionCookie"] != null && (DateTime)Session["VeecSessionCookieTimeout"] <= DateTime.Now)
            {
                Session.Remove("VeecSessionCookie");
            }
            var VeecSessionCookie = (CookieContainer)Session["VeecSessionCookie"];
            if (VeecSessionCookie == null)
            {
                VeecSessionCookie = VEECLogin();
            }

            veecPortalId = 6887022;

            //Accounts/Activities/Edit.aspx?ID=6887022

            string postUrl = ProjectConfiguration.VEECUrl + "Accounts/Activities/Edit.aspx?ID=" + veecPortalId.ToString();

            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;

            request.Method = "GET";
            request.CookieContainer = VeecSessionCookie;

            string[] key = Request.Form.AllKeys;

            CookieContainer ck = VeecSessionCookie;

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream());
            ck.Add(response.Cookies);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();

            HtmlDocument do2c = new HtmlDocument();
            do2c.LoadHtml(responseFromServer);

            Dictionary<string, string> postParameters = new Dictionary<string, string>();

            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}