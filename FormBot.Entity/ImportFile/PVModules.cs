using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class PVModules
    {
        public int? PVModuleId { get; set; }
        public string CertificateHolder { get; set; }
        public string ModelNumber { get; set; }
        public int? Wattage { get; set; }
        public DateTime? CECApprovedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string FireTested { get; set; }

        [NotMapped]
        public int TotalRecords { get; set; }

    }
    public class Spvmanufacturer
    {
        public int SPVManufactureId { get; set; }
        public string SPVManufactureName { get; set; }
        public string SPVManufactureProductVerificationUrl { get; set; }
        public string SPVManufactureInstallationVerificationUrl { get; set; }
        public bool IsSpvAllowedBySpvmanufacturer { get; set; }
        public string Supplier { get; set; }
        public string ServiceAdministrator { get; set; }
        public int TotalRecords { get; set; }
    }

}
