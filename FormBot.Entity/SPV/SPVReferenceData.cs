using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.SPV
{

    public class Supplier
    {
        public string name { get; set; }
        public string id { get; set; }
        public string publickeyid { get; set; }
        public string startdate { get; set; }
        public string enddate { get; set; }
        public string endpointid { get; set; }
    }

    public class Manufacturer
    {
        public string name { get; set; }
        public List<Supplier> suppliers { get; set; }
    }

    public class Endpoint
    {
        public string id { get; set; }
        public string owner { get; set; }
        public string productverification { get; set; }
        public string installverification { get; set; }
        public string serialtomodellookup { get; set; }
        public string serviceadministrator { get; set; }
    }

    public class App
    {
        public string id { get; set; }
        public string owner { get; set; }
        public string appname { get; set; }
        public string publickeyid { get; set; }
    }

    public class Publickey
    {
        public string id { get; set; }
        public string subject { get; set; }
        public string serialnumber { get; set; }
        public string validto { get; set; }
        public string publickey { get; set; }
    }

    public class SPVReferenceJson
    {
        public List<Manufacturer> manufacturers { get; set; }
        public List<Endpoint> endpoints { get; set; }
        public List<App> apps { get; set; }
        public List<Publickey> publickeys { get; set; }
    }
    public class SPVFailureReason
    {
        public int JobID { get; set; }
        public string RefNumber { get; set; }
        public string HistoryMessage { get; set; }
        public string ModifiedBy { get; set; }
        public string HistoryCategory { get; set; }
        public string CreateDate { get; set; }
        public List<SPVFailureReason> lstSPVHistory { get; set; }
        public List<SPVFailureReason> lstHistoryJobId { get; set; }
        [NotMapped]
        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.JobID));
            }
        }


    }

}
