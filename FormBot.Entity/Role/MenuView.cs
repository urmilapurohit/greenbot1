using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class MenuView
    {
        public int MenuId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int ParentID { get; set; }
        public string CheckboxId { get; set; }
        public int SortOrder { get; set; }
        public int SubMenuParentID { get; set; }
        public string Title { get; set; }

        public string Controller { get; set; }
        public string Action { get; set; }
        public string Image { get; set; }
        public bool WithoutSubMenu { get; set; }
    }
}
