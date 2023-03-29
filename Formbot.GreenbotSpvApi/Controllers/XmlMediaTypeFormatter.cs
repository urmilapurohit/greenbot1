using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;

namespace Formbot.GreenbotSpvApi.Controllers
{
    public class XmlMediaTypeFormatter : MediaTypeFormatter
    {
        public XmlMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/xml"));
        }

        //public override Task<object> ReadFromStreamAsync(Type type, Stream stream,
        //     HttpContentHeaders contentHeaders,
        //     IFormatterLogger formatterLogger)
        public virtual Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            try
            {
                var memoryStream = new MemoryStream();
                readStream.CopyTo(memoryStream);
                var s = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(s);

                taskCompletionSource.SetResult(xmlDoc);
            }
            catch (Exception e)
            {
                taskCompletionSource.SetException(e);
            }
            return taskCompletionSource.Task;
        }

        public override bool CanReadType(Type type)
        {
            return type == typeof(XmlDocument);
        }

        public override bool CanWriteType(Type type)
        {
            return false;
        }
    }



}