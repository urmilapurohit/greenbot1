using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class SolarCompanyNotification
    {

        public int NotificationId { get; set; }

        [DisplayName("Title:")]
        [Required(ErrorMessage = "Title is required.")]
        public string NotificationTitle { get; set; }

        [DisplayName("Content:")]
        [Required(ErrorMessage = "Content is required.")]
        public string NotificationContent { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [Required(ErrorMessage = "ExpiryDate cannot be empty")]
        [DataType(DataType.DateTime)]
        public DateTime ExpiryDate { get; set; }

        public bool IsDeleted { get; set; }

        public int TotalRecords { get; set; }

        public Int64 RowNumber { get; set; }

        public string StrExpiryDate { get; set; }

        public bool IsSpecialNotification { get; set; }

        public string strCreateDate { get; set; }
        public bool ShowToAll { get; set; }

    }
}
