using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class JobScheduleCalendar
    {
        public List<SolarElectricianView> solarElectrician { get; set; }

        public List<JobDetails> job { get; set; }
    }
}
