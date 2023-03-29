using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FormBot.Entity
{
    public class FCOUserGroup
    {
        public int FCOUserGroupId { get; set; }

        public int FCOGroupId { get; set; }

        public int UserID { get; set; }

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
    }
}
