using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class ResellerBatch
    {
        public int ID { get; set; }
        public string BatchName { get; set; }
        public string ResellerIDs { get; set; }
    }
}
