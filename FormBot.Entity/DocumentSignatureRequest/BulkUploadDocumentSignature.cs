using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.DocumentSignatureRequest
{
	public class BulkUploadDocumentSignature
	{
		public int BulkUploadDocumentSignatureId { get; set; }
		public int BulkUploadDocumentGroupId { get; set; }
		public int JobId { get; set; }
		public string InstallerSignatureStatus { get; set; }
		public string DesignerSignatureStatus { get; set; }
		public string ElectricianSignatureStatus { get; set; }
		public string HomeOwnerSignatureStatus { get; set; }
		public string SolarCompanySignatureStatus { get; set; }
		public DateTime CreatedDate { get; set; }
		public int CreatedBy { get; set; }
		public DateTime ModifiedDate { get; set; }
		public int ModifiedBy { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsApplicable { get; set; }
		public int SentEmailStatus { get; set; }
		public string PVDSWHCode { get; set; }
		public int JobDocumentId { get; set; }
		public string CustomMessage { get; set; }
		public bool IsCompleted { get; set; }
	}
}
