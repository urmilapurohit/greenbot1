using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity;

namespace FormBot.BAL.Service
{
    public interface IResellerDetailBAL
    {
        List<ResellerDetailView> GetData();
    }
}
