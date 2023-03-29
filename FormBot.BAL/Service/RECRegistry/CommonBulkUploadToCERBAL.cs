using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FormBot.BAL.Service.RECRegistry
{
    public class CommonBulkUploadToCERBAL : ICommonBulkUploadToCERBAL
    {
        private readonly ICreateJobBAL _createJob;

        public CommonBulkUploadToCERBAL(ICreateJobBAL createJob)
        {
            _createJob = createJob;
        }

        /// <summary>
        /// Generate CSV File For PVD Jobs
        /// </summary>
        /// <param name="JobID"></param>
        /// <param name="FilePath"></param>
        /// <param name="dtCSV_JobID"></param>
        public StringBuilder GetBulkUploadCSV_PVD(string JobID, string FilePath, ref DataSet dtCSV_JobID, ref DataTable dtSPVXmlPath, bool IsFileCreation = false, bool isDownloadCSVFile = false)
        {
            DataSet ds = _createJob.GetBulkUploadForJob(JobID);
            DataTable dt = new DataTable();
            DateTime? installationDate = null;
            DateTime april2022Date = Convert.ToDateTime("2022-04-01");
            StringBuilder csv = new StringBuilder();
            if (ds != null && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
                dtCSV_JobID = ds;
                dtSPVXmlPath = ds.Tables[1];
                if(ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                {
                    //installationDate = DateTime.ParseExact(ds.Tables[2].Rows[0]["Installation date"].ToString(), "dd/MM/yyyy hh:mm:ss", null);
                    installationDate = Convert.ToDateTime(ds.Tables[2].Rows[0]["Installation date"]);
                }
                else
                {
                    installationDate = DateTime.ParseExact(dt.Rows[0]["Installation date"].ToString(), "dd/MM/yyyy", null);
                }
            }

            if(installationDate < april2022Date)
            {
                csv.Append(@"Type of system,Installation date,System/panel brand,System/panel model,Inverter manufacturer,Inverter series,Inverter model number,Type of connection to the electricity grid,System mounting type,Is a site specific audit report available?,Are you installing a complete unit (adding capacity to an existing system is not considered a complete unit)?,If this system is additional capacity to an existing system please provide detailed information on the position of the new panels and inverter (if applicable). System upgrades without a note explaining new panel locations will be failed by the Clean Energy Regulator,For what period would you like to create RECs,What is the rated power output (in kW) of your small generation unit,Number of panels,Do you wish to use the default resource availability figure?,What is your resource availability (hours per annum) for your system?,Equipment model serial number(s),Are you creating certificates for a system that has previously been failed by the Clean Energy Regulator?,Accreditation code,Explanatory notes for re-creating certificates previously failed,Reference,Certificate tag,CEC accreditation statement,""Local, State and Territory government siting approvals"",Electrical safety documentation,Australian/New Zealand standards statement,Not grid-connected statement,Installation property type,Single or multi-story,""Installation property name, boat name or chassis number"",Installation unit type,Installation unit number,Installation street number,Installation street name,Installation street type,Installation town/Suburb,Installation state,Installation postcode,Installation latitude,Installation longitude,Is there more than one SGU at this address?,If the address entered above does not adequately describe the location of the system please provide further detailed information for the Clean Energy Regulator to locate the system,Additional system information,Owner type,Owner organisation name,Owner first name,Owner surname,Owner phone,Owner fax,Owner mobile,Owner email,Owner address type,Owner postal delivery type,Owner postal delivery number,Owner unit type,Owner unit number,Owner street number,Owner street name,Owner street type,Owner town/suburb,Owner state,Owner postcode,Owner country,Installer first name,Installer surname,CEC accredited installer number,Installer phone,Installer fax,Installer mobile,Installer email,Installer address type,Installer postal delivery type,Installer postal delivery number,Installer unit type,Installer unit number,Installer street number,Installer street name,Installer street type,Installer town/suburb,Installer state,Installer postcode,Electrician first name,Electrician surname,Licensed electrician number,Electrician phone,Electrician fax,Electrician mobile,Electrician email,Electrician address type,Electrician postal delivery type,Electrician postal delivery number,Electrician unit type,Electrician unit number,Electrician street number,Electrician street name,Electrician street type,Electrician town/suburb,Electrician state,Electrician postcode,Designer first name,Designer surname,CEC accredited designer number,Designer phone,Designer fax,Designer mobile,Designer email,Designer address type,Designer postal delivery type,Designer postal delivery number,Designer unit type,Designer unit number,Designer street number,Designer street name,Designer street type,Designer town/suburb,Designer state,Designer postcode,Retailer Name,Retailer ABN,National metering identifier (NMI),Battery storage manufacturer,Battery storage model,Is the battery system part of an aggregated control?,Has the installer changed default manufacturer setting of the battery storage system?,Signed data package,Documents Zip File,Installation Type,Installation Type Additional Information");
                csv.Append("\r\n");
            }
            else
            {
                csv.Append(@"Type of system,Installation date,Was a solar retailer involved in the procurement and installation of the system?,System/panel brand,System/panel model,Inverter manufacturer,Inverter series,Inverter model number,Type of connection to the electricity grid,System mounting type,Is a site specific audit report available?,Are you installing a complete unit (adding capacity to an existing system is not considered a complete unit)?,If this system is additional capacity to an existing system please provide detailed information on the position of the new panels and inverter (if applicable). System upgrades without a note explaining new panel locations will be failed by the Clean Energy Regulator,For what period would you like to create RECs,What is the rated power output (in kW) of your small generation unit,Number of panels,Number of inverters,Inverter serial number(s),Do you wish to use the default resource availability figure?,What is your resource availability (hours per annum) for your system?,Equipment model serial number(s),Are you creating certificates for a system that has previously been failed by the Clean Energy Regulator?,Accreditation code,Explanatory notes for re-creating certificates previously failed,Reference,Certificate tag,Designer CEC accreditation statement,Installer CEC accreditation statement,Design and Installation of Unit Statement,""Local, State and Territory government requirements"",Electrical safety documentation,Unit componentry statements,On-site attendance written statements,On-site attendance evidence statements,Not grid-connected statement,""True, correct and complete statement"",Retailer CEC accreditation statement,Retailer unit performance statement,Retailer completion and connection statements,Retailer provision of unit information statements,Retailer Conflicts of interest,Retailer eligibility statement,""Retailer True, correct and complete statement"",Installation property type,Single or multi-story,""Installation property name, boat name or chassis number"",Installation unit type,Installation unit number,Installation street number,Installation street name,Installation street type,Installation town/Suburb,Installation state,Installation postcode,Installation latitude,Installation longitude,Is there more than one SGU at this address?,If the address entered above does not adequately describe the location of the system please provide further detailed information for the Clean Energy Regulator to locate the system,Additional system information,Owner type,Owner organisation name,Owner first name,Owner surname,Owner phone,Owner fax,Owner mobile,Owner email,Owner address type,Owner postal delivery type,Owner postal delivery number,Owner unit type,Owner unit number,Owner street number,Owner street name,Owner street type,Owner town/suburb,Owner state,Owner postcode,Owner country,Installer first name,Installer surname,CEC accredited installer number,Installer phone,Installer fax,Installer mobile,Installer email,Installer address type,Installer postal delivery type,Installer postal delivery number,Installer unit type,Installer unit number,Installer street number,Installer street name,Installer street type,Installer town/suburb,Installer state,Installer postcode,Electrician first name,Electrician surname,Licensed electrician number,Electrician phone,Electrician fax,Electrician mobile,Electrician email,Electrician address type,Electrician postal delivery type,Electrician postal delivery number,Electrician unit type,Electrician unit number,Electrician street number,Electrician street name,Electrician street type,Electrician town/suburb,Electrician state,Electrician postcode,Designer first name,Designer surname,CEC accredited designer number,Designer phone,Designer fax,Designer mobile,Designer email,Designer address type,Designer postal delivery type,Designer postal delivery number,Designer unit type,Designer unit number,Designer street number,Designer street name,Designer street type,Designer town/suburb,Designer state,Designer postcode,Retailer Name,Retailer ABN,Retailer representative,Retailer role,National metering identifier (NMI),Battery storage manufacturer,Battery storage model,Is the battery system part of an aggregated control?,Has the installer changed default manufacturer setting of the battery storage system?,Signed data package,Documents Zip File,Installation Type,Installation Type Additional Information");
                csv.Append("\r\n");
            }

            if (IsFileCreation)
            {
                foreach (DataRow row in dt.Rows)
                {
                    bool IsFirstCol = true;
                    foreach (string item in row.ItemArray)
                    {
                        if (!IsFirstCol)
                            csv.Append(Helper.Helper.Common.StringToCSVCell(item.Contains(";") ? HttpUtility.HtmlDecode(item) : item) + ",");
                        IsFirstCol = false;
                    }
                    csv.Remove(csv.ToString().Length - 1, 1);
                    csv.Append("\r\n");
                }

                if(!isDownloadCSVFile)
                {
                    StreamWriter sw = new StreamWriter(FilePath, false);
                    sw.Write(csv);
                    sw.Close();
                }
            }
            return csv;
        }

        /// <summary>
        /// Get Bulk UploadSWHCSV
        /// </summary>
        /// <param name="JobID"></param>
        /// <param name="FilePath"></param>
        /// <param name="dtCSV_JobID"></param>
        public StringBuilder GetBulkUploadSWHCSV(string JobID, string FilePath, ref DataSet dtCSV_JobID, bool IsFileCreation = false, bool isDownloadCSVFile = false)
        {
            DataSet swhDataset = _createJob.GetSWHBulkUploadForJob(JobID);
            DataTable swhDatatable = new DataTable();
            StringBuilder csv = new StringBuilder();
            if (swhDataset != null && swhDataset.Tables.Count > 0)
            {
                swhDatatable = swhDataset.Tables[0];
                dtCSV_JobID = swhDataset;
            }
            if (IsFileCreation)
            {
                csv.Append("System brand,System model,Tank serial number(s),Number of panels,Installation date,Installation type,Is the volumetric capacity of this installation greater than 700L,Statutory declarations sent,Is your water heater second hand,Reference,Certificate tag,Installation property type,Installation single or multi-story,Installation property name or boat name or chassis number,Installation unit type,Installation unit number,Installation street number,Installation street name,Installation street type,Installation town/suburb,Installation state,Installation postcode,Installation latitude,Installation longitude,Is there more than one SWH/ASHP at this address,Additional system information,Creating certificates for previously failed SWH,Failed accreditation code,Explanatory notes for failed accreditation code,Installation special address,Owner type,Owner organisation name,Owner first name,Owner surname,Owner phone,Owner fax,Owner mobile,Owner email,Owner address type,Owner postal delivery type,Owner postal delivery number,Owner unit type,Owner unit number,Owner street number,Owner street name,Owner street type,Owner town/suburb,Owner state,Owner postcode,Owner country,Installer first name,Installer surname,Installer phone,Installer fax,Installer mobile,Installer email,Installer address type,Installer postal delivery type,Installer postal delivery number,Installer unit type,Installer unit number,Installer street number,Installer street name,Installer street type,Installer town/suburb,Installer state,Installer postcode");
                csv.Append("\r\n");
                foreach (DataRow row in swhDatatable.Rows)
                {
                    bool IsFirstCol = true;
                    foreach (string item in row.ItemArray)
                    {
                        if (!IsFirstCol)
                            csv.Append(Helper.Helper.Common.StringToCSVCell(item.Contains(";") ? HttpUtility.HtmlDecode(item) : item) + ",");
                        IsFirstCol = false;
                    }

                    csv.Remove(csv.ToString().Length - 1, 1);
                    csv.Append("\r\n");
                }
                if(!isDownloadCSVFile)
                {
                    StreamWriter sw = new StreamWriter(FilePath, false);
                    sw.Write(csv);
                    sw.Close();
                }
            }
            return csv;
        }
    }
}
