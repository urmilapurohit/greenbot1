using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity;

namespace FormBot.BAL.Service
{
    public interface IUnitTypeBAL
    {

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns>unit list</returns>
        List<UnitType> GetData();

        /// <summary>
        /// Gets the unitname by unit identifier.
        /// </summary>
        /// <param name="unitId">The unit identifier.</param>
        /// <returns>Returns Unit Name</returns>
        string GetUnitnameByUnitId(int unitId);
    }
}
