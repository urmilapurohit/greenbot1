using FormBot.BAL.Service;
using FormBot.DAL;
using FormBot.Entity;
using FormBot.Entity.VEEC;
using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service.VEEC
{
    class VEECSchedulingBAL : IVEECSchedulingBAL
    {




        public DataSet VeecScheduling_InsertUpdateScheduling(int veecSchedulingID, int veecID, int userId, string label, string detail, DateTime startDate, TimeSpan startTime, DateTime? endDate, TimeSpan? endTime, DateTime createdDate, int createdBy, DateTime? modifiedDate, int? modifiedBy, bool isDeleted, int status, bool isNotification, bool isDrop, int solarCompanyId, int userTypeId, int veecCheckListTemplateId, string veecVisitCheckListItemIds, Int64 tempVeecSchedulingId, bool IsFromCalendarView)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VeecSchedulingID", SqlDbType.BigInt, veecSchedulingID));
            sqlParameters.Add(DBClient.AddParameters("VeecID", SqlDbType.BigInt, veecID));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.BigInt, userId));
            sqlParameters.Add(DBClient.AddParameters("Label", SqlDbType.NVarChar, label));
            sqlParameters.Add(DBClient.AddParameters("detail", SqlDbType.NVarChar, detail));
            sqlParameters.Add(DBClient.AddParameters("startDate", SqlDbType.Date, startDate));
            sqlParameters.Add(DBClient.AddParameters("startTime", SqlDbType.Time, startTime));
            sqlParameters.Add(DBClient.AddParameters("endDate", SqlDbType.Date, endDate));
            sqlParameters.Add(DBClient.AddParameters("endTime", SqlDbType.Time, endTime));
            sqlParameters.Add(DBClient.AddParameters("createdDate", SqlDbType.DateTime, createdDate));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, modifiedDate));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.BigInt, createdBy));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.BigInt, modifiedBy));
            sqlParameters.Add(DBClient.AddParameters("isDeleted", SqlDbType.Bit, isDeleted));
            sqlParameters.Add(DBClient.AddParameters("Status", SqlDbType.TinyInt, status));
            sqlParameters.Add(DBClient.AddParameters("isNotification", SqlDbType.Bit, isNotification));
            sqlParameters.Add(DBClient.AddParameters("isDrop", SqlDbType.Bit, isDrop));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.BigInt, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.BigInt, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("VeecCheckListTemplateId", SqlDbType.BigInt, veecCheckListTemplateId));

            sqlParameters.Add(DBClient.AddParameters("VeecVisitCheckListItemIds", SqlDbType.NVarChar, veecVisitCheckListItemIds));
            sqlParameters.Add(DBClient.AddParameters("TempVeecSchedulingId", SqlDbType.BigInt, tempVeecSchedulingId));

            sqlParameters.Add(DBClient.AddParameters("IsFromCalendarView", SqlDbType.Bit, IsFromCalendarView));


            DataSet veecSchedulingDetail = CommonDAL.ExecuteDataSet("VeecScheduling_InsertUpdateScheduling", sqlParameters.ToArray());
            return veecSchedulingDetail;

        }
        public VEECScheduling GetAllSchedulingDataOfVEEC(string id = null, bool isCheckListView = false, bool isReloadGridView = false, ICreateVeecBAL _veec = null)
        {
            int veecId = 0;
            if (!string.IsNullOrEmpty(id))
            {
                int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out veecId);
            }
            VEECScheduling veecScheduling = new VEECScheduling();
            veecScheduling.IsFromCalendarView = false;
            veecScheduling.VeecID = veecId;
            veecScheduling.lstVeecSchedule = _veec.GetVEECschedulingByVEECID(veecId);
            veecScheduling.lstVEECNotes = _veec.GetVeecNotesListOnVisit(veecId);

            veecScheduling.NewNotesCount = veecScheduling.lstVEECNotes.ToList().AsEnumerable().Where(a => a.IsSeen == false || a.IsSeen == null).Count();
            string notSeenJobNotesId = string.Join(",", veecScheduling.lstVEECNotes.ToList().AsEnumerable().Where(a => a.IsSeen == false || a.IsSeen == null).Select(a => a.VEECNotesID));

            veecScheduling.IsCheckListView = isCheckListView;

            int solarCompanyId = ProjectSession.SolarCompanyId;
            int userTypeId = ProjectSession.UserTypeId;
            int userId = ProjectSession.LoggedInUserId;

            DataSet calendarData = GetVeecSchedulingDataByVeecID(userId, userTypeId, solarCompanyId, veecId);
            if (calendarData != null && calendarData.Tables.Count > 0)
            {
                if (calendarData.Tables[0] != null && calendarData.Tables[0].Rows.Count > 0)
                {
                    var itemsSolarElectrician = calendarData.Tables[0].ToListof<SolarElectricianView>();

                    veecScheduling.solarElectrician = itemsSolarElectrician;
                }
                else
                {
                    veecScheduling.solarElectrician = new List<SolarElectricianView>();
                }
                if (calendarData.Tables[1] != null && calendarData.Tables[1].Rows.Count > 0)
                {
                    var itemsJobDetails = calendarData.Tables[1].ToListof<VEECDetail>();

                    

                    veecScheduling.veec = itemsJobDetails;
                }
                else
                {
                    veecScheduling.veec = new List<VEECDetail>();

                }
                if (calendarData.Tables[2] != null && calendarData.Tables[2].Rows.Count > 0)
                {
                    veecScheduling.DefaultVEECCheckListTemplateId = Convert.ToInt32(calendarData.Tables[2].Rows[0]["VEECCheckListTemplateId"]);
                }

            }
            veecScheduling.IsDashboard = false;

            if (!string.IsNullOrEmpty(notSeenJobNotesId))
            {
                _veec.VeecNotesMarkAsSeen(notSeenJobNotesId);
            }

            return veecScheduling;
        }

        public DataSet GetVeecSchedulingDataByVeecID(int userId, int userTypeId, int solarCompanyId, int veecId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.BigInt, userId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.BigInt, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.BigInt, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("veecId", SqlDbType.BigInt, veecId));

            DataSet dsJobSchedulingDetail = CommonDAL.ExecuteDataSet("Veec_GetSchedulingDataByVeecId", sqlParameters.ToArray());
            return dsJobSchedulingDetail;
        }

        public DataSet GetVeecSchedulingDetail(int veecSchedulingID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VeecSchedulingID", SqlDbType.BigInt, veecSchedulingID));
            DataSet dsJobSchedulingDetail = CommonDAL.ExecuteDataSet("VeecScheduleDetail_GetVeecScheduleDetail", sqlParameters.ToArray());
            return dsJobSchedulingDetail;
        }


        public List<SolarElectricianView> GetSolarElectricianForVeecType(int userId, int userTypeId, int solarCompanyId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.BigInt, userId));
            sqlParameters.Add(DBClient.AddParameters("UserTypeId", SqlDbType.BigInt, userTypeId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyID", SqlDbType.BigInt, solarCompanyId));
            List<SolarElectricianView> lstSolarElectrician = CommonDAL.ExecuteProcedure<SolarElectricianView>("GET_SE_SCVEEC", sqlParameters.ToArray()).ToList();
            return lstSolarElectrician;
        }

        public DataSet ChangeVisitStatus(int veecSchedulingId, int visitStatus, DateTime? completedDate)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VeecSchedulingId", SqlDbType.Int, veecSchedulingId));
            sqlParameters.Add(DBClient.AddParameters("VisitStatus", SqlDbType.Int, visitStatus));
            sqlParameters.Add(DBClient.AddParameters("CompletedDate", SqlDbType.DateTime, completedDate));
            //CommonDAL.Crud("JobScheduling_ChangeVisitStatus", sqlParameters.ToArray());

            DataSet dsJobSchedulingDetail = CommonDAL.ExecuteDataSet("VeecScheduling_ChangeVisitStatus", sqlParameters.ToArray());
            return dsJobSchedulingDetail;
        }

        public void MakeVisitAsDefaultSubmission(int veecId, int veecSchedulingId, bool isDefault)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("VeecId", SqlDbType.Int, veecId));
            sqlParameters.Add(DBClient.AddParameters("VeecSchedulingId", SqlDbType.Int, veecSchedulingId));
            sqlParameters.Add(DBClient.AddParameters("IsDefaultSubmission", SqlDbType.Bit, isDefault));
            CommonDAL.Crud("VeecScheduling_MakeVisitAsDefaultSubmission", sqlParameters.ToArray());
        }
    }
}
