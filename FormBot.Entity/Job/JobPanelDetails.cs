using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FormBot.Entity
{
    public class JobPanelDetails
    {
        public int JobPanelID { get; set; }
        public int JobID { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
      
        public int? NoOfPanel { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsSPVRequired { get; set; }
        public string CertificateHolder { get; set; }
        public string ModelNumber { get; set; }
        public string Supplier { get; set; }
        public string CECApprovedDate { get; set; }
        public string ExpiryDate { get; set; }

    }

    [XmlRoot(ElementName = "Panel")]
    public class xmlPanel
    {
        [XmlElement("Brand")]
        public string Brand { get; set; }
        [XmlElement("Model")]
        public string Model { get; set; }
        [XmlElement("NoOfPanel")]
        public int? NoOfPanel { get; set; }
    }
}
