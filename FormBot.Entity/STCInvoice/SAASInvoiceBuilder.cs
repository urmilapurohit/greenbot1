using FormBot.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class SAASInvoiceBuilder
    {
        public int UserTypeID { get; set; }
        public int UserRole { get; set; }

        public List<UserTypesSASS> lstUserTypes { get; set; }
        public List<UserRolesSASS> lstUserRoles { get; set; }

        public string strJobID { get; set; }
        public string strAllJobIds { get; set; }
        public string strProcessedJobID { get; set; }
        public string InvoiceID { get; set; }
        public int SettelmentTerm { get; set; }
        public int GlobalTermId { get; set; }
        public bool IsGlobalGST { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Rate { get; set; }
        public int QTY { get; set; }
        public int InvoiceAmount { get; set; }
        public string BillingPeriod { get; set; }
        public int CreatedBy
        {
            get
            {
                return Convert.ToInt32(ProjectSession.LoggedInUserId);
            }
        }

        public string BillingMonth { get; set; }
        public string BillingYear { get; set; }
        public int ResellerID { get; set; }
        public int LastInsertedJobId { get; set; }
        public bool isBulkRecord { get; set; }
    }

    public class UserTypesSASS
    {
        public int UserTypeID { get; set; }
        public string UserTypeName { get; set; }
        public string UserInfo { get; set; }
        public int UserTypeOrder { get; set; }
        public string ShortName { get; set; }
    }

    public class UserRolesSASS
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
    }
}
