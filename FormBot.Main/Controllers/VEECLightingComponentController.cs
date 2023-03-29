using FormBot.Entity.Documents;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FormBot.Entity;
using FormBot.BAL.Service.VEECLightingComponent;
using System.IO;
using System.Xml.Linq;

namespace FormBot.Main.Controllers
{
    public class VEECLightingComponentController : Controller
    {

        #region Properties
        private readonly IVEECLightingComponentBAL _veecLightingComponentBAL;
        #endregion

        #region Constructor

        public VEECLightingComponentController(IVEECLightingComponentBAL veecLightingComponentBAL)
        {
            this._veecLightingComponentBAL = veecLightingComponentBAL;
        }

        #endregion
        // GET: VEECLigntingComponent
        public ActionResult Index()
        {
            VEECLightingComponent objVEECLigntingComponent = new VEECLightingComponent();
            return View(objVEECLigntingComponent);
        }

        /// <summary>
        /// Get all lighiting component file list
        /// </summary>
        /// <param name="lightingComponentName"></param>
        /// <param name="solarCompanyIds"></param>
        public void GetLightingComponentTemplateList(string lightingComponentName, string solarCompanyIds = "")
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            IList<VEECLightingComponent> lstVEECLightingComponent = new List<VEECLightingComponent>();

            string companyIds = string.Empty;
            if (ProjectSession.UserTypeId == 4)
                companyIds = Convert.ToString(ProjectSession.SolarCompanyId);
            else
                companyIds = solarCompanyIds;

            lstVEECLightingComponent = _veecLightingComponentBAL.GetLightingComponentList(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, lightingComponentName);

            if (lstVEECLightingComponent.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstVEECLightingComponent.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstVEECLightingComponent.FirstOrDefault().TotalRecords;
            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstVEECLightingComponent, gridParam));
        }

        /// <summary>
        /// Add / update lighting component file and create xml file in physical path
        /// </summary>
        /// <param name="veecLightingComponentId"></param>
        /// <param name="veecLightingComponentName"></param>
        /// <returns></returns>
        public JsonResult SaveLightingComponentTemplate(string veecLightingComponentId ,string veecLightingComponentName )
        {

            try
            {
                if (!string.IsNullOrEmpty(veecLightingComponentId))
                    veecLightingComponentId =  QueryString.GetValueFromQueryString(veecLightingComponentId, "id");
                string path = "UserDocuments\\" + ProjectSession.LoggedInUserId + "\\LightingComponent";
                string fullPath = Path.Combine(ProjectSession.ProofDocuments, path);
                if (!System.IO.Directory.Exists(fullPath))
                {
                    System.IO.Directory.CreateDirectory(fullPath);
                }
                System.IO.File.Create(fullPath + "\\" + veecLightingComponentName + ".xml");
                int VEECLightingComponentId = _veecLightingComponentBAL.SaveLightingComponentTemplate(veecLightingComponentId, veecLightingComponentName, Path.Combine(path, veecLightingComponentName + ".xml"));
                return Json(new { id = QueryString.QueryStringEncode("id=" + veecLightingComponentId) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { id = 0 }, JsonRequestBehavior.AllowGet);
            }
            
            
        }


        /// <summary>
        /// Delete lighting component file (soft delete)
        /// </summary>
        /// <param name="deleteLightingComponentIds"></param>
        /// <returns></returns>
        public JsonResult DeleteLightingComponent(string deleteLightingComponentIds)
        {
            try
            {
                if (ProjectSession.LoggedInUserId == 0)
                    return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);
                List<string> LightingComponentIds = new List<string>();
                if (!string.IsNullOrEmpty(deleteLightingComponentIds))
                    LightingComponentIds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(deleteLightingComponentIds);

                List<int> lstNewTemplateId = new List<int>();

                for (int i = 0; i < LightingComponentIds.Count; i++)
                {
                    int templateId = 0;
                    if (!string.IsNullOrEmpty(LightingComponentIds[i]))
                    {
                        int.TryParse(QueryString.GetValueFromQueryString(LightingComponentIds[i], "id"), out templateId);
                    }
                    lstNewTemplateId.Add(templateId);
                }

                string templateIds = string.Join(",", lstNewTemplateId.ToArray());

                //delete multiple file pass with comma seperated id
                _veecLightingComponentBAL.DeleteLightingComponent(templateIds);

                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status =  false}, JsonRequestBehavior.AllowGet);
            }
            
        }


        /// <summary>
        /// open draw.io page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult OpenLightingComponent(string id)
        {
            try
            {
                int VEECLightingComponentId = 0;
                if (!string.IsNullOrEmpty(id))
                {
                    int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out VEECLightingComponentId);

                }
                //Get lightingcomponent file path from lightingcomponentid
                string path = _veecLightingComponentBAL.GetLightingComponentFilePath(VEECLightingComponentId);
                string fullPath = Path.Combine(ProjectSession.ProofDocuments, path);
                string xmlData = System.IO.File.ReadAllText(fullPath);
                ViewData["XMLData"] = xmlData.Replace(Environment.NewLine, " ");

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
            return View();
        }


        /// <summary>
        /// Method used for save xml data in file 
        /// </summary>
        /// <param name="id">Lighting Component ID</param>
        /// <param name="xml">XML data </param>
        /// <returns></returns>
        public JsonResult SaveLightingComponent(string id,string xml)
        {
            try
            {
                int VEECLightingComponentId = 0;
                if (!string.IsNullOrEmpty(id))
                {
                    int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out VEECLightingComponentId);

                }
                XDocument xdoc = XDocument.Parse(xml);
                xdoc.Declaration = null;

                xml = xdoc.ToString();
                //Get lightingcomponent file path from lightingcomponentid
                string path = _veecLightingComponentBAL.GetLightingComponentFilePath(VEECLightingComponentId);
                string fullPath = Path.Combine(ProjectSession.ProofDocuments, path);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.WriteAllText(fullPath, xml);
                }
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false ,message = ex.Message}, JsonRequestBehavior.AllowGet);
            }
            
        }
    }
}