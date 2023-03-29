using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class ServiceResponse: ServiceResponseSerialNumberUpdateAPI
    {
        public object Data { get; set; }
        public string ApiToken { get; set; }
        public string Error { get; set; }
        public string ExceptionMessage { get; set; }
        public bool Status { get; set; }
    }
    public class ServiceResponseSerialNumberUpdateAPI 
    {
        public bool WithSpvVerified { get; set; }
        //for return photo id from portal db to app
        public int VisitChecklistPhotoId { get; set; }
    }
}
