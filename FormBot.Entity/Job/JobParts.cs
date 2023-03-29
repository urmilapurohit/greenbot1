using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class JobParts
    {
        public int JobPartID { get; set; }

        public string Description { get; set; }

        public string ItemCode { get; set; }

        public decimal? Sale { get; set; }

        public decimal? Purchase { get; set; }

        public decimal? Margin { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public int? SolarCompanyId { get; set; }

        public bool isXeroParts { get; set; }

        [NotMapped]
        public int UserTypeId { get; set; }

        [NotMapped]
        public int TotalRecords { get; set; }

        [NotMapped]
        public bool isAllPart { get; set; }

        public string XeroPartId { get; set; }

        [NotMapped]
        public int SyncValue { get; set; }

        public string TaxType { get; set; }

        public string SaleAccountCode { get; set; }

        public string PurchaseAccountCode { get; set; }
    }
}
