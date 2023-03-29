using FormBot.Entity.VEECManageList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service.VEECManageList
{
    public interface IVEECManageListBAL
    {        
        IList<VEECProductBrands> VEECProductBrandsList(int PageNumber, int PageSize, string SortColumn, string SortDirection, string Brand, string Model, string ProductType, string ProductCategory, string TechnologyClass, string ApplicationDate, string EffectiveFrom, string EffectiveTo);

        void InsertProductBrands(DataTable dtProductBrands);
    }
}
