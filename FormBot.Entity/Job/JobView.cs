using FormBot.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class JobView
    {

        [JsonProperty("1", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? CalculatedSTC { get; set; }
        [JsonProperty("2", NullValueHandling = NullValueHandling.Ignore)]
        public string ClientName { get; set; }
        [JsonProperty("3", NullValueHandling = NullValueHandling.Ignore)]
        public string ColorCode { get; set; }
        [JsonProperty("4", NullValueHandling = NullValueHandling.Ignore)]
        public int CreatedBy { get; set; }
        [JsonProperty("5", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime CreatedDate { get; set; }
        [JsonProperty("6", NullValueHandling = NullValueHandling.Ignore)]
        public string DeemingPeriod { get; set; }
        [JsonIgnore]
        public string DesignerAccreditationNumber { get; set; }
        [JsonIgnore]
        public string DesignerFirstName { get; set; }
        [JsonIgnore]
        public string DesignerFullAddress { get; set; }
        [JsonIgnore]
        public string DesignerFullName { get; set; }
        [JsonIgnore]
        public string DesignerLastName { get; set; }
        [JsonIgnore]
        public string DesignerLicenseNumber { get; set; }
        [JsonIgnore]
        public string DesignerPostCode { get; set; }
        [JsonIgnore]
        public string DesignerState { get; set; }
        [JsonIgnore]
        public string DesignerStreetName { get; set; }
        [JsonIgnore]
        public string DesignerStreetNumber { get; set; }
        [JsonIgnore]
        public string DesignerStreetTypeID { get; set; }
        [JsonIgnore]
        public string DesignerTown { get; set; }
        [JsonIgnore]
        public string DesignerUnitNumber { get; set; }
        [JsonIgnore]
        public string DesignerUnitTypeID { get; set; }
        [JsonProperty("7", NullValueHandling = NullValueHandling.Ignore)]
        public string Distributor { get; set; }
        [JsonProperty("8", NullValueHandling = NullValueHandling.Ignore)]
        public int? DocumentCount { get; set; }
        [JsonProperty("9", NullValueHandling = NullValueHandling.Ignore)]
        public string ElectricianCompanyName { get; set; }
        [JsonProperty("10", NullValueHandling = NullValueHandling.Ignore)]
        public string ElectricianFirstName { get; set; }
        [JsonProperty("11", NullValueHandling = NullValueHandling.Ignore)]
        public string ElectricianFullAddress { get; set; }
        [JsonProperty("12", NullValueHandling = NullValueHandling.Ignore)]
        public string ElectricianFullName { get; set; }
        [JsonProperty("13", NullValueHandling = NullValueHandling.Ignore)]
        public string ElectricianLastName { get; set; }
        [JsonProperty("14", NullValueHandling = NullValueHandling.Ignore)]
        public string ElectricianLicenseNumber { get; set; }
        [JsonProperty("15", NullValueHandling = NullValueHandling.Ignore)]
        public string ElectricianPostCode { get; set; }
        [JsonProperty("16", NullValueHandling = NullValueHandling.Ignore)]
        public string ElectricianState { get; set; }
        [JsonProperty("17", NullValueHandling = NullValueHandling.Ignore)]
        public string ElectricianStreetName { get; set; }
        [JsonProperty("18", NullValueHandling = NullValueHandling.Ignore)]
        public string ElectricianStreetNumber { get; set; }
        [JsonProperty("19", NullValueHandling = NullValueHandling.Ignore)]
        public string ElectricianStreetTypeID { get; set; }
        [JsonProperty("20", NullValueHandling = NullValueHandling.Ignore)]
        public string ElectricianTown { get; set; }
        [JsonProperty("21", NullValueHandling = NullValueHandling.Ignore)]
        public string ElectricianUnitNumber { get; set; }
        [JsonProperty("22", NullValueHandling = NullValueHandling.Ignore)]
        public string ElectricianUnitTypeID { get; set; }
        [JsonProperty("23", NullValueHandling = NullValueHandling.Ignore)]
        public string ElectricityProvider { get; set; }
        [JsonProperty("24", NullValueHandling = NullValueHandling.Ignore)]
        public string FullBasicDetails { get; set; }
        [JsonProperty("25", NullValueHandling = NullValueHandling.Ignore)]
        public string FullInstallationAddress { get; set; }
        [JsonProperty("26", NullValueHandling = NullValueHandling.Ignore)]
        public string FullOwnerAddress { get; set; }
        [JsonProperty("27", NullValueHandling = NullValueHandling.Ignore)]
        public string FullOwnerCompanyDetails { get; set; }
        [JsonProperty("28", NullValueHandling = NullValueHandling.Ignore)]
        public string FullOwnerName { get; set; }
        [JsonProperty("29", NullValueHandling = NullValueHandling.Ignore)]
        public string FullPanelDetails { get; set; }
        [JsonProperty("30", NullValueHandling = NullValueHandling.Ignore)]
        public string GSTDocument { get; set; }
        [JsonProperty("31", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? InstallationDate { get; set; }
        [JsonProperty("32", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallationPostCode { get; set; }
        [JsonProperty("33", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallationState { get; set; }
        [JsonProperty("34", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallationStreetName { get; set; }
        [JsonProperty("35", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallationStreetNumber { get; set; }
        [JsonProperty("36", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallationStreetTypeID { get; set; }
        [JsonProperty("37", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallationTown { get; set; }
        [JsonProperty("38", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallationType { get; set; }
        [JsonProperty("39", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallationUnitNumber { get; set; }
        [JsonProperty("40", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallationUnitTypeID { get; set; }
        [JsonIgnore]
        public string InstallerAccreditationNumber { get; set; }
        [JsonIgnore]
        public string InstallerFirstName { get; set; }
        [JsonIgnore]
        public string InstallerFullAddress { get; set; }
        [JsonIgnore]
        public string InstallerFullName { get; set; }
        [JsonIgnore]
        public string InstallerLastName { get; set; }
        [JsonIgnore]
        public string InstallerLicenseNumber { get; set; }
        [JsonIgnore]
        public string InstallerPostCode { get; set; }
        [JsonIgnore]
        public string InstallerState { get; set; }
        [JsonIgnore]
        public string InstallerStreetName { get; set; }
        [JsonIgnore]
        public string InstallerStreetNumber { get; set; }
        [JsonIgnore]
        public string InstallerStreetTypeID { get; set; }
        [JsonIgnore]
        public string InstallerTown { get; set; }
        [JsonIgnore]
        public string InstallerUnitNumber { get; set; }
        [JsonIgnore]
        public string InstallerUnitTypeID { get; set; }
        [JsonProperty("41", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsAccept { get; set; }
        [JsonProperty("42", NullValueHandling = NullValueHandling.Ignore)]
        public int IsBasicValidation { get; set; }
        [JsonProperty("43", NullValueHandling = NullValueHandling.Ignore)]
        public int IsCESForm { get; set; }
        [JsonProperty("44", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsConnectionCompleted { get; set; }
        [JsonProperty("45", NullValueHandling = NullValueHandling.Ignore)]
        public int IsCustPrice { get; set; }
        [JsonProperty("46", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsDeleted { get; set; }
        [JsonProperty("47", NullValueHandling = NullValueHandling.Ignore)]
        public int IsGroupSiganture { get; set; }
        [JsonProperty("48", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsGst { get; set; }
        [JsonProperty("49", NullValueHandling = NullValueHandling.Ignore)]
        public int IsInstallerSiganture { get; set; }
        [JsonProperty("50", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsInvoiced { get; set; }
        [JsonProperty("51", NullValueHandling = NullValueHandling.Ignore)]
        public int IsOwnerSiganture { get; set; }
        [JsonProperty("52", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsPreApprovaApproved { get; set; }
        [JsonProperty("53", NullValueHandling = NullValueHandling.Ignore)]
        public int IsSerialNumberCheck { get; set; }
        [JsonProperty("54", NullValueHandling = NullValueHandling.Ignore)]
        public int IsSTCForm { get; set; }
        [JsonProperty("55", NullValueHandling = NullValueHandling.Ignore)]
        public int IsSTCSubmissionPhotos { get; set; }
        [JsonProperty("56", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsTraded { get; set; }
        [JsonProperty("57", NullValueHandling = NullValueHandling.Ignore)]
        public string JobAddress { get; set; }
        [JsonProperty("58", NullValueHandling = NullValueHandling.Ignore)]
        public string JobDescription { get; set; }
        [JsonProperty("59", NullValueHandling = NullValueHandling.Ignore)]
        public int JobID { get; set; }
        [JsonProperty("60", NullValueHandling = NullValueHandling.Ignore)]
        public string JobNumber { get; set; }
        [JsonProperty("61", NullValueHandling = NullValueHandling.Ignore)]
        public string JobStage { get; set; }
        [JsonProperty("62", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? JobStageChangeDate { get; set; }
        [JsonProperty("63", NullValueHandling = NullValueHandling.Ignore)]
        public string JobTitle { get; set; }
        [JsonProperty("64", NullValueHandling = NullValueHandling.Ignore)]
        public int JobTypeId { get; set; }
        [JsonProperty("65", NullValueHandling = NullValueHandling.Ignore)]
        public string MeterNumber { get; set; }
        [JsonProperty("66", NullValueHandling = NullValueHandling.Ignore)]
        public string NMI { get; set; }
        [JsonProperty("67", NullValueHandling = NullValueHandling.Ignore)]
        public int? NoOfPanel { get; set; }
        [JsonProperty("68", NullValueHandling = NullValueHandling.Ignore)]
        public string OwnerEmail { get; set; }
        [JsonProperty("69", NullValueHandling = NullValueHandling.Ignore)]
        public string OwnerMobile { get; set; }
        [JsonProperty("70", NullValueHandling = NullValueHandling.Ignore)]
        public string OwnerPhone { get; set; }
        [JsonProperty("71", NullValueHandling = NullValueHandling.Ignore)]
        public string OwnerPostCode { get; set; }
        [JsonProperty("72", NullValueHandling = NullValueHandling.Ignore)]
        public string OwnerState { get; set; }
        [JsonProperty("73", NullValueHandling = NullValueHandling.Ignore)]
        public string OwnerStreetName { get; set; }
        [JsonProperty("74", NullValueHandling = NullValueHandling.Ignore)]
        public string OwnerStreetNumber { get; set; }
        [JsonProperty("75", NullValueHandling = NullValueHandling.Ignore)]
        public string OwnerStreetTypeID { get; set; }
        [JsonProperty("76", NullValueHandling = NullValueHandling.Ignore)]
        public string OwnerTown { get; set; }
        [JsonProperty("77", NullValueHandling = NullValueHandling.Ignore)]
        public string OwnerType { get; set; }
        [JsonProperty("78", NullValueHandling = NullValueHandling.Ignore)]
        public string OwnerUnitNumber { get; set; }
        [JsonProperty("79", NullValueHandling = NullValueHandling.Ignore)]
        public string OwnerUnitTypeID { get; set; }
        [JsonProperty("80", NullValueHandling = NullValueHandling.Ignore)]
        public string PhaseProperty { get; set; }
        [JsonProperty("81", NullValueHandling = NullValueHandling.Ignore)]
        public int? Priority { get; set; }
        [JsonProperty("82", NullValueHandling = NullValueHandling.Ignore)]
        public string PropertyType { get; set; }
        [JsonProperty("83", NullValueHandling = NullValueHandling.Ignore)]
        public string RefNumber { get; set; }
        [JsonProperty("84", NullValueHandling = NullValueHandling.Ignore)]
        public string SCOName { get; set; }
        [JsonProperty("85", NullValueHandling = NullValueHandling.Ignore)]
        public int? ScoUserId { get; set; }
        [JsonProperty("86", NullValueHandling = NullValueHandling.Ignore)]
        public string SerialNumbers { get; set; }
        [JsonIgnore]
        public string SolarCompany { get; set; }
        [JsonIgnore]
        public string SolarCompanyABN { get; set; }
        [JsonIgnore]
        public string SolarCompanyAccreditationNumber { get; set; }
        [JsonIgnore]
        public string SolarCompanyFirstName { get; set; }
        [JsonIgnore]
        public string SolarCompanyFullAddress { get; set; }
        [JsonIgnore]
        public string SolarCompanyFullName { get; set; }
        [JsonProperty("87", NullValueHandling = NullValueHandling.Ignore)]
        public int? SolarCompanyId { get; set; }
        [JsonIgnore]
        public string SolarCompanyLastName { get; set; }
        [JsonIgnore]
        public string SolarCompanyName { get; set; }
        [JsonIgnore]
        public string SolarCompanyPostCode { get; set; }
        [JsonIgnore]
        public string SolarCompanyState { get; set; }
        [JsonIgnore]
        public string SolarCompanyStreetName { get; set; }
        [JsonIgnore]
        public string SolarCompanyStreetNumber { get; set; }
        [JsonIgnore]
        public string SolarCompanyStreetTypeID { get; set; }
        [JsonIgnore]
        public string SolarCompanyTown { get; set; }
        [JsonIgnore]
        public string SolarCompanyUnitNumber { get; set; }
        [JsonIgnore]
        public string SolarCompanyUnitTypeID { get; set; }
        [JsonProperty("88", NullValueHandling = NullValueHandling.Ignore)]
        public int? SSCID { get; set; }
        [JsonProperty("89", NullValueHandling = NullValueHandling.Ignore)]
        public string StaffName { get; set; }
        [JsonProperty("90", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? STC { get; set; }
        [JsonProperty("91", NullValueHandling = NullValueHandling.Ignore)]
        public string SystemMountingType { get; set; }
        [JsonProperty("92", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? SystemSize { get; set; }
        [JsonProperty("93", NullValueHandling = NullValueHandling.Ignore)]
        public int TradeStatus { get; set; }
        [JsonProperty("94", NullValueHandling = NullValueHandling.Ignore)]
        public string TypeOfConnection { get; set; }
        [JsonProperty("95", NullValueHandling = NullValueHandling.Ignore)]
        public int? InstallerID { get; set; }
        [JsonProperty("96", NullValueHandling = NullValueHandling.Ignore)]
        public int? DesignerID { get; set; }
        [JsonProperty("97", NullValueHandling = NullValueHandling.Ignore)]
        public string PanelBrand { get; set; }
        [JsonProperty("98", NullValueHandling = NullValueHandling.Ignore)]
        public string PanelModel { get; set; }
        [JsonProperty("99", NullValueHandling = NullValueHandling.Ignore)]
        public string InverterBrand { get; set; }
        [JsonProperty("100", NullValueHandling = NullValueHandling.Ignore)]
        public string InverterSeries { get; set; }
        [JsonProperty("101", NullValueHandling = NullValueHandling.Ignore)]
        public string InverterModel { get; set; }
        [JsonProperty("102", NullValueHandling = NullValueHandling.Ignore)]
        public int Totalcount { get; set; }
        [JsonProperty("103", NullValueHandling = NullValueHandling.Ignore)]
        public string STCStatus { get; set; }


        [JsonIgnore]
        public string Id { get { return QueryString.QueryStringEncode("id=" + Convert.ToString(JobID)); } }

        [JsonIgnore]
        public string strInstallationDate
        {
            get
            {
                return InstallationDate.HasValue ? Convert.ToDateTime(InstallationDate.Value).ToString("dd/MM/yyyy") : null;
            }
        }

        [JsonIgnore]
        public string strCreatedDate
        {
            get
            {
                return CreatedDate != null ? Convert.ToDateTime(CreatedDate).ToString("dd/MM/yyyy") : null;
            }
        }

        [JsonIgnore]
        public string strSignatureDate
        {
            get
            {
                return null;
            }
        }

        [JsonIgnore]
        public string strInstallerSignatureDate
        {
            get
            {
                return null;
            }
        }

        [JsonIgnore]
        public string strDesignerSignatureDate
        {
            get
            {
                return null;
            }
        }

        [JsonIgnore]
        public string strElectricianSignatureDate
        {
            get
            {
                return null;
            }
        }

        [JsonIgnore]
        public bool Urgent
        {
            get
            {
                if (JobStageChangeDate.HasValue)
                {
                    if ((System.DateTime.Now - JobStageChangeDate.Value).TotalDays >= Convert.ToInt32(ConfigurationManager.AppSettings["UrgentJobDay"].ToString()))
                        return true;
                }
                return false;
            }
        }
    }

    public class JobsInstallerDesignerView
    {
        [JsonProperty("1", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallerDesignerAccreditationNumber { get; set; }
        [JsonProperty("2", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallerDesignerFirstName { get; set; }
        [JsonProperty("3", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallerDesignerFullAddress { get; set; }
        [JsonProperty("4", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallerDesignerFullName { get; set; }
        [JsonProperty("5", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallerDesignerLastName { get; set; }
        [JsonProperty("6", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallerDesignerLicenseNumber { get; set; }
        [JsonProperty("7", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallerDesignerPostCode { get; set; }
        [JsonProperty("8", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallerDesignerState { get; set; }
        [JsonProperty("9", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallerDesignerStreetName { get; set; }
        [JsonProperty("10", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallerDesignerStreetNumber { get; set; }
        [JsonProperty("11", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallerDesignerStreetTypeID { get; set; }
        [JsonProperty("12", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallerDesignerTown { get; set; }
        [JsonProperty("13", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallerDesignerUnitNumber { get; set; }
        [JsonProperty("14", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallerDesignerUnitTypeID { get; set; }
        [JsonProperty("15", NullValueHandling = NullValueHandling.Ignore)]
        public int InstallerDesignerID { get; set; }
    }
    public class JobsSolarCompanyView
    {
        [JsonProperty("1", NullValueHandling = NullValueHandling.Ignore)]
        public string SolarCompany { get; set; }
        [JsonProperty("2", NullValueHandling = NullValueHandling.Ignore)]
        public string SolarCompanyABN { get; set; }
        [JsonProperty("3", NullValueHandling = NullValueHandling.Ignore)]
        public string SolarCompanyAccreditationNumber { get; set; }
        [JsonProperty("4", NullValueHandling = NullValueHandling.Ignore)]
        public string SolarCompanyFirstName { get; set; }
        [JsonProperty("5", NullValueHandling = NullValueHandling.Ignore)]
        public string SolarCompanyFullAddress { get; set; }
        [JsonIgnore]
        public string SolarCompanyFullName { get { return SolarCompany; } }
        [JsonProperty("7", NullValueHandling = NullValueHandling.Ignore)]
        public int SolarCompanyId { get; set; }
        [JsonProperty("8", NullValueHandling = NullValueHandling.Ignore)]
        public string SolarCompanyLastName { get; set; }
        [JsonIgnore]
        public string SolarCompanyName { get { return SolarCompany; } }
        [JsonProperty("10", NullValueHandling = NullValueHandling.Ignore)]
        public string SolarCompanyPostCode { get; set; }
        [JsonProperty("11", NullValueHandling = NullValueHandling.Ignore)]
        public string SolarCompanyState { get; set; }
        [JsonProperty("12", NullValueHandling = NullValueHandling.Ignore)]
        public string SolarCompanyStreetName { get; set; }
        [JsonProperty("13", NullValueHandling = NullValueHandling.Ignore)]
        public string SolarCompanyStreetNumber { get; set; }
        [JsonProperty("14", NullValueHandling = NullValueHandling.Ignore)]
        public string SolarCompanyStreetTypeID { get; set; }
        [JsonProperty("15", NullValueHandling = NullValueHandling.Ignore)]
        public string SolarCompanyTown { get; set; }
        [JsonProperty("16", NullValueHandling = NullValueHandling.Ignore)]
        public string SolarCompanyUnitNumber { get; set; }
        [JsonProperty("17", NullValueHandling = NullValueHandling.Ignore)]
        public string SolarCompanyUnitTypeID { get; set; }
    }

    public class STCSubmissionView
    {
        [JsonProperty("1", NullValueHandling = NullValueHandling.Ignore)]
        public string AccountManager { get; set; }
        [JsonProperty("2", NullValueHandling = NullValueHandling.Ignore)]
        public string ColorCode { get; set; }
        [JsonProperty("3", NullValueHandling = NullValueHandling.Ignore)]
        public int? ComplianceBy { get; set; }
        [JsonProperty("4", NullValueHandling = NullValueHandling.Ignore)]
        public string ComplianceNotes { get; set; }
        [JsonProperty("5", NullValueHandling = NullValueHandling.Ignore)]
        public string ComplianceOfficer { get; set; }
        [JsonProperty("6", NullValueHandling = NullValueHandling.Ignore)]
        public int CreatedBy { get; set; }
        [JsonProperty("7", NullValueHandling = NullValueHandling.Ignore)]
        public int? CustomSettlementTerm { get; set; }
        [JsonProperty("8", NullValueHandling = NullValueHandling.Ignore)]
        public string GBBatchRECUploadId { get; set; }
        [JsonProperty("9", NullValueHandling = NullValueHandling.Ignore)]
        public bool? HasMultipleRecords { get; set; }
        [JsonProperty("10", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallationAddress { get; set; }
        [JsonProperty("11", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? InstallationDate { get; set; }
        [JsonProperty("12", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallationState { get; set; }
        [JsonProperty("13", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallationTown { get; set; }
        [JsonProperty("14", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallerName { get; set; }
        [JsonProperty("15", NullValueHandling = NullValueHandling.Ignore)]
        public string InverterBrand { get; set; }
        [JsonProperty("16", NullValueHandling = NullValueHandling.Ignore)]
        public string InverterModel { get; set; }
        [JsonProperty("17", NullValueHandling = NullValueHandling.Ignore)]
        public string InverterSeries { get; set; }
        [JsonProperty("18", NullValueHandling = NullValueHandling.Ignore)]
        public int? InvoiceStatus { get; set; }
        [JsonProperty("19", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsCreditNote { get; set; }
        [JsonProperty("20", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsDeleted { get; set; }
        [JsonProperty("21", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsGst { get; set; }
        [JsonProperty("22", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsInvoiced { get; set; }
        [JsonProperty("23", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsPartialValidForSTCInvoice { get; set; }
        [JsonProperty("24", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsPayment { get; set; }
        [JsonProperty("25", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsRelease { get; set; }
        [JsonProperty("26", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsSPVInstallationVerified { get; set; }
        [JsonProperty("27", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsSPVRequired { get; set; }
        [JsonProperty("28", NullValueHandling = NullValueHandling.Ignore)]
        public int? JobID { get; set; }
        [JsonProperty("29", NullValueHandling = NullValueHandling.Ignore)]
        public string JobNumber { get; set; }
        [JsonProperty("30", NullValueHandling = NullValueHandling.Ignore)]
        public int JobTypeId { get; set; }
        [JsonProperty("31", NullValueHandling = NullValueHandling.Ignore)]
        public string OwnerCompany { get; set; }
        [JsonProperty("32", NullValueHandling = NullValueHandling.Ignore)]
        public string OwnerName { get; set; }
        [JsonProperty("33", NullValueHandling = NullValueHandling.Ignore)]
        public string PanelBrand { get; set; }
        [JsonProperty("34", NullValueHandling = NullValueHandling.Ignore)]
        public string PanelModel { get; set; }
        [JsonProperty("35", NullValueHandling = NullValueHandling.Ignore)]
        public string PVDSWHCode { get; set; }
        [JsonProperty("36", NullValueHandling = NullValueHandling.Ignore)]
        public int? RamId { get; set; }
        [JsonProperty("37", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? RECBulkUploadTimeDate { get; set; }
        [JsonProperty("38", NullValueHandling = NullValueHandling.Ignore)]
        public string RefNumberOwnerName { get; set; }
        [JsonProperty("39", NullValueHandling = NullValueHandling.Ignore)]
        public int? ScoUserId { get; set; }
        [JsonProperty("40", NullValueHandling = NullValueHandling.Ignore)]
        public string SolarCompany { get; set; }
        [JsonProperty("41", NullValueHandling = NullValueHandling.Ignore)]
        public int SolarCompanyId { get; set; }
        [JsonProperty("42", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? STC { get; set; }
        [JsonProperty("43", NullValueHandling = NullValueHandling.Ignore)]
        public int? STCInvoiceCount { get; set; }
        [JsonProperty("44", NullValueHandling = NullValueHandling.Ignore)]
        public int STCJobComplianceID { get; set; }
        [JsonProperty("45", NullValueHandling = NullValueHandling.Ignore)]
        public int STCJobDetailsId { get; set; }
        [JsonProperty("46", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? STCPrice { get; set; }
        [JsonProperty("47", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? STCSettlementDate { get; set; }
        [JsonProperty("48", NullValueHandling = NullValueHandling.Ignore)]
        public int? STCSettlementTerm { get; set; }
        [JsonProperty("49", NullValueHandling = NullValueHandling.Ignore)]
        public string STCStatus { get; set; }
        [JsonProperty("50", NullValueHandling = NullValueHandling.Ignore)]
        public int? STCStatusId { get; set; }
        [JsonProperty("51", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? STCSubmissionDate { get; set; }
        [JsonProperty("52", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? SystemSize { get; set; }
        [JsonProperty("53", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime CreatedDate { get; set; }
        [JsonProperty("54", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsUrgentSubmission { get; set; }
        [JsonProperty("55", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? CERAuditedDate { get; set; }
        [JsonProperty("56", NullValueHandling = NullValueHandling.Ignore)]
        public int Totalcount { get; set; }
        [JsonIgnore]
        public string Id { get { return QueryString.QueryStringEncode("id=" + Convert.ToString(JobID.Value)); } }
        [JsonIgnore]
        public string CustomTermLabel { get; set; }
        [JsonIgnore]
        public string strRECBulkUploadTimeDate { get { return RECBulkUploadTimeDate.HasValue ? RECBulkUploadTimeDate.Value.ToString("dd/MM/yyyy") : ""; } }
        [JsonIgnore]
        public string strInstallationDate { get { return InstallationDate.HasValue ? InstallationDate.Value.ToString("dd/MM/yyyy") : ""; } }
        [JsonIgnore]
        public string strSTCSettlementDate { get { return STCSettlementDate.HasValue ? STCSettlementDate.Value.ToString("dd/MM/yyyy") : ""; } }
        [JsonIgnore]
        public string strSTCSubmissionDate { get { return STCSubmissionDate.HasValue ? STCSubmissionDate.Value.ToString("dd/MM/yyyy") : ""; } }
        [JsonIgnore]
        public string strCERAuditedDate { get { return CERAuditedDate.HasValue ? CERAuditedDate.Value.ToString("dd/MM/yyyy") : ""; } }
        [JsonIgnore]
        public int STCJobDetailsID { get { return STCJobDetailsId; } set { STCJobDetailsId = value; } }
        [JsonIgnore]
        public int JobType { get { return JobTypeId; } }


    }

    public class PeakPayView
    {
        [JsonProperty("1", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? CERApprovedDate { get; set; }
        [JsonProperty("2", NullValueHandling = NullValueHandling.Ignore)]
        public int? DaysLeft { get; set; }
        [JsonProperty("3", NullValueHandling = NullValueHandling.Ignore)]
        public string InstallationAddress { get; set; }
        [JsonProperty("4", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsGst { get; set; }
        [JsonProperty("5", NullValueHandling = NullValueHandling.Ignore)]
        public int? IsInvoiced { get; set; }
        [JsonProperty("6", NullValueHandling = NullValueHandling.Ignore)]
        public int? JobID { get; set; }
        [JsonProperty("7", NullValueHandling = NullValueHandling.Ignore)]
        public string OwnerName { get; set; }
        [JsonProperty("8", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? PaymentDate { get; set; }
        [JsonProperty("9", NullValueHandling = NullValueHandling.Ignore)]
        public string Reference { get; set; }
        [JsonProperty("10", NullValueHandling = NullValueHandling.Ignore)]
        public long? rownum { get; set; }
        [JsonProperty("11", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? SettleBefore { get; set; }
        [JsonProperty("12", NullValueHandling = NullValueHandling.Ignore)]
        public string SolarCompany { get; set; }
        [JsonProperty("13", NullValueHandling = NullValueHandling.Ignore)]
        public int? SolarCompanyId { get; set; }
        [JsonProperty("14", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? STCAmount { get; set; }
        [JsonProperty("15", NullValueHandling = NullValueHandling.Ignore)]
        public decimal STCFee { get; set; }
        [JsonProperty("16", NullValueHandling = NullValueHandling.Ignore)]
        public long STCInvoiceCnt { get; set; }
        [JsonProperty("17", NullValueHandling = NullValueHandling.Ignore)]
        public int STCJobDetailsID { get; set; }
        [JsonProperty("18", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? STCPrice { get; set; }
        [JsonProperty("19", NullValueHandling = NullValueHandling.Ignore)]
        public string STCPVDCode { get; set; }
        [JsonProperty("20", NullValueHandling = NullValueHandling.Ignore)]
        public int? STCSettlementTerm { get; set; }
        [JsonProperty("21", NullValueHandling = NullValueHandling.Ignore)]
        public string StcStatus { get; set; }
        [JsonProperty("22", NullValueHandling = NullValueHandling.Ignore)]
        public int? STCStatusId { get; set; }
        [JsonProperty("23", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? SubmissionDate { get; set; }
        [JsonProperty("24", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? SystemSize { get; set; }
        [JsonProperty("25", NullValueHandling = NullValueHandling.Ignore)]
        public decimal Total { get; set; }
        [JsonProperty("26", NullValueHandling = NullValueHandling.Ignore)]
        public int Totalcount { get; set; }

        [JsonIgnore]
        public string Id { get { return QueryString.QueryStringEncode("id=" + Convert.ToString(JobID)); } }

    }

    public class JobViewLists
    {
        public List<JobView> jobViews { get; set; }
        public List<UserWiseColumns> userWiseColumns { get; set; }
    }
}
