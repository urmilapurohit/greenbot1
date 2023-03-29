using FormBot.Entity.ImportFile;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class AccreditedInstallers
    {
        public int AccreditedInstallerId { get; set; }
        public string InstallerStatus { get; set; }
        public string AccreditationNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccountName { get; set; }
        public string MailingAddressUnitType { get; set; }
        public string MailingAddressUnitNumber { get; set; }
        public string MailingAddressStreetNumber { get; set; }
        public string MailingAddressStreetName { get; set; }
        public string MailingAddressStreetType { get; set; }
        public string MailingCity { get; set; }
        public string MailingState { get; set; }
        public string PostalCode { get; set; }
        public string MailingCountry { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string GridType { get; set; }
        public string SPS { get; set; }
        public DateTime? InstallerFullAwardDate { get; set; }
        public DateTime? InstallerProvisionalAwardDate { get; set; }
        public DateTime? InstallerExpiryDate { get; set; }
        public DateTime? SuspensionStartDate { get; set; }
        public DateTime? SuspensionEndDate { get; set; }
        public string LicensedElectricianNumber { get; set; }
        public string Endorsements { get; set; }

        [NotMapped]
        public int TotalRecords { get; set; }
        [NotMapped]
        public string Abbreviation { get; set; }

        public int StateID { get; set; }
        public int UnitTypeID { get; set; }
        public int StreetTypeID { get; set; }
        public int RoleId { get; set; }
        public string Inst_Phone { get; set; }
        public string Inst_Mobile { get; set; }
        public int Inst_UnitTypeID { get; set; }
        public string SendRequest { get; set; }
        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.AccreditedInstallerId));
            }
        }

        public int? SWHInstallerId { get; set; }
        public string FullName { get; set; }
        public string FullAddress { get; set; }
        public bool IsSystemUser { get; set; }
        public bool IsPVDUser { get; set; }
        public bool IsSWHUser { get; set; }
        public int UserId { get; set; }
        public bool IsPostalAddress { get; set; }
       public string PostalDeliveryNumber { get; set; }
        public int PostalAddressID { get; set; }
    }
    public class AccreditedInstallerData
    {
        public List<Accredited_Solar_Installer> Accredited_Solar_Installer { get; set; }
    }
    public class Accredited_Solar_Installer
    {
        public  Details details { get; set; }
        public  Account account { get; set; }
    }
    public class Details
    {
        public string ID { get; set; }
        public string Contact_SNo { get; set; }
        public string SolarInstallerStatus { get; set; }
        public string SolarAccreditationNumber { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Concatenated_MailAddress { get; set; }
        public string MailingAddressUnitType { get; set; }
        public string MailingAddressUnitNumber { get; set; }
        public string MailingAddressStreetNumber { get; set; }
        public string MailingAddressStreetName { get; set; }
        public string MailingAddressStreetType { get; set; }
        public string MailingCity { get; set; }
        public string MailingStateProvince { get; set; }
        public string MailingZipPostalCode { get; set; }
        public string Mailing_Country { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string GridType { get; set; }
        public string SPS { get; set; }
        public string SolarInstallerFullAwardDate { get; set; }
        public string SolarInstallerProvisionalAwardDate { get; set; }
        public string Solar_InstallerExpiry_Date { get; set; }
        public string SuspensionStart_Date { get; set; }
        public string SuspensionEndDate { get; set; }
        public string LicensedElectricianNumber { get; set; }
        public string Endorsements { get; set; }

       
    }
   
}
