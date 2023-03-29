using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FormBot.Entity
{
    public class Report
    {
        public int ReportID { get; set; }
        public string ReportName { get; set; }
        public string ReportDesc { get; set; }
        public string ReportDefaultInput { get; set; }
        public List<FormBot.Entity.Report> reportList { get; set; }
        public int DateGrouping { get; set; }

        public int ActionTypeBit { get; set; }
        public int OwnerAccountBit { get; set; }
        public int minOwnerAccount { get; set; }
        public int StatusTypeBit { get; set; }
        public int FuelSourceTypeBit { get; set; }
        public bool IsDateGroup { get; set; }

        public IEnumerable<SelectListItem> LstOwnerAccountUser { get; set; }
        public List<SelectListItem> LstOwnerAccountAssignedUser { get; set; }
        public string[] RECOwnerAccountAssignedUser { get; set; }
        public string hdnRECOwnerAccountAssignedUser { get; set; }
        public int RECOwnerAccountId { get; set; }
        public string OwnerAccount { get; set; }

        public IEnumerable<SelectListItem> LstActionTypeUser { get; set; }
        public List<SelectListItem> LstActionTypeAssignedUser { get; set; }
        public string[] RECActionTypeAssignedUser { get; set; }
        public string hdnRECActionTypeAssignedUser { get; set; }
        public int RECActionTypeId { get; set; }
        public string ActionType { get; set; }

        public IEnumerable<SelectListItem> LstStatusTypeUser { get; set; }
        public List<SelectListItem> LstStatusTypeAssignedUser { get; set; }
        public string[] RECStatusTypeAssignedUser { get; set; }
        public string hdnRECStatusTypeAssignedUser { get; set; }
        public int RECStatusId { get; set; }
        public string Status { get; set; }

        public IEnumerable<SelectListItem> LstFuelSourceUser { get; set; }
        public List<SelectListItem> LstFuelSourceAssignedUser { get; set; }
        public string[] RECFuelSourceAssignedUser { get; set; }
        public string hdnRECFuelSourceAssignedUser { get; set; }
        public int RECFuelSourceId { get; set; }
        public string FuelSource { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime Date { get; set; }
        public string strDate
        {
            get
            {
                return Date.ToString("dd/MM/yyyy");
            }

        }

        public int RECCOUNT { get; set; }

        public string State { get; set; }
        public string BuyingAccount { get; set; }
        public string SellingAccount { get; set; }

    }
}
