using FormBot.Helper;
using FormBot.Main.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using Xero.Api.Core;
using Xero.Api.Example.Applications.Public;
using Xero.Api.Infrastructure.Interfaces;
using Xero.Api.Infrastructure.OAuth;
using Xero.Api.Serialization;
using System.Configuration;
using System.Web.Mvc;
using System.Net.Http;
using Xero.NetStandard.OAuth2.Client;
using Xero.NetStandard.OAuth2.Config;
using Microsoft.Extensions.DependencyInjection;
using Xero.NetStandard.OAuth2.Token;
using Xero.NetStandard.OAuth2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormBot.Main.Helpers
{
    [Serializable]
    public class ApplicationSettings : Attribute
    {
        public string BaseApiUrl { get; set; }
        //public Consumer Consumer { get; set; }

        private Consumer _consumer;  // backing field for your property, which can have the NonSerialized attribute.

        public Consumer Consumer // property, which now doesn't need the NonSerialized attribute.
        {
            get { return _consumer; }
            set { _consumer = value; }
        }

        public object Authenticator { get; set; }
    }

    //[AttributeUsageAttribute(AttributeTargets.Field, Inherited = false)]
    //[ComVisibleAttribute(true)]
    //public sealed class NonSerializedAttribute : Attribute

    [Serializable]
    public class XeroApiHelper
    {
       
        PublicMvcAuthenticator _publicAuthenticator;

        public PublicMvcAuthenticator PublicAuthenticator // property, which now doesn't need the NonSerialized attribute.
        {
            get { return _publicAuthenticator; }
            set { _publicAuthenticator = value; }
        }
        
        public static XeroApiHelper xeroApiHelperSessionTest
        {
            get
            {
                if (HttpContext.Current.Session["xeroApiHelperSessionTest"] == null)
                {
                    return null;
                }
                else
                {
                    return (XeroApiHelper)(HttpContext.Current.Session["xeroApiHelperSessionTest"]);
                }
            }
            set
            {
                HttpContext.Current.Session["xeroApiHelperSessionTest"] = value;
            }
        }

        private ApplicationSettings _applicationSettings;

        //public static string setCallBackUrl = string.Empty;

        public  XeroApiHelper()
        {
            var callbackUrl = ProjectSession.XeroAuthorizeURL;

            var publicAppConsumerKey = ProjectConfiguration.XeroPublicAppConsumerKey;
            var publicAppConsumerSecret = ProjectConfiguration.XeroPublicAppConsumerSecret;
            MemoryAccessTokenStore memoryStore = new MemoryAccessTokenStore();
            MemoryRequestTokenStore requestTokenStore = new MemoryRequestTokenStore();

            var baseApiUrl = "https://api.xero.com";
            // Public Application Settings
            var publicConsumer = new Consumer(publicAppConsumerKey, publicAppConsumerSecret);

            PublicAuthenticator = new PublicMvcAuthenticator(baseApiUrl, baseApiUrl, callbackUrl, memoryStore,
                publicConsumer, requestTokenStore);

            var publicApplicationSettings = new ApplicationSettings
            {
                BaseApiUrl = baseApiUrl,
                Consumer = publicConsumer,
                Authenticator = PublicAuthenticator
            };

            //var serviceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();
            //var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

            //XeroConfiguration XeroConfig = new XeroConfiguration
            //{
            //    ClientId = ConfigurationManager.AppSettings["XeroClientId"],
            //    ClientSecret = ConfigurationManager.AppSettings["XeroClientSecret"],
            //    CallbackUri = new Uri(ConfigurationManager.AppSettings["XeroCallbackUri"]),
            //    Scope = ConfigurationManager.AppSettings["XeroScope"],
            //    State = ConfigurationManager.AppSettings["XeroState"]
            //};

            //var client = new XeroClient(XeroConfig, httpClientFactory);

            //return Redirect(client.BuildLoginUri());


            _applicationSettings = publicApplicationSettings;
        }

        public ApiUser User()
        {
            //return new ApiUser { Name = Environment.MachineName };
            return new ApiUser { Name = ProjectSession.LoggedInUserId.ToString() };
        }

        public IConsumer Consumer()
        {
            return _applicationSettings.Consumer;
        }

        public IMvcAuthenticator MvcAuthenticator()
        {
            return (IMvcAuthenticator)_applicationSettings.Authenticator;
        }

        public IXeroCoreApi CoreApi()
        {
            if (_applicationSettings.Authenticator is ICertificateAuthenticator)
            {
                return new XeroCoreApi(_applicationSettings.BaseApiUrl, _applicationSettings.Authenticator as ICertificateAuthenticator,
                    _applicationSettings.Consumer, User(), new DefaultMapper(), new DefaultMapper());
            }

            if (_applicationSettings.Authenticator is IAuthenticator)
            {
                return new XeroCoreApi(_applicationSettings.BaseApiUrl, _applicationSettings.Authenticator as IAuthenticator,
                    _applicationSettings.Consumer, User(), new DefaultMapper(), new DefaultMapper());
            }

            return null;
        }

    }

}