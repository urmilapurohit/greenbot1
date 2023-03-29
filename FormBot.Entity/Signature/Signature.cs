using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Signature
{
    public class Signature
    {
        public string MobileNumber { get; set; }

        public string UserId { get; set; }

        public string Type { get; set; }

        public string EmailFrom { get; set; }

        public string EmailTo { get; set; }

        public string SinatureType { get; set; }

        public string JobId { get; set; }

    }

    public class SignatureApproved
    {
        public string MobileNumber { get; set; }

        public string UserId { get; set; }

        public string Type { get; set; }

        public string EmailFrom { get; set; }

        public string EmailTo { get; set; }

        public string SinatureType { get; set; }

        public string JobId { get; set; }

    }
}
