using FormBot.BAL.Service;
using FormBot.BAL.Service.Job;
using FormBot.Entity.Job;
using FormBot.Helper;
using FormBot.Main.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Xero.Api.Core.Model;
using Xero.Api.Example.Applications.Public;
using Xero.Api.Infrastructure.Exceptions;
using Xero.Api.Infrastructure.OAuth;

namespace FormBot.Main.Controllers
{
    public class XeroOrganisationController : BaseController
    {
        #region Properties

        private readonly IJobPartsBAL _jobParts;
        private IMvcAuthenticator _authenticator;
        private ApiUser _user;

        #endregion

        #region Constructor

        /// <summary>
        /// XeroOrganisationController
        /// </summary>
        public XeroOrganisationController()
        {
            //this._jobParts = jobParts;
            //_user = XeroApiHelper.User();
            //_authenticator = XeroApiHelper.MvcAuthenticator();
        }

        #endregion

        #region Method

        /// <summary>
        /// Connect
        /// </summary>
        /// <returns>Action Result</returns>
        public ActionResult Connect()
        {
            var authorizeUrl = _authenticator.GetRequestTokenAuthorizeUrl(_user.Name);

            return Redirect(authorizeUrl);
        }

        /// <summary>
        /// Authorize
        /// </summary>
        /// <param name="oauth_token">oauth_token</param>
        /// <param name="oauth_verifier">oauth_verifier</param>
        /// <param name="org">org</param>
        /// <returns>action result</returns>
        public ActionResult Authorize(string oauth_token, string oauth_verifier, string org)
        {
            var accessToken = _authenticator.RetrieveAndStoreAccessToken(_user.Name, oauth_token, oauth_verifier, org);
            if (accessToken == null)
                return View("NoAuthorized");

            try
            {
                //SyncJobPartItems();
                return RedirectToAction("GetJobParts", "XeroOrganisation");
            }
            catch (RenewTokenException e)
            {
                return RedirectToAction("Connect", "XeroConnect");
            }
            //return View(accessToken);
        }

        /// <summary>
        /// SyncJobPartItems
        /// </summary>
        public void SyncJobPartItems()
        {
        }

        /// <summary>
        /// GetJobParts
        /// </summary>
        /// <returns>action result</returns>
        [HttpGet]      
        public ActionResult GetJobParts()
        {
            JobParts jobParts = new JobParts();
            jobParts.UserTypeId = ProjectSession.UserTypeId;
            return View("GetJobParts", jobParts);
        }

        /// <summary>
        /// GetJobPartsList
        /// </summary>
        /// <param name="itemCodeOrDescription">itemCodeOrDescription</param>
        public void GetJobPartsList(string itemCodeOrDescription = "")
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);

            JobPartsBAL jobParts = new JobPartsBAL();
            IList<JobParts> lstJobParts = jobParts.GetJobPartsList(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, ProjectSession.SolarCompanyId, itemCodeOrDescription,false);
            if (lstJobParts.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstJobParts.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstJobParts.FirstOrDefault().TotalRecords;

            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstJobParts, gridParam));
        }

        #endregion
    }
}