using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class SolarElectricianView
    {
        [DisplayName("First Name:")]
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [DisplayName("Last Name:")]
        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        [DisplayName("Unit Type:")]
        //[Required(ErrorMessage = "Unit Type is required.")]
        [RequiredIf("IsSendRequest", RequiredIfAttribute.Operator.EqualTo, false, ErrorMessage = "Unit Type is required.")]
        public int UnitTypeID { get; set; }

        [DisplayName("Unit Number:")]
        //[Required(ErrorMessage = "Unit Number is required.")]
        [RequiredIf("IsSendRequest", RequiredIfAttribute.Operator.EqualTo, false, ErrorMessage = "Unit Number is required.")]
        public string UnitNumber { get; set; }

        [DisplayName("Street Number:")]
        //[Required(ErrorMessage = "Street Number is required.")]
        [RequiredIf("IsSendRequest", RequiredIfAttribute.Operator.EqualTo, false, ErrorMessage = "Street Number is required.")]
        public string StreetNumber { get; set; }

        [DisplayName("Street Name:")]
        //[Required(ErrorMessage = "Street Name is required.")]
        [RequiredIf("IsSendRequest", RequiredIfAttribute.Operator.EqualTo, false, ErrorMessage = "Street Name is required.")]
        public string StreetName { get; set; }

        [DisplayName("Street Type:")]
        //[Required(ErrorMessage = "Street Type is required.")]
        [RequiredIf("IsSendRequest", RequiredIfAttribute.Operator.EqualTo, false, ErrorMessage = "Street Type is required.")]
        public int StreetTypeID { get; set; }

        [DisplayName("Town/Suburb:")]
        //[Required(ErrorMessage = "Town/Suburb is required.")]
        [RequiredIf("IsSendRequest", RequiredIfAttribute.Operator.EqualTo, false, ErrorMessage = "Town/Suburb is required.")]
        public string Town { get; set; }

        [DisplayName("State:")]
        //[Required(ErrorMessage = "State is required.")]
        [RequiredIf("IsSendRequest", RequiredIfAttribute.Operator.EqualTo, false, ErrorMessage = "State is required.")]
        public string State { get; set; }

        [DisplayName("PostCode:")]
        //[Required(ErrorMessage = "PostCode is required.")]
        [RequiredIf("IsSendRequest", RequiredIfAttribute.Operator.EqualTo, false, ErrorMessage = "PostCode is required.")]
        public string PostCode { get; set; }

        [DisplayName("CEC Accreditation Number:")]
        [Required(ErrorMessage = "CEC Accreditation Number is required.")]
        public string CECAccreditationNumber { get; set; }

        [DisplayName("CEC Designer Number:")]
        [RequiredIf("IsSendRequest", RequiredIfAttribute.Operator.EqualTo, false, ErrorMessage = "CEC Designer Number is required.")]
        public string CECDesignerNumber { get; set; }

        public int CreatedBy { get; set; }

        public int ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }

        public int? UserId { get; set; }

        public int SolarCompanyId { get; set; }

        public bool IsPostalAddress { get; set; }

        [DisplayName("Postal Delivery Type:")]
        //[Required(ErrorMessage = "Postal Delivery Type is required.")]
        [RequiredIf("IsSendRequest", RequiredIfAttribute.Operator.EqualTo, false, ErrorMessage = "Postal Delivery Type is required.")]
        public int PostalAddressID { get; set; }

        [DisplayName("Postal Delivery Number:")]
        //[Required(ErrorMessage = "Postal Delivery Number is required.")]
        [RequiredIf("IsSendRequest", RequiredIfAttribute.Operator.EqualTo, false, ErrorMessage = "Postal Delivery Number is required.")]
        public string PostalDeliveryNumber { get; set; }

        //[RequiredIf("IsSendRequest", RequiredIfAttribute.Operator.EqualTo, false)]
        public int AddressID { get; set; }

        [NotMapped]
        [RequiredIf("IsSendRequest", RequiredIfAttribute.Operator.EqualTo, false, ErrorMessage = "Unit Type Name is required.")]
        public string UnitTypeName { get; set; }

        [NotMapped]
        [RequiredIf("IsSendRequest", RequiredIfAttribute.Operator.EqualTo, false, ErrorMessage = "Street Type Name is required.")]
        public string StreetTypeName { get; set; }

        [NotMapped]
        public string Code { get; set; }
        [NotMapped]
        [RequiredIf("IsSendRequest", RequiredIfAttribute.Operator.EqualTo, false, ErrorMessage = "Postal Delivery Type is required.")]
        public string PostalDeliveryType { get; set; }

        [NotMapped]
        public bool IsActiveDiv { get; set; }
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email.")]
        [RequiredIf("IsSendRequest", RequiredIfAttribute.Operator.EqualTo, false, ErrorMessage = "Email is required.")]
        [DisplayName("Email:")]
        [NotMapped]
        public string Email { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "SE Role is required.")]
        public int SEDesignRoleId { get; set; }

        public int SERole { get; set; }

        [DisplayName("Licensed Electrician Number:")]
        [RequiredIf("IsSendRequest", RequiredIfAttribute.Operator.EqualTo, false, ErrorMessage = "Electrical Contractors License Number is required.")]
        public string ElectricalContractorsLicenseNumber { get; set; }

		public int SolarElectricianId { get; set; }

        public bool IsSendRequest { get; set; }

        public int SendBy { get; set; }

        public bool IsInstallerDesignerAddByProfile { get; set; }
        public string SESignature { get; set; }
        public int UserTypeID { get; set; }

        [DisplayName("SWH License Number:")]
        [Required(ErrorMessage = "SWH License Number is required.")]
        public string SWHLicenseNumber { get; set; }

        public int ElectricianStatusId { get; set; }
		public bool IsSWHUser { get; set; }
		public bool IsPVDUser { get; set; }
        public bool IsAutoRequest { get; set; }

	}
}
