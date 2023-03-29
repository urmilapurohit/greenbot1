using FormBot.DAL;
using FormBot.Entity;
using FormBot.Entity.SpvVerification;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL
{
    public class SpvVerificationBAL: ISpvVerificationBAL
    {
        public List<SpvPanelManufacturer> GetData()
        {
            string spName = "[SpvPanelManufacturer_BindDropDown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<SpvPanelManufacturer> spvPanelManufacturerList = CommonDAL.ExecuteProcedure<SpvPanelManufacturer>(spName, sqlParameters.ToArray());
            return spvPanelManufacturerList.ToList();
        }
        public virtual void BulkInsertSpvPanelManufacturer(DataTable dtSpvPanelManufacturer,int SpvUserId)
        {
            string spName = "[BulkInsertSpvPanelManufacturer]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("tblSpvPanelManufacturerType", SqlDbType.Structured, dtSpvPanelManufacturer));
            sqlParameters.Add(DBClient.AddParameters("SpvUserId", SqlDbType.Int, SpvUserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
        }
        public List<SpvPanelDetailsModel> GetSpvPanelDetailsSearchByManufacturer(int SpvUserId, string SerialNumber, string ModelNumber, bool isDownload = false)
        {
            string spName = "[GetSpvPanelDetailsSearchByManufacturer]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SpvUserId", SqlDbType.Int, SpvUserId));
            sqlParameters.Add(DBClient.AddParameters("SerialNumber", SqlDbType.VarChar, SerialNumber));
            sqlParameters.Add(DBClient.AddParameters("ModelNumber", SqlDbType.VarChar, ModelNumber));
            sqlParameters.Add(DBClient.AddParameters("IsDownload", SqlDbType.Bit, isDownload));
            IList<SpvPanelDetailsModel> spvPanelDetailsList = CommonDAL.ExecuteProcedure<SpvPanelDetailsModel>(spName, sqlParameters.ToArray());
            return spvPanelDetailsList.ToList();
        }
        public List<SpvPanelDetails> GetSpvPanelDetails(string SerialNumber, string ModelNumber, string Supplier, string Manufacturer)
        {
            string spName = "[GetSpvPanelDetails]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SerialNumber", SqlDbType.VarChar, SerialNumber));
            sqlParameters.Add(DBClient.AddParameters("ModelNumber", SqlDbType.VarChar, ModelNumber));
            sqlParameters.Add(DBClient.AddParameters("Supplier", SqlDbType.VarChar, Supplier));
            sqlParameters.Add(DBClient.AddParameters("Manufacturer", SqlDbType.VarChar, Manufacturer));
            IList<SpvPanelDetails> spvPanelDetails= CommonDAL.ExecuteProcedure<SpvPanelDetails>(spName, sqlParameters.ToArray());
            return spvPanelDetails.ToList();
        }
        public virtual bool CheckManufacturerIsExsistOrNot(string ManufacturerName)
        {
            string spName = "[spv_CheckManufacturerIsExsistOrNot]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ManufacturerName", SqlDbType.NVarChar, ManufacturerName));
            DataSet ds = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return Convert.ToBoolean(ds.Tables[0].Rows[0][0].ToString());
        }
        public void InsertSpvVerifiedSerialNos(string serialnumbers,int createdBy)
        {
            string spName = "[InsertSpvVerifiedSerialNos]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SerialNumbers", SqlDbType.VarChar, serialnumbers));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));

            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, createdBy));
        
            CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
        }

        public void ReleaseSerailNumbers(string spvPanelDetailsIds)
        {
            string spName = "[Spv_ReleasePanelSerialNumber]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SpvPanelDetailsId", SqlDbType.VarChar, spvPanelDetailsIds));

            CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
        }
        public SpvPanelDetailsUploadHistory SpvPanelDetailsUploadHistoryInsert(string FilePath,string FileName,int ManufacturerId)
        {
            string spName = "[SpvPanelDetailsUploadHistory_Insert]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("FilePath", SqlDbType.VarChar, FilePath));
            sqlParameters.Add(DBClient.AddParameters("FileName", SqlDbType.NVarChar, FileName));
            sqlParameters.Add(DBClient.AddParameters("ManufacturerId", SqlDbType.Int, ManufacturerId));
            sqlParameters.Add(DBClient.AddParameters("UploadedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("UploadedOn", SqlDbType.DateTime, DateTime.Now));
            IList<SpvPanelDetailsUploadHistory> spvPanelDetailsUploadHistory = CommonDAL.ExecuteProcedure<SpvPanelDetailsUploadHistory>(spName, sqlParameters.ToArray());
            return spvPanelDetailsUploadHistory.FirstOrDefault();
        }
        public List<SpvPanelDetailsUploadHistory> GetSpvPanelDetailsUploadedHistory(int ManufacturerId)
        {
            string spName = "[GetSpvPanelDetailsUploadedHistory]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ManufactureID", SqlDbType.Int, ManufacturerId));
            IList<SpvPanelDetailsUploadHistory> spvPanelDetailsUploadHistory = CommonDAL.ExecuteProcedure<SpvPanelDetailsUploadHistory>(spName, sqlParameters.ToArray());
            return spvPanelDetailsUploadHistory.ToList();
        }
        /// <summary>
        /// Get supplier list by manufacturer name
        /// </summary>
        /// <param name="ManufacturerName"></param>
        /// <returns>list of supplier</returns>
        public List<string> GetSupplierByManufacturer(string ManufacturerName)
        {
            List<string> lstSupplier = new List<string>();
            string spName = "[GetSupplierByManufacturer]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ManufacturerName", SqlDbType.VarChar, ManufacturerName));
            lstSupplier = CommonDAL.ExecuteProcedure<JobPanelDetails>(spName, sqlParameters.ToArray()).Select(x=>x.Supplier).ToList();
            return lstSupplier;
        }
    }
}
