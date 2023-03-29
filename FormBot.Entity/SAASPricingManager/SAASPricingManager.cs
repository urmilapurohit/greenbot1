using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    public class SAASPricingManager
    {
        public string ResellerName { get; set; }

        public int SAASPricingId { get; set; }
        public int SAASUserId { get; set; }
        public decimal Price { get; set; }
        public string strPrice { get; set; }
        public int SettlementTermId { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public int LastUpdatedBy { get; set; }
        public bool IsEnable { get; set; }
        public bool IsGST { get; set; }
        public int BillingPeriod { get; set; }
        public int SettlementPeriod { get; set; }
        public int TotalRecords { get; set; }
        public string strLastUpdatedDate
        {
            get
            {
                return LastUpdatedDate.ToString("dd/MM/yyyy");
            }
        }

        public string strPricingType
        {
            get
            {
                if (SettlementTermId == 1)
                {
                    return "Price/User";
                }
                else if (SettlementTermId == 2)
                {
                    return "Price/STC";
                }
                else
                {
                    return "Price/Job";
                }

            }
        }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int AppliedRolesUserCount { get; set; }

        public string strAppliedRolesUserCount
        {
            //get
            //{
            //    return AppliedRolesUserCount + " of " + TotalAppliedRoles + " Users";
            //}
            get
            {
                //return AppliedRolesUserCount + "-users";
                return TotalAppliedRoles + "-users";
            }
        }
        public int TotalAppliedRoles { get; set; }
        public int Id { get; set; }
        //public string BillableTerm
        //{
        //    get
        //    {
        //        return "";
        //    }
        //}
        public string GlobalBillableTermId { get; set; }
        public string BillerCode { get; set; }
        public string BillableTerm { get; set; }
        public string TermName { get; set; }
        public string TermCode { get; set; }
        public string TermDescription { get; set; }
        public string strBillableSettingsId { get; set; }

        public string strBillableTerm
        {
            get
            {
                return AppliedRolesUserCount + " of " + TotalAppliedRoles + " Users";
            }
        }

        public bool IsGlobalGST { get; set; }
        public decimal GlobalPrice { get; set; }

        public string strBillableCode
        {
            get
            {
                //return "$" + GlobalPrice + "+GST " + TermCode;
                return "(" + BillerCode + ") " + TermName + " $" + GlobalPrice + "+GST " + TermCode;
            }
        }

        // User List Role wise
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserTypeName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
    }
}
