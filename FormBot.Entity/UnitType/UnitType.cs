using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class UnitType
    {
        public int UnitTypeID { get; set; }
        public string UnitTypeName { get; set; }
    }
}
