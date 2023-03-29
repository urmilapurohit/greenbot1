using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class SolarCompany
    {
        public int SolarCompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyABN { get; set; }
        public int ResellerID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int Status { get; set; }
        public string Note { get; set; }
        public int IsSubContractor { get; set; }
        public DateTime ComplainDate { get; set; }
        public int ComplainBy { get; set; }

        [NotMapped]
        public string IsValid { get; set; }

        public string Phone { get; set; }
    }
    public class Representative
    {
        public int UserId { get; set; }
        public string  Name { get; set; }
    }
}
