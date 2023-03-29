using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.CheckList  
{
    public class CheckListTemplate
    {
        [DisplayName("CheckList Template:")]
        public int? CheckListTemplateId { get; set; }

        public string CopyOfCheckListTemplateId { get; set; }

        public string DeleteDefaultItemId { get; set; }

        [DisplayName("Template Name:")]
        [Required(ErrorMessage = "Template name is required.")]
        public string CheckListTemplateName { get; set; }

        public int? SolarCompanyId { get; set; }

        public string CompanyName { get; set; }

        public int? ResellerID { get; set; }

        public int CreatedBy { get; set; }

        public string CreatedName { get; set; }
        public DateTime CreatedDate { get; set; }

        public string strCreatedDate
        {
            get
            {
                return CreatedDate.ToString("dd/MM/yyyy");
            }
        }

        public string strModifiedDate
        {
            get
            {
                return ModifiedDate == null ? "" :  Convert.ToDateTime(ModifiedDate).ToString("dd/MM/yyyy");
            }
        }
        public int? ModifiedBy{ get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }

        [DisplayName("Is Default:")]
        public bool IsDefault { get; set; }

        public int TotalRecords { get; set; }

        public List<CheckListItem> lstCheckListItem { get; set; }


        [NotMapped]
        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.CheckListTemplateId));
            }
        }

        public bool isSetFromSetting { get; set; }

        public int JobSchedulingId { get; set; }

        public string VisitCheckListItemIds { get; set; }

        public int JobType { get; set; }
        public string defaultCheckListTemplateName { get; set; }
        public bool isFromIsDeletedChecklistItem { get; set; }
    }
}
