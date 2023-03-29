using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Email
{
    /// <summary>
    /// Email Thread
    /// </summary>
    public class EmailThread
    {
        public long EmailThreadId { get; set; }

        public int JobId { get; set; }

        public int Id_AcctId { get; set; }

        public string Sender { get; set; }

        public string Receiver { get; set; }

        public string SubjectGroup { get; set; }

        public string Subject { get; set; }

        public DateTime ModifiedDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public string SenderEmail { get; set; }

        public string ReceiverEmail { get; set; }

        public string FullDate { get; set; }
    }
}
