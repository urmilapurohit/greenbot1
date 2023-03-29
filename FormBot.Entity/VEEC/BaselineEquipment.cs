using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.VEEC
{
    public class BaselineEquipment
    {
        public int VEECId { get; set; }

        public int VEECAreaId { get; set; }

        public int? BaselineEquipmentId { get; set; }

        [Required(ErrorMessage = "Lamp type is Required.")]
        [Display(Name = "Lamp Type")]
        public int LampType { get; set; }

        [Required(ErrorMessage = "Space type is Required.")]
        [Display(Name = "Space Type")]
        public int Spacetype { get; set; }

		[Required(ErrorMessage = "Space type(Unlisted) is Required.")]
        [Display(Name = "Space Type(Unlisted)")]
        public string  SpaceTypeUnlisted { get; set; }

		[Required(ErrorMessage = "BCA Classification is Required.")]
        [Display(Name = "BCA Classification")]
        public int? BCAClassification { get; set; }

		[Required(ErrorMessage = "Baseline/Upgrade is Required.")]
        [Display(Name = "Baseline/Upgrade")]
        public int BaselineUpgrade { get; set; }

		[Required(ErrorMessage = "Lamp Ballast Combination is Required.")]
        [Display(Name = "Lamp Ballast Combination")]
        public int LampBallastCombination { get; set; }

        [Display(Name = "Lamp category")]
        [Required(ErrorMessage = "Lamp Category Required is Required.")]
        public int Lampcategory { get; set; }
              
		[Required(ErrorMessage = "Quantity is Required.")]			  
        public decimal Quantity { get; set; }

		[Required(ErrorMessage = "Baseline Asset Lifetime Reference is Required.")]
        [Display(Name = "Baseline Asset Lifetime Reference")]
        public int BaselineAssetLifetimeReference { get; set; }

		[Required(ErrorMessage = "Upgrade Asset Lifetime Reference is Required.")]
        [Display(Name = "Upgrade Asset Lifetime Reference")]
        public int UpgradeAssetLifetimeReference { get; set; }

		[Required(ErrorMessage = "Product Brand is Required.")]
        [Display(Name = "Product Brand")]
        public string ProductBrand { get; set; }

		[Required(ErrorMessage = "Product Model is Required.")]
        [Display(Name = "Product Model")]
        public string ProductModel { get; set; }

		[Required(ErrorMessage = "Rated Lifetime Hours is Required.")]
        [Display(Name = "Rated Lifetime Hours")]
        public decimal RatedLifetimeHours { get; set; }

		[Required(ErrorMessage = "Nominal Lamp Power is Required.")]
        [Display(Name = "Nominal Lamp Power")]
        public decimal NominalLampPower { get; set; }

		[Required(ErrorMessage = "Type of First Controller is Required.")]
        [Display(Name = "Type of First Controller")]
        public int TypeOfFirstController { get; set; }

		//[Required(ErrorMessage = "Type of Second Controller is Required.")]
        [Display(Name = "Type of Second Controller")]
        public int TypeOfSecondController { get; set; }

		//[Required(ErrorMessage = "VRU Product Brand is Required.")]
        [Display(Name = "VRU Product Brand")]
        public string VRUProductBrand { get; set; }

		//[Required(ErrorMessage = "VRU Product Model is Required.")]
        [Display(Name = "VRU Product Model")]
        public string VRUProductModel { get; set; }

        [Display(Name = "HVAC A/C?")]
        public bool HVACAC { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }
		
		public int CalcZone { get; set; }
		
		public string SpaceTypeName { get; set; }       
        public string BCAClassificationName { get; set; }
        public string LampBallastCombinationName { get; set; }
        public string LampCategoryName { get; set; }
        public string BaselineAssetLifetimeReferenceName { get; set; }
        public string UpgradeAssetLifetimeReferenceName { get; set; }
        public string ProductBrandName { get; set; }
        public string ProductModelName { get; set; }
        public string FirstControllerTypeName { get; set; }
        public string SecondControllerTypeName { get; set; }
        public string VRUProductBrandName { get; set; }
        public string VRUProductModelName { get; set; }

        public string VEECAreaName { get; set; }

        public List<VEECNonJ6Scenario> lstVEECNonJ6Scenario { get; set; }
    }
}
