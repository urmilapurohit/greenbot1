using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class VEECLightingComponent
    {
        public int VEECLightingComponentId { get; set; }

        [Display(Name = "Lighting Component Name")]
        [Required(ErrorMessage = "Lighting Component Name Is Required")]
        public string LightingComponentName { get; set; }

        public string LightingComponentFilePath { get; set; }

        public int SolarCompanyId { get; set; }

        public DateTime CreatedDate{ get; set; }

        public int CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }

        public int TotalRecords { get; set; }

        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.VEECLightingComponentId));
            }
        }
    }
}
