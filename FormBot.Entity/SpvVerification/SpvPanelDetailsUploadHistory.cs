using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.SpvVerification
{
    public class SpvPanelDetailsUploadHistory
    {
        public int SpvPanelDetailsUploadId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public int ManufacturerId { get; set; }
        public int UploadedBy { get; set; }
        public DateTime UploadedOn { get; set; }
        [NotMapped]
        public string ManufacturerName { get; set; }
    }
}
