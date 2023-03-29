using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Email
{
    public class EmailMessageWithAttachments
    {
        public string from { get; set; }
        public string to { get; set; }
        public string subject { get; set; }
        public string short_date { get; set; }
        public string full_date { get; set; }
        public long id { get; set; }
        public bool has_attachments { get; set; }
        public bool flags { get; set; }
        public string MessageClass { get; set; }
        [NotMapped]
        public DateTime msg_date { get; set; }

        public string body_text { get; set; }

        public string FolderName { get; set; }

        public List<string> Attachments { get; set; }

        public string email { get; set; }

        public long id_acct { get; set; }

        public bool IsSendFromSender { get; set; }

        public string SenderName { get; set; }

        public string ReceiverName { get; set; }
    }
}
