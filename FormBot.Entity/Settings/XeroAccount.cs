using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Settings
{
    public class XeroAccount
    {
        public int AccountId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public decimal Tax { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int SolarCompanyId { get; set; }

        public bool IsDeleted { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string XeroAccountId { get; set; }

        public bool? EnablePayments { get; set; }

        public string TaxType { get; set; }

        [NotMapped]
        public int TotalRecords { get; set; }
    }
}
