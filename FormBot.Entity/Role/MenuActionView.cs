using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class MenuActionView
    {
        public int MenuActionId { get; set; }
        public int MenuId { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
    }
}
