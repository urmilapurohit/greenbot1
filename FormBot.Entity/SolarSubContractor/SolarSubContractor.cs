using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class SolarSubContractor
    {
        public int JobID { get; set; }
        public string JobDetails { get; set; }
        public DateTime SSCRemoveDate { get; set; }
        public string CompanyName { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public int SSCID { get; set; }
        public bool IsSSCRemove { get; set; }
        [NotMapped]
        public int TotalRecords { get; set; }
        [NotMapped]
        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.JobID));
            }
        }

        public string RequestedBy { get; set; }
    }
}
