using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Email
{
    public class EmailInfo
    {
        [NotMapped]
        public int TemplateID { get; set; }

        [NotMapped]
        public string Sender { get; set; }

        [NotMapped]
        public List<string> Receiver { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string CompanyABN { get; set; }

        public string CompanyName { get; set; }

        public string BSB { get; set; }

        public string AccountNumber { get; set; }

        public string AccountName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string Phone { get; set; }

        public string LoginLink { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Details { get; set; }

        public string SolarCompanyDetails { get; set; }

        public string SolarElectrician { get; set; }

        public string PasswordLink { get; set; }

        public string JobName { get; set; }

        public string Address { get; set; }

        public string CECAccreditationNumber { get; set; }

        public string LicensedElectricianNumber { get; set; }

        public string CECDesignerNumber { get; set; }

        #region ComplianceIssue Template
        public string ReferenceNumber { get; set; }

        public string OwnerName { get; set; }

        public string InstallationAddress { get; set; }

        public decimal? SystemSize { get; set; }

        public string STCsValue { get; set; }

        public string AutomatedErrors { get; set; }

        public string ComplianceNotes { get; set; }

        public decimal TotalValue { get; set; }

        public string ComplianceOfficerName { get; set; }

        public DateTime Date { get; set; }
        #endregion

        public string FailureNotice { get; set; }

        public string UrgentJobList { get; set; }
        public string Signature { get; set; }
        public string PVDCode { get; set; }

        public string RECCreationDate { get; set; }
        public string IndexOfEmail { get; set; }
        public string TotalEmailCount { get; set; }
        public string RACompanyName { get; set; }
        public string JobDuplicationDetails { get; set; }
        public string PVDorSWHDescription { get; set; }
        public string SolarCompanyFullName { get; set; }
        public string ResellerFullName { get; set; }

        public string FullFileName { get; set; }
        public string DisplayFileName { get; set; }

        public string SignatureType { get; set; }
        public string SignatureCapturedFullName { get; set; }

        public string VisitId { get; set; }

        public string PanelBrand { get; set; }

        public string PanelModel { get; set; }

        public string TotalPanelAmount { get; set; }

        public string InverterBrand { get; set; }

        public string InverterModel { get; set; }

        public string TotalInverterAmount { get; set; }

        public DateTime InstallationDate { get; set; }

        public string InstallerName { get; set; }

        public string JobDetailLink { get; set; }

        public string OwnerCompanyName { get; set; }

        public int JobID { get; set; }

        public int UserID { get; set; }
        public string UserDetailLink { get; set; }

        [NotMapped]
        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.JobID));
            }
        }

        public string InstallerEmail { get; set; }
        public string LoggedInUsername { get; set; }
    }



}
