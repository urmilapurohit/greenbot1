using System;
using Xero.Api.Example.Applications.Public;
using Xero.Api.Infrastructure.OAuth;
using System.Configuration;
using System.Web.Mvc;
using System.Net.Http;
using Xero.NetStandard.OAuth2.Client;
using Xero.NetStandard.OAuth2.Config;
using Microsoft.Extensions.DependencyInjection;
using Xero.NetStandard.OAuth2.Token;
using System.Threading.Tasks;
using System.Web;

namespace FormBot.Main.Helpers
{
    [Serializable]
    public class ApplicationSettingsTest
    {
        public string BaseApiUrl { get; set; }
        public Consumer Consumer { get; set; }
        public PublicMvcAuthenticator Authenticator { get; set; }
        public string Token { get; set; }
        public async Task<XeroOAuth2Token> CheckToken()
        {
            var xeroToken = TokenUtilities.GetStoredToken();
            var utcTimeNow = DateTime.UtcNow;

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

            if (utcTimeNow > xeroToken.ExpiresAtUtc)
            {
                var client = new XeroClient(XeroConfig, httpClientFactory);
                xeroToken = (XeroOAuth2Token)await client.RefreshAccessTokenAsync(xeroToken);
                TokenUtilities.StoreToken(xeroToken);
            }
            return xeroToken;
        }
        public static ApplicationSettingsTest xeroApiHelperSession
        {
            get
            {
                if (HttpContext.Current.Session["xeroApiHelperSession"] == null)
                {
                    return null;
                }
                else
                {
                    return (ApplicationSettingsTest)(HttpContext.Current.Session["xeroApiHelperSession"]);
                }
            }
            set
            {
                HttpContext.Current.Session["xeroApiHelperSession"] = value;
            }
        }
    }
}