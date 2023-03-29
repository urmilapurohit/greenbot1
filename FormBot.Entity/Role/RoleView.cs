using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using FormBot.Helper;
using System.ComponentModel;

namespace FormBot.Entity
{
    public class RoleView
    {
        public int RoleId { get; set; }
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
        public string Id {
            get { 
                return QueryString.QueryStringEncode("id=" + Convert.ToString(RoleId));
            } 
        }

        public bool IsSystemRole { get; set; }
        public string CreateBy { get; set; }
        public int CreatedByUsertype { get; set; }
        [DisplayName("Invoicer:")]
        public int? Invoicer { get; set; }
        public int? SelectedInvoicer { get; set; }
    }
}
