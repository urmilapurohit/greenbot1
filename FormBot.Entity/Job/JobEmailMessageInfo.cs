using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    /// <summary>
    /// JobEmail Message Info
    /// </summary>
    public class JobEmailMessageInfo
    {
        /// <summary>
        /// Gets or sets the message_date.
        /// </summary>
        /// <value>
        /// The message_date.
        /// </value>
        public DateTime msg_date { get; set; }
        /// <summary>
        /// Gets or sets the attachments.
        /// </summary>
        /// <value>
        /// The attachments.
        /// </value>
        public bool attachments { get; set; }
        /// <summary>
        /// Gets or sets the id_acct.
        /// </summary>
        /// <value>
        /// The id_acct.
        /// </value>
        public int id_acct { get; set; }
        /// <summary>
        /// Gets or sets the id_message.
        /// </summary>
        /// <value>
        /// The id_message.
        /// </value>
        public int id_msg { get; set; }
        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        public string subject { get; set; }
        /// <summary>
        /// Gets or sets the from_message.
        /// </summary>
        /// <value>
        /// The from_message.
        /// </value>
        public string from_msg { get; set; }
        /// <summary>
        /// Gets or sets the to_message.
        /// </summary>
        /// <value>
        /// The to_message.
        /// </value>
        public string to_msg { get; set; }
        /// <summary>
        /// Gets or sets the CC_MSG.
        /// </summary>
        /// <value>
        /// The CC_MSG.
        /// </value>
        public string cc_msg { get; set; }
        /// <summary>
        /// Gets or sets the BCC_MSG.
        /// </summary>
        /// <value>
        /// The BCC_MSG.
        /// </value>
        public string bcc_msg { get; set; }

        /// <summary>
        /// Gets or sets the name of the folder.
        /// </summary>
        /// <value>
        /// The name of the folder.
        /// </value>
        public string FolderName { get; set; }

        /// <summary>
        /// Gets or sets the send or receive from email.
        /// </summary>
        /// <value>
        /// The send or receive from email.
        /// </value>
        public string SendOrReceiveFromEmail { get; set; }

        public string FullDate { get; set; }
    }
}
