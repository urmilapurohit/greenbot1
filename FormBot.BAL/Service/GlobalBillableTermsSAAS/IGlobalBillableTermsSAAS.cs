using FormBot.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service
{
    public interface IGlobalBillableTermsSAAS
    {
        /// <summary>
        /// Sets the Global Pricing terms for saas.
        /// </summary>
        /// <param name="GlobalBillableTerms">Pricing Options for Settlement Terms</param>
        List<GlobalBillableTerms> SaveGlobalBillableTermSAAS(GlobalBillableTerms GlobalBillableTerms);

        /// <summary>
        /// Get saas pricinig list.
        /// </summary>
        List<GlobalBillableTerms> GetGlobalPricingList(string TermName, string BillerCode, string TermDescription, string TermCode, int PageNumber, int PageSize, string sortColumn, string sortDirectoin);

        /// <summary>
        /// Deletes billable term by id.
        /// </summary>
        /// <param name="Id">Billing term id</param>
        List<GlobalBillableTerms> DeleteBillableTermByID(int Id);

        /// <summary>
        /// Restores deleted billable term.
        /// </summary>
        /// <param name="Id">Billing term id</param>
        List<GlobalBillableTerms> RetoreBillingTermByID(int Id);
    }
}
