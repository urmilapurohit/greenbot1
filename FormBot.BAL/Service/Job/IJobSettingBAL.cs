using FormBot.Entity;
using FormBot.Entity.Job;
using FormBot.Entity.Notification;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service.Job
{
    public interface IJobSettingBAL
    {
        int InsertUpdateJobSettingData(int jobSettingId, bool isDefaultJobViewNew, int solarCompanyId, int createdBy, DateTime createdDate, int modifiedBy, DateTime modifiedDate);

        List<JobCustomField> JobCustomFieldList(int pageNumber, int pageSize, string sortCol, string sortDir, string name, int solarCompanyId);

        void DeleteJobCustomField(string jobCustomFieldIds);

        int JobCustomFieldInsertUpdate(JobCustomField jobCustomField);

        JobCustomField GetCustomField(int jobCustomFieldId);

        List<JobCustomField> GetAllCustomFieldOfCompany();

        void UpdateDefaultSetting(bool IsPreapproval, bool IsConnection, bool IsAllowTrade,bool IsAllowCreateJobNotification);

        DataSet GetDefaultSettings();

        DefaultSetting GetDefaultSettingsForJob(int JobId);

        void SaveDefaultForJob(bool IsPreapproval, bool IsConnection, int JobId);

        int SolarCompanyNotification_InsertUpdate(SolarCompanyNotification solarCompanyNotification);

        List<SolarCompanyNotification> SolarCompanyNotificationList(int pageNumber, int pageSize, string sortCol, string sortDir, string name);

        void DeleteNotification(string notificationIds);

        SolarCompanyNotification GetNotification(int notificationId);

        void PushNotificationInsert(PushNotificationsSend pushNotification);
        List<PushNotificationsSend> PushNotificationsSendList(int pageNumber, int pageSize, string sortCol, string sortDir, string name);

        List<SolarElectricianView> GetElectricianOfAllType(int JobType, int solarCompanyId, int ResellerId);

        List<User> GetContractor(int JobType, int SolarCompanyId, int ResellerId);
        List<SolarCompany> GetSolarCompanyByResellerID(int id);

        List<JobSettingLog> GetJobSettingLogs();


    }
}
