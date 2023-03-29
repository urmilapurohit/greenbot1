using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Dashboard
{
    public class DashboardJobList
    {
        public int JobID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CompanyName { get; set; }
        public string ClientDetails { get; set; }
        public string Description { get; set; }
        public bool IsAccept { get; set; }
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
