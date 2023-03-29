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

namespace FormBot.Entity.Documents
{
    public class DocumentTemplate
    {
        [DisplayName("Document Template:")]
        public int DocumentTemplateId { get; set; }

        [DisplayName("Document Template Name:")]
        [Required(ErrorMessage = "Document Template name is required.")]
        public string DocumentTemplateName { get; set; }

        public int? SolarCompanyId { get; set; }

        public string Path { get; set; }

        public string FileName { get; set; }        

        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }

        public int? StateID { get; set; }

        public string StateName { get; set; }

        public int? JobTypeID { get; set; }

        [DisplayName("Is Default:")]
        public bool IsDefault { get; set; }

        public int TotalRecords { get; set; }

        [NotMapped]
        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.DocumentTemplateId));
            }
        }

        public string Data { get; set; }
        public List<Pdf.PdfItems> PdfItems { get; set; }
        public string PDFURL { get; set; }

        public string PDFSource { get; set; }
        public string ParsedData
        {
            get
            {
                try
                {
                    return Newtonsoft.Json.JsonConvert.SerializeObject(PdfItems);
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        public int JobFieldId { get; set; }

        public List<SelectListItem> JobFields { get; set; }
    }


    public class Doc
    {
        public string name { get; set; }
        public string stage { get; set; }
        public string abbr { get; set; }
        public string sp { get; set; }
        public string path { get; set; }
        public string TemplateName { get; set; }
    }

    public class DocObject
    {
        public string jobId { get; set; }
        public List<Doc> docs { get; set; }
        public int UserId { get; set; }
        public bool fillData { get; set; }
        public bool UseNewDocTemplate { get; set; }
        public string type { get; set; }
    }
}
