using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
namespace FormBot.Helper
{
    public class Serializer
    {
       /// <summary>
        /// JSON Serialization
       /// </summary>
        /// <typeparam name="T">Type T</typeparam>
        /// <param name="t">object t</param>
        /// <returns>The string</returns>
        public static string JsonSerializer<T>(T t)
        {
            try
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                MemoryStream ms = new MemoryStream();
                ser.WriteObject(ms, t);
                string jsonString = Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();
                return jsonString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       /// <summary>
        /// JSON Deserialization
       /// </summary>
       /// <typeparam name="T">Type T</typeparam>
        /// <param name="jsonString">json String</param>
       /// <returns>Type t</returns>
        public static T JsonDeserialize<T>(string jsonString)
        {
            try
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
                T obj = (T)ser.ReadObject(ms);
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string SerializeObject(object myObject)
        {
            var stream = new MemoryStream();
            var xmldoc = new XmlDocument();
            var serializer = new XmlSerializer(myObject.GetType());
            using (stream)
            {
                serializer.Serialize(stream, myObject);
                stream.Seek(0, SeekOrigin.Begin);
                xmldoc.Load(stream);
            }

            return xmldoc.InnerXml;
        }

        private static object DeSerializeObject(object myObject, Type objectType)
        {
            var xmlSerial = new XmlSerializer(objectType);
            var xmlStream = new StringReader(myObject.ToString());
            return xmlSerial.Deserialize(xmlStream);
        }

        public static JsonResult GetJsonResult(object Data)
        {
            JsonResult json = new JsonResult();
            json.Data = Data;
            json.MaxJsonLength = Int32.MaxValue;
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return json;
        }
    }
}
