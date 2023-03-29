using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class JobRetailerSetting
    {
        [DisplayName("Representative:")]
        [Required(ErrorMessage = "Representative is required.")]
        public int JobRetailerUserId { get; set; }
        public string RepresentativeName { get; set; }
       // [DisplayName("Position Held:")]
        //[Required(ErrorMessage = "Position is required.")]
        public string PositionHeld { get; set; }
        public string PositionHeldlbl { get; set; }
        public bool IsSubContractor { get; set; }
        //public bool IsEmployee { get; set; }
        //public bool IsChangedDesign { get; set; }
        [DisplayName("Default Installer:")]
        [Required(ErrorMessage = "Default Installer is required.")]
        public int IsEmployee { get; set; }
        [DisplayName("System Design:")]
        [Required(ErrorMessage = "System design is required.")]
        public int IsChangedDesign { get; set; }
        public string InstallerName { get; set; }
        public string AccerdiationNumber { get; set; }
        public string Statement { get; set; }
        public string Signature { get; set; }
        public string SignedBy { get; set; }
        public string SignedDate { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string strIsEmployee { get; set; }
        public string base64Img { get; set; }
    }
}
