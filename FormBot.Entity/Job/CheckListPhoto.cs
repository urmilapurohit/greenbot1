using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class CheckListPhoto
    {
        public int VisitCheckListPhotoId { get; set; }

        public int CheckListItemId { get; set; }

        public int JobSchedulingId { get; set; }

        public int JobId { get; set; }
        public string Path { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
       
    }
}
