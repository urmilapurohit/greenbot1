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
    public class Invoicer
    {
        [DisplayName("Invoicer:")]
        public string InvoicerName { get; set; }

        [NotMapped]
        [DisplayName("Invoicer First Name:")]
        public string InvoicerFirstName { get; set; }

        [DisplayName("Invoicer Last Name:")]
        public string InvoicerLastName { get; set; }

        [DisplayName("Invoicer Phone:")]
        public string InvoicerPhone { get; set; }
        [DisplayName("Unique ContactID")]
        public string UniqueContactId { get; set; }
        [DisplayName("Invoicer Postal Address Type:")]
        [Required(ErrorMessage = "Invoicer Postal Address Type is required.")]
        public int InvoicerAddressID { get; set; }
        public bool InvoicerIsPostalAddress { get; set; }
        [DisplayName("Invoicer Unit Type:")]
        ////[Required(ErrorMessage = "Unit Type is required.")]
        public int InvoicerUnitTypeID { get; set; }

        [DisplayName("Invoicer Unit Number:")]
        ////[Required(ErrorMessage = "Unit Number is required.")]
        public string InvoicerUnitNumber { get; set; }

        [DisplayName("Invoicer Street Number:")]
        [Required(ErrorMessage = "Invoicer Street Number is required.")]
        public string InvoicerStreetNumber { get; set; }

        [DisplayName("Invoicer Street Name:")]
        [Required(ErrorMessage = "Invoicer Street Name is required.")]
        public string InvoicerStreetName { get; set; }

        [DisplayName("Invoicer Street Type:")]
        [Required(ErrorMessage = "Invoicer Street Type is required.")]
        public int InvoicerStreetTypeID { get; set; }

        [DisplayName("Invoicer Postal Delivery Type:")]
        [Required(ErrorMessage = "Invoicer Postal Delivery Type is required.")]
        public int InvoicerPostalAddressID { get; set; }

        [DisplayName("Invoicer Postal Delivery Number:")]
        [Required(ErrorMessage = "Invoicer Postal Delivery Number is required.")]
        public string InvoicerPostalDeliveryNumber { get; set; }

        [DisplayName("Invoicer Town/Suburb:")]
        [Required(ErrorMessage = "Invoicer Town/Suburb is required.")]
        public string InvoicerTown { get; set; }

        [DisplayName("Invoicer State:")]
        [Required(ErrorMessage = "Invoicer State is required.")]
        public string InvoicerState { get; set; }

        [DisplayName("Invoicer Post Code:")]
        [Required(ErrorMessage = "Invoicer PostCode is required.")]
        public string InvoicerPostCode { get; set; }

        [DisplayName("Invoicer Company ABN:")]
        public string InvoicerCompanyABN { get; set; }
        [DisplayName("Unique Company Number:")]
        public string UniqueCompanyNumber { get; set; }
        [DisplayName("Account Code:")]
        public string AccountCode { get; set; }
        public int UserTypeID { get; set; }
        public bool isActive { get; set; }
        public int DATAOPMODE { get; set; }
        public int TotalRecords { get; set; }
        public decimal InvoicerId { get; set; }
    }
}
