using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class GlobalBillableTerms
    {
        public int Id { get; set; }
        public string BillerCode { get; set; }
        public string TermName { get; set; }
        public string TermCode { get; set; }
        public string TermDescription { get; set; }
        public decimal GlobalPrice { get; set; }
        public decimal OldGlobalPrice { get; set; }
        public string ResellerName { get; set; }
        public bool IsGlobalGST { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByName { get; set; }
        public int UserTypeID { get; set; }
        public int TotalRecords { get; set; }
        public bool isEnable { get; set; }
        public int OutPutValue { get; set; }


        public string HistoryMessage { get; set; }
        public string BillableTermHistoryMessage { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string CreateDate { get; set; }
        public int CategoryID { get; set; }
        public string HistoryCategory { get; set; }
        public int ModifiedBy { get; set; }
        public string Modifier { get; set; }
        public int currentPage { get; set; }
        public int FilterID { get; set; }
        public int NoteId { get; set; }
    }
}



