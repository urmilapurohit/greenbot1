using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.KendoGrid
{
    [Serializable()]
    public class UserWiseGridConfiguration
    {
        public int UserWiseGridConfigurationId { get; set; }
        public int  UserId { get; set; }
        public int  PageSize { get; set; }
        public bool IsKendoGrid { get; set; }
        public int  ViewPageId { get; set; }
    }
}
