using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Job
{
    public class UserWiseColumns
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int ColumnID { get; set; }
        public double Width { get; set; }
        public int MenuID { get; set; }
        public int OrderNumber { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public int PageSize { get; set; }
    }

    public class ColumnMaster
    {
        public int ColumnID { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int MenuId { get; set; }
        public int Category { get; set; }
        public string EditColDisplayName { get; set; }
}

    public class ListColumnMaster
    {
        public List<ColumnMaster> lstBasicDetails { get; set; }
        public List<ColumnMaster> lstOwnerDetails { get; set; }
        public List<ColumnMaster> lstInstallationDetails { get; set; }
        public List<ColumnMaster> lstSystemDetails { get; set; }
        public List<ColumnMaster> lstSTCDetails { get; set; }
        public List<ColumnMaster> lstInstallerDetails { get; set; }
        public List<ColumnMaster> lstDesignerDetails { get; set; }
        public List<ColumnMaster> lstElectricianDetails { get; set; }
        public List<ColumnMaster> lstSolarCompanyDetails { get; set; }       
        public List<ColumnMaster> lstExtraSpecialColumns { get; set; }
        public List<ColumnMaster> lstValidationColumns { get; set; }

        public List<ColumnMaster> lstStcInvoiceDetails { get; set; }



    }
}
