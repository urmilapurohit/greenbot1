using FormBot.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service
{
    public interface IVEECPricingManagerBAL
    {
        FormBot.Entity.VEECPricingManager GetVEECGlobalPriceForReseller(int ResellerId);

        List<FormBot.Entity.VEECPricingManager> GetSCAUserForVEECPricingManager(int UserId, int UserTypeId, int PageNumber, int PageSize, string SortCol, string SortDir, int ResellerId, int RAMID, string SolarCompany, string Name);

        List<FormBot.Entity.VEECPricingManager> GetVEECsForVEECPricingManager(int UserId, int UserTypeId, int PageNumber, int PageSize, string SortCol, string SortDir, int ResellerId, int RAMID, int SolarCompanyId, string veecRef, string HomeOwnerName, string VeecInstallationAddress);

        void SaveVEECGlobalPriceForSolarCompany(int ResellerId, decimal optiPayPrice);

        void SaveVEECCustomPriceForSolarCompany(List<int> lstSolarCompany, DateTime ExpiryDate, int ResellerId, decimal optiPayPrice);

        void SaveVEECCustomPriceForVeec(List<int> lstVEEC, DateTime ExpiryDate, int SolarCompanyId, decimal optiPayPrice);

        List<VEECList> GetVEECsForCustomPricing(int SolarCompanyId, string OwnerName, string InstallationAddress, string RefNumber);
    }
}
