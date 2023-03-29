using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace FormBot.Helper
{
    public class HelperClasses
    {
        public class UploadStatus
        {
            public string FileName { get; set; }
            public bool Status { get; set; }
            public string Message { get; set; }
            public long AttachmentID { get; set; }
            public string Path { get; set; }
            public string MimeType { get; set; }
            public string AbsolutePath { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string isUnderInstallationArea { get; set; }

            public string createdDate { get; set; }
        }
    }
}
