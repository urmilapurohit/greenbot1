using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity;

namespace FormBot.BAL.Service
{
    public interface IUserTypeBAL
    {

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns>user list</returns>
        List<UserType> GetData();
    }
}
