using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Email
{
    public class EmailFolder
    {
        public long id_folder { get; set; }
        public int id_acct { get; set; }
        public long id_parent { get; set; }
        public int type { get; set; }
        public string name { get; set; }
        public string full_path { get; set; }
        public int sync_type { get; set; }
        public bool hide { get; set; }
        public int fld_order { get; set; }
    }
}
