using FormBot.DAL;
using FormBot.Entity.Notification;
using Newtonsoft.Json.Linq;
using PushSharp.Apple;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FormBot.BAL.Service
{
    public class PushNotification : IPushNotification
    {
        /// <summary>
        /// Sends the push notification.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        public void SendPushNotification(int userID, string message)
        {
            string spName = "[GetPushToken]";
            message = userID.ToString() + "::" + message;
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userID));
            DataSet dataset = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            string deviceToken = string.Empty;
            string deviceType = string.Empty;
            if (dataset != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dataset.Tables[0].Rows.Count; i++)
                {
                    deviceToken = Convert.ToString(dataset.Tables[0].Rows[i]["AccessToken"]);
                    deviceType = Convert.ToString(dataset.Tables[0].Rows[i]["Type"]);
                    //string message = "Job has been schedule";
                    if (deviceType == "iphone")
                    {
                        if (!string.IsNullOrEmpty(deviceToken))
                        {
                            IphoneNotification(message, deviceToken);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(deviceToken))
                        {
                            AndroidNotification(message, deviceToken);
                        }
                    }
                }
            }


        }

        /// <summary>
        /// Androids the notification.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="token">The token.</param>
        public void AndroidNotification(string message, string token)
        {
            string serverKey = ConfigurationManager.AppSettings["AndroidNotificationFCMServerKey"].ToString();
            string AppKey = ConfigurationManager.AppSettings["AndroidNotificationFCMAppKey"].ToString();

            try
            {
                var result = "-1";
                var webAddr = "https://fcm.googleapis.com/fcm/send";

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add("Authorization:key=" + serverKey);
                httpWebRequest.Headers.Add("Sender: id=" + AppKey);
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"to\": \"" + token + "\",\"data\": {\"message\": \"" + message + "\"},\"priority\":\"high\"}";
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }

                // return result;
            }
            catch (Exception ex)
            {
                WriteToLogFile("Andoid failure :" + ex.Message + DateTime.Now);
            }

        }

        /// <summary>
        /// Iphones the notification.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="token">The token.</param>
        public void IphoneNotification(string message, string token)
        {
            //string[] MY_DEVICE_TOKENS = new string[] { token };

            //// Configuration (NOTE: .pfx can also be used here)
            //var config = new ApnsConfiguration(ApnsConfiguration.ApnsServerEnvironment.Production, System.IO.File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/Certificate/") + "GreenBot.p12"), ConfigurationManager.AppSettings["AppleCertificatePrivateKeyPassword"].ToString());

            //// Create a new broker
            //var apnsBroker = new ApnsServiceBroker(config);
            ////bool isSuccess = false;
            //// Wire up events
            //apnsBroker.OnNotificationFailed += (notification, aggregateEx) =>
            //{

            //    aggregateEx.Handle(ex =>
            //    {
            //        //isSuccess = false;
            //        // See what kind of exception it was to further diagnose
            //        if (ex is ApnsNotificationException)
            //        {
            //            var notificationException = (ApnsNotificationException)ex;

            //            // Deal with the failed notification
            //            var apnsNotification = notificationException.Notification;
            //            var statusCode = notificationException.ErrorStatusCode;

            //            //Utility.Log("Apple Notification Failed: ID={apnsNotification.Identifier}, Code={statusCode}");
            //            //WriteToLogFile("Apple Notification Failed: ID={" + apnsNotification.Identifier + "}, Code={" + statusCode + "}" + DateTime.Now);

            //        }
            //        else
            //        {
            //            // Inner exception might hold more useful information like an ApnsConnectionException           
            //            //Utility.Log("Apple Notification Failed for some unknown reason : {ex.InnerException}");
            //            //WriteToLogFile("Apple Notification Failed for some unknown reason : {" + ex.InnerException + "}" + DateTime.Now);
            //        }

            //        // Mark it as handled
            //        return true;
            //    });
            //};

            //apnsBroker.OnNotificationSucceeded += (notification) =>
            //{
            //    //Console.WriteLine("Apple Notification Sent!");
            //    WriteToLogFile("Push success : " + DateTime.Now);
            //    //isSuccess = true;
            //};

            //// Start the broker
            //apnsBroker.Start();

            //foreach (var deviceToken in MY_DEVICE_TOKENS)
            //{
            //    // Queue a notification to send
            //    apnsBroker.QueueNotification(new ApnsNotification
            //    {
            //        DeviceToken = deviceToken,
            //        Payload = JObject.Parse("{\"aps\" : { \"alert\" : \"" + message + "\" }}")
            //    });
            //}

            //// Stop the broker, wait for it to finish   
            //// This isn't done after every message, but after you're
            //// done with the broker
            //apnsBroker.Stop();
        }

        private static void WriteToLogFile(string content)
        {
            FileStream fs = null;
            try
            {
                //set up a filestream
                fs = new FileStream(FormBot.Helper.ProjectSession.LogFilePath, FileMode.OpenOrCreate, FileAccess.Write);
                //set up a streamwriter for adding text
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    //add the text
                    sw.WriteLine(content);
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

        public void SendPushNotificationForFilteredUser(PushNotificationsSend pushNotification)

        {
            //string spName = "[GetPushTokenForAll]";
            //List<SqlParameter> sqlParameters = new List<SqlParameter>();

            //DataSet dataset = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            string deviceToken = string.Empty;
            string deviceType = string.Empty;
            string spName = "[SendPushNotificationForFilteredUser]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, pushNotification.ResellerId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, pushNotification.SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("ElectricianId", SqlDbType.Int, pushNotification.ElectricianId));
            sqlParameters.Add(DBClient.AddParameters("ContractorId", SqlDbType.Int, pushNotification.ContractorId));
            sqlParameters.Add(DBClient.AddParameters("JobType", SqlDbType.Int, pushNotification.JobType));
            sqlParameters.Add(DBClient.AddParameters("Platform", SqlDbType.Int, pushNotification.Platform));

            DataSet dataset = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());

            //var userids = dataset.Tables[0].AsEnumerable().Select(r => r.Field<int>("UserID")).ToList();

            //if (dataset != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
            //{
            //foreach(var user in userids)
            //{

            if (dataset != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in dataset.Tables[0].Rows)
                {
                    deviceToken = Convert.ToString(dr["AccessToken"]);
                    deviceType = Convert.ToString(dr["Type"]);
                    string userId= Convert.ToString(dr["UserID"]);

                    string msg = string.Empty;
                    if (deviceType == "iphone")
                    {
                        if (!string.IsNullOrEmpty(deviceToken))
                        {
                            IphoneNotification(pushNotification.Notification, deviceToken);

                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(deviceToken))
                        {
                            msg = userId.ToString() + "::" + pushNotification.Notification;
                            AndroidNotification(msg, deviceToken);

                        }

                        //}


                    }
                }

            }


        }

        public void SendPushNotificationForAll(string message)
        {

            string spName = "[GetPushTokenForAll]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            DataSet dataset = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            string deviceToken = string.Empty;
            string deviceType = string.Empty;
            var userids = dataset.Tables[0].AsEnumerable().Select(r => r.Field<int>("UserID")).ToList();

            //}


            if (dataset != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
            {
                foreach (var user in userids)
                {
                    deviceToken = Convert.ToString(dataset.Tables[0].Rows[0]["AccessToken"]);
                    deviceType = Convert.ToString(dataset.Tables[0].Rows[0]["Type"]);
                    if (deviceType == "iphone")
                    {
                        if (!string.IsNullOrEmpty(deviceToken))
                        {
                            IphoneNotification(message, deviceToken);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(deviceToken))
                        {
                            AndroidNotification(message, deviceToken);



                        }
                    }
                }
            }
        }
    }
}