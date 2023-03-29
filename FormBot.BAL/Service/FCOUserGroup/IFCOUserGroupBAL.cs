using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FormBot.BAL.Service
{
    public interface IFCOUserGroupBAL
    {
        /// <summary>
        /// Gets the fco user group list.
        /// </summary>
        /// <param name="FCOGroupId">The fco group identifier.</param>
        /// <returns>select list</returns>
        List<SelectListItem> GetFCOUserGroupList(int FCOGroupId);

        /// <summary>
        /// Deletes the specified fco group identifier.
        /// </summary>
        /// <param name="FCOGroupId">The fco group identifier.</param>
        void Delete(int FCOGroupId);
    }
}
