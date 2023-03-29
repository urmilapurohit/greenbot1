using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriorityWindowService
{
    public class RECData
    {
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("result")]
        public List<Result> result { get; set; }

        public class Result
        {
            [JsonProperty("actionType")]
            public string actionType { get; set; }
            [JsonProperty("completedTime")]
            public string completedTime { get; set; }
            [JsonProperty("certificateRanges")]
            public List<CertificateRanges> certificateRanges { get; set; }

        }

        public class CertificateRanges
        {
            [JsonProperty("certificateType")]
            public string certificateType { get; set; }
            [JsonProperty("registeredPersonNumber")]
            public string registeredPersonNumber { get; set; }
            [JsonProperty("accreditationCode")]
            public string accreditationCode { get; set; }
            [JsonProperty("generationYear")]
            public string generationYear { get; set; }
            [JsonProperty("generationState")]
            public string generationState { get; set; }
            [JsonProperty("startSerialNumber")]
            public string startSerialNumber { get; set; }
            [JsonProperty("endSerialNumber")]
            public string endSerialNumber { get; set; }
            [JsonProperty("fuelSource")]
            public string fuelSource { get; set; }
            [JsonProperty("ownerAccount")]
            public string ownerAccount { get; set; }
            [JsonProperty("ownerAccountId")]
            public string ownerAccountId { get; set; }
            [JsonProperty("status")]
            public string status { get; set; }
        }
       
    }   
}
