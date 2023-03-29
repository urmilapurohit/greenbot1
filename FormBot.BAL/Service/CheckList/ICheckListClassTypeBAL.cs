using FormBot.DAL;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using FormBot.Entity.CheckList;
using System.Linq;
using System;

namespace FormBot.BAL.Service.CheckList
{
    public interface ICheckListClassTypeBAL
    {
        /// <summary>
        /// Gets the data
        /// </summary>
        /// <returns></returns>
        List<CheckListClassType> GetData(bool isSetFromSetting);
        /// <summary>
        /// get checklist photo type
        /// </summary>
        /// <returns></returns>
        List<CheckListPhotoType> GetPhototype();
    }
}
