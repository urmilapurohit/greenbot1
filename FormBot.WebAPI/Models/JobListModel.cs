using FormBot.Entity;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FormBot.WebAPI.Models
{
    public class JobListModel
    {
        public int JobID { get; set; }

        public int Urgent { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Priority { get; set; }

        public string ClientName { get; set; }

        public string StaffName { get; set; }

        public string Phone { get; set; }

        public string JobAddress { get; set; }
        public string JobStage { get; set; }

        public string JobType { get; set; }

        public long? InstallationDateTick { get; set; }

        public decimal STC { get; set; }

        public int UserTypeID { get; set; }

        public int? ResellerID { get; set; }

        public int? SolarCompanyId { get; set; }

        public List<JobStage> LstJobStages { get; set; }
        [NotMapped]
        public long? StartDateTick { get; set; }

        [NotMapped]
        public long? EndDateTick { get; set; }

        [NotMapped]
        public long? StartTimeTick { get; set; }

        [NotMapped]
        public long? EndTimeTick { get; set; }

        [NotMapped]
        public long? CreatedDateTick { get; set; }

        public int CreatedBy { get; set; }

        [NotMapped]
        public long? ModifiedDateTick { get; set; }

        public int ModifiedBy { get; set; }

        [NotMapped]
        public int TotalRecords { get; set; }

        [NotMapped]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UserID { get; set; }
        public string SSCJobID { get; set; }
    }
}