using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    /// <summary>
    /// JobEmail Conversation For PreAndConn
    /// </summary>
    public class JobEmailConversationForPreAndConn
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
        /// Gets or sets the is pre apr connection.
        /// </summary>
        /// <value>
        /// The is pre apr connection.
        /// </value>
        public int IsPreAprConn { get; set; }
        /// <summary>
        /// Gets or sets the step identifier.
        /// </summary>
        /// <value>
        /// The step identifier.
        /// </value>
        public int StetpId { get; set; }

        /// <summary>
        /// Gets or sets the type of the document.
        /// </summary>
        /// <value>
        /// The type of the document.
        /// </value>
        public int DocumentType { get; set; }
        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>
        /// The comment.
        /// </value>
        public string Comment { get; set; }
        /// <summary>
        /// Gets or sets the subject code.
        /// </summary>
        /// <value>
        /// The subject code.
        /// </value>
        public string SubjectCode { get; set; }
        /// <summary>
        /// Gets or sets the distributor.
        /// </summary>
        /// <value>
        /// The distributor.
        /// </value>
        public string Distributor { get; set; }
    }
}
