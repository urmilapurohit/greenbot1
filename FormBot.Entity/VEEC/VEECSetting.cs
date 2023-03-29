using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.VEEC
{
    public class VEECSetting
    {
        public int JobSettingId { get; set; }

        [DisplayName("Is Default Job View New:")]
        public bool IsDefaultJobViewNew { get; set; }

        public int? ResellerID { get; set; }

        public int SolarCompanyId { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public VEECCheckList.VEECCheckListTemplate veecCheckListTemplate { get; set; }

        //public JobCustomField jobCustomField { get; set; }

        public Documents.DocumentTemplate documentTemplate { get; set; }

        public SolarCompanyNotification solarCompanyNotification { get; set; }

        public VEECScheduling veecSchedulingTemplate { get; set; }


    }
}
