using FormBot.BAL.Service.Job;
using FormBot.Entity;
using FormBot.Entity.Job;
using FormBot.Entity.KendoGrid;
using FormBot.Helper;
using FormBot.Helper.Helper;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace FormBot.BAL.Service.CommonRules
{
    public class CommonBAL
    {
        public static Object myLock = new Object();
        public static Logger _log;
        private static readonly ICreateJobBAL _job;
        private static readonly IPeakPayBAL _peakPay;
        public static string[] FieldsForInstallerDesignerSolarCompany { get { return new[] { "InstallerAccreditationNumber", "InstallerFirstName", "InstallerFullAddress", "InstallerFullName", "InstallerLastName", "InstallerLicenseNumber", "InstallerPostCode", "InstallerState", "InstallerStreetName", "InstallerStreetNumber", "InstallerStreetTypeID", "InstallerTown", "InstallerUnitNumber", "InstallerUnitTypeID", "DesignerAccreditationNumber", "DesignerFirstName", "DesignerFullAddress", "DesignerFullName", "DesignerLastName", "DesignerLicenseNumber", "DesignerPostCode", "DesignerState", "DesignerStreetName", "DesignerStreetNumber", "DesignerStreetTypeID", "DesignerTown", "DesignerUnitNumber", "DesignerUnitTypeID", "SolarCompany", "SolarCompanyABN", "SolarCompanyAccreditationNumber", "SolarCompanyFirstName", "SolarCompanyFullAddress", "SolarCompanyFullName", "SolarCompanyLastName", "SolarCompanyName", "SolarCompanyPostCode", "SolarCompanyState", "SolarCompanyStreetName", "SolarCompanyStreetNumber", "SolarCompanyStreetTypeID", "SolarCompanyTown", "SolarCompanyUnitNumber", "SolarCompanyUnitTypeID" }; } }
        static CommonBAL()
        {
            _job = new CreateJobBAL();
            _peakPay = new PeakPayBAL();
            _log = new Logger();
        }

        //public static object GetPropValue(object src, string propName)
        //{
        //    return src.GetType().GetProperty(propName).GetValue(src, null);
        //}

        public static void SetPropValue(object src, string propName, object value)
        {
            try
            {
                var prop = src.GetType().GetProperty(propName);
                if (prop != null)
                {
                    object val = Common.ResetValue(value, prop.PropertyType);
                    prop.SetValue(src, val);
                }
            }
            catch (Exception ex)
            {
                _log.LogException("SetPropValue property - " + propName, ex);
            }
        }

        #region List To DataTable
        public static DataTable ToDataTable<T>(IList<T> data)
        {
            if (data == null || data.Count == 0)
                return new DataTable();
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
        #endregion

        public static void ValidateYearForGridData(ref int year)
        {
            year = year > 2010 ? year : DateTime.Now.Year;
        }

        public static Tuple<int, int> GetYearForJobGrid(int year)
        {
            int fromYear, toYear;
            if (year == 0)
            {
                fromYear = DateTime.Now.Year - 1;
                toYear = DateTime.Now.Year;
            }
            else
                fromYear = toYear = year;
            return new Tuple<int, int>(fromYear, toYear);
        }

        #region DataTable To List
        //Latest by 27Apr22
        public static List<JobView> DataTableToListJobs(DataSet ds)
        {
            if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
                return new List<JobView>();
            return DataTableToListJobs(ds.Tables[0]);
        }
        //Latest by 27Apr22
        public static List<JobView> DataTableToListJobs(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return new List<JobView>();

            try
            {
                return dt.AsEnumerable().Select(m => new JobView
                {
                    CalculatedSTC = m.Field<decimal?>("CalculatedSTC"),
                    ClientName = m.Field<string>("ClientName"),
                    ColorCode = m.Field<string>("ColorCode"),
                    CreatedBy = m.Field<int>("CreatedBy"),
                    CreatedDate = m.Field<DateTime>("CreatedDate"),
                    DeemingPeriod = m.Field<string>("DeemingPeriod"),
                    DesignerAccreditationNumber = m.Field<string>("DesignerAccreditationNumber"),
                    DesignerFirstName = m.Field<string>("DesignerFirstName"),
                    DesignerFullAddress = m.Field<string>("DesignerFullAddress"),
                    DesignerFullName = m.Field<string>("DesignerFullName"),
                    DesignerLastName = m.Field<string>("DesignerLastName"),
                    DesignerLicenseNumber = m.Field<string>("DesignerLicenseNumber"),
                    DesignerPostCode = m.Field<string>("DesignerPostCode"),
                    DesignerState = m.Field<string>("DesignerState"),
                    DesignerStreetName = m.Field<string>("DesignerStreetName"),
                    DesignerStreetNumber = m.Field<string>("DesignerStreetNumber"),
                    DesignerStreetTypeID = m.Field<string>("DesignerStreetTypeID"),
                    DesignerTown = m.Field<string>("DesignerTown"),
                    DesignerUnitNumber = m.Field<string>("DesignerUnitNumber"),
                    DesignerUnitTypeID = m.Field<string>("DesignerUnitTypeID"),
                    Distributor = m.Field<string>("Distributor"),
                    DocumentCount = m.Field<int?>("DocumentCount"),
                    ElectricianCompanyName = m.Field<string>("ElectricianCompanyName"),
                    ElectricianFirstName = m.Field<string>("ElectricianFirstName"),
                    ElectricianFullAddress = m.Field<string>("ElectricianFullAddress"),
                    ElectricianFullName = m.Field<string>("ElectricianFullName"),
                    ElectricianLastName = m.Field<string>("ElectricianLastName"),
                    ElectricianLicenseNumber = m.Field<string>("ElectricianLicenseNumber"),
                    ElectricianPostCode = m.Field<string>("ElectricianPostCode"),
                    ElectricianState = m.Field<string>("ElectricianState"),
                    ElectricianStreetName = m.Field<string>("ElectricianStreetName"),
                    ElectricianStreetNumber = m.Field<string>("ElectricianStreetNumber"),
                    ElectricianStreetTypeID = m.Field<string>("ElectricianStreetTypeID"),
                    ElectricianTown = m.Field<string>("ElectricianTown"),
                    ElectricianUnitNumber = m.Field<string>("ElectricianUnitNumber"),
                    ElectricianUnitTypeID = m.Field<string>("ElectricianUnitTypeID"),
                    ElectricityProvider = m.Field<string>("ElectricityProvider"),
                    FullBasicDetails = m.Field<string>("FullBasicDetails"),
                    FullInstallationAddress = m.Field<string>("FullInstallationAddress"),
                    FullOwnerAddress = m.Field<string>("FullOwnerAddress"),
                    FullOwnerCompanyDetails = m.Field<string>("FullOwnerCompanyDetails"),
                    FullOwnerName = m.Field<string>("FullOwnerName"),
                    FullPanelDetails = m.Field<string>("FullPanelDetails"),
                    GSTDocument = m.Field<string>("GSTDocument"),
                    InstallationDate = m.Field<DateTime?>("InstallationDate"),
                    InstallationPostCode = m.Field<string>("InstallationPostCode"),
                    InstallationState = m.Field<string>("InstallationState"),
                    InstallationStreetName = m.Field<string>("InstallationStreetName"),
                    InstallationStreetNumber = m.Field<string>("InstallationStreetNumber"),
                    InstallationStreetTypeID = m.Field<string>("InstallationStreetTypeID"),
                    InstallationTown = m.Field<string>("InstallationTown"),
                    InstallationType = m.Field<string>("InstallationType"),
                    InstallationUnitNumber = m.Field<string>("InstallationUnitNumber"),
                    InstallationUnitTypeID = m.Field<string>("InstallationUnitTypeID"),
                    InstallerAccreditationNumber = m.Field<string>("InstallerAccreditationNumber"),
                    InstallerFirstName = m.Field<string>("InstallerFirstName"),
                    InstallerFullAddress = m.Field<string>("InstallerFullAddress"),
                    InstallerFullName = m.Field<string>("InstallerFullName"),
                    InstallerLastName = m.Field<string>("InstallerLastName"),
                    InstallerLicenseNumber = m.Field<string>("InstallerLicenseNumber"),
                    InstallerPostCode = m.Field<string>("InstallerPostCode"),
                    InstallerState = m.Field<string>("InstallerState"),
                    InstallerStreetName = m.Field<string>("InstallerStreetName"),
                    InstallerStreetNumber = m.Field<string>("InstallerStreetNumber"),
                    InstallerStreetTypeID = m.Field<string>("InstallerStreetTypeID"),
                    InstallerTown = m.Field<string>("InstallerTown"),
                    InstallerUnitNumber = m.Field<string>("InstallerUnitNumber"),
                    InstallerUnitTypeID = m.Field<string>("InstallerUnitTypeID"),
                    IsAccept = m.Field<bool>("IsAccept"),
                    IsBasicValidation = m.Field<int>("IsBasicValidation"),
                    IsCESForm = m.Field<int>("IsCESForm"),
                    IsConnectionCompleted = m.Field<bool?>("IsConnectionCompleted"),
                    IsCustPrice = m.Field<int>("IsCustPrice"),
                    IsDeleted = m.Field<bool>("IsDeleted"),
                    IsGroupSiganture = m.Field<int>("IsGroupSiganture"),
                    IsGst = m.Field<bool?>("IsGst"),
                    IsInstallerSiganture = m.Field<int>("IsInstallerSiganture"),
                    IsInvoiced = m.Field<bool?>("IsInvoiced"),
                    IsOwnerSiganture = m.Field<int>("IsOwnerSiganture"),
                    IsPreApprovaApproved = m.Field<bool?>("IsPreApprovaApproved"),
                    IsSerialNumberCheck = m.Field<int>("IsSerialNumberCheck"),
                    IsSTCForm = m.Field<int>("IsSTCForm"),
                    IsSTCSubmissionPhotos = m.Field<int>("IsSTCSubmissionPhotos"),
                    IsTraded = m.Field<bool?>("IsTraded"),
                    JobAddress = m.Field<string>("JobAddress"),
                    JobDescription = m.Field<string>("JobDescription"),
                    JobID = m.Field<int>("JobID"),
                    JobNumber = m.Field<string>("JobNumber"),
                    JobStage = m.Field<string>("JobStage"),
                    JobStageChangeDate = m.Field<DateTime?>("JobStageChangeDate"),
                    JobTitle = m.Field<string>("JobTitle"),
                    JobTypeId = m.Field<byte>("JobTypeId"),
                    MeterNumber = m.Field<string>("MeterNumber"),
                    NMI = m.Field<string>("NMI"),
                    NoOfPanel = m.Field<int?>("NoOfPanel"),
                    OwnerEmail = m.Field<string>("OwnerEmail"),
                    OwnerMobile = m.Field<string>("OwnerMobile"),
                    OwnerPhone = m.Field<string>("OwnerPhone"),
                    OwnerPostCode = m.Field<string>("OwnerPostCode"),
                    OwnerState = m.Field<string>("OwnerState"),
                    OwnerStreetName = m.Field<string>("OwnerStreetName"),
                    OwnerStreetNumber = m.Field<string>("OwnerStreetNumber"),
                    OwnerStreetTypeID = m.Field<string>("OwnerStreetTypeID"),
                    OwnerTown = m.Field<string>("OwnerTown"),
                    OwnerType = m.Field<string>("OwnerType"),
                    OwnerUnitNumber = m.Field<string>("OwnerUnitNumber"),
                    OwnerUnitTypeID = m.Field<string>("OwnerUnitTypeID"),
                    PhaseProperty = m.Field<string>("PhaseProperty"),
                    Priority = m.Field<byte>("Priority"),
                    PropertyType = m.Field<string>("PropertyType"),
                    RefNumber = m.Field<string>("RefNumber"),
                    SCOName = m.Field<string>("SCOName"),
                    ScoUserId = m.Field<int?>("ScoUserId"),
                    SerialNumbers = m.Field<string>("SerialNumbers"),
                    SolarCompany = m.Field<string>("SolarCompany"),
                    SolarCompanyABN = m.Field<string>("SolarCompanyABN"),
                    SolarCompanyAccreditationNumber = m.Field<string>("SolarCompanyAccreditationNumber"),
                    SolarCompanyFirstName = m.Field<string>("SolarCompanyFirstName"),
                    SolarCompanyFullAddress = m.Field<string>("SolarCompanyFullAddress"),
                    SolarCompanyFullName = m.Field<string>("SolarCompanyFullName"),
                    SolarCompanyId = m.Field<int?>("SolarCompanyId"),
                    SolarCompanyLastName = m.Field<string>("SolarCompanyLastName"),
                    SolarCompanyName = m.Field<string>("SolarCompanyName"),
                    SolarCompanyPostCode = m.Field<string>("SolarCompanyPostCode"),
                    SolarCompanyState = m.Field<string>("SolarCompanyState"),
                    SolarCompanyStreetName = m.Field<string>("SolarCompanyStreetName"),
                    SolarCompanyStreetNumber = m.Field<string>("SolarCompanyStreetNumber"),
                    SolarCompanyStreetTypeID = m.Field<string>("SolarCompanyStreetTypeID"),
                    SolarCompanyTown = m.Field<string>("SolarCompanyTown"),
                    SolarCompanyUnitNumber = m.Field<string>("SolarCompanyUnitNumber"),
                    SolarCompanyUnitTypeID = m.Field<string>("SolarCompanyUnitTypeID"),
                    SSCID = m.Field<int?>("SSCID"),
                    StaffName = m.Field<string>("StaffName"),
                    STC = m.Field<decimal?>("STC"),
                    SystemMountingType = m.Field<string>("SystemMountingType"),
                    SystemSize = m.Field<decimal?>("SystemSize"),
                    TradeStatus = m.Field<int>("TradeStatus"),
                    TypeOfConnection = m.Field<string>("TypeOfConnection"),
                    InstallerID = m.Field<int?>("InstallerID"),
                    DesignerID = m.Field<int?>("DesignerID"),
                    PanelBrand = m.Field<string>("PanelBrand"),
                    PanelModel = m.Field<string>("PanelModel"),
                    InverterBrand = m.Field<string>("InverterBrand"),
                    InverterModel = m.Field<string>("InverterModel"),
                    InverterSeries = m.Field<string>("InverterSeries")
                    //Totalcount = m.Field<int>("Totalcount")
                }).ToList();
            }
            catch (Exception ex)
            {
                _log.LogException("DataTableToListJobs", ex);
                return new List<JobView>();
            }
        }

        //Latest by 27Apr22
        public static List<PeakPayView> DataTableToListPeakPay(DataSet ds)
        {
            if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
                return new List<PeakPayView>();
            return DataTableToListPeakPay(ds.Tables[0]);
        }

        //Latest by 27Apr22
        public static List<PeakPayView> DataTableToListPeakPay(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return new List<PeakPayView>();

            try
            {
                return dt.AsEnumerable().Select(m => new PeakPayView
                {
                    CERApprovedDate = m.Field<DateTime?>("CERApprovedDate"),
                    DaysLeft = m.Field<int?>("DaysLeft"),
                    InstallationAddress = m.Field<string>("InstallationAddress"),
                    IsGst = m.Field<bool?>("IsGst"),
                    IsInvoiced = m.Field<int?>("IsInvoiced"),
                    JobID = m.Field<int?>("JobID"),
                    OwnerName = m.Field<string>("OwnerName"),
                    PaymentDate = m.Field<DateTime?>("PaymentDate"),
                    Reference = m.Field<string>("Reference"),
                    rownum = m.Field<long?>("rownum"),
                    SettleBefore = m.Field<DateTime?>("SettleBefore"),
                    SolarCompany = m.Field<string>("SolarCompany"),
                    SolarCompanyId = m.Field<int?>("SolarCompanyId"),
                    STCAmount = m.Field<decimal?>("STCAmount"),
                    STCFee = m.Field<decimal>("STCFee"),
                    STCInvoiceCnt = m.Field<long>("STCInvoiceCnt"),
                    STCJobDetailsID = m.Field<int>("STCJobDetailsID"),
                    STCPrice = m.Field<decimal?>("STCPrice"),
                    STCPVDCode = m.Field<string>("STCPVDCode"),
                    STCSettlementTerm = m.Field<int?>("STCSettlementTerm"),
                    StcStatus = m.Field<string>("StcStatus"),
                    STCStatusId = m.Field<int?>("STCStatusId"),
                    SubmissionDate = m.Field<DateTime?>("SubmissionDate"),
                    SystemSize = m.Field<decimal?>("SystemSize"),
                    Total = m.Field<decimal>("Total"),

                }).ToList();
            }
            catch (Exception ex)
            {
                _log.LogException("DataTableToListPeakPay", ex);
                return new List<PeakPayView>();
            }
        }

        public static List<STCSubmissionView> DataTableToListSTCSubmission(DataSet ds)
        {
            if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
                return new List<STCSubmissionView>();
            return DataTableToListSTCSubmission(ds.Tables[0]);
        }

        public static List<STCSubmissionView> DataTableToListSTCSubmission(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return new List<STCSubmissionView>();

            try
            {
                return dt.AsEnumerable().Select(m => new STCSubmissionView
                {
                    AccountManager = m.Field<string>("AccountManager"),
                    ColorCode = m.Field<string>("ColorCode"),
                    ComplianceBy = m.Field<int?>("ComplianceBy"),
                    ComplianceNotes = m.Field<string>("ComplianceNotes"),
                    ComplianceOfficer = m.Field<string>("ComplianceOfficer"),
                    CreatedBy = m.Field<int>("CreatedBy"),
                    CreatedDate = m.Field<DateTime>("CreatedDate"),
                    CustomSettlementTerm = m.Field<int?>("CustomSettlementTerm"),
                    GBBatchRECUploadId = m.Field<string>("GBBatchRECUploadId"),
                    HasMultipleRecords = m.Field<bool?>("HasMultipleRecords"),
                    InstallationAddress = m.Field<string>("InstallationAddress"),
                    InstallationDate = m.Field<DateTime?>("InstallationDate"),
                    InstallationState = m.Field<string>("InstallationState"),
                    InstallationTown = m.Field<string>("InstallationTown"),
                    InstallerName = m.Field<string>("InstallerName"),
                    InverterBrand = m.Field<string>("InverterBrand"),
                    InverterModel = m.Field<string>("InverterModel"),
                    InverterSeries = m.Field<string>("InverterSeries"),
                    InvoiceStatus = m.Field<int?>("InvoiceStatus"),
                    IsCreditNote = m.Field<bool?>("IsCreditNote"),
                    IsDeleted = m.Field<bool>("IsDeleted"),
                    IsGst = m.Field<bool?>("IsGst"),
                    IsInvoiced = m.Field<bool?>("IsInvoiced"),
                    IsPartialValidForSTCInvoice = m.Field<bool?>("IsPartialValidForSTCInvoice"),
                    IsPayment = m.Field<bool?>("IsPayment"),
                    IsRelease = m.Field<bool?>("IsRelease"),
                    IsSPVInstallationVerified = m.Field<bool?>("IsSPVInstallationVerified"),
                    IsSPVRequired = m.Field<bool?>("IsSPVRequired"),
                    JobID = m.Field<int?>("JobID"),
                    JobNumber = m.Field<string>("JobNumber"),
                    JobTypeId = m.Field<byte>("JobTypeId"),
                    OwnerCompany = m.Field<string>("OwnerCompany"),
                    OwnerName = m.Field<string>("OwnerName"),
                    PanelBrand = m.Field<string>("PanelBrand"),
                    PanelModel = m.Field<string>("PanelModel"),
                    PVDSWHCode = m.Field<string>("PVDSWHCode"),
                    RamId = m.Field<int?>("RamId"),
                    RECBulkUploadTimeDate = m.Field<DateTime?>("RECBulkUploadTimeDate"),
                    RefNumberOwnerName = m.Field<string>("RefNumberOwnerName"),
                    ScoUserId = m.Field<int?>("ScoUserId"),
                    SolarCompany = m.Field<string>("SolarCompany"),
                    SolarCompanyId = m.Field<int>("SolarCompanyId"),
                    STC = m.Field<decimal?>("STC"),
                    STCInvoiceCount = m.Field<int?>("STCInvoiceCount"),
                    STCJobComplianceID = m.Field<int>("STCJobComplianceID"),
                    STCJobDetailsId = m.Field<int>("STCJobDetailsId"),
                    STCPrice = m.Field<decimal?>("STCPrice"),
                    STCSettlementDate = m.Field<DateTime?>("STCSettlementDate"),
                    STCSettlementTerm = m.Field<int?>("STCSettlementTerm"),
                    STCStatus = m.Field<string>("STCStatus"),
                    STCStatusId = m.Field<int?>("STCStatusId"),
                    STCSubmissionDate = m.Field<DateTime?>("STCSubmissionDate"),
                    SystemSize = m.Field<decimal?>("SystemSize"),
                    IsUrgentSubmission = m.Field<bool>("IsUrgentSubmission"),
                    CERAuditedDate = m.Field<DateTime?>("CERAuditedDate")
                }).ToList();
            }
            catch (Exception ex)
            {
                _log.LogException(SystemEnums.Severity.Error, "Method Name : DataTableToListSTCSubmission", ex);
                return new List<STCSubmissionView>();
            }
        }


        //Latest by 04May22
        public static List<JobsInstallerDesignerView> DataTableToListInstallerDesigner(DataSet ds)
        {
            if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
                return new List<JobsInstallerDesignerView>();

            return ds.Tables[0].AsEnumerable().Select(m => new JobsInstallerDesignerView
            {
                InstallerDesignerID = m.Field<int>("InstallerDesignerID"),
                InstallerDesignerFullName = m.Field<string>("InstallerDesignerFullName"),
                InstallerDesignerFirstName = m.Field<string>("InstallerDesignerFirstName"),
                InstallerDesignerLastName = m.Field<string>("InstallerDesignerLastName"),
                InstallerDesignerFullAddress = m.Field<string>("InstallerDesignerFullAddress"),
                InstallerDesignerUnitTypeID = m.Field<string>("InstallerDesignerUnitTypeID"),
                InstallerDesignerUnitNumber = m.Field<string>("InstallerDesignerUnitNumber"),
                InstallerDesignerStreetNumber = m.Field<string>("InstallerDesignerStreetNumber"),
                InstallerDesignerStreetName = m.Field<string>("InstallerDesignerStreetName"),
                InstallerDesignerStreetTypeID = m.Field<string>("InstallerDesignerStreetTypeID"),
                InstallerDesignerTown = m.Field<string>("InstallerDesignerTown"),
                InstallerDesignerState = m.Field<string>("InstallerDesignerState"),
                InstallerDesignerPostCode = m.Field<string>("InstallerDesignerPostCode"),
                InstallerDesignerAccreditationNumber = m.Field<string>("InstallerDesignerAccreditationNumber"),
                InstallerDesignerLicenseNumber = m.Field<string>("InstallerDesignerLicenseNumber")
            }).ToList();
        }

        //Latest by 05May22
        public static List<JobsSolarCompanyView> DataTableToListSolarCompany(DataSet ds)
        {
            if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
                return new List<JobsSolarCompanyView>();

            return ds.Tables[0].AsEnumerable().Select(m => new JobsSolarCompanyView
            {
                SolarCompanyId = m.Field<int>("SolarCompanyId"),
                SolarCompany = m.Field<string>("SolarCompany"),
                SolarCompanyFirstName = m.Field<string>("SolarCompanyFirstName"),
                SolarCompanyLastName = m.Field<string>("SolarCompanyLastName"),
                SolarCompanyABN = m.Field<string>("SolarCompanyABN"),
                SolarCompanyFullAddress = m.Field<string>("SolarCompanyFullAddress"),
                SolarCompanyUnitTypeID = m.Field<string>("SolarCompanyUnitTypeID"),
                SolarCompanyUnitNumber = m.Field<string>("SolarCompanyUnitNumber"),
                SolarCompanyStreetNumber = m.Field<string>("SolarCompanyStreetNumber"),
                SolarCompanyStreetName = m.Field<string>("SolarCompanyStreetName"),
                SolarCompanyStreetTypeID = m.Field<string>("SolarCompanyStreetTypeID"),
                SolarCompanyTown = m.Field<string>("SolarCompanyTown"),
                SolarCompanyState = m.Field<string>("SolarCompanyState"),
                SolarCompanyPostCode = m.Field<string>("SolarCompanyPostCode"),
                SolarCompanyAccreditationNumber = m.Field<string>("SolarCompanyAccreditationNumber")
            }).ToList();
        }

        //Latest by 05May22
        public static List<UserWiseColumns> DataTableToListUserWiseColumns(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return new List<UserWiseColumns>();

            return dt.AsEnumerable().Select(m => new UserWiseColumns
            {
                ColumnID = m.Field<int>("ColumnID"),
                Name = m.Field<string>("Name"),
                DisplayName = m.Field<string>("DisplayName"),
                Width = m.Field<double>("Width"),
                OrderNumber = m.Field<int>("OrderNumber"),
            }).ToList();
        }
        #endregion

        #region Redis Cache Hash Methods

        #region Redis cache Hash Parent Object Methods
        public static void RemoveHashKeys(IDatabase cache, List<int> ids, string parentKey, string childKey, int year = 0)
        {
            string mainKey = year == 0 ? parentKey : string.Format(parentKey, year);
            List<DistributedCacheAllKeysInfoForHashSetView> parentCaches = CommonBAL.DistributedCacheAllKeysInfoGet(cache, mainKey);

            Dictionary<int, string> dictJobsWithRedisKey = new Dictionary<int, string>();
            parentCaches.Where(d => ids.Contains(d.PID) && !string.IsNullOrEmpty(d.Ids)).ToList().ForEach(d =>
            {
                if (!string.IsNullOrEmpty(d.Ids))
                {
                    string redisKey = string.Format(childKey, d.PID);
                    foreach (var item in d.Ids.Split(','))
                    {
                        int id = 0;
                        int.TryParse(item, out id);
                        if (id > 0 && !dictJobsWithRedisKey.ContainsKey(id))
                            dictJobsWithRedisKey.Add(id, redisKey);
                    }
                }
            });
            for (int i = 0; i < decimal.Divide(dictJobsWithRedisKey.Count, ProjectConfiguration.RedisCacheBatchCommandCount); i++)
            {
                Dictionary<int, string> keysToConsider = dictJobsWithRedisKey.Skip(i * ProjectConfiguration.RedisCacheBatchCommandCount).Take(ProjectConfiguration.RedisCacheBatchCommandCount).ToDictionary(pair => pair.Key, pair => pair.Value);
                RedisCacheConfiguration.DeleteHashAsync(cache, keysToConsider);
            }
            parentCaches = DistributedCacheAllKeysInfoGetAndRemoveRequired(cache, ids, mainKey);
            DistributedCacheAllKeysInfoSet(cache, mainKey, parentCaches);
        }


        //Latest by 27Apr22
        public static List<DistributedCacheAllKeysInfoForHashSetView> DistributedCacheAllKeysInfoGet(IDatabase cache, string key)
        {
            var timer = new Stopwatch();
            timer.Start();
            List<DistributedCacheAllKeysInfoForHashSetView> listDistributedCacheAllKeysInfoForHashSetViews = RedisCacheConfiguration.Get<List<DistributedCacheAllKeysInfoForHashSetView>>(cache, key);
            timer.Stop();
            if (listDistributedCacheAllKeysInfoForHashSetViews == null)
                listDistributedCacheAllKeysInfoForHashSetViews = new List<DistributedCacheAllKeysInfoForHashSetView>();

            if (timer.ElapsedMilliseconds > 1000)
                Helper.Log.WriteLog("BAL : DistributedCacheAllKeysInfoGet Method Key : " + key + " Redis Time : " + (timer.ElapsedMilliseconds));
            return listDistributedCacheAllKeysInfoForHashSetViews;
        }

        //Latest by 27Apr22
        public static void DistributedCacheAllKeysInfoSet(IDatabase cache, string key, List<DistributedCacheAllKeysInfoForHashSetView> lstAllKeysInfoToHash)
        {
            var timer = new Stopwatch();
            timer.Start();
            RedisCacheConfiguration.Set(cache, key, lstAllKeysInfoToHash);
            timer.Stop();
            Helper.Log.WriteLog("BAL : DistributedCacheAllKeysInfoSet Method Key : " + key + " Redis Time : " + (timer.ElapsedMilliseconds));
        }

        //Latest by 27Apr22
        private static List<DistributedCacheAllKeysInfoForHashSetView> DistributedCacheAllKeysInfoGetAndRemoveRequired(IDatabase cache, List<int> lstParentIds, string key)
        {
            List<DistributedCacheAllKeysInfoForHashSetView> parentCaches = DistributedCacheAllKeysInfoGet(cache, key);
            if (parentCaches != null && parentCaches.Count > 0)
                parentCaches.RemoveAll(r => lstParentIds.Contains(r.PID));
            else
                parentCaches = new List<DistributedCacheAllKeysInfoForHashSetView>();
            return parentCaches;
        }

        // Latest by 27Apr22
        private static void DistributedCachePrepareAllKeysInfoCacheObject(ref List<DistributedCacheAllKeysInfoForHashSetView> parentCaches, int parentId, string childIds, string RedisKey)
        {
            parentCaches.Add(new DistributedCacheAllKeysInfoForHashSetView(parentId, childIds, RedisKey));
        }
        #endregion

        //Latest by 24May2022
        public static void PeakPayDistributedCacheHashSet(IDatabase cache, string SolarCompanyId, int resellerID)
        {
            DataSet dsAllColumnsData = new DataSet();
            List<PeakPayView> lstPeakPay = new List<PeakPayView>();
            PeakPayDistributedCacheHashSet(cache, SolarCompanyId, resellerID, ref dsAllColumnsData, ref lstPeakPay);
        }

        //Latest by 24May22
        public static void PeakPayDistributedCacheHashSet(IDatabase cache, string SolarCompanyId, int ResellerId, ref DataSet dsAllColumnsData, ref List<PeakPayView> lstPeakPay)
        {
            var timerDB = new Stopwatch();
            var timer = new Stopwatch();
            List<DistributedCacheAllKeysInfoForHashSetView> parentCaches = new List<DistributedCacheAllKeysInfoForHashSetView>();
            lock (myLock)
            {
                List<int> solarCompanyIds = !string.IsNullOrEmpty(SolarCompanyId) && SolarCompanyId != "0" && SolarCompanyId != "-1" ? SolarCompanyId.Split(',').Select(k => Convert.ToInt32(k)).ToList() : new List<int>();
                //parentCaches = DistributedCacheAllKeysInfoGetAndRemoveRequired(cache, solarCompanyIds, RedisCacheConfiguration.dsPeakPayAllKeysInfoHashKey);
                timerDB.Start();
                dsAllColumnsData = _peakPay.GetPeakPayListForCache(ResellerId > 0 ? "" : SolarCompanyId, ResellerId);
                timerDB.Stop();
                timer.Start();
                if (dsAllColumnsData.Tables[0] != null && dsAllColumnsData.Tables[0].Rows.Count > 0)
                {
                    lstPeakPay = DataTableToListPeakPay(dsAllColumnsData);

                    var lstJobsToHash = lstPeakPay.Select(k => new RedisHashSetKeyValuePair()
                    {
                        RedisKey = string.Format(RedisCacheConfiguration.dsPeakPayHashKey, k.SolarCompanyId),
                        HashKey = k.JobID.Value,
                        HashValuePayload = RedisCacheConfiguration.Serialize(k)
                    }).ToList();

                    for (int i = 0; i < decimal.Divide(lstJobsToHash.Count, ProjectConfiguration.RedisCacheBatchCommandCount); i++)
                    {
                        var jobsToHash = lstJobsToHash.Skip(i * ProjectConfiguration.RedisCacheBatchCommandCount).Take(ProjectConfiguration.RedisCacheBatchCommandCount).ToList();
                        RedisCacheConfiguration.SetBatchUsingHash(cache, jobsToHash);
                        RedisCacheConfiguration.SetExpireForHash(cache, jobsToHash);
                    }

                    var lstSolarCompanyIdToRemoveFromParentCache = lstPeakPay.Select(d => d.SolarCompanyId.Value).Distinct().ToList();
                    //parentCaches = DistributedCacheAllKeysInfoGetAndRemoveRequired(cache, lstSolarCompanyIdToRemoveFromParentCache, RedisCacheConfiguration.dsPeakPayAllKeysInfoHashKey);

                    List<DistributedCacheAllKeysInfoForHashSetView> lstJobsParentToHash =
                            (from i in lstPeakPay
                             group i by i.SolarCompanyId into g
                             select new DistributedCacheAllKeysInfoForHashSetView { PID = g.Key.Value, RedisKey = string.Format(RedisCacheConfiguration.dsPeakPayHashKey, g.Key), Ids = string.Join(",", g.Select(kvp => kvp.JobID)) })
                             .ToList();
                    List<int> lstJobsParentToHashRemaining = solarCompanyIds.Where(d => !lstJobsParentToHash.Any(k => k.PID == d)).ToList();
                    if (lstJobsParentToHashRemaining.Any())
                    {
                        lstJobsParentToHashRemaining.ForEach(k => { lstJobsParentToHash.Add(new DistributedCacheAllKeysInfoForHashSetView(k, "", "")); });
                        lstSolarCompanyIdToRemoveFromParentCache.AddRange(lstJobsParentToHashRemaining);
                    }
                    parentCaches = DistributedCacheAllKeysInfoGetAndRemoveRequired(cache, lstSolarCompanyIdToRemoveFromParentCache, RedisCacheConfiguration.dsPeakPayAllKeysInfoHashKey);
                    parentCaches.AddRange(lstJobsParentToHash);
                    DistributedCacheAllKeysInfoSet(cache, RedisCacheConfiguration.dsPeakPayAllKeysInfoHashKey, parentCaches);
                }
                else
                {
                    parentCaches = DistributedCacheAllKeysInfoGetAndRemoveRequired(cache, solarCompanyIds, RedisCacheConfiguration.dsPeakPayAllKeysInfoHashKey);
                    for (int i = 0; i < solarCompanyIds.Count; i++)
                    {
                        var iSolarID = solarCompanyIds[i];
                        DistributedCachePrepareAllKeysInfoCacheObject(ref parentCaches, iSolarID, "", "");
                    }
                    DistributedCacheAllKeysInfoSet(cache, RedisCacheConfiguration.dsPeakPayAllKeysInfoHashKey, parentCaches);
                }
                //DistributedCacheAllKeysInfoSet(cache, RedisCacheConfiguration.dsPeakPayAllKeysInfoHashKey, parentCaches);
                timer.Stop();
                //Helper.Log.WriteLog("BAL : PeakPayDistributedCacheHashSet Method DB Time : " + timerDB.ElapsedMilliseconds + " Redis Time : " + (timer.ElapsedMilliseconds));
            }
            //List<JobsInstallerDesignerView> lstJobsInstallerDesignerView = new List<JobsInstallerDesignerView>();
            //List<JobsSolarCompanyView> lstJobsSolarCompanyView = new List<JobsSolarCompanyView>();
            //InstallerDesignerDistributedCacheHashSet(cache, ref lstJobsInstallerDesignerView);
            //SolarCompanyDistributedCacheHashSet(cache, ref lstJobsSolarCompanyView);
        }

        //Latest by 24May22
        public static void PeakPayDistributedCacheHashGet(IDatabase cache, List<int> solarCompanyIDs, ref List<PeakPayView> lstPeakPay)
        {
            var timer = new Stopwatch();
            var timer1 = new Stopwatch();
            timer.Start();

            List<DistributedCacheAllKeysInfoForHashSetView> parentCaches = DistributedCacheAllKeysInfoGet(cache, RedisCacheConfiguration.dsPeakPayAllKeysInfoHashKey);
            Dictionary<int, string> dictPeakPayWithRedisKey = new Dictionary<int, string>();
            if (parentCaches != null)
                parentCaches.Where(d => solarCompanyIDs.Contains(d.PID) && !string.IsNullOrEmpty(d.Ids)).ToList().ForEach(d =>
                {
                    if (!string.IsNullOrEmpty(d.Ids))
                    {
                        string redisKey = string.Format(RedisCacheConfiguration.dsPeakPayHashKey, d.PID);
                        foreach (var item in d.Ids.Split(','))
                        {
                            int id = 0;
                            int.TryParse(item, out id);
                            if (id > 0 && !dictPeakPayWithRedisKey.ContainsKey(id))
                                dictPeakPayWithRedisKey.Add(id, redisKey);
                        }
                    }
                });

            for (int i = 0; i < decimal.Divide(dictPeakPayWithRedisKey.Count, ProjectConfiguration.RedisCacheBatchCommandCount); i++)
            {
                Dictionary<int, string> keysToConsider = dictPeakPayWithRedisKey.Skip(i * ProjectConfiguration.RedisCacheBatchCommandCount).Take(ProjectConfiguration.RedisCacheBatchCommandCount).ToDictionary(pair => pair.Key, pair => pair.Value);
                List<PeakPayView> lstPeakPayFetch = RedisCacheConfiguration.GetBatchUsingHash<PeakPayView>(cache, keysToConsider);
                if (lstPeakPayFetch != null && lstPeakPayFetch.Count > 0)
                    lstPeakPay.AddRange(lstPeakPayFetch);
            }
            timer.Stop();
            //Helper.Log.WriteLog("BAL : PeakPayDistributedCacheHashGet Method Redis Time : " + timer.ElapsedMilliseconds);
        }

        //Latest by 24May2022
        public static void STCDistributedCacheHashSet(IDatabase cache, string SolarCompanyId, int resellerID, int year, bool forService = false)
        {
            List<int> solarCompanyIds = !string.IsNullOrEmpty(SolarCompanyId) && SolarCompanyId != "0" && SolarCompanyId != "-1" ? SolarCompanyId.Split(',').Select(k => Convert.ToInt32(k)).ToList() : new List<int>();
            DataSet dsAllColumnsData = new DataSet();
            List<STCSubmissionView> lstSTCFull = new List<STCSubmissionView>();
            CommonBAL.STCDistributedCacheHashSet(cache, solarCompanyIds, resellerID, year, ref dsAllColumnsData, ref lstSTCFull, forService);
        }
        //Latest by 09May2022
        public static void STCDistributedCacheHashSet(IDatabase cache, List<int> solarCompanyIDs, int resellerID, int year, ref DataSet dsAllColumnsData, ref List<STCSubmissionView> lstSTC, bool forService = false)
        {
            var timerDB = new Stopwatch();
            var timer = new Stopwatch();
            List<DistributedCacheAllKeysInfoForHashSetView> parentCaches = new List<DistributedCacheAllKeysInfoForHashSetView>();
            string mainKey = year > 0 ? string.Format(RedisCacheConfiguration.dsSTCAllKeysInfoHashKey, year) : RedisCacheConfiguration.dsSTCCERApprovedNotInvoicedAllKeysInfoHashKey;
            lock (myLock)
            {
                timerDB.Start();
                if (forService)
                {
                    dsAllColumnsData = _job.GetJobSTCSubmissionForCacheService((resellerID > 0 || solarCompanyIDs.Count == 0 ? "0" : string.Join(",", solarCompanyIDs)), resellerID, year);
                }
                else
                {
                    dsAllColumnsData = _job.GetJobSTCSubmissionKendoByYear((resellerID > 0 || solarCompanyIDs.Count == 0 ? "0" : string.Join(",", solarCompanyIDs)), resellerID, year);
                }
                timerDB.Stop();
                timer.Start();
                if (dsAllColumnsData.Tables[0] != null && dsAllColumnsData.Tables[0].Rows.Count > 0)
                {
                    lstSTC = DataTableToListSTCSubmission(dsAllColumnsData);
                    var lstSTCToHash = lstSTC.Select(k => new RedisHashSetKeyValuePair()
                    {
                        RedisKey = string.Format(RedisCacheConfiguration.dsSTCHashKey, k.SolarCompanyId),
                        HashKey = k.JobID.Value,
                        HashValuePayload = RedisCacheConfiguration.Serialize(k)
                    }).ToList();

                    for (int i = 0; i < decimal.Divide(lstSTCToHash.Count, ProjectConfiguration.RedisCacheBatchCommandCount); i++)
                    {
                        var stcToHash = lstSTCToHash.Skip(i * ProjectConfiguration.RedisCacheBatchCommandCount).Take(ProjectConfiguration.RedisCacheBatchCommandCount).ToList();
                        RedisCacheConfiguration.SetBatchUsingHash(cache, stcToHash);
                        RedisCacheConfiguration.SetExpireForHash(cache, stcToHash);
                    }
                    var lstSolarCompanyIdToRemoveFromParentCache = lstSTC.Select(d => d.SolarCompanyId).Distinct().ToList();

                    List<DistributedCacheAllKeysInfoForHashSetView> lstSTCParentToHash =
                            (from i in lstSTC
                             group i by i.SolarCompanyId into g
                             select new DistributedCacheAllKeysInfoForHashSetView { PID = g.Key, RedisKey = string.Format(RedisCacheConfiguration.dsSTCHashKey, g.Key), Ids = string.Join(",", g.Where(kvp => kvp.JobID.HasValue).Select(kvp => kvp.JobID)) })
                             .ToList();
                    List<int> lstSTCParentToHashRemaining = solarCompanyIDs.Where(d => !lstSTCParentToHash.Any(k => k.PID == d)).ToList();
                    if (lstSTCParentToHashRemaining.Any())
                    {
                        lstSTCParentToHashRemaining.ForEach(k => { lstSTCParentToHash.Add(new DistributedCacheAllKeysInfoForHashSetView(k, "", "")); });
                        lstSolarCompanyIdToRemoveFromParentCache.AddRange(lstSTCParentToHashRemaining);
                    }
                    parentCaches = DistributedCacheAllKeysInfoGetAndRemoveRequired(cache, lstSolarCompanyIdToRemoveFromParentCache, mainKey);
                    parentCaches.AddRange(lstSTCParentToHash);
                    DistributedCacheAllKeysInfoSet(cache, mainKey, parentCaches);
                }
                else
                {
                    parentCaches = DistributedCacheAllKeysInfoGetAndRemoveRequired(cache, solarCompanyIDs, mainKey);
                    for (int i = 0; i < solarCompanyIDs.Count; i++)
                    {
                        var iSolarID = solarCompanyIDs[i];
                        DistributedCachePrepareAllKeysInfoCacheObject(ref parentCaches, iSolarID, "", "");
                    }
                    DistributedCacheAllKeysInfoSet(cache, mainKey, parentCaches);
                }
                //DistributedCacheAllKeysInfoSet(cache, mainKey, parentCaches);
                timer.Stop();
                Helper.Log.WriteLog("BAL : STCDistributedCacheHashSet Method Key: " + mainKey + " DB Time : " + timerDB.ElapsedMilliseconds + " Redis Timem : " + (timer.ElapsedMilliseconds));
            }
        }
        //Latest by 09May2022
        public static void STCDistributedCacheHashGet(IDatabase cache, List<int> solarCompanyIDs, int year, ref List<STCSubmissionView> lstSTC)
        {
            var timer = new Stopwatch();
            var timer1 = new Stopwatch();
            timer.Start();
            string mainKey = year > 0 ? string.Format(RedisCacheConfiguration.dsSTCAllKeysInfoHashKey, year) : RedisCacheConfiguration.dsSTCCERApprovedNotInvoicedAllKeysInfoHashKey;
            List<DistributedCacheAllKeysInfoForHashSetView> parentCaches = DistributedCacheAllKeysInfoGet(cache, mainKey);
            Dictionary<int, string> dictSTCJobsWithRedisKey = new Dictionary<int, string>();
            parentCaches.Where(d => solarCompanyIDs.Contains(d.PID) && !string.IsNullOrEmpty(d.Ids)).ToList().ForEach(d =>
            {
                if (!string.IsNullOrEmpty(d.Ids))
                {
                    string redisKey = string.Format(RedisCacheConfiguration.dsSTCHashKey, d.PID);
                    foreach (var item in d.Ids.Split(','))
                    {
                        int id = 0;
                        int.TryParse(item, out id);
                        if (id > 0 && !dictSTCJobsWithRedisKey.ContainsKey(id))
                            dictSTCJobsWithRedisKey.Add(id, redisKey);
                    }
                }
            });

            for (int i = 0; i < decimal.Divide(dictSTCJobsWithRedisKey.Count, ProjectConfiguration.RedisCacheBatchCommandCount); i++)
            {
                Dictionary<int, string> keysToConsider = dictSTCJobsWithRedisKey.Skip(i * ProjectConfiguration.RedisCacheBatchCommandCount).Take(ProjectConfiguration.RedisCacheBatchCommandCount).ToDictionary(pair => pair.Key, pair => pair.Value);
                List<STCSubmissionView> lstSTCFetch = RedisCacheConfiguration.GetBatchUsingHash<STCSubmissionView>(cache, keysToConsider);
                if (lstSTCFetch != null && lstSTCFetch.Count > 0)
                    lstSTC.AddRange(lstSTCFetch);
            }

            timer.Stop();
            //Helper.Log.WriteLog("BAL : STCDistributedCacheHashGet Method Redis Time : " + timer.ElapsedMilliseconds);
        }

        public static void JobsDistributedCacheHashSet(IDatabase cache, List<int> solarCompanyIDs, int year)
        {
            DataSet dsAllColumnsData = new DataSet();
            List<JobView> lstJobView = new List<JobView>();
            JobsDistributedCacheHashSet(cache, solarCompanyIDs, year, ref dsAllColumnsData, ref lstJobView);
        }

        //Latest by 27Apr22
        public static void JobsDistributedCacheHashSet(IDatabase cache, List<int> solarCompanyIDs, int year, ref DataSet dsAllColumnsData, ref List<JobView> lstJobs)
         {
            var timerDB = new Stopwatch();
            var timer = new Stopwatch();
            List<DistributedCacheAllKeysInfoForHashSetView> parentCaches = new List<DistributedCacheAllKeysInfoForHashSetView>();
            string mainKey = string.Format(RedisCacheConfiguration.dsJobAllKeysInfoHashKey, year);
            lock (myLock)
            {
                timerDB.Start();
                dsAllColumnsData = _job.GetJobList_ForCachingDataKendoByYear(string.Join(",", solarCompanyIDs), ProjectSession.LoggedInUserId, SystemEnums.MenuId.JobView.GetHashCode(), year);
                timerDB.Stop();
                timer.Start();
                if (dsAllColumnsData.Tables[0] != null && dsAllColumnsData.Tables[0].Rows.Count > 0)
                {
                    List<JobView> lstJobsNew = DataTableToListJobs(dsAllColumnsData);

                    Log.WriteLog($"Got {lstJobsNew.Count()} record from SP");
                    if (lstJobs.Count != 0)
                    {
                        lstJobs.AddRange(lstJobsNew);
                    }
                    else
                    {
                        lstJobs = lstJobsNew;
                    }

                    var lstJobsToHash = lstJobsNew.Select(k => new RedisHashSetKeyValuePair()
                    {
                        RedisKey = string.Format(RedisCacheConfiguration.dsJobHashKey, k.SolarCompanyId),
                        HashKey = k.JobID,
                        HashValuePayload = RedisCacheConfiguration.Serialize(k)
                    }).ToList();
                    

                    for (int i = 0; i < decimal.Divide(lstJobsToHash.Count, ProjectConfiguration.RedisCacheBatchCommandCount); i++)
                    {
                        var jobsToHash = lstJobsToHash.Skip(i * ProjectConfiguration.RedisCacheBatchCommandCount).Take(ProjectConfiguration.RedisCacheBatchCommandCount).ToList();
                        RedisCacheConfiguration.SetBatchUsingHash(cache, jobsToHash);
                        RedisCacheConfiguration.SetExpireForHash(cache, jobsToHash);
                    }

                    List<DistributedCacheAllKeysInfoForHashSetView> lstJobsParentToHash =
                            (from i in lstJobsNew
                             group i by i.SolarCompanyId into g
                             select new DistributedCacheAllKeysInfoForHashSetView { PID = g.Key.Value, RedisKey = string.Format(RedisCacheConfiguration.dsJobHashKey, g.Key), Ids = string.Join(",", g.Select(kvp => kvp.JobID)) })
                             .ToList();
                    
                    List<int> lstJobsParentToHashRemaining = solarCompanyIDs.Where(d => !lstJobsParentToHash.Any(k => k.PID == d)).ToList();
                    if (lstJobsParentToHashRemaining.Any())
                        lstJobsParentToHashRemaining.ForEach(k => { lstJobsParentToHash.Add(new DistributedCacheAllKeysInfoForHashSetView(k, "", "")); });
                    parentCaches = DistributedCacheAllKeysInfoGetAndRemoveRequired(cache, solarCompanyIDs, mainKey);
                    parentCaches.AddRange(lstJobsParentToHash);
                    
                    DistributedCacheAllKeysInfoSet(cache, mainKey, parentCaches);
                }
                else
                {
                    parentCaches = DistributedCacheAllKeysInfoGetAndRemoveRequired(cache, solarCompanyIDs, mainKey);
                    for (int i = 0; i < solarCompanyIDs.Count; i++)
                    {
                        var iSolarID = solarCompanyIDs[i];
                        DistributedCachePrepareAllKeysInfoCacheObject(ref parentCaches, iSolarID, "", "");
                    }
                    DistributedCacheAllKeysInfoSet(cache, mainKey, parentCaches);
                }
                //DistributedCacheAllKeysInfoSet(cache, mainKey, parentCaches);
                timer.Stop();
                //Helper.Log.WriteLog("BAL : JobsDataSplitInGroupRedisSet Method DB Time : " + timerDB.ElapsedMilliseconds + " Redis Timem : " + (timer.ElapsedMilliseconds));
            }
            List<JobsInstallerDesignerView> lstJobsInstallerDesignerView = new List<JobsInstallerDesignerView>();
            List<JobsSolarCompanyView> lstJobsSolarCompanyView = new List<JobsSolarCompanyView>();
            InstallerDesignerDistributedCacheHashSet(cache, ref lstJobsInstallerDesignerView);
            SolarCompanyDistributedCacheHashSet(cache, ref lstJobsSolarCompanyView);
        }

        //Latest by 27Apr22
        public static void JobsDistributedCacheHashGet(IDatabase cache, List<int> solarCompanyIDs, int year, ref List<JobView> lstJobs, bool isJoinWithOthers = false, bool isDefault = true, string strYear = "", int page = 1, int pageSize = 10)
        {
            var timer = new Stopwatch();
            var timer1 = new Stopwatch();
            timer.Start();
            string mainKey = string.Format(RedisCacheConfiguration.dsJobAllKeysInfoHashKey, year);
            List<DistributedCacheAllKeysInfoForHashSetView> parentCaches = DistributedCacheAllKeysInfoGet(cache, mainKey);
            Dictionary<int, string> dictJobsWithRedisKey = new Dictionary<int, string>();
            parentCaches.Where(d => solarCompanyIDs.Contains(d.PID) && !string.IsNullOrEmpty(d.Ids)).ToList().ForEach(d =>
            {
                if (!string.IsNullOrEmpty(d.Ids))
                {
                    string redisKey = string.Format(RedisCacheConfiguration.dsJobHashKey, d.PID);
                    foreach (var item in d.Ids.Split(','))
                    {
                        int id = 0;
                        int.TryParse(item, out id);
                        if (id > 0 && !dictJobsWithRedisKey.ContainsKey(id))
                            dictJobsWithRedisKey.Add(id, redisKey);
                    }
                }
            });

            List<JobView> listJobs = new List<JobView>();
            for (int i = 0; i < decimal.Divide(dictJobsWithRedisKey.Count, ProjectConfiguration.RedisCacheBatchCommandCount); i++)
            {
                Dictionary<int, string> keysToConsider = dictJobsWithRedisKey.Skip(i * ProjectConfiguration.RedisCacheBatchCommandCount).Take(ProjectConfiguration.RedisCacheBatchCommandCount).ToDictionary(pair => pair.Key, pair => pair.Value);
                List<JobView> lstJobsFetch = RedisCacheConfiguration.GetBatchUsingHash<JobView>(cache, keysToConsider);
                if (lstJobsFetch != null && lstJobsFetch.Count > 0)
                    lstJobs.AddRange(lstJobsFetch);
            }
            timer.Stop();
            timer1.Start();
            if (isJoinWithOthers)
                JobsWithInstallerDesignerSolarCompanyJoins(cache, ref lstJobs);
            timer1.Stop();
            //Helper.Log.WriteLog("BAL : JobsDistributedCacheHashGet Method Redis Time : " + timer.ElapsedMilliseconds + " & Inst Designer SolarComp Redis Time: " + timer1.ElapsedMilliseconds);
        }

        public static void JobsWithoutCache(List<int> solarCompanyIDs, int year, ref JobViewLists dsAllColumnsData, string strYear = "", int page = 1, int pageSize = 10, DataTable dtFilter = null, DataTable dtSort = null)
        {
            dsAllColumnsData = _job.GetJobList_ForCachingDataKendoByYearWithoutCacheDapper(string.Join(",", solarCompanyIDs), ProjectSession.LoggedInUserId, SystemEnums.MenuId.JobView.GetHashCode(), Convert.ToInt32(year), strYear, page, pageSize, dtFilter, dtSort);
        }
        
       
        private static DataTable getDataTableFromKendoFilter(KendoFilter filter)
        {
            DataTable table = new DataTable();

            table.Columns.Add("Field");
            table.Columns.Add("Operator");
            table.Columns.Add("Value");
            table.Columns.Add("Logic");
            table.Columns.Add("Group");

            if (filter != null && filter.Filters != null)
            {
                foreach (var item in filter.Filters)
                {
                    if (item.Operator != null)
                    {
                        DataRow dataRow = table.NewRow();
                        dataRow["Field"] = item.Field;
                        dataRow["Operator"] = item.Operator;
                        dataRow["Value"] = item.Value;
                        dataRow["Logic"] = item.Logic;
                        dataRow["Group"] = null;

                        table.Rows.Add(dataRow);
                    }
                    else
                    {
                        Guid guid = Guid.NewGuid();
                        foreach (var item1 in item.Filters)
                        {
                            DataRow dataRow = table.NewRow();
                            dataRow["Field"] = item1.Field;
                            dataRow["Operator"] = item1.Operator;
                            dataRow["Value"] = item1.Value;
                            dataRow["Logic"] = item1.Logic;
                            dataRow["Group"] = guid;

                            table.Rows.Add(dataRow);
                        }
                    }
                }
            }

            return table;
        }

        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            DataTable table = new DataTable();
            PropertyInfo[] props = typeof(T).GetProperties();

            if (data?.Count > 0)
            {
                foreach (PropertyInfo prop in props)
                {
                   table.Columns.Add(prop.Name);
                }


                foreach (T item in data)
                {
                    var values = new object[props.Length];

                    for (int i = 0; i < props.Length; i++)
                    {
                        values[i] = props[i].GetValue(item, null);
                    }
                    table.Rows.Add(values);
                }
            }
            return table;
        }
            //Latest by 27Apr22
            public static List<JobView> JobsDistributedCacheHashGetUsingJobIds(IDatabase cache, DataTable dt)
        {
            List<JobView> listJobs = new List<JobView>();
            Dictionary<int, string> dictJobsWithRedisKey = new Dictionary<int, string>();

            dt.AsEnumerable().ToList().ForEach(d =>
            {
                int solarCompanyId = d.Field<int>("SolarCompanyId");
                int jobId = d.Field<int>("JobId");
                if (solarCompanyId > 0 && jobId > 0)
                {
                    string redisKey = string.Format(RedisCacheConfiguration.dsJobHashKey, solarCompanyId);
                    dictJobsWithRedisKey.Add(jobId, redisKey);
                }
            });

            for (int i = 0; i < decimal.Divide(dictJobsWithRedisKey.Count, ProjectConfiguration.RedisCacheBatchCommandCount); i++)
            {
                Dictionary<int, string> keysToConsider = dictJobsWithRedisKey.Skip(i * ProjectConfiguration.RedisCacheBatchCommandCount).Take(ProjectConfiguration.RedisCacheBatchCommandCount).ToDictionary(pair => pair.Key, pair => pair.Value);
                List<JobView> lstJobsFetch = RedisCacheConfiguration.GetBatchUsingHash<JobView>(cache, keysToConsider);
                if (lstJobsFetch != null && lstJobsFetch.Count > 0)
                    listJobs.AddRange(lstJobsFetch);
            }
            JobsWithInstallerDesignerSolarCompanyJoins(cache, ref listJobs);
            return listJobs;
        }

        //Latet by 06May22
        public static void JobsWithInstallerDesignerSolarCompanyJoins(IDatabase cache, ref List<JobView> lstJobs)
        {
            List<JobsInstallerDesignerView> lstJobsInstallerDesignerViews = new List<JobsInstallerDesignerView>();
            List<JobsSolarCompanyView> lstJobSolarCompanyView = new List<JobsSolarCompanyView>();
            InstallerDesignerDistributedCacheHashGetAll(cache, ref lstJobsInstallerDesignerViews);
            SolarCompanyDistributedCacheHashGetAll(cache, ref lstJobSolarCompanyView);
            JobsWithInstallerDesignerSolarCompanyPrepareList(ref lstJobs, lstJobsInstallerDesignerViews, lstJobSolarCompanyView);
        }

        public static void JobsWithInstallerDesignerSolarCompanyPrepareList(ref List<JobView> lstJobs, List<JobsInstallerDesignerView> lstJobsInstallerDesignerViews, List<JobsSolarCompanyView> lstJobSolarCompanyView)
        {
            lstJobs = (from j in lstJobs
                       join idi in lstJobsInstallerDesignerViews
                           on j.InstallerID equals idi.InstallerDesignerID into idiResult
                       from idi in idiResult.DefaultIfEmpty()
                       join idd in lstJobsInstallerDesignerViews
                           on j.DesignerID equals idd.InstallerDesignerID into iddResult
                       from idd in iddResult.DefaultIfEmpty()
                       join sc in lstJobSolarCompanyView
                           on j.SolarCompanyId equals sc.SolarCompanyId into scResult
                       from sc in scResult.DefaultIfEmpty()
                       select new JobView
                       {
                           CalculatedSTC = j.CalculatedSTC,
                           ClientName = j.ClientName,
                           ColorCode = j.ColorCode,
                           CreatedBy = j.CreatedBy,
                           CreatedDate = j.CreatedDate,
                           DeemingPeriod = j.DeemingPeriod,
                           DesignerAccreditationNumber = idd != null ? idd.InstallerDesignerAccreditationNumber : "",
                           DesignerFirstName = idd != null ? idd.InstallerDesignerFirstName : "",
                           DesignerFullAddress = idd != null ? idd.InstallerDesignerFullAddress : "",
                           DesignerFullName = idd != null ? idd.InstallerDesignerFullName : "",
                           DesignerLastName = idd != null ? idd.InstallerDesignerLastName : "",
                           DesignerLicenseNumber = idd != null ? idd.InstallerDesignerLicenseNumber : "",
                           DesignerPostCode = idd != null ? idd.InstallerDesignerPostCode : "",
                           DesignerState = idd != null ? idd.InstallerDesignerState : "",
                           DesignerStreetName = idd != null ? idd.InstallerDesignerStreetName : "",
                           DesignerStreetNumber = idd != null ? idd.InstallerDesignerStreetNumber : "",
                           DesignerStreetTypeID = idd != null ? idd.InstallerDesignerStreetTypeID : "",
                           DesignerTown = idd != null ? idd.InstallerDesignerTown : "",
                           DesignerUnitNumber = idd != null ? idd.InstallerDesignerUnitNumber : "",
                           DesignerUnitTypeID = idd != null ? idd.InstallerDesignerUnitTypeID : "",
                           Distributor = j.Distributor,
                           DocumentCount = j.DocumentCount,
                           ElectricianCompanyName = j.ElectricianCompanyName,
                           ElectricianFirstName = j.ElectricianFirstName,
                           ElectricianFullAddress = j.ElectricianFullAddress,
                           ElectricianFullName = j.ElectricianFullName,
                           ElectricianLastName = j.ElectricianLastName,
                           ElectricianLicenseNumber = j.ElectricianLicenseNumber,
                           ElectricianPostCode = j.ElectricianPostCode,
                           ElectricianState = j.ElectricianState,
                           ElectricianStreetName = j.ElectricianStreetName,
                           ElectricianStreetNumber = j.ElectricianStreetNumber,
                           ElectricianStreetTypeID = j.ElectricianStreetTypeID,
                           ElectricianTown = j.ElectricianTown,
                           ElectricianUnitNumber = j.ElectricianUnitNumber,
                           ElectricianUnitTypeID = j.ElectricianUnitTypeID,
                           ElectricityProvider = j.ElectricityProvider,
                           FullBasicDetails = j.FullBasicDetails,
                           FullInstallationAddress = j.FullInstallationAddress,
                           FullOwnerAddress = j.FullOwnerAddress,
                           FullOwnerCompanyDetails = j.FullOwnerCompanyDetails,
                           FullOwnerName = j.FullOwnerName,
                           FullPanelDetails = j.FullPanelDetails,
                           GSTDocument = j.GSTDocument,
                           InstallationDate = j.InstallationDate,
                           InstallationPostCode = j.InstallationPostCode,
                           InstallationState = j.InstallationState,
                           InstallationStreetName = j.InstallationStreetName,
                           InstallationStreetNumber = j.InstallationStreetNumber,
                           InstallationStreetTypeID = j.InstallationStreetTypeID,
                           InstallationTown = j.InstallationTown,
                           InstallationType = j.InstallationType,
                           InstallationUnitNumber = j.InstallationUnitNumber,
                           InstallationUnitTypeID = j.InstallationUnitTypeID,
                           InstallerAccreditationNumber = idi != null ? idi.InstallerDesignerAccreditationNumber : "",
                           InstallerFirstName = idi != null ? idi.InstallerDesignerFirstName : "",
                           InstallerFullAddress = idi != null ? idi.InstallerDesignerFullAddress : "",
                           InstallerFullName = idi != null ? idi.InstallerDesignerFullName : "",
                           InstallerLastName = idi != null ? idi.InstallerDesignerLastName : "",
                           InstallerLicenseNumber = idi != null ? idi.InstallerDesignerLicenseNumber : "",
                           InstallerPostCode = idi != null ? idi.InstallerDesignerPostCode : "",
                           InstallerState = idi != null ? idi.InstallerDesignerState : "",
                           InstallerStreetName = idi != null ? idi.InstallerDesignerStreetName : "",
                           InstallerStreetNumber = idi != null ? idi.InstallerDesignerStreetNumber : "",
                           InstallerStreetTypeID = idi != null ? idi.InstallerDesignerStreetTypeID : "",
                           InstallerTown = idi != null ? idi.InstallerDesignerTown : "",
                           InstallerUnitNumber = idi != null ? idi.InstallerDesignerUnitNumber : "",
                           InstallerUnitTypeID = idi != null ? idi.InstallerDesignerUnitTypeID : "",
                           IsAccept = j.IsAccept,
                           IsBasicValidation = j.IsBasicValidation,
                           IsCESForm = j.IsCESForm,
                           IsConnectionCompleted = j.IsConnectionCompleted,
                           IsCustPrice = j.IsCustPrice,
                           IsDeleted = j.IsDeleted,
                           IsGroupSiganture = j.IsGroupSiganture,
                           IsGst = j.IsGst,
                           IsInstallerSiganture = j.IsInstallerSiganture,
                           IsInvoiced = j.IsInvoiced,
                           IsOwnerSiganture = j.IsOwnerSiganture,
                           IsPreApprovaApproved = j.IsPreApprovaApproved,
                           IsSerialNumberCheck = j.IsSerialNumberCheck,
                           IsSTCForm = j.IsSTCForm,
                           IsSTCSubmissionPhotos = j.IsSTCSubmissionPhotos,
                           IsTraded = j.IsTraded,
                           JobAddress = j.JobAddress,
                           JobDescription = j.JobDescription,
                           JobID = j.JobID,
                           JobNumber = j.JobNumber,
                           JobStage = j.JobStage,
                           JobStageChangeDate = j.JobStageChangeDate,
                           JobTitle = j.JobTitle,
                           JobTypeId = j.JobTypeId,
                           MeterNumber = j.MeterNumber,
                           NMI = j.NMI,
                           NoOfPanel = j.NoOfPanel,
                           OwnerEmail = j.OwnerEmail,
                           OwnerMobile = j.OwnerMobile,
                           OwnerPhone = j.OwnerPhone,
                           OwnerPostCode = j.OwnerPostCode,
                           OwnerState = j.OwnerState,
                           OwnerStreetName = j.OwnerStreetName,
                           OwnerStreetNumber = j.OwnerStreetNumber,
                           OwnerStreetTypeID = j.OwnerStreetTypeID,
                           OwnerTown = j.OwnerTown,
                           OwnerType = j.OwnerType,
                           OwnerUnitNumber = j.OwnerUnitNumber,
                           OwnerUnitTypeID = j.OwnerUnitTypeID,
                           PhaseProperty = j.PhaseProperty,
                           Priority = j.Priority,
                           PropertyType = j.PropertyType,
                           RefNumber = j.RefNumber,
                           SCOName = j.SCOName,
                           ScoUserId = j.ScoUserId,
                           SerialNumbers = j.SerialNumbers,
                           SolarCompany = sc != null ? sc.SolarCompany : "",
                           SolarCompanyABN = sc != null ? sc.SolarCompanyABN : "",
                           SolarCompanyAccreditationNumber = sc != null ? sc.SolarCompanyAccreditationNumber : "",
                           SolarCompanyFirstName = sc != null ? sc.SolarCompanyFirstName : "",
                           SolarCompanyFullAddress = sc != null ? sc.SolarCompanyFullAddress : "",
                           SolarCompanyFullName = sc != null ? sc.SolarCompanyFullName : "",
                           SolarCompanyId = j.SolarCompanyId,
                           SolarCompanyLastName = sc != null ? sc.SolarCompanyLastName : "",
                           SolarCompanyName = sc != null ? sc.SolarCompanyName : "",
                           SolarCompanyPostCode = sc != null ? sc.SolarCompanyPostCode : "",
                           SolarCompanyState = sc != null ? sc.SolarCompanyState : "",
                           SolarCompanyStreetName = sc != null ? sc.SolarCompanyStreetName : "",
                           SolarCompanyStreetNumber = sc != null ? sc.SolarCompanyStreetNumber : "",
                           SolarCompanyStreetTypeID = sc != null ? sc.SolarCompanyStreetTypeID : "",
                           SolarCompanyTown = sc != null ? sc.SolarCompanyTown : "",
                           SolarCompanyUnitNumber = sc != null ? sc.SolarCompanyUnitNumber : "",
                           SolarCompanyUnitTypeID = sc != null ? sc.SolarCompanyUnitTypeID : "",
                           SSCID = j.SSCID,
                           StaffName = j.StaffName,
                           STC = j.STC,
                           SystemMountingType = j.SystemMountingType,
                           SystemSize = j.SystemSize,
                           TradeStatus = j.TradeStatus,
                           TypeOfConnection = j.TypeOfConnection,
                           InstallerID = j.InstallerID,
                           DesignerID = j.DesignerID,
                           PanelBrand = j.PanelBrand,
                           PanelModel = j.PanelModel,
                           InverterBrand = j.InverterBrand,
                           InverterModel = j.InverterModel,
                           InverterSeries = j.InverterSeries,
                       }).ToList();
        }


        //Latet by 06May22
        public static void JobsFilteredWithInstallerDesignerSolarCompanyJoins(IDatabase cache, ref List<JobView> lstJobs)
        {
            if (lstJobs == null || lstJobs.Count == 0)
                return;

            List<JobsInstallerDesignerView> lstJobsInstallerDesignerViews = new List<JobsInstallerDesignerView>();
            List<JobsSolarCompanyView> lstJobSolarCompanyView = new List<JobsSolarCompanyView>();
            List<int> lstInstallerDesignerIds = lstJobs.Where(k => k.InstallerID.HasValue).Select(k => k.InstallerID.Value).
                Union(lstJobs.Where(k => k.DesignerID.HasValue).Select(k => k.DesignerID.Value)).Distinct().ToList();
            var lstSolarCompanyIds = lstJobs.Where(k => k.SolarCompanyId.HasValue).Select(k => k.SolarCompanyId.Value).Distinct().ToList();
            InstallerDesignerDistributedCacheHashGetByIds(cache, lstInstallerDesignerIds, ref lstJobsInstallerDesignerViews);
            SolarCompanyDistributedCacheHashGetByIds(cache, lstSolarCompanyIds, ref lstJobSolarCompanyView);
            JobsWithInstallerDesignerSolarCompanyPrepareList(ref lstJobs, lstJobsInstallerDesignerViews, lstJobSolarCompanyView);
        }

        //Latest by 04May22
        public static void InstallerDesignerDistributedCacheHashSet(IDatabase cache, ref List<JobsInstallerDesignerView> lstJobsInstallerDesignerViews)
        {
            IJobDetailsBAL _jobDetailsBAL = new JobDetailsBAL();
            var timerDB = new Stopwatch();
            var timer = new Stopwatch();
            lock (myLock)
            {
                timerDB.Start();
                DataSet dsAllData = _jobDetailsBAL.GetAllInstallerDesigner();
                timerDB.Stop();

                if (dsAllData.Tables[0] != null && dsAllData.Tables[0].Rows.Count > 0)
                {
                    lstJobsInstallerDesignerViews = DataTableToListInstallerDesigner(dsAllData);
                    timer.Start();
                    var lstItemsToHash = lstJobsInstallerDesignerViews.Select(k => new RedisHashSetKeyValuePair()
                    {
                        RedisKey = RedisCacheConfiguration.dsInstallerDesignerHashKey,
                        HashKey = k.InstallerDesignerID,
                        HashValuePayload = RedisCacheConfiguration.Serialize(k)
                    }).ToList();

                    for (int i = 0; i < decimal.Divide(lstItemsToHash.Count, ProjectConfiguration.RedisCacheBatchCommandCount); i++)
                    {
                        var jobsToHash = lstItemsToHash.Skip(i * ProjectConfiguration.RedisCacheBatchCommandCount).Take(ProjectConfiguration.RedisCacheBatchCommandCount).ToList();
                        RedisCacheConfiguration.SetBatchUsingHash(cache, jobsToHash);
                        RedisCacheConfiguration.SetExpireForHash(cache, jobsToHash);
                    }
                }
                timer.Stop();
                //Helper.Log.WriteLog("BAL : InstallerDesignerDistributedCacheHashSet Method DB Time : " + timerDB.ElapsedMilliseconds + " Redis Time : " + (timer.ElapsedMilliseconds));
            }
        }

        //Latest by 04May22
        public static void InstallerDesignerDistributedCacheHashGetAll(IDatabase cache, ref List<JobsInstallerDesignerView> lstJobsInstallerDesignerViews)
        {
            var timer = new Stopwatch();
            timer.Start();
            lstJobsInstallerDesignerViews = RedisCacheConfiguration.GetAllBatchUsingHash<JobsInstallerDesignerView>(cache, RedisCacheConfiguration.dsInstallerDesignerHashKey);
            if (lstJobsInstallerDesignerViews == null || lstJobsInstallerDesignerViews.Count == 0)
                InstallerDesignerDistributedCacheHashSet(cache, ref lstJobsInstallerDesignerViews);
            timer.Stop();

            //Helper.Log.WriteLog("BAL : InstallerDesignerDistributedCacheHashGetAll Method Redis Time : " + timer.ElapsedMilliseconds + " & Rows Count : " + lstJobsInstallerDesignerViews.Count);
        }

        //Latest by 05May22
        public static void InstallerDesignerDistributedCacheHashGetByIds(IDatabase cache, List<int> installerDesignerIds, ref List<JobsInstallerDesignerView> lstJobsInstallerDesignerViews)
        {
            var timer = new Stopwatch();
            timer.Start();
            Dictionary<int, string> dictJobsWithRedisKey = new Dictionary<int, string>();
            if (installerDesignerIds != null && installerDesignerIds.Count > 0)
            {
                installerDesignerIds.ForEach(k =>
                {
                    if (k > 0 && !dictJobsWithRedisKey.ContainsKey(k))
                        dictJobsWithRedisKey.Add(k, RedisCacheConfiguration.dsInstallerDesignerHashKey);
                });
            }

            for (int i = 0; i < decimal.Divide(dictJobsWithRedisKey.Count, ProjectConfiguration.RedisCacheBatchCommandCount); i++)
            {
                Dictionary<int, string> keysToConsider = dictJobsWithRedisKey.Skip(i * ProjectConfiguration.RedisCacheBatchCommandCount).Take(ProjectConfiguration.RedisCacheBatchCommandCount).ToDictionary(pair => pair.Key, pair => pair.Value);
                List<JobsInstallerDesignerView> lstJobsInstallerDesignerViewsFetch = RedisCacheConfiguration.GetBatchUsingHash<JobsInstallerDesignerView>(cache, keysToConsider);
                if (lstJobsInstallerDesignerViewsFetch != null && lstJobsInstallerDesignerViewsFetch.Count > 0)
                    lstJobsInstallerDesignerViews.AddRange(lstJobsInstallerDesignerViewsFetch);
            }

            if (lstJobsInstallerDesignerViews == null || lstJobsInstallerDesignerViews.Count == 0)
                InstallerDesignerDistributedCacheHashSet(cache, ref lstJobsInstallerDesignerViews);
            timer.Stop();

            //Helper.Log.WriteLog("BAL : InstallerDesignerDistributedCacheHashGetByIds Method Redis Time : " + timer.ElapsedMilliseconds + " & Rows Count : " + lstJobsInstallerDesignerViews.Count);
        }

        //Latest by 05May22
        public static void SolarCompanyDistributedCacheHashSet(IDatabase cache, ref List<JobsSolarCompanyView> lstJobsSolarCompanyViews)
        {
            IJobDetailsBAL _jobDetailsBAL = new JobDetailsBAL();
            var timerDB = new Stopwatch();
            var timer = new Stopwatch();
            lock (myLock)
            {
                timerDB.Start();
                DataSet dsAllData = _jobDetailsBAL.GetAllSolarCompanies();
                timerDB.Stop();
                if (dsAllData.Tables[0] != null && dsAllData.Tables[0].Rows.Count > 0)
                {
                    lstJobsSolarCompanyViews = DataTableToListSolarCompany(dsAllData);
                    timer.Start();
                    var lstItemsToHash = lstJobsSolarCompanyViews.Select(k => new RedisHashSetKeyValuePair()
                    {
                        RedisKey = RedisCacheConfiguration.dsSolarCompanyHashKey,
                        HashKey = k.SolarCompanyId,
                        HashValuePayload = RedisCacheConfiguration.Serialize(k)
                    }).ToList();

                    for (int i = 0; i < decimal.Divide(lstItemsToHash.Count, ProjectConfiguration.RedisCacheBatchCommandCount); i++)
                    {
                        var jobsToHash = lstItemsToHash.Skip(i * ProjectConfiguration.RedisCacheBatchCommandCount).Take(ProjectConfiguration.RedisCacheBatchCommandCount).ToList();
                        RedisCacheConfiguration.SetBatchUsingHash(cache, jobsToHash);
                        RedisCacheConfiguration.SetExpireForHash(cache, jobsToHash);
                    }
                }
                timer.Stop();
                //Helper.Log.WriteLog("BAL : SolarCompanyDistributedCacheHashSet Method DB Time : " + timerDB.ElapsedMilliseconds + " Redis Time : " + (timer.ElapsedMilliseconds));
            }
        }

        //Latest by 05May22
        public static void SolarCompanyDistributedCacheHashGetAll(IDatabase cache, ref List<JobsSolarCompanyView> lstJobsSolarCompanyViews)
        {
            var timer = new Stopwatch();
            timer.Start();
            lstJobsSolarCompanyViews = RedisCacheConfiguration.GetAllBatchUsingHash<JobsSolarCompanyView>(cache, RedisCacheConfiguration.dsSolarCompanyHashKey);
            if (lstJobsSolarCompanyViews == null || lstJobsSolarCompanyViews.Count == 0)
                SolarCompanyDistributedCacheHashSet(cache, ref lstJobsSolarCompanyViews);
            timer.Stop();

            //Helper.Log.WriteLog("BAL : SolarCompanyDistributedCacheHashGetAll Method Redis Time : " + timer.ElapsedMilliseconds + " & Rows Count : " + lstJobsSolarCompanyViews.Count);
        }

        //Latest by 05May22
        public static void SolarCompanyDistributedCacheHashGetByIds(IDatabase cache, List<int> solarCompanyIds, ref List<JobsSolarCompanyView> lstJobsSolarCompanyViews)
        {
            var timer = new Stopwatch();
            timer.Start();
            Dictionary<int, string> dictWithRedisKey = new Dictionary<int, string>();
            if (solarCompanyIds != null && solarCompanyIds.Count > 0)
            {
                solarCompanyIds.ForEach(k =>
                {
                    if (k > 0 && !dictWithRedisKey.ContainsKey(k))
                        dictWithRedisKey.Add(k, RedisCacheConfiguration.dsSolarCompanyHashKey);
                });
            }

            for (int i = 0; i < decimal.Divide(dictWithRedisKey.Count, ProjectConfiguration.RedisCacheBatchCommandCount); i++)
            {
                Dictionary<int, string> keysToConsider = dictWithRedisKey.Skip(i * ProjectConfiguration.RedisCacheBatchCommandCount).Take(ProjectConfiguration.RedisCacheBatchCommandCount).ToDictionary(pair => pair.Key, pair => pair.Value);
                List<JobsSolarCompanyView> lstJobsSolarCompanyViewsFetch = RedisCacheConfiguration.GetBatchUsingHash<JobsSolarCompanyView>(cache, keysToConsider);
                if (lstJobsSolarCompanyViewsFetch != null && lstJobsSolarCompanyViewsFetch.Count > 0)
                    lstJobsSolarCompanyViews.AddRange(lstJobsSolarCompanyViewsFetch);
            }

            if (lstJobsSolarCompanyViews == null || lstJobsSolarCompanyViews.Count == 0)
                SolarCompanyDistributedCacheHashSet(cache, ref lstJobsSolarCompanyViews);
            timer.Stop();

            //Helper.Log.WriteLog("BAL : SolarCompanyDistributedCacheHashGetByIds Method Redis Time : " + timer.ElapsedMilliseconds + " & Rows Count : " + lstJobsSolarCompanyViews.Count);
        }
        #endregion

        #region Get STC Status Count
        public static List<JobStage> GetSTCCount(int resellerId, int ramId, int solarCompanyId, string isAllScaJobView, string isShowOnlyAssignJobsSCO = "", int year = 0)
        {
            if (year >= ProjectConfiguration.ArchiveMinYear)
                return _job.GetSTCJobStagesWithCountByYearForCERApproved(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, resellerId, ramId, solarCompanyId, isAllScaJobView, isShowOnlyAssignJobsSCO, year);
            else
                return _job.GetSTCJobStagesWithCountForCERNotApproved(ProjectSession.LoggedInUserId, ProjectSession.UserTypeId, resellerId, ramId, solarCompanyId, isAllScaJobView, isShowOnlyAssignJobsSCO);
        }
        #endregion

        #region Set Cache Data Methods
        public async static Task SetCacheDataForInstallerDesignerID(int installerDesignerID)
        {
            try
            {
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                IJobDetailsBAL _job = new JobDetailsBAL();
                JobsInstallerDesignerView jobsInstallerDesignerView = _job.GetAllInstallerDesignerById(installerDesignerID);
                if (jobsInstallerDesignerView != null)
                {
                    var payLoad = RedisCacheConfiguration.Serialize(jobsInstallerDesignerView);
                    RedisCacheConfiguration.SetHashAsync(cache, RedisCacheConfiguration.dsInstallerDesignerHashKey, installerDesignerID, payLoad);
                }
            }
            catch (Exception e)
            {
                Helper.Log.WriteError(e, "Exception in SetCacheDataForInstallerDesignerID:");
            }
        }
        #endregion

        public async static Task SetCacheDataForJobID(int SolarCompanyId, int JobID, int LoggedInUserId = 0)
        {
            try
            {
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                ICreateJobBAL _job = new CreateJobBAL();
                DataRow dr = _job.GetJobIDWiseCachingData(Convert.ToString(JobID));
                if (dr != null)
                    SolarCompanyId = Convert.ToInt32(dr["SolarCompanyId"]);
                else
                    SolarCompanyId = 0;

                if (SolarCompanyId > 0 && JobID > 0)
                {
                    var lstJobView = DataTableToListJobs(dr.Table);
                    if (lstJobView != null && lstJobView.Any())
                    {
                        var jobView = lstJobView.First();
                        var payLoad = RedisCacheConfiguration.Serialize(jobView);
                        var isInsert = RedisCacheConfiguration.SetHash(cache, string.Format(RedisCacheConfiguration.dsJobHashKey, SolarCompanyId), JobID, payLoad);
                        if (isInsert)
                        {
                            int year = jobView.InstallationDate.HasValue ? jobView.InstallationDate.Value.Year : jobView.CreatedDate.Year;
                            string mainRedisKey = string.Format(RedisCacheConfiguration.dsJobAllKeysInfoHashKey, year);
                            bool isCreateCacheForWholeSolarCompany = false;

                            lock (myLock)
                            {
                                List<DistributedCacheAllKeysInfoForHashSetView> mainJobCache = CommonBAL.DistributedCacheAllKeysInfoGet(cache, mainRedisKey);
                                if (mainJobCache != null && mainJobCache.Any(d => d.PID == SolarCompanyId))
                                {
                                    var jobCache = mainJobCache.FirstOrDefault(d => d.PID == SolarCompanyId);
                                    string ids = string.Join(",", jobCache.Ids, Convert.ToString(JobID));
                                    jobCache.Ids = string.Join(",", ids.Split(',').ToList().Distinct());
                                    DistributedCacheAllKeysInfoSet(cache, mainRedisKey, mainJobCache);
                                }
                                else
                                    isCreateCacheForWholeSolarCompany = true;
                            }

                            if (isCreateCacheForWholeSolarCompany)
                                JobsDistributedCacheHashSet(cache, new List<int>() { SolarCompanyId }, year);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Helper.Log.WriteError(e, "Exception in SetCacheDataForJobID:");
            }
        }

        public static void WriteToLogFile(string errorMsg, string methodName, string ID, bool IsFromVendorAPIWriteLog = true)
        {
            FileStream fs = null;
            try
            {
                //set up a filestream
                //fs = new FileStream(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6) + "\\FormBotLog.txt", FileMode.OpenOrCreate, FileAccess.Write);
                if (IsFromVendorAPIWriteLog)
                    fs = new FileStream(FormBot.Helper.ProjectSession.VendorAPIErrorLogs, FileMode.OpenOrCreate, FileAccess.Write);
                else
                    fs = new FileStream(FormBot.Helper.ProjectSession.LogFilePath, FileMode.OpenOrCreate, FileAccess.Write);

                //set up a streamwriter for adding text
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    //add the text
                    sw.WriteLine("Date:" + DateTime.Now + " MethodName:" + methodName + " " + ID + " Error:" + errorMsg);
                    //add the text to the underlying filestream
                    sw.Flush();
                    //close the writer
                    sw.Close();
                }
            }
            catch (Exception)
            {
                fs.Dispose();
                //throw;
            }
            //StreamWriter sw = new StreamWriter(fs);
            //find the end of the underlying filestream            
        }

        /// <summary>
        /// Update cache in Stc submission page
        /// </summary>
        /// <param name="StcJobDetailsId"></param>
        /// <param name="SolarCompanyId"></param>
        public async static Task SetCacheDataForSTCSubmission(int? StcJobDetailsId, int? JobId = null, SortedList<string, string> data = null, bool isResetParentCache = false, bool isMainToArchiveValidate = false, bool isArchiveToMainValidate = false)
        {
            try
            {
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                ICreateJobBAL _job = new CreateJobBAL();
                DataTable dataTable = _job.GetResellerSolarCompnayFromJobIdOrStcJobdetailId(JobId, StcJobDetailsId);

                if (dataTable.Rows.Count > 0 && dataTable.Rows[0]["STCJobDetailsId"].ToString() != "")
                {
                    DataRow dr = dataTable.Rows[0];
                    int solarCompanyId = Convert.ToInt32(dr["SolarCompnayId"]);
                    int jobID = Convert.ToInt32(dr["JobId"]);
                    int stcJobdetailId = Convert.ToInt32(dr["STCJobDetailsId"]);
                    int year = Convert.ToInt32(dr["JobYear"]);
                    STCSubmissionView obj = new STCSubmissionView();

                    if (dr == null)
                        dr = _job.GetSTCJobDetailsIDWiseCachingData(stcJobdetailId);

                    string redisKey = string.Format(RedisCacheConfiguration.dsSTCHashKey, solarCompanyId);
                    List<STCSubmissionView> lstStcView = RedisCacheConfiguration.GetHash<STCSubmissionView>(cache, redisKey, jobID);
                    if (lstStcView.Any())
                    {
                        var stcView = lstStcView.First();
                        for (int i = 0; i < data.Keys.Count; i++)
                        {
                            SetPropValue(stcView, data.Keys[i], data.Values[i]);
                        }
                        var payLoad = RedisCacheConfiguration.Serialize(stcView);
                        RedisCacheConfiguration.SetHashAsync(cache, redisKey, jobID, payLoad);

                        if (isResetParentCache)
                        {
                            #region Logic to manage STC Main Vs Archived
                            bool isInvoiced = stcView.IsInvoiced.HasValue ? stcView.IsInvoiced.Value : false;
                            int stcStatusId = stcView.STCStatusId.Value;
                            int iYear = stcView.InstallationDate.HasValue ? stcView.InstallationDate.Value.Year : stcView.CreatedDate.Year;

                            if (isMainToArchiveValidate && (stcStatusId != SystemEnums.STCJobStatus.CERApproved.GetHashCode() ||
                                (stcStatusId == SystemEnums.STCJobStatus.CERApproved.GetHashCode() && !isInvoiced)))
                                return;

                            if (isArchiveToMainValidate && (isInvoiced))
                                return;

                            string mainRedisKey = RedisCacheConfiguration.dsSTCCERApprovedNotInvoicedAllKeysInfoHashKey; // string.Format(RedisCacheConfiguration.dsSTCAllKeysInfoHashKey, year);
                            string mainRedisKeyArchive = string.Format(RedisCacheConfiguration.dsSTCAllKeysInfoHashKey, iYear);
                            //List<DistributedCacheAllKeysInfoForHashSetView> mainSTCCache = CommonBAL.DistributedCacheAllKeysInfoGet(cache, mainRedisKey);
                            //List<DistributedCacheAllKeysInfoForHashSetView> mainSTCCacheArchive = CommonBAL.DistributedCacheAllKeysInfoGet(cache, mainRedisKeyArchive);
                            if (stcStatusId == SystemEnums.STCJobStatus.CERApproved.GetHashCode() && isInvoiced)
                            {
                                // remove from main and insert to archive
                                RemoveIdsFromParentCache(cache, mainRedisKey, solarCompanyId, jobID);
                                bool isExists = InsertIdToParentCache(cache, mainRedisKeyArchive, solarCompanyId, jobID);
                                if (!isExists)
                                    STCDistributedCacheHashSet(cache, solarCompanyId.ToString(), 0, iYear);
                            }
                            else
                            {
                                // remove from archive and insert to main
                                RemoveIdsFromParentCache(cache, mainRedisKeyArchive, solarCompanyId, jobID);
                                bool isExists = InsertIdToParentCache(cache, mainRedisKey, solarCompanyId, jobID);
                                if (!isExists)
                                    STCDistributedCacheHashSet(cache, solarCompanyId.ToString(), 0, 0);
                            }
                            #endregion
                        }
                    }
                    else
                        STCDistributedCacheHashSet(cache, solarCompanyId.ToString(), 0, year);
                }
            }
            catch (Exception e)
            {
                Helper.Log.WriteError(e, "Exception in setcacheDataForStcSubmission:");
            }
        }

        private static void RemoveIdsFromParentCache(IDatabase cache, string mainRedisKey, int solarCompanyId, int jobId)
        {
            lock (myLock)
            {
                List<DistributedCacheAllKeysInfoForHashSetView> mainSTCCache = CommonBAL.DistributedCacheAllKeysInfoGet(cache, mainRedisKey);
                if (mainSTCCache != null && mainSTCCache.Any(d => d.PID == solarCompanyId))
                {
                    var stcCache = mainSTCCache.FirstOrDefault(d => d.PID == solarCompanyId);
                    List<string> ids = stcCache.Ids.Split(',').ToList();
                    ids.Remove(jobId.ToString());
                    stcCache.Ids = string.Join(",", ids.Distinct());
                    DistributedCacheAllKeysInfoSet(cache, mainRedisKey, mainSTCCache);
                }
            }
        }

        private static bool InsertIdToParentCache(IDatabase cache, string mainRedisKey, int solarCompanyId, int jobId)
        {
            bool isExists = false;
            lock (myLock)
            {
                List<DistributedCacheAllKeysInfoForHashSetView> mainSTCCache = CommonBAL.DistributedCacheAllKeysInfoGet(cache, mainRedisKey);
                if (mainSTCCache != null && mainSTCCache.Any(d => d.PID == solarCompanyId))
                {
                    var stcCache = mainSTCCache.FirstOrDefault(d => d.PID == solarCompanyId);
                    string ids = String.Join(",", stcCache.Ids, Convert.ToString(jobId));
                    stcCache.Ids = string.Join(",", ids.Split(',').ToList().Distinct());
                    DistributedCacheAllKeysInfoSet(cache, mainRedisKey, mainSTCCache);
                    isExists = true;
                }
            }
            return isExists;
        }

        /// <summary>
        /// update cache of all solarcompany of reseller or specific solar company wise
        /// </summary>
        /// <param name="ResellerId"></param>
        /// <param name="SolarCompanyId"></param>
        public async static Task SetCacheDataForSTCSubmissionFromSolarCompanyId(int ResellerId, string SolarCompanyId = "0", int year = 0)
        {
            try
            {
                //ValidateYearForGridData(ref year);
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                CommonBAL.STCDistributedCacheHashSet(cache, SolarCompanyId, ResellerId, year);
            }
            catch (Exception e)
            {
                Helper.Log.WriteError(e, "Exception in SetCacheDataForSTCSubmissionFromSolarCompanyId:");
            }
        }

        /// <summary>
        /// update cache of all solarcompany of reseller or specific solar company wise for non approved jobs
        /// </summary>
        /// <param name="ResellerId"></param>
        /// <param name="SolarCompanyId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async static Task SetCacheDataForSTCSubmissionFromSolarCompanyIdNonApproved(int ResellerId, string SolarCompanyId = "0", int year = 0)
        {
            try
            {
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                CommonBAL.STCDistributedCacheHashSet(cache, SolarCompanyId, ResellerId, year);
            }
            catch (Exception e)
            {
                Helper.Log.WriteError(e, "Exception in SetCacheDataForSTCSubmissionFromSolarCompanyId:");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ResellerBatchId"></param>
        /// <param name="SolarCompanyId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async static Task SetCacheDataForSTCSubmissionFromSolarCompanyIdBatch(int ResellerBatchId, string SolarCompanyId = "0", int year = 0)
        {
            try
            {
                ValidateYearForGridData(ref year);
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                CommonBAL.STCDistributedCacheHashSet(cache, SolarCompanyId, ResellerBatchId, year);
            }
            catch (Exception e)
            {
                Helper.Log.WriteError(e, "Exception in SetCacheDataForSTCSubmissionFromSolarCompanyId:");
            }
        }

        public async static Task SetCacheDataOnSCARARAMForSTCSubmission(int? solarCompanyId = 0, SortedList<string, string> data = null)
        {
            try
            {
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                for (int year = DateTime.Now.Year; year < ProjectConfiguration.ArchiveMinYear; year--)
                {
                    List<STCSubmissionView> lstSTCFull = new List<STCSubmissionView>();
                    STCDistributedCacheHashGet(cache, new List<int> { solarCompanyId.Value }, year, ref lstSTCFull);
                    DataTable dt = ToDataTable<STCSubmissionView>(lstSTCFull);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        dt.AsEnumerable().Where(a => a.Field<Int32>("SolarCompanyId") == solarCompanyId).ToList().ForEach(r =>
                        {
                            for (int i = 0; i < data.Keys.Count; i++)
                            {
                                r[data.Keys[i]] = string.IsNullOrWhiteSpace(data.Values[i]) ? DBNull.Value : (object)(data.Values[i]);
                            }
                        });
                        lstSTCFull = DataTableToListSTCSubmission(dt);

                        var lstSTCToHash = lstSTCFull.Select(k => new RedisHashSetKeyValuePair()
                        {
                            RedisKey = string.Format(RedisCacheConfiguration.dsSTCHashKey, k.SolarCompanyId),
                            HashKey = k.JobID.Value,
                            HashValuePayload = RedisCacheConfiguration.Serialize(k)
                        }).ToList();

                        for (int i = 0; i < decimal.Divide(lstSTCToHash.Count, ProjectConfiguration.RedisCacheBatchCommandCount); i++)
                        {
                            var stcToHash = lstSTCToHash.Skip(i * ProjectConfiguration.RedisCacheBatchCommandCount).Take(ProjectConfiguration.RedisCacheBatchCommandCount).ToList();
                            RedisCacheConfiguration.SetBatchUsingHash(cache, stcToHash);
                            RedisCacheConfiguration.SetExpireForHash(cache, stcToHash);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Helper.Log.WriteError(ex, "Exception in SetCacheDataOnSCARARAMForSTCSubmission:");
            }
        }
        public async static Task SetCacheDataForNewStcSubmission(int stcJobdetailId)
        {
            try
            {
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                ICreateJobBAL _job = new CreateJobBAL();

                DataTable dataTable = _job.GetResellerSolarCompnayFromJobIdOrStcJobdetailId(null, stcJobdetailId);
                int SolarCompanyId = 0;
                int _JobID = 0;
                if (dataTable.Rows.Count > 0 && dataTable.Rows[0]["STCJobDetailsId"] != null)
                {
                    DataRow dr = dataTable.Rows[0];
                    SolarCompanyId = Convert.ToInt32(dr["SolarCompnayId"]);
                    _JobID = Convert.ToInt32(dr["JobID"]);
                    DataRow _dr = _job.GetSTCJobDetailsIDWiseCachingData(stcJobdetailId);
                    var lstSTCView = DataTableToListSTCSubmission(_dr.Table);

                    if (lstSTCView != null && lstSTCView.Any())
                    {
                        var stcView = lstSTCView.First();
                        var payLoad = RedisCacheConfiguration.Serialize(stcView);
                        RedisCacheConfiguration.SetHashAsync(cache, string.Format(RedisCacheConfiguration.dsSTCHashKey, SolarCompanyId), _JobID, payLoad);
                        int year = stcView.CreatedDate.Year;
                        string mainRedisKey = RedisCacheConfiguration.dsSTCCERApprovedNotInvoicedAllKeysInfoHashKey; // string.Format(RedisCacheConfiguration.dsSTCAllKeysInfoHashKey, year);
                        bool isExists = true;
                        lock (myLock)
                        {
                            List<DistributedCacheAllKeysInfoForHashSetView> mainSTCCache = CommonBAL.DistributedCacheAllKeysInfoGet(cache, mainRedisKey);
                            if (mainSTCCache != null && mainSTCCache.Any(d => d.PID == SolarCompanyId))
                            {
                                var stcCache = mainSTCCache.FirstOrDefault(d => d.PID == SolarCompanyId);
                                string ids = String.Join(",", stcCache.Ids, Convert.ToString(_JobID));
                                stcCache.Ids = string.Join(",", ids.Split(',').ToList().Distinct());
                                DistributedCacheAllKeysInfoSet(cache, mainRedisKey, mainSTCCache);
                            }
                            else
                                isExists = false;
                        }
                        if (!isExists)
                            STCDistributedCacheHashSet(cache, SolarCompanyId.ToString(), 0, year);
                    }
                }
            }
            catch (Exception e)
            {
                Helper.Log.WriteError(e, "Exception in SetCacheDataForNewStcSubmission:");
            }
        }

        #region SnackBar Notification For All Pages Get Set Method in Cache.
        public static void SetCacheDataForSnackbar<T>(string key, T value)
        {
            try
            {
                CacheConfiguration.Set(key, value);
            }
            catch (Exception e)
            {
                _log.LogException(SystemEnums.Severity.Error, "Method Name : SetCacheDataForSnackbar", e);
            }
        }
        public static DataTable GetCacheDataForSnackbar<T>(string key)
        {
            try
            {
                DataTable data;
                if (!string.IsNullOrEmpty(ProjectSession.SnackbarId))
                {
                    data = CacheConfiguration.Get<DataTable>(key);
                    // data.AsEnumerable().Where(X => X.Field<DateTime>("ExpiryDate") >= DateTime.Now);
                    //data = from row in data.AsEnumerable()
                    //       where row.Field<DateTime>("ExpiryDate") >= DateTime.Now
                    //       && row.Field<bool>("IsDeleted") == false
                    //       && !(row.Field<int>("NotificationId")).c
                    // select row;
                    var rows = data.Select("NotificationId not in ('" + ProjectSession.SnackbarId.Replace(",", "','") + "') and ExpiryDate >= '" + DateTime.Now + "' and IsDeleted = 0 ");
                    data = rows.Any() ? rows.CopyToDataTable() : new DataTable();
                    //  data = data.Select("NotificationId not in ('" + ProjectSession.SnackbarId.Replace(",", "','") + "') and ExpiryDate >= '" + DateTime.Now + "' and IsDeleted = 0 ").CopyToDataTable();

                }
                else
                    data = CacheConfiguration.Get<DataTable>(key);
                return data;
            }
            catch (Exception e)
            {
                _log.LogException(SystemEnums.Severity.Error, "Method Name : GetCacheDataForSnackbar", e);
                return null;
            }
        }
        #endregion
        #region cache update from vendor api
        /// <summary>
        /// Update cache in Stc submission page
        /// </summary>
        /// <param name="StcJobDetailsId"></param>
        /// <param name="SolarCompanyId"></param>
        public async static Task SetCacheDataForSTCSubmission_VendorAPI(int StcJobDetailsId)
        {
            try
            {
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                ICreateJobBAL _job = new CreateJobBAL();
                DataRow dr = _job.GetSTCJobDetailsIDWiseCachingData(StcJobDetailsId);
                int JobId = Convert.ToInt32(dr["JobID"]);
                int SolarCompanyId = Convert.ToInt32(dr["SolarCompanyId"]);

                if (dr != null)
                {
                    var lstSTCView = DataTableToListSTCSubmission(dr.Table);
                    if (lstSTCView != null && lstSTCView.Any())
                    {
                        var stcView = lstSTCView.First();
                        var payLoad = RedisCacheConfiguration.Serialize(stcView);
                        var isInsert = RedisCacheConfiguration.SetHash(cache, string.Format(RedisCacheConfiguration.dsSTCHashKey, SolarCompanyId), JobId, payLoad);
                        if (isInsert)
                        {
                            string mainRedisKey = RedisCacheConfiguration.dsSTCCERApprovedNotInvoicedAllKeysInfoHashKey;
                            //int year = stcView.InstallationDate.HasValue ? stcView.InstallationDate.Value.Year : stcView.CreatedDate.Year;
                            //if (stcView.STCStatusId == SystemEnums.STCJobStatus.CERApproved.GetHashCode() && (stcView.IsInvoiced.HasValue && stcView.IsInvoiced.Value))
                            //{
                            //    mainRedisKey = string.Format(RedisCacheConfiguration.dsSTCAllKeysInfoHashKey, year);
                            //}
                            List<DistributedCacheAllKeysInfoForHashSetView> mainSTCCache = CommonBAL.DistributedCacheAllKeysInfoGet(cache, mainRedisKey);
                            if (mainSTCCache != null && mainSTCCache.Any(d => d.PID == SolarCompanyId))
                            {
                                var stcCache = mainSTCCache.FirstOrDefault(d => d.PID == SolarCompanyId);
                                string ids = String.Join(",", stcCache.Ids, Convert.ToString(JobId));
                                stcCache.Ids = string.Join(",", ids.Split(',').ToList().Distinct());
                                DistributedCacheAllKeysInfoSet(cache, mainRedisKey, mainSTCCache);

                            }
                            else
                                STCDistributedCacheHashSet(cache, Convert.ToString(SolarCompanyId), Convert.ToInt32(dr["ResellerId"]), 0);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Helper.Log.WriteError(e, "Exception in SetCacheDataForSTCSubmission_VendorAPI:");
            }
        }
        #endregion
        /// <summary>
        /// get panel list from xmldata
        /// </summary>
        /// <param name="xmlData"></param>
        /// <returns></returns>
        public static List<xmlPanel> getPanelDetailsFromXML(string xmlData)
        {
            List<xmlPanel> objPanelxml = new List<xmlPanel>();
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<xmlPanel>));

                XDocument doc = new XDocument();
                if (!string.IsNullOrEmpty(xmlData))
                {
                    doc = XDocument.Parse(xmlData);
                }
                objPanelxml = doc.Descendants("panel").Select(d =>
                                   new xmlPanel
                                   {
                                       Brand = (d.Element("Brand").Value),
                                       Model = d.Element("Model").Value,
                                       NoOfPanel = Convert.ToInt32(d.Element("NoOfPanel").Value),

                                   }).ToList();


            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
            }
            return objPanelxml;

        }
        /// <summary>
        /// get inverter list from xmldata
        /// </summary>
        /// <param name="xmlData"></param>
        /// <returns></returns>
        public static List<xmlInverter> getInverterDetailsFromXML(string xmlData)
        {
            List<xmlInverter> objInverterxml = new List<xmlInverter>();
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<xmlInverter>));

                XDocument doc = new XDocument();
                if (!string.IsNullOrEmpty(xmlData))
                {
                    doc = XDocument.Parse(xmlData);
                }
                objInverterxml = doc.Descendants("inverter").Select(d =>
                                   new xmlInverter
                                   {
                                       Brand = (d.Element("Brand").Value),
                                       Model = d.Element("Model").Value,
                                       Series = d.Element("Series").Value,
                                   }).ToList();


            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
            }
            return objInverterxml;

        }

        public async static Task SetCacheDataForPeakPay(int? StcJobDetailsId, int? JobId = null, SortedList<string, string> data = null)
        {
            try
            {
                DataTable dataTable = _job.GetResellerSolarCompnayFromJobIdOrStcJobdetailId(JobId, StcJobDetailsId);
                if (dataTable.Rows.Count > 0 && dataTable.Rows[0]["STCJobDetailsId"].ToString() != "")
                {
                    DataRow dr = dataTable.Rows[0];
                    IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                    int solarCompanyId = Convert.ToInt32(dr["SolarCompnayId"]);
                    int jobId = Convert.ToInt32(dr["JobId"]);
                    string redisKey = string.Format(RedisCacheConfiguration.dsPeakPayHashKey, solarCompanyId);
                    List<PeakPayView> lstPeakPayView = RedisCacheConfiguration.GetHash<PeakPayView>(cache, redisKey, jobId);

                    if (lstPeakPayView.Any())
                    {
                        var peakPayView = lstPeakPayView.First();
                        for (int i = 0; i < data.Keys.Count; i++)
                        {
                            SetPropValue(peakPayView, data.Keys[i], data.Values[i]);
                        }
                        var payLoad = RedisCacheConfiguration.Serialize(peakPayView);
                        RedisCacheConfiguration.SetHashAsync(cache, redisKey, jobId, payLoad);
                    }
                    else
                        await SetCacheDataForPeakPayFromSolarCompanyId(solarCompanyId.ToString(), 0);

                    //if (peakPayView != null)
                    //{
                    //    DataTable dt = ToDataTable<PeakPayView>(peakPayView);
                    //    if (dr != null && dt != null)
                    //    {
                    //        dt.AsEnumerable().Where(a => Convert.ToInt32(a["JobID"]) == jobId).ToList().ForEach(drCache =>
                    //        {
                    //            for (int i = 0; i < data.Keys.Count; i++)
                    //            {
                    //                drCache[data.Keys[i]] = string.IsNullOrWhiteSpace(data.Values[i]) ? DBNull.Value : (object)(data.Values[i]);
                    //            }
                    //        });
                    //    }
                    //    peakPayView = DataTableToListPeakPay(dt);
                    //    var payLoad = RedisCacheConfiguration.Serialize(peakPayView.First());
                    //    RedisCacheConfiguration.SetHashAsync(cache, redisKey, jobId, payLoad);
                    //}
                    //else
                    //   await SetCacheDataForPeakPayFromSolarCompanyId(solarCompanyId.ToString(), 0);
                }
            }
            catch (Exception e)
            {
                Helper.Log.WriteError(e, "Exception in setcacheDataForPeakPay:");
            }
        }

        public async static Task SetCacheDataForPeakPayFromSolarCompanyId(string SolarCompanyId, int ResellerId)
        {
            try
            {
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                PeakPayDistributedCacheHashSet(cache, SolarCompanyId, ResellerId);
            }
            catch (Exception e)
            {
                string s = e.Message;
            }
        }

        public async static Task SetCacheDataForPeakPayFromSolarCompanyIdNew(string SolarCompanyId, int ResellerId)
        {
            try
            {
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                PeakPayDistributedCacheHashSet(cache, SolarCompanyId, ResellerId);
            }
            catch (Exception e)
            {
                string s = e.Message;
            }
        }

        public async static Task SetCacheDataForPeakPayFromJobId(string JobIds = "", string STCjobdetailIds = "")
        {
            try
            {
                IDatabase cache = await RedisCacheConfiguration.GetDatabaseAsync();
                List<int> solarCompanyIds = new List<int>();
                DataSet dsPeakPay = _peakPay.GetPeakPayListForCache("", 0, JobIds, STCjobdetailIds);
                if (dsPeakPay != null && dsPeakPay.Tables[0] != null && dsPeakPay.Tables[0].Rows != null && dsPeakPay.Tables[0].Rows.Count > 0)
                {
                    List<PeakPayView> listPeakPayView = DataTableToListPeakPay(dsPeakPay);
                    listPeakPayView.ForEach(d =>
                    {
                        var peakPayView = d;
                        var payLoad = RedisCacheConfiguration.Serialize(peakPayView);
                        var isInsert = RedisCacheConfiguration.SetHash(cache, string.Format(RedisCacheConfiguration.dsPeakPayHashKey, d.SolarCompanyId), d.JobID.Value, payLoad);
                        if (isInsert)
                        {
                            string mainRedisKey = RedisCacheConfiguration.dsPeakPayAllKeysInfoHashKey;
                            List<DistributedCacheAllKeysInfoForHashSetView> mainJobCache = CommonBAL.DistributedCacheAllKeysInfoGet(cache, mainRedisKey);
                            if (mainJobCache != null && mainJobCache.Any(x => x.PID == d.SolarCompanyId))
                            {
                                var jobCache = mainJobCache.FirstOrDefault(y => y.PID == d.SolarCompanyId);
                                string ids = string.Join(",", jobCache.Ids, Convert.ToString(d.JobID.Value));
                                jobCache.Ids = string.Join(",", ids.Split(',').ToList().Distinct());
                                DistributedCacheAllKeysInfoSet(cache, mainRedisKey, mainJobCache);
                            }
                            else
                                PeakPayDistributedCacheHashSet(cache, d.SolarCompanyId.Value.ToString(), 0);
                        }
                    });
                    //var lstPPToHash = listPeakPayView.Select(k => new RedisHashSetKeyValuePair()
                    //{
                    //    RedisKey = string.Format(RedisCacheConfiguration.dsPeakPayHashKey, k.SolarCompanyId),
                    //    HashKey = k.JobID.Value,
                    //    HashValuePayload = RedisCacheConfiguration.Serialize(k)
                    //}).ToList();

                    //for (int i = 0; i < decimal.Divide(lstPPToHash.Count, ProjectConfiguration.RedisCacheBatchCommandCount); i++)
                    //{
                    //    var jobsToHash = lstPPToHash.Skip(i * ProjectConfiguration.RedisCacheBatchCommandCount).Take(ProjectConfiguration.RedisCacheBatchCommandCount).ToList();
                    //    RedisCacheConfiguration.SetBatchUsingHash(cache, jobsToHash);
                    //    RedisCacheConfiguration.SetExpireForHash(cache, jobsToHash);
                    //}
                }
            }
            catch (Exception ex)
            {
                _log.LogException("SetCacheDataForPeakPayFromJobId", ex);
            }
        }

        public static void STCDistributedWithoutCache(List<int> lstSolarCompanyId, int resellerId, int year, ref List<STCSubmissionView> lstSTCFull, int page, int pageSize, DataTable dtFilter = null, DataTable dtSort = null, int StageId = 0, string sStageId = "")
        {
            lstSTCFull = _job.GetJobSTCSubmissionKendoByYearWithoutCacheDapper(string.Join(",", lstSolarCompanyId), resellerId, year, page, pageSize, dtFilter, dtSort, StageId, sStageId);
        }

        public static void PeakPayDistributedWithoutCache(string solarCompanyIds, int resellerId, ref List<PeakPayView> lstPeakPayView, int pageNumber, int pageSize, int stageId, string sortCol, string sortDir, string searchText, Decimal stcFromPrice, Decimal stcToPrice, string cerApprovedFromDate, string cerApprovedToDate, string settleBeforeFromDate, string settleBeforeToDate, string paymentFromDate, string paymentToDate, bool isSentInvoice, bool isUnsentInvoice, bool isReadytoSentInvoice, string systSize)
        {
            lstPeakPayView = _peakPay.GetPeakPayListForWithoutCache(solarCompanyIds, resellerId, pageNumber, pageSize, stageId, sortCol, sortDir, searchText, stcFromPrice, stcToPrice, cerApprovedFromDate, cerApprovedToDate, settleBeforeFromDate, settleBeforeToDate, paymentFromDate, paymentToDate, isSentInvoice, isUnsentInvoice, isReadytoSentInvoice, systSize);
        }


    }
}
