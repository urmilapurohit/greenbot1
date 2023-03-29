using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class Inverters
    {
        public int? InverterId { get; set; }
        public string Manufacturer { get; set; }
        public string Series { get; set; }
        public string ModelNumber { get; set; }
        public int? AcPowerKW { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

        [NotMapped]
        public int TotalRecords { get; set; }
    }
}
