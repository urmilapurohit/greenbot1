using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.VEECCheckList
{
    public class VEECCheckListTemplate
    {
        [DisplayName("CheckList Template:")]
        public int? VEECCheckListTemplateId { get; set; }

        public string CopyOfCheckListTemplateId { get; set; }

        public string DeleteDefaultItemId { get; set; }

        [DisplayName("Template Name:")]
        [Required(ErrorMessage = "Template name is required.")]
        public string VEECCheckListTemplateName { get; set; }

        public int? SolarCompanyId { get; set; }

        public int? ResellerID { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }

        [DisplayName("Is Default:")]
        public bool IsDefault { get; set; }

        public int TotalRecords { get; set; }

        public List<VEECCheckListItem> lstCheckListItem { get; set; }


        [NotMapped]
        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.VEECCheckListTemplateId));
            }
        }

        public bool isSetFromSetting { get; set; }

        public int JobSchedulingId { get; set; }

        public string VisitCheckListItemIds { get; set; }

        public int JobType { get; set; }
    }
}
