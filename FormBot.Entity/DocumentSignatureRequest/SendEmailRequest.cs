using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.DocumentSignatureRequest
{
    public class SendEmailRequest
    {
		public SendEmailRequest()
		{
			lstDocumentWiseSignatureDetail = new List<DocumentWiseSignatureDetails>();
		}
		[DisplayName("Reseller Name:")]        
        public int? ResellerID { get; set; }

        [DisplayName("Solar Company:")]
        public int? SolarCompanyId { get; set; }

        [DisplayName("Solar Company")]
        public string SolarCompany { get; set; }

		public int BulkUploadDocumentGroupId { get; set; }

		[DisplayName("Document Group Name")]
		public string DocumentGroupName { get; set; }

        public List<DocumentWiseSignatureDetails> lstDocumentWiseSignatureDetail { get; set; }

    }
	public class DocumentWiseSignatureDetails
	{

		public int JobId { get; set; }
		public string JobRefNumber { get; set; }
		public string CompanyName { get; set; }

		public string ClientName { get; set; }
		public string InstallerSignatureStatus { get; set; }
		public string DesignerSignatureStatus { get; set; }
		public string ElectricianSignatureStatus { get; set; }
		public string HomeOwnerSignatureStatus { get; set; }
		public string SolarCompanySignatureStatus { get; set; }


		public bool IsApplicable { get; set; }

		public int SentEmailStatus { get; set; }
		public string PVDSWHCode { get; set; }
		public int TotalRecords { get; set; }

		//Document Details
		public int JobDocumentId { get; set; }
		public string JobDocumentPath { get; set; }
		public int DocumentId { get; set; }

		public bool IsCompleted { get; set; }
		public string CustomMessage { get; set; }

		public int BulkUploadDocumentSignatureId { get; set; }	

	}
	public class EmailStatusResponce
	{
		public int InstallerEmailStatus { get; set; }
		public string InstallerFirstName { get; set; }
		public string InstallerLastName { get; set; }
		public string InstallerEmail { get; set; }


		public int DesignerEmailStatus { get; set; }
		public string DesignerFirstName { get; set; }
		public string DesignerLastName { get; set; }
		public string DesignerEmail { get; set; }


		public int ElectricianEmailStatus { get; set; }
		public string ElectricianFirstName { get; set; }
		public string ElectricianLastName { get; set; }
		public string ElectricianEmail { get; set; }

		public int HomeOwnerEmailStatus { get; set; }
		public string HomeOwnerFirstName { get; set; }
		public string HomeOwnerLastName { get; set; }
		public string HomeOwnerEmail { get; set; }

		public int SolarCompanyEmailStatus { get; set; }
		public string SolarCompanyFirstName { get; set; }
		public string SolarCompanyLastName { get; set; }
		public string SolarCompanyEmail { get; set; }

		public int SentEmailStatus { get; set; }

		public int BulkUploadDocumentSignatureId { get; set; }
	}
	
	public enum BulkUploadDocumentSignatureStatus
	{
		Yes = 1,
		No = 2,
		NA = 3
	}
}
