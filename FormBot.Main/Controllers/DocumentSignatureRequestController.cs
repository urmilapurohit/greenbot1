using FormBot.BAL;
using FormBot.BAL.Service;
using FormBot.BAL.Service.CommonRules;
using FormBot.BAL.Service.Documents;
using FormBot.BAL.Service.DocumentSignatureRequest;
using FormBot.BAL.Service.Job;
using FormBot.BAL.Service.SPV;
using FormBot.Entity.Documents;
using FormBot.Entity.DocumentSignatureRequest;
using FormBot.Entity.Email;
using FormBot.Entity.Job;
using FormBot.Entity.Pdf;
using FormBot.Helper;
using FormBot.Main.Infrastructure;
using FormBot.Main.Models;
using GenericParsing;
using iTextSharp.text.pdf;
using LumenWorks.Framework.IO.Csv;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static FormBot.Helper.SystemEnums;
using static System.Enum;
namespace FormBot.Main.Controllers
{
	public class DocumentSignatureRequestController : Controller
	{
		#region Properties
		private readonly IDocumentsBAL _documentsBAL;
		private readonly IDocumentSignatureRequestBAL _documentSignatureRequestBAL;
		private readonly ICreateJobBAL _job;
		private readonly IEmailBAL _emailBAL;
		private readonly IUserBAL _user;
		private readonly IEmailBAL _emailService;
		private readonly IUserBAL _userBAL;
		private readonly IJobSchedulingBAL _jobSchedule;
		private readonly IJobInvoiceBAL _jobInvoice;
		private readonly IJobInvoiceDetailBAL _jobInvoiceDetail;
		private readonly IJobDetailsBAL _jobDetails;
		private readonly ICreateJobHistoryBAL _jobHistory;
		private readonly ISTCInvoiceBAL _stcInvoiceServiceBAL;
		private readonly IJobRulesBAL _jobRules;
		private readonly ICERImportBAL _cerImportBAL;
		private readonly IGenerateStcReportBAL _generateStcReportBAL;
		private readonly IJobSettingBAL _jobSettingBAL;
		private readonly Logger _logger;
		private readonly ICommonRECMethodsBAL _commonRECMethodsBAL;
		private readonly IDocumentSignatureLogBAL _documentSignatureLogBAL;
		private readonly IDocumentSignatureOptionBAL _documentSignatureOptionBAL;
		private readonly IDocumentUserEmailServiceBAL _documentUserEmailServiceBAL;
        private readonly ISpvLogBAL _spvLog;
		private readonly ISpvVerificationBAL _spvVerificationBAL;
		#endregion
		#region Constructor
		public DocumentSignatureRequestController(IUserBAL user, ICreateJobBAL job, IUserBAL userBAL, IDocumentsBAL documentsBAL, IJobSchedulingBAL jobScheduling, IEmailBAL emailService, IJobInvoiceBAL jobInvoice, IJobInvoiceDetailBAL jobInvoiceDetailBAL, IJobDetailsBAL jobDetails, ICreateJobHistoryBAL jobHistory, ISTCInvoiceBAL stcInvoiceServiceBAL, IEmailBAL emailBAL, IJobRulesBAL jobRules, ICERImportBAL cerImportBAL, IGenerateStcReportBAL generateStcReportBAL, IJobSettingBAL jobSettingBAL, IDocumentSignatureRequestBAL documentSignatureRequestBAL, ICommonRECMethodsBAL commonRECMethodsBAL, IDocumentSignatureLogBAL documentSignatureLogBAL, IDocumentSignatureOptionBAL documentSignatureOptionBAL, IDocumentUserEmailServiceBAL documentUserEmailServiceBAL, ISpvLogBAL spvLog, ISpvVerificationBAL spvVerificationBAL)
		{
			this._user = user;
			this._job = job;
			this._userBAL = userBAL;
			this._documentsBAL = documentsBAL;
			this._jobSchedule = jobScheduling;
			this._emailService = emailService;
			this._jobInvoice = jobInvoice;
			this._jobInvoiceDetail = jobInvoiceDetailBAL;
			this._jobDetails = jobDetails;
			this._jobHistory = jobHistory;
			this._stcInvoiceServiceBAL = stcInvoiceServiceBAL;
			this._emailBAL = emailBAL;
			this._jobRules = jobRules;
			this._cerImportBAL = cerImportBAL;
			this._generateStcReportBAL = generateStcReportBAL;
			this._jobSettingBAL = jobSettingBAL;
			this._documentSignatureRequestBAL = documentSignatureRequestBAL;
			this._logger = new Logger();
			this._commonRECMethodsBAL = commonRECMethodsBAL;
			this._documentSignatureLogBAL = documentSignatureLogBAL;
			this._documentSignatureOptionBAL = documentSignatureOptionBAL;
			this._documentUserEmailServiceBAL = documentUserEmailServiceBAL;
            this._spvLog = spvLog;
			this._spvVerificationBAL = spvVerificationBAL;
		}
		#endregion
		#region Method
		// GET: DocumentEmail
		public ActionResult Index()
		{
			return View();
		}
		public JsonResult GetDocumentTemplateByStateId(int stateId)
		{
			DataSet dsState = _documentsBAL.GetDocumentTemplateByStateId(stateId);
			return Json(Newtonsoft.Json.JsonConvert.SerializeObject(dsState), JsonRequestBehavior.AllowGet);
		}
		[HttpPost]
		public JsonResult InsertUpdateGroupName(string GroupName, string BulkUploadDocumentGroupId, int DocumentTemplateId = 0)
		{

			int UploadDocumentGroupId = 0;
			if (BulkUploadDocumentGroupId != "0" && !string.IsNullOrEmpty(BulkUploadDocumentGroupId))
			{
				int.TryParse(QueryString.GetValueFromQueryString(BulkUploadDocumentGroupId, "id"), out UploadDocumentGroupId);
			}

			int bulkUploadDocumentGroupId = _documentSignatureRequestBAL.InsertUpdateGroupName(GroupName, UploadDocumentGroupId);
			if (UploadDocumentGroupId <= 0)
			{
				DataSet ds = _documentSignatureRequestBAL.GetDocumentPathFromDocumentTemplateId(DocumentTemplateId);
				string documentTemplatePath = ds.Tables[0].Rows[0]["Path"].ToString();
				string FileTypeName = ds.Tables[0].Rows[0]["FileTypeName"].ToString();
				string groupDocumentPath = "BulkUploadGroupDocuments//" + bulkUploadDocumentGroupId + "//" + FileTypeName;
				string sourcePath = Path.Combine(ProjectSession.ProofDocuments, documentTemplatePath);
				string destPath = Path.Combine(ProjectSession.ProofDocuments, groupDocumentPath);
				if (!Directory.Exists(destPath))
				{
					Directory.CreateDirectory(destPath);
				}
				string renameFileName = string.Empty;
				int j = 0;
				while (true)
				{
					if (j == 0)
						renameFileName = destPath + "/" + System.IO.Path.GetFileNameWithoutExtension(documentTemplatePath) + ".pdf";
					else
						renameFileName = destPath + "/" + System.IO.Path.GetFileNameWithoutExtension(documentTemplatePath) + "(" + j + ")" + ".pdf";

					if (System.IO.File.Exists(renameFileName))
						j++;
					else
						break;
				}
				if (System.IO.File.Exists(sourcePath))
				{
					System.IO.File.Copy(sourcePath, renameFileName, true);
				}
				bulkUploadDocumentGroupId = _documentSignatureRequestBAL.InsertUpdateGroupName(GroupName, bulkUploadDocumentGroupId, groupDocumentPath + "\\" + Path.GetFileName(renameFileName));
			}

			if (bulkUploadDocumentGroupId > 0)
			{
				return Json(new { status = true }, JsonRequestBehavior.AllowGet);
			}
			return Json(new { status = false }, JsonRequestBehavior.AllowGet);
		}
		public void GetBulkUploadDocumentGroupNameList(string groupName)
		{
			GridParam gridParam = Grid.ParseParams(HttpContext.Request);
			int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
			IList<BulkUploadDocumentGroup> lstBulkUploadDocumentGroup = new List<BulkUploadDocumentGroup>();

			lstBulkUploadDocumentGroup = _documentSignatureRequestBAL.GroupNameList(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, groupName);

			if (lstBulkUploadDocumentGroup.Any())
			{
				foreach (var item in lstBulkUploadDocumentGroup)
				{
					item.GroupEditURL = Url.Action("EditGroup", "DocumentSignatureRequest", new { groupId = item.Id });
					item.DocumentTemplateName = Path.GetFileNameWithoutExtension(item.GroupDocumentPath);
				}
			}

			if (lstBulkUploadDocumentGroup.Count > 0)
			{
				gridParam.TotalDisplayRecords = lstBulkUploadDocumentGroup.FirstOrDefault().TotalRecords;
				gridParam.TotalRecords = lstBulkUploadDocumentGroup.FirstOrDefault().TotalRecords;
			}

			HttpContext.Response.Write(Grid.PrepareDataSet(lstBulkUploadDocumentGroup, gridParam));
		}
		public void GetBulkJobListForSendEmail(string bulkUploadDocumentGroupId)
		{
			IList<DocumentWiseSignatureDetails> lstBulkUploadDocumentGroup = new List<DocumentWiseSignatureDetails>();
			GridParam gridParam = Grid.ParseParams(HttpContext.Request);
			int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
			if (string.IsNullOrEmpty(bulkUploadDocumentGroupId))
			{
				gridParam.TotalDisplayRecords = 0;
				gridParam.TotalRecords = 0;
				HttpContext.Response.Write(Grid.PrepareDataSet(lstBulkUploadDocumentGroup, gridParam));
				return;
			}
			else
			{
				//string path = _documentSignatureRequestBAL.GetDocumentPathFromGroupId(Convert.ToInt32(bulkUploadDocumentGroupId));
				////string path = @"JobDocuments\DocumentsTemplate\Pre Approvals\NT\NT Power and Water Corp\PWC-Power-Purchase-Agreement.pdf";
				//string Fullpath = Path.Combine(ProjectSession.ProofDocuments, path);
				//string data = new JavaScriptSerializer().Serialize(GetSignatureStatus(Fullpath).Data);
				//var obj = JsonConvert.DeserializeObject<DocumentWiseSignatureDetails>(data);
				//lstBulkUploadDocumentGroup = _documentSignatureRequestBAL.GetBulkJobListForSendEmail(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, referenceNumber, solarCompanyId, resellerId, obj.InstallerSignatureStatus.Equals("true") ? true : false, obj.HomeOwnerSignatureStatus.Equals("true") ? true : false, obj.SolarCompanySignatureStatus.Equals("true") ? true : false, obj.ElectricianSignatureStatus.Equals("true") ? true : false, obj.DesignerSignatureStatus.Equals("true") ? true : false);
				lstBulkUploadDocumentGroup = _documentSignatureRequestBAL.GetBulkJobListForSendEmail(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, Convert.ToInt32(bulkUploadDocumentGroupId));
				if (lstBulkUploadDocumentGroup.Count > 0)
				{
					gridParam.TotalDisplayRecords = lstBulkUploadDocumentGroup.FirstOrDefault().TotalRecords;
					gridParam.TotalRecords = lstBulkUploadDocumentGroup.FirstOrDefault().TotalRecords;
				}

				HttpContext.Response.Write(Grid.PrepareDataSet(lstBulkUploadDocumentGroup, gridParam));
			}

		}
		public void GetBulkJobListForAddToGroup(string resellerId, string referenceNumber = "", string solarCompanyId = "", int jobId = 0, int jobType = 0, string PVDSWHCode = "")
		{
			IList<DocumentWiseSignatureDetails> lstBulkUploadDocumentGroup = new List<DocumentWiseSignatureDetails>();
			GridParam gridParam = Grid.ParseParams(HttpContext.Request);
			int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
			lstBulkUploadDocumentGroup = _documentSignatureRequestBAL.GetBulkJobListForAddGroup(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, resellerId, referenceNumber, solarCompanyId, jobId, jobType, PVDSWHCode);
			if (lstBulkUploadDocumentGroup.Count > 0)
			{
				gridParam.TotalDisplayRecords = lstBulkUploadDocumentGroup.FirstOrDefault().TotalRecords;
				gridParam.TotalRecords = lstBulkUploadDocumentGroup.FirstOrDefault().TotalRecords;

			}

			HttpContext.Response.Write(Grid.PrepareDataSet(lstBulkUploadDocumentGroup, gridParam));
		}
		public static DataTable GetCSVTable()
		{
			DataTable dtCSVData = new DataTable();
			dtCSVData.Columns.Add("JobId", typeof(int));
			dtCSVData.Columns.Add("PVDSWHCode", typeof(string));
			return dtCSVData;
		}
		[HttpPost]
		public JsonResult ImportJobsInGroupsrtCSV(int BulkUploadDocumentGroupId)
		{
			try
			{
				var postedFile = Request.Files[0];
				DataTable dtCSV = new DataTable();
				var textReader = new StreamReader(postedFile.InputStream);
				using (CsvReader csv = new CsvReader(new StreamReader(postedFile.InputStream), true))
				{
					//dtCSV = ReadHeaders(csv);
					//while (csv.ReadNextRecord())
					//{
					dtCSV.Load(csv);
					//	DataRow drcsv = dtCSV.NewRow();
					//	for (int i = 0; i < dtCSV.Columns.Count; i++)
					//	{
					//		drcsv[i] = dtCSV.Columns[i].ToString();
					//	}
					//	dtCSV.Rows.Add(drcsv);
					//	dtCSV.AcceptChanges();
					//}
				}
				SendEmailRequest sendEmailRequest = new SendEmailRequest();
				sendEmailRequest.BulkUploadDocumentGroupId = BulkUploadDocumentGroupId;
				foreach (DataRow dr in dtCSV.Rows)
				{
					sendEmailRequest.lstDocumentWiseSignatureDetail.Add(new DocumentWiseSignatureDetails() { JobId = Convert.ToInt32(dr["JobId"]), PVDSWHCode = Convert.ToString(dr["PVDSWHCode"]) });
				}
				string path = _documentSignatureRequestBAL.GetDocumentPathFromGroupId(bulkUploadDocumentGroupId: Convert.ToInt32(sendEmailRequest.BulkUploadDocumentGroupId));
				if (!string.IsNullOrEmpty(path))
				{
					AddJobToGroup(path, sendEmailRequest);
					return Json(new { status = true }, JsonRequestBehavior.AllowGet);
				}
				return Json(new { status = false, error = "Document Path not getting." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				return Json(new { status = false }, JsonRequestBehavior.AllowGet);
			}
		}

		//Signature request email page
		public ActionResult SignatureRequest(string jobdocid, string type, string groupid)
		{
			JobDocumentSignatureDetails objDocumentSignature = new JobDocumentSignatureDetails();
			//DocumentCollectionView objDocumentCollectionView = new DocumentCollectionView();
			try
			{
				ViewBag.PageFrom = "SignatureRequest";

				int DocumentGroupId = 0, JobDocumentId = 0;
				List<int> SignatureType = new List<int>();
				if (!string.IsNullOrEmpty(groupid) && !string.IsNullOrEmpty(jobdocid) && !string.IsNullOrEmpty(type))
				{
					int.TryParse(QueryString.QueryStringDecode(groupid), out DocumentGroupId);
					int.TryParse(QueryString.QueryStringDecode(jobdocid), out JobDocumentId);

					if(!string.IsNullOrEmpty(type))
					{
						var data = QueryString.QueryStringDecode(type).Split(',').ToList();
						SignatureType.AddRange(data.Select(x => Convert.ToInt32(x)));
					}

					//int.TryParse(QueryString.QueryStringDecode(type), out SignatureType);
				}
				ViewBag.type = SignatureType;
				//JobController objJobController = new JobController(_user, _job, _userBAL, _documentsBAL, _jobSchedule, _emailService, _jobInvoice, _jobInvoiceDetail, _jobDetails, _jobHistory, _stcInvoiceServiceBAL, _emailBAL, _jobRules, _cerImportBAL, _generateStcReportBAL, _jobSettingBAL, _documentSignatureRequestBAL, _commonRECMethodsBAL, _documentSignatureLogBAL, _spvLog, _spvVerificationBAL);
				DocumentsView objDV = _job.GetJobDocumentByJobDocumentId(JobDocumentId);
				//objDocumentCollectionView = objJobController.GetPdfDetails(QueryString.QueryStringEncode("id=" + Convert.ToString(objDV.JobId)), QueryString.QueryStringEncode("id=" + Convert.ToString(0)), false, objDV.Path, JobDocumentId.ToString());
				GetBulkSignatureRequestDetails(jobdocid, type, groupid, out objDocumentSignature);
				CommonMethods.DocumentOpenFromEmailLog(JobDocumentId, objDocumentSignature, _documentSignatureLogBAL);
				return View(objDocumentSignature);
			}
			catch (Exception ex)
			{
				_logger.LogException(Severity.Error, "SignatureRequest Error", ex);
				return View(objDocumentSignature);
			}
		}
		public void GetBulkSignatureRequestDetails(string jobdocid, string type, string groupid, out JobDocumentSignatureDetails objDocumentSignature)
		{
			JobDocumentSignatureDetails objDocumentSignatureTemp = new JobDocumentSignatureDetails();
			int DocumentGroupId = 0, JobDocumentId = 0;//, SignatureType = 0;
			List<int> SignatureType = new List<int>();
			if (!string.IsNullOrEmpty(groupid) && !string.IsNullOrEmpty(jobdocid) && !string.IsNullOrEmpty(type))
			{
				int.TryParse(QueryString.QueryStringDecode(groupid), out DocumentGroupId);
				int.TryParse(QueryString.QueryStringDecode(jobdocid), out JobDocumentId);
				//int.TryParse(QueryString.QueryStringDecode(type), out SignatureType);
				if (!string.IsNullOrEmpty(type))
				{
					SignatureType.AddRange(QueryString.QueryStringDecode(type).Split(',').ToList().Select(x => Convert.ToInt32(x)));
				}
			}
			//string description = FormBot.Helper.SystemEnums.GetDescription(TypeOfSignature.Electrician);
			//TypeOfSignature value = FormBot.Helper.SystemEnums.GetValueFromDescription<TypeOfSignature>(description);
			DocumentsView objDV = _job.GetJobDocumentByJobDocumentId(JobDocumentId);
			objDocumentSignatureTemp.jobDocumentId = JobDocumentId;
			objDocumentSignatureTemp.jobDocumentPath = objDV.Path.Replace(@"\", @"\\");
			//objDocumentSignatureTemp.type = SignatureType;

			objDocumentSignatureTemp.jobid = objDV.JobId;
			ViewBag.DocumentGroupId = DocumentGroupId;
			ViewBag.FieldName = string.Join(",", SignatureType.Select(x=> FormBot.Helper.SystemEnums.GetDescription((TypeOfSignature)x)));
			objDocumentSignatureTemp.lsttype = SignatureType;
			objDocumentSignatureTemp.fieldName = ViewBag.FieldName;
			var UsersData = _documentSignatureRequestBAL.GetUserBasicDetailsBySignatureTypeOfJobDocument(SignatureType.FirstOrDefault(), objDV.JobId);
			if (UsersData != null)
			{
				objDocumentSignatureTemp.Firstname = UsersData.Firstname;
				objDocumentSignatureTemp.Lastname = UsersData.Lastname;
				objDocumentSignatureTemp.Email = UsersData.Email;
				objDocumentSignatureTemp.mobileNumber = UsersData.mobileNumber;
			}
			objDocumentSignature = objDocumentSignatureTemp;
		}
		[HttpGet]
		public PartialViewResult GetDocumentSpecificAllLogByDocumentId(int JobDocId, string DocName = "")
		{
			List<DocumentSignatureLog> lstDocumentSignatureLog = new List<DocumentSignatureLog>();
			try
			{
				ViewBag.FileName = DocName;
				lstDocumentSignatureLog = _documentSignatureLogBAL.GetByJobDocId(JobDocId);
				return PartialView("_DocumentSignatureLogListing", lstDocumentSignatureLog);
			}
			catch (Exception ex)
			{
				return PartialView("_DocumentSignatureLogListing", lstDocumentSignatureLog);
			}
		}
		[HttpGet]
		public PartialViewResult JobWiseDocumentSignatureRequest(string jobdocid, string type)
		{
			ViewBag.PageFrom = "JobDetails";
			JobDocumentSignatureDetails objDocumentSignature = new JobDocumentSignatureDetails();
			try
			{
				GetBulkSignatureRequestDetails(QueryString.QueryStringEncode(jobdocid), QueryString.QueryStringEncode(type), QueryString.QueryStringEncode("0"), out objDocumentSignature);
				return PartialView("_BulkUploadSignatureRequest", objDocumentSignature);
			}
			catch (Exception ex)
			{
				_logger.LogException(Severity.Error, "SignatureRequest Error", ex);
				return PartialView("_BulkUploadSignatureRequest", objDocumentSignature);
			}
		}
		[HttpPost]
		public JsonResult AddSignature(List<JobDocumentSignatureDetails> lstCaptureUserSign, string SignatureType, int DocumentGroupId, int JobId)
		{
			bool IsFail = false;
			string FilePath = "";
			int ControlFoundCount = 0;
			try
			{
				foreach (var item in lstCaptureUserSign)
				{
					DataSet ds = _job.InsertUserSignature(item.jobDocId.ToString(), "0", item.signString, item.IsImage, item.fieldName, item.mobileNumber, item.Firstname, item.Lastname, SystemEnums.SignatureType.Draw.GetHashCode(), item.Email);
					int jobDocId = item.jobDocId;
					if (ds.Tables[0].Rows.Count > 0)
					{
						DataTable dt = _documentsBAL.GetJobDocumentPath(jobDocId);
						int jobId = Convert.ToInt32(dt.Rows[0][1]);
						string path = Convert.ToString(dt.Rows[0][0]);
						FilePath = path;
						string jsonData = Convert.ToString(dt.Rows[0]["JsonData"]);
						string documentFullPath = Path.Combine(ProjectSession.ProofDocuments + "\\" + path);
						MemoryStream memStream = new MemoryStream();
						using (FileStream fileStream = System.IO.File.OpenRead(documentFullPath))
						{
							memStream.SetLength(fileStream.Length);
							fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
						}
						PdfReader pdfReader = null;
						PdfStamper pdfStamper = null;
						try
						{
							pdfReader = new PdfReader(memStream);
							Type type = typeof(PdfReader);
							FieldInfo info = type.GetField("unethicalreading", BindingFlags.Public | BindingFlags.Static);
							info.SetValue(pdfReader, true);
							pdfStamper = new PdfStamper(pdfReader, new FileStream(documentFullPath, FileMode.Create, FileAccess.ReadWrite));
							AcroFields pdfFormFields = pdfStamper.AcroFields;
							foreach (var field in pdfFormFields.Fields)
							{
								if (field.Key == item.fieldName)
								{
									PdfItems k = new PdfItems(0, field.Key, pdfFormFields.GetField(Convert.ToString(field.Key)), pdfFormFields.GetFieldType(Convert.ToString(field.Key)));
									k.Value = item.signString;
									if (item.IsImage)
									{
										k.Base64 = item.signString;
									}
									else
									{
										k.Base64 = item.Base64;
									}
									_job.FillSignature(k, null, false, pdfFormFields, pdfStamper);
									//JobController objJobController = new JobController(_user, _job, _userBAL, _documentsBAL, _jobSchedule, _emailService, _jobInvoice, _jobInvoiceDetail, _jobDetails, _jobHistory, _stcInvoiceServiceBAL, _emailBAL, _jobRules, _cerImportBAL, _generateStcReportBAL, _jobSettingBAL, _documentSignatureRequestBAL, _commonRECMethodsBAL, _documentSignatureLogBAL, _spvLog, _spvVerificationBAL);
									CloseOpenFileConnection(pdfStamper, pdfReader, memStream);
									List<PdfItems> lstPdfItems = CommonHelper.GetPDFItems(documentFullPath);
									var pdfItem = lstPdfItems.Where(m => m.FieldName.Equals(field.Key)).Select(m => { m.Base64 = k.Base64; m.Value = k.Value; return m; }).ToList();
									DocumentsView objDocumentView = new DocumentsView();
									objDocumentView.CreatedBy = ProjectSession.LoggedInUserId;
									objDocumentView.CreatedDate = DateTime.Now;
									objDocumentView.DocumentId = Convert.ToInt32(0);
									objDocumentView.JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(lstPdfItems);
									objDocumentView.JobDocumentId = jobDocId;
									_job.CreateJobDocuments(objDocumentView, false);
									ControlFoundCount++;
								}
							}
							//PrepareDocumentSignatureLog(jobDocId, new CaptureUserSign() { fieldName = item.Firstname, Lastname = item.Lastname }, SignatureType, 3);
						}
						catch (Exception ex)
						{
							IsFail = true;
						}
						finally
						{
							if (pdfStamper != null)
								pdfStamper.Close();
							if (pdfReader != null)
								pdfReader.Close();
							memStream.Close();
							memStream.Dispose();
						}
					}
				}
				if (ControlFoundCount > 0)
				{
					string UsersRoleName = "";
					foreach(var item in lstCaptureUserSign)
					{
						if(!UsersRoleName.Contains(item.fieldName))
						{
							if (string.IsNullOrEmpty(UsersRoleName))
								UsersRoleName = item.fieldName;
							else
								UsersRoleName += "," + item.fieldName;
						}
					}
					CommonMethods.DocumentSignLog(lstCaptureUserSign.FirstOrDefault(), _documentSignatureLogBAL, UsersRoleName);
				}
			}
			catch (Exception ex)
			{
				IsFail = true;
				return Json(false, JsonRequestBehavior.AllowGet);
			}
			if (!IsFail && ControlFoundCount > 0 && lstCaptureUserSign.Any())
			{
				var data = new DocumentWiseSignatureDetails();
				foreach(var item in SignatureType.Split(',').ToList())
				{
					data = _documentSignatureRequestBAL.UpdateSignatureStatusInBulkUploadSignatureRequest(Convert.ToInt32(item), lstCaptureUserSign.FirstOrDefault().jobDocId, DocumentGroupId, JobId);
				}
				
				if (!data.IsCompleted)
				{
					if (_documentSignatureRequestBAL.GetSignatureCompletedStatus(lstCaptureUserSign.FirstOrDefault().jobDocId, DocumentGroupId) == 1)
					{
						List<int> lstTypeSkip = new List<int>();
						string SentEmails = "";
						List<DocumentSignatureStatusWithEmailResponce> SentEmailDetails = _documentSignatureRequestBAL.GetDocumentSignatureStatusWithEmailByDocumentGroupIdAndJobId(0, JobId, true, lstCaptureUserSign.FirstOrDefault().jobDocId);

						var UsersEmailDetails = _documentUserEmailServiceBAL.GetByBulkUploadDocumentSignatureId(data.BulkUploadDocumentSignatureId);
						var DocumentOption = _documentSignatureOptionBAL.GetByBulkUploadDocumentSignatureId(data.BulkUploadDocumentSignatureId);
						if (SentEmailDetails.Any())
						{
							//int EmailTemplateID = 43;//Local
							int EmailTemplateID = 40;//Live
							var emailData = SentEmailDetails.FirstOrDefault();
							if (data.InstallerSignatureStatus == "True"
								&& !string.IsNullOrEmpty(emailData.InstallerEmail)
								&& !string.IsNullOrEmpty(emailData.InstallerFirstName)
								&& !string.IsNullOrEmpty(emailData.InstallerLastName))
							{
								if (DocumentOption.Any())
								{
									if (DocumentOption.Any(x => x.SameAsInstaller && x.SendCopy))
									{
										lstTypeSkip.AddRange(DocumentOption.Where(x => x.SameAsInstaller && x.SendCopy).Select(x => x.Type).ToList());
									}
								}

								SendMailWithAttechment(GetEmailIdWithManuallyAdded(UsersEmailDetails,emailData.InstallerEmail, TypeOfSignature.Installer), emailData.InstallerFirstName, emailData.InstallerLastName, FilePath, EmailTemplateID);
							}
							if (data.DesignerSignatureStatus == "True" && !lstTypeSkip.Any(x => x == TypeOfSignature.Designer.GetHashCode())
								&& !string.IsNullOrEmpty(emailData.DesignerEmail)
								&& !string.IsNullOrEmpty(emailData.DesignerFirstName)
								&& !string.IsNullOrEmpty(emailData.DesignerLastName)
								&& !SentEmails.ToLower().Contains(emailData.DesignerEmail))
							{
								SendMailWithAttechment(GetEmailIdWithManuallyAdded(UsersEmailDetails, emailData.DesignerEmail, TypeOfSignature.Designer), emailData.DesignerFirstName, emailData.DesignerLastName, FilePath, EmailTemplateID);
							}
							if (data.ElectricianSignatureStatus == "True" && !lstTypeSkip.Any(x => x == TypeOfSignature.Electrician.GetHashCode())
								&& !string.IsNullOrEmpty(emailData.ElectricianEmail)
								&& !string.IsNullOrEmpty(emailData.ElectricianFirstName)
								&& !string.IsNullOrEmpty(emailData.ElectricianLastName)
								&& !SentEmails.ToLower().Contains(emailData.DesignerEmail))
							{
								SendMailWithAttechment(GetEmailIdWithManuallyAdded(UsersEmailDetails, emailData.ElectricianEmail, TypeOfSignature.Electrician), emailData.ElectricianFirstName, emailData.ElectricianLastName, FilePath, EmailTemplateID);
							}
							if (data.HomeOwnerSignatureStatus == "True"
								&& !string.IsNullOrEmpty(emailData.OwnerEmail)
								&& !string.IsNullOrEmpty(emailData.OwnerFirstName)
								&& !string.IsNullOrEmpty(emailData.OwnerLastName))
							{
								SendMailWithAttechment(GetEmailIdWithManuallyAdded(UsersEmailDetails, emailData.OwnerEmail, TypeOfSignature.Home_Owner), emailData.OwnerFirstName, emailData.OwnerLastName, FilePath, EmailTemplateID);
							}
							if (data.SolarCompanySignatureStatus == "True"
								&& !string.IsNullOrEmpty(emailData.SolarCompanyEmail)
								&& !string.IsNullOrEmpty(emailData.SolarCompanyFirstName)
								&& !string.IsNullOrEmpty(emailData.SolarCompanyLastName))
							{
								SendMailWithAttechment(GetEmailIdWithManuallyAdded(UsersEmailDetails, emailData.SolarCompanyEmail, TypeOfSignature.SolarCompnay), emailData.SolarCompanyFirstName, emailData.SolarCompanyLastName, FilePath, EmailTemplateID);
							}

							if (_documentSignatureRequestBAL.GetSignatureCompletedStatus(lstCaptureUserSign.FirstOrDefault().jobDocId, DocumentGroupId) == 1)
							{
								CommonMethods.DocumentCompletedLog(lstCaptureUserSign.FirstOrDefault().jobDocId, _documentSignatureLogBAL);
								CommonMethods.CompletedDocumentSentLog(lstCaptureUserSign.FirstOrDefault().jobDocId, _documentSignatureLogBAL);
							}
						}
					}
				}
				else
					CommonMethods.DocumentCompletedLog(lstCaptureUserSign.FirstOrDefault().jobDocId, _documentSignatureLogBAL);
			}
			return Json("1");
		}
		public string GetEmailIdWithManuallyAdded(List<DocumentUserEmail> lstDocumentUserEmail,string EmailId,TypeOfSignature User)
		{
			if (lstDocumentUserEmail.Any(x => x.Type == User.GetHashCode()))
				return lstDocumentUserEmail.Where(x => x.Type == User.GetHashCode()).FirstOrDefault().EmailId;
			else
				return EmailId;
		}
		public void SendMailWithAttechment(string Email, string FirstName, string LastName, string FilePath,int TemplateID)
		{
			EmailInfo objEmailInfo = new EmailInfo();
			objEmailInfo.FirstName = FirstName;
			objEmailInfo.LastName = LastName;
			objEmailInfo.TemplateID = TemplateID;
			List<EmialAttechment> lstEmialAttechment = new List<EmialAttechment>();
			foreach (var path in FilePath.Split(';').ToList())
			{
				string fileExtention = Path.GetExtension(path);
				string fileName = Path.GetFileName(path);
				EmialAttechment objEmialAttechment = new EmialAttechment();
				objEmialAttechment.CreatedDate = DateTime.Now;
				objEmialAttechment.FileMimeType = MimeMapping.GetMimeMapping(fileExtention);
				objEmialAttechment.FileName = fileName;
				objEmialAttechment.FilePath = path;
				lstEmialAttechment.Add(objEmialAttechment);
			}

			_emailBAL.ComposeAndSendEmail(objEmailInfo, Email, null, lstEmialAttechment, Guid.NewGuid());
		}
		public void CloseOpenFileConnection(PdfStamper pdfStamper, PdfReader pdfReader, MemoryStream memStream)
		{
			if (pdfStamper != null)
				pdfStamper.Close();
			if (pdfReader != null)
				pdfReader.Close();
			memStream.Close();
			memStream.Dispose();
		}
		public ActionResult GetAllDocumentMessages(int JobDocumentId)
		{
			try
			{
				return PartialView("_AllMessageDocumentWise", _documentSignatureRequestBAL.GetAllMessageByJobDocumentIdWise(JobDocumentId));
			}
			catch (Exception ex)
			{
				return null;
			}
		}
		[HttpPost]
		//public JsonResult AddMessageWithDocumentMapped(JobDocumentMessage objJobDocumentMessage, string AllType, int GroupId = 0, int JobId = 0, int JobDocId = 0)
		//{
		//	try
		//	{
		//		int	emailTemplateId = 41;
		//		bool Result = false;
		//		List<string> AllReadySendEmail = new List<string>();
		//		foreach (var type in AllType.Split(','))
		//		{
					
		//			string description = FormBot.Helper.SystemEnums.GetDescription((TypeOfSignature)Convert.ToInt32(type));
		//			//TypeOfSignature value = FormBot.Helper.SystemEnums.GetValueFromDescription<TypeOfSignature>(fieldName);
		//			objJobDocumentMessage.TypeId = Convert.ToInt32(type);//value.GetHashCode();
		//			objJobDocumentMessage.MessageCategory = 4;
		//			List<DocumentSignatureStatusWithEmailResponce> SentEmailDetails = _documentSignatureRequestBAL.GetDocumentSignatureStatusWithEmailByDocumentGroupIdAndJobId(GroupId, JobId, true, JobDocId);
		//			List<CaptureUserSign> lstUserDetailsEmail = new List<CaptureUserSign>();
		//			if (SentEmailDetails != null)
		//			{
		//				if (SentEmailDetails.Any())
		//				{
		//					var data = SentEmailDetails.FirstOrDefault();
		//					string FirstName = "", LastName = "", Email = "";
		//					if (data.InstallerEmail != "NA" && description != "Installer_Signature")
		//					{
		//						lstUserDetailsEmail.Add(
		//							new CaptureUserSign()
		//							{
		//								Firstname = data.InstallerFirstName
		//								, Lastname = data.InstallerLastName
		//								, Email = data.InstallerEmail
		//							});
		//					}
		//					if (data.DesignerEmail != "NA" && description != "Designer_Signature")
		//					{
		//						lstUserDetailsEmail.Add(
		//							new CaptureUserSign()
		//							{
		//								Firstname = data.DesignerFirstName
		//								, Lastname = data.DesignerLastName
		//								, Email = data.DesignerEmail
		//							});
		//					}
		//					if (data.ElectricianEmail != "NA" && description != "Electrician_Signature")
		//					{
		//						lstUserDetailsEmail.Add(
		//							new CaptureUserSign()
		//							{
		//								Firstname = data.ElectricianFirstName
		//								, Lastname = data.ElectricianLastName
		//								, Email = data.ElectricianEmail
		//							});
		//					}
		//					if (data.OwnerEmail != "NA" && description != "Owner_Signature")
		//					{
		//						lstUserDetailsEmail.Add(
		//							new CaptureUserSign()
		//							{
		//								Firstname = data.OwnerFirstName
		//								, Lastname = data.OwnerLastName
		//								, Email = data.OwnerEmail
		//							});
		//					}
		//					if (data.SolarCompanyEmail != "NA" && description != "SCA_Signature")
		//					{
		//						lstUserDetailsEmail.Add(
		//							new CaptureUserSign()
		//							{
		//								Firstname = data.SolarCompanyFirstName
		//								, Lastname = data.SolarCompanyLastName
		//								, Email = data.SolarCompanyEmail
		//							});
		//					}
		//					if (!AllReadySendEmail.Any(x => x == Email))
		//					{
		//						int Id = _documentSignatureRequestBAL.AddMessageForJobDocument(objJobDocumentMessage);
		//						SendMailForDocumentSignature(
		//							FirstName
		//							, LastName
		//							, ProjectSession.LoginLink
		//								+ Url.Action("SignatureRequest", "DocumentSignatureRequest")
		//								+ "?groupid=" + QueryString.QueryStringEncode(GroupId.ToString())
		//								+ "&jobdocid=" + QueryString.QueryStringEncode(objJobDocumentMessage.JobDocId.ToString())
		//								+ "&type=" + QueryString.QueryStringEncode(AllType)
		//							, emailTemplateId
		//							, Email);
		//						AllReadySendEmail.Add(Email);
		//					}
		//					Result = true;
		//				}
		//			}
		//				//return Json(new { status = true, Message = "Message is send." });
		//			//else
		//				//return Json(new { status = false, Message = "Message not send." });
		//		}
		//		if(Result)
		//			return Json(new { status = true, Message = "Message is send." });
		//		else
		//			return Json(new { status = false, Message = "Message not send." });
		//	}
		//	catch (Exception ex)
		//	{
		//		return Json(new { status = false, Message = "Message not send." });
		//	}
		//}
		public JsonResult AddMessageWithDocumentMapped(JobDocumentMessage objJobDocumentMessage, string type, int GroupId = 0, int JobId = 0, int JobDocId = 0)
		{
			try
			{
				//int emailTemplateId = 1041;//Local
				int emailTemplateId = 41;//Live
				List<string> lstSentEmail = new List<string>();
				string description = string.Join(",", type.Split(',').Select(x=> FormBot.Helper.SystemEnums.GetDescription((TypeOfSignature)Convert.ToInt32(x))).ToList());
				//TypeOfSignature value = FormBot.Helper.SystemEnums.GetValueFromDescription<TypeOfSignature>(fieldName);
				objJobDocumentMessage.TypeId = type;//value.GetHashCode();
				objJobDocumentMessage.MessageCategory = 4;
				int Id = _documentSignatureRequestBAL.AddMessageForJobDocument(objJobDocumentMessage);
				if (Id > 0)
				{
					List<DocumentSignatureStatusWithEmailResponce> SentEmailDetails = _documentSignatureRequestBAL.GetDocumentSignatureStatusWithEmailByDocumentGroupIdAndJobId(GroupId, JobId, true, JobDocId);
					if (SentEmailDetails != null)
					{
						if (SentEmailDetails.Any())
						{
							var data = SentEmailDetails.FirstOrDefault();
							if (data.InstallerEmail != "NA" && !description.Contains(GetDescription((TypeOfSignature)TypeOfSignature.Installer.GetHashCode())))
							{
								SendMailForDocumentSignature(data.InstallerFirstName, data.InstallerLastName, ProjectSession.LoginLink + Url.Action("SignatureRequest", "DocumentSignatureRequest") + "?groupid=" + QueryString.QueryStringEncode(GroupId.ToString()) + "&jobdocid=" + QueryString.QueryStringEncode(objJobDocumentMessage.JobDocId.ToString()) + "&type=" + QueryString.QueryStringEncode(SetCombineSameSignatureType(data,description, data.InstallerEmail)), emailTemplateId, data.InstallerEmail);
								lstSentEmail.Add(data.InstallerEmail);
							}
							if (data.DesignerEmail != "NA" && !description.Contains(GetDescription((TypeOfSignature)TypeOfSignature.Designer.GetHashCode())))
							{
								if (!lstSentEmail.Any(x => x == data.DesignerEmail))
								{
									SendMailForDocumentSignature(data.DesignerFirstName, data.DesignerLastName, ProjectSession.LoginLink + Url.Action("SignatureRequest", "DocumentSignatureRequest") + "?groupid=" + QueryString.QueryStringEncode(GroupId.ToString()) + "&jobdocid=" + QueryString.QueryStringEncode(objJobDocumentMessage.JobDocId.ToString()) + "&type=" + QueryString.QueryStringEncode(SetCombineSameSignatureType(data, description, data.DesignerEmail)), emailTemplateId, data.DesignerEmail);
									lstSentEmail.Add(data.DesignerEmail);
								}
							}
							if (data.ElectricianEmail != "NA" && !description.Contains(GetDescription((TypeOfSignature)TypeOfSignature.Electrician.GetHashCode())))
							{
								if (!lstSentEmail.Any(x => x == data.ElectricianEmail))
								{
									SendMailForDocumentSignature(data.ElectricianFirstName, data.ElectricianLastName, ProjectSession.LoginLink + Url.Action("SignatureRequest", "DocumentSignatureRequest") + "?groupid=" + QueryString.QueryStringEncode(GroupId.ToString()) + "&jobdocid=" + QueryString.QueryStringEncode(objJobDocumentMessage.JobDocId.ToString()) + "&type=" + QueryString.QueryStringEncode(SetCombineSameSignatureType(data, description, data.ElectricianEmail)), emailTemplateId, data.ElectricianEmail);
									lstSentEmail.Add(data.ElectricianEmail);
								}
							}
							if (data.OwnerEmail != "NA" && !description.Contains(GetDescription((TypeOfSignature)TypeOfSignature.Electrician.GetHashCode())))
							{
								if (!lstSentEmail.Any(x => x == data.OwnerEmail))
								{
									SendMailForDocumentSignature(data.OwnerFirstName, data.OwnerLastName, ProjectSession.LoginLink + Url.Action("SignatureRequest", "DocumentSignatureRequest") + "?groupid=" + QueryString.QueryStringEncode(GroupId.ToString()) + "&jobdocid=" + QueryString.QueryStringEncode(objJobDocumentMessage.JobDocId.ToString()) + "&type=" + QueryString.QueryStringEncode(SetCombineSameSignatureType(data, description, data.OwnerEmail)), emailTemplateId, data.OwnerEmail);
									lstSentEmail.Add(data.OwnerEmail);
								}
							}
							if (data.SolarCompanyEmail != "NA" && !description.Contains(GetDescription((TypeOfSignature)TypeOfSignature.SolarCompnay.GetHashCode())))
							{
								if (!lstSentEmail.Any(x => x == data.SolarCompanyEmail))
								{
									SendMailForDocumentSignature(data.SolarCompanyFirstName, data.SolarCompanyLastName, Url.Action("SignatureRequest", "DocumentSignatureRequest") + "?groupid=" + QueryString.QueryStringEncode(GroupId.ToString()) + "&jobdocid=" + QueryString.QueryStringEncode(objJobDocumentMessage.JobDocId.ToString()) + "&type=" + QueryString.QueryStringEncode(SetCombineSameSignatureType(data, description, data.SolarCompanyEmail)), emailTemplateId, data.SolarCompanyEmail);
								}
							}
							//_documentSignatureRequestBAL.UpdateSentMailStatusForBulkUploadSignature(Convert.ToInt32(sendEmailRequest.BulkUploadDocumentGroupId), sendEmailRequest.lstDocumentWiseSignatureDetail[i].JobId, 3);
						}
					}
					return Json(new { status = true, Message = "Message is send." });
				}
				else
					return Json(new { status = false, Message = "Message not send." });
			}
			catch (Exception ex)
			{
				return Json(new { status = false, Message = "Message not send." });
			}
		}
		public string SetCombineSameSignatureType(DocumentSignatureStatusWithEmailResponce data,string description,string email)
		{
			try
			{
				List<int> lstType = new List<int>();
				if (data.InstallerEmail != "NA" && !description.Contains(GetDescription((TypeOfSignature)TypeOfSignature.Installer.GetHashCode())))
				{
					if (data.InstallerEmail == email)
						lstType.Add(TypeOfSignature.Installer.GetHashCode());
				}
				if (data.DesignerEmail != "NA" && !description.Contains(GetDescription((TypeOfSignature)TypeOfSignature.Designer.GetHashCode())))
				{
					if (data.DesignerEmail == email)
						lstType.Add(TypeOfSignature.Designer.GetHashCode());
				}
				if (data.ElectricianEmail != "NA" && !description.Contains(GetDescription((TypeOfSignature)TypeOfSignature.Electrician.GetHashCode())))
				{
					if (data.ElectricianEmail == email)
						lstType.Add(TypeOfSignature.Electrician.GetHashCode());
				}
				if (data.OwnerEmail != "NA" && !description.Contains(GetDescription((TypeOfSignature)TypeOfSignature.Electrician.GetHashCode())))
				{
					if (data.OwnerEmail == email)
						lstType.Add(TypeOfSignature.Home_Owner.GetHashCode());
				}
				if (data.SolarCompanyEmail != "NA" && !description.Contains(GetDescription((TypeOfSignature)TypeOfSignature.SolarCompnay.GetHashCode())))
				{
					if (data.SolarCompanyEmail == email)
						lstType.Add(TypeOfSignature.SolarCompnay.GetHashCode());
				}
				return string.Join(",", lstType);
			}
			catch (Exception ex)
			{
				return "";
			}
		}
		//public void ShowJobInGroup(int bulkUploadDocumentGroupId, string refNumber)
		//{
		//	IList<DocumentWiseSignatureDetails> lstBulkUploadDocumentGroup = new List<DocumentWiseSignatureDetails>();
		//	GridParam gridParam = Grid.ParseParams(HttpContext.Request);
		//	int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
		//	lstBulkUploadDocumentGroup = _documentSignatureRequestBAL.ShowJobInGroup(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, bulkUploadDocumentGroupId, refNumber);
		//	if (lstBulkUploadDocumentGroup.Count > 0)
		//	{
		//		gridParam.TotalDisplayRecords = lstBulkUploadDocumentGroup.FirstOrDefault().TotalRecords;
		//		gridParam.TotalRecords = lstBulkUploadDocumentGroup.FirstOrDefault().TotalRecords;
		//	}
		//	HttpContext.Response.Write(Grid.PrepareDataSet(lstBulkUploadDocumentGroup, gridParam));
		//}
		public JsonResult GetSignatureStatus(string fileName)
		{
			bool InstallerSignatureStatus = false;
			bool DesignerSignatureStatus = false;
			bool ElectricianSignatureStatus = false;
			bool SolarCompanySignatureStatus = false;
			bool HomeOwnerSignatureStatus = false;
			MemoryStream memStream = new MemoryStream();
			using (FileStream fileStream = System.IO.File.OpenRead(fileName))
			{
				memStream.SetLength(fileStream.Length);
				fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
			}

			string newFile = fileName;
			PdfReader pdfReader = null;
			List<PdfItems> lstPdfItems = new List<PdfItems>();

			//tempcode
			PdfReader reader = new PdfReader(fileName);
			PdfReader.unethicalreading = true;
			//string tempfile = fileName.Replace(Path.GetFileNameWithoutExtension(fileName), Path.GetFileNameWithoutExtension(fileName) + "_temp");
			string tempfile = Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName) + "_temp" + Path.GetExtension(fileName);
			var out1 = System.IO.File.Open(tempfile, FileMode.Create, FileAccess.Write);
			PdfStamper stamp = new PdfStamper(reader, out1);

			try
			{
				pdfReader = new PdfReader(memStream);
				AcroFields af = pdfReader.AcroFields;
				StringBuilder sb = new StringBuilder();
				foreach (var field in af.Fields)
				{
					//tempcode
					iTextSharp.text.Rectangle rect = new iTextSharp.text.Rectangle(100, 100, 350, 450);

					PdfItems k = new PdfItems(lstPdfItems.Count, field.Key, af.GetField(Convert.ToString(field.Key)), af.GetFieldType(Convert.ToString(field.Key)));

					if (k.FieldName.ToString().ToLower().Contains("owner_signature"))
					{
						HomeOwnerSignatureStatus = true;
					}
					else if (k.FieldName.ToString().ToLower().Contains("sca_signature"))
					{
						SolarCompanySignatureStatus = true;
					}
					else if (k.FieldName.ToString().ToLower().Contains("installer_signature"))
					{
						InstallerSignatureStatus = true;
					}
					else if (k.FieldName.ToString().ToLower().Contains("designer_signature"))
					{
						DesignerSignatureStatus = true;
					}
					else if (k.FieldName.ToString().ToLower().Contains("electrician_signature"))
					{
						ElectricianSignatureStatus = true;
					}
				}
				return Json(new { InstallerSignatureStatus = InstallerSignatureStatus, DesignerSignatureStatus = DesignerSignatureStatus, SolarCompanySignatureStatus = SolarCompanySignatureStatus, ElectricianSignatureStatus = ElectricianSignatureStatus, HomeOwnerSignatureStatus = HomeOwnerSignatureStatus }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				Helper.Log.WriteError(ex);
				return Json(false);
			}

			finally
			{
				pdfReader.Close();
				memStream.Close();
				memStream.Dispose();
				stamp.Close();
				stamp.Dispose();
				reader.Close();
				reader.Dispose();
				System.IO.File.Delete(tempfile);
			}
		}
		[HttpPost]
		public JsonResult JobSpecificSendEmailForSignatureRequest(int emailTemplateId = 0, int JobId = 0, int jobDocId = 0, DocumentSignatureStatusWithEmailResponce data = null)
		{
			try
			{
				if (data != null)
				{
					if (data.InstallerEmail != "NA")
					{
						SendMailForDocumentSignature(data.InstallerFirstName, data.InstallerLastName, ProjectSession.LoginLink + Url.Action("SignatureRequest", "DocumentSignatureRequest") + "?groupid=" + QueryString.QueryStringEncode(0.ToString()) + "&jobdocid=" + QueryString.QueryStringEncode(jobDocId.ToString()) + "&type=" + QueryString.QueryStringEncode(TypeOfSignature.Installer.GetHashCode().ToString()), emailTemplateId, data.InstallerEmail);
						//PrepareDocumentSignatureLog(jobDocId, new CaptureUserSign() { Firstname = data.InstallerFirstName, Lastname = data.InstallerLastName }, TypeOfSignature.Installer.GetHashCode(), 2);
					}
					if (data.DesignerEmail != "NA")
					{
						SendMailForDocumentSignature(data.DesignerFirstName, data.DesignerLastName, ProjectSession.LoginLink + Url.Action("SignatureRequest", "DocumentSignatureRequest") + "?groupid=" + QueryString.QueryStringEncode(0.ToString()) + "&jobdocid=" + QueryString.QueryStringEncode(jobDocId.ToString()) + "&type=" + QueryString.QueryStringEncode(TypeOfSignature.Designer.GetHashCode().ToString()), emailTemplateId, data.DesignerEmail);
						//PrepareDocumentSignatureLog(jobDocId, new CaptureUserSign() { Firstname = data.DesignerFirstName, Lastname = data.DesignerLastName }, TypeOfSignature.Designer.GetHashCode(), 2);
					}
					if (data.ElectricianEmail != "NA")
					{
						SendMailForDocumentSignature(data.ElectricianFirstName, data.ElectricianLastName, ProjectSession.LoginLink + Url.Action("SignatureRequest", "DocumentSignatureRequest") + "?groupid=" + QueryString.QueryStringEncode(0.ToString()) + "&jobdocid=" + QueryString.QueryStringEncode(jobDocId.ToString()) + "&type=" + QueryString.QueryStringEncode(TypeOfSignature.Electrician.GetHashCode().ToString()), emailTemplateId, data.ElectricianEmail);
						//PrepareDocumentSignatureLog(jobDocId, new CaptureUserSign() { Firstname = data.ElectricianFirstName, Lastname = data.ElectricianLastName }, TypeOfSignature.Electrician.GetHashCode(), 2);
					}
					if (data.OwnerEmail != "NA")
					{
						SendMailForDocumentSignature(data.OwnerFirstName, data.OwnerLastName, ProjectSession.LoginLink + Url.Action("SignatureRequest", "DocumentSignatureRequest") + "?groupid=" + QueryString.QueryStringEncode(0.ToString()) + "&jobdocid=" + QueryString.QueryStringEncode(jobDocId.ToString()) + "&type=" + QueryString.QueryStringEncode(TypeOfSignature.Home_Owner.GetHashCode().ToString()), emailTemplateId, data.OwnerEmail);
						//PrepareDocumentSignatureLog(jobDocId, new CaptureUserSign() { Firstname = data.OwnerFirstName, Lastname = data.OwnerLastName }, TypeOfSignature.Home_Owner.GetHashCode(), 2);
					}
					if (data.SolarCompanyEmail != "NA")
					{
						SendMailForDocumentSignature(data.SolarCompanyFirstName, data.SolarCompanyLastName, ProjectSession.LoginLink + Url.Action("SignatureRequest", "DocumentSignatureRequest") + "?groupid=" + QueryString.QueryStringEncode(0.ToString()) + "&jobdocid=" + QueryString.QueryStringEncode(jobDocId.ToString()) + "&type=" + QueryString.QueryStringEncode(TypeOfSignature.SolarCompnay.GetHashCode().ToString()), emailTemplateId, data.SolarCompanyEmail);
						//PrepareDocumentSignatureLog(jobDocId, new CaptureUserSign() { Firstname = data.SolarCompanyFirstName, Lastname = data.SolarCompanyLastName }, TypeOfSignature.SolarCompnay.GetHashCode(), 2);
					}
					_documentSignatureRequestBAL.UpdateSentMailStatusForBulkUploadSignature(0, JobId, 3, jobDocId);
				}
				return Json(true);
			}
			catch (Exception ex)
			{
				_logger.LogException("Error : JobSpecificSendEmailForSignatureRequest :: ", ex);
				return Json(false);
			}
		}
		public void SendMailForDocumentSignature(string FirstName, string LastName, string LoginLink, int TemplateId, string ToEmail, string CustomMessage = "", string FilePath = "")
		{
			EmailInfo emailInfo = new EmailInfo();
			emailInfo.TemplateID = TemplateId;
			emailInfo.FirstName = FirstName;
			emailInfo.LastName = LastName;
			emailInfo.Details = CustomMessage;
			emailInfo.LoginLink = LoginLink;//ProjectSession.LoginLink + "DocumentSignatureRequest/_GetUserSignature";
			_emailBAL.ComposeAndSendEmail(emailInfo, ToEmail);
		}
		public JsonResult AddJobToGroup(SendEmailRequest sendEmailRequest)
		{
			try
			{
				string path = _documentSignatureRequestBAL.GetDocumentPathFromGroupId(bulkUploadDocumentGroupId: Convert.ToInt32(sendEmailRequest.BulkUploadDocumentGroupId));
				if (!string.IsNullOrEmpty(path))
				{
					AddJobToGroup(path, sendEmailRequest);
					return Json(new { status = true }, JsonRequestBehavior.AllowGet);
				}
				return Json(new { status = false, error = "Document Path not getting." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { status = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
			}

		}
		[NonAction]
		public void AddJobToGroup(string path, SendEmailRequest sendEmailRequest)
		{
			var obj = GetSignatureStatusOverFilePath(path);
			string Jobid = string.Join(",", sendEmailRequest.lstDocumentWiseSignatureDetail.Select(m => m.JobId));
			string PVDSWHCode = string.Join(",", sendEmailRequest.lstDocumentWiseSignatureDetail.Select(m => m.PVDSWHCode));
			_documentSignatureRequestBAL.AddJobtoGroup(Jobid, PVDSWHCode, sendEmailRequest.BulkUploadDocumentGroupId, obj.InstallerSignatureStatus.Equals("true") ? "False" : "NA", obj.HomeOwnerSignatureStatus.Equals("true") ? "False" : "NA", obj.SolarCompanySignatureStatus.Equals("true") ? "False" : "NA", obj.ElectricianSignatureStatus.Equals("true") ? "False" : "NA", obj.DesignerSignatureStatus.Equals("true") ? "False" : "NA");
		}
		[NonAction]
		public DocumentWiseSignatureDetails GetSignatureStatusOverFilePath(string path)
		{
			string FileTypeName = Path.GetFileName(Directory.GetParent(path).FullName);
			string Fullpath = Path.Combine(ProjectSession.ProofDocuments, path);
			string data = new JavaScriptSerializer().Serialize(GetSignatureStatus(Fullpath).Data);
			var obj = JsonConvert.DeserializeObject<DocumentWiseSignatureDetails>(data);
			return obj;
		}
		[HttpPost]
		public JsonResult GetDocumentSignatureStatusOverJobDocumentid(int JobDocumentId)
		{
			try
			{
				bool InstallerSignatureStatus = false;
				bool DesignerSignatureStatus = false;
				bool ElectricianSignatureStatus = false;
				bool SolarCompanySignatureStatus = false;
				bool HomeOwnerSignatureStatus = false;


				bool IsSignInstaller = false;
				bool IsSignDesigner = false;
				bool IsSignElectrician = false;
				bool IsSignSolarCompany = false;
				bool IsSignHomeOwner = false;
				var lstData = _documentSignatureRequestBAL.GetInsertOrUpdateBulkSendDocumentSignatureRequest(1, JobDocumentId: JobDocumentId);

				if (!lstData.Any())
				{
					DataTable dt = _documentsBAL.GetJobDocumentPath(JobDocumentId);
					int jobId = Convert.ToInt32(dt.Rows[0][1]);
					string jsonData = Convert.ToString(dt.Rows[0]["JsonData"]);
					string path = Convert.ToString(dt.Rows[0]["Path"]);
					//JobController objJobController = new JobController(_user, _job, _userBAL, _documentsBAL, _jobSchedule, _emailService, _jobInvoice, _jobInvoiceDetail, _jobDetails, _jobHistory, _stcInvoiceServiceBAL, _emailBAL, _jobRules, _cerImportBAL, _generateStcReportBAL, _jobSettingBAL, _documentSignatureRequestBAL, _commonRECMethodsBAL, _documentSignatureLogBAL, _spvLog, _spvVerificationBAL);
					List<PdfItems> lstGetPDFItem = CommonHelper.GetPDFItems(ProjectSession.ProofDocuments + (ProjectSession.ProofDocuments.EndsWith(@"\") ? "" : @"\") + path);
					//if (!string.IsNullOrEmpty(jsonData))
					if (lstGetPDFItem.Any())
					{
						//List<PdfItems> pdfitems = JsonConvert.DeserializeObject<List<PdfItems>>(jsonData);
						foreach (var k in lstGetPDFItem)//pdfitems)
						{
							if (k.FieldName.ToString().ToLower().Contains("owner_signature"))
							{
								HomeOwnerSignatureStatus = true;
								if (k.Value.Split(',')[1] != "")
									IsSignHomeOwner = true;
							}
							else if (k.FieldName.ToString().ToLower().Contains("sca_signature"))
							{
								SolarCompanySignatureStatus = true;
								if (k.Value.Split(',')[1] != "")
									IsSignSolarCompany = true;
							}
							else if (k.FieldName.ToString().ToLower().Contains("installer_signature"))
							{
								InstallerSignatureStatus = true;
								if (k.Value.Split(',')[1] != "")
									IsSignInstaller = true;
							}
							else if (k.FieldName.ToString().ToLower().Contains("designer_signature"))
							{
								DesignerSignatureStatus = true;
								if (k.Value.Split(',')[1] != "")
									IsSignDesigner = true;
							}
							else if (k.FieldName.ToString().ToLower().Contains("electrician_signature"))
							{
								ElectricianSignatureStatus = true;
								if (k.Value.Split(',')[1] != "")
									IsSignElectrician = true;
							}
						}

						DocumentWiseSignatureDetails objDocumentWiseSignatureDetails = new DocumentWiseSignatureDetails();
						objDocumentWiseSignatureDetails.InstallerSignatureStatus = InstallerSignatureStatus ? IsSignInstaller.ToString() : "NA";
						objDocumentWiseSignatureDetails.DesignerSignatureStatus = DesignerSignatureStatus ? IsSignDesigner.ToString() : "NA";
						objDocumentWiseSignatureDetails.ElectricianSignatureStatus = ElectricianSignatureStatus ? IsSignElectrician.ToString() : "NA";
						objDocumentWiseSignatureDetails.HomeOwnerSignatureStatus = HomeOwnerSignatureStatus ? IsSignHomeOwner.ToString() : "NA";
						objDocumentWiseSignatureDetails.SolarCompanySignatureStatus = SolarCompanySignatureStatus ? IsSignSolarCompany.ToString() : "NA";
						objDocumentWiseSignatureDetails.IsApplicable = false;
						objDocumentWiseSignatureDetails.PVDSWHCode = "";
						objDocumentWiseSignatureDetails.JobId = jobId;
						objDocumentWiseSignatureDetails.SentEmailStatus = 0;
						lstData = _documentSignatureRequestBAL.GetInsertOrUpdateBulkSendDocumentSignatureRequest(2, DateTime.Now, ProjectSession.LoggedInUserId, DateTime.Now, ProjectSession.LoggedInUserId,
							false, 0, JobDocumentId, objDocumentWiseSignatureDetails);
						var EmailStatus = _documentSignatureRequestBAL.GetEmailStatusByJobDocumentId(JobDocumentId);
						EmailStatus = EmailSetFromDocumentUserEmail(EmailStatus.BulkUploadDocumentSignatureId, EmailStatus);
						if (lstData.Any())
							InsertUpdateInDocumentUserEmail(lstData.FirstOrDefault(), EmailStatus);
						var lstDocumentSignatureOption = _documentSignatureOptionBAL.GetByBulkUploadDocumentSignatureId(EmailStatus.BulkUploadDocumentSignatureId);
						return Json(new { UsersTypeWiseData = lstData, UserEmailStatusData = EmailStatus, DocumentDetails = lstDocumentSignatureOption }, JsonRequestBehavior.AllowGet);
					}
					else
						return Json("", JsonRequestBehavior.AllowGet);
				}
				else
				{
					var EmailStatus = _documentSignatureRequestBAL.GetEmailStatusByJobDocumentId(JobDocumentId);
					EmailStatus = EmailSetFromDocumentUserEmail(EmailStatus.BulkUploadDocumentSignatureId, EmailStatus);
					if (lstData.Any())
						InsertUpdateInDocumentUserEmail(lstData.FirstOrDefault(), EmailStatus);
					var lstDocumentSignatureOption = _documentSignatureOptionBAL.GetByBulkUploadDocumentSignatureId(EmailStatus.BulkUploadDocumentSignatureId);
					return Json(new { UsersTypeWiseData = lstData, UserEmailStatusData = EmailStatus, DocumentDetails = lstDocumentSignatureOption }, JsonRequestBehavior.AllowGet);
				}
			}
			catch (Exception ex)
			{
				_logger.LogException(Severity.Error, "GetDocumentSignatureStatusOverJobDocumentid", ex);
				return Json("", JsonRequestBehavior.AllowGet);
			}
		}
		public EmailStatusResponce EmailSetFromDocumentUserEmail(int BulkUploadDocumentSignatureId, EmailStatusResponce EmailStatus)
		{
			var data = _documentUserEmailServiceBAL.GetByBulkUploadDocumentSignatureId(BulkUploadDocumentSignatureId);
			if(data.Any())
			{
				foreach (var item in data)
				{
					if (item.Type == TypeOfSignature.Installer.GetHashCode() && EmailStatus.InstallerEmailStatus == 1)
						EmailStatus.InstallerEmail = item.EmailId;
					else if (item.Type == TypeOfSignature.Designer.GetHashCode() && EmailStatus.DesignerEmailStatus == 1)
						EmailStatus.DesignerEmail = item.EmailId;
					else if (item.Type == TypeOfSignature.Electrician.GetHashCode() && EmailStatus.ElectricianEmailStatus == 1)
						EmailStatus.ElectricianEmail = item.EmailId;
					else if (item.Type == TypeOfSignature.Home_Owner.GetHashCode() && EmailStatus.HomeOwnerEmailStatus == 1)
						EmailStatus.HomeOwnerEmail = item.EmailId;
					else if (item.Type == TypeOfSignature.SolarCompnay.GetHashCode() && EmailStatus.SolarCompanyEmailStatus == 1)
						EmailStatus.SolarCompanyEmail = item.EmailId;
				}
			}
			return EmailStatus;
		}

		[HttpPost]
		public JsonResult UpdateDocumentEmail(int BulkUploadDocumentSignatureId,string Email,int type,string OldEmail)
		{
			try
			{
				DocumentUserEmail objDocumentUserEmail = new DocumentUserEmail();
				objDocumentUserEmail.BulkUploadDocumentSignatureId = BulkUploadDocumentSignatureId;
				objDocumentUserEmail.EmailId = Email;
				objDocumentUserEmail.Type = type;
				_documentUserEmailServiceBAL.Insert(objDocumentUserEmail);

				var data = _documentSignatureRequestBAL.GetByBulkUploadDocumentSignatureId(BulkUploadDocumentSignatureId);
				DocumentSignatureLog objDocumentSignatureLog = new DocumentSignatureLog();
				objDocumentSignatureLog.CreatedOn = DateTime.Now;
				objDocumentSignatureLog.IpAddress = CommonMethods.GetIp();
				objDocumentSignatureLog.JobDocId = data.JobDocumentId;
				objDocumentSignatureLog.Message = FormBot.Helper.SystemEnums.GetDescription((TypeOfSignature)type).Split('_')[0] + " email update from " + OldEmail + " To " + Email;
				objDocumentSignatureLog.MessageType = 7;
				objDocumentSignatureLog.UserFullName = ProjectSession.LoggedInName;
				if(ProjectSession.UserTypeId == 1)
					objDocumentSignatureLog.UserTypeName =  "FSA";
				if (ProjectSession.UserTypeId == 2)
					objDocumentSignatureLog.UserTypeName = "RA";
				if (ProjectSession.UserTypeId == 3)
					objDocumentSignatureLog.UserTypeName = "FCO";
				if (ProjectSession.UserTypeId == 4)
					objDocumentSignatureLog.UserTypeName = "SCA";

				_documentSignatureLogBAL.Insert(objDocumentSignatureLog);
				return Json(1);
			}
			catch (Exception ex)
			{
				_logger.LogException(Severity.Error.ToString(), ex);
				return Json(0);
			}
		}
		[HttpPost]
		public JsonResult UpdateDocumentUserSendCopyDetails(int BulkUploadDocumentSignatureId, int Type,bool Value)
		{
			try
			{
				DocumentSignatureOption objDocumentSignatureOption = new DocumentSignatureOption();
				objDocumentSignatureOption.ModifiedDate = DateTime.Now;
				objDocumentSignatureOption.CreatedDate = DateTime.Now;
				objDocumentSignatureOption.ModifiedBy = ProjectSession.LoggedInUserId;
				objDocumentSignatureOption.SendCopy = Value;
				objDocumentSignatureOption.BulkUploadDocumentSignatureId = BulkUploadDocumentSignatureId;
				objDocumentSignatureOption.Type = Type;
				_documentSignatureOptionBAL.InsertUpdateDelete(objDocumentSignatureOption, 2);
				return Json(1);
			}
			catch (Exception ex)
			{
				_logger.LogException(Severity.Error.ToString(), ex);
				return Json(0);
			}
		}
		[NonAction]
		public void InsertUpdateInDocumentUserEmail(DocumentWiseSignatureDetails objDocumentWiseSignatureDetails, EmailStatusResponce EmailStatus)
		{
			List<DocumentUserEmail> lstDocumentUserEmail = new List<DocumentUserEmail>();
			
			if (objDocumentWiseSignatureDetails.InstallerSignatureStatus != "NA" && !string.IsNullOrEmpty(EmailStatus.InstallerEmail))
			{
				DocumentUserEmail objDocumentUserEmail = new DocumentUserEmail();
				objDocumentUserEmail.BulkUploadDocumentSignatureId = objDocumentWiseSignatureDetails.BulkUploadDocumentSignatureId;
				objDocumentUserEmail.EmailId = EmailStatus.InstallerEmail;
				objDocumentUserEmail.Type = TypeOfSignature.Installer.GetHashCode();
				lstDocumentUserEmail.Add(objDocumentUserEmail);
			}
			if (objDocumentWiseSignatureDetails.DesignerSignatureStatus != "NA" && !string.IsNullOrEmpty(EmailStatus.DesignerEmail))
			{
				DocumentUserEmail objDocumentUserEmail = new DocumentUserEmail();
				objDocumentUserEmail.BulkUploadDocumentSignatureId = objDocumentWiseSignatureDetails.BulkUploadDocumentSignatureId;
				objDocumentUserEmail.EmailId = EmailStatus.DesignerEmail;
				objDocumentUserEmail.Type = TypeOfSignature.Designer.GetHashCode();
				lstDocumentUserEmail.Add(objDocumentUserEmail);
			}
			if (objDocumentWiseSignatureDetails.ElectricianSignatureStatus != "NA" && !string.IsNullOrEmpty(EmailStatus.ElectricianEmail))
			{
				DocumentUserEmail objDocumentUserEmail = new DocumentUserEmail();
				objDocumentUserEmail.BulkUploadDocumentSignatureId = objDocumentWiseSignatureDetails.BulkUploadDocumentSignatureId;
				objDocumentUserEmail.EmailId = EmailStatus.ElectricianEmail;
				objDocumentUserEmail.Type = TypeOfSignature.Electrician.GetHashCode();
				lstDocumentUserEmail.Add(objDocumentUserEmail);
			}
			if (objDocumentWiseSignatureDetails.HomeOwnerSignatureStatus != "NA" && !string.IsNullOrEmpty(EmailStatus.HomeOwnerEmail))
			{
				DocumentUserEmail objDocumentUserEmail = new DocumentUserEmail();
				objDocumentUserEmail.BulkUploadDocumentSignatureId = objDocumentWiseSignatureDetails.BulkUploadDocumentSignatureId;
				objDocumentUserEmail.EmailId = EmailStatus.HomeOwnerEmail;
				objDocumentUserEmail.Type = TypeOfSignature.Home_Owner.GetHashCode();
				lstDocumentUserEmail.Add(objDocumentUserEmail);
			}
			if (objDocumentWiseSignatureDetails.SolarCompanySignatureStatus != "NA" && !string.IsNullOrEmpty(EmailStatus.SolarCompanyEmail))
			{
				DocumentUserEmail objDocumentUserEmail = new DocumentUserEmail();
				objDocumentUserEmail.BulkUploadDocumentSignatureId = objDocumentWiseSignatureDetails.BulkUploadDocumentSignatureId;
				objDocumentUserEmail.EmailId = EmailStatus.SolarCompanyEmail;
				objDocumentUserEmail.Type = TypeOfSignature.SolarCompnay.GetHashCode();
				lstDocumentUserEmail.Add(objDocumentUserEmail);
			}

			foreach (var item in lstDocumentUserEmail)
			{
				_documentUserEmailServiceBAL.Insert(item);
			}
		}
		#region Bulk Signature for Group specific
		public ActionResult DeleteGroupName(string bulkDocumentGroupId)
		{
			if (ProjectSession.LoggedInUserId == 0)
				return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

			try
			{
				List<string> lstbulkDocumentGroupId = new List<string>();
				if (!string.IsNullOrEmpty(bulkDocumentGroupId))
					lstbulkDocumentGroupId = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(bulkDocumentGroupId);

				List<int> lstNewbulkDocumentGroupId = new List<int>();

				for (int i = 0; i < lstbulkDocumentGroupId.Count; i++)
				{
					int templateId = 0;
					if (!string.IsNullOrEmpty(lstbulkDocumentGroupId[i]))
					{
						int.TryParse(QueryString.GetValueFromQueryString(lstbulkDocumentGroupId[i], "id"), out templateId);
					}
					lstNewbulkDocumentGroupId.Add(templateId);
				}

				bulkDocumentGroupId = string.Join(",", lstNewbulkDocumentGroupId.ToArray());

				_documentSignatureRequestBAL.DeleteGroupName(bulkDocumentGroupId);
				return Json(new { status = true, id = 1 }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { status = false, error = ex.Message, id = 0 }, JsonRequestBehavior.AllowGet);
			}
		}
		public JsonResult GetBulkDocumentGroupNameBasedOnId(string bulkDocumentGroupId)
		{
			int DocumentGroupId;
			if (ProjectSession.LoggedInUserId == 0)
				return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

			try
			{
				int.TryParse(QueryString.GetValueFromQueryString(bulkDocumentGroupId, "id"), out DocumentGroupId);
				BulkUploadDocumentGroup bulkUploadDocumentGroup = _documentSignatureRequestBAL.GetBulkDocumentGroupName(DocumentGroupId);
				return Json(new { status = true, data = bulkUploadDocumentGroup }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { status = false, error = ex.Message, id = 0 }, JsonRequestBehavior.AllowGet);
			}
		}
		public JsonResult Binddropdown_GetBulkUploadDocumentGroupNameList()
		{
			List<SelectListItem> lstSelectListItem = _documentSignatureRequestBAL.GetBulkUploadDocumentGroupNameList().Select(a => new SelectListItem() { Text = a.GroupName, Value = a.BulkUploadDocumentGroupId.ToString() }).ToList();
			return Json(lstSelectListItem, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetEmailTemplateList()
		{
			List<SelectListItem> lstSelectListItem = _documentSignatureRequestBAL.GetEmailTemplateList().Select(a => new SelectListItem() { Text = a.TemplateName, Value = a.TemplateID.ToString() }).ToList();
			return Json(lstSelectListItem, JsonRequestBehavior.AllowGet);
		}
		[HttpPost]
		public JsonResult DeleteJobInGroup(int bulkDocumentGroupId, string JobId)
		{
			try
			{
				_documentSignatureRequestBAL.DeleteJobInGroup(bulkDocumentGroupId, JobId);
				return Json(new { status = true }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { status = false }, JsonRequestBehavior.AllowGet);
			}
		}
		public ActionResult EditGroup(string groupId)
		{
			int DocumentGroupId = 0;
			JobDocumentSignatureModels objJobDocumentSignatureModels = new JobDocumentSignatureModels();
			if (!string.IsNullOrEmpty(groupId))
			{
				int.TryParse(QueryString.GetValueFromQueryString(groupId, "id"), out DocumentGroupId);
			}
			objJobDocumentSignatureModels.EncryptedGroupId = groupId;
			BulkUploadDocumentGroup bulkUploadDocumentGroup = _documentSignatureRequestBAL.GetBulkDocumentGroupName(DocumentGroupId);
			objJobDocumentSignatureModels.GroupName = bulkUploadDocumentGroup.GroupName;
			objJobDocumentSignatureModels.GroupId = DocumentGroupId;

			return View(objJobDocumentSignatureModels);
		}
		//public void GetBulkJobListByGroupId(string EncryptedGroupId, string referenceNumber = "")
		//{
		//	int DocumentGroupId = 0;
		//	if (!string.IsNullOrEmpty(EncryptedGroupId))
		//	{
		//		int.TryParse(QueryString.GetValueFromQueryString(EncryptedGroupId, "id"), out DocumentGroupId);
		//	}
		//	IList<DocumentWiseSignatureDetails> lstBulkUploadDocumentGroup = new List<DocumentWiseSignatureDetails>();
		//	GridParam gridParam = Grid.ParseParams(HttpContext.Request);
		//	int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
		//	lstBulkUploadDocumentGroup = _documentSignatureRequestBAL.ShowJobInGroup(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, DocumentGroupId, referenceNumber);
		//	if (lstBulkUploadDocumentGroup.Count > 0)
		//	{
		//		gridParam.TotalDisplayRecords = lstBulkUploadDocumentGroup.FirstOrDefault().TotalRecords;
		//		gridParam.TotalRecords = lstBulkUploadDocumentGroup.FirstOrDefault().TotalRecords;

		//	}
		//	HttpContext.Response.Write(Grid.PrepareDataSet(lstBulkUploadDocumentGroup, gridParam));
		//}
		#endregion
		[HttpPost]
		//public JsonResult SendEmailForSignatureRequest(SendEmailRequest sendEmailRequest = null, int emailTemplateId = 0, int jobDocId = 0, List<int> typesList = null)
		public JsonResult SendEmailForSignatureRequest(List<DocumentSignatureOptionCustom> lstDocumentSignatureOption, string CustomMessage)
		{
			try
			{
				int emailTemplateId;
				//HttpApplication http = new HttpApplication();
				//if (http.Request.IsLocal)
				//	emailTemplateId = 1040;
				//else
				//	emailTemplateId = 40;
				emailTemplateId = 40;


				List<int> SkipUserForMail = new List<int>();
				if (lstDocumentSignatureOption.Any())
				{
					foreach (var item in lstDocumentSignatureOption)
					{
						item.CreatedBy = ProjectSession.LoggedInUserId;
						item.CreatedDate = DateTime.Now;
						item.ModifiedBy = ProjectSession.LoggedInUserId;
						item.ModifiedDate = DateTime.Now;
						item.IsDeleted = false;
						_documentSignatureOptionBAL.InsertUpdateDelete(item, 1, CustomMessage);
						if ((item.Email != "NA" && item.Type == TypeOfSignature.Installer.GetHashCode())
						|| (item.Email != "NA" && item.Type == TypeOfSignature.Designer.GetHashCode())
						|| (item.Email != "NA" && item.Type == TypeOfSignature.Electrician.GetHashCode())
						|| (item.Email != "NA" && item.Type == TypeOfSignature.Home_Owner.GetHashCode())
						|| (item.Email != "NA" && item.Type == TypeOfSignature.SolarCompnay.GetHashCode()))
						{
							if (!SkipUserForMail.Any(x => x == item.Type))
							{
								string Types = "";
								if (lstDocumentSignatureOption.Where(x => x.Email == item.Email).ToList().Count > 1)
								{
									Types = string.Join(",", lstDocumentSignatureOption.Where(x => x.Email == item.Email).ToList().Select(x =>  x.Type.ToString()));
									foreach(var itemDuplicate in lstDocumentSignatureOption.Where(x => x.Email == item.Email).ToList())
									{
										SkipUserForMail.Add(itemDuplicate.Type);
									}
								}
								SendMailFromJobDetailsPage(item, emailTemplateId, CustomMessage, string.IsNullOrEmpty(Types) ? item.Type.ToString() : Types);
								//if (lstDocumentSignatureOption.Any(x => x.SameAsInstaller) && item.Type == TypeOfSignature.Installer.GetHashCode())
								//{
								//	SkipUserForMail = lstDocumentSignatureOption.Where(x => x.SameAsInstaller && x.Type != TypeOfSignature.Installer.GetHashCode()).Select(x => x.Type).ToList();
								//	SendMailFromJobDetailsPage(item, emailTemplateId, CustomMessage);
								//}
								//else
								//	SendMailFromJobDetailsPage(item, emailTemplateId, CustomMessage);
							}
							_documentSignatureRequestBAL.UpdateSentMailStatusForBulkUploadSignature(0, item.JobId, 3, item.JobDocId);
						}
					}
					CommonMethods.DocumentSendRequestLog(lstDocumentSignatureOption, _documentSignatureLogBAL);
				}
				return Json(true);
			}
			catch (Exception ex)
			{
				_logger.LogException(Severity.Error.ToString(), ex);
				return Json(false);
			}
			#region Old Logic
			//try
			//{
			//	if (jobDocId > 0)
			//	{
			//		var lstSignatureStatus = _documentSignatureRequestBAL.GetInsertOrUpdateBulkSendDocumentSignatureRequest(1, JobDocumentId: jobDocId);
			//		if (lstSignatureStatus.Any())
			//		{
			//			string path = _documentSignatureRequestBAL.GetDocumentPathFromGroupId(JobDocumentId: jobDocId);
			//			if (!string.IsNullOrEmpty(path))
			//			{
			//				DocumentsView objDocumentsView = _job.GetJobDocumentByJobDocumentId(jobDocId);
			//				if (objDocumentsView != null)
			//				{
			//					int JobId = objDocumentsView.JobId;
			//					path = Path.Combine(ProjectSession.ProofDocuments, path);
			//					string FileTypeName = Path.GetFileName(Directory.GetParent(path).FullName);
			//					string fileName = Path.GetFileNameWithoutExtension(path);
			//					List<DocumentSignatureStatusWithEmailResponce> SentEmailDetails = _documentSignatureRequestBAL.GetDocumentSignatureStatusWithEmailByDocumentGroupIdAndJobId(0, 0, false, jobDocId);
			//					if (SentEmailDetails != null)
			//					{
			//						if (SentEmailDetails.Any())
			//						{
			//							var data = SentEmailDetails.FirstOrDefault();
			//							if (data.InstallerEmail != "NA" && typesList.Any(x => x == TypeOfSignature.Installer.GetHashCode()))
			//								SendMailForDocumentSignature(data.InstallerFirstName, data.InstallerLastName, ProjectSession.LoginLink + Url.Action("SignatureRequest", "DocumentSignatureRequest") + "?groupid=" + QueryString.QueryStringEncode(0.ToString()) + "&jobdocid=" + QueryString.QueryStringEncode(jobDocId.ToString()) + "&type=" + QueryString.QueryStringEncode(TypeOfSignature.Installer.GetHashCode().ToString()), emailTemplateId, data.InstallerEmail);
			//							if (data.DesignerEmail != "NA" && typesList.Any(x => x == TypeOfSignature.Designer.GetHashCode()))
			//								SendMailForDocumentSignature(data.DesignerFirstName, data.DesignerLastName, ProjectSession.LoginLink + Url.Action("SignatureRequest", "DocumentSignatureRequest") + "?groupid=" + QueryString.QueryStringEncode(0.ToString()) + "&jobdocid=" + QueryString.QueryStringEncode(jobDocId.ToString()) + "&type=" + QueryString.QueryStringEncode(TypeOfSignature.Designer.GetHashCode().ToString()), emailTemplateId, data.DesignerEmail);
			//							if (data.ElectricianEmail != "NA" && typesList.Any(x => x == TypeOfSignature.Electrician.GetHashCode()))
			//								SendMailForDocumentSignature(data.ElectricianFirstName, data.ElectricianLastName, ProjectSession.LoginLink + Url.Action("SignatureRequest", "DocumentSignatureRequest") + "?groupid=" + QueryString.QueryStringEncode(0.ToString()) + "&jobdocid=" + QueryString.QueryStringEncode(jobDocId.ToString()) + "&type=" + QueryString.QueryStringEncode(TypeOfSignature.Electrician.GetHashCode().ToString()), emailTemplateId, data.ElectricianEmail);
			//							if (data.OwnerEmail != "NA" && typesList.Any(x => x == TypeOfSignature.Home_Owner.GetHashCode()))
			//								SendMailForDocumentSignature(data.OwnerFirstName, data.OwnerLastName, ProjectSession.LoginLink + Url.Action("SignatureRequest", "DocumentSignatureRequest") + "?groupid=" + QueryString.QueryStringEncode(0.ToString()) + "&jobdocid=" + QueryString.QueryStringEncode(jobDocId.ToString()) + "&type=" + QueryString.QueryStringEncode(TypeOfSignature.Home_Owner.GetHashCode().ToString()), emailTemplateId, data.OwnerEmail);
			//							if (data.SolarCompanyEmail != "NA" && typesList.Any(x => x == TypeOfSignature.SolarCompnay.GetHashCode()))
			//								SendMailForDocumentSignature(data.SolarCompanyFirstName, data.SolarCompanyLastName, ProjectSession.LoginLink + Url.Action("SignatureRequest", "DocumentSignatureRequest") + "?groupid=" + QueryString.QueryStringEncode(0.ToString()) + "&jobdocid=" + QueryString.QueryStringEncode(jobDocId.ToString()) + "&type=" + QueryString.QueryStringEncode(TypeOfSignature.SolarCompnay.GetHashCode().ToString()), emailTemplateId, data.SolarCompanyEmail);
			//							_documentSignatureRequestBAL.UpdateSentMailStatusForBulkUploadSignature(0, JobId, 3, jobDocId);
			//						}
			//					}
			//				}
			//			}
			//			return Json(true);
			//		}
			//		else
			//			return Json(false);
			//	}
			//	else
			//	{


			//		string jobId = string.Join(",", sendEmailRequest.lstDocumentWiseSignatureDetail.Select(m => m.JobId));
			//		for (int i = 0; i < sendEmailRequest.lstDocumentWiseSignatureDetail.Count; i++)
			//		{
			//			string path = _documentSignatureRequestBAL.GetDocumentPathFromGroupId(bulkUploadDocumentGroupId: Convert.ToInt32(sendEmailRequest.BulkUploadDocumentGroupId));
			//			if (!string.IsNullOrEmpty(path))
			//			{

			//				string FileTypeName = Path.GetFileName(Directory.GetParent(path).FullName);
			//				string fileName = Path.GetFileNameWithoutExtension(path);
			//				string mainDirToSavePath = Path.Combine(ProjectConfiguration.JobDocumentsToSaveFullPath, sendEmailRequest.lstDocumentWiseSignatureDetail[i].JobId.ToString(), FileTypeName);
			//				if (!Directory.Exists(mainDirToSavePath))
			//				{
			//					Directory.CreateDirectory(mainDirToSavePath);
			//				}
			//				string renameFileName = string.Empty;
			//				int j = 0;
			//				while (true)
			//				{
			//					if (j == 0)
			//						renameFileName = mainDirToSavePath + "/" + fileName + ".pdf";
			//					else
			//						renameFileName = mainDirToSavePath + "/" + fileName + "(" + j + ")" + ".pdf";

			//					if (System.IO.File.Exists(renameFileName))
			//						j++;
			//					else
			//						break;
			//				}
			//				string sourcePath = Path.Combine(ProjectSession.ProofDocuments, path);
			//				if (System.IO.File.Exists(sourcePath))
			//				{
			//					System.IO.File.Copy(sourcePath, renameFileName, true);
			//				}
			//				JobController objJobController = new JobController(_user, _job, _userBAL, _documentsBAL, _jobSchedule, _emailService, _jobInvoice, _jobInvoiceDetail, _jobDetails, _jobHistory, _stcInvoiceServiceBAL, _emailBAL, _jobRules, _cerImportBAL, _generateStcReportBAL, _jobSettingBAL, _documentSignatureRequestBAL, _commonRECMethodsBAL);
			//				objJobController.PreFillItems(renameFileName, sendEmailRequest.lstDocumentWiseSignatureDetail[i].JobId, string.Empty, false);
			//				List<PdfItems> lstGetPDFItem = objJobController.GetPDFItems(renameFileName);
			//				string jsonPDFData = Newtonsoft.Json.JsonConvert.SerializeObject(lstGetPDFItem);
			//				DocumentsView documentsView = new DocumentsView();
			//				documentsView.CreatedBy = ProjectSession.LoggedInUserId;
			//				documentsView.CreatedDate = DateTime.Now;
			//				documentsView.DocumentId = 0;
			//				documentsView.IsUpload = true;
			//				documentsView.JobId = sendEmailRequest.lstDocumentWiseSignatureDetail[i].JobId;
			//				documentsView.Path = Path.Combine("JobDocuments", sendEmailRequest.lstDocumentWiseSignatureDetail[i].JobId.ToString(), FileTypeName, Path.GetFileName(renameFileName));
			//				documentsView.JsonData = jsonPDFData;
			//				documentsView.Type = FileTypeName;
			//				int JobDocumentId = _job.CreateJobDocuments(documentsView, false);
			//				List<DocumentSignatureStatusWithEmailResponce> SentEmailDetails = _documentSignatureRequestBAL.GetDocumentSignatureStatusWithEmailByDocumentGroupIdAndJobId(Convert.ToInt32(sendEmailRequest.BulkUploadDocumentGroupId), sendEmailRequest.lstDocumentWiseSignatureDetail[i].JobId);
			//				if (SentEmailDetails != null)
			//				{
			//					if (SentEmailDetails.Any())
			//					{
			//						var data = SentEmailDetails.FirstOrDefault();
			//						if (data.InstallerEmail != "NA")
			//							SendMailForDocumentSignature(data.InstallerFirstName, data.InstallerLastName, ProjectSession.LoginLink + Url.Action("SignatureRequest", "DocumentSignatureRequest") + "?groupid=" + QueryString.QueryStringEncode(sendEmailRequest.BulkUploadDocumentGroupId.ToString()) + "&jobdocid=" + QueryString.QueryStringEncode(JobDocumentId.ToString()) + "&type=" + QueryString.QueryStringEncode(TypeOfSignature.Installer.GetHashCode().ToString()), emailTemplateId, data.InstallerEmail);
			//						if (data.DesignerEmail != "NA")
			//							SendMailForDocumentSignature(data.DesignerFirstName, data.DesignerLastName, ProjectSession.LoginLink + Url.Action("SignatureRequest", "DocumentSignatureRequest") + "?groupid=" + QueryString.QueryStringEncode(sendEmailRequest.BulkUploadDocumentGroupId.ToString()) + "&jobdocid=" + QueryString.QueryStringEncode(JobDocumentId.ToString()) + "&type=" + QueryString.QueryStringEncode(TypeOfSignature.Designer.GetHashCode().ToString()), emailTemplateId, data.DesignerEmail);
			//						if (data.ElectricianEmail != "NA")
			//							SendMailForDocumentSignature(data.ElectricianFirstName, data.ElectricianLastName, ProjectSession.LoginLink + Url.Action("SignatureRequest", "DocumentSignatureRequest") + "?groupid=" + QueryString.QueryStringEncode(sendEmailRequest.BulkUploadDocumentGroupId.ToString()) + "&jobdocid=" + QueryString.QueryStringEncode(JobDocumentId.ToString()) + "&type=" + QueryString.QueryStringEncode(TypeOfSignature.Electrician.GetHashCode().ToString()), emailTemplateId, data.ElectricianEmail);
			//						if (data.OwnerEmail != "NA")
			//							SendMailForDocumentSignature(data.OwnerFirstName, data.OwnerLastName, ProjectSession.LoginLink + Url.Action("SignatureRequest", "DocumentSignatureRequest") + "?groupid=" + QueryString.QueryStringEncode(sendEmailRequest.BulkUploadDocumentGroupId.ToString()) + "&jobdocid=" + QueryString.QueryStringEncode(JobDocumentId.ToString()) + "&type=" + QueryString.QueryStringEncode(TypeOfSignature.Home_Owner.GetHashCode().ToString()), emailTemplateId, data.OwnerEmail);
			//						if (data.SolarCompanyEmail != "NA")
			//							SendMailForDocumentSignature(data.SolarCompanyFirstName, data.SolarCompanyLastName, ProjectSession.LoginLink + Url.Action("SignatureRequest", "DocumentSignatureRequest") + "?groupid=" + QueryString.QueryStringEncode(sendEmailRequest.BulkUploadDocumentGroupId.ToString()) + "&jobdocid=" + QueryString.QueryStringEncode(JobDocumentId.ToString()) + "&type=" + QueryString.QueryStringEncode(TypeOfSignature.SolarCompnay.GetHashCode().ToString()), emailTemplateId, data.SolarCompanyEmail);
			//						_documentSignatureRequestBAL.UpdateSentMailStatusForBulkUploadSignature(Convert.ToInt32(sendEmailRequest.BulkUploadDocumentGroupId), sendEmailRequest.lstDocumentWiseSignatureDetail[i].JobId, 3, JobDocumentId);
			//					}
			//				}
			//			}
			//		}
			//		return Json(true);
			//	}
			//}
			//catch (Exception ex)
			//{
			//	return Json(false);
			//}
			#endregion
		}
		[NonAction]
		public void SendMailFromJobDetailsPage(DocumentSignatureOptionCustom objDocumentSignatureOptionCustom, int emailTemplateId, string CustomMessage = "", string Types = "")
		{
			SendMailForDocumentSignature(
				objDocumentSignatureOptionCustom.FirstName
				, objDocumentSignatureOptionCustom.LastName
				, ProjectSession.LoginLink
					+ Url.Action("SignatureRequest", "DocumentSignatureRequest")
					+ "?groupid="
					+ QueryString.QueryStringEncode(0.ToString())
					+ "&jobdocid="
					+ QueryString.QueryStringEncode(objDocumentSignatureOptionCustom.JobDocId.ToString())
					+ "&type="
					+ QueryString.QueryStringEncode(Types)
				, emailTemplateId
				, objDocumentSignatureOptionCustom.Email, CustomMessage);
		}
		#endregion
	}
}