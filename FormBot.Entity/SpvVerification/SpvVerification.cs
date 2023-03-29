using FormBot.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class SpvPanelDetails
    {
        public int SpvPanelDetailsId { get; set; }
        public int SpvPanelManufacturerId { get; set; }
        public string Manufacturer { get; set; }
        public string ModelNumber { get; set; }
        public string SerialNumber { get; set; }
        public int Wattage { get; set; }
        public DateTime EndOfWarranty { get; set; }
        public string WarrantyDescription { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string ContactEmail { get; set; }
        public decimal VOC { get; set; }
        public decimal ISC { get; set; }
        public decimal PM { get; set; }
        public decimal VM { get; set; }
        public decimal IM { get; set; }
        public decimal FF { get; set; }
        public string BillOfMaterials { get; set; }
        public string FactoryName { get; set; }
        public string FactoryLocation { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsValid { get; set; }
        public int Result { get; set; }
        public bool IsInstallationVerified { get; set; }
        public int IsVerified { get; set; }
    }
    public class SpvPanelDetailsModel : SpvPanelDetails
    {
        public string ReferenceId { get; set; }
        public string Manufacturer { get; set; }
        public string Supplier { get; set; }
    }

    public class SpvPanelManufacturer
    {
        public int SpvPanelManufacturerId { get; set; }

        public string Manufacturer { get; set; }

    }
    public class SpvVerificationRequest
    {
        public string RequestorName { get; set; }
        public string RequestorABN { get; set; }
       
        public List<SpvVerificationProduct> ProductList { get; set; }
        //public List<SpvVerificationModel> ModelList { get; set; }
        public List<SpvPanelDetails> ModelList { get; set; }
        //public SpvVerificationRequest() { ProductList = new List<SpvVerificationProduct>(); ModelList = new List<SpvVerificationModel>(); }
        public SpvVerificationRequest() { ProductList = new List<SpvVerificationProduct>(); ModelList = new List<SpvPanelDetails>(); }

    }
}
