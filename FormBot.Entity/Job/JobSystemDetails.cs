using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class JobSystemDetails
    {
        public int JobSystemDetailID { get; set; }
        public int JobID { get; set; }

        [Display(Name = "System Size:")]        
        public decimal? SystemSize { get; set; }

        [Display(Name = "Serial Numbers:")]
        public string SerialNumbers { get; set; }

        public string InverterSerialNumbers { get; set; }


        [Display(Name = "Calculated STC:")]
        public decimal? CalculatedSTC { get; set; }

        [NotMapped]
        [Display(Name = "Calculated STC:")]
        public decimal? CalculatedSTCForSWH { get; set; }

        [Display(Name = "System Brand:")]
        [NotMapped]
        public string SystemBrand { get; set; }

        [Display(Name = "System Model:")]
        [NotMapped]
        public string SystemModel { get; set; }

        [Display(Name = "Total Number of Panels:")]
        [NotMapped]
        public int? NoOfPanel { get; set; }
        public int? NoOfInverter { get; set; }

        [Display(Name = "Installation Type:")]
        public string InstallationType { get; set; }

        public string panelXmlTabular { get; set; }
        public string inverterXmlTabular { get; set; }
        public List<JobBatteryManufacturer> lstJobBatteryManufacturer { get; set; }
        public int jobTypeTab { get; set; }

        public string StoredSerialNumber { get; set; }

        public string batterySystemPartOfAnAggregatedControl { get; set; }
        public string changedSettingOfBatteryStorageSystem { get; set; }
                
        public decimal? ModifiedCalculatedSTC { get; set; }
        public decimal? PreviousSystemSize { get; set; }
        public string CECApprovedDate { get; set; }
        public string ExpiryDate { get; set; }
    }
    public class JobSystemDetailsVendorAPI
    {
        public decimal? SystemSize { get; set; }
        public string SerialNumbers { get; set; }
        public decimal? CalculatedSTC { get; set; }
        public decimal? CalculatedSTCForSWH { get; set; }
        public string SystemBrand { get; set; }
        public string SystemModel { get; set; }
        public int? NoOfPanel { get; set; }
        public string InstallationType { get; set; }
    }
}
