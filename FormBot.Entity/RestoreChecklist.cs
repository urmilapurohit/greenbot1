using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
   public class RestoreChecklist
    {
        public int jobSchedulingId { get; set; }
        public int jobId { get; set; }
        public int VisitCheckListItemId { get; set; }
        public bool IsDeleted { get; set; }
        public string ItemName { get; set; }
       // public RestoreChecklist restoreChecklist { get; set; }
        public List<RestoreChecklist> restoreChecklists { get; set; }
    }

}
