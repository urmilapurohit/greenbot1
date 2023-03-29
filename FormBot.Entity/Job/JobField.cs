using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class JobField
    {
        public int JobFieldId { get; set; }
        public string FieldName { get; set; }
        public string FieldLabel { get; set; }
        public string TableName { get; set; }
        public string PropertyName { get; set; }
    }
}
