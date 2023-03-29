using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class SpvMenuActionView
    {
        public int SpvMenuActionId { get; set; }
        public int SpvMenuId { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
    }
}
