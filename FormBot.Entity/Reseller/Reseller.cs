using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class Reseller
    {
        public int ResellerID { get; set; }
        public int ResellerIdForFilter { get; set; }
        public string ResellerName { get; set; }
        public string Logo { get; set; }
        public int Theme { get; set; }
        public string CompanyABN { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string LoginCompanyName { get; set; }
        [NotMapped]
        public string IsValid { get; set; }
        public DateTime? SyncXeroDate { get; set; }
        public bool IsWholeSaler { get; set; }
        public int UserId { get; set; }
        public bool IsSAASUser { get; set; }
    }
}
