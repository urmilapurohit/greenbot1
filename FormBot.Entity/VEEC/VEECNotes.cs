using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.VEEC
{
    public class VEECNotes
    {
        public int VEECNotesID { get; set; }
        public int VEECID { get; set; }

        [DisplayName("Notes:")]
        [Required(ErrorMessage = "Notes is required.")]
        public string Notes { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string strCreatedDate { get; set; }
        public string Created { get; set; }
        public IEnumerable<JobNotes> PagedList
        {
            get;
            set;
        }

        [NotMapped]
        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.VEECID));
            }
        }

        public string VEECTitle { get; set; }

        public bool IsSeen { get; set; }

        public Int64 RowNumber { get; set; }

        public int VEECSchedulingId { get; set; }

        public string VendorVEECNoteId { get; set; }
    }
}
