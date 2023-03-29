﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Email
{
    [Serializable]
    public class JobWiseUsers
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public int UserTypeID { get; set; }
    }
}
