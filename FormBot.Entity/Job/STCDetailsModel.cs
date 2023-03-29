using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class STCDetailsModel
    {
        /// <summary>
        /// Gets or sets the STC basic details.
        /// </summary>
        /// <value>
        /// The STC basic details.
        /// </value>
        public STCBasicDetails stcBasicDetails { get; set; }
        //public string stcBasicDetailSTR { get; set; }

        /// <summary>
        /// Gets or sets the STC job history.
        /// </summary>
        /// <value>
        /// The STC job history.
        /// </value>
        public STCJobHistory stcJobHistory { get; set; }
        //public string stcJobHistorySTR { get; set; }

        /// <summary>
        /// Gets or sets the pricing manager.
        /// </summary>
        /// <value>
        /// The pricing manager.
        /// </value>
        public PricingManager pricingManager { get; set; }
        //public string pricingManagerSTR { get; set; }

        public int? ErrorLength { get; set; }

        public List<CheckList.CheckListItem> lstCheckListItem { get; set; }
    }
    public class STCSettlementTermsDetails
    {
        public int STCJobDetailsId { get; set; }
        public int CustomSettlementTerm { get; set; }
        public int STCSettlementTerm { get; set; }
    }
}
