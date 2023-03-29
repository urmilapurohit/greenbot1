using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.DocumentSignatureRequest
{

    public class DocumentSignatureRequest
    {
        public BulkUploadDocumentGroup bulkUploadDocumentGroup { get; set; }

        public SendEmailRequest sendEmailRequest { get; set; }
    }
    public class BulkUploadDocumentGroup
    {
        public int BulkUploadDocumentGroupId {get; set;}

        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.BulkUploadDocumentGroupId));
            }
        }

        [Required(ErrorMessage = "Group Name is required")]
        [Display(Name = "Group Name:")]
        public string GroupName { get; set; }

        public string GroupDocumentPath { get; set; }

		[Display(Name = "Document Template Name:")]
		public string DocumentTemplateName { get; set; }

		public int CreatedBy { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public bool IsDeleted { get; set; }

        public int TotalRecords { get; set; }

        public int StateID { get; set; }

		public string GroupEditURL { get; set; }
	}

	public class DocumentSignatureStatusWithEmailResponce
	{
		public string InstallerEmail { get; set; }
		public string InstallerFirstName { get; set; }
		public string InstallerLastName { get; set; }


		public string DesignerEmail { get; set; }
		public string DesignerFirstName { get; set; }
		public string DesignerLastName { get; set; }

		public string ElectricianEmail { get; set; }
		public string ElectricianFirstName { get; set; }
		public string ElectricianLastName { get; set; }

		public string OwnerEmail { get; set; }
		public string OwnerFirstName { get; set; }
		public string OwnerLastName { get; set; }

		public string SolarCompanyEmail { get; set; }
		public string SolarCompanyFirstName { get; set; }
		public string SolarCompanyLastName	 { get; set; }
	}
}
