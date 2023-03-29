using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public  class JobDocument
    {
        public int JobDocumentId { get; set; }
        public int JobId { get; set; }
        public int DocumentId { get; set; }
        public int isUpload { get; set; }
    }
}
