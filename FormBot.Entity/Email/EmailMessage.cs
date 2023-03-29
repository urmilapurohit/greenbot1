using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Email
{
    public class EmailMessage
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
    }
}
