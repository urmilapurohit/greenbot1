using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FormBot.WebAPI.Models
{
    public class JobScheduleModel
    {
        public int JobSchedulingID { get; set; }
        public int JobID { get; set; }
        public string JobTitle { get; set; }
        public string SESCName { get; set; }
        public int? SSCID { get; set; }
        public string RefNumber { get; set; }
        public string Priority { get; set; }
        public string Location { get; set; }
        public string Client { get; set; }
        public string JobStage { get; set; }
        public int UserId { get; set; }
        public string Label { get; set; }
        public string Detail { get; set; }
        public int Diff { get; set; }
        public string StrVisitStartDate { get; set; }
        public string StrVisitStartTime { get; set; }
        public string StrVisitEndDate { get; set; }
        public string StrVisitEndTime { get; set; }
        public string CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public string ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public int Status { get; set; }
        public int SolarCompanyId { get; set; }
    }
}