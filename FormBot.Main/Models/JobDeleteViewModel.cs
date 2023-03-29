using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FormBot.Main.Models
{
    public class JobDeleteViewModel
    {
        public string Id { get; set; }
        public string JobTitle { get; set; }
    }

    //public class Doc
    //{
    //    public string name { get; set; }
    //    public string stage { get; set; }
    //    public string abbr { get; set; }
    //    public string sp { get; set; }
    //    public string path { get; set; }
    //    public string TemplateName { get; set; }
    //}

    //public class DocObject
    //{
    //    public string jobId { get; set; }
    //    public List<Doc> docs { get; set; }
    //    public int UserId { get; set; }
    //    public bool fillData { get; set; }
    //    public bool UseNewDocTemplate { get; set; }
    //    public string type { get; set; }
    //}

    public class FileStatus
    {
        public bool isAlreadyExists { get; set; }
        public string name { get; set; }

        public bool isSourceExists { get; set; }

        public bool isFileCopied { get; set; }
        public string message { get; set; }

        public int JobDocumentId { get; set; }

        public string Path { get; set; }
    }

    public class DeleteDoc
    {
        public string id { get; set; }
        public string path { get; set; }
        public bool deleteFile { get; set; }
        public string JobId { get; set; }
    }
}