using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FormBot.Entity
{
    public class ResellerDetailView
    {
        public int ResellerID { get; set; }

        public string ResellerName { get; set; }

        public string Logo { get; set; }

        public int Theme { get; set; }

        public string LoginCompanyName { get; set; }

    }
}
