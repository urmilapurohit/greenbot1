using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
   public  class SpvVerificationModel
    {
        public string Manufacturer { get; set; }
        public string ModelNumber { get; set; }
        public string BillOfMaterials { get; set; }
        public string FactoryName { get; set; }
        public string FactoryLocation { get; set; }
        public int Wattage { get; set; }

        public string SerialNumber { get; set; }
        public bool IsValid { get; set; }
        
    }
}
