using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.VEEC
{
    public class VEECInstallationDetail
    {


        public int VEECInstallationId { get; set; }

        public int VEECId { get; set; }


        [DisplayName("Unit Type:")]
        //[Required(ErrorMessage = "Unit Type is required.")]
        public int? UnitTypeID { get; set; }

        [DisplayName("Unit Number:")]
        //[Required(ErrorMessage = "Unit Number is required.")]
        public string UnitNumber { get; set; }

        [DisplayName("Street Number:")]
        [Required(ErrorMessage = "Street Number is required.")]
        public string StreetNumber { get; set; }

        [DisplayName("Street Name:")]
        [Required(ErrorMessage = "Street Name is required.")]
        public string StreetName { get; set; }

        [DisplayName("Street Type:")]
        [Required(ErrorMessage = "Street Type is required.")]
        public int? StreetTypeID { get; set; }

        [DisplayName("Town:")]
        [Required(ErrorMessage = "Town is required.")]
        public string Town { get; set; }

        [DisplayName("State:")]
        [Required(ErrorMessage = "State is required.")]
        public string State { get; set; }

        [DisplayName("Post Code:")]
        [Required(ErrorMessage = "PostCode is required.")]
        public string PostCode { get; set; }

        [DisplayName("Postal Address:")]
        [Required(ErrorMessage = "Postal Address is required.")]
        public bool IsPostalAddress { get; set; }

        [DisplayName("Postal Delivery Type:")]
        //[Required(ErrorMessage = "Postal Delivery Type is required.")]
        public string PostalAddressID { get; set; }

        [DisplayName("Postal Delivery Number:")]
        //[Required(ErrorMessage = "Postal Delivery Number is required.")]
        public string PostalDeliveryNumber { get; set; }

        [DisplayName("Industry/Business Type:")]
        [Required(ErrorMessage = "Industry business type is required.")]
        public int IndustryBusinessType { get; set; }

        [DisplayName("Number Of Levels")]
        [Required(ErrorMessage = "Number of levels is required.")]
        public decimal? NumberOfLevels { get; set; }

        [DisplayName("Floor Space (m2)")]
        [Required(ErrorMessage = "Floor Space is required.")]
        public decimal? FloorSpace { get; set; }

        [DisplayName("Floor Space Upgraded Area (m2)")]
        public decimal? FloorSpaceUpgradedArea { get; set; }

        public string IndustryBusinessTypeName { get; set; }

        public string StreetTypeName { get; set; }

        public string UnitTypeName { get; set; }

        public bool IsSameAsOwnerAddress { get; set; }
        [DisplayName("Level Type")]
        public string LevelType { get; set; }
        [DisplayName("Level Number")]
        public string LevelNumber { get; set; }

    }
}
