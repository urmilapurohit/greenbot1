using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FormBot.Entity
{
    public class AssignSCO
    {
        [Required(ErrorMessage = "Solar Connections Officer is required.")]
        public int UserTypeID { get; set; }

        public int JobID { get; set; }

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

        public IEnumerable<SelectListItem> LstJobSCOUser { get; set; }

        public List<SelectListItem> LstJobSCOAssignedUser { get; set; }

        public string[] JobSCOAssignedUser { get; set; }

        public int UserID { get; set; }

        [NotMapped]
        public int TotalRecords { get; set; }

        public string Title { get; set; }

        public IEnumerable<System.Web.Mvc.SelectListItem> nodeList { get; set; }
        public List<System.Web.Mvc.SelectListItem> nodeListAssigned { get; set; }

        public string RefNumber { get; set; }

        public string FullJobDetails { get; set; }
    }
}
