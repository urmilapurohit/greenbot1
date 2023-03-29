using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class SpvInstallationVerification
    {
        public string ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string StreetType { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public bool ManuallyEntered { get; set; }
        public List<SpvVerificationProduct> ProductList { get; set; }
        public List<SpvPanelDetails> ModelList { get; set; }
        //public List<SpvVerificationModel> ModelList { get; set; }
    }

    //public class SpvInstallationVerificationProduct
    //{
    //    public string SerialNumber { get; set; }
    //    public string Manufacturer { get; set; }
    //    public string ModelNumber { get; set; }
    //    public string Supplier { get; set; }
    //}

}
