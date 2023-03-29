using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Documents
{
    public class DocumentsView
    {
        public string Id
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.DocumentId));
            }
        }

        public string JobIDEncrypted
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.JobId));
            }
        }

        public string DocID
        {
            get
            {
                return QueryString.QueryStringEncode("id=" + Convert.ToString(this.DocsID));
            }
        }

        public int DocsID { get; set; }
        public int JobId { get; set; }
        public int? DocumentId { get; set; }
        public string Stage { get; set; }
        public string ServiceProviderName { get; set; }
        public string Name { get; set; }
        public string PhysicalPath { get; set; }
        public bool IsOnline { get; set; }
        public bool IsGenerate { get; set; }
        public string StateName { get; set; }
        public int TotalRecords { get; set; }
        public bool IsUpload { get; set; }
        public bool IsExist { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public string JsonData { get; set; }
        public string DownloadURLPath { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
        public int JobDocumentId { get; set; }
        public string FileName { get; set; }
        public string VendorJobDocumentId { get; set; }
		public bool IsCompleted { get; set; }
		public int SentEmailStatus { get; set; }
	}
}
