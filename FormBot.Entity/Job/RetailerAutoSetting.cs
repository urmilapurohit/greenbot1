using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FormBot.Entity.Job
{
   public class RetailerAutoSetting
    {
        [DisplayName("Representative:")]
        [Required(ErrorMessage = "Representative is required.")]
        public int RetailerUserId { get; set; }
        [DisplayName("Position Held:")]
        [Required(ErrorMessage = "Position is required.")]
        public string PositionHeld { get; set; }
        public string Signature { get; set; }
        public bool IsSubContractor { get; set; }
        [DisplayName("Default Installer:")]
        [Required(ErrorMessage = "Default Installer is required.")]
        public int IsEmployee { get; set; }
        [DisplayName("System Design:")]
        [Required(ErrorMessage = "System design is required.")]
        public int IsChangedDesign { get; set; }
        public string base64Img { get; set; }
       

    }
}
