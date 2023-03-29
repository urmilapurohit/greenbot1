using FormBot.DAL;
using FormBot.Entity.Job;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormBot.Helper;
using FormBot.Entity;
using FormBot.Entity.Notification;

namespace FormBot.BAL.Service.Job
{
    public class JobSettingBAL : IJobSettingBAL
    {
        public int InsertUpdateJobSettingData(int jobSettingId, bool isDefaultJobViewNew, int solarCompanyId, int createdBy, DateTime createdDate, int modifiedBy, DateTime modifiedDate)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobSettingId", SqlDbType.Int, jobSettingId));
            sqlParameters.Add(DBClient.AddParameters("IsDefaultJobViewNew", SqlDbType.Bit, isDefaultJobViewNew));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, createdBy));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, createdDate));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, modifiedBy));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, modifiedDate));
            object settingId = CommonDAL.ExecuteScalar("JobSetting_InsertUpdate", sqlParameters.ToArray());
            return Convert.ToInt32(settingId);
        }

        public JobSetting GetJobSettingData(int solarCompanyId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyId));
            JobSetting jobSettingData = CommonDAL.SelectObject<JobSetting>("JobSetting_GetJobSettingData", sqlParameters.ToArray());
            return jobSettingData;
        }

        public List<JobCustomField> JobCustomFieldList(int pageNumber, int pageSize, string sortCol, string sortDir, string name, int solarCompanyId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Name", SqlDbType.NVarChar, string.IsNullOrEmpty(name) ? null : name));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, sortDir));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyId));
            List<JobCustomField> lstCustomField = CommonDAL.ExecuteProcedure<JobCustomField>("JobCustomFieldList", sqlParameters.ToArray()).ToList();
            return lstCustomField;
        }

        public void DeleteJobCustomField(string jobCustomFieldIds)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobCustomFieldIds", SqlDbType.NVarChar, jobCustomFieldIds));
            CommonDAL.Crud("JobCustomField_Delete", sqlParameters.ToArray());
        }

        public int JobCustomFieldInsertUpdate(JobCustomField jobCustomField)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobCustomFieldId", SqlDbType.Int, jobCustomField.JobCustomFieldId));
            sqlParameters.Add(DBClient.AddParameters("CustomField", SqlDbType.NVarChar, jobCustomField.CustomField));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, jobCustomField.SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.NVarChar, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.NVarChar, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));

            object jobCustomFieldId = CommonDAL.ExecuteScalar("JobCustomField_InsertUpdate", sqlParameters.ToArray());
            return Convert.ToInt32(jobCustomFieldId);
        }

        public JobCustomField GetCustomField(int jobCustomFieldId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobCustomFieldId", SqlDbType.Int, jobCustomFieldId));
            JobCustomField jobCustomField = CommonDAL.SelectObject<JobCustomField>("JobCustomField_GetCustomField", sqlParameters.ToArray());
            return jobCustomField;
        }

		public List<JobCustomField> GetAllCustomFieldOfCompany()
		{
			string spName = "[GetAllCustomFieldOfCompany]";
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, ProjectSession.SolarCompanyId));
			IList<JobCustomField> customField = CommonDAL.ExecuteProcedure<JobCustomField>(spName, sqlParameters.ToArray());
			return customField.ToList();
		}

        public void UpdateDefaultSetting(bool IsPreapproval, bool IsConnection,bool IsAllowTrade,bool IsAllowCreateJobNotification)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            if (ProjectSession.UserTypeId == 4)
            {
                sqlParameters.Add(DBClient.AddParameters("IsPreapproval", SqlDbType.Bit, IsPreapproval));
                sqlParameters.Add(DBClient.AddParameters("IsConnection", SqlDbType.Bit, IsConnection));
                sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, ProjectSession.SolarCompanyId));
            }
            else if(ProjectSession.UserTypeId == 1)
            {
                sqlParameters.Add(DBClient.AddParameters("IsAllowTrade", SqlDbType.Bit, IsAllowTrade));
            }
            if(ProjectSession.UserTypeId == 1 || ProjectSession.UserTypeId == 2 || ProjectSession.UserTypeId == 3 || ProjectSession.UserTypeId == 4 || ProjectSession.UserTypeId == 5)
            {
                sqlParameters.Add(DBClient.AddParameters("IsAllowCreateJobNotification", SqlDbType.Bit, IsAllowCreateJobNotification));
                sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, ProjectSession.LoggedInUserId));
            }
            CommonDAL.Crud("UpdateDefaultSettings", sqlParameters.ToArray());
        }


        public DataSet GetDefaultSettings()
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, ProjectSession.SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("UserId", SqlDbType.Int, ProjectSession.LoggedInUserId));
            return CommonDAL.ExecuteDataSet("GetDefaultSettings", sqlParameters.ToArray());
        }

        public DefaultSetting GetDefaultSettingsForJob(int JobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, ProjectSession.SolarCompanyId));
            return CommonDAL.SelectObject<DefaultSetting>("GetDefaultSettingsForJob", sqlParameters.ToArray());
        }

        public void SaveDefaultForJob(bool IsPreapproval, bool IsConnection, int JobId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("IsPreapproval", SqlDbType.Bit, IsPreapproval));
            sqlParameters.Add(DBClient.AddParameters("IsConnection", SqlDbType.Bit, IsConnection));
            sqlParameters.Add(DBClient.AddParameters("JobId", SqlDbType.Int, JobId));
            CommonDAL.Crud("UpdateDefaultSettingsForJob", sqlParameters.ToArray());
        }

        public int SolarCompanyNotification_InsertUpdate(SolarCompanyNotification solarCompanyNotification)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("NotificationId", SqlDbType.Int, solarCompanyNotification.NotificationId));
            sqlParameters.Add(DBClient.AddParameters("NotificationTitle", SqlDbType.NVarChar, solarCompanyNotification.NotificationTitle));
            sqlParameters.Add(DBClient.AddParameters("NotificationContent", SqlDbType.NVarChar, solarCompanyNotification.NotificationContent));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("ExpiryDate", SqlDbType.DateTime, solarCompanyNotification.ExpiryDate));
            sqlParameters.Add(DBClient.AddParameters("IsSpecialNotification", SqlDbType.Bit, solarCompanyNotification.IsSpecialNotification));
            sqlParameters.Add(DBClient.AddParameters("IsShowToAll", SqlDbType.Bit, solarCompanyNotification.ShowToAll));

            object NotificationId = CommonDAL.ExecuteScalar("SolarCompanyNotification_InsertUpdate", sqlParameters.ToArray());
            return Convert.ToInt32(NotificationId);
        }

        public List<SolarCompanyNotification> SolarCompanyNotificationList(int pageNumber, int pageSize, string sortCol, string sortDir, string name)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Name", SqlDbType.NVarChar, string.IsNullOrEmpty(name) ? null : name));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("SortCol", SqlDbType.NVarChar, sortCol));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, sortDir));

            List<SolarCompanyNotification> lstSolarCompanyNotification = CommonDAL.ExecuteProcedure<SolarCompanyNotification>("[SolarCompanyNotificationList]", sqlParameters.ToArray()).ToList();
            return lstSolarCompanyNotification;
        }

        public SolarCompanyNotification GetNotification(int notificationId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("NotificationId", SqlDbType.Int, notificationId));
            SolarCompanyNotification notification = CommonDAL.SelectObject<SolarCompanyNotification>("SolarCompanyNotification_GetNotification", sqlParameters.ToArray());
            return notification;
        }

        public void DeleteNotification(string notificationIds)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("NotificationId", SqlDbType.NVarChar, notificationIds));
            sqlParameters.Add(DBClient.AddParameters("ModifiedBy", SqlDbType.NVarChar, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("ModifiedDate", SqlDbType.DateTime, DateTime.Now));
            CommonDAL.Crud("[SolarCompanyNotification_Delete]", sqlParameters.ToArray());
        }

        public void PushNotificationInsert(PushNotificationsSend pushNotification)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Notification", SqlDbType.NVarChar, pushNotification.Notification));
            sqlParameters.Add(DBClient.AddParameters("CreatedBy", SqlDbType.Int, ProjectSession.LoggedInUserId));
            sqlParameters.Add(DBClient.AddParameters("CreatedDate", SqlDbType.DateTime, DateTime.Now));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, pushNotification.SolarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, pushNotification.ResellerId));
            sqlParameters.Add(DBClient.AddParameters("Electricianid", SqlDbType.Int, pushNotification.ElectricianId));
            sqlParameters.Add(DBClient.AddParameters("ContractorId", SqlDbType.Int, pushNotification.ContractorId));
            sqlParameters.Add(DBClient.AddParameters("JobType", SqlDbType.Int, pushNotification.JobType));
            sqlParameters.Add(DBClient.AddParameters("Platform", SqlDbType.Int, pushNotification.Platform));

            CommonDAL.ExecuteScalar("PushNotificationInsert", sqlParameters.ToArray());

        }

        public List<PushNotificationsSend> PushNotificationsSendList(int pageNumber, int pageSize, string sortCol, string sortDir, string name)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("Name", SqlDbType.NVarChar, string.IsNullOrEmpty(name) ? null : name));
            sqlParameters.Add(DBClient.AddParameters("PageNumber", SqlDbType.Int, pageNumber));
            sqlParameters.Add(DBClient.AddParameters("PageSize", SqlDbType.Int, pageSize));
            sqlParameters.Add(DBClient.AddParameters("SortDir", SqlDbType.VarChar, sortDir));
            List<PushNotificationsSend> lstpushNotification = CommonDAL.ExecuteProcedure<PushNotificationsSend>("[PushNotificationList]", sqlParameters.ToArray()).ToList();
            return lstpushNotification;
        }
        public List<SolarCompany> GetSolarCompanyByResellerID(int id)
        {
            string spName = "[SolarCompanyByResellerID_BindDropDown]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("ResellerID", SqlDbType.Int, id));
            IList<SolarCompany> solarCompanyList = CommonDAL.ExecuteProcedure<SolarCompany>(spName, sqlParameters.ToArray());
            return solarCompanyList.ToList();
        }
        public List<SolarElectricianView> GetElectricianOfAllType(int JobType, int solarCompanyId, int ResellerId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, solarCompanyId));
            sqlParameters.Add(DBClient.AddParameters("JobType", SqlDbType.Int, JobType));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            List<SolarElectricianView> lstSolarElectrician = CommonDAL.ExecuteProcedure<SolarElectricianView>("GetElectricianOfAllType", sqlParameters.ToArray()).ToList();
            return lstSolarElectrician;
        }
        public List<User> GetContractor(int JobType, int SolarCompanyId, int ResellerId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(DBClient.AddParameters("JobType", SqlDbType.Int, JobType));
            sqlParameters.Add(DBClient.AddParameters("ResellerId", SqlDbType.Int, ResellerId));
            sqlParameters.Add(DBClient.AddParameters("SolarCompanyId", SqlDbType.Int, SolarCompanyId));

            List<User> lstcontractor = CommonDAL.ExecuteProcedure<User>("[GetContractor]", sqlParameters.ToArray()).ToList();
            return lstcontractor;
        }
        public List<JobSettingLog> GetJobSettingLogs()
        {
            List<JobSettingLog> jobSettingLogs = CommonDAL.ExecuteProcedure<JobSettingLog>("GETJobSettingLog", null).ToList();
            return jobSettingLogs;
        }

    }
}
