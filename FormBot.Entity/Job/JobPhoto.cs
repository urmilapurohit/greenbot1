using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBot.Entity
{
    public class JobPhoto
    {
        public int JobID { get; set; }
        public string Guid { get; set; }
        [NotMapped]
        public string FileName { get; set; }

        [NotMapped]
        public List<string> FileNamesCreate { get; set; }

        [NotMapped]
        public List<UserDocument> lstUserDocument { get; set; }
        public List<UserDocument> lstSerialDocument { get; set; }
    }

    //public class JobPhotosList
    //{

    //    public string VisitID { get; set; }

    //    public List<Checklistitems> panel { get; set; }
    //}

    //public class paths
    //{
    //    public List<string> paths { get; set; }
    //    public string VisitId { get; set; }
    //}

    public class Pl
    {
        public string p { get; set; }
        public string s { get; set; }
        public string fn { get; set; }
    }

    public class Vp
    {
        public List<Pl> pl { get; set; }
        public object id { get; set; }
    }

    public class MainObject
    {
        public string vclid { get; set; }
        public string vsid { get; set; }
        public List<Vp> vp { get; set; }
        public string rp { get; set; }
        public bool isDownloadAll { get; set; }
    }

    public class ImageDetails
    {
        public int Angle { get; set; }

        public string Src { get; set; }

        public string Path { get; set; }
    }
}
