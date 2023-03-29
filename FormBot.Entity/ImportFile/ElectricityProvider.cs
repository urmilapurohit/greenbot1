using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class ElectricityProvider
    {
        public int ElectricityProviderId { get; set; }
        public string Provider { get; set; }
        public string Type { get; set; }
        public string State { get; set; }
        public string Preapprovals { get; set; }
        public string MyProperty { get; set; }
        public string Connections { get; set; }

        [NotMapped]
        public int TotalRecords { get; set; }
    }
}
