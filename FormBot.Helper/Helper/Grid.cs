using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
//using System.Linq.Dynamic;
using System.Web.Script.Serialization;
namespace FormBot.Helper
{
    public static class Grid
    {
        /// <summary>
        /// This method will get all necessary parameters from HttpRequest and assign it to GridParam object.
        /// </summary>
        /// <param name="httpRequestBase">http Request Base</param>
        /// <returns>Grid Param</returns>
        public static GridParam ParseParams(HttpRequestBase httpRequestBase)
        {
            GridParam gridParam = new GridParam();
			if (httpRequestBase["iDisplayLength"] != null)
			{
				gridParam.PageSize = int.Parse(httpRequestBase["iDisplayLength"]);
				gridParam.PageStart = int.Parse(httpRequestBase["iDisplayStart"]);
				if (httpRequestBase["iSortCol_0"] != null)
				{
					gridParam.SortCol_0 = int.Parse(httpRequestBase["iSortCol_0"]);
					gridParam.SortCol = Convert.ToString(httpRequestBase["mDataProp_" + gridParam.SortCol_0]);
				}
				gridParam.SortDir = Convert.ToString(httpRequestBase["sSortDir_0"]);
				gridParam.Search = Convert.ToString(httpRequestBase["sSearch"]);
				if (gridParam.PageStart < 0)
				{
					gridParam.PageStart = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["DefaultPageSize"].ToString());
				}
			}
			return gridParam;
        }

        ///// <summary>
        ///// Prepare query with passed GriParam and return query to T type query.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="qry"></param>
        ///// <param name="gridParam"></param>
        ///// <returns></returns>
        //public static IQueryable<T> PrepareDataSetQuery<T>(this IQueryable<T> qry, GridParam gridParam)
        //{
        //    gridParam.TotalRecords = qry.Count();
        //    gridParam.TotalDisplayRecords = qry.Count();
        //    if (!string.IsNullOrEmpty(gridParam.SortCol))
        //        return qry.OrderBy(string.Format("{0} {1}", gridParam.SortCol, gridParam.SortDir)).Skip(gridParam.PageStart).Take(gridParam.PageSize);
        //    else
        //        return qry.Skip(gridParam.PageStart).Take(gridParam.PageSize);
        //}

        /// <summary>
        /// Prepares the data set.
        /// </summary>
        /// <typeparam name="T">type param</typeparam>
        /// <param name="qry">The qry.</param>
        /// <param name="gridParam">The grid parameter.</param>
        /// <returns></returns>
        public static string PrepareDataSet<T>(this IList<T> qry, GridParam gridParam)
        {
            var result = new
            {
                iTotalRecords = gridParam.TotalRecords,
                iTotalDisplayRecords = gridParam.TotalDisplayRecords,
                iTotalAmount = gridParam.TotalAmount,
                aaData = qry.ToList()
            };
                        
            JavaScriptSerializer js = new JavaScriptSerializer();
            js.MaxJsonLength = Int32.MaxValue;
            return js.Serialize(result);
        }
    }

    public class GridParam
    {
        public int PageSize { get; set; }
        public int PageStart { get; set; }
        public int SortCol_0 { get; set; }
        public string SortDir { get; set; }
        public string Search { get; set; }
        public string SortCol { get; set; }
        public int TotalRecords { get; set; }
        public int TotalDisplayRecords { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
