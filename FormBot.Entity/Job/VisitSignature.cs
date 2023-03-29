using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class VisitSignature
    {
        public int VisitSignatureId { get; set; }
        public int CheckListItemId { get; set; }
        public int JobSchedulingId { get; set; }
        public int JobId { get; set; }
        public string Path { get; set; }
        public int SignatureTypeId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string IpAddress { get; set; }
        public string Location { get; set; }
        public string Image { get; set; }
    }
}

