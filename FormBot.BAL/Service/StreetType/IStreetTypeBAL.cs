using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity;

namespace FormBot.BAL.Service
{
    public interface IStreetTypeBAL
    {

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns>street list</returns>
        List<StreetType> GetData();

        /// <summary>
        /// Gets the street type name by street type identifier.
        /// </summary>
        /// <param name="streetTypeId">The street type identifier.</param>
        /// <returns>Returns Street type name</returns>
        string GetStreetTypeNameByStreetTypeId(int streetTypeId);
    }
}
