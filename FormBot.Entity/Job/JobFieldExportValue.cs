using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class JobFieldExportValue
    {
        /// <summary>
        /// Gets or sets the job field export value identifier.
        /// </summary>
        /// <value>
        /// The job field export value identifier.
        /// </value>
        public int JobFieldExportValueID { get; set; }

        /// <summary>
        /// Gets or sets the job field identifier.
        /// </summary>
        /// <value>
        /// The job field identifier.
        /// </value>
        public int JobFieldID { get; set; }

        /// <summary>
        /// Gets or sets the export value identifier.
        /// </summary>
        /// <value>
        /// The export value identifier.
        /// </value>
        public int ExportValueID { get; set; }

        /// <summary>
        /// Gets or sets the export value.
        /// </summary>
        /// <value>
        /// The export value.
        /// </value>
        public string ExportValue { get; set; }
    }
}
