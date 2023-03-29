using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Collections;

namespace FormBot.Entity
{
    public class RAMSolarCompany
    {
        [Required(ErrorMessage = "Reseller Account Manager is required.")]
        public int UserTypeID { get; set; }

        public int SolarCompanyId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }

        public string RAMSolarCompanyUser { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string CompanyName { get; set; }

        public IEnumerable<SelectListItem> LstRAMSolarCompanyUser { get; set; }

        public List<SelectListItem> LstRAMSolarCompanyAssignedUser { get; set; }

        public string[] RAMSolarCompanyAssignedUser { get; set; }

        public IEnumerable<SelectListItem> nodeList { get; set; }

        public List<SelectListItem> nodeListAssigned { get; set; }
       
        public int UserID { get; set; }
        [NotMapped]
        public int TotalRecords { get; set; }
    }
}
