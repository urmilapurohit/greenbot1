using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity;

namespace FormBot.BAL.Service.SolarElectrician
{
    public interface ISolarElectricianBAL
    {
        //DataSet GetAllElectrician(bool isInstaller, int solarCompanyId);

        List<SolarElectricianView> GetAllElectrician(bool isInstaller, int solarCompanyId);
    }
}
