using FormBot.BAL.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PriorityWindowService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new PriorityService() 
            };
            ServiceBase.Run(ServicesToRun);

            //List<string> lstFailureReasons = new List<string>();

            //DataTable dtReason = new DataTable();
            //dtReason.Clear();
            //dtReason.Columns.Add("jobId");
            //dtReason.Columns.Add("reason");
            //dtReason.Columns.Add("completedTime");

            //// Get Failure reasons from REC using PVDCode 
            //RECRegistry.AuthenticateUser_UploadFileForREC(ref lstFailureReasons);

            ////Insert into Reason and JobReason Table
            //foreach (var reason in lstFailureReasons)
            //{
            //    dtReason.Rows.Add(1, reason, DateTime.Now);
            //}
            //if (dtReason != null && dtReason.Rows.Count > 0) {
            //    CreateJobBAL objCreateJobBAL = new CreateJobBAL(); ;
            //    objCreateJobBAL.InsertRECFailureJobReason(dtReason);
            //}

            //RECRegistry.AuthenticateUser_UploadFileForREC(ref lstFailureReasons);

            //PriorityService objPriorityService = new PriorityService();
            //objPriorityService.FetchResellerNewMail();
            //ps.UpdatePriorityForJobs();
        }
    }
}
