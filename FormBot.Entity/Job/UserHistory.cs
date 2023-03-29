using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class UserHistory
    {
        public string HistoryMessage { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public int CategoryID { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsImportant { get; set; }
        public int IsDeleted { get; set; }
        public int Id { get; set; }
    }
}
