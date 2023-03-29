using FormBot.BAL.Service.CheckList;
using FormBot.BAL.Service.Documents;
using FormBot.Entity.CheckList;
using FormBot.Helper;
using FormBot.Helper.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormBot.Main.Controllers
{
    public class CheckListItemController : Controller
    {
        #region Properties
        private readonly ICheckListItemBAL _checkListItemBAL;
        private readonly IDocumentsBAL _documentsBAL;
        private readonly ILogger _log;
        #endregion

        #region Constructor
        public CheckListItemController(ICheckListItemBAL checkListItemBAL, IDocumentsBAL documentsBAL, ILogger log)
        {
            this._checkListItemBAL = checkListItemBAL;
            this._documentsBAL = documentsBAL;
            this._log = log;
        }
        #endregion

        #region Events

        /// <summary>
        /// Get checkList item dropdownlist.
        /// </summary>
        /// <returns>Items</returns>
        [HttpGet]
        public JsonResult GetCheckListItem(string templateId)
        {
            List<SelectListItem> Items = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(templateId))
            {
                int checkListTemplateId = 0;
                if (!string.IsNullOrEmpty(templateId))
                {
                    int.TryParse(QueryString.GetValueFromQueryString(templateId, "id"), out checkListTemplateId);
                }
                Items = _checkListItemBAL.GetData(checkListTemplateId).Select(a => new SelectListItem { Text = a.ItemName, Value = a.Id.ToString() }).ToList();
            }
            return Json(Items, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSeparator()
        {
            var serialNumberSeparator = from SystemEnums.SerialNumberSeparatorId s in Enum.GetValues(typeof(SystemEnums.SerialNumberSeparatorId))
                                        select new { ID = s.GetHashCode(), Name = s.ToString() };
            return Json(new SelectList(serialNumberSeparator, "ID", "Name"), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCheckListDocument(string jobId)
        {
            List<SelectListItem> Items = _documentsBAL.GetDocumentForCheckListItem(Convert.ToInt32(jobId)).Select(a => new SelectListItem { Text = a.FileName, Value = a.JobDocumentId.ToString() }).ToList();
            return Json(Items, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPDFLocation()
        {
            var PDFLocation = from SystemEnums.CheckListPDFLocation s in Enum.GetValues(typeof(SystemEnums.CheckListPDFLocation))
                              select new { ID = s.GetHashCode(), Name = s.ToString().Replace("_", " ") };
            return Json(new SelectList(PDFLocation, "ID", "Name"), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetCheckListItemByTemplateId(string id, bool isSetFromSetting, int jobSchedulingId = 0, bool isTemplateChange = false, Int64 tempJobSchedulingId = 0, int jobId = 0, string visitCheckListIdsString = "", bool isAddVisit = false, int JobType = 1,int SolarCompanyId=0,bool isFromIsDeletedChecklistItem=false)
        {
            CheckListTemplate checkListTemplate = new CheckListTemplate();
            try
            {
                int templateId = 0;
                if (!string.IsNullOrEmpty(id))
                {
                    int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out templateId);
                }

                checkListTemplate = _checkListItemBAL.GetCheckListItemByTemplateId(templateId, jobSchedulingId, isSetFromSetting, isTemplateChange, tempJobSchedulingId, jobId, !string.IsNullOrEmpty(visitCheckListIdsString) ? visitCheckListIdsString : null);
                checkListTemplate.isSetFromSetting = isSetFromSetting;
                checkListTemplate.JobSchedulingId = jobSchedulingId;

                if (checkListTemplate.JobType == 0)
                    checkListTemplate.JobType = JobType;

                if (isAddVisit)
                {
                    checkListTemplate.CheckListTemplateId = templateId;
                }
                if (SolarCompanyId > 0)
                {
                    DataSet ds = _checkListItemBAL.GetDefaultCheckListTemplateId(JobType, SolarCompanyId);
                    if (ds.Tables.Count > 0)
                    {
                        checkListTemplate.defaultCheckListTemplateName = ds.Tables[0].Rows[0]["CheckListTemplateName"].ToString();
                    }
                }
                checkListTemplate.isFromIsDeletedChecklistItem = isFromIsDeletedChecklistItem;
                if (checkListTemplate.CheckListTemplateName == "" || string.IsNullOrEmpty(checkListTemplate.CheckListTemplateName))
                {
                    //WriteToLogFile(DateTime.Now.ToString() + " enter in getChecklistItemByTempId ,checklistTemplateName: " + checkListTemplate.CheckListTemplateName + " IsFromDeleted:" + checkListTemplate.isFromIsDeletedChecklistItem + " JobschedulingId:" + jobSchedulingId + " IsFromsetting:" + checkListTemplate.isSetFromSetting + " VisitChecklistItemId:" + visitCheckListIdsString);
                    _log.LogException(DateTime.Now.ToString() + " enter in getChecklistItemByTempId ,checklistTemplateName: " + checkListTemplate.CheckListTemplateName + " IsFromDeleted:" + checkListTemplate.isFromIsDeletedChecklistItem + " JobschedulingId:" + jobSchedulingId + " IsFromsetting:" + checkListTemplate.isSetFromSetting + " VisitChecklistItemId:" + visitCheckListIdsString, null);
                }
              
                return PartialView("_CheckListItemList", checkListTemplate);
            }
            catch (Exception ex)
            {
                //WriteToLogFile(DateTime.Now.ToString()+" Exception For GetChecklistItemByTemplateId: " + checkListTemplate.CheckListTemplateName + "JobSchedulingId: " + checkListTemplate.JobSchedulingId);
                _log.LogException(DateTime.Now.ToString() + " Exception For GetChecklistItemByTemplateId: " + checkListTemplate.CheckListTemplateName + "JobSchedulingId: " + checkListTemplate.JobSchedulingId, ex);
                return PartialView("_CheckListItemList", new CheckListTemplate());
            }
        }

        [HttpGet]
        public ActionResult CreateCheckListItemPopup(bool isSetFromSetting, string templateId, string itemId, bool isDefaultTemplate = false, int visitCheckListItemId = 0)
        {
            CheckListItem checkListItem = new CheckListItem();

            if (!string.IsNullOrEmpty(templateId))
            {
                int checkListTemplateId = 0;
                if (!string.IsNullOrEmpty(templateId))
                    int.TryParse(QueryString.GetValueFromQueryString(templateId, "id"), out checkListTemplateId);

                checkListItem.CheckListTemplateId = checkListTemplateId;
            }
            if (!string.IsNullOrEmpty(itemId))
                checkListItem = GetCheckListItemByItemId(itemId, visitCheckListItemId);
            else
            {
                checkListItem.CheckListClassTypeId = 1;
                checkListItem.CheckListItemSelectedId = 0;
                checkListItem.IsNoneFieldMap = false;
                checkListItem.AllowUploadPhotoFromGallary = true;
            }

            checkListItem.isSetFromSetting = isSetFromSetting;
            checkListItem.IsDefaultTemplateItem = isDefaultTemplate;
            return PartialView("_CheckListItem", checkListItem);
        }

        [HttpGet]
        public JsonResult FillCheckListItemByItemId(string itemId)
        {
            CheckListItem checkListItem = new CheckListItem();
            checkListItem = GetCheckListItemByItemId(itemId, 0);
            if (checkListItem != null)
            {
                var data = Newtonsoft.Json.JsonConvert.SerializeObject(checkListItem);
                return Json(new { status = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }

        public CheckListItem GetCheckListItemByItemId(string itemId, int visitCheckListItemId)
        {
            CheckListItem checkListItem = new CheckListItem();
            if (!string.IsNullOrEmpty(itemId))
            {
                int checkListItemId = 0;
                if (!string.IsNullOrEmpty(itemId))
                {
                    int.TryParse(QueryString.GetValueFromQueryString(itemId, "id"), out checkListItemId);
                }
                if (checkListItemId > 0 || visitCheckListItemId > 0)
                {
                    checkListItem = _checkListItemBAL.GetCheckListItemByItemId(checkListItemId, visitCheckListItemId);
                    if (checkListItem.CheckListClassTypeId == 1 || checkListItem.CheckListClassTypeId == 2 || checkListItem.CheckListClassTypeId == 6)
                    {
                        if (checkListItem.IsSameAsTotalPanelAmount)
                            checkListItem.NumberOptions = 1;
                        else if (checkListItem.IsAtLeastOne)
                            checkListItem.NumberOptions = 2;
                        else
                            checkListItem.NumberOptions = 3;
                    }
                    checkListItem.CheckListItemSelectedId = checkListItem.CheckListItemId;
                }
                else
                {
                    checkListItem.CheckListClassTypeId = 1;
                    checkListItem.CheckListItemSelectedId = 0;
                }
            }
            return checkListItem;
        }

        [HttpPost]
        public JsonResult AddEditCheckListItem(CheckListItem checkListItem)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            int insertUpdateId = 0;
            string templateId = string.Empty;

            RemoveRequiredField(checkListItem);

            try
            {
                if (ModelState.IsValid)
                {
                    if (checkListItem.CheckListClassTypeId == 1 || checkListItem.CheckListClassTypeId == 2 || checkListItem.CheckListClassTypeId == 6)
                    {
                        if (checkListItem.NumberOptions == 1)
                        {
                            checkListItem.IsSameAsTotalPanelAmount = true;
                            checkListItem.IsAtLeastOne = false;
                        }
                        else if (checkListItem.NumberOptions == 2)
                        {
                            checkListItem.IsAtLeastOne = true;
                            checkListItem.IsSameAsTotalPanelAmount = false;
                        }
                        else
                        {
                            checkListItem.IsSameAsTotalPanelAmount = false;
                            checkListItem.IsAtLeastOne = false;
                        }

                        //if (checkListItem.CheckListClassTypeId == 1)
                        //    checkListItem.SerialNumFileName = checkListItem.SerialNumFileName + ".txt";
                    }
                    if (checkListItem.CheckListClassTypeId == 3)
                    {
                        if (!(checkListItem.IsOwnerSignature || checkListItem.IsInstallerSignature || checkListItem.IsDesignerSignature || checkListItem.IsElectricianSignature || checkListItem.IsOtherSignature))
                            return Json(new { status = false, error = "Please select at least one signature type", id = 0 }, JsonRequestBehavior.AllowGet);
                    }

                    //if (checkListItem.CheckListClassTypeId == 4)
                    //{
                    //    checkListItem.CaptureUploadImagePDFName = checkListItem.CaptureUploadImagePDFName + ".pdf";
                    //}

                    bool isTempItemAdd = false;
                    if (!checkListItem.isSetFromSetting)
                        isTempItemAdd = true;
                    if (checkListItem.CheckListPhotoTypeId != 2)
                    {
                        checkListItem.SelfieTypeId = null;
                    }
                   
                    insertUpdateId = _checkListItemBAL.CheckListItemInsertUpdate(checkListItem, checkListItem.isSetFromSetting, checkListItem.JobSchedulingId, checkListItem.JobId, isTempItemAdd);

                    if (checkListItem.isSetFromSetting)
                        templateId = QueryString.QueryStringEncode("id=" + Convert.ToString(checkListItem.CheckListTemplateId));
                    else
                        templateId = null;

                    return Json(new { status = true, id = insertUpdateId, templateId = templateId, jobSchedulingId = checkListItem.JobSchedulingId, TempJobSchedulingId = checkListItem.TempJobSchedulingId }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string error = String.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception));
                    return Json(new { status = false, error = error, id = 0 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message, id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult DeleteCheckListItem(string templateId, string itemId, int visitCheckListItemId)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                int checkListItemId = 0;
                if (!string.IsNullOrEmpty(itemId))
                {
                    int.TryParse(QueryString.GetValueFromQueryString(itemId, "id"), out checkListItemId);
                }
                if (checkListItemId > 0 || visitCheckListItemId > 0)
                {
                    _checkListItemBAL.DeleteCheckListItemByItemId(checkListItemId, visitCheckListItemId);
                    //WriteToLogFile(DateTime.Now.ToString()+" DeleteChecklitem Successfully: "+"ChecklistItemId: " + checkListItemId + "visitchecklistItemId: " + visitCheckListItemId +"by UserId: "+ProjectSession.LoggedInUserId);
                    _log.LogException(DateTime.Now.ToString() + " DeleteChecklitem Successfully: " + "ChecklistItemId: " + checkListItemId + "visitchecklistItemId: " + visitCheckListItemId + "by UserId: " + ProjectSession.LoggedInUserId, null);
                    return Json(new { status = true, templateId = templateId, id = checkListItemId, visitCheckListItemId = visitCheckListItemId }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { status = false, id = 0 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message, id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult MarkUnMarkCheckListItem(string templateId, int itemId, bool isMark, int jobSchedulingId)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                int checkListItemId = 0;
                //if (!string.IsNullOrEmpty(itemId))
                //{
                //    int.TryParse(QueryString.GetValueFromQueryString(itemId, "id"), out checkListItemId);
                //}
                if (itemId > 0)
                {
                    _checkListItemBAL.MarkUnMarkCheckListItem(itemId, isMark, jobSchedulingId);
                    return Json(new { status = true, templateId = templateId, id = itemId }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { status = false, id = 0 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message, id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ChangeOrderOfCheckListItem(string templateId, string itemId, int oldIndex, int newIndex)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                int checkListItemId = 0;
                int checkListTemplateId = 0;
                if (!string.IsNullOrEmpty(itemId))
                {
                    int.TryParse(QueryString.GetValueFromQueryString(itemId, "id"), out checkListItemId);
                }
                if (!string.IsNullOrEmpty(templateId))
                {
                    int.TryParse(QueryString.GetValueFromQueryString(templateId, "id"), out checkListTemplateId);
                }
                if (checkListTemplateId > 0 && checkListItemId > 0 && oldIndex > 0 && newIndex > 0)
                {
                    _checkListItemBAL.ChangeOrderOfCheckListItem(checkListItemId, checkListTemplateId, oldIndex, newIndex);
                    return Json(new { status = true, templateId = templateId, id = checkListItemId }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { status = false, id = 0 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message, id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult MoveUPAndDownOrderOfCheckListItem(string templateId, bool isMoveUp)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                //int checkListItemId = 0;
                int checkListTemplateId = 0;
                //if (!string.IsNullOrEmpty(itemId))
                //{
                //    int.TryParse(QueryString.GetValueFromQueryString(itemId, "id"), out checkListItemId);
                //}
                if (!string.IsNullOrEmpty(templateId))
                {
                    int.TryParse(QueryString.GetValueFromQueryString(templateId, "id"), out checkListTemplateId);
                }
                if (checkListTemplateId > 0)
                {
                    _checkListItemBAL.MoveUPAndDownOrderOfCheckListItem(checkListTemplateId, isMoveUp);
                    return Json(new { status = true, templateId = templateId, id = checkListTemplateId }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { status = false, id = 0 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message, id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult MoveUPAndDownOrderOfCheckListItemNew(string strData)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                //int checkListItemId = 0;
                int checkListTemplateId = 0;
                //if (!string.IsNullOrEmpty(itemId))
                //{
                //    int.TryParse(QueryString.GetValueFromQueryString(itemId, "id"), out checkListItemId);
                //}


                _checkListItemBAL.MoveUPAndDownOrderOfCheckListItemNew(strData);
                return Json(new { status = true, id = checkListTemplateId }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message, id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        public void RemoveRequiredField(CheckListItem checkListItem)
        {
            if (checkListItem.CheckListClassTypeId == 3 || checkListItem.CheckListClassTypeId == 5 || checkListItem.CheckListClassTypeId == 6)
            {
                ModelState.Remove("PhotoQualityId");
            }
            if (checkListItem.CheckListPhotoTypeId != 2)
            {
                ModelState.Remove("SelfieTypeId");
            }
            if (checkListItem.CheckListClassTypeId == 1 || checkListItem.CheckListClassTypeId == 6)
            {
                if (checkListItem.NumberOptions != 3)
                {
                    ModelState.Remove("TotalNumber");
                }
                if (checkListItem.CheckListClassTypeId == 6)
                {
                    ModelState.Remove("FolderName");
                }
                if (checkListItem.IsCustomSerialNumField)
                    ModelState.Remove("JobFieldId");
                else
                    ModelState.Remove("CustomFieldId");

                if (checkListItem.IsNoneFieldMap)
                {
                    ModelState.Remove("CustomFieldId");
                    ModelState.Remove("JobFieldId");
                }

                if (!checkListItem.IsSaveCopyofSerialNum)
                    ModelState.Remove("SerialNumFileName");

                RemoveSigntureRequiredField(checkListItem);
                RemoveImagePDFRequiredField(checkListItem);
                RemoveCustomRequiredField(checkListItem);
            }
            else if (checkListItem.CheckListClassTypeId == 2)
            {
                if (checkListItem.NumberOptions != 3)
                {
                    ModelState.Remove("TotalNumber");
                }
                RemoveSerialNumRequiredField(checkListItem);
                RemoveSigntureRequiredField(checkListItem);
                RemoveImagePDFRequiredField(checkListItem);
                RemoveCustomRequiredField(checkListItem);
            }
            else if (checkListItem.CheckListClassTypeId == 3)
            {
                ModelState.Remove("TotalNumber");
                RemoveSerialNumRequiredField(checkListItem);
                RemoveImagePDFRequiredField(checkListItem);
                RemoveCustomRequiredField(checkListItem);
                if (!checkListItem.IsOtherSignature)
                {
                    RemoveSigntureRequiredField(checkListItem);
                }
            }
            else if (checkListItem.CheckListClassTypeId == 4)
            {
                ModelState.Remove("TotalNumber");
                ModelState.Remove("FolderName");
                RemoveSerialNumRequiredField(checkListItem);
                RemoveSigntureRequiredField(checkListItem);
                RemoveCustomRequiredField(checkListItem);
            }

            else if (checkListItem.CheckListClassTypeId == 5)
            {
                ModelState.Remove("TotalNumber");
                ModelState.Remove("FolderName");
                RemoveSerialNumRequiredField(checkListItem);
                RemoveSigntureRequiredField(checkListItem);
                RemoveImagePDFRequiredField(checkListItem);
                if (!checkListItem.IsLinkToDocument)
                {
                    RemoveCustomRequiredField(checkListItem);
                }
            }
            
        }

        public void RemoveSerialNumRequiredField(CheckListItem checkListItem)
        {
            ModelState.Remove("JobFieldId");
            ModelState.Remove("CustomFieldId");
            //ModelState.Remove("SerialNumTitle");
            ModelState.Remove("SerialNumFileName");
            ModelState.Remove("SeparatorId");
        }
        public void RemoveSigntureRequiredField(CheckListItem checkListItem)
        {
            ModelState.Remove("OtherSignName");
            ModelState.Remove("OtherSignLabel");
        }
        public void RemoveImagePDFRequiredField(CheckListItem checkListItem)
        {
            ModelState.Remove("CaptureUploadImagePDFName");
            ModelState.Remove("PDFLocationId");
        }

        public void RemoveCustomRequiredField(CheckListItem checkListItem)
        {
            ModelState.Remove("LinkedDocumentId");
        }

        [HttpGet]
        public JsonResult TempCheckListTemplateItemAdd(string checkListTemplateId, Int64 tempJobSchedulingId, string jobSchedulingId, string jobId)
        {
            try
            {
                int templateId = 0;
                if (!string.IsNullOrEmpty(checkListTemplateId))
                {
                    int.TryParse(QueryString.GetValueFromQueryString(checkListTemplateId, "id"), out templateId);
                }
                string visitCheckListItemIds = string.Empty;
                DataSet dsVisitCheckList = _checkListItemBAL.TempCheckListTemplateItemAdd(templateId, tempJobSchedulingId, Convert.ToInt32(jobSchedulingId), Convert.ToInt32(jobId));
                if (dsVisitCheckList != null && dsVisitCheckList.Tables.Count > 0 && dsVisitCheckList.Tables[0].Rows.Count > 0)
                {
                    visitCheckListItemIds = string.Join(",", dsVisitCheckList.Tables[0].AsEnumerable().Select(a => a["VisitCheckListItemId"].ToString()).ToArray());
                }

                return Json(new { status = true, visitCheckListItemIds = visitCheckListItemIds }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetPhotoQuality()
        {
            var photoQuality = from SystemEnums.PhotoQuality s in Enum.GetValues(typeof(SystemEnums.PhotoQuality))
                                        select new { ID = s.GetHashCode(), Name = s.ToString() };
            return Json(new SelectList(photoQuality, "ID", "Name"), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetSelfiePhotoType()
        {
            var photoType =
                from  SystemEnums.SelfiePhotoType s in Enum.GetValues(typeof(SystemEnums.SelfiePhotoType))
                               select new { ID = s.GetHashCode(), Name =Common.GetDescription((SystemEnums.SelfiePhotoType)s.GetHashCode(),"")};
            return Json(new SelectList(photoType, "ID", "Name"), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDefaultCheckListTemplateId(int JobType,int SolarCompanyId)
        {
            try
            {
                DataSet ds= _checkListItemBAL.GetDefaultCheckListTemplateId(JobType, SolarCompanyId);
                string DefaultTemplateId = "";
                if (ds.Tables.Count > 0)
                {
                    DefaultTemplateId = ds.Tables[0].Rows[0]["CheckListTemplateId"].ToString();
                }
                return Json(new { status = true, DefaultCheckListTemplateId = QueryString.QueryStringEncode("id=" + DefaultTemplateId) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message, id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveDefaultChecklistTemplate(bool isDefault,int jobTypeId)
        {
            try
            {
                _checkListItemBAL.SaveDefaultChecklistTemplate(isDefault, jobTypeId);
                return Json(new { status = true, JsonRequestBehavior.AllowGet });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message, id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}