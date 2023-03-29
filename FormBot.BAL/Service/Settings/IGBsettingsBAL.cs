using FormBot.Entity.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service
{
    public interface IGBsettingsBAL
    {
        GBsetting GetGBsettingValueByKeyName(string KeyName);
        void SetGBsettingValueByKeyName(string KeyName,string Value,int UserID);
    }
}
