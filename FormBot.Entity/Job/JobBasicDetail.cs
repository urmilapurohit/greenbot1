using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class JobBasicDetail
    {
        public int JobID { get; set; }

        public DateTime InstallationDate { get; set; }

        public int NoOfPanel { get; set; }

        public string InstallationAddress { get; set; }

        public string OwnerName { get; set; }

        public string CompanyName { get; set; }

        public string CompanyABN { get; set; }

        public string Phone { get; set; }

        public string CompanyWebsite { get; set; }

        public string Model { get; set; }

        public string Brand { get; set; }

        public  string Supplier { get; set; }

        public string SerialNumber { get; set; }

        public string Email { get; set; }
    }
}
