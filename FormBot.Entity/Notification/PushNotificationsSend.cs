using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Notification
{
    public class PushNotificationsSend
    {
        public int pushNotificationId { get; set; }
        [DisplayName("Notification:")]
        [Required(ErrorMessage = "Notification Message is required.")]
        public string Notification { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public int TotalRecords { get; set; }
        public string StrCreateDate { get; set; }
        public int ResellerId { get; set; }
        public int SolarCompanyId { get; set; }

        //[DisplayName("ElectricianId:")]
        //[Required(ErrorMessage = "Electrician is required.")]
        public int ElectricianId { get; set; }
        //[DisplayName("ContractorId:")]
        //[Required(ErrorMessage = "Contractor is required.")]
        public int ContractorId { get; set; }
        [DisplayName("JobType:")]
        [Required(ErrorMessage = "Job Type is required.")]
        public int JobType { get; set; }
        [DisplayName("Platform:")]
        [Required(ErrorMessage = "Platform is required.")]
        public int Platform { get; set; }
        public string ResellerName { get; set; }
        public string CompanyName { get; set; }
        public string SolarElectrician { get; set; }
        public string Contractor { get; set; }
        public string strJobType { get; set; }
        public string strPlatform { get; set; }

    }
}
