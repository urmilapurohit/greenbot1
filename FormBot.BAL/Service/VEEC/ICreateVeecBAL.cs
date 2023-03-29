using FormBot.Entity;
using FormBot.Entity.VEEC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service
{
    public interface ICreateVeecBAL
    {
        int InsertVeec(CreateVEEC createVEEC);

        CreateVEEC GetVeecByID(int veecID);

        List<VEECList> GetVEECList(int PageNumber, int PageSize, string SortCol, string SortDir, string solarcompanyid, DateTime? FromDate, DateTime? ToDate, DateTime? FromDateCommencement, DateTime? ToDateCommencement, DateTime? FromDateActivity, DateTime? ToDateActivity, string Searchtext);

        List<VEECScheduling> GetVEECschedulingByVEECID(int jobID);

        List<VEECNotes> GetVeecNotesListOnVisit(int jobID);

        void VeecNotesMarkAsSeen(string jobNoteIds);

        //List<VEECStage> GetVeecStagesWithCount(int UserId, int UserTypeId, int SolarCompanyId);

        //List<JobList> GetVeecStages();

        //DataSet GetPreApprovalStatus();

        List<CustomDetail> GetVeecCustomDetails(int JobId, int SolarCompanyId);

        List<string> GetProductmodel(string ProductBrand);


        int InsertVeecArea(VEECArea VEECArea);

        List<VEECArea> GetVEECAreaNameRecords(long VEECIds,string SortCol , string SortDir);

        int RearrangeCalcZone(int VEECId);

        long DeleteVEECAreaName(long VEECAreaId);

        List<VEECArea> CheckIfNameExists(string name);
		
		List<string> RemoveRequiredFields(BaselineEquipment baselineequipment);

        int InsertBaseLineEquipment(BaselineEquipment BaselineEquipment);

        long DeleteBaselineUpgradeEquipment(long BaselineEquipmentId);

        DataSet Lstbaseline(int id, int areaId);

        DataSet Lstupgradeline(int id, int areaId);

        DataSet GetData(List<CommonData> cData);

        int GetSCOIdByVeecId(int veecID);

        List<AssignSCO> GetSCOUser();

        DataSet GetChecklistPhotosVeec(int VeecId);

        DataSet GetVeecsPhotos(int veecId);

        DataSet GetPhotosPath(string VeecVisitCheckListPhotoIds, string VeecVisitSignatureIds);

        DataSet GetVeecPhotosPath(string VeecPhotoId);

        DataSet DeleteCheckListPhotos(string chkIds, string sigIds, string VeecSchedulingIds);

        DataSet DeleteVeecPhotos(string photoId, string areaId, string veecId);

        int InsertReferencePhoto(int veecID, string filename, int UserId, string veecscId, string cId, bool isDefault, string ClassType, string VendorVeecPhotoId, string Status = "", string Latitude = "", string Longitude = "", string ImageTakenDate = "");

        int InsertVeecPhoto(int veecID, string filename, int UserId, int veecAreaId, int folderID,string Status = "", string Latitude = "", string Longitude = "", string ImageTakenDate = "");

        DataSet GetCesPhotosByVEECVisitCheckListId(int VeecVisitCheckListItemId);

        int InsertCESDocuments(int VeecId, string Path, int UserID, string Type, string JsonData);

        DataSet GetVeecFolderStructure();

        int showHideBCAClassificationOnSpaceTypeChange(int spaceTypeId);

        DataSet GetVeecData(int VeecId);

        void DeleteVEECActivitySchedulePremises();

        int AddVEECInstaller(int SolarCompanyId, int UserId, string Firstname = "", string Lastname = "", string CompanyName="", string ElectricalComplianceNumber = "", string ElectricalContractorsLicenseNumber = "",int VeecInstallerId = 0);

        DataSet GetVEECIntaller(int SolarCompanyId);


        void UpdateVEECInstallerDetail(int veecInstallerId, int veecId);

        DataSet GetUpgradeManager(int SolarCompanyId);

        int AddVEECUpgradeManager(VEECUpgradeManagerDetail veecUpgradeManagerDetail);

        void UpdateVEECUpgradeManager(int veecUpgradeManagerId, int VeecId, bool IsSysUser);

        void DeleteVEECCustomUpgradeManager(int VEECUpgradeManagerId);

        void DeleteVEECCustomInstaller(int VeecInstallerId); 

        DataSet UploadVeec_GetVeecByID(int veecId);

        void VEECUpdateDetails_InsertUpdate(int veecPortalId, int veecUploadId, int noOfVEECs, int veecId, bool isDeleted);

    }
}
