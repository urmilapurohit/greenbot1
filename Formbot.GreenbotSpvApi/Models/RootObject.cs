using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Formbot.GreenbotSpvApi.Models
{
   
    public class RootObject
    {
        public localities localities { get; set; }
    }

    public class localities
    {
        public List<locality> locality { get; set; }
    }

    public class locality
    {
        public string location { get; set; }
        public string state { get; set; }
        public string postcode { get; set; }
    }
}