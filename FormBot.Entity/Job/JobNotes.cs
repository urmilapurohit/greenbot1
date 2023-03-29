using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FormBot.Helper;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBot.Entity
{
    public class JobNotes
    {
        public int JobNotesID { get; set; }
        public int JobID { get; set; }

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
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.JobID));
            }
        }

        public string JobTitle { get; set; }

        public bool IsSeen { get; set; }

        public Int64 RowNumber { get; set; }

        public int JobSchedulingId { get; set; }

        public string VendorJobNoteId { get; set; }

        public string TaggedUser { get; set; }

        public bool IsImportantNote { get; set; }
    }
}
