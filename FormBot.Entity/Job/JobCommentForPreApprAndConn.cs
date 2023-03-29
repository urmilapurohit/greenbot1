using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    /// <summary>
    /// Job Stage Comment
    /// </summary>
    public class JobCommentForPreApprAndConn
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the job identifier.
        /// </summary>
        /// <value>
        /// The job identifier.
        /// </value>
        public int JobId { get; set; }

        /// <summary>
        /// Gets or sets the job status identifier.
        /// </summary>
        /// <value>
        /// The job status identifier.
        /// </value>
        public int JobStatusId { get; set; }

        /// <summary>
        /// Gets or sets the job comment for pre approval and connection.
        /// </summary>
        /// <value>
        /// The job comment for pre approval and connection.
        /// </value>
        public int PreApprovalAndConnectionId { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>
        /// The comment.
        /// </value>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        /// <value>
        /// The modified by.
        /// </value>
        public int ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status { get; set; }
    }
}
