using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FormBot.Entity
{
    public class JobInverterDetails
    {
        public int JobInverterID { get; set; }
        public int JobID { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Series { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }

        public string ModelNumber { get; set; }
        public string Manufacturer { get; set; }

        public int? NoOfInverter { get; set; }
        public string CECApprovedDate { get; set; }
        public string ExpiryDate { get; set; }

    }
    [XmlRoot(ElementName = "Inverter")]
    public class xmlInverter
    {
        [XmlElement("Brand")]
        public string Brand { get; set; }
        [XmlElement("Model")]
        public string Model { get; set; }
        [XmlElement("Series")]
        public string Series { get; set; }
    }
}
