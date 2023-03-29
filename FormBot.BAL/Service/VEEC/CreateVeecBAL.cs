using FormBot.DAL;
using FormBot.Entity;
using FormBot.Entity.VEEC;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace FormBot.BAL.Service
{
    public class CreateVeecBAL : ICreateVeecBAL
    {
        /// <summary>
        /// Insert VEEC
        /// </summary>
        /// <param name="createVEEC">Object of CreateVEEC</param>
        /// <returns></returns>
        public int InsertVeec(CreateVEEC createVEEC)
        {
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();

                /**************************************************VEEC Detail***************************************************************/
                sqlParameters.Add(DBClient.AddParameters("VEECId", SqlDbType.Int, createVEEC.VEECDetail.VEECId));
                sqlParameters.Add(DBClient.AddParameters("ScheduleActivityType", SqlDbType.NVarChar, createVEEC.VEECDetail.ScheduleActivityType));
                sqlParameters.Add(DBClient.AddParameters("RefNumber", SqlDbType.NVarChar, createVEEC.VEECDetail.VEECId > 0 ? string.Concat(createVEEC.VEECDetail.RefNumber_Prefix, createVEEC.VEECDetail.RefNumber_Suffix) : createVEEC.VEECDetail.RefNumber));
                sqlParameters.Add(DBClient.AddParameters("Title", SqlDbType.NVarChar, createVEEC.VEECDetail.Title));
                sqlParameters.Add(DBClient.AddParameters("Description", SqlDbType.NVarChar, createVEEC.VEECDetail.Description));
                sqlParameters.Add(DBClient.AddParameters("CommencementDate", SqlDbType.DateTime, createVEEC.VEECDetail.CommencementDate));
                sqlParameters.Add(DBClient.AddParameters("ActivityDate", SqlDbType.DateTime, createVEEC.VEECDetail.ActivityDate));
                sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.Int, createVEEC.VEECDetail.SolarCompanyId));//ProjectSession.SolarCompanyId));
                sqlParameters.Add(DBClient.AddParameters("VEECInstallerId", SqlDbType.Int, createVEEC.VEECDetail.VEECInstallerId == 0 ? null : createVEEC.VEECDetail.VEECInstallerId));
                sqlParameters.Add(DBClient.AddParameters("VEECInstallerSignature", SqlDbType.NVarChar, createVEEC.VEECDetail.VEECInstallerSignature));
                sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
                sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
                sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
                sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
                sqlParameters.Add(DBClient.AddParameters("ScoIdVEEC", SqlDbType.Int, createVEEC.VEECDetail.ScoIDVEEC));

                /**************************************************VEEC Owner Detail***************************************************************/
                sqlParameters.Add(DBClient.AddParameters("OwnerType", SqlDbType.NVarChar, createVEEC.VEECOwnerDetail.OwnerType));
                sqlParameters.Add(DBClient.AddParameters("OwnerCompanyName", SqlDbType.NVarChar, createVEEC.VEECOwnerDetail.CompanyName));
                sqlParameters.Add(DBClient.AddParameters("OwnerFirstName", SqlDbType.NVarChar, createVEEC.VEECOwnerDetail.FirstName));
                sqlParameters.Add(DBClient.AddParameters("OwnerLastName", SqlDbType.NVarChar, createVEEC.VEECOwnerDetail.LastName));
                sqlParameters.Add(DBClient.AddParameters("OwnerUnitTypeID", SqlDbType.Int, createVEEC.VEECOwnerDetail.UnitTypeID));
                sqlParameters.Add(DBClient.AddParameters("OwnerUnitNumber", SqlDbType.NVarChar, createVEEC.VEECOwnerDetail.UnitNumber));
                sqlParameters.Add(DBClient.AddParameters("OwnerStreetNumber", SqlDbType.NVarChar, createVEEC.VEECOwnerDetail.StreetNumber));
                sqlParameters.Add(DBClient.AddParameters("OwnerStreetName", SqlDbType.NVarChar, createVEEC.VEECOwnerDetail.StreetName));
                sqlParameters.Add(DBClient.AddParameters("OwnerStreetTypeID", SqlDbType.Int, createVEEC.VEECOwnerDetail.StreetTypeID));
                sqlParameters.Add(DBClient.AddParameters("OwnerTown", SqlDbType.NVarChar, createVEEC.VEECOwnerDetail.Town));
                sqlParameters.Add(DBClient.AddParameters("OwnerState", SqlDbType.NVarChar, createVEEC.VEECOwnerDetail.State));
                sqlParameters.Add(DBClient.AddParameters("OwnerPostCode", SqlDbType.NVarChar, createVEEC.VEECOwnerDetail.PostCode));
                sqlParameters.Add(DBClient.AddParameters("OwnerPhone", SqlDbType.NVarChar, createVEEC.VEECOwnerDetail.Phone));
                sqlParameters.Add(DBClient.AddParameters("OwnerMobile", SqlDbType.NVarChar, createVEEC.VEECOwnerDetail.Mobile));
                sqlParameters.Add(DBClient.AddParameters("OwnerEmail", SqlDbType.NVarChar, createVEEC.VEECOwnerDetail.Email));
                sqlParameters.Add(DBClient.AddParameters("OwnerIsPostalAddress", SqlDbType.Bit, createVEEC.VEECOwnerDetail.IsPostalAddress));
                sqlParameters.Add(DBClient.AddParameters("OwnerPostalAddressID", SqlDbType.NVarChar, createVEEC.VEECOwnerDetail.PostalAddressID));
                sqlParameters.Add(DBClient.AddParameters("OwnerPostalDeliveryNumber", SqlDbType.NVarChar, createVEEC.VEECOwnerDetail.PostalDeliveryNumber));
                sqlParameters.Add(DBClient.AddParameters("OwnerSignature", SqlDbType.NVarChar, createVEEC.VEECOwnerDetail.OwnerSignature));
                sqlParameters.Add(DBClient.AddParameters("OwnerSignatureDate", SqlDbType.DateTime, createVEEC.VEECOwnerDetail.SignatureDate));
                sqlParameters.Add(DBClient.AddParameters("OwnerLatitude", SqlDbType.NVarChar, createVEEC.VEECOwnerDetail.Latitude));
                sqlParameters.Add(DBClient.AddParameters("OwnerLongitude", SqlDbType.NVarChar, createVEEC.VEECOwnerDetail.Longitude));
                sqlParameters.Add(DBClient.AddParameters("OwnerIpAddress", SqlDbType.NVarChar, createVEEC.VEECOwnerDetail.IpAddress));
                sqlParameters.Add(DBClient.AddParameters("OwnerLocation", SqlDbType.NVarChar, createVEEC.VEECOwnerDetail.Location));

                /**************************************************VEEC Installation Detail***************************************************************/
                sqlParameters.Add(DBClient.AddParameters("InsUnitTypeID", SqlDbType.Int, createVEEC.VEECInstallationDetail.UnitTypeID));
                sqlParameters.Add(DBClient.AddParameters("InsUnitNumber", SqlDbType.NVarChar, createVEEC.VEECInstallationDetail.UnitNumber));
                sqlParameters.Add(DBClient.AddParameters("InsStreetNumber", SqlDbType.NVarChar, createVEEC.VEECInstallationDetail.StreetNumber));
                sqlParameters.Add(DBClient.AddParameters("InsStreetName", SqlDbType.NVarChar, createVEEC.VEECInstallationDetail.StreetName));
                sqlParameters.Add(DBClient.AddParameters("InsStreetTypeID", SqlDbType.Int, createVEEC.VEECInstallationDetail.StreetTypeID));
                sqlParameters.Add(DBClient.AddParameters("InsTown", SqlDbType.NVarChar, createVEEC.VEECInstallationDetail.Town));
                sqlParameters.Add(DBClient.AddParameters("InsState", SqlDbType.NVarChar, createVEEC.VEECInstallationDetail.State));
                sqlParameters.Add(DBClient.AddParameters("InsPostCode", SqlDbType.NVarChar, createVEEC.VEECInstallationDetail.PostCode));
                sqlParameters.Add(DBClient.AddParameters("InsIsPostalAddress", SqlDbType.Bit, createVEEC.VEECInstallationDetail.IsPostalAddress));
                sqlParameters.Add(DBClient.AddParameters("InsPostalAddressID", SqlDbType.NVarChar, createVEEC.VEECInstallationDetail.PostalAddressID));
                sqlParameters.Add(DBClient.AddParameters("InsPostalDeliveryNumber ", SqlDbType.NVarChar, createVEEC.VEECInstallationDetail.PostalDeliveryNumber));
                sqlParameters.Add(DBClient.AddParameters("IndustryBusinessType", SqlDbType.Int, createVEEC.VEECInstallationDetail.IndustryBusinessType));
                sqlParameters.Add(DBClient.AddParameters("NumberOfLevels", SqlDbType.Decimal, createVEEC.VEECInstallationDetail.NumberOfLevels));
                sqlParameters.Add(DBClient.AddParameters("FloorSpace", SqlDbType.Decimal, createVEEC.VEECInstallationDetail.FloorSpace));
                sqlParameters.Add(DBClient.AddParameters("FloorSpaceUpgradedArea", SqlDbType.Decimal, createVEEC.VEECInstallationDetail.FloorSpaceUpgradedArea));
                //sqlParameters.Add(DBClient.AddParameters("CertificateElectricalComplianceNumber", SqlDbType.NVarChar, createVEEC.VEECInstallationDetail.CertificateElectricalComplianceNumber));

                /**************************************************VEEC 1680 Lighting Design Detail***************************************************************/
                sqlParameters.Add(DBClient.AddParameters("LightingDesignMethod1680", SqlDbType.Int, createVEEC.VEECDetail.LightingDesignMethodId));
                sqlParameters.Add(DBClient.AddParameters("QualificationsOfLightingDesigner1680", SqlDbType.Int, createVEEC.VEECDetail.QualificationsOfLightingDesignerId));
                sqlParameters.Add(DBClient.AddParameters("DesignerQualificationDetails", SqlDbType.NVarChar, createVEEC.VEECDetail.DesignerQualificationDetails));
                sqlParameters.Add(DBClient.AddParameters("LightLevelVerification1680", SqlDbType.Int, createVEEC.VEECDetail.LightLevelVerificationId));
                sqlParameters.Add(DBClient.AddParameters("QualificationOfLightLevelVerifier1680", SqlDbType.Int, createVEEC.VEECDetail.QualificationOfLightLevelVerifierId));
                sqlParameters.Add(DBClient.AddParameters("VerifierQualificationDetail1680", SqlDbType.NVarChar, createVEEC.VEECDetail.VerifierQualificationDetails));
                sqlParameters.Add(DBClient.AddParameters("ContractualArrangement1680", SqlDbType.Int, createVEEC.VEECDetail.ContractualArrangementId));
                sqlParameters.Add(DBClient.AddParameters("ContractualArrangement1680Name", SqlDbType.NVarChar, createVEEC.VEECDetail.ContractualDetails));

                /**************************************************VEEC Installer Detail***************************************************************/
                //sqlParameters.Add(DBClient.AddParameters("InstallerSolarCompanyId", SqlDbType.Int, createVEEC.VEECDetail.SolarCompanyId));
                //sqlParameters.Add(DBClient.AddParameters("InstallerFirstName", SqlDbType.NVarChar, createVEEC.VEECInstaller.FirstName));
                //sqlParameters.Add(DBClient.AddParameters("InstallerLastName", SqlDbType.NVarChar, createVEEC.VEECInstaller.LastName));
                //sqlParameters.Add(DBClient.AddParameters("InstallerEmail", SqlDbType.NVarChar, createVEEC.VEECInstaller.Email));
                //sqlParameters.Add(DBClient.AddParameters("InstallerSignature", SqlDbType.NVarChar, createVEEC.VEECInstaller.Signature));
                //sqlParameters.Add(DBClient.AddParameters("InstallerPhone", SqlDbType.NVarChar, createVEEC.VEECInstaller.Phone));
                //sqlParameters.Add(DBClient.AddParameters("InstallerMobile", SqlDbType.NVarChar, createVEEC.VEECInstaller.Mobile));
                //sqlParameters.Add(DBClient.AddParameters("InstallerUnitTypeID", SqlDbType.Int, createVEEC.VEECInstaller.UnitTypeID));
                //sqlParameters.Add(DBClient.AddParameters("InstallerUnitNumber", SqlDbType.NVarChar, createVEEC.VEECInstaller.UnitNumber));
                //sqlParameters.Add(DBClient.AddParameters("InstallerStreetNumber", SqlDbType.NVarChar, createVEEC.VEECInstaller.StreetNumber));
                //sqlParameters.Add(DBClient.AddParameters("InstallerStreetName", SqlDbType.NVarChar, createVEEC.VEECInstaller.StreetName));
                //sqlParameters.Add(DBClient.AddParameters("InstallerStreetTypeID", SqlDbType.NVarChar, createVEEC.VEECInstaller.StreetTypeID));
                //sqlParameters.Add(DBClient.AddParameters("InstallerTown", SqlDbType.NVarChar, createVEEC.VEECInstaller.Town));
                //sqlParameters.Add(DBClient.AddParameters("InstallerState", SqlDbType.NVarChar, createVEEC.VEECInstaller.State));
                //sqlParameters.Add(DBClient.AddParameters("InstallerPostCode", SqlDbType.NVarChar, createVEEC.VEECInstaller.PostCode));
                //sqlParameters.Add(DBClient.AddParameters("InstallerIsPostalAddress", SqlDbType.Bit, createVEEC.VEECInstaller.IsPostalAddress));
                //sqlParameters.Add(DBClient.AddParameters("InstallerPostalAddressID", SqlDbType.NVarChar, createVEEC.VEECInstaller.PostalAddressID));
                //sqlParameters.Add(DBClient.AddParameters("InstallerPostalDeliveryNumber", SqlDbType.NVarChar, createVEEC.VEECInstaller.PostalDeliveryNumber));
                //sqlParameters.Add(DBClient.AddParameters("InstallerElectricalContractorsLicenseNumber", SqlDbType.NVarChar, createVEEC.VEECInstaller.ElectricalContractorsLicenseNumber));
                //sqlParameters.Add(DBClient.AddParameters("InstallerCECAccreditationNumber", SqlDbType.NVarChar, createVEEC.VEECInstaller.CECAccreditationNumber));
                //sqlParameters.Add(DBClient.AddParameters("InstallerCECDesignerNumber", SqlDbType.NVarChar, createVEEC.VEECInstaller.CECDesignerNumber));
                //sqlParameters.Add(DBClient.AddParameters("InstallerLatitude", SqlDbType.NVarChar, createVEEC.VEECInstaller.Latitude));
                //sqlParameters.Add(DBClient.AddParameters("InstallerLongitude", SqlDbType.NVarChar, createVEEC.VEECInstaller.Longitude));
                //sqlParameters.Add(DBClient.AddParameters("InstallerIpAddress", SqlDbType.NVarChar, createVEEC.VEECInstaller.IpAddress));
                //sqlParameters.Add(DBClient.AddParameters("InstallerLocation", SqlDbType.NVarChar, createVEEC.VEECInstaller.Location));


                int veecId = 0;
                //DataSet dataset = CommonDAL.ExecuteDataSet("VEEC_Insert", sqlParameters.ToArray());
                //if (dataset != null && dataset.Tables.Count > 0)
                //{
                //    veecId = Convert.ToInt32(dataset.Tables[0].Rows[0].ItemArray[0].ToString());
                //}
                veecId = Convert.ToInt32(CommonDAL.ExecuteScalar("VEEC_Insert", sqlParameters.ToArray()));
                return veecId;
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return 0;
            }
        }

        /// <summary>
        /// Get VEEC Detail
        /// </summary>
        /// <param name="veecID">VEECId</param>
        /// <returns></returns>
        public CreateVEEC GetVeecByID(int veecID)
        {
            CreateVEEC createVEEC = new CreateVEEC();
            string spName = "[Veec_GetVeecByVeecId]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VeecId", SqlDbType.Int, veecID));
            DataSet dataSet = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());

            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                createVEEC.VEECDetail = dataSet.Tables[0].Rows.Count > 0 ? DBClient.DataTableToList<VEECDetail>(dataSet.Tables[0])[0] : new VEECDetail();
                string[] refNumber = createVEEC.VEECDetail.RefNumber.Split('-');
                createVEEC.VEECDetail.RefNumber_Prefix = string.Concat(refNumber[0], '-', refNumber[1], '-');
                for (int i = 2; i < refNumber.Length; i++)
                {
                    createVEEC.VEECDetail.RefNumber_Suffix = string.IsNullOrEmpty(createVEEC.VEECDetail.RefNumber_Suffix) ? refNumber[i] : string.Concat(createVEEC.VEECDetail.RefNumber_Suffix, '-', refNumber[i]);
                }
                createVEEC.VEECOwnerDetail = dataSet.Tables[1].Rows.Count > 0 ? DBClient.DataTableToList<VEECOwnerDetail>(dataSet.Tables[1])[0] : new VEECOwnerDetail();
                createVEEC.VEECInstallationDetail = dataSet.Tables[2].Rows.Count > 0 ? DBClient.DataTableToList<VEECInstallationDetail>(dataSet.Tables[2])[0] : new VEECInstallationDetail();
                createVEEC.lstBaselineEquipment = dataSet.Tables[3].Rows.Count > 0 ? DBClient.DataTableToList<BaselineEquipment>(dataSet.Tables[3]) : new List<BaselineEquipment>();
                createVEEC.lstUpgradeEquipment = dataSet.Tables[4].Rows.Count > 0 ? DBClient.DataTableToList<BaselineEquipment>(dataSet.Tables[4]) : new List<BaselineEquipment>();
                createVEEC.VEECInstaller = dataSet.Tables[5].Rows.Count > 0 ? DBClient.DataTableToList<VEECInstaller>(dataSet.Tables[5])[0] : new VEECInstaller();
                createVEEC.VEECUpgradeManagerDetail = dataSet.Tables[6].Rows.Count > 0 ? DBClient.DataTableToList<VEECUpgradeManagerDetail>(dataSet.Tables[6])[0] : new VEECUpgradeManagerDetail();
            }
            return createVEEC;
        }

        /// <summary>
        /// Get VEECList Based On Search Parameter
        /// </summary>
        /// <param name="PageNumber"></param>
        /// <param name="PageSize"></param>
        /// <param name="SortCol"></param>
        /// <param name="SortDir"></param>
        /// <param name="solarcompanyid"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="FromDateCommencement"></param>
        /// <param name="ToDateCommencement"></param>
        /// <param name="FromDateActivity"></param>
        /// <param name="ToDateActivity"></param>
        /// <param name="Searchtext"></param>
        /// <returns></returns>
        public List<VEECList> GetVEECList(int PageNumber, int PageSize, string SortCol, string SortDir, string solarcompanyid, DateTime? FromDate, DateTime? ToDate, DateTime? FromDateCommencement, DateTime? ToDateCommencement, DateTime? FromDateActivity, DateTime? ToDateActivity, string Searchtext)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            //sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            //sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, UserTypeId));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, PageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, PageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, SortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, SortDir));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, Convert.ToInt32(solarcompanyid)));
            sqlParameters.Add(DBClient.AddParameters("FromDate", SqlDbType.DateTime, FromDate != null ? FromDate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("ToDate", SqlDbType.DateTime, ToDate != null ? ToDate : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("FromDateCommencement", SqlDbType.DateTime, FromDateCommencement != null ? FromDateCommencement : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("ToDateCommencement", SqlDbType.DateTime, ToDateCommencement != null ? ToDateCommencement : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("FromDateActivity", SqlDbType.DateTime, FromDateActivity != null ? FromDateActivity : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("ToDateActivity", SqlDbType.DateTime, ToDateActivity != null ? ToDateActivity : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("Searchtext", SqlDbType.NVarChar, Searchtext));
            List<VEECList> lstVEEC = CommonDAL.ExecuteProcedure<VEECList>("VEEC_GetVEECList", sqlParameters.ToArray()).ToList();
            return lstVEEC;
        }


        public int GetSCOIdByVeecId(int VeecID)
        {
            string spName = "[Veec_GetSCOByVeecId]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VeecID", SqlDbType.Int, VeecID));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            object scoID = CommonDAL.ExecuteScalar(spName, sqlParameters.ToArray());
            if (Convert.ToString(scoID) == string.Empty)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(scoID);
            }

        }

        /// <summary>
        /// Get VEEC Data For Upload Excel to VEEC Portal
        /// </summary>
        /// <param name="VeecId"></param>
        /// <returns></returns>
        public DataSet GetVeecData(int VeecId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VeecID", SqlDbType.Int, VeecId));
            DataSet ds = CommonDAL.ExecuteDataSet("GetVeecDataForExcel", sqlParameters.ToArray());
            return ds;
        }


        public List<VEECNotes> GetVeecNotesListOnVisit(int veecID)
        {
            List<VEECNotes> lstVeecNotes = new List<VEECNotes>();
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("VeecID", SqlDbType.Int, veecID));
                lstVeecNotes = CommonDAL.ExecuteProcedure<VEECNotes>("VEECNotes_GetVeecNoteList", sqlParameters.ToArray()).ToList();
                if (lstVeecNotes != null && lstVeecNotes.Count > 0)
                {
                    for (int i = 0; i < lstVeecNotes.Count; i++)
                    {
                        lstVeecNotes[i].strCreatedDate = lstVeecNotes[i].CreatedDate.ToString("dd/MM/yyyy");
                    }
                }
                return lstVeecNotes;
            }
            catch (Exception ex)
            {
                return lstVeecNotes;
            }

        }

        /// <summary>
        /// Get VEEC Scheduling Detail
        /// </summary>
        /// <param name="veecID"></param>
        /// <returns></returns>
        public List<VEECScheduling> GetVEECschedulingByVEECID(int veecID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VeecId", SqlDbType.Int, veecID));
            List<VEECScheduling> lstVeecSchedule = CommonDAL.ExecuteProcedure<VEECScheduling>("veec_GetSchedulingByVeecID", sqlParameters.ToArray()).ToList();
            if (lstVeecSchedule != null && lstVeecSchedule.Count > 0)
            {
                for (int i = 0; i < lstVeecSchedule.Count; i++)
                {
                    lstVeecSchedule[i].UserName = lstVeecSchedule[i].FirstName + "  " + lstVeecSchedule[i].LastName;
                    int month = lstVeecSchedule[i].CreatedDate.Month;
                    lstVeecSchedule[i].Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month).Substring(0, 3);
                    lstVeecSchedule[i].Year = lstVeecSchedule[i].CreatedDate.Year;
                    lstVeecSchedule[i].Date = lstVeecSchedule[i].CreatedDate.Day;

                    lstVeecSchedule[i].StatusName = ((SystemEnums.UserStatus)lstVeecSchedule[i].Status).ToString();
                    if (lstVeecSchedule[i].CreatedDate.Minute < 10)
                    {
                        lstVeecSchedule[i].time = lstVeecSchedule[i].CreatedDate.Hour + ":" + "0" + lstVeecSchedule[i].CreatedDate.Minute;
                    }
                    else
                    {
                        lstVeecSchedule[i].time = lstVeecSchedule[i].CreatedDate.Hour + ":" + lstVeecSchedule[i].CreatedDate.Minute;
                    }

                    //string visitNum = string.Empty;
                    //if (lstJobSchedule[i].VisitNum.ToString().Length == 1)
                    //    visitNum = "00" + lstJobSchedule[i].VisitNum.ToString();
                    //else if (lstJobSchedule[i].VisitNum.ToString().Length == 2)
                    //    visitNum = "0" + lstJobSchedule[i].VisitNum.ToString();
                    //else
                    //    visitNum = lstJobSchedule[i].VisitNum.ToString();

                    //lstJobSchedule[i].UniqueVisitID = lstJobSchedule[i].CreatedDate.Year.ToString().Substring(2) + (lstJobSchedule[i].CreatedDate.Month.ToString().Length == 1 ? "0" + lstJobSchedule[i].CreatedDate.Month.ToString() : lstJobSchedule[i].CreatedDate.Month.ToString()) + lstJobSchedule[i].CreatedDate.Day.ToString() + visitNum;
                    lstVeecSchedule[i].UniqueVisitID = lstVeecSchedule[i].VisitUniqueId;
                    lstVeecSchedule[i].strVisitStartDate = Convert.ToDateTime(lstVeecSchedule[i].strVisitStartDate).ToString("dd/MM/yyyy hh:mm tt");
                    lstVeecSchedule[i].strCompletedDate = Convert.ToDateTime(lstVeecSchedule[i].CompletedDate).ToString("dd/MM/yyyy hh:mm tt");
                }
            }

            return lstVeecSchedule;
        }

        public void VeecNotesMarkAsSeen(string veecNoteIds)
        {
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("VeecNotesId", SqlDbType.NVarChar, veecNoteIds));
                CommonDAL.Crud("VeecNotes_MarkAsSeen", sqlParameters.ToArray());
            }
            catch (Exception ex)
            {
            }
        }


        public List<AssignSCO> GetSCOUser()
        {
            string spName = "[User_GetSCOUser]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, FormBot.Helper.ProjectSession.LoggedInUserId));
            IList<AssignSCO> userTypeList = CommonDAL.ExecuteProcedure<AssignSCO>(spName, sqlParameters.ToArray());
            return userTypeList.ToList();
        }
        //public List<JobStage> GetVeecStagesWithCount(int userId, int userTypeId, int SolarCompanyId)
        //{
        //    string spName = "[Veec_GetVeecSatgesWithCount]";
        //    List<SqlParameter> sqlParameters = new List<SqlParameter>();
        //    sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, userId));
        //    sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.Int, userTypeId));
        //    sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
        //    List<JobStage> lstJobStage = CommonDAL.ExecuteProcedure<JobStage>(spName, sqlParameters.ToArray()).ToList();
        //    return lstJobStage;
        //}

        public List<JobList> GetVeecStages()
        {
            string spName = "[VeecStages_BindDropDown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            IList<JobList> lstJobStages = CommonDAL.ExecuteProcedure<JobList>(spName, sqlParameters.ToArray());
            return lstJobStages.ToList();
        }

        public DataSet GetPreApprovalStatus()
        {
            string spName = "[VeecStatus_GetPreApprovalStatus]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            DataSet dsPreApprovalStatus = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
            return dsPreApprovalStatus;
        }

        public List<CustomDetail> GetVeecCustomDetails(int JobId, int SolarCompanyId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobID", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            List<CustomDetail> lstCustomDetail = CommonDAL.ExecuteProcedure<CustomDetail>("GetJobCustomDetails", sqlParameters.ToArray()).ToList();
            return lstCustomDetail;

        }

        /// <summary>
        /// Add VEEC Installer to VEEC Installer Dropdown Menu
        /// </summary>
        /// <param name="SolarCompanyId"></param>
        /// <param name="UserId"></param>
        /// <param name="Firstname"></param>
        /// <param name="Lastname"></param>
        /// <param name="CompanyName"></param>
        /// <param name="ElectricalComplianceNumber"></param>
        /// <param name="ElectricalContractorsLicenseNumber"></param>
        /// <param name="VeecInstallerId"></param>
        /// <returns></returns>
        public int AddVEECInstaller(int SolarCompanyId, int UserId, string Firstname, string Lastname, string CompanyName, string ElectricalComplianceNumber, string ElectricalContractorsLicenseNumber, int VeecInstallerId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));
            sqlParameters.Add(DBClient.AddParameters("VEECInstallerId", SqlDbType.Int, VeecInstallerId));
            sqlParameters.Add(DBClient.AddParameters("Firstname", SqlDbType.NVarChar, Firstname));
            sqlParameters.Add(DBClient.AddParameters("Lastname", SqlDbType.NVarChar, Lastname));
            sqlParameters.Add(DBClient.AddParameters("CompanyName", SqlDbType.NVarChar, CompanyName));
            sqlParameters.Add(DBClient.AddParameters("ElectricalComplianceNumber", SqlDbType.NVarChar, ElectricalComplianceNumber));
            sqlParameters.Add(DBClient.AddParameters("ElectricalContractorsLicenseNumber", SqlDbType.NVarChar, ElectricalContractorsLicenseNumber));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            int id = Convert.ToInt32(CommonDAL.ExecuteScalar("AddVEECInstaller", sqlParameters.ToArray()));
            return id;
        }

        /// <summary>
        /// Add Installer For Perticular VEEC
        /// </summary>
        /// <param name="veecInstallerId"></param>
        /// <param name="veecId"></param>
        public void UpdateVEECInstallerDetail(int veecInstallerId, int veecId)
        {

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VEECInstallerId", SqlDbType.Int, (veecInstallerId != 0) ? veecInstallerId : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("VEECId", SqlDbType.Int, veecId));
            CommonDAL.ExecuteScalar("UpdateVEECInstallerDetail", sqlParameters.ToArray());

        }

        /// <summary>
        /// Get VEEC Installer For SolarCompany
        /// </summary>
        /// <param name="SolarCompanyId"></param>
        /// <returns></returns>
        public DataSet GetVEECIntaller(int SolarCompanyId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            DataSet ds = CommonDAL.ExecuteDataSet("GetVEECInstaller", sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Get upgrade manager for solartcompany
        /// </summary>
        /// <param name="SolarCompanyId"></param>
        /// <returns></returns>
        public DataSet GetUpgradeManager(int SolarCompanyId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));
            DataSet ds = CommonDAL.ExecuteDataSet("GetVEECUpgradeManager", sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Delete veec custom installer (not system user) 
        /// </summary>
        /// <param name="VeecInstallerId"></param>
        public void DeleteVEECCustomInstaller(int VeecInstallerId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VeecInstallerId", SqlDbType.Int, VeecInstallerId));
            CommonDAL.ExecuteScalar("DeleteVEECCustomInstaller", sqlParameters.ToArray());
        }

        /// <summary>
        /// Delete custom upgrade manager (not system user)
        /// </summary>
        /// <param name="VEECUpgradeManagerId"></param>
        public void DeleteVEECCustomUpgradeManager(int VEECUpgradeManagerId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VEECUpgradeManagerId", SqlDbType.Int, VEECUpgradeManagerId));
            CommonDAL.ExecuteScalar("DeleteVEECCustomUpgradeManager", sqlParameters.ToArray());
        }

        /// <summary>
        /// Add upgrade manager to veec 
        /// </summary>
        /// <param name="veecUpgradeManagerId"></param>
        /// <param name="VeecId"></param>
        /// <param name="IsSysUser"></param>
        public void UpdateVEECUpgradeManager(int veecUpgradeManagerId, int VeecId, bool IsSysUser)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VEECUpgradeManagerId", SqlDbType.Int, (veecUpgradeManagerId != 0) ? veecUpgradeManagerId : (object)DBNull.Value));
            sqlParameters.Add(DBClient.AddParameters("VEECId", SqlDbType.Int, VeecId));
            sqlParameters.Add(DBClient.AddParameters("IsSysUser", SqlDbType.Bit, IsSysUser));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.ExecuteScalar("UpdateVEECUpgradeManager", sqlParameters.ToArray());

        }

        /// <summary>
        /// Add upgrade manager to dropdown menu
        /// </summary>
        /// <param name="veecUpgradeManagerDetail"></param>
        /// <returns></returns>
        public int AddVEECUpgradeManager(VEECUpgradeManagerDetail veecUpgradeManagerDetail)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, veecUpgradeManagerDetail.SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("Firstname", SqlDbType.NVarChar, veecUpgradeManagerDetail.FirstName));
            sqlParameters.Add(DBClient.AddParameters("LastName", SqlDbType.NVarChar, veecUpgradeManagerDetail.LastName));
            sqlParameters.Add(DBClient.AddParameters("CompanyName", SqlDbType.NVarChar, veecUpgradeManagerDetail.CompanyName));
            sqlParameters.Add(DBClient.AddParameters("Phone", SqlDbType.NVarChar, veecUpgradeManagerDetail.Phone));
            sqlParameters.Add(DBClient.AddParameters("CompanyABN", SqlDbType.NVarChar, veecUpgradeManagerDetail.CompanyABN));
            sqlParameters.Add(DBClient.AddParameters("VEECUpgradeManagerDetailId", SqlDbType.Int, veecUpgradeManagerDetail.VEECUpgradeManagerDetailId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            int id = Convert.ToInt32(CommonDAL.ExecuteScalar("AddVEECUpgradeManager", sqlParameters.ToArray()));
            return id;
        }

        /// <summary>
        /// Insert veec area for calculation
        /// </summary>
        /// <param name="VEECArea"></param>
        /// <returns></returns>
        public int InsertVeecArea(VEECArea VEECArea)
        {
            try
            {
                int VEECAreaId = 0;
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                /**************************************************VEEC Area Detail***************************************************************/
                sqlParameters.Add(DBClient.AddParameters("VEECAreaId", SqlDbType.Int, VEECArea.VEECAreaId));
                sqlParameters.Add(DBClient.AddParameters("VEECId", SqlDbType.Int, VEECArea.VEECId));
                sqlParameters.Add(DBClient.AddParameters("Name", SqlDbType.VarChar, VEECArea.Name));
                sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
                sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
                sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
                sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));

                VEECAreaId = Convert.ToInt32(CommonDAL.ExecuteScalar("Insert_Area", sqlParameters.ToArray()));
                return VEECAreaId;
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return 0;
            }

        }

        /// <summary>
        /// Get all area 
        /// </summary>
        /// <param name="VEECIds"></param>
        /// <param name="SortCol"></param>
        /// <param name="SortDir"></param>
        /// <returns></returns>
        public List<VEECArea> GetVEECAreaNameRecords(long VEECIds, string SortCol, string SortDir)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VEECId", SqlDbType.BigInt, VEECIds));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, SortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, SortDir));
            List<VEECArea> lstareaNameRecords = CommonDAL.ExecuteProcedure<VEECArea>("VEEC_GetVEECAreaName", sqlParameters.ToArray()).ToList();
            return lstareaNameRecords;
        }

        /// <summary>
        /// Delete veec area 
        /// </summary>
        /// <param name="VEECAreaId"></param>
        /// <returns></returns>
        public long DeleteVEECAreaName(long VEECAreaId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VEECAreaId", SqlDbType.BigInt, VEECAreaId));
            CommonDAL.ExecuteScalar("DeleteAreaName", sqlParameters.ToArray());
            return VEECAreaId;
        }

        public List<VEECArea> CheckIfNameExists(string name)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Name", SqlDbType.VarChar, name));
            List<VEECArea> lstNameExists = CommonDAL.ExecuteProcedure<VEECArea>("CheckIfVEECAreaNameExists", sqlParameters.ToArray()).ToList();
            return lstNameExists;
        }

        public List<string> RemoveRequiredFields(BaselineEquipment baselineequipment)
        {
            List<string> lstField = new List<string>();
            if (baselineequipment.BaselineUpgrade == 1)
            {
                lstField.Add("baselineequipment.UpgradeAssetLifetimeReference");
                lstField.Add("baselineequipment.ProductBrand");
                lstField.Add("baselineequipment.ProductModel");
                lstField.Add("baselineequipment.TypeOfFirstController");
                lstField.Add("baselineequipment.TypeOfSecondController");
                if (baselineequipment.Spacetype.ToString() == "26")
                {

                }
                else
                {
                    lstField.Add("baselineequipment.SpaceTypeUnlisted");
                }
            }
            if (baselineequipment.BaselineUpgrade == 2)
            {
                lstField.Add("baselineequipment.LampBallastCombination");
                lstField.Add("baselineequipment.BaselineAssetLifetimeReference");
                lstField.Add("baselineequipment.NominalLampPower");
                lstField.Add("baselineequipment.SpaceTypeUnlisted");
                //if(baselineequipment.Spacetype.ToString() == "26")
                //{
                //    lstField.Add("baselineequipment.SpaceType_Unlisted");
                //}
            }
            return lstField;
        }

        /// <summary>
        /// Insert baseline and upgrade data
        /// </summary>
        /// <param name="BaselineEquipment"></param>
        /// <returns></returns>
        public int InsertBaseLineEquipment(BaselineEquipment BaselineEquipment)
        {
            try
            {
                int BaselineEquipmentId = 0;
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                /**************************************************BaseLine Equipment Detail***************************************************************/
                sqlParameters.Add(DBClient.AddParameters("BaselineEquipmentId", SqlDbType.Int, BaselineEquipment.BaselineEquipmentId));
                sqlParameters.Add(DBClient.AddParameters("VEECId", SqlDbType.Int, BaselineEquipment.VEECId));
                sqlParameters.Add(DBClient.AddParameters("VEECAreaId", SqlDbType.Int, BaselineEquipment.VEECAreaId));
                sqlParameters.Add(DBClient.AddParameters("SpaceType", SqlDbType.Int, BaselineEquipment.Spacetype));
                sqlParameters.Add(DBClient.AddParameters("SpaceTypeUnlisted", SqlDbType.NVarChar, BaselineEquipment.SpaceTypeUnlisted));
                sqlParameters.Add(DBClient.AddParameters("BCAClassification", SqlDbType.Int, BaselineEquipment.BCAClassification));
                sqlParameters.Add(DBClient.AddParameters("BaselineUpgrade", SqlDbType.Int, BaselineEquipment.BaselineUpgrade));
                sqlParameters.Add(DBClient.AddParameters("LampBallastCombination", SqlDbType.Int, BaselineEquipment.LampBallastCombination));
                sqlParameters.Add(DBClient.AddParameters("LampCategory", SqlDbType.Int, BaselineEquipment.Lampcategory));
                sqlParameters.Add(DBClient.AddParameters("Quantity", SqlDbType.Decimal, BaselineEquipment.Quantity));
                sqlParameters.Add(DBClient.AddParameters("BaselineAssetLifetimeReference", SqlDbType.Int, BaselineEquipment.BaselineAssetLifetimeReference));
                sqlParameters.Add(DBClient.AddParameters("UpgradeAssetLifetimeRefrence", SqlDbType.Int, BaselineEquipment.UpgradeAssetLifetimeReference));
                sqlParameters.Add(DBClient.AddParameters("ProductBrand", SqlDbType.NVarChar, BaselineEquipment.ProductBrand));
                sqlParameters.Add(DBClient.AddParameters("ProductModel", SqlDbType.NVarChar, BaselineEquipment.ProductModel));
                sqlParameters.Add(DBClient.AddParameters("RatedLifetimeHours", SqlDbType.Decimal, BaselineEquipment.RatedLifetimeHours));
                sqlParameters.Add(DBClient.AddParameters("NominalLampPower", SqlDbType.Decimal, BaselineEquipment.NominalLampPower));
                sqlParameters.Add(DBClient.AddParameters("TypeOfFirstController", SqlDbType.Int, BaselineEquipment.TypeOfFirstController));
                sqlParameters.Add(DBClient.AddParameters("TypeOfSecondController", SqlDbType.Int, BaselineEquipment.TypeOfSecondController));
                sqlParameters.Add(DBClient.AddParameters("VRUProductBrand", SqlDbType.NVarChar, BaselineEquipment.VRUProductBrand));
                sqlParameters.Add(DBClient.AddParameters("VRUProductModel", SqlDbType.NVarChar, BaselineEquipment.VRUProductModel));
                sqlParameters.Add(DBClient.AddParameters("HVACAC", SqlDbType.Bit, BaselineEquipment.HVACAC));
                sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
                sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
                sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
                sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
                sqlParameters.Add(DBClient.AddParameters("LampType", SqlDbType.Int, BaselineEquipment.LampType));
                BaselineEquipmentId = Convert.ToInt32(CommonDAL.ExecuteScalar("Insert_BaselineUpgradeEquipment", sqlParameters.ToArray()));
                return BaselineEquipmentId;
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return 0;
            }

        }

        /// <summary>
        /// Delete baseline and upgrade data 
        /// </summary>
        /// <param name="BaselineEquipmentId"></param>
        /// <returns></returns>
        public long DeleteBaselineUpgradeEquipment(long BaselineEquipmentId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("BaselineEquipmentId", SqlDbType.BigInt, BaselineEquipmentId));
            CommonDAL.ExecuteScalar("DeleteBaselineUpgradeEquipment", sqlParameters.ToArray());
            return BaselineEquipmentId;
        }

        /// <summary>
        /// Rearrange calc zone based on baseline and upgrade and area name asc 
        /// </summary>
        /// <param name="VEECId"></param>
        /// <returns></returns>
        public int RearrangeCalcZone(int VEECId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VEECId", SqlDbType.Int, VEECId));
            int lastUpdateCaclcZoneId = Convert.ToInt32(CommonDAL.ExecuteScalar("RearrangeCalcZoneBaselineEquipment", sqlParameters.ToArray()));
            return lastUpdateCaclcZoneId;
        }

        /// <summary>
        /// Get baseline data
        /// </summary>
        /// <param name="id"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public DataSet Lstbaseline(int id, int areaId)
        {

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VeecId", SqlDbType.BigInt, id));
            sqlParameters.Add(DBClient.AddParameters("VeecAreaId", SqlDbType.BigInt, areaId));
            sqlParameters.Add(DBClient.AddParameters("BaselineUpgrade", SqlDbType.Int, 1));
            DataSet ds = CommonDAL.ExecuteDataSet("Veec_GetVeecBaselineUpgradeAndNonJ6Scenario", sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Get Upgrade data
        /// </summary>
        /// <param name="id"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public DataSet Lstupgradeline(int id, int areaId)
        {

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VeecId", SqlDbType.BigInt, id));
            sqlParameters.Add(DBClient.AddParameters("VeecAreaId", SqlDbType.BigInt, areaId));
            sqlParameters.Add(DBClient.AddParameters("BaselineUpgrade", SqlDbType.Int, 2));
            DataSet ds = CommonDAL.ExecuteDataSet("Veec_GetVeecBaselineUpgradeAndNonJ6Scenario", sqlParameters.ToArray());
            return ds;
        }

        /// <summary>
        /// Show hide bca classification if space type annual opertaing hour not defined
        /// </summary>
        /// <param name="spaceTypeid"></param>
        /// <returns></returns>
        public int showHideBCAClassificationOnSpaceTypeChange(int spaceTypeid)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SpaceTypeId", SqlDbType.Int, spaceTypeid));
            int result = Convert.ToInt32(CommonDAL.ExecuteScalar("showHideBCAClassificationOnSpaceTypeChange", sqlParameters.ToArray()));
            return result;
        }
        /// <summary>
        /// Get dropdown data
        /// </summary>
        /// <param name="cData"></param>
        /// <returns></returns>
        public DataSet GetData(List<CommonData> cData)
        {
            DataSet ds = new DataSet();
            foreach (CommonData obj in cData)
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                if (obj.parameters != null && obj.parameters.Length > 0)
                {
                    foreach (Dictionary<string, string> obj1 in obj.parameters)
                    {
                        sqlParameters.Add(DBClient.AddParameters((obj1.Keys).First(), SqlDbType.Int, obj1[(obj1.Keys).First()]));
                    }

                }
                if (obj.id == "ScoIDVEEC")
                {
                    sqlParameters.Add(DBClient.AddParameters("UserID", SqlDbType.Int, FormBot.Helper.ProjectSession.LoggedInUserId));
                }

                DataTable dt = new DataTable();
                dt = CommonDAL.ExecuteDataSet(obj.proc, sqlParameters.ToArray()).Tables[0].Copy();
                dt.TableName = obj.id;
                ds.Tables.Add(dt);
            }
            return ds;
        }

        /// <summary>
        /// Get checklist photo
        /// </summary>
        /// <param name="VeecId"></param>
        /// <returns></returns>
        public DataSet GetChecklistPhotosVeec(int VeecId)
        {
            try
            {

                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("VeecId", SqlDbType.Int, VeecId));
                DataSet ds = CommonDAL.ExecuteDataSet("GetVeecVisitCheckListPhotos", sqlParameters.ToArray());
                return ds;
            }


            catch (Exception ex)
            {
                return new DataSet();
            }
        }

        public DataSet GetPhotosPath(string VeecVisitCheckListPhotoIds, string VeecVisitSignatureIds)
        {
            //DataSet ds = new DataSet();
            //  foreach (CommonData obj in cData)
            // {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VeecVisitCheckListPhotoIds", SqlDbType.NVarChar, VeecVisitCheckListPhotoIds));
            sqlParameters.Add(DBClient.AddParameters("VeecVisitSignatureIds", SqlDbType.NVarChar, VeecVisitSignatureIds));
            DataSet ds = CommonDAL.ExecuteDataSet("Get_VeecVisitCheckListPhotos_VeecVisitSignature", sqlParameters.ToArray());
            return ds;
        }

        public DataSet GetVeecPhotosPath(string VeecPhotoId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VeecPhotoId", SqlDbType.NVarChar, VeecPhotoId));
            DataSet ds = CommonDAL.ExecuteDataSet("Get_VeecPhotosFolder", sqlParameters.ToArray());
            return ds;
        }
        public DataSet DeleteVeecPhotos(string photoId, string areaId, string veecId)
        {
            string spName = "[DeleteVeecPhotos]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VeecPhotoId", SqlDbType.VarChar, photoId));
            sqlParameters.Add(DBClient.AddParameters("VeecAreaId", SqlDbType.VarChar, areaId));
            sqlParameters.Add(DBClient.AddParameters("VeecId", SqlDbType.VarChar, veecId));
            return CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
        }

        public List<string> GetProductmodel(string ProductBrand)
        {
            string spName = "[VEEC_GetProductModel]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ProductBrand", SqlDbType.NVarChar, ProductBrand));
            DataSet ds = CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());

            return ds.Tables[0].AsEnumerable().Select(r => r.Field<string>("Model")).ToList();
        }
        public DataSet DeleteCheckListPhotos(string chkIds, string sigIds, string VeecSchedulingIds)
        {
            string spName = "[DeleteCheckListPhotosVeec]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("CheckListIds", SqlDbType.VarChar, chkIds));
            sqlParameters.Add(DBClient.AddParameters("SignatureIds", SqlDbType.VarChar, sigIds));
            sqlParameters.Add(DBClient.AddParameters("VeecSchedulingIds", SqlDbType.VarChar, VeecSchedulingIds));
            return CommonDAL.ExecuteDataSet(spName, sqlParameters.ToArray());
        }
        public DataSet GetCesPhotosByVEECVisitCheckListId(int VeecVisitCheckListItemId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VisitCheckListItemId", SqlDbType.Int, VeecVisitCheckListItemId));
            return CommonDAL.ExecuteDataSet("GetCesPhotosByVeecVisitCheckListId", sqlParameters.ToArray());
        }

        public void DeleteVEECActivitySchedulePremises()
        {
            CommonDAL.ExecuteScalar("DeleteVEECActivitySchedulePremises");
        }
        public int InsertReferencePhoto(int veecID, string filename, int UserId, string veecscId, string cId, bool isDefault, string ClassType, string VendorVeecPhotoId, string Status, string Latitude, string Longitude, string ImageTakenDate)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VeecId", SqlDbType.Int, veecID));
            sqlParameters.Add(DBClient.AddParameters("Path", SqlDbType.NVarChar, filename));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserId));

            if (!string.IsNullOrEmpty(cId))
                sqlParameters.Add(DBClient.AddParameters("VeecVisitCheckListItemId", SqlDbType.Int, Convert.ToInt32(cId)));

            if (!string.IsNullOrEmpty(veecscId))
                sqlParameters.Add(DBClient.AddParameters("VeecSchedulingId", SqlDbType.Int, Convert.ToInt32(veecscId)));

            if (string.IsNullOrEmpty(veecscId) && string.IsNullOrEmpty(cId) && !isDefault)
                sqlParameters.Add(DBClient.AddParameters("IsReference", SqlDbType.Bit, true));


            if (isDefault)
            {
                sqlParameters.Add(DBClient.AddParameters("ClassType", SqlDbType.VarChar, ClassType));
                sqlParameters.Add(DBClient.AddParameters("IsDefault", SqlDbType.Bit, isDefault));
            }

            if (!string.IsNullOrEmpty(Status))
            {
                sqlParameters.Add(DBClient.AddParameters("Status", SqlDbType.Int, Convert.ToInt32(Status)));
            }
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("Latitude", SqlDbType.VarChar, Latitude));
            sqlParameters.Add(DBClient.AddParameters("Longitude", SqlDbType.VarChar, Longitude));
            sqlParameters.Add(DBClient.AddParameters("VendorVeecPhotoId", SqlDbType.NVarChar, VendorVeecPhotoId));

            if (!string.IsNullOrEmpty(ImageTakenDate))
                sqlParameters.Add(DBClient.AddParameters("ImageTakenDate", SqlDbType.NVarChar, ImageTakenDate));

            return Convert.ToInt32(CommonDAL.ExecuteScalar("InsertPreferencePhotosVeec", sqlParameters.ToArray()));
        }

        public int InsertCESDocuments(int VeecId, string Path, int UserID, string Type, string JsonData)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VeecID", SqlDbType.Int, VeecId));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, UserID));
            sqlParameters.Add(DBClient.AddParameters("Path", SqlDbType.VarChar, Path));
            sqlParameters.Add(DBClient.AddParameters("Type", SqlDbType.VarChar, Type));
            sqlParameters.Add(DBClient.AddParameters("JsonData", SqlDbType.VarChar, JsonData));
            return Convert.ToInt32(CommonDAL.ExecuteScalar("Insert_CES_DocumentVeec", sqlParameters.ToArray()));
            //Crud("Insert_CES_Document", sqlParameters.ToArray());
        }

        public DataSet GetVeecsPhotos(int veecId)
        {
            try
            {

                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(DBClient.AddParameters("VeecId", SqlDbType.Int, veecId));
                DataSet ds = CommonDAL.ExecuteDataSet("GetVeecPhotos", sqlParameters.ToArray());
                return ds;
            }


            catch (Exception ex)
            {
                return new DataSet();
            }
        }

        public DataSet GetVeecFolderStructure()
        {
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                DataSet ds = CommonDAL.ExecuteDataSet("GetVeecFolderStructure", sqlParameters.ToArray());
                return ds;
            }
            catch (Exception ex)
            {
                return new DataSet();
            }
        }

        public int InsertVeecPhoto(int veecID, string filename, int UserId, int veecAreaId, int folderId, string Status, string Latitude, string Longitude, string ImageTakenDate)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VeecId", SqlDbType.Int, veecID));
            sqlParameters.Add(DBClient.AddParameters("VeecAreaId", SqlDbType.Int, veecAreaId));
            sqlParameters.Add(DBClient.AddParameters("FullPath", SqlDbType.VarChar, filename));
            sqlParameters.Add(DBClient.AddParameters("FolderId", SqlDbType.Int, folderId));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            return Convert.ToInt32(CommonDAL.ExecuteScalar("InsertPhotosVeec", sqlParameters.ToArray()));
        }

        public DataSet UploadVeec_GetVeecByID(int veecId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VEECId", SqlDbType.Int, veecId));
            DataSet dataSet = CommonDAL.ExecuteDataSet("UploadVEEC_GetVEECById", sqlParameters.ToArray());
            return dataSet;
        }

        public void VEECUpdateDetails_InsertUpdate(int veecPortalId, int veecUploadId, int noOfVEECs, int veecId, bool isDeleted)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(DBClient.AddParameters("VEECPortalId", SqlDbType.Int, veecPortalId));
            sqlParameters.Add(DBClient.AddParameters("VEECUploadId", SqlDbType.Int, veecUploadId));
            sqlParameters.Add(DBClient.AddParameters("NoOfVEECs", SqlDbType.Int, noOfVEECs));
            sqlParameters.Add(DBClient.AddParameters("VEECId", SqlDbType.Int, veecId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("IsDeleted", SqlDbType.Bit, isDeleted));

            CommonDAL.ExecuteScalar("VEECUpdateDetails_InsertUpdate", sqlParameters.ToArray());
        }
    }
}
