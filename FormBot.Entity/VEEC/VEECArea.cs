using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.VEEC
{
    public class VEECArea
    {
        public int VEECId { get; set; }

        public int VEECAreaId { get; set; }

        [Display(Name = "Area Name:")]
        [Required(ErrorMessage = "Area Name Is Required")]
        public string Name { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }

        


        public List<VEECFolder> veecFolder { get; set; }

        
    }

    public class VeecPhotos
    {
        public int veecPhotosId { get; set; }
        public string Name { get; set; }
        public string Foldername { get; set; }

        public string FullPath { get; set; }

        public int VeecAreaId { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }


    }

    public class VEECFolder
    {
        public int Id { get; set; }
        public string FolderName { get; set; }

        public List<VEECSubFolder> subFolder {get;set;}

        public List<VeecPhotos> lstPhotos { get; set; }
    }

    public class VEECSubFolder
    {
        public int Id { get; set; }
        public string FolderName { get; set; }

        public List<VeecPhotos> lstPhotos { get; set; }
    }

    public class Vpf
    {

    }
}
