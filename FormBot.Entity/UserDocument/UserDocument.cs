using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FormBot.Helper;

namespace FormBot.Entity
{
    [Serializable]
    public class UserDocument
    {
        public int UserDocumentID { get; set; }
        public int UserID { get; set; }
        public string DocumentPath { get; set; }

        public string MimeType { get; set; }
        public int index { get; set; }
        public string strDocumentPath { get; set; }

        public int Status { get; set; }

        public int JobSchedulingId { get; set; }

        public int VisitCheckListItemId { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public string ImageTakenDate { get; set; }

        public int DocumentType { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public int DocLoc { get; set; }
        public string DocLocStr { get; set; }

    }
}
