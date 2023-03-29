using FormBot.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Entity.Job;
using FormBot.Helper;
using FormBot.Helper.Helper;

namespace FormBot.BAL.Service.Job
{
    public class JobDetailsBAL : IJobDetailsBAL
    {
        /// <summary>
        /// Gets all job.
        /// </summary>
        /// <param name="createdBy">The created by.</param>
        /// <returns>job list</returns>
        public List<JobDetails> GetAllJob(int createdBy)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.BigInt, createdBy));
            IList<JobDetails> jobDetailList = CommonDAL.ExecuteProcedure<JobDetails>("Job_GetAllJobByUserId", sqlParameters.ToArray());
            return jobDetailList.ToList();
        }

        /// <summary>
        /// Gets the job STC price.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <returns>price manager</returns>
        public FormBot.Entity.PricingManager GetJobSTCPrice(string jobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.NVarChar, jobId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            var result = CommonDAL.ExecuteProcedure<FormBot.Entity.PricingManager>("Jobs_GetJobSTCPrice", sqlParameters.ToArray()).FirstOrDefault();
            if (result == null)
            {
                FormBot.Entity.PricingManager pricing = new Entity.PricingManager();
                pricing.Hour24 = 0;
                pricing.Days3 = 0;
                pricing.Days7 = 0;
                pricing.CERApproved = 0;
                pricing.STCAmount = 0;
                pricing.HaveNotCustomPrice = true;
                return pricing;
            }
            else
            {
                result.IsApproachingExpiryDate = Convert.ToDateTime(result.InstallationDate).AddDays(325) <= DateTime.Now.Date ? true : false;
            }

            if (result.CustomSettlementTerm > 0)
            {
                result.CustomTermText = "Custom - " + Common.GetDescription((FormBot.Helper.SystemEnums.STCSettlementTerm)result.CustomSettlementTerm, "");
                result.CustomSubDescription = Common.GetSubDescription((FormBot.Helper.SystemEnums.STCSettlementTerm)result.CustomSettlementTerm, "");
            }

            return result;
        }

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
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet ApplyTradeStc(int userId, string jobId, int STCSettlementTerm, DateTime? STCSettlementDate, decimal? STCPrice, bool STCIsTraded, int STCStatus, int customSettlementTerm, decimal? peakPayGst, int? peakPayTimeperiod, decimal? peakPayFee, string STCRemark = null)//, bool IsGenerateRecZip)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, userId));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.NVarChar, jobId));
            sqlParameters.Add(DBClient.AddParameters("STCSettlementTerm", SqlDbType.Int, STCSettlementTerm));
            sqlParameters.Add(DBClient.AddParameters("STCSettlementDate", SqlDbType.DateTime, STCSettlementDate));
            sqlParameters.Add(DBClient.AddParameters("STCPrice", SqlDbType.Decimal, STCPrice));
            sqlParameters.Add(DBClient.AddParameters("STCIsTraded", SqlDbType.Bit, STCIsTraded));
            sqlParameters.Add(DBClient.AddParameters("STCStatus", SqlDbType.Int, STCStatus));
            sqlParameters.Add(DBClient.AddParameters("STCSubmissionDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("CustomSettlementTerm", SqlDbType.Int, customSettlementTerm));
            sqlParameters.Add(DBClient.AddParameters("PeakPayGst", SqlDbType.Decimal, peakPayGst));
            sqlParameters.Add(DBClient.AddParameters("PeakPayTimeperiod", SqlDbType.Int, peakPayTimeperiod));
            sqlParameters.Add(DBClient.AddParameters("PeakPayFee", SqlDbType.Decimal, peakPayFee));
            sqlParameters.Add(DBClient.AddParameters("STCRemark", SqlDbType.NVarChar, STCRemark));
            // sqlParameters.Add(DBClient.AddParameters("IsGenerateRecZip", SqlDbType.Bit, IsGenerateRecZip));
            return CommonDAL.ExecuteDataSet("JobDetails_ApplyTradeStc", sqlParameters.ToArray()); // Live or Staging
			//return CommonDAL.ExecuteDataSet("JobDetails_ApplyTradeStc_Bk", sqlParameters.ToArray()); //Local
		}


        /// <summary>
        /// Get global billable terms.
        /// </summary>
        /// <param name="jobId">The user identifier.</param>
        /// <param name="UserId">The job identifier.</param>
        /// <returns>DataSet</returns>
        public DataSet GetGlobalBillableTermsSAAS_PricingTerm(int jobId, int UserId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            //sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            return CommonDAL.ExecuteDataSet("GetGlobalBillableTermsSAAS_PricingTerm", sqlParameters.ToArray());
        }

        /// <summary>
        /// Gets the documents steps for pre approval and connection.
        /// </summary>
        /// <param name="distributorID">The distributor identifier.</param>
        /// <param name="stage">The stage.</param>
        /// <param name="jobId">The job identifier.</param>
        /// <returns>doc list</returns>
        public List<FormBot.Entity.Documents.DocumentSteps> GetDocumentsStepsForPreApprovalAndConn(string distributorID, string stage, int jobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("DistributorID", SqlDbType.NVarChar, distributorID));
            sqlParameters.Add(DBClient.AddParameters("Stage", SqlDbType.NVarChar, stage));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, jobId));
            return CommonDAL.ExecuteProcedure<FormBot.Entity.Documents.DocumentSteps>("DocumentSteps_GetByNMI", sqlParameters.ToArray()).ToList();
        }

        /// <summary>
        /// Gets the STC basic details with status.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <param name="UserTypeId">The user type identifier.</param>
        /// <returns>
        /// stc basic details
        /// </returns>
        public STCBasicDetails GetStcBasicDetailsWithStatus(int JobId, int UserTypeId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, @JobId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            STCBasicDetails stcBasicDetails = CommonDAL.ExecuteProcedure<STCBasicDetails>("GetStcBasicDetailsWithStatus", sqlParameters.ToArray()).FirstOrDefault();
            if(stcBasicDetails!= null)
            {
                if (stcBasicDetails.SettlementDate != null)
                {
                    stcBasicDetails.STCSettlementDate = Convert.ToDateTime(stcBasicDetails.SettlementDate).ToString("yyyy/MM/dd");
                }
                if (stcBasicDetails.SubmittedDate != null)
                {
                    stcBasicDetails.STCSubmittedDate = Convert.ToDateTime(stcBasicDetails.SubmittedDate).ToString("yyyy/MM/dd");
                }
            }
            else
            {
                new STCBasicDetails();
            }
            return stcBasicDetails;
        }

        /// <summary>
        /// Gets status details.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <returns>
        /// dataset
        /// </returns>
        public DataSet GetStatusDetails(int JobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, @JobId));
            DataSet ds = CommonDAL.ExecuteDataSet("GetStatusDetails", sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Gets the job documents_ for email attachment by job identifier.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet GetJobDocuments_ForEmailAttachmentByJobID(int JobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, @JobId));
            DataSet ds = CommonDAL.ExecuteDataSet("GetJobDocuments_ForEmailAttachmentByJobID", sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Gets the job duplication details by job identifier.
        /// </summary>
        /// <param name="JobId">The job identifier.</param>
        /// <returns>
        /// DataSet
        /// </returns>
        public DataSet GetJobDuplicationDetailsByJobID(int JobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, @JobId));
            DataSet ds = CommonDAL.ExecuteDataSet("GetJobDuplicationDetailsByJobID", sqlParameters.ToArray());
            return ds;
        }

        public string GetInstallerDesignerElectricianSignature(int signatureTypeId, int jobId, int installerDesignerElectricianId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, jobId));
            sqlParameters.Add(DBClient.AddParameters("SignatureTypeId", SqlDbType.Int, signatureTypeId));
            sqlParameters.Add(DBClient.AddParameters("InstallerDesignerElectricianId", SqlDbType.Int, installerDesignerElectricianId));
            object signature = CommonDAL.ExecuteScalar("InstallerDesignerElectricianSignature", sqlParameters.ToArray());
            return Convert.ToString(signature);
        }

        /// <summary>
        /// Gets All Installer Designer Records.
        /// </summary>
        /// <returns>
        /// dataset
        /// </returns>
        public DataSet GetAllInstallerDesigner()
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            DataSet ds = CommonDAL.ExecuteDataSet("InstallerDesigner_GetAll", null);
            return ds;
        }

        /// <summary>
        /// Gets All Installer Designer Records by ids.
        /// </summary>
        /// <returns>
        /// dataset
        /// </returns>
        public JobsInstallerDesignerView GetAllInstallerDesignerById(int id)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ID", SqlDbType.Int, id));
            JobsInstallerDesignerView jobsInstallerDesignerView = CommonDAL.ExecuteProcedure<JobsInstallerDesignerView>("InstallerDesigner_GetByID", sqlParameters.ToArray()).FirstOrDefault();
            return jobsInstallerDesignerView;
        }

        /// <summary>
        /// Gets All Solar Company Records.
        /// </summary>
        /// <returns>
        /// dataset
        /// </returns>
        public DataSet GetAllSolarCompanies()
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            DataSet ds = CommonDAL.ExecuteDataSet("SolarCompany_GetAll", null);
            return ds;
        }
    }
}
