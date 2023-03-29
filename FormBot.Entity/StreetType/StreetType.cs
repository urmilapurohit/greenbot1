using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class StreetType
    {
        public int StreetTypeID { get; set; }
        public string StreetTypeName { get; set; }
    }
}
