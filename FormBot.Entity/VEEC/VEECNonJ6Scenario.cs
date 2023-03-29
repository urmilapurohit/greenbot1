using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.VEEC
{
    public class VEECNonJ6Scenario
    {

        public int VEECNonJ6ScenarioId { get; set; }
        public string ProjectForm { get; set; }

        public string Scenario { get; set; }

        public int BaselineAssetLifetimeReferenceId { get; set; }

        public int UpgradeAssetLifetimeReferenceId { get; set; }
    }
}
