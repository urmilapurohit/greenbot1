using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FormBot.WebAPI.Models
{
    public class SPVProductVerification
    {
        public string SerialNumber { get; set; }

        public string Manufacturer { get; set; }

        public string ModelNumber { get; set; }

        public bool IsVerify { get; set; }
    }
}