using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.VEEC
{
    public class VEECSchedulingPopup
    {
        [DisplayName("Visit Label:")]
        public string Label { get; set; }

        [DisplayName("Visit Detail:")]
        public string Detail { get; set; }

        [DisplayName("Visit Start:")]
        [Required(ErrorMessage = "Start date is required.")]
        public DateTime visitStartDate { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        public TimeSpan visitStartTime { get; set; }

        [DisplayName("Visit End:")]
        [Required(ErrorMessage = "End date is required.")]
        public DateTime visitEndDate { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        public TimeSpan visitEndTime { get; set; }

        public int VeecSchedulingID { get; set; }

        [Required]
        public int VeecID { get; set; }

        [Required]
        public int UserId { get; set; }

        public int Status { get; set; }

        public int isNotification { get; set; }
        public int isDrop { get; set; }

        public string strVisitStartDate { get; set; }
        public string strVisitEndDate { get; set; }
        public string strVisitStartTime { get; set; }
        public string strVisitEndTime { get; set; }
        public string UserName { get; set; }
        public string veecTitle { get; set; }

        public int VeecCheckListTemplateId { get; set; }
        public string TemplateId { get; set; }

        public string VeecVisitCheckListItemIds { get; set; }

        public Int64 TempVeecSchedulingId { get; set; }
        public bool IsFromCalendarView { get; set; }

        public int SolarCompanyId { get; set; }
    }
}
