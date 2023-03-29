using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FormBot.Helper;

namespace FormBot.Entity
{
    public class FCOGroup
    {
        public int FCOGroupId { get; set; }

        [DisplayName("Group Name:")]
        [Required(ErrorMessage = "Group Name is required.")]
        public string GroupName { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public IEnumerable<SelectListItem> lstUser { get; set; }

        public List<SelectListItem> lstAssignedUser { get; set; }

        [AllowHtml]
        public string[] AssignedUser { get; set; }

        public string AssignFCOUser { get; set; }

        public int UserID { get; set; }
        [NotMapped]
        public int TotalRecords { get; set; }

        [NotMapped]
        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(FCOGroupId));
            }

        }

    }

    [Serializable]
    public class FCOGroup1
    {
        public int FCOGroupId { get; set; }
        public string GroupName { get; set; }

    }
}
