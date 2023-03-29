using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class InvoiceHistory
    {
        public int JobID { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceTo { get; set; }
    }
}
