using FormBot.Entity;
using FormBot.Entity.Job;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FormBot.WebAPI.Models
{
    public class JobSchedulingModel
    {
        public string Id { get; set; }
        public int JobSchedulingID { get; set; }
        public int JobID { get; set; }
        public int UserId { get; set; }
        public string Detail { get; set; }
        public DateTime VisitStartDate { get; set; }
        public TimeSpan VisitStartTime { get; set; }
        public DateTime VisitEndDate { get; set; }
        public TimeSpan VisitEndTime { get; set; }
        public string StrVisitStartDate { get; set; }
        public string StrVisitEndDate { get; set; }
        public string StrVisitStartTime { get; set; }
        public string StrVisitEndTime { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public string UserName { get; set; }
        public string StatusName { get; set; }
        public long? CreatedDateTick { get; set; }
        public long? StartDateTick { get; set; }
        public long? StartTimeTick { get; set; }
        public long? EndDateTick { get; set; }
        public long? EndDateTime { get; set; }
        public int JobVisitID { get; set; }
        
    }
    
}