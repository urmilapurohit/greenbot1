using FormBot.BAL.Service.Documents;
using FormBot.BAL.Service.VEECCheckList;
using FormBot.Entity.VEECCheckList;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormBot.Main.Controllers
{
    public class VEECCheckListItemController : Controller
    {

        #region Properties
        private readonly IVEECCheckListItemBAL _vecccheckListItemBAL;
        private readonly IDocumentsBAL _documentsBAL;
        #endregion

        #region Constructor
        public VEECCheckListItemController(IVEECCheckListItemBAL vecccheckListItemBAL, IDocumentsBAL documentsBAL)
        {
            this._vecccheckListItemBAL = vecccheckListItemBAL;
            this._documentsBAL = documentsBAL;
        }
        #endregion
        // GET: VEECCheckListItem
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public JsonResult GetVEECCheckListItem(string templateId)
        {
            List<SelectListItem> Items = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(templateId))
            {
                int veccCheckListTemplateId = 0;
                if (!string.IsNullOrEmpty(templateId))
                {
                    int.TryParse(QueryString.GetValueFromQueryString(templateId, "id"), out veccCheckListTemplateId);
                }
                Items = _vecccheckListItemBAL.GetData(veccCheckListTemplateId).Select(a => new SelectListItem { Text = a.ItemName, Value = a.Id.ToString() }).ToList();
            }
            return Json(Items, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult TempVEECCheckListTemplateItemAdd(string checkListTemplateId, Int64 tempJobSchedulingId, string jobSchedulingId, string jobId)
        {
            try
            {
                int templateId = 0;
                if (!string.IsNullOrEmpty(checkListTemplateId))
                {
                    int.TryParse(QueryString.GetValueFromQueryString(checkListTemplateId, "id"), out templateId);
                }
                string visitVeecCheckListItemIds = string.Empty;
                DataSet dsVisitCheckList = _vecccheckListItemBAL.TempVEECCheckListTemplateItemAdd(templateId, tempJobSchedulingId, Convert.ToInt32(jobSchedulingId), Convert.ToInt32(jobId));
                if (dsVisitCheckList != null && dsVisitCheckList.Tables.Count > 0 && dsVisitCheckList.Tables[0].Rows.Count > 0)
                {
                    visitVeecCheckListItemIds = string.Join(",", dsVisitCheckList.Tables[0].AsEnumerable().Select(a => a["VisitVEECCheckListItemId"].ToString()).ToArray());
                }

                return Json(new { status = true, visitCheckListItemIds = visitVeecCheckListItemIds }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CreateVEECCheckListItemPopup(bool isSetFromSetting, string templateId, string itemId, bool isDefaultTemplate = false, int visitCheckListItemId = 0)
        {
            VEECCheckListItem checkListItem = new VEECCheckListItem();

            if (!string.IsNullOrEmpty(templateId))
            {
                int checkListTemplateId = 0;
                if (!string.IsNullOrEmpty(templateId))
                    int.TryParse(QueryString.GetValueFromQueryString(templateId, "id"), out checkListTemplateId);

                checkListItem.VEECCheckListTemplateId = checkListTemplateId;
            }
            if (!string.IsNullOrEmpty(itemId))
                checkListItem = GetCheckListItemByItemId(itemId, visitCheckListItemId);
            else
            {
                checkListItem.VEECCheckListClassTypeId = 1;
                checkListItem.CheckListItemSelectedId = 0;
            }

            checkListItem.isSetFromSetting = isSetFromSetting;
            checkListItem.IsDefaultTemplateItem = isDefaultTemplate;
            return PartialView("_VEECCheckListItem", checkListItem);
        }
        [HttpGet]
        public ActionResult GetCheckListItemByTemplateId(string id, bool isSetFromSetting, int jobSchedulingId = 0, bool isTemplateChange = false, Int64 tempJobSchedulingId = 0, int veecid = 0, string visitCheckListIdsString = "", bool isAddVisit = false, int JobType = 1)
        {
            VEECCheckListTemplate checkListTemplate = new VEECCheckListTemplate();
            try
            {
                int templateId = 0;
                if (!string.IsNullOrEmpty(id))
                {
                    int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out templateId);
                }

                checkListTemplate = _vecccheckListItemBAL.GetCheckListItemByTemplateId(templateId, jobSchedulingId, isSetFromSetting, isTemplateChange, tempJobSchedulingId, veecid, !string.IsNullOrEmpty(visitCheckListIdsString) ? visitCheckListIdsString : null);
                checkListTemplate.isSetFromSetting = isSetFromSetting;
                checkListTemplate.JobSchedulingId = jobSchedulingId;

                if (checkListTemplate.JobType == 0)
                    checkListTemplate.JobType = JobType;

                if (isAddVisit)
                {
                    checkListTemplate.VEECCheckListTemplateId = templateId;
                }

                return PartialView("_VEECCheckListItemList", checkListTemplate);
            }
            catch (Exception ex)
            {
                return PartialView("_VEECCheckListItemList", new VEECCheckListTemplate());
            }
        }

        public VEECCheckListItem GetCheckListItemByItemId(string itemId, int visitCheckListItemId)
        {
            VEECCheckListItem checkListItem = new VEECCheckListItem();
            if (!string.IsNullOrEmpty(itemId))
            {
                int checkListItemId = 0;
                if (!string.IsNullOrEmpty(itemId))
                {
                    int.TryParse(QueryString.GetValueFromQueryString(itemId, "id"), out checkListItemId);
                }
                if (checkListItemId > 0 || visitCheckListItemId > 0)
                {
                    checkListItem = _vecccheckListItemBAL.GetCheckListItemByItemId(checkListItemId, visitCheckListItemId);
                    if (checkListItem.VEECCheckListClassTypeId == 1 || checkListItem.VEECCheckListClassTypeId == 2)
                    {
                        if (checkListItem.IsSameAsTotalPanelAmount)
                            checkListItem.NumberOptions = 1;
                        else if (checkListItem.IsAtLeastOne)
                            checkListItem.NumberOptions = 2;
                        else
                            checkListItem.NumberOptions = 3;
                    }
                    checkListItem.CheckListItemSelectedId = checkListItem.VEECCheckListItemId;
                }
                else
                {
                    checkListItem.VEECCheckListClassTypeId = 1;
                    checkListItem.CheckListItemSelectedId = 0;
                }
            }
            return checkListItem;
        }


        [HttpPost]
        public JsonResult AddEditCheckListItem(VEECCheckListItem checkListItem)
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
                    if (checkListItem.VEECCheckListClassTypeId == 1 || checkListItem.VEECCheckListClassTypeId == 2)
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
                    if (checkListItem.VEECCheckListClassTypeId == 3)
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

                    insertUpdateId = _vecccheckListItemBAL.VEECCheckListItemInsertUpdate(checkListItem, checkListItem.isSetFromSetting, checkListItem.JobSchedulingId, checkListItem.JobId, isTempItemAdd);

                    if (checkListItem.isSetFromSetting)
                        templateId = QueryString.QueryStringEncode("id=" + Convert.ToString(checkListItem.VEECCheckListTemplateId));
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


        public void RemoveRequiredField(VEECCheckListItem checkListItem)
        {
            if (checkListItem.VEECCheckListClassTypeId == 3 || checkListItem.VEECCheckListClassTypeId == 5)
            {
                ModelState.Remove("PhotoQualityId");
            }

            if (checkListItem.VEECCheckListClassTypeId == 1)
            {
                if (checkListItem.NumberOptions != 3)
                {
                    ModelState.Remove("TotalNumber");
                }

                if (checkListItem.IsCustomSerialNumField)
                    ModelState.Remove("JobFieldId");
                else
                    ModelState.Remove("CustomFieldId");

                if (!checkListItem.IsSaveCopyofSerialNum)
                    ModelState.Remove("SerialNumFileName");

                RemoveSigntureRequiredField(checkListItem);
                RemoveImagePDFRequiredField(checkListItem);
                RemoveCustomRequiredField(checkListItem);
            }
            else if (checkListItem.VEECCheckListClassTypeId == 2)
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
            else if (checkListItem.VEECCheckListClassTypeId == 3)
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
            else if (checkListItem.VEECCheckListClassTypeId == 4)
            {
                ModelState.Remove("TotalNumber");
                ModelState.Remove("FolderName");
                RemoveSerialNumRequiredField(checkListItem);
                RemoveSigntureRequiredField(checkListItem);
                RemoveCustomRequiredField(checkListItem);
            }

            else if (checkListItem.VEECCheckListClassTypeId == 5)
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



        public void RemoveSerialNumRequiredField(VEECCheckListItem checkListItem)
        {
            ModelState.Remove("JobFieldId");
            ModelState.Remove("CustomFieldId");
            //ModelState.Remove("SerialNumTitle");
            ModelState.Remove("SerialNumFileName");
            ModelState.Remove("SeparatorId");
        }
        public void RemoveSigntureRequiredField(VEECCheckListItem checkListItem)
        {
            ModelState.Remove("OtherSignName");
            ModelState.Remove("OtherSignLabel");
        }
        public void RemoveImagePDFRequiredField(VEECCheckListItem checkListItem)
        {
            ModelState.Remove("CaptureUploadImagePDFName");
            ModelState.Remove("PDFLocationId");
        }

        public void RemoveCustomRequiredField(VEECCheckListItem checkListItem)
        {
            ModelState.Remove("LinkedDocumentId");
        }

        [HttpGet]
        public JsonResult GetSeparator()
        {
            var serialNumberSeparator = from SystemEnums.SerialNumberSeparatorId s in Enum.GetValues(typeof(SystemEnums.SerialNumberSeparatorId))
                                        select new { ID = s.GetHashCode(), Name = s.ToString() };
            return Json(new SelectList(serialNumberSeparator, "ID", "Name"), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPhotoQuality()
        {
            var photoQuality = from SystemEnums.PhotoQuality s in Enum.GetValues(typeof(SystemEnums.PhotoQuality))
                               select new { ID = s.GetHashCode(), Name = s.ToString() };
            return Json(new SelectList(photoQuality, "ID", "Name"), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPDFLocation()
        {
            var PDFLocation = from SystemEnums.CheckListPDFLocation s in Enum.GetValues(typeof(SystemEnums.CheckListPDFLocation))
                              select new { ID = s.GetHashCode(), Name = s.ToString().Replace("_", " ") };
            return Json(new SelectList(PDFLocation, "ID", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVEECCheckListDocument(string jobId)
        {
            List<SelectListItem> Items = _documentsBAL.GetDocumentForCheckListItem(Convert.ToInt32(jobId)).Select(a => new SelectListItem { Text = a.FileName, Value = a.JobDocumentId.ToString() }).ToList();
            return Json(Items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillCheckListItemByItemId(string itemId)
        {
            VEECCheckListItem checkListItem = new VEECCheckListItem();
            checkListItem = GetCheckListItemByItemId(itemId, 0);
            if (checkListItem != null)
            {
                var data = Newtonsoft.Json.JsonConvert.SerializeObject(checkListItem);
                return Json(new { status = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
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
                    _vecccheckListItemBAL.MoveUPAndDownOrderOfCheckListItem(checkListTemplateId, isMoveUp);
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


                _vecccheckListItemBAL.MoveUPAndDownOrderOfCheckListItemNew(strData);
                return Json(new { status = true, id = checkListTemplateId }, JsonRequestBehavior.AllowGet);

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
                    _vecccheckListItemBAL.DeleteCheckListItemByItemId(checkListItemId, visitCheckListItemId);
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

    }
}