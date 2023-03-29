using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class STCBasicDetails
    {
        /// <summary>
        /// Gets or sets the STC status identifier.
        /// </summary>
        /// <value>
        /// The STC status identifier.
        /// </value>
        public int STCStatusId { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the STC description.
        /// </summary>
        /// <value>
        /// The STC description.
        /// </value>
        public string STCDescription { get; set; }

        /// <summary>
        /// Gets or sets the STC heading.
        /// </summary>
        /// <value>
        /// The STC heading.
        /// </value>
        public string STCHeading { get; set; }

        /// <summary>
        /// Gets or sets the installation details.
        /// </summary>
        /// <value>
        /// The installation details.
        /// </value>
        public string InstallationDetails { get; set; }

        /// <summary>
        /// Gets or sets the job identifier.
        /// </summary>
        /// <value>
        /// The job identifier.
        /// </value>
        public int JobID { get; set; }

        /// <summary>
        /// Gets or sets the STC last updated date.
        /// </summary>
        /// <value>
        /// The STC last updated date.
        /// </value>
        public DateTime? STCLastUpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public string Color { get; set; }

        /// <summary>
        /// Gets or sets the STC settlement term.
        /// </summary>
        /// <value>
        /// The STC settlement term.
        /// </value>
        public int? STCSettlementTerm { get; set; }

        public string Header { get; set; }

        public DateTime? SettlementDate { get; set; }

        public string STCSettlementDate { get; set; }

        public DateTime? SubmittedDate { get; set; }
        public string STCSubmittedDate { get; set; }
        public string SubmittedBy { get; set; }

        public decimal STCPrice { get; set; }

        public decimal TotalAmount { get; set; }
        public bool? IsSpvInstallationVerified { get; set; }
        //public bool IsGenerateRecZip { get; set; }

        public string ColorCode { get; set; }

        public class STCSubmissionModel
        {
            public string VendorJobId { get; set; }
            public decimal STC { get; set; }
            public string STCStatus { get; set; }
            public string STCPVDCode { get; set; }
            //public DateTime? CERApprovedDate { get; set; }
            public string CERApprovedDate { get; set; }
            public string TradedBy { get; set; }
        }

    }
}
