using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    //public class SpvRequiredSolarCompanyWise
    //{
    //    //public int SolarCompanyId { get; set; }
    //    public int JobId { get; set; }
    //    public int STCJobDetailsID { get; set; }
    //    public bool SCORGlobalIsAllowedSPV { get; set; }
    //}
    public class GetPhotoFromPortalWithNullLatLongIsdeletedFlagApiRequest
    {
        public int VisitCheckListPhotoId { get; set; }
       
    }
    public class CheckSPVrequired
    {
        public int JobId { get; set; }
        public int STCJobDetailsID { get; set; }
        public bool IsSPVRequired { get; set; }
    }
}
