using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Documents
{
    public class DocumentSteps
    {
        public int DocumentStepId { get; set; }
        public int StateId { get; set; }
        public string Distributors { get; set; }
        public string Stage { get; set; }
        public Int16 Type { get; set; }
        public string StepName { get; set; }
        public string ToEmailid { get; set; }
        public bool IsApplied { get; set; }
        public DateTime? CommentDate { get; set; }
        public string SubjectCode { get; set; }
        public int? JobId { get; set; }
        public string Comment { get; set; }
        public string OnlineLink { get; set; }
    }

    public class DocumentStepType
    {
        public int Type { get; set; }
        public bool IsClassic { get; set; }
    }
}
