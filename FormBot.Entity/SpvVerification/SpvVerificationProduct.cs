using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class SpvVerificationProduct
    {
        public string SerialNumber { get; set; }
        public string Manufacturer { get; set; }
        public string ModelNumber { get; set; }
        public string Supplier { get; set; }

        public decimal VOC { get; set; }
        public decimal ISC { get; set; }
        public decimal PM { get; set; }
        public decimal VM { get; set; }
        public decimal IM { get; set; }
        public decimal FF { get; set; }

    }
}
