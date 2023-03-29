using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Email
{
    public class ComposeEmail
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public innerBody Body { get; set; }
        public string Attachment { get; set; }
        public bool IsSave { get; set; }
        public string FullDate { get; set; }
        public string ShortDate { get; set; }
        public List<AttachmentData> Attachments { get; set; }
        public int SaveMailFolderId { get; set; }
    }

    public class innerBody
    {
        public string body { get; set; }
    }

    public class AttachmentData
    {
        public string FileName { get; set; }
        public string GeneratedName { get; set; }
    }
}
