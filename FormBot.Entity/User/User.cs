using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FormBot.Helper;
using FormBot.Entity.Email;
using FormBot.Entity.SolarElectrician;

namespace FormBot.Entity
{
    [Serializable]
    public class User
    {
        public int hiddenTheme { get; set; }
        public int UserId { get; set; }

        [DisplayName("First Name:")]
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }
        public string PreviousFirstName { get; set; }
        [DisplayName("Last Name:")]
        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }
        public string PreviousLastName { get; set; }
        public string Name { get; set; }

        public string Address { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email.")]
        //// [RegularExpression(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-‌​]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$", ErrorMessage = "Email is not valid")]
        [Required(ErrorMessage = "Email is required.")]
        [DisplayName("Email:")]
        [NotMapped]
        public string Email { get; set; }
        public string PreviousEmail { get; set; }

        [DisplayName("Phone:")]
        [Required(ErrorMessage = "Phone is required.")]
        public string Phone { get; set; }
        public string PreviousPhone { get; set; }
        [DisplayName("Mobile:")]
        public string Mobile { get; set; }
        public string PreviousMobile { get; set; }
        [DisplayName("User Name:")]
        [NotMapped]
        [Required(ErrorMessage = "User Name is required.")]
        public string UserName { get; set; }
        public string PreviousUserName { get; set; }
        [DisplayName("Password:")]
        [NotMapped]
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }
        public string PreviousPassword { get; set; }
        public string PasswordHash { get; set; }

        [DisplayName("Company/Business Name:")]
        [Required(ErrorMessage = "Company/Business Name is required.")]
        public string CompanyName { get; set; }
        [DisplayName("Entity Name:")]
        public string EntityName { get; set; }
        public string PreviousCompanyName { get; set; }
        [DisplayName("Company ABN:")]
        [Required(ErrorMessage = "Company ABN is required.")]
        public string CompanyABN { get; set; }
        public string PreviousCompanyABN { get; set; }
        ////public string Address { get; set; }

        [DisplayName("Unit Type:")]
        ////[Required(ErrorMessage = "Unit Type is required.")]
        public int UnitTypeID { get; set; }
        public int PreviousUnitTypeID { get; set; }
        [DisplayName("Unit Number:")]
        ////[Required(ErrorMessage = "Unit Number is required.")]
        public string UnitNumber { get; set; }
        public string PreviousUnitNumber { get; set; }
        [DisplayName("Street Number:")]
        [Required(ErrorMessage = "Street Number is required.")]
        public string StreetNumber { get; set; }
        public string PreviousStreetNumber { get; set; }
        [DisplayName("Street Name:")]
        [Required(ErrorMessage = "Street Name is required.")]
        public string StreetName { get; set; }
        public string PreviousStreetName { get; set; }
        [DisplayName("Street Type:")]
        [Required(ErrorMessage = "Street Type is required.")]
        public int StreetTypeID { get; set; }
        public int PreviousStreetTypeID { get; set; }
        [DisplayName("Town/Suburb:")]
        [Required(ErrorMessage = "Town/Suburb is required.")]
        public string Town { get; set; }
        public string PreviousTown { get; set; }
        [DisplayName("State:")]
        [Required(ErrorMessage = "State is required.")]
        public string State { get; set; }
        public string PreviousState { get; set; }
        [DisplayName("Post Code:")]
        [Required(ErrorMessage = "PostCode is required.")]
        public string PostCode { get; set; }
        public string PreviousPostCode { get; set; }
        [DisplayName("Company Website:")]
        public string CompanyWebsite { get; set; }
        public string PreviousCompanyWebsite { get; set; }
        [DisplayName("BSB:")]
        [Required(ErrorMessage = "BSB is required.")]
        public string BSB { get; set; }
        public string PreviousBSB { get; set; }
        [DisplayName("Account Number:")]
        [Required(ErrorMessage = "Account Number is required.")]
        public string AccountNumber { get; set; }
        public string PreviousAccountNumber { get; set; }
        [DisplayName("Account Name:")]
        [Required(ErrorMessage = "Account Name is required.")]
        public string AccountName { get; set; }
        public string PreviousAccountName { get; set; }
        [DisplayName("Greenbot Account Manager:")]
        public string RAMId { get; set; }
        public string PreviousRAMId { get; set; }
        [DisplayName("Licensed Electrician Number:")]
        [RequiredIf("IsSolarElectrician", RequiredIfAttribute.Operator.EqualTo , true ,  ErrorMessage = "Licensed Electrician Number is required.")]
        public string ElectricalContractorsLicenseNumber { get; set; }
        public string PreviousElectricalContractorsLicenseNumber { get; set; }
        public bool IsSolarElectrician { get { return UserTypeID == 7; } }

        [DisplayName("CEC Accreditation Number:")]
        [RequiredIf("IsSolarElectrician", RequiredIfAttribute.Operator.EqualTo, true, ErrorMessage = "CEC Accreditation Number is required.")]
        ////[Required(ErrorMessage = "CEC Accreditation Number is required.")]
        public string CECAccreditationNumber { get; set; }
        public string PreviousCECAccreditationNumber { get; set; }
        [DisplayName("CEC Designer Number:")]
        public string CECDesignerNumber { get; set; }
        public string PreviousCECDesignerNumber { get; set; }
        [DisplayName("Signature:")]
        public string Signature { get; set; }
        public string PreviousSignature { get; set; }
        [DisplayName("Selfie:")]
        public string SESelfie { get; set; }
        public string PreviousSESelfie { get; set; }
        [DisplayName("User Type:")]
        public int UserTypeID { get; set; }

        [DisplayName("Is Active:")]
        public bool IsActive { get; set; }
        public bool PreviousIsActive { get; set; }
        [NotMapped]
        public string IsActiveStr { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }
        public DateTime CreatedpwdDate { get; set; }
        public DateTime ModifiedpwdDate { get; set; }

        public string strCreatedDate
        {
            get
            {
                return CreatedDate.ToString("dd/MM/yyyy");
            }
        }

        public string strModifiedDate
        {
            get
            {
                return ModifiedDate.ToString("dd/MM/yyyy");
            }
        }
        
        public bool IsDeleted { get; set; }

        [DisplayName("Solar Company/Sub Contractor Name:")]
        [Required(ErrorMessage = "Solar Company is required.")]
        public int? SolarCompanyId { get; set; }
        public int? PreviousSolarCompanyId { get; set; }
        [DisplayName("Sub Contractor")]
        public int? SolarSubContractorID { get; set; }

        public string SolarSubContractorName { get; set; }

        [NotMapped]
        [DisplayName("Solar Company")]
        public string SolarCompany { get; set; }

        [DisplayName("Reseller Name:")]
        [Required(ErrorMessage = "Reseller Name is required.")]
        public int? ResellerID { get; set; }
        public int? PreviousResellerID { get; set; }
        [NotMapped]
        public int TotalRecords { get; set; }

        [NotMapped]
        public string ResellerName { get; set; }

        public string AspNetUserId { get; set; }

        [DisplayName("Logo:")]
        public string Logo { get; set; }

        public int Theme { get; set; }

        [DisplayName("From Date:")]
        public DateTime? FromDate { get; set; }

        [NotMapped]
        public string strFromDate { get; set; }
        [NotMapped]
        public string strToDate { get; set; }

        [DisplayName("To Date:")]
        public DateTime? ToDate { get; set; }

        [NotMapped]
        public string UserTypeName { get; set; }

        [NotMapped]
        public string UserInfo { get; set; }

        [NotMapped]
        public int FileCount { get; set; }

        [DisplayName("Role:")]
        [Required(ErrorMessage = "Role is required.")]
        public int RoleID { get; set; }
        public int PreviousRoleID { get; set; }
        [NotMapped]
        [DisplayName("Login Company Name:")]
        [Required(ErrorMessage = "Login Company Name is required.")]
        public string LoginCompanyName { get; set; }
        public string PreviousLoginCompanyName { get; set; }
        [NotMapped]
        public string Created { get; set; }

        public DateTime? LastLogIn { get; set; }

        public DateTime? LastAccessFromApp { get; set; }

        public DateTime? LastAccessFromPortal { get; set; }

        public string strLastLogIn
        {
            get
            {
                return LastLogIn != null ? Convert.ToDateTime(LastLogIn).ToString("dd/MM/yyyy"):"";
            }
        }
        public string strLastAccessFromApp
        {
            get
            {
                return LastAccessFromApp != null ? Convert.ToDateTime(LastAccessFromApp).ToString("dd/MM/yyyy") : "";
            }
        }

        public string strLastAccessFromPortal
        {
            get
            {
                return LastAccessFromPortal != null ? Convert.ToDateTime(LastAccessFromPortal).ToString("dd/MM/yyyy") : "";
            }
        }

        [NotMapped]
        public string Role { get; set; }

        [DisplayName("Notes:")]
        public string Note { get; set; }
        public int NotesType { get; set; }
        public bool IsImportantNote { get; set; }
        [NotMapped]
        [DisplayName("FCO Group:")]
        public int FCOGroupId { get; set; }

        [DisplayName("Status:")]
        public byte? Status { get; set; }
        public byte? PreviousStatus { get; set; }
        [DisplayName("Verification Status:")]
        public int? IsVerified { get; set; }
        public int? PreviousIsVerified { get; set; }
        [DisplayName("Reason For Unverified Documents:")]
        public string Reason { get; set; }

        public bool IsFirstLogin { get; set; }

        [NotMapped]
        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.UserId));
            }
        }

        [NotMapped]
        [Required(ErrorMessage = "SE Role is required.")]
        public int SEDesignRoleId { get; set; }

        [NotMapped]
        public string UnitTypeName { get; set; }

        [NotMapped]
        public string StreetTypeName { get; set; }

        [NotMapped]
        public string FileName { get; set; }

        [NotMapped]
        public List<string> FileNamesCreate { get; set; }

        [NotMapped]
        public List<ProofFileNames> ProofFileNamesCreate { get; set; }
        [NotMapped]
        public List<ProofFileNames> ContractPathFile { get; set; }
        public string ContractPath { get; set; }
        [NotMapped]
        public List<UserDocument> lstUserDocument { get; set; }

        [NotMapped]
        public string ResellerUrl { get; set; }

        public bool IsPostalAddress { get; set; }

        [DisplayName("Postal Delivery Type:")]
        [Required(ErrorMessage = "Postal Delivery Type is required.")]
        public int PostalAddressID { get; set; }
        public int PreviousPostalAddressID { get; set; }
        [DisplayName("Postal Delivery Number:")]
        [Required(ErrorMessage = "Postal Delivery Number is required.")]
        public string PostalDeliveryNumber { get; set; }
        public string PreviousPostalDeliveryNumber { get; set; }
        public int AddressID { get; set; }
        public int PreviousAddressID { get; set; }
        [NotMapped]
        public List<FCOGroup1> lstFCOGroup1 { get; set; }

        [NotMapped]
        public List<string> FCOGroupSelected { get; set; }
        public int[] ArrFcoGroup { get; set; }

        [NotMapped]
        public string Code { get; set; }
        [NotMapped]
        public string PostalDeliveryType { get; set; }

        public string Guid { get; set; }

        public EmailSignup EmailSignup { get; set; }

        //Rec email changes
        public RecEmailSignup RecEmailSignup { get; set; }

        public bool IsSTC { get; set; }

        [NotMapped]
        public int SEDesigner { get; set; }
        public int PreviousSEDesigner { get; set; }
        [NotMapped]
        public int chkSTC { get; set; }

        [NotMapped]
        public bool isActiveDiv { get; set; }

        [NotMapped]
        public string DisplayDate { get; set; }

        [NotMapped]
        public bool IsSCDetailConfirm { get; set; }
        public bool PreviousIsSCDetailConfirm { get; set; }
        public bool IsSEDetailConfirm { get; set; }
        public bool PreviousIsSEDetailConfirm { get; set; }
        [NotMapped]
        public bool IsInstaller { get; set; }
        public bool PreviousIsInstaller { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string ApprovedDisplayDate { get; set; }
        public bool IsSubContractor { get; set; }
        public string PasswordLink { get; set; }
        public string AspNetUserIdSE { get; set; }

        public int ComplainBy { get; set; }
        public string StatusName { get; set; }
        public bool IsSSCReseller { get; set; }
        public string Fullname { get; set; }

        public string OwnerAddress { get; set; }
        public string JobName { get; set; }

        //public SolarElectricianView solarElectricianView { get; set; }

        public InstallerDesignerView installerDesignerView { get; set; }

        [DisplayName("Client Number:")]
        public string ClientNumber { get; set; }
        public string PreviousClientNumber { get; set; }
		[DisplayName("Unique Company Number:")]
		public string UniqueCompanyNumber { get; set; }
        public string PreviousUniqueCompanyNumber { get; set; }
		[DisplayName("Client Code Prefix:")]
        [Required(ErrorMessage = "Client Code Prefix is required.")]
        public string ClientCodePrefix { get; set; }
        public string PreviousClientCodePrefix { get; set; }
        public string RAMName { get; set; }

        public string ToEmailAddress { get; set; }

        [DisplayName("GST:")]
        public int IsGSTSetByAdminUser { get; set; }
        public int PreviousIsGSTSetByAdminUser { get; set; }
        [DisplayName("Goods & Services Tax (GST):")]
        public string GSTText { get; set; }

        public int SolarCompanyStatusBySE { get; set; } //  Accept/reject by solar electrician

        public int SolarElectricianUserId { get; set; }

        public bool IsAllowTrade { get; set; }

        public int UserJobType { get; set; }

        [DisplayName("SWH License Number:")]
        //[Required(ErrorMessage = "SWH License Number is required.")]
        public string SWHLicenseNumber { get; set; }

        [DisplayName("Is WholeSaler:")]
        public bool IsWholeSaler { get; set; }
        public bool PreviousIsWholeSaler { get; set; }
        [DisplayName("Solar Company Code:")]
        public string GB_SCACode { get; set; }
        public string PreviousGB_SCACode { get; set; }

        [DisplayName("Reseller Code:")]
        public string GB_RACode { get; set; }
        public string PreviousGB_RACode { get; set; }
        public int AccreditedInstallerId { get; set; }

        public string DeletedName { get; set; }

        //public Nullable<bool> IsPVDUser
        //{
        //    get
        //    {
        //        if (!IsPVDUser.HasValue) // Call CompanyActive get again to get the property value : infinite loop
        //        {
        //            return IsPVDUser;
        //        }
        //        else
        //        {
        //            return this.IsPVDUser;
        //        } // Call CompanyActive get again to get the property value : infinite loop
        //    }
        //    set { this.IsPVDUser = value; } //Call CompanyActive set again to set the property value : infinite loop
        //}
        //public Nullable<bool> IsSWHUser
        //{
        //    get
        //    {
        //        if (!IsSWHUser.HasValue) // Call CompanyActive get again to get the property value : infinite loop
        //        {
        //            return IsSWHUser;
        //        }
        //        else
        //        {
        //            return this.IsSWHUser;
        //        } // Call CompanyActive get again to get the property value : infinite loop
        //    }
        //    set { this.IsSWHUser = value; } //Call CompanyActive set again to set the property value : infinite loop
        //}

        public bool? IsPVDUser { get; set; }
        public bool? PreviousIsPVDUser { get; set; }
        public bool? IsSWHUser { get; set; }
        public bool? PreviousIsSWHUser { get; set; }
        public bool? IsVEECUser { get; set; }
        public bool IsAutoRequest { get; set; }
        public bool PreviousIsAutoRequest { get; set; }
        [Required(ErrorMessage = "Custom company name is required")]
        [DisplayName("Custom CompanyName : ")]
        public string CustomCompanyName { get; set; }
        public string PreviousCustomCompanyName { get; set; }
		public bool IsNewViewer { get; set; }

        public bool IsTabularView { get; set; }

        [DisplayName("Wholesaler First Name:")]
        [Required(ErrorMessage = "Wholesaler first name is required.")]
        public string WholesalerFirstName { get; set; }
        public string PreviousWholesalerFirstName { get; set; }
        [DisplayName("Wholesaler Last Name:")]
        [Required(ErrorMessage = "Wholesaler last Name is required.")]
        public string WholesalerLastName { get; set; }
        public string PreviousWholesalerLastName { get; set; }
        [DisplayName("Wholesaler Email:")]
        [EmailAddress(ErrorMessage = "Invalid Email.")]
        //[Required(ErrorMessage = "Wholesaler Email is required.")]
        [NotMapped]
        public string WholesalerEmail { get; set; }
        public string PreviousWholesalerEmail { get; set; }
        [DisplayName("Wholesaler Phone:")]
        //[Required(ErrorMessage = "Wholesaler Phone is required.")]
        public string WholesalerPhone { get; set; }
        public string PreviousWholesalerPhone { get; set; }

        [DisplayName("Wholesaler Mobile:")]
        public string WholesalerMobile { get; set; }
        public string PreviousWholesalerMobile { get; set; }
        [DisplayName("Wholesaler Company ABN:")]
        [Required(ErrorMessage = "Wholesaler ABN is required.")]
        public string WholesalerCompanyABN { get; set; }
        public string PreviousWholesalerCompanyABN { get; set; }
        [DisplayName("Wholesaler company Name:")]
        [Required(ErrorMessage = "Wholesaler company name is required.")]
        public string WholesalerCompanyName { get; set; }
        public string PreviousWholesalerCompanyname { get; set; }
        [DisplayName("Wholesaler Postal Address Type:")]
        [Required(ErrorMessage = "Wholesaler Postal Address Type is required.")]
        public int WholesalerIsPostalAddress { get; set; }
        public int PreviousWholesalerIsPostalAddress { get; set; }
        [DisplayName("Wholesaler Unit Type:")]
        ////[Required(ErrorMessage = "Unit Type is required.")]
        public int WholesalerUnitTypeID { get; set; }
        public int PreviousWholesalerUnitTypeID { get; set; }
        [DisplayName("Wholesaler Unit Number:")]
        ////[Required(ErrorMessage = "Unit Number is required.")]
        public string WholesalerUnitNumber { get; set; }
        public string PreviousWholesalerUnitNumber { get; set; }
        [DisplayName("Wholesaler Street Number:")]
        [Required(ErrorMessage = "Wholesaler Street Number is required.")]
        public string WholesalerStreetNumber { get; set; }
        public string PreviousWholesalerStreetNumber { get; set; }
        [DisplayName("Wholesaler Street Name:")]
        [Required(ErrorMessage = "Wholesaler Street Name is required.")]
        public string WholesalerStreetName { get; set; }
        public string PreviousWholesalerStreetName { get; set; }
        [DisplayName("Wholesaler Street Type:")]
        [Required(ErrorMessage = "Wholesaler Street Type is required.")]
        public int WholesalerStreetTypeID { get; set; }
        public int PreviousWholesalerStreetTypeID { get; set; }
        [DisplayName("Wholesaler Postal Delivery Type:")]
        [Required(ErrorMessage = "Wholesaler Postal Delivery Type is required.")]
        public int WholesalerPostalAddressID { get; set; }
        public int PreviousWholesalerPostalAddressID { get; set; }
        [DisplayName("Wholesaler Postal Delivery Number:")]
        [Required(ErrorMessage = "Wholesaler Postal Delivery Number is required.")]
        public string WholesalerPostalDeliveryNumber { get; set; }
        public string PreviousWholesalerPostalDeliveryNumber { get; set; }
        [DisplayName("Wholesaler Town/Suburb:")]
        [Required(ErrorMessage = "Wholesaler Town/Suburb is required.")]
        public string WholesalerTown { get; set; }
        public string PreviousWholesalerTown { get; set; }
        [DisplayName("Wholesaler State:")]
        [Required(ErrorMessage = "Wholesaler State is required.")]
        public string WholesalerState { get; set; }
        public string PreviousWholesalerState { get; set; }
        [DisplayName("Wholesaler Post Code:")]
        [Required(ErrorMessage = "Wholesaler PostCode is required.")]
        public string WholesalerPostCode { get; set; }
        public string PreviousWholesalerPostCode { get; set; }
        [DisplayName("Wholesaler BSB:")]
        //[Required(ErrorMessage = "Wholesaler BSB is required.")]
        public string WholesalerBSB { get; set; }
        public string PreviousWholesalerBSB { get; set; }
        [DisplayName("Wholesaler Account Number:")]
        //[Required(ErrorMessage = "Wholesaler Account Number is required.")]
        public string WholesalerAccountNumber { get; set; }
        public string PreviousWholesalerAccountNumber { get; set; }
        [DisplayName("Wholesaler Account Name:")]
        //[Required(ErrorMessage = "Wholesaler Account Name is required.")]
        public string WholesalerAccountName { get; set; }
        public string PreviousWholesalerAccountName { get; set; }
        [NotMapped]
        public string WholesalerUnitTypeName { get; set; }
        [NotMapped]
        public string WholesalerStreetTypeName { get; set; }

        [NotMapped]
        public string WholesalerCode { get; set; }
        [NotMapped]
        public string WholesalerPostalDeliveryType { get; set; }

        [DisplayName("Unique Wholesaler Number:")]
        public string UniqueWholesalerNumber { get; set; }
        public string PreviousUniqueWholesalerNumber { get; set; }
        public List<UserDevice> userDevice { get; set; }

        public bool IsResetPwd { get; set; }
        public bool SCisAllowedSPV { get; set; }
        public bool PreviousSCisAllowedSPV { get; set; }
        public string Flag { get; set; }

        public bool isAllowedMasking { get; set; }
        public bool PreviousIsAllowedMasking { get; set; }
        [DisplayName("Default REC Credential")]
        public bool UseCredentialFrom { get; set; }

        [DisplayName("REC Accounts")]
        public string RECAccount { get; set; }

        ////[Required(ErrorMessage = "Rec User Name is required.")]
        public string RecUserName { get; set; }

        [DisplayName("Rec Password:")]
        ////[Required(ErrorMessage = "Rec Password is required.")]
        public string RecPassword { get; set; }

        public string LoginType { get; set; }

        public string SuperAdminCERLoginId { get; set; }
        public string hdnSuperAdminCERLoginId { get; set; }

        public string SuperAdminCERPassword { get; set; }

        public string RecSuperAdminUserName { get; set; }

        public string SuperAdminRECName { get; set; }

        public string SuperAdminRECCompName { get; set; }

        public string CERLoginId { get; set; }
        public string hdnCERLoginId { get; set; }

        public string CERPassword { get; set; }

        public string RecCompUserName { get; set; }

        public string RECName { get; set; }

        public string RECCompName { get; set; }

        public List<RECAccount> lstRECAccounts { get; set; }

        [DisplayName("Usage Type:")]
        public int UsageType { get; set; }

        [NotMapped]
        public List<Reseller> lstInvoicer { get; set; }

        [DisplayName("Invoicer:")]
        public int? Invoicer { get; set; }
        public decimal? InvoicerId { get; set; }
        public string InvoicerName { get; set; }
        [NotMapped]
        [DisplayName("Invoicer First Name:")]
        public string InvoicerFirstName { get; set; }

        [DisplayName("Invoicer Last Name:")]
        public string InvoicerLastName { get; set; }

        [DisplayName("Invoicer Phone:")]
        public string InvoicerPhone { get; set; }
        [DisplayName("Unique ContactID")]
        public string UniqueContactId { get; set; }
        [DisplayName("Invoicer Postal Address Type:")]
        [Required(ErrorMessage = "Invoicer Postal Address Type is required.")]
        public int InvoicerAddressID { get; set; }
        public bool InvoicerIsPostalAddress { get; set; }
        [DisplayName("Invoicer Unit Type:")]
        ////[Required(ErrorMessage = "Unit Type is required.")]
        public int InvoicerUnitTypeID { get; set; }

        [DisplayName("Invoicer Unit Number:")]
        ////[Required(ErrorMessage = "Unit Number is required.")]
        public string InvoicerUnitNumber { get; set; }

        [DisplayName("Invoicer Street Number:")]
        [Required(ErrorMessage = "Invoicer Street Number is required.")]
        public string InvoicerStreetNumber { get; set; }

        [DisplayName("Invoicer Street Name:")]
        [Required(ErrorMessage = "Invoicer Street Name is required.")]
        public string InvoicerStreetName { get; set; }

        [DisplayName("Invoicer Street Type:")]
        [Required(ErrorMessage = "Invoicer Street Type is required.")]
        public int InvoicerStreetTypeID { get; set; }

        [DisplayName("Invoicer Postal Delivery Type:")]
        [Required(ErrorMessage = "Invoicer Postal Delivery Type is required.")]
        public int InvoicerPostalAddressID { get; set; }

        [DisplayName("Invoicer Postal Delivery Number:")]
        [Required(ErrorMessage = "Invoicer Postal Delivery Number is required.")]
        public string InvoicerPostalDeliveryNumber { get; set; }

        [DisplayName("Invoicer Town/Suburb:")]
        [Required(ErrorMessage = "Invoicer Town/Suburb is required.")]
        public string InvoicerTown { get; set; }

        [DisplayName("Invoicer State:")]
        [Required(ErrorMessage = "Invoicer State is required.")]
        public string InvoicerState { get; set; }

        [DisplayName("Invoicer Post Code:")]
        [Required(ErrorMessage = "Invoicer PostCode is required.")]
        public string InvoicerPostCode { get; set; }

        [DisplayName("Invoicer Company ABN:")]
        public string InvoicerCompanyABN { get; set; }
[DisplayName("Select Verified Documents:")]
        public bool SelectAll { get; set; }
        public bool IsSignatureVerified { get; set; }
        public bool IsSelfieVerified { get; set; }
        public bool IsDriverLicVerified { get; set; }
        public bool IsOtherDocVerified { get; set; }
        public int VerifiedBy { get; set; }
        public DateTime LastVerifiedOn { get; set; }
        public bool IsSAASUser { get; set; }
        [DisplayName("Account Code:")]
        public string AccountCode { get; set; }
    }

    public class Title
    {
        public long TitleId { get; set; }
        public string TitleName { get; set; }
        public int Year { get; set; }
        public long ProviderId { get; set; }
    }

    public class UserType
    {
        public int UserTypeID { get; set; }
        public string UserTypeName { get; set; }
        [NotMapped]
        public string IsValid { get; set; }
    }

    public class UserXeroContact
    {
        public User UserView { get; set; }
        public XeroContact XeroContact { get; set; }
    }

    public class UserVendor
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UserTypeID { get; set; }
        public int ResellerID { get; set; }
        public int SolarCompanyId { get; set; }
    }

    public class SpResponce_CheckAccreditationNumber
	{
		public bool IsSameAccreditationNumber { get; set; }
		public bool IsSameLicenseNumber { get; set; }
		public int UserId { get; set; }
	}

    public class ProofFileNames
    {
        public string FileName { get; set; }

        public int ProofDocumentType { get; set; }

        public int DocLoc { get; set; }

        public string DocLocStr { get; set; }
    }

    [Serializable]
    public class UserDevice
    {
        public int UserDeviceID { get; set; }

        public string Type { get; set; }

        public string DeviceInfo { get; set; }
    }
 public class UpdateContactXeroLog
    {
        public int UserId { get; set; }
        public int CreatedBy { get; set; }
        public string HistoryMessage { get; set; }
    }

    public class RECAccount
    {
        public string RECName { get; set; }
        public string RECCompName { get; set; }
        public string RECAccName { get; set; }
        public string RECUserName { get; set; }
        public string CERLoginId { get; set; }
        public string CERPassword { get; set; }
        public bool IsDefault { get; set; }
        public string RECLoginType { get; set; }
        public int UserId { get; set; }
        [NotMapped]
        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.UserId));
            }
        }
    }
}
