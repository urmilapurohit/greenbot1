using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class JobSCAandRA
    {
        [Required(ErrorMessage = "SolarCompany is required.")]
        public int SolarComapnyID { get; set; }
        [Required(ErrorMessage = "Reseller is required.")]
        public int ResellerID { get; set; }
        [Required(ErrorMessage = "JobID is required.")]
        public string JobIDs { get; set; }
    }
}
