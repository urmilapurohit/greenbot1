using FormBot.Main.Helpers;
using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Xero.NetStandard.OAuth2.Client;
using Xero.NetStandard.OAuth2.Config;
using Xero.NetStandard.OAuth2.Token;
using System.Threading.Tasks;

public static class TokenUtilities
{
        
    public static void StoreToken(XeroOAuth2Token xeroToken)
    {
        string serializedXeroToken = JsonSerializer.Serialize(xeroToken);
        ApplicationSettingsTest xeroApiHelper = new ApplicationSettingsTest();
        xeroApiHelper.Token = serializedXeroToken;
        ApplicationSettingsTest.xeroApiHelperSession = xeroApiHelper;
    }

    public static XeroOAuth2Token GetStoredToken()
    {
        
        var xeroApiHelper = ApplicationSettingsTest.xeroApiHelperSession;
        var xeroToken = JsonSerializer.Deserialize<XeroOAuth2Token>(xeroApiHelper.Token);

        return xeroToken;
    }

    public static bool TokenExists()
    {
        var xeroApiHelper = ApplicationSettingsTest.xeroApiHelperSession;
        if (xeroApiHelper != null)
        {
            if (!string.IsNullOrWhiteSpace(xeroApiHelper.Token))
            {
                var xeroToken = JsonSerializer.Deserialize<XeroOAuth2Token>(xeroApiHelper.Token);
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
                    var response = Task.Run(async() => await client.RefreshAccessTokenAsync(xeroToken));
                    response.Wait();
                    xeroToken = (XeroOAuth2Token)(response.Result);
                    StoreToken(xeroToken);
                }
            }
            return !string.IsNullOrWhiteSpace(xeroApiHelper.Token);
        }
        else
            return false;
    }

}