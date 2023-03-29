using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service.VEECLightingComponent
{
    public interface IVEECLightingComponentBAL
    {
        int SaveLightingComponentTemplate(string veecLightingComponentId, string veecLightingComponentName,string path);

        IList<FormBot.Entity.VEECLightingComponent> GetLightingComponentList(int pageNumber, int pageSize, string sortCol, string sortDir, string docTemplateName);

        void DeleteLightingComponent(string deleteLightingComponentIds);

        string GetLightingComponentFilePath(int VEECLightingComponentId);
    }
}
