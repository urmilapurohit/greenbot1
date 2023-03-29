using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FormBot.Entity.Email
{
    public class EmailTemplate
    {
        public int TemplateID { get; set; }

        [DisplayName("Template Name:")]
        [Required(ErrorMessage = "Template Name is required.")]
        public string TemplateName { get; set; }

        [DisplayName("Subject:")]
        [Required(ErrorMessage = "Subject is required.")]
        public string Subject { get; set; }

        [AllowHtml]
        [DisplayName("Email Content:")]
        [Required(ErrorMessage = "Email Content is required.")]
        public string Content { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }

        [NotMapped]
        public int TotalRecords { get; set; }

        [NotMapped]
        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.TemplateID)); 
            }
        }
    }
}
