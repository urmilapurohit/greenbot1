using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class NotificationViewModel
    {
        public int NotificationId { get; set; }
        public int JobId { get; set; }
        public string SendByName { get; set; }
        public string JobLink { get; set; }
        public string Notes { get; set; }
    }
}
