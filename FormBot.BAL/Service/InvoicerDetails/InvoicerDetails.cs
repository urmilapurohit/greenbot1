using FormBot.DAL;
using FormBot.Entity;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service.InvoicerDetails
{
    public class InvoicerDetails : IInvoicerDetails
    {
        public void SaveAccountCodes(string Code, string Name, string TaxType, int isSync)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Code", SqlDbType.Int, Code));
            sqlParameters.Add(DBClient.AddParameters("Name", SqlDbType.VarChar, Name));
            sqlParameters.Add(DBClient.AddParameters("TaxType", SqlDbType.VarChar, TaxType));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("isSync", SqlDbType.Int, isSync));
            CommonDAL.ExecuteDataSet("InsertUpdate_XeroAccountCodes", sqlParameters.ToArray());
        }

        public void SaveInvoicerDetails(Invoicer invoicer)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("InvoicerId", SqlDbType.Int, invoicer.InvoicerId));
            sqlParameters.Add(DBClient.AddParameters("InvoicerName", SqlDbType.VarChar, invoicer.InvoicerName));
            sqlParameters.Add(DBClient.AddParameters("InvoicerFirstName", SqlDbType.VarChar, invoicer.InvoicerFirstName));
            sqlParameters.Add(DBClient.AddParameters("InvoicerLastName", SqlDbType.VarChar, invoicer.InvoicerLastName));
            sqlParameters.Add(DBClient.AddParameters("InvoicerPhone", SqlDbType.VarChar, invoicer.InvoicerPhone));
            sqlParameters.Add(DBClient.AddParameters("UniqueCompanyNumber", SqlDbType.VarChar, invoicer.UniqueCompanyNumber));
            sqlParameters.Add(DBClient.AddParameters("AccountCode", SqlDbType.VarChar, invoicer.AccountCode));
            sqlParameters.Add(DBClient.AddParameters("PostalAddressType", SqlDbType.Int, invoicer.InvoicerAddressID));
            sqlParameters.Add(DBClient.AddParameters("UnitTypeID", SqlDbType.Int, invoicer.InvoicerUnitTypeID));
            sqlParameters.Add(DBClient.AddParameters("UnitNumber", SqlDbType.VarChar, invoicer.InvoicerUnitNumber));
            sqlParameters.Add(DBClient.AddParameters("StreetNumber", SqlDbType.VarChar, invoicer.InvoicerStreetNumber));
            sqlParameters.Add(DBClient.AddParameters("StreetName", SqlDbType.VarChar, invoicer.InvoicerStreetName));
            sqlParameters.Add(DBClient.AddParameters("StreetTypeID", SqlDbType.Int, invoicer.InvoicerStreetTypeID));
            sqlParameters.Add(DBClient.AddParameters("PostalAddressID", SqlDbType.Int, invoicer.InvoicerPostalAddressID));
            sqlParameters.Add(DBClient.AddParameters("PostalDeliveryNumber", SqlDbType.VarChar, invoicer.InvoicerPostalDeliveryNumber));
            sqlParameters.Add(DBClient.AddParameters("Town", SqlDbType.VarChar, invoicer.InvoicerTown));
            sqlParameters.Add(DBClient.AddParameters("State", SqlDbType.VarChar, invoicer.InvoicerState));
            sqlParameters.Add(DBClient.AddParameters("PostCode", SqlDbType.VarChar, invoicer.InvoicerPostCode));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("isActive", SqlDbType.Bit, invoicer.isActive));
            sqlParameters.Add(DBClient.AddParameters("DATAOPMODE", SqlDbType.Int, invoicer.DATAOPMODE));
            CommonDAL.ExecuteDataSet("InsertUpdate_InvoicerDetails", sqlParameters.ToArray());
        }


        /// <summary>
        /// Get invoicer details list.
        /// </summary>
        public List<Invoicer> GetInvoicerList(int PageNumber, int PageSize, string InvoicerName, string AccountCode)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            //sqlParameters.Add(DBClient.AddParameters("InvoicerId", SqlDbType.Int, InvoicerId != "" ? InvoicerId : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("InvoicerName", SqlDbType.VarChar, InvoicerName != "" ? InvoicerName : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("AccountCode", SqlDbType.VarChar, AccountCode != "" ? AccountCode : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, PageSize));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, PageNumber));
            List<Invoicer> lstInvoicerDetails = CommonDAL.ExecuteProcedure<Invoicer>("GetInvoicerDetailsList", sqlParameters.ToArray()).ToList();
            return lstInvoicerDetails;
        }
    }
}
