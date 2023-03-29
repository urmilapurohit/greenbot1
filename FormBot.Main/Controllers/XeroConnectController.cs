using FormBot.Helper;
using FormBot.Main.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Xero.Api.Example.Applications.Public;
using Xero.Api.Infrastructure.OAuth;
using Xero.NetStandard.OAuth2.Client;
using Xero.NetStandard.OAuth2.Config;

namespace FormBot.Main.Controllers
{
    public class XeroConnectController : BaseController
    {
        #region Properties

        private IMvcAuthenticator _authenticator;
        private ApiUser _user;
        ApplicationSettingsTest xeroApiHelper;

        #endregion

        #region Constructor

        /// <summary>
        /// XeroConnectController
        /// </summary>
        public XeroConnectController()
        {
            //if (XeroApiHelper.xeroApiHelperSession == null)
            //{
            //    xeroApiHelper = new XeroApiHelper();
            //    XeroApiHelper.xeroApiHelperSession = xeroApiHelper;
            //}
            //else
            //    xeroApiHelper = XeroApiHelper.xeroApiHelperSession;

            if(ApplicationSettingsTest.xeroApiHelperSession == null)
            {
                xeroApiHelper = new ApplicationSettingsTest();
                ApplicationSettingsTest.xeroApiHelperSession = xeroApiHelper;
            }
            else
               xeroApiHelper = ApplicationSettingsTest.xeroApiHelperSession;
            //_user = xeroApiHelper.User();
            //_authenticator = xeroApiHelper.MvcAuthenticator();
        }

        #endregion

        #region method

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>action result</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Connect
        /// </summary>
        /// <returns>action result</returns>
        public ActionResult Connect()
        {
            string authorizeUrl;
            try
            {
                var serviceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();
                var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

                XeroConfiguration XeroConfig = new XeroConfiguration
                {
                    ClientId = ConfigurationManager.AppSettings["XeroClientId"],
                    ClientSecret = ConfigurationManager.AppSettings["XeroClientSecret"],
                    CallbackUri = new Uri(ConfigurationManager.AppSettings["XeroCallbackUri"]),
                    Scope = ConfigurationManager.AppSettings["XeroScope"],
                    State = ConfigurationManager.AppSettings["XeroState"]
                };

                var client = new XeroClient(XeroConfig, httpClientFactory);

                return Redirect(client.BuildLoginUri());

            }
            catch (Exception ex)
            {
                Logger _log = new Logger();
                _log.LogFormat(SystemEnums.Severity.Error, "", ex);
                throw;
            }
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
            {
                var authorizeUrl = _authenticator.GetRequestTokenAuthorizeUrl(_user.Name);
                return Redirect(authorizeUrl);
            }
            else
            {
                return View(accessToken);
            }

            //string temp = "oauth_token :" + oauth_token + " _user.Name:" + _user.Name + " oauth_verifier:" + oauth_verifier + "TokenKey :" + accessToken.TokenKey + "TokenSecret: " + accessToken.TokenSecret + "UserId :" + accessToken.UserId + " Session :" + accessToken.Session;

            //Log(temp);
            //Log("UserName : " + _user.Name);
            //System.Web.HttpContext.Current.Session["TokenKey"] = accessToken.TokenKey ;
            //System.Web.HttpContext.Current.Session["TokenSecret"] = accessToken.TokenSecret;
            //System.Web.HttpContext.Current.Session["oauth_token"] = oauth_token;

            //if (!string.IsNullOrEmpty(oauth_token))
            //{
            //    if (XeroApiHelper.TokenForXero == null)
            //        XeroApiHelper.TokenForXero = XeroApiHelper.xeroApiHelperSession.MvcAuthenticator().RetrieveAndStoreAccessToken(XeroApiHelper.xeroApiHelperSession.User().Name, oauth_token, oauth_verifier, org);

            //}
        }

        #endregion
    }
}