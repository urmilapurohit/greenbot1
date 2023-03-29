using FormBot.Entity.Notification;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class JobSetting
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

        public CheckList.CheckListTemplate checkListTemplate { get; set; }

        public JobCustomField jobCustomField { get; set; }

        public Documents.DocumentTemplate documentTemplate { get; set; }

        public SolarCompanyNotification solarCompanyNotification { get; set; }

        public PushNotificationsSend pushNotificationSend { get; set; }
        public BasicDetails BasicDetails { get; set; }
        [DisplayName("solarElectrician:")]
        [Required(ErrorMessage = "Electrician is required.")]
        public List<SolarElectricianView> solarElectrician { get; set; }
        [DisplayName("platform:")]
        [Required(ErrorMessage = "platform is required.")]
        public int platform { get; set; }
        public User Users { get; set; }
        [DisplayName("solarContractor:")]
        [Required(ErrorMessage = "Contactor is required.")]
        public List<User> solarContractor { get; set; }


    }

    public class JobSettingLog
    {
        public int JobSettingID { get; set; }
        public string HistoryMessage { get; set; }
        public string UserName { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string strModifiedDate { get; set; }
        public int ModifiedBy { get; set; }

    }
}
