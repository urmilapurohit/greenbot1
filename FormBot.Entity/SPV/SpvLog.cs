using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FormBot.Entity.SPV
{
    public class SpvLog
    {
        public int SPVLogId { get;set;}
        public string SerialNumber{get;set;}
        public int JobId{get;set;}
        public int SPVMethod {get;set;}
        public int VerificationStatus {get;set;}
        public string ResponseCode {get;set;}
        public DateTime? ResponseTime {get;set;}
        public string ServiceAdministrator {get;set;}
        public DateTime RequestTime{get;set;}
        public string Manufacturer {get;set;}
        public string ModelNumber { get; set; }
        public string Supplier { get;set;}
        public string ResponseMessage {get;set;}
		public int TotalRecordCount { get; set; }
        public List<SpvLog> lstSpvlog { get; set; }
        [NotMapped]
        public string EncryptedJobId
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.JobId));
            }
        }

    }
    [XmlRoot(ElementName = "Error")]
    public class SpvErrorCode
    {
        [XmlElement("Code")]
        public int Code { get; set; }
        [XmlElement("Description")]
        public string Description { get; set; }
        [XmlElement("Details")]
        public string Details { get; set; }
    }
    public class SPVLogDetail
    {
        public int JobID { get; set; }
        public string Refnumber { get; set; }
        public DateTime RequestTime { get; set; }
        public string JobAddress { get; set; }
        public string InstallerName { get; set; }
        public string Manufacturer { get; set; }
        public string modelNumber { get; set; }
        public string ResellerName { get; set; }
        public string CompanyName { get; set; }
        public int NoOfPanel { get; set; }
        public string SerialNumber { get; set; }
        public int SPVMethod { get; set; }
        public string ServiceAdministrator { get; set; }
        public string ResponseMessage { get; set; }
        public int SCAUserId { get; set; }
        [NotMapped]
        public string EncryptedJobId
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.JobID));
            }
        }
        [NotMapped]
        public string EncryptedSolarCompanyId
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.SCAUserId));
            }
        }
    }
}
