using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class JobBatteryManufacturer
    {
        public int JobBatteryManufacturerId { get; set; }
        public int JobID { get; set; }
        public string Manufacturer { get; set; }
        public string ModelNumber { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
