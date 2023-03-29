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
    public class FCOGroupView
    {
        public int FCOGroupId { get; set; }

        [DisplayName("Group Name:")]
        [Required(ErrorMessage = "Group Name is required.")]
        public string GroupName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public IEnumerable<SelectListItem> lstUser { get; set; }

        public List<SelectListItem> lstAssignedUser { get; set; }

        [AllowHtml]
        public string[] AssignedUser { get; set; }

        public int UserID { get; set; }

        [NotMapped]
        public int TotalRecords { get; set; }
    }
}
