using Newtonsoft.Json.Linq;
using PushSharp.Apple;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FormBot.WebAPI.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>action result</returns>
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult SendPush(string token, string type)
        {
            if (type.Equals("iphone"))
            {
                string[] myDeviceToken = new string[] { token };
                var config = new ApnsConfiguration(ApnsConfiguration.ApnsServerEnvironment.Production, System.IO.File.ReadAllBytes(Server.MapPath("~/Certificate/") + "GreenBot.p12"), ConfigurationManager.AppSettings["AppleCertificatePrivateKeyPassword"].ToString());
                var apnsBroker = new ApnsServiceBroker(config);
                apnsBroker.OnNotificationFailed += (notification, aggregateEx) =>
                {
                    aggregateEx.Handle(ex =>
                    {
                        if (ex is ApnsNotificationException)
                        {
                            var notificationException = (ApnsNotificationException)ex;
                            var apnsNotification = notificationException.Notification;
                            var statusCode = notificationException.ErrorStatusCode;
                            WriteToLogFile("Apple Notification Failed: ID={" + apnsNotification.Identifier + "}, Code={" + statusCode + "}" + DateTime.Now);
                        }
                        else
                        { 
                            WriteToLogFile("Apple Notification Failed for some unknown reason : {" + ex.InnerException + "}" + DateTime.Now);
                        }

                        return true;
                    });

                };

                apnsBroker.OnNotificationSucceeded += (notification) =>
                {
                    WriteToLogFile("Push success final : " + DateTime.Now);
                };

                apnsBroker.Start();
                foreach (var deviceToken in myDeviceToken)
                {
                    apnsBroker.QueueNotification(new ApnsNotification
                    {
                        DeviceToken = deviceToken,
                        Payload = JObject.Parse("{\"aps\" : { \"alert\" : \"hello\" }}")

                    });
                }

                apnsBroker.Stop();
            }
            else
            {
                const string MESSAGE = "Good one123 - 000";
                string[] tokens = { token };
                var jGcmData = new JObject();
                var jData = new JObject();
                JArray jarrayTokens = new JArray();
                foreach (string token1 in tokens)
                {
                    jarrayTokens.Add(token1);
                }

                jData.Add("message", MESSAGE);
                jGcmData.Add("registration_ids", jarrayTokens);
                jGcmData.Add("data", jData);
                var url = new Uri("https://gcm-http.googleapis.com/gcm/send");
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));

                        client.DefaultRequestHeaders.TryAddWithoutValidation(
                            "Authorization", "key=" + ConfigurationManager.AppSettings["AndroidNotificationAPIKey"].ToString());

                        Task.WaitAll(client.PostAsync(url,
                            new StringContent(jGcmData.ToString(), Encoding.Default, "application/json"))
                                .ContinueWith(response =>
                                {
                                    Console.WriteLine(response);
                                    Console.WriteLine("Message sent: check the client device notification tray.");
                                }));
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("Unable to send GCM message:");
                    Console.Error.WriteLine(e.StackTrace);
                }

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        private static void WriteToLogFile(string content)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(FormBot.Helper.ProjectSession.LogFilePath, FileMode.OpenOrCreate, FileAccess.Write);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine(content);
                    sw.Flush();
                    sw.Close();
                }

            }
            catch (Exception)
            {
                fs.Dispose();
            }
           
        }
    }
}