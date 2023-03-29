using FormBot.DAL;
using FormBot.Entity.VEECManageList;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace FormBot.BAL.Service.VEECManageList
{
    public class VEECManageListBAL : IVEECManageListBAL
    {
        public IList<VEECProductBrands> VEECProductBrandsList(int PageNumber, int PageSize, string SortColumn, string SortDirection, string Brand, string Model, string ProductType, string ProductCategory, string TechnologyClass, string ApplicationDate, string EffectiveFrom, string EffectiveTo)
        {
            try
            {
                DateTime dtApplication, dtEffectiveFrom, dtEffectiveTo;
                DateTime? dtApplicationDate, dtEffectiveFromDate, dtEffectiveToDate;
                if (DateTime.TryParseExact(ApplicationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dtApplication))
                {
                    dtApplicationDate = dtApplication;
                }
                else
                {
                    dtApplicationDate = null;
                }

                if (DateTime.TryParseExact(EffectiveFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dtEffectiveFrom))
                {
                    dtEffectiveFromDate = dtEffectiveFrom;
                }
                else
                {
                    dtEffectiveFromDate = null;
                }

                if (DateTime.TryParseExact(EffectiveTo, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dtEffectiveTo))
                {
                    dtEffectiveToDate = dtEffectiveTo;
                }
                else
                {
                    dtEffectiveToDate = null;
                }

                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, PageNumber));
                sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, PageSize));
                sqlParameters.Add(DBClient.AddParameters("SortColumn", SqlDbType.NVarChar, SortColumn));
                sqlParameters.Add(DBClient.AddParameters("SortDirection", SqlDbType.VarChar, SortDirection));
                sqlParameters.Add(DBClient.AddParameters("Brand", SqlDbType.NVarChar, Brand));
                sqlParameters.Add(DBClient.AddParameters("Model", SqlDbType.NVarChar, Model));
                sqlParameters.Add(DBClient.AddParameters("ProductType", SqlDbType.NVarChar, ProductType));
                sqlParameters.Add(DBClient.AddParameters("ProductCategory", SqlDbType.NVarChar, ProductCategory));
                sqlParameters.Add(DBClient.AddParameters("TechnologyClass", SqlDbType.NVarChar, TechnologyClass));
                sqlParameters.Add(DBClient.AddParameters("ApplicationDate", SqlDbType.Date, dtApplicationDate));
                sqlParameters.Add(DBClient.AddParameters("EffectiveFrom", SqlDbType.Date, dtEffectiveFromDate));
                sqlParameters.Add(DBClient.AddParameters("EffectiveTo", SqlDbType.Date, dtEffectiveToDate));
                IList<VEECProductBrands> lstVEECProductBrands = CommonDAL.ExecuteProcedure<VEECProductBrands>("VEEC_GetProductBrandsList", sqlParameters.ToArray()).ToList();
                return lstVEECProductBrands;
            }
            catch (Exception ex)
            {                
                return null;
                Log.WriteError(ex);
            }
        }

        public void InsertProductBrands(DataTable dtProductBrands)
        {
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("DataTable", SqlDbType.Structured, dtProductBrands));
                sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
                sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));

                CommonDAL.ExecuteScalar("InsertProductBrands", sqlParameters.ToArray());
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
            }
        }
    }
}
