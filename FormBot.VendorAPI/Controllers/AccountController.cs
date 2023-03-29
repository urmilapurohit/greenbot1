using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FormBot.VendorAPI.Models;
using Microsoft.AspNet.Identity;
using FormBot.BAL.Service;
using System.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity.EntityFramework;
using FormBot.Helper;
using FormBot.VendorAPI.Service;
using Microsoft.Owin.Security;
using FormBot.Entity;
using System.IO;
using FormBot.Entity.Job;
using FormBot.BAL.Service.CommonRules;
using System.Data;
using FormBot.VendorAPI.Service.Job;
using FormBot.Entity.SolarElectrician;
using System.Security.Claims;
using System.Web.Http.Filters;
//using System.Web.Mvc;

namespace FormBot.VendorAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {

        #region " Private Variables "
        //private readonly IVendorAPI _vendorService;
        private readonly IJobDetails _JobDetails;
        private readonly IJobRulesBAL _jobRules;
        private readonly IUserBAL _user;
        private readonly ICreateJobBAL _jobBAL;
        public UserManager<ApplicationUser> UserManager { get; private set; }

        //public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }
        #endregion

        #region " Controller "

        public AccountController(IUserBAL user, IJobRulesBAL jobRules, IJobDetails jobDetails, ICreateJobBAL jobBAL)//ISecureDataFormat<AuthenticationTicket> accessTokenFormat)//
        {
            //this._vendorService = vendorService;
            this._JobDetails = jobDetails;
            this._jobRules = jobRules;
            this._user = user;
            this._jobBAL = jobBAL;
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            //AccessTokenFormat = accessTokenFormat;
        }

        #endregion

        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public HttpResponseMessage Login([FromBody]LoginRequest loginReq)
        {
            LoginResponseModel data = new LoginResponseModel();
            try
            {
                data = Login(loginReq.Username, loginReq.Password);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message, "Login", string.Empty);
                Log.WriteError(ex);
                data.Message = ex.Message;
                data.Status = false;
                data.StatusCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, data);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("CreateJob")]
        public HttpResponseMessage CreateJob([FromBody]CreateJob createJob)
        {
            JobResponse data = new JobResponse();
            try
            {
                FormBot.Entity.User userdata = new User();
                userdata = GetUserInfo();

                if (userdata.SolarCompanyId == 0 || userdata.SolarCompanyId == null)
                {
                    data.Message = "You are not authorized to Create Job";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, data);
                }

                string msg = string.Empty;
                DataTable dt = _jobBAL.GetJobIdByVendorjobId(createJob.VendorJobId, Convert.ToInt32(userdata.SolarCompanyId));
                int JobId = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["JobID"]) : 0;

                if (JobId == 0)
                {
                    createJob.BasicDetails.JobID = JobId;
                    data = AddEditJob(createJob, userdata);
                    if (data.Status)
                        return Request.CreateResponse(HttpStatusCode.OK, data);
                    else
                        return Request.CreateResponse(HttpStatusCode.BadRequest, data);
                }
                else
                {
                    msg += "VendorJobId already exists.";
                    data = _JobDetails.JobResponse(null, msg);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, data);
                }
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message, "CreateJob", "RefNumber:" + createJob.BasicDetails.RefNumber);
                Log.WriteError(ex);
                data.Message = ex.Message;
                data.Status = false;
                data.StatusCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, data);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("UpdateJob")]
        public HttpResponseMessage UpdateJob([FromBody]CreateJob createJob)
        {
            JobResponse data = new JobResponse();
            try
            {
                FormBot.Entity.User userdata = new User();
                userdata = GetUserInfo();

                if (userdata.SolarCompanyId == 0 || userdata.SolarCompanyId == null)
                {
                    data.Message = "You are not authorized to Edit Job";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, data);
                }

                string msg = string.Empty;

                DataTable dt = _jobBAL.GetJobIdByVendorjobId(createJob.VendorJobId, Convert.ToInt32(userdata.SolarCompanyId));
                int JobId = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["JobID"]) : 0;
                int STCStatus = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["STCStatus"]):0;

                if (JobId > 0)
                {
                    createJob.BasicDetails.JobID = JobId;
                    createJob.STCStatus = STCStatus;
                    data = AddEditJob(createJob, userdata);
                    if (data.Status)
                        return Request.CreateResponse(HttpStatusCode.OK, data);
                    else
                        return Request.CreateResponse(HttpStatusCode.BadRequest, data);
                }
                else
                {
                    msg += "VendorJobId doesn't exists.";
                    data = _JobDetails.JobResponse(null, msg);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, data);
                }
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message, "CreateJob", "RefNumber:" + createJob.BasicDetails.RefNumber);
                Log.WriteError(ex);
                data.Message = ex.Message;
                data.Status = false;
                data.StatusCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, data);
            }
        }

        public JobResponse AddEditJob(CreateJob createJob, User userdata)
        {
            OutputJobResponse data = new OutputJobResponse();
            data.obj = new JobResponse();
            try
            {
                string msg = string.Empty;
                if (createJob.BasicDetails != null && createJob.JobInstallationDetails != null && createJob.JobOwnerDetails != null)
                {
                    if (!(Enum.IsDefined(typeof(SystemEnums.JobType), createJob.BasicDetails.JobType)))
                    {
                        ModelState.AddModelError("", "JobType is not valid");
                    }
                    if (createJob.JobElectricians != null)
                    {
                        List<JobElectricians> lstJobElectricians = _user.GetElectricianList((int)userdata.SolarCompanyId, 0);
                        for (int i = 0; i < lstJobElectricians.Count; i++)
                        {
                            if (createJob.JobElectricians.FirstName + ' ' + createJob.JobElectricians.LastName == lstJobElectricians[i].Name.Split('(')[0] && createJob.JobElectricians.LicenseNumber == lstJobElectricians[i].LicenseNumber)
                            {
                                createJob.JobElectricians.JobElectricianID = lstJobElectricians[i].Id;
                                createJob.JobElectricians.IsCustomElectrician = lstJobElectricians[i].IsCustomElectrician;
                                break;
                            }
                        }
                    }
                    List<string> lstFields = _jobRules.RemoveRequiredFields(createJob);
                    if (lstFields.Count > 0)
                    {
                        for (int i = 0; i < lstFields.Count; i++)
                        {
                            ModelState.Remove("createJob." + lstFields[i]);
                        }
                    }
                    ModelState.Remove("createJob.BasicDetails.JobNumber");
                    if (createJob.JobElectricians != null && createJob.JobElectricians.ElectricianID > 0)
                        ModelState.Remove("createJob.JobElectricians");
                    RemoveInstallerValidations(createJob);
                    ModelStateValidators(createJob);

                    if (createJob.JobOwnerDetails.PostCode != null)
                    {
                        createJob.JobOwnerDetails.Town = createJob.JobOwnerDetails.Town.ToUpper();
                        createJob.JobOwnerDetails.State = createJob.JobOwnerDetails.State.ToUpper();
                        LocationResponseModel localities = PostCodeValidation("JobOwnerDetails", createJob.JobOwnerDetails.PostCode, createJob.JobOwnerDetails.Town, createJob.JobOwnerDetails.State);
                        if (localities != null && !localities.Status)
                        {
                            data.obj = _JobDetails.JobResponse(null, localities.Message);
                            if (localities.locality != null && localities.locality.Count > 0)
                            {
                                data.obj.locality = localities.locality;
                            }
                            return data.obj;
                        }
                    }
                    if (createJob.JobInstallationDetails.PostCode != null)
                    {
                        createJob.JobInstallationDetails.Town = createJob.JobInstallationDetails.Town.ToUpper();
                        createJob.JobInstallationDetails.State = createJob.JobInstallationDetails.State.ToUpper();
                        LocationResponseModel localities = PostCodeValidation("JobInstallationDetails", createJob.JobInstallationDetails.PostCode, createJob.JobInstallationDetails.Town, createJob.JobInstallationDetails.State);
                        if (localities != null && !localities.Status)
                        {
                            data.obj = _JobDetails.JobResponse(null, localities.Message);
                            if (localities.locality != null && localities.locality.Count > 0)
                            {
                                data.obj.locality = localities.locality;
                            }
                            return data.obj;
                        }
                    }
                    //if (createJob.InstallerView != null && createJob.InstallerView.PostCode != null)
                    //{
                    //    createJob.InstallerView.Town = createJob.InstallerView.Town.ToUpper();
                    //    createJob.InstallerView.State = createJob.InstallerView.State.ToUpper();
                    //    LocationResponseModel localities = PostCodeValidation("InstallerView", createJob.InstallerView.PostCode, createJob.InstallerView.Town, createJob.InstallerView.State);
                    //    if (localities != null && !localities.Status)
                    //    {
                    //        data.obj = _JobDetails.JobResponse(null, localities.Message);
                    //        if (localities.locality != null && localities.locality.Count > 0)
                    //        {
                    //            data.obj.locality = localities.locality;
                    //        }
                    //        return data.obj;
                    //    }
                    //}
                    //if (createJob.DesignerView != null && createJob.DesignerView.PostCode != null)
                    //{
                    //    createJob.DesignerView.Town = createJob.DesignerView.Town.ToUpper();
                    //    createJob.DesignerView.State = createJob.DesignerView.State.ToUpper();
                    //    LocationResponseModel localities = PostCodeValidation("DesignerView", createJob.DesignerView.PostCode, createJob.DesignerView.Town, createJob.DesignerView.State);
                    //    if (localities != null && !localities.Status)
                    //    {
                    //        data.obj = _JobDetails.JobResponse(null, localities.Message);
                    //        if (localities.locality != null && localities.locality.Count > 0)
                    //        {
                    //            data.obj.locality = localities.locality;
                    //        }
                    //        return data.obj;
                    //    }
                    //}
                    if (createJob.JobElectricians != null && createJob.JobElectricians.JobElectricianID == 0 && createJob.JobElectricians.PostCode != null)
                    {
                        createJob.JobElectricians.Town = createJob.JobElectricians.Town.ToUpper();
                        createJob.JobElectricians.State = createJob.JobElectricians.State.ToUpper();
                        LocationResponseModel localities = PostCodeValidation("JobElectricians", createJob.JobElectricians.PostCode, createJob.JobElectricians.Town, createJob.JobElectricians.State);
                        if (localities != null && !localities.Status)
                        {
                            data.obj = _JobDetails.JobResponse(null, localities.Message);
                            if (localities.locality != null && localities.locality.Count > 0)
                            {
                                data.obj.locality = localities.locality;
                            }
                            return data.obj;
                        }
                    }

                    if (createJob.JobOwnerDetails.AddressID == 1)
                        createJob.JobOwnerDetails.IsPostalAddress = false;
                    else
                        createJob.JobOwnerDetails.IsPostalAddress = true;

                    //if (createJob.InstallerView != null)
                    //{
                    //    if (createJob.InstallerView.AddressID == 1)
                    //        createJob.InstallerView.IsPostalAddress = false;
                    //    else
                    //        createJob.InstallerView.IsPostalAddress = true;
                    //}

                    //if (createJob.DesignerView != null)
                    //{
                    //    if (createJob.DesignerView.AddressID == 1)
                    //        createJob.DesignerView.IsPostalAddress = false;
                    //    else
                    //        createJob.DesignerView.IsPostalAddress = true;
                    //}

                    //if (createJob.JobSTCDetails != null && !string.IsNullOrEmpty(createJob.JobSTCDetails.DeemingPeriod))
                    //{
                    //    int deemingPeriod = 0;

                    //    if (Int32.TryParse(createJob.JobSTCDetails.DeemingPeriod, out deemingPeriod))
                    //    {
                    //        string deemingYear = ConvertNumbertoWords(deemingPeriod);

                    //        string year = !string.IsNullOrEmpty(createJob.BasicDetails.strInstallationDate) ? Convert.ToDateTime(createJob.BasicDetails.strInstallationDate).ToString("yyyy") : null;
                    //        List<System.Web.Mvc.SelectListItem> Items = _jobRules.GetDeemingPeriod(year);
                    //        bool isMatch = false;
                    //        for (int i = 0; i < Items.Count; i++)
                    //        {
                    //            if (Items[i].Text.ToLower() == deemingYear.ToLower())
                    //            {
                    //                isMatch = true;
                    //                createJob.JobSTCDetails.DeemingPeriod = Items[i].Text;
                    //                break;
                    //            }
                    //        }
                    //        if (!isMatch)
                    //        {
                    //            msg = msg + "Please enter valid deeming period based on installation year.";
                    //            data.obj = _JobDetails.JobResponse(null, msg);
                    //            return data.obj;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        msg = msg + "Please enter deeming period as number based on installation year.";
                    //        data.obj = _JobDetails.JobResponse(null, msg);
                    //        data.obj.Status = false;
                    //        return data.obj;
                    //    }
                    //}

                    if (ModelState.IsValid && userdata.UserId > 0 && userdata.SolarCompanyId > 0)
                    {
                        data = _JobDetails.CreateJob(createJob, userdata);
                        UpdateCache(data.JobId);
                        //string serviceUrl = string.Format(FormBot.Helper.ProjectSession.LiveWebUrl + "Service/UpdateCache/{0}",data.JobId);
                        //WebRequest request = WebRequest.Create(serviceUrl);
                        //CommonBAL.SetCacheDataForJobID(0, data.JobId, userdata.UserId);
                        //if (CacheConfiguration.IsContainsKey(CacheConfiguration.dsJobIndex + "_" + userdata.SolarCompanyId))
                        //{
                        //    DataTable dt = CacheConfiguration.Get<DataTable>(CacheConfiguration.dsJobIndex + "_" + userdata.SolarCompanyId);
                        //    DataRow[] dr = dt.Select("JobID = " + data.JobId);
                        //    if (dr.Count() > 0)
                        //        WriteToLogFile("JobID exist in Cache : " + userdata.SolarCompanyId, "AddEditJob", data.JobId.ToString());
                        //}

                        return data.obj;
                    }
                    else
                    {
                        foreach (var modelError in ModelState)
                        {
                            string propertyName = modelError.Key;
                            if (propertyName.Contains("JobOwnerDetails"))
                            {
                                propertyName = "Owner";
                            }
                            else if (propertyName.Contains("JobInstallationDetails"))
                            {
                                propertyName = "Installation";
                            }
                            else if (propertyName.Contains("InstallerView"))
                            {
                                propertyName = "Installer";
                            }
                            else if (propertyName.Contains("DesignerView"))
                            {
                                propertyName = "Designer";
                            }
                            else if (propertyName.Contains("JobElectricians"))
                            {
                                propertyName = "Electrician";
                            }
                            else
                            {
                                propertyName = "";
                            }
                            if (modelError.Value.Errors.Count > 0 && modelError.Value.Errors[0].Exception == null)
                            {
                                msg += propertyName + " " + modelError.Value.Errors[0].ErrorMessage + Environment.NewLine;
                            }
                            if (modelError.Value.Errors.Count > 0 && modelError.Value.Errors[0].Exception != null)
                            {
                                msg += propertyName + " " + modelError.Value.Errors[0].ErrorMessage + modelError.Value.Errors[0].Exception.Message + Environment.NewLine;
                            }
                        }

                        if (userdata.UserId == null || userdata.UserId <= 0)
                        {
                            msg += "UserId is Required";
                        }
                        if (userdata.SolarCompanyId == null || userdata.SolarCompanyId <= 0)
                        {
                            msg += "SolarCompanyId is Required";
                        }

                        data.obj = _JobDetails.JobResponse(null, msg);
                        data.obj.Status = false;
                        return data.obj;
                    }
                }
                else
                {
                    msg += "BasicDetails or Owner details or Installation details not found";
                    data.obj = _JobDetails.JobResponse(null, msg);
                    data.obj.Status = false;
                    return data.obj;
                }
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message, "CreateJob", "RefNumber:" + createJob.BasicDetails.RefNumber);
                Log.WriteError(ex);
                data.obj.Message = ex.Message;
                data.obj.Status = false;
                data.obj.StatusCode = HttpStatusCode.InternalServerError.ToString();
                return data.obj;
            }
        }

        public LocationResponseModel PostCodeValidation(string Details, string PostCode, string Town, string State)
        {
            LocationResponseModel objLocationResponseModel = new LocationResponseModel();

            //string q = createJob.JobOwnerDetails.PostCode;
            string q = PostCode;
            string ret = string.Empty;
            var webRequest = System.Net.WebRequest.Create(string.Format("https://auspost.com.au/api/postcode/search.json?q=" + q + "&excludePostBoxFlag=true"));

            if (webRequest != null)
            {
                webRequest.Headers.Add("AUTH-KEY", "0344e02f-843b-49a7-8fd6-d35acd471480");
                webRequest.Method = "GET";
                webRequest.Timeout = 20000;
                webRequest.ContentType = "application/json";
            }

            HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();
            Stream resStream = resp.GetResponseStream();
            StreamReader reader = new StreamReader(resStream);
            ret = reader.ReadToEnd();
            ret = ret.Replace(@"\", "");
            if (!ret.Contains("["))
            {
                ret = ret.Replace("{\"c", "[{\"c");
                ret = ret.Replace("}}}", "}]}}");
            }
            var model = JsonConvert.DeserializeObject<RootObject>(ret);

            if (model.localities == null)
            {
                objLocationResponseModel.Message = Details + ": Invalid PostCode.";
                objLocationResponseModel.Status = false;
                return objLocationResponseModel;
            }
            if (!ret.Contains(Town) || !ret.Contains(State))
            {
                objLocationResponseModel.locality = model.localities.locality;
                objLocationResponseModel.Status = false;
                objLocationResponseModel.Message = Details + ": Invalid PostCode/Town/State. Please enter valid details from given data";
                return objLocationResponseModel;
            }
            return null;

        }

        public void ModelStateValidators(CreateJob createJob)
        {
            try
            {
                if (createJob.JobOwnerDetails.AddressID == 0)
                {
                    ModelState.AddModelError("createJob.JobOwnerDetails.AddressID", "AddressID is required.");
                }
                if (createJob.JobInstallationDetails.AddressID == 0)
                {
                    ModelState.AddModelError("createJob.JobInstallationDetails.AddressID", "AddressID is required.");
                }
                if ((createJob.STCStatus == 0 ||createJob.STCStatus == 10 || createJob.STCStatus == 12 || createJob.STCStatus == 14 || createJob.STCStatus == 17) && createJob.JobOwnerDetails.OwnerType == null || createJob.JobOwnerDetails.OwnerType == "")
                {
                    ModelState.AddModelError("createJob.JobOwnerDetails.OwnerType", "Owner Type is required.");
                }
                if ((createJob.JobOwnerDetails.OwnerType != null && createJob.JobOwnerDetails.OwnerType != "" && createJob.JobOwnerDetails.OwnerType != "Individual") && (createJob.JobOwnerDetails.CompanyName == null || createJob.JobOwnerDetails.CompanyName == ""))
                {
                    ModelState.AddModelError("createJob.JobOwnerDetails.CompanyName", "Company Name is required.");
                }
                if (createJob.JobOwnerDetails.UnitTypeID > 0 && (createJob.JobOwnerDetails.UnitNumber == null || string.IsNullOrWhiteSpace(createJob.JobOwnerDetails.UnitNumber)))
                {
                    ModelState.AddModelError("createJob.JobOwnerDetails.UnitNumber", "UnitNumber is required.");
                }

                if (createJob.JobInstallationDetails.UnitTypeID > 0 && (createJob.JobInstallationDetails.UnitNumber == null || string.IsNullOrWhiteSpace(createJob.JobInstallationDetails.UnitNumber)))
                {
                    ModelState.AddModelError("createJob.JobInstallationDetails.UnitNumber", "UnitNumber is required in installation details");
                }

                if (createJob.JobInstallationDetails != null)
                {
                    if (createJob.JobInstallationDetails.NMI != null)
                    {
                        if (_jobRules.IsDigitsOnly(createJob.JobInstallationDetails.NMI) == false || createJob.JobInstallationDetails.NMI.Length > 25)
                        {
                            ModelState.AddModelError("createJob.JobInstallationDetails.NMI", "NMI can have only digits and length should not exceed 25 digits");
                        }
                    }
                    if (createJob.JobInstallationDetails.MeterNumber != null)
                    {
                        if (createJob.JobInstallationDetails.MeterNumber.Length > 25)
                        {
                            ModelState.AddModelError("createJob.JobInstallationDetails.MeterNumber", "MeterNumber length should not exceed 25 digits");
                        }
                    }
                    if (createJob.JobInstallationDetails.ExistingSystem == false && createJob.JobInstallationDetails.ExistingSystemSize != null)
                    {
                        ModelState.AddModelError("createJob.JobInstallationDetails.ExistingSystemSize", "ExistingSystem is required true for ExistingSystemSize");
                    }
                    if (createJob.JobInstallationDetails.ExistingSystem == false && createJob.JobInstallationDetails.NoOfPanels > 0)
                    {
                        ModelState.AddModelError("createJob.JobInstallationDetails.NoOfPanels", "ExistingSystem is required true for NoOfPanels");
                    }
                    if (createJob.JobInstallationDetails.ExistingSystem == false && createJob.JobInstallationDetails.SystemLocation != null)
                    {
                        ModelState.AddModelError("createJob.JobInstallationDetails.SystemLocation", "ExistingSystem is required true for SystemLocation");
                    }
                }

                if (createJob.JobSystemDetails != null)
                {
                    if (createJob.BasicDetails.JobType == 1)
                    {
                        //if (createJob.JobSystemDetails.SystemSize > 0 && (createJob.BasicDetails.strInstallationDate == null || createJob.JobSTCDetails.DeemingPeriod == null || createJob.JobInstallationDetails.PostCode == null))
                        if (createJob.JobSystemDetails.SystemSize > 0 && (createJob.BasicDetails.strInstallationDate == null || createJob.JobInstallationDetails.PostCode == null))
                        {
                            ModelState.AddModelError(" ", "Please fill Installation Date and Installation postcode to set STC value.");
                        }
                        //if (createJob.BasicDetails.strInstallationDate != null && Convert.ToDateTime(createJob.BasicDetails.strInstallationDate) >= DateTime.Now)
                        //{
                        //    ModelState.AddModelError("createJob.BasicDetails.strInstallationDate", "Installation Date cannot be greater than today's date.");
                        //}
                        if (createJob.panel != null && createJob.panel.Count > 0)
                        {
                            foreach (var item in createJob.panel)
                            {
                                if (item.NoOfPanel == 0 || item.NoOfPanel > 10000)
                                {
                                    ModelState.AddModelError("createJob.JobPanelDetails.NoOfPanel", "Panel Brand: " + item.Brand + " - No of panel should be between 1 to 10000");
                                }
                            }
                        }
                        if (createJob.JobSystemDetails.InstallationType != null)
                        {
                            ModelState.AddModelError("createJob.JobSystemDetails.InstallationType", "InstallationType is only for SWH job");
                        }
                        if (createJob.JobSystemDetails.SystemBrand != null)
                        {
                            ModelState.AddModelError("createJob.JobSystemDetails.SystemBrand", "SystemBrand is only for SWH job");
                        }
                        if (createJob.JobSystemDetails.SystemModel != null)
                        {
                            ModelState.AddModelError("createJob.JobSystemDetails.SystemModel", "SystemModel is only for SWH job");
                        }
                        if (createJob.JobSystemDetails.NoOfPanel > 0)
                        {
                            ModelState.AddModelError("createJob.JobSystemDetails.NoOfPanel", "NoOfPanel is only for SWH job");
                        }
                    }
                    if (createJob.BasicDetails.JobType == 2)
                    {
                        if (createJob.BasicDetails.strInstallationDate != null && (createJob.JobSystemDetails.SystemBrand == null || createJob.JobSystemDetails.SystemModel == null || createJob.JobInstallationDetails.PostCode == null))
                        {
                            ModelState.AddModelError(" ", "Please fill Installation Date, System brand, System Model, Installation postcode to set STC value.");
                        }
                        if (createJob.JobSystemDetails.SystemBrand != null && createJob.JobSystemDetails.SystemModel != null && (createJob.JobSystemDetails.NoOfPanel == null || createJob.JobSystemDetails.NoOfPanel == 0))
                        {
                            ModelState.AddModelError("createJob.JobSystemDetails.NoOfPanel", "System Brand: " + createJob.JobSystemDetails.SystemBrand + " - No of panel is required.");
                        }
                        if (createJob.panel != null)
                        {
                            ModelState.AddModelError("createJob.panel", "Panel is only for PVD job");
                        }
                        if (createJob.inverter != null)
                        {
                            ModelState.AddModelError("createJob.inverter", "Inverter is only for PVD job");
                        }
                    }
                }

                if (createJob.JobSTCDetails != null)
                {
                    if ((createJob.JobSTCDetails.MultipleSGUAddress == null || createJob.JobSTCDetails.MultipleSGUAddress == "No") && createJob.JobSTCDetails.Location != null)
                    {
                        ModelState.AddModelError("createJob.JobSTCDetails.Location", "Location can have value only when MultipleSGUAddress has value Yes");
                    }
                    if ((createJob.JobSTCDetails.VolumetricCapacity == null || createJob.JobSTCDetails.VolumetricCapacity == "No") && createJob.JobSTCDetails.StatutoryDeclarations != null)
                    {
                        ModelState.AddModelError("createJob.JobSTCDetails.StatutoryDeclarations", "StatutoryDeclarations can have value only when VolumetricCapacity has value Yes");
                    }
                }

                if (createJob.BasicDetails.JobType == 2)
                {
                    if (createJob.JobInstallationDetails != null)
                    {
                        if (createJob.JobInstallationDetails.InstallingNewPanel != null)
                        {
                            ModelState.AddModelError("createJob.JobInstallationDetails.InstallingNewPanel", "InstallingNewPanel is for PVD job only");
                        }

                        if (createJob.JobInstallationDetails.ElectricityProviderID != null)
                        {
                            ModelState.AddModelError("createJob.JobInstallationDetails.ElectricityProviderID", "ElectricityProviderID is for PVD job only");
                        }

                        if (createJob.JobInstallationDetails.NMI != null)
                        {
                            ModelState.AddModelError("createJob.JobInstallationDetails.NMI", "NMI is for PVD job only");
                        }
                        if (createJob.JobInstallationDetails.MeterNumber != null)
                        {
                            ModelState.AddModelError("createJob.JobInstallationDetails.MeterNumber", "MeterNumber is for PVD job only");
                        }
                        if (createJob.JobInstallationDetails.PhaseProperty != null)
                        {
                            ModelState.AddModelError("createJob.JobInstallationDetails.PhaseProperty", "PhaseProperty is for PVD job only");
                        }
                        if (createJob.JobInstallationDetails.ExistingSystemSize != null)
                        {
                            ModelState.AddModelError("createJob.JobInstallationDetails.ExistingSystemSize", "ExistingSystemSize is for PVD job only");
                        }
                        if (createJob.JobInstallationDetails.NoOfPanels != null)
                        {
                            ModelState.AddModelError("createJob.JobInstallationDetails.NoOfPanels", "NoOfPanels is for PVD job only");
                        }
                        if (createJob.JobInstallationDetails.SystemLocation != null)
                        {
                            ModelState.AddModelError("createJob.JobInstallationDetails.SystemLocation", "SystemLocation is for PVD job only");
                        }
                    }
                    if (createJob.JobSTCDetails != null)
                    {
                        if (createJob.JobSTCDetails.TypeOfConnection != null)
                        {
                            ModelState.AddModelError("createJob.JobSTCDetails.TypeOfConnection", "TypeOfConnection is for PVD job only");
                        }

                        if (createJob.JobSTCDetails.SystemMountingType != null)
                        {
                            ModelState.AddModelError("createJob.JobSTCDetails.SystemMountingType", "SystemMountingType is for PVD job only");
                        }

                        if (createJob.JobSTCDetails.DeemingPeriod != null)
                        {
                            ModelState.AddModelError("createJob.JobSTCDetails.DeemingPeriod", "DeemingPeriod is for PVD job only");
                        }
                    }
                }

                if (createJob.BasicDetails.JobType == 1)
                {
                    if (createJob.JobSTCDetails != null)
                    {
                        if (createJob.JobSTCDetails.VolumetricCapacity != null)
                        {
                            ModelState.AddModelError("createJob.JobSTCDetails.VolumetricCapacity", "VolumetricCapacity is for SWH job only");
                        }

                        if (createJob.JobSTCDetails.SecondhandWaterHeater != null)
                        {
                            ModelState.AddModelError("createJob.JobSTCDetails.SecondhandWaterHeater", "SecondhandWaterHeater is for SWH job only");
                        }

                        if (createJob.JobSTCDetails.StatutoryDeclarations != null)
                        {
                            ModelState.AddModelError("createJob.JobSTCDetails.StatutoryDeclarations", "StatutoryDeclarations is for SWH job only");
                        }
                    }

                }

                //if (!string.IsNullOrEmpty(createJob.InstallerView.SESignature))
                //{
                //    FileValidations(createJob.InstallerView.SESignature, true, "Installer signature");
                //}


                if (createJob.InstallerView != null)
                {
                    if (string.IsNullOrEmpty(createJob.InstallerView.CECAccreditationNumber))
                    {
                        ModelState.AddModelError("createJob.InstallerView.CECAccreditationNumber", "CECAccreditationNumber is required.");
                    }
                    //ModelState.AddModelError(" ", "InstallerView is no more required.");
                    //if (createJob.InstallerView.AddressID == 0)
                    //{
                    //    ModelState.AddModelError("createJob.InstallerView.AddressID", "AddressID is required.");
                    //}
                    //else
                    //{
                    //    if (createJob.InstallerView.AddressID == 1)
                    //    {
                    //        if (createJob.InstallerView.UnitTypeID == 0 && (createJob.InstallerView.StreetTypeID == 0 && string.IsNullOrEmpty(createJob.InstallerView.StreetName)))
                    //        {
                    //            ModelState.AddModelError("createJob.InstallerView.UnitTypeID", "UnitTypeID is required.");
                    //            ModelState.AddModelError("createJob.InstallerView.StreetName", "StreetName is required.");
                    //            ModelState.AddModelError("createJob.InstallerView.StreetTypeID", "StreetTypeID is required.");
                    //        }
                    //        if (createJob.InstallerView.UnitTypeID == 0 && string.IsNullOrEmpty(createJob.InstallerView.StreetNumber))
                    //        {
                    //            ModelState.AddModelError("createJob.InstallerView.StreetNumber", "StreetNumber is required.");
                    //        }
                    //        if (createJob.InstallerView.UnitTypeID > 0 && (createJob.InstallerView.UnitNumber == null || createJob.InstallerView.UnitNumber == ""))
                    //        {
                    //            ModelState.AddModelError("createJob.InstallerView.UnitNumber", "UnitNumber is required.");
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (createJob.InstallerView.PostalAddressID == 0)
                    //            ModelState.AddModelError("createJob.InstallerView.PostalAddressID", "PostalAddressID is required.");
                    //        if (string.IsNullOrEmpty(createJob.InstallerView.PostalDeliveryNumber))
                    //            ModelState.AddModelError("createJob.InstallerView.PostalDeliveryNumber", "PostalDeliveryNumber is required.");

                    //    }
                    //}
                }

                if (createJob.DesignerView != null)
                {
                    if (string.IsNullOrEmpty(createJob.DesignerView.CECAccreditationNumber))
                    {
                        ModelState.AddModelError("createJob.DesignerView.CECAccreditationNumber", "CECAccreditationNumber is required.");
                    }
                    //ModelState.AddModelError(" ", "DesignerView is no more required.");
                    //if (createJob.DesignerView.AddressID == 0)
                    //{
                    //    ModelState.AddModelError("createJob.DesignerView.AddressID", "AddressID is required.");
                    //}
                    //else
                    //{
                    //    if (createJob.DesignerView.AddressID == 1)
                    //    {
                    //        if (createJob.DesignerView.UnitTypeID == 0 && (createJob.DesignerView.StreetTypeID == 0 && string.IsNullOrEmpty(createJob.DesignerView.StreetName)))
                    //        {
                    //            ModelState.AddModelError("createJob.DesignerView.UnitTypeID", "UnitTypeID is required.");
                    //            ModelState.AddModelError("createJob.DesignerView.StreetName", "StreetName is required.");
                    //            ModelState.AddModelError("createJob.DesignerView.StreetTypeID", "StreetTypeID is required.");
                    //        }
                    //        if (createJob.DesignerView.UnitTypeID == 0 && string.IsNullOrEmpty(createJob.DesignerView.StreetNumber))
                    //        {
                    //            ModelState.AddModelError("createJob.DesignerView.StreetNumber", "StreetNumber is required.");
                    //        }
                    //        if (createJob.DesignerView.UnitTypeID > 0 && (createJob.DesignerView.UnitNumber == null || createJob.DesignerView.UnitNumber == ""))
                    //        {
                    //            ModelState.AddModelError("createJob.DesignerView.UnitNumber", "UnitNumber is required.");
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (createJob.DesignerView.PostalAddressID == 0)
                    //            ModelState.AddModelError("createJob.DesignerView.PostalAddressID", "PostalAddressID is required.");
                    //        if (string.IsNullOrEmpty(createJob.DesignerView.PostalDeliveryNumber))
                    //            ModelState.AddModelError("createJob.DesignerView.PostalDeliveryNumber", "PostalDeliveryNumber is required.");
                    //    }
                    //}
                }

                if (createJob.JobElectricians != null && createJob.JobElectricians.JobElectricianID == 0)
                {
                    if (string.IsNullOrEmpty(createJob.JobElectricians.FirstName) || string.IsNullOrEmpty(createJob.JobElectricians.LastName))
                    {
                        ModelState.AddModelError("createJob.JobElectricians.FirstName", "FirstName is required.");
                        ModelState.AddModelError("createJob.JobElectricians.LastName", "LastName is required.");
                    }
                    if (string.IsNullOrEmpty(createJob.JobElectricians.CompanyName))
                    {
                        ModelState.AddModelError("createJob.JobElectricians.CompanyName", "Company Name is required.");
                    }
                    if (!createJob.JobElectricians.IsPostalAddress)
                    {
                        if ((createJob.JobElectricians.UnitTypeID == 0 || createJob.JobElectricians.UnitTypeID == null) && (createJob.JobElectricians.StreetTypeID == 0 || createJob.JobElectricians.StreetTypeID == null && string.IsNullOrEmpty(createJob.JobElectricians.StreetName)))
                        {
                            ModelState.AddModelError("createJob.JobElectricians.UnitTypeID", "UnitTypeID is required.");
                            ModelState.AddModelError("createJob.JobElectricians.StreetName", "StreetName is required.");
                            ModelState.AddModelError("createJob.JobElectricians.StreetTypeID", "StreetTypeID is required.");
                        }
                        if ((createJob.JobElectricians.UnitTypeID == 0 || createJob.JobElectricians.UnitTypeID == null) && string.IsNullOrEmpty(createJob.JobElectricians.StreetNumber))
                        {
                            ModelState.AddModelError("createJob.JobElectricians.StreetNumber", "StreetNumber is required.");
                        }
                        if (createJob.JobElectricians.UnitTypeID > 0 && (createJob.JobElectricians.UnitNumber == null || createJob.JobElectricians.UnitNumber == ""))
                        {
                            ModelState.AddModelError("createJob.JobElectricians.UnitNumber", "UnitNumber is required.");
                        }
                    }
                    else
                    {
                        if (createJob.JobElectricians.PostalAddressID == 0)
                            ModelState.AddModelError("createJob.JobElectricians.PostalAddressID", "PostalAddressID is required.");
                        if (string.IsNullOrEmpty(createJob.JobElectricians.PostalDeliveryNumber))
                            ModelState.AddModelError("createJob.JobElectricians.PostalDeliveryNumber", "PostalDeliveryNumber is required.");
                    }
                }

                if (!string.IsNullOrEmpty(createJob.BasicDetails.GSTDocument))
                {
                    FileValidations(createJob.BasicDetails.GSTDocument, false, "GST file");
                }
                if (createJob.lstCustomDetails.Count > 0)
                {
                    foreach (var item in createJob.lstCustomDetails)
                    {
                        if (string.IsNullOrEmpty(item.VendorJobCustomFieldId) || string.IsNullOrEmpty(item.FieldName))
                        {
                            ModelState.AddModelError("createJob.lstCustomDetails", " VendorJobCustomFieldId or FieldName is missing.");
                        }
                    }
                }

                if ((createJob.STCStatus != 0 && createJob.STCStatus != 10 && createJob.STCStatus != 12 && createJob.STCStatus != 14 && createJob.STCStatus != 17)
                    && (!string.IsNullOrEmpty(createJob.BasicDetails.strInstallationDate) || !string.IsNullOrEmpty(createJob.JobOwnerDetails.OwnerType)
                        || !string.IsNullOrEmpty(createJob.JobInstallationDetails.PropertyType) || createJob.JobSystemDetails.SystemSize > 0))
                {
                    ModelState.AddModelError(" ", "Installation Date, Owner Type, Installation Property Type and System Size can't be updated once job has been traded. ");
                }
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message, "ModelStateValidators", "RefNumber:" + Convert.ToString(createJob.BasicDetails.RefNumber));
            }
        }

        public void RemoveInstallerValidations(CreateJob createJob)
        {
            try
            {
                //ModelState.Remove("createJob.JobElectricians.IsPostalAddress");
                if (createJob.InstallerView == null)
                {
                    ModelState.Remove("createJob.InstallerView.FirstName");
                    ModelState.Remove("createJob.InstallerView.LastName");
                    ModelState.Remove("createJob.InstallerView.Town");
                    ModelState.Remove("createJob.InstallerView.State");
                    ModelState.Remove("createJob.InstallerView.PostCode");
                    ModelState.Remove("createJob.InstallerView.CECAccreditationNumber");
                    ModelState.Remove("createJob.InstallerView.SEDesignRoleId");
                    ModelState.Remove("createJob.InstallerView.Phone");
                }
                if (createJob.DesignerView == null)
                {
                    ModelState.Remove("createJob.DesignerView.FirstName");
                    ModelState.Remove("createJob.DesignerView.LastName");
                    ModelState.Remove("createJob.DesignerView.Town");
                    ModelState.Remove("createJob.DesignerView.State");
                    ModelState.Remove("createJob.DesignerView.PostCode");
                    ModelState.Remove("createJob.DesignerView.CECAccreditationNumber");
                    ModelState.Remove("createJob.DesignerView.SEDesignRoleId");
                    ModelState.Remove("createJob.DesignerView.Phone");
                }
                //if (createJob.JobElectricians == null)
                //{
                ModelState.Remove("createJob.JobElectricians.Town");
                ModelState.Remove("createJob.JobElectricians.State");
                ModelState.Remove("createJob.JobElectricians.PostCode");
                ModelState.Remove("createJob.JobElectricians.IsPostalAddress");
                ModelState.Remove("createJob.JobElectricians.PostalAddressID");
                ModelState.Remove("createJob.JobElectricians.PostalDeliveryNumber");
                ModelState.Remove("createJob.JobElectricians.StreetName");
                ModelState.Remove("createJob.JobElectricians.StreetNumber");
                ModelState.Remove("createJob.JobElectricians.StreetTypeID");
                ModelState.Remove("createJob.JobElectricians.Phone");
                if (createJob.JobElectricians == null)
                {
                    ModelState.Remove("createJob.JobElectricians.LicenseNumber");
                }
                //}

                if (createJob.BasicDetails.JobType != 2)
                {
                    ModelState.Remove("createJob.InstallerView.SWHLicenseNumber");
                    ModelState.Remove("createJob.DesignerView.SWHLicenseNumber");
                }
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message, "ModelStateValidators", "RefNumber:" + Convert.ToString(createJob.BasicDetails.RefNumber));
            }
        }

        public void FileValidations(string Filename, bool isImage, string fileType)
        {
            if (isImage)
            {
                if (!(MimeMapping.GetMimeMapping(Filename).Contains("image")))
                {
                    ModelState.AddModelError(" ", "Please upload a " + fileType + " with .jpg , .jpeg or .png extension.");
                }
            }
            else
            {
                string[] allowedType = new string[] { ".png", ".jpg", ".jpeg", ".gif", ".doc", ".docx", ".xls", ".csv", ".pdf", ".txt" };
                if (!(allowedType.Contains(Path.GetExtension(Filename).ToLower())))
                {
                    ModelState.AddModelError(" ", "Please upload a " + fileType + " with .png, .jpg, .jpeg, .gif, .doc, .docx, .xls, .csv, .pdf or .txt extension.");
                }
            }
        }

        public LoginResponseModel Login(string Username, string Password)
        {
            LoginResponseModel objLoginResponseModel = new LoginResponseModel();
            //objLoginResponseModel.UserData = new UserVendor();
            try
            {
                var user = UserManager.Find(Username, Password);
                if (user != null)
                {
                    var userDetail = _user.GetUserByAspnetUserId_VendorAPI(user.Id);
                    if (userDetail != null)
                    {
                        Token TokenResult = CreateToken(Username, Password);

                        //objLoginResponseModel.UserData = userDetail;
                        objLoginResponseModel.TokenData = TokenResult;

                        objLoginResponseModel.Status = true;
                        objLoginResponseModel.StatusCode = HttpStatusCode.OK.ToString();
                        objLoginResponseModel.Message = "Login Successfully";


                    }
                    else
                    {
                        objLoginResponseModel.Message = "User does not exist";
                        objLoginResponseModel.Status = false;
                        objLoginResponseModel.StatusCode = HttpStatusCode.NotFound.ToString();
                    }
                }
                else
                {
                    objLoginResponseModel.Message = "Invalid username or password.";
                    objLoginResponseModel.Status = false;
                    objLoginResponseModel.StatusCode = HttpStatusCode.Unauthorized.ToString();
                }
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message, "Login", string.Empty);
            }
            return objLoginResponseModel;
        }

        public User GetUserInfo()
        {
            User userDetail = null;
            try
            {
                ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;
                var claims = claimsIdentity.Claims.Select(x => new { type = x.Type, value = x.Value });
                userDetail = _user.GetUserByUserName(claims.FirstOrDefault().value.Replace("'", "")); //_user.GetUserByAspnetUserId(userData.Id);
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message, "GetUserInfo", string.Empty);
            }
            return userDetail;
        }

        public Token CreateToken(string username, string password)
        {
            Token TokenResult = new Token();
            try
            {
                string token;
                string siteUrl = String.Format("{0}://{1}{2}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority, HttpContext.Current.Request.ApplicationPath); ;

                using (WebClient client = new WebClient())
                {
                    //client.Headers.Add("content-type", "application/x-www-form-urlencoded");
                    token = client.UploadString(siteUrl + "/token", "post", "grant_type=password&username='" + username + "'");
                }

                dynamic tokendata = JObject.Parse(token);
                TokenResult.access_token = tokendata.access_token;
                TokenResult.token_type = tokendata.token_type;
                //TokenResult.expires_in = tokendata.expires_in;
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message, "CreateToken", string.Empty);
            }
            return TokenResult;
        }

        /// <summary>
        /// Requireds the validation field.
        /// </summary>
        /// <param name="createJob">The create job.</param>
        public void RequiredValidationField(CreateJob createJob)
        {
            try
            {
                //ModelState.Remove("createJob.JobOwnerDetails");
                //ModelState.Remove("createJob.JobInstallationDetails");
                ModelState.Remove("createJob.BasicDetails.JobNumber");
                ModelState.Remove("createJob.BasicDetails.Notes");
                ModelState.Remove("createJob.panelXml");
                ModelState.Remove("createJob.inverterXml");
                ModelState.Remove("createJob.JobElectricians.StreetNumber");
                ModelState.Remove("createJob.JobElectricians.StreetName");
                ModelState.Remove("createJob.JobElectricians.StreetTypeID");
                ModelState.Remove("createJob.JobElectricians.PostalAddressID");
                ModelState.Remove("createJob.JobElectricians.PostalDeliveryNumber");
                ModelState.Remove("createJob.JobElectricians.Town");
                ModelState.Remove("createJob.JobElectricians.State");
                ModelState.Remove("createJob.JobElectricians.PostCode");

                ModelState.Remove("createJob.JobElectricians.Phone");
                ModelState.Remove("createJob.JobElectricians.LicenseNumber");

                ModelState.Remove("createJob.JobInstallerDetails.FirstName");
                ModelState.Remove("createJob.JobInstallerDetails.Surname");
                ModelState.Remove("createJob.JobInstallerDetails.Phone");
                ModelState.Remove("createJob.JobInstallerDetails.UnitTypeID");
                ModelState.Remove("createJob.JobInstallerDetails.UnitNumber");
                ModelState.Remove("createJob.JobInstallerDetails.StreetNumber");
                ModelState.Remove("createJob.JobInstallerDetails.StreetName");
                ModelState.Remove("createJob.JobInstallerDetails.StreetTypeID");
                ModelState.Remove("createJob.JobInstallerDetails.PostalAddressID");
                ModelState.Remove("createJob.JobInstallerDetails.PostalDeliveryNumber");
                ModelState.Remove("createJob.JobInstallerDetails.Town");
                ModelState.Remove("createJob.JobInstallerDetails.State");
                ModelState.Remove("createJob.JobInstallerDetails.PostCode");
                ModelState.Remove("createJob.JobOwnerDetails.OwnerType");
                ModelState.Remove("createJob.JobOwnerDetails.CompanyName");
                ModelState.Remove("createJob.JobOwnerDetails.FirstName");
                ModelState.Remove("createJob.JobOwnerDetails.LastName");
                ModelState.Remove("createJob.JobInstallerDetails.ElectricianID");
                if (createJob.JobInstallationDetails.AddressID == 2)
                {
                    ModelState.Remove("createJob.JobInstallationDetails.StreetNumber");
                    ModelState.Remove("createJob.JobInstallationDetails.StreetName");
                    ModelState.Remove("createJob.JobInstallationDetails.StreetTypeID");
                }

                if (createJob.JobInstallationDetails.AddressID == 1)
                {
                    ModelState.Remove("createJob.JobInstallationDetails.PostalAddressID");
                    ModelState.Remove("createJob.JobInstallationDetails.PostalDeliveryNumber");
                    if (createJob.JobInstallationDetails.UnitNumber != null && createJob.JobInstallationDetails.UnitTypeID != 0)
                    {
                        ModelState.Remove("createJob.JobInstallationDetails.StreetNumber");
                    }
                }

                if (createJob.JobOwnerDetails.AddressID == 2)
                {
                    ModelState.Remove("createJob.JobOwnerDetails.StreetNumber");
                    ModelState.Remove("createJob.JobOwnerDetails.StreetName");
                    ModelState.Remove("createJob.JobOwnerDetails.StreetTypeID");
                }
                if (createJob.JobOwnerDetails.AddressID == 1)
                {
                    ModelState.Remove("createJob.JobOwnerDetails.PostalAddressID");
                    ModelState.Remove("createJob.JobOwnerDetails.PostalDeliveryNumber");
                    if (createJob.JobOwnerDetails.UnitNumber != null && createJob.JobOwnerDetails.UnitTypeID != 0)
                    {
                        ModelState.Remove("createJob.JobOwnerDetails.StreetNumber");
                    }
                }
                if (createJob.BasicDetails.JobType == 2)
                {
                    ModelState.Remove("createJob.JobInstallationDetails.NMI");
                }
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message, "RequiredValidationField", string.Empty);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("GetJobs")]
        public HttpResponseMessage GetJobs(string CreatedDate = null, string FromDate = null, string ToDate = null, string RefNumber = null, string VendorJobId = null, string CompanyABN = null)
        {
            GetJobsModel data = new GetJobsModel();
            try
            {
                FormBot.Entity.User userdata = new User();
                userdata = GetUserInfo();
                data = _JobDetails.GetJobs(CreatedDate, FromDate, ToDate, RefNumber, userdata, VendorJobId, CompanyABN);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message, "GetJobs", "CreatedDate:" + CreatedDate + " FromDate:" + FromDate + " ToDate:" + ToDate + " RefNumber:" + RefNumber + " VendorJobId:" + VendorJobId + "CompanyABN:" + CompanyABN);
                Log.WriteError(ex);
                data.Message = ex.Message;
                data.Status = false;
                data.StatusCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, data);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("GetPanel")]
        public HttpResponseMessage GetPanel()
        {
            PanelResponseModel data = new PanelResponseModel();
            try
            {
                data = _JobDetails.GetPanel();
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message, "GetPanel", string.Empty);
                Log.WriteError(ex);
                data.Message = ex.Message;
                data.Status = false;
                data.StatusCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, data);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("GetInverter")]
        public HttpResponseMessage GetInverter()
        {
            InverterResponseModel data = new InverterResponseModel();
            try
            {
                data = _JobDetails.GetInverter();
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message, "GetInverter", string.Empty);
                Log.WriteError(ex);
                data.Message = ex.Message;
                data.Status = false;
                data.StatusCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, data);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("GetSystemBrand")]
        public HttpResponseMessage GetSystemBrand()
        {
            SystemBrandResponseModel data = new SystemBrandResponseModel();
            try
            {
                data = _JobDetails.GetSystemBrand();
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message, "GetSystemBrand", string.Empty);
                Log.WriteError(ex);
                data.Message = ex.Message;
                data.Status = false;
                data.StatusCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, data);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("UploadJobImage")]
        public HttpResponseMessage UploadJobImage(JobPhotoRequest jobPhotoRequest)
        {
            OutputJobResponse data = new OutputJobResponse();
            data.obj = new JobResponse();
            try
            {
                FormBot.Entity.User userdata = new User();
                userdata = GetUserInfo();
                if (userdata.SolarCompanyId == 0 || userdata.SolarCompanyId == null)
                {
                    data.obj.Message = "You are not authorized to Upload Image";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, data.obj);
                }
                DataTable dt = _jobBAL.GetJobIdByVendorjobId(jobPhotoRequest.VendorJobId, Convert.ToInt32(userdata.SolarCompanyId));
                int JobId = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["JobID"]) : 0;
                if (JobId > 0)
                {
                    jobPhotoRequest.JobId = JobId;

                    if (!(Enum.IsDefined(typeof(SystemEnums.PhotoType), jobPhotoRequest.PhotoType)))
                    {
                        ModelState.AddModelError("", "PhotoType is not valid");
                    }
                    FileValidations(jobPhotoRequest.Filename, true, "Image");

                    if (ModelState.IsValid)
                    {
                        if (jobPhotoRequest.ImageBase64.Length < Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["FileSize"]))
                        {
                            data = _JobDetails.JobPhoto(jobPhotoRequest, userdata);

                            UpdateCache(JobId);
                            WriteToLogFile("UploadJobImage", "Cache updated", JobId.ToString());
                            //UpdateCache(data.JobId);
                            //CommonBAL.SetCacheDataForJobID(0, data.JobId, userdata.UserId);
                            return Request.CreateResponse(HttpStatusCode.OK, data.obj);
                        }
                        else
                        {
                            data.obj = _JobDetails.JobResponse(null, "File Size should be less than 10mb");
                            return Request.CreateResponse(HttpStatusCode.BadRequest, data.obj);
                        }
                    }
                    else
                    {
                        string msg = string.Empty;
                        ModelState.Values.AsEnumerable().ToList().ForEach(d =>
                        {
                            for (int i = 0; i < d.Errors.Count; i++)
                            {
                                msg += d.Errors[i].ErrorMessage + Environment.NewLine;
                                if (d.Errors[i].Exception != null)
                                {
                                    msg += d.Errors[i].Exception.Message + Environment.NewLine;
                                }
                            }
                        });
                        data.obj = _JobDetails.JobResponse(null, msg);
                        return Request.CreateResponse(HttpStatusCode.BadRequest, data.obj);
                    }
                }
                else
                {
                    data.obj = _JobDetails.JobResponse(null, "VendorJobId doesn't exists.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, data.obj);
                }
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message, "UploadJobImage", "VendorJobId:" + Convert.ToString(jobPhotoRequest.VendorJobId));
                Log.WriteError(ex);
                data.obj.Message = ex.Message;
                data.obj.Status = false;
                data.obj.StatusCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, data.obj);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("UploadDocuments")]
        public HttpResponseMessage UploadDocuments(JobDocumentRequest jobDocumentRequest)
        {
            OutputJobResponse data = new OutputJobResponse();
            data.obj = new JobResponse();
            try
            {
                FormBot.Entity.User userdata = new User();
                userdata = GetUserInfo();
                if (userdata.SolarCompanyId == 0 || userdata.SolarCompanyId == null)
                {
                    data.obj.Message = "You are not authorized to Upload Document.";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, data.obj);
                }

                DataTable dt = _jobBAL.GetJobIdByVendorjobId(jobDocumentRequest.VendorJobId, Convert.ToInt32(userdata.SolarCompanyId));
                int JobId = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["JobID"]) : 0;
                if (JobId > 0)
                {
                    jobDocumentRequest.JobId = JobId;

                    if (!(Enum.IsDefined(typeof(SystemEnums.JobType), jobDocumentRequest.JobType)))
                    {
                        ModelState.AddModelError("", "JobType is not valid");
                    }
                    if (!(Enum.IsDefined(typeof(SystemEnums.DocumentType), jobDocumentRequest.DocumentType)))
                    {
                        ModelState.AddModelError("", "DocumentType is not valid");
                    }
                    FileValidations(jobDocumentRequest.DocumentName, false, "Document");

                    if (ModelState.IsValid)
                    {
                        if (jobDocumentRequest.DocumentBase64.Length < Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["FileSize"]))
                        {
                            data = _JobDetails.JobDocument(jobDocumentRequest, userdata);
                            UpdateCache(JobId);
                            WriteToLogFile("UploadJobDocument", "Cache updated", JobId.ToString());
                            //CommonBAL.SetCacheDataForJobID(0, data.JobId, userdata.UserId);
                            return Request.CreateResponse(HttpStatusCode.OK, data.obj);
                        }
                        else
                        {
                            data.obj = _JobDetails.JobResponse(null, "File Size should be less than 10mb");
                            return Request.CreateResponse(HttpStatusCode.BadRequest, data.obj);
                        }
                    }
                    else
                    {
                        string msg = string.Empty;
                        ModelState.Values.AsEnumerable().ToList().ForEach(d =>
                        {
                            for (int i = 0; i < d.Errors.Count; i++)
                            {
                                msg += d.Errors[i].ErrorMessage + Environment.NewLine;
                                if (d.Errors[i].Exception != null)
                                {
                                    msg += d.Errors[i].Exception.Message + Environment.NewLine;
                                }
                            }
                        });
                        data.obj = _JobDetails.JobResponse(null, msg);
                        return Request.CreateResponse(HttpStatusCode.BadRequest, data.obj);
                    }
                }
                else
                {
                    data.obj = _JobDetails.JobResponse(null, "VendorJobId doesn't exists.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, data.obj);
                }
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message, "UploadDocuments", "VendorJobId:" + Convert.ToString(jobDocumentRequest.VendorJobId));
                Log.WriteError(ex);
                data.obj.Message = ex.Message;
                data.obj.Status = false;
                data.obj.StatusCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, data.obj);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("DeleteJobImage")]
        public HttpResponseMessage DeleteJobImage(JobPhotoDeleteRequest jobPhotoDeleteRequest)
        {
            JobResponse data = new JobResponse();

            try
            {
                FormBot.Entity.User userdata = new User();
                userdata = GetUserInfo();
                if (userdata.SolarCompanyId == 0 || userdata.SolarCompanyId == null)
                {
                    data.Message = "You are not authorized to Delete Images.";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, data);
                }
                DataTable dt = _jobBAL.GetJobIdByVendorjobId(jobPhotoDeleteRequest.VendorJobId, Convert.ToInt32(userdata.SolarCompanyId));
                int JobId = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["JobID"]) : 0;
                if (JobId > 0)
                {

                    if (ModelState.IsValid)
                    {
                        data = _JobDetails.JobPhotoDelete(jobPhotoDeleteRequest);
                        UpdateCache(JobId);
                        //CommonBAL.SetCacheDataForJobID(0, JobId, userdata.UserId);
                        return Request.CreateResponse(HttpStatusCode.OK, data);
                    }
                    else
                    {
                        string msg = string.Empty;
                        ModelState.Values.AsEnumerable().ToList().ForEach(d =>
                        {
                            for (int i = 0; i < d.Errors.Count; i++)
                            {
                                msg += d.Errors[i].ErrorMessage + Environment.NewLine;
                                if (d.Errors[i].Exception != null)
                                {
                                    msg += d.Errors[i].Exception.Message + Environment.NewLine;
                                }
                            }
                        });
                        data = _JobDetails.JobResponse(null, msg);
                        return Request.CreateResponse(HttpStatusCode.BadRequest, data);
                    }
                }
                else
                {
                    data = _JobDetails.JobResponse(null, "VendorJobId doesn't exists.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, data);
                }
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message, "DeleteJobImage", "VendorJobPhotoId:" + Convert.ToString(jobPhotoDeleteRequest.VendorJobPhotoId));
                Log.WriteError(ex);
                data.Message = ex.Message;
                data.Status = false;
                data.StatusCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, data);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("DeleteDocuments")]
        public HttpResponseMessage DeleteDocuments(JobDocumentDeleteRequest jobDocumentDeleteRequest)
        {
            JobResponse data = new JobResponse();
            try
            {
                FormBot.Entity.User userdata = new User();
                userdata = GetUserInfo();
                if (userdata.SolarCompanyId == 0 || userdata.SolarCompanyId == null)
                {
                    data.Message = "You are not authorized to Delete Documents.";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, data);
                }
                DataTable dt = _jobBAL.GetJobIdByVendorjobId(jobDocumentDeleteRequest.VendorJobId, Convert.ToInt32(userdata.SolarCompanyId));
                int JobId = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["JobID"]) : 0;
                if (JobId > 0)
                {

                    if (ModelState.IsValid)
                    {
                        data = _JobDetails.JobDocumentDelete(jobDocumentDeleteRequest);
                        UpdateCache(JobId);
                        //CommonBAL.SetCacheDataForJobID(0, JobId, userdata.UserId);
                        return Request.CreateResponse(HttpStatusCode.OK, data);
                    }
                    else
                    {
                        string msg = string.Empty;
                        ModelState.Values.AsEnumerable().ToList().ForEach(d =>
                        {
                            for (int i = 0; i < d.Errors.Count; i++)
                            {
                                msg += d.Errors[i].ErrorMessage + Environment.NewLine;
                                if (d.Errors[i].Exception != null)
                                {
                                    msg += d.Errors[i].Exception.Message + Environment.NewLine;
                                }
                            }
                        });
                        data = _JobDetails.JobResponse(null, msg);
                        return Request.CreateResponse(HttpStatusCode.BadRequest, data);
                    }
                }
                else
                {
                    data = _JobDetails.JobResponse(null, "VendorJobId doesn't exists.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, data);
                }
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message, "DeleteDocuments", "VendorJobDocumentId:" + Convert.ToString(jobDocumentDeleteRequest.VendorJobDocumentId));
                Log.WriteError(ex);
                data.Message = ex.Message;
                data.Status = false;
                data.StatusCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, data);
            }
        }

        //private void WriteToLogFile(string errorMsg, string methodName, string ID)
        //{
        //    string sError = "Date:" + DateTime.Now + " MethodName:" + methodName + " " + ID + " Error:" + errorMsg;
        //    //StreamWriter sw = new StreamWriter(ProjectSession.VerdorAPIErrorLogs + "ErrorLogs.txt", append: true);
        //    StreamWriter sw = new StreamWriter(ProjectSession.VerdorAPIErrorLogs, append: true);
        //    sw.WriteLine(sError + "\n");
        //    sw.Flush();
        //    sw.Close();
        //}

        private void WriteToLogFile(string errorMsg, string methodName, string ID)
        {
            FileStream fs = null;
            try
            {
                //set up a filestream
                //fs = new FileStream(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6) + "\\FormBotLog.txt", FileMode.OpenOrCreate, FileAccess.Write);
                fs = new FileStream(FormBot.Helper.ProjectSession.VendorAPIErrorLogs, FileMode.OpenOrCreate, FileAccess.Write);

                //set up a streamwriter for adding text
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    //add the text
                    sw.WriteLine("Date:" + DateTime.Now + " MethodName:" + methodName + " " + ID + " Error:" + errorMsg);
                    //add the text to the underlying filestream
                    sw.Flush();
                    //close the writer
                    sw.Close();
                }
            }
            catch (Exception)
            {
                fs.Dispose();
                //throw;
            }
            //StreamWriter sw = new StreamWriter(fs);
            //find the end of the underlying filestream            
        }

        public string ConvertNumbertoWords(long number)
        {
            string words = string.Empty;
            if (number < 20 && number > 0)
            {
                var unitsMap = new[]
                {
                    "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN"
                };

                words = unitsMap[number] + (number == 1 ? " YEAR" : " YEARS");
            }
            return words;
        }

        [HttpGet]
        [Authorize]
        [Route("GetCustomField")]
        public HttpResponseMessage GetCustomField()
        {
            CustomFieldResponseModel data = new CustomFieldResponseModel();
            FormBot.Entity.User userdata = new User();
            userdata = GetUserInfo();
            try
            {
                data = _JobDetails.GetCustomField(userdata);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message, "GetCustomField", string.Empty);
                Log.WriteError(ex);
                data.Message = ex.Message;
                data.Status = false;
                data.StatusCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, data);
            }
        }

        public void UpdateCache(int jobId)
        {
            //var webRequest = System.Net.WebRequest.Create(string.Format(FormBot.Helper.ProjectSession.LiveWebUrl + "Service/UpdateCache?jobid=" + jobId));
            string serviceUrl = string.Format(FormBot.Helper.ProjectSession.LiveWebUrl + "Service/CheckBusinessRules_UpdateCache?jobid=" + jobId);
            WebRequest request = WebRequest.Create(serviceUrl);
            try
            {
                WebResponse response = request.GetResponse();
            }
            catch (Exception e)
            {
                WriteToLogFile("UpdateCache", e.Message, jobId.ToString());
            }
            WriteToLogFile("", request.RequestUri.AbsoluteUri.ToString(), jobId.ToString());

        }

        //public void PostalAddressValidation(CreateJob createJob)
        //{

        //    if (createJob.InstallerView != null)
        //    {
        //        CreateJob.InstallerView obj = new CreateJob.InstallerView();

        //        //obj = createJob.InstallerView;
        //    }
        //    if (createJob.DesignerView != null)
        //    {
        //        obj = createJob.DesignerView;
        //    }

        //    if (obj != null)
        //    {
        //        if (createJob.InstallerView.AddressID == 0)
        //        {
        //            ModelState.AddModelError("createJob.InstallerView.AddressID", "AddressID is required.");
        //        }
        //        else
        //        {
        //            if (createJob.InstallerView.AddressID == 1)
        //            {
        //                if (createJob.InstallerView.UnitTypeID == 0 && (createJob.InstallerView.StreetTypeID == 0 && string.IsNullOrEmpty(createJob.InstallerView.StreetName)))
        //                {
        //                    ModelState.AddModelError("createJob.InstallerView.UnitTypeID", "UnitTypeID is required.");
        //                    ModelState.AddModelError("createJob.InstallerView.StreetName", "StreetName is required.");
        //                    ModelState.AddModelError("createJob.InstallerView.StreetTypeID", "StreetTypeID is required.");
        //                }
        //                if (createJob.InstallerView.UnitTypeID == 0 && string.IsNullOrEmpty(createJob.InstallerView.StreetNumber))
        //                {
        //                    ModelState.AddModelError("createJob.InstallerView.StreetNumber", "StreetNumber is required.");
        //                }
        //                if (createJob.InstallerView.UnitTypeID > 0 && (createJob.InstallerView.UnitNumber == null || createJob.InstallerView.UnitNumber == ""))
        //                {
        //                    ModelState.AddModelError("createJob.InstallerView.UnitNumber", "UnitNumber is required.");
        //                }
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("createJob.InstallerView.PostalAddressID", "PostalAddressID is required.");
        //                ModelState.AddModelError("createJob.InstallerView.PostalDeliveryNumber", "PostalDeliveryNumber is required.");
        //            }
        //        }

        //    }

        //    //if (createJob.InstallerView != null)
        //    //{
        //    //    if (createJob.InstallerView.AddressID == 0)
        //    //    {
        //    //        ModelState.AddModelError("createJob.InstallerView.AddressID", "AddressID is required.");
        //    //    }
        //    //    else
        //    //    {
        //    //        if (createJob.InstallerView.AddressID == 1)
        //    //        {
        //    //            if (createJob.InstallerView.UnitTypeID == 0 && (createJob.InstallerView.StreetTypeID == 0 && string.IsNullOrEmpty(createJob.InstallerView.StreetName)))
        //    //            {
        //    //                ModelState.AddModelError("createJob.InstallerView.UnitTypeID", "UnitTypeID is required.");
        //    //                ModelState.AddModelError("createJob.InstallerView.StreetName", "StreetName is required.");
        //    //                ModelState.AddModelError("createJob.InstallerView.StreetTypeID", "StreetTypeID is required.");
        //    //            }
        //    //            if (createJob.InstallerView.UnitTypeID == 0 && string.IsNullOrEmpty(createJob.InstallerView.StreetNumber))
        //    //            {
        //    //                ModelState.AddModelError("createJob.InstallerView.StreetNumber", "StreetNumber is required.");
        //    //            }
        //    //            if (createJob.InstallerView.UnitTypeID > 0 && (createJob.InstallerView.UnitNumber == null || createJob.InstallerView.UnitNumber == ""))
        //    //            {
        //    //                ModelState.AddModelError("createJob.InstallerView.UnitNumber", "UnitNumber is required.");
        //    //            }
        //    //        }
        //    //        else
        //    //        {
        //    //            ModelState.AddModelError("createJob.InstallerView.PostalAddressID", "PostalAddressID is required.");
        //    //            ModelState.AddModelError("createJob.InstallerView.PostalDeliveryNumber", "PostalDeliveryNumber is required.");
        //    //        }
        //    //    }

        //    //}
        //}

        [HttpGet]
        [Authorize]
        [Route("GetStcSubmission")]
        public HttpResponseMessage GetStcSubmission(string VendorJobId = null)
        {
            GetStcSubmissionModel data = new GetStcSubmissionModel();
            try
            {
                //FormBot.Entity.User userdata = new User();
                //userdata = GetUserInfo();
                data = _JobDetails.GetStcSubmission(VendorJobId);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message, "GetStcSubmission", " VendorJobId:" + VendorJobId);
                Log.WriteError(ex);
                data.Message = ex.Message;
                data.Status = false;
                data.StatusCode = HttpStatusCode.InternalServerError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, data);
            }
        }

        //[HttpGet]
        //[Authorize]
        //[Route("GetJobPhotos")]
        //public HttpResponseMessage GetJobPhotos(string VendorJobId = null)
        //{
        //    GetJobPhotosModel data = new GetJobPhotosModel();
        //    try
        //    {
        //        //FormBot.Entity.User userdata = new User();
        //        //userdata = GetUserInfo();
        //        data = _JobDetails.GetJobPhotos(VendorJobId);
        //        return Request.CreateResponse(HttpStatusCode.OK, data);
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteToLogFile(ex.Message, "GetJobPhotos", " VendorJobId:" + VendorJobId);
        //        Log.WriteError(ex);
        //        data.Message = ex.Message;
        //        data.Status = false;
        //        data.StatusCode = HttpStatusCode.InternalServerError.ToString();
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, data);
        //    }
        //}

    }
}
