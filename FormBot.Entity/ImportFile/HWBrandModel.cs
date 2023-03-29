using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class HWBrandModel
    {
        public int HWBrandModelId { get; set; }
        public string Item { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public DateTime? EligibleFrom { get; set; }
        public DateTime? EligibleTo { get; set; }
        public string Zone1Certificates { get; set; }
        public string Zone2Certificates { get; set; }
        public string Zone3Certificates { get; set; }
        public string Zone4Certificates { get; set; }
        public string Zone5Certificates { get; set; }

        [NotMapped]
        public int TotalRecords { get; set; }
    }
}
