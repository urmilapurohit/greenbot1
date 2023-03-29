using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FormBot.Entity
{
    public class CERImport
    {
        public int FileId { get; set; }
        public List<SelectListItem> FileType { get; set; }
    }

    public class BulkSendRequest
    {
        [DisplayName("User Type:")]
        public int UserTypeID { get; set; }

        [Required(ErrorMessage = "Reseller Name is required.")]
        public int? ResellerID { get; set; }
             
        [Required(ErrorMessage = "Solar Company is required.")]
        public int SolarCompanyId { get; set; }

        public int ElectricianStatusId { get; set; }
    }   
}
