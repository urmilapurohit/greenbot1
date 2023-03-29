using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity.Job;

namespace FormBot.BAL.Service.Job
{
    public interface IJobDetailsBAL
    {
        List<JobDetails> GetAllJob(int userId);

        /// <summary>
        /// Gets the job STC price.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <returns>Pricing Manager</returns>
        FormBot.Entity.PricingManager GetJobSTCPrice(string jobId);

        /// <summary>
        /// Applies the trade STC.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="STCSettlementTerm">The STC settlement term.</param>
        /// <param name="STCSettlementDate">The STC settlement date.</param>
        /// <param name="STCPrice">The STC price.</param>
        /// <param name="STCIsTraded">if set to <c>true</c> [STC is traded].</param>
        /// <param name="STCStatus">The STC status.</param>
        /// <returns>DataSet</returns>
        DataSet ApplyTradeStc(int userId, string jobId, int STCSettlementTerm, DateTime? STCSettlementDate, decimal? STCPrice, bool STCIsTraded, int STCStatus, int customSettlementTerm, decimal? peakPayGst, int? peakPayTimeperiod, decimal? peakPayFee, string STCRemark = null);//,bool IsGenerateRecZip);


        /// <summary>
        /// Get global billable terms.
        /// </summary>
        /// <param name="jobId">The user identifier.</param>
        /// <param name="UserId">The job identifier.</param>
        /// <returns>DataSet</returns>
        DataSet GetGlobalBillableTermsSAAS_PricingTerm(int jobId, int UserId);

        /// <summary>
        /// Gets the documents steps for pre approval and connection.
        /// </summary>
        /// <param name="NMI">The nmi.</param>
        /// <param name="stage">The stage.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <returns>doc list</returns>
        List<FormBot.Entity.Documents.DocumentSteps> GetDocumentsStepsForPreApprovalAndConn(string NMI,string stage,int jobId);

        /// <summary>
        /// Gets the STC basic details with status.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <returns>stc basic details</returns>
        STCBasicDetails GetStcBasicDetailsWithStatus(int JobId, int UserTypeId);

        /// <summary>
        /// Gets status details.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <returns>dataset</returns>
        DataSet GetStatusDetails(int JobId);

        /// <summary>
        /// Gets the job documents_ for email attachment by job identifier.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <returns>DataSet</returns>
        DataSet GetJobDocuments_ForEmailAttachmentByJobID(int JobId);

        /// <summary>
        /// Gets the job duplication details by job identifier.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <returns>DataSet</returns>
        DataSet GetJobDuplicationDetailsByJobID(int JobId);

        string GetInstallerDesignerElectricianSignature(int signatureTypeId, int jobId, int installerDesignerElectricianId);

        /// <summary>
        /// Gets All Installer Designer Records.
        /// </summary>
        /// <returns>
        /// dataset
        /// </returns>
        DataSet GetAllInstallerDesigner();

        /// <summary>
        /// Gets All Installer Designer Records by ids.
        /// </summary>
        /// <returns>
        /// dataset
        /// </returns>
        JobsInstallerDesignerView GetAllInstallerDesignerById(int id);

        /// <summary>
        /// Gets All Solar Company Records.
        /// </summary>
        /// <returns>
        /// dataset
        /// </returns>
        DataSet GetAllSolarCompanies();
    }
}
