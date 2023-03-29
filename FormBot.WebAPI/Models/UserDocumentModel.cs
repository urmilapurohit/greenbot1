using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FormBot.WebAPI.Models
{
    public class UserDocumentModel
    {
        public string DocumentPath { get; set; }
        public int Index { get; set; }
        public string MimeType { get; set; }
        public string StrDocumentPath { get; set; }
        public int UserDocumentID { get; set; }
        public int UserID { get; set; }
        public int Status { get; set; }
        public int JobID { get; set; }

        public int JobSchedulingId { get; set; }

        public int VisitCheckListItemId { get; set; }

        public bool IsClassic { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public string ImageTakenDate { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
    }
}