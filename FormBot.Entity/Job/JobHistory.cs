using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class JobHistory
    {
        public int JobHistoryID { get; set; }
        public int JobID { get; set; }
        public int IsDeletedJobNote { get; set; }
        public string Title { get; set; }
        public string HistoryMessage { get; set; }
        public bool IsSSC { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string CreateDate { get; set; }
        public int CategoryID { get; set; }
        public string HistoryCategory { get; set; }
        public int ModifiedBy { get; set; }
        public string Modifier { get; set; }
        public int AssignTo { get; set; }
        public string DocumentPath { get; set; }
        public string DocumentName { get; set; }
        public string RefNumber { get; set; }

        public string JobIdEncoded
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.JobID));
            }
        }

        public string Name { get; set; }        

        public string FunctionalityName { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDetails { get; set; }
        public string ErrorDescription { get; set; }
        public string SerialNumbers { get; set; }
        public string DocumentType { get; set; }
        public string UniqueVisitId { get; set; }
        public string PhotosCount { get; set; }
        public int TotalRecords { get; set; }
        public int currentPage { get; set; }
        public string stcInvoiceNumber { get; set; }
        public int FilterID { get; set; }
        public bool IsImportant { get; set; }
        public string strNotesTypeValue { get; set; }
        public int NotesTypeValue { get; set; }
        public int NoteId { get; set; }
        public string ItemName { get; set; }
        public string PhotoName { get; set; }

    }
}
