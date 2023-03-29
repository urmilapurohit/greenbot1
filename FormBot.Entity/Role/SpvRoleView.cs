using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FormBot.Entity.Role
{
   public class SpvRoleView
    {
        public int SpvRoleId { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        ////public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        [Required(ErrorMessage = "User type is required.")]
        public int UserType { get; set; }

        [NotMapped]
        public SelectList UserTypeList { get; set; }
        [NotMapped]
        public string Rights { get; set; }
        [NotMapped]
        public int TotalRecords { get; set; }
        [NotMapped]
        public string UserTypeName { get; set; }
        [NotMapped]
        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(SpvRoleId));
            }
        }

        public bool IsSystemRole { get; set; }
    }
}
