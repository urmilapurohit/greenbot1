using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Email
{
	public partial class QueuedEmail
	{
		public QueuedEmail()
		{
			lstAttechment = new List<EmialAttechment>();
		}
		public int QueuedEmailForNotifyInstallerId { get; set; }
		public int QueuedEmailId { get; set; }
		public string FromEmail { get; set; }
		public string ToEmail { get; set; }
		public string CC { get; set; }
		public string Bcc { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
		public Guid Guid { get; set; }
		public bool IsSent { get; set; }
		public int SentTries { get; set; }
		public DateTime? SentOn { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime ModifiedDate { get; set; }
		[NotMapped]
		public virtual List<EmialAttechment> lstAttechment { get; set; }
		public string JobId { get; set; }	
	}
	public partial class EmialAttechment
	{
		public int AttachmentId { get; set; }
		public int QueuedEmailId { get; set; }
		public string FilePath { get; set; }
		public string FileName { get; set; }
		public Guid Guid { get; set; }
		public string FileMimeType { get; set; }
		public DateTime CreatedDate { get; set; }
	}
}
