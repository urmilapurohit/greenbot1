using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class BatteryStorage
    {
        public int BatteryStorageId { get; set; }
        public string Manufacturer { get; set; }
        public string ModelNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }

        [NotMapped]
        public int TotalRecords { get; set; }
        public string EquipmentCategory { get; set; }
        public string CompliancePathway { get; set; }
        public string BrandName { get; set; }
        public string Series { get; set; }
        public decimal RatedApparentACPowerkVA { get; set; }
        public decimal NominalBatteryCapacitykWh { get; set; }
        public int DepthOfDischarge { get; set; }
        public decimal UsableCapacitykWh { get; set; }
        public int MinOperatingTemp { get;set;}
        public int MaxOperatingTemp { get; set; }
        public string OutdoorUsage { get; set; }
        public DateTime CECApprovalDate { get; set; }
        public DateTime CECExpiryDate { get; set; }
    }
}
