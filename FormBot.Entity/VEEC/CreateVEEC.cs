using FormBot.Entity.VEEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class CreateVEEC
    {
        public VEECDetail VEECDetail { get; set; }

        public VEECInstallationDetail VEECInstallationDetail { get; set; }

        public VEECInstaller VEECInstaller { get; set; }

        public VEECOwnerDetail VEECOwnerDetail { get; set; }

        public VEECUpgradeManagerDetail VEECUpgradeManagerDetail { get; set; }

        public VEECArea VEECArea { get; set; }

        public BaselineEquipment BaselineEquipment { get; set; }

        public string Header { get; set; }

        public string Guid { get; set; }

        public List<VEECScheduling> lstVeecSchedule { get; set; }

        public VEECScheduling VeecScheduling { get; set; }

        //public JobElectricians JobElectricians { get; set; }

        public List<VEECCheckList.VEECCheckListItem> lstCheckListItem { get; set; }

        public List<CustomDetail> lstCustomDetails { get; set; }
		
		public List<BaselineEquipment> lstBaselineEquipment { get; set; }

        public List<BaselineEquipment> lstUpgradeEquipment { get; set; }

        public int VEECId { get; set; }
        public BasicDetailsVEEC BasicDetailsVeec { get; set; }

        public int UserType { get; set; }

        public chkPhotosVeec chkPhotosAll { get; set; }


        public List<VEECArea> lstArea { get; set; }

        

    }

  

    public class chkPhotosVeec
    {
        public int veecId { get; set; }
        public List<VeecSchedulingPhotos> chkVeecPhotos { get; set; }
        public List<Photo> ReferencePhotos { get; set; }



        public List<Photo> InstallationPhotos { get; set; }

        public List<Photo> SerialPhotos { get; set; }


    }
   

    public class VeecSchedulingPhotos
    {
        public string UniqueVisitID { get; set; }
        public int veecSchedulingId { get; set; }
        public int veecId { get; set; }
        public List<Photo> chkSerial { get; set; }

        public string ItemName { get; set; }
        public List<VEECVisitCheckListItems> lstVeecVisitCheckListItem { get; set; }
        public List<Photo> chkPhotos { get; set; }
        public List<Photo> chkCapturePhoto { get; set; }
        public List<Photo> chkSignature { get; set; }
        public List<Photo> chkCustom { get; set; }
        public bool IsDefaultSubmission { get; set; }
        public bool IsDeleted { get; set; }
        public int serialNumTotalCount { get; set; }
        public int capturePhotoTotalCount { get; set; }
        public int signatureTotalCount { get; set; }

    }

    public class VEECVisitCheckListItems
    {
        public string VeecVisitCheckListItemId { get; set; }

        public string FolderName { get; set; }

        public List<Photo> lstCheckListPhoto { get; set; }

        public int TotalCount { get; set; }
        public int VisitedCount { get; set; }
        public int CheckListClassTypeId { get; set; }

        public string PDFLocationId { get; set; }

        public string CaptureUploadImagePDFName { get; set; }
    }

    
}
