using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.VEECManageList
{
    public class VEECProductBrands
    {
        public int ProductBrandId { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string ProductType { get; set; }
        public string ProductCategory { get; set; }
        public string TechnologyClass { get; set; }
        public string NoOfLamps { get; set; }
        public string LCP { get; set; }
        public string NLP { get; set; }
        public string RatedLifetime { get; set; }
        public string VRUVoltage { get; set; }
        public string InBuiltLCD { get; set; }
        public string Status { get; set; }
        public DateTime? ApplicationDate { get; set; }        
        public DateTime? EffectiveFrom { get; set; }        
        public DateTime? EffectiveTo { get; set; }        
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }       
        [NotMapped]
        public int TotalRecords { get; set; }
    }
}
