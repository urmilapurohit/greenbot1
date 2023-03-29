using FormBot.BAL.Service.DocumentSignatureRequest;
using FormBot.Entity.DocumentSignatureRequest;
using FormBot.Helper;
using FormBot.Helper.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace FormBot.Main.Models
{
	public class CommonMethods
	{
		public static string GetIp()
		{
			string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
			if (string.IsNullOrEmpty(ip))
			{
				ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
			}
			return ip;
		}
		public static void DocumentCreatedLog(int jobDocId, IDocumentSignatureLogBAL _documentSignatureLogBAL)
		{
			DocumentSignatureLog objDocumentLog = new DocumentSignatureLog()
			{
				CreatedOn = DateTime.Now,
				IpAddress = GetIp(),
				JobDocId = jobDocId,
				MessageType = SystemEnums.DocumentChangeLog.Created.GetHashCode(),
				UserFullName = ProjectSession.LoggedInName,
				UserTypeName = SystemEnums.GetDescription((SystemEnums.UserTypeForDocument)ProjectSession.UserTypeId),
				Message = ""
			};
			_documentSignatureLogBAL.Insert(objDocumentLog);
		}
		public static void DocumentSendRequestLog(List<DocumentSignatureOptionCustom> lstDocumentSignatureOption,IDocumentSignatureLogBAL _documentSignatureLogBAL)
		{
			string message = "";
            string Temphistorydescription = string.Empty;
            string historydescription = string.Empty;
            foreach (var item in lstDocumentSignatureOption)
			{
				message += "<p class='user-details'><<span>" + item.FirstName + " " + item.LastName + "</span> - <span>" + SystemEnums.GetDescription((SystemEnums.TypeOfSignature)item.Type).ToString().Split('_')[0] + "</span>, <span>" + item.Email + "</span>" + (item.SameAsInstaller ? " <span>(same as installer)</span>" : "") + "></p>";
                Temphistorydescription += SystemEnums.GetDescription((SystemEnums.TypeOfSignature)item.Type).ToString().Split('_')[0] + " <b class=\"blue-title\">" + item.FirstName + " " + item.LastName + " - " + item.Email+"</b>, ";


            }
			if (!string.IsNullOrEmpty(message))
			{
				DocumentSignatureLog objDocumentLog = new DocumentSignatureLog()
				{
					CreatedOn = DateTime.Now,
					IpAddress = GetIp(),
					JobDocId = lstDocumentSignatureOption.FirstOrDefault().JobDocId,
					MessageType = SystemEnums.DocumentChangeLog.RequestSent.GetHashCode(),
					UserFullName = ProjectSession.LoggedInName,
					UserTypeName = SystemEnums.GetDescription((SystemEnums.UserTypeForDocument)ProjectSession.UserTypeId),
					Message = message
				};
                
				_documentSignatureLogBAL.Insert(objDocumentLog);
                #region Add log in xml 
                DataSet docdata = _documentSignatureLogBAL.GetDocumentPathAndJobIdByJobDocumentId(lstDocumentSignatureOption.FirstOrDefault().JobDocId);
                
                if(docdata != null && docdata.Tables.Count > 0)
                {
                    string docPath = docdata.Tables[0].Rows[0]["Path"].ToString();
                    string docName = !string.IsNullOrEmpty(docPath) ? Path.GetFileName(docPath) : "";
                    string jobId = docdata.Tables[0].Rows[0]["JobId"].ToString();

                    //string JobHistoryMessage = "sent a document sign request -<b class=\"blue-title\"> (" + jobId+ ") JobRefNo </b>";
                    historydescription = Temphistorydescription.TrimEnd();

                    string JobHistoryMessage = "sent a signature request for document: <b class=\"blue-title\">" + docName + "</b> to: <b style=\"color:black\">" + historydescription.TrimEnd(',')+"</b>";
                    Common.SaveJobHistorytoXML(Convert.ToInt32(jobId), JobHistoryMessage, "Documents", "SentSignRequestForDoc", ProjectSession.LoggedInName, false, null);
                }
                #endregion
            }
        }
		public static void DocumentOpenFromEmailLog(int jobDocId, JobDocumentSignatureDetails objJobDocumentSignatureDetails, IDocumentSignatureLogBAL _documentSignatureLogBAL)
		{
			string message = "<p> <span class='user-details'>" + objJobDocumentSignatureDetails.Firstname + " " + objJobDocumentSignatureDetails.Lastname + "</span> <" + objJobDocumentSignatureDetails.Email + "> has viewed the document.</p>";
			DocumentSignatureLog objDocumentLog = new DocumentSignatureLog()
			{
				CreatedOn = DateTime.Now,
				IpAddress = GetIp(),
				JobDocId = jobDocId,
				MessageType = SystemEnums.DocumentChangeLog.Open.GetHashCode(),
				UserFullName = objJobDocumentSignatureDetails.Firstname + " " + objJobDocumentSignatureDetails.Lastname,
				UserTypeName = objJobDocumentSignatureDetails.fieldName.ToString().Split('_')[0],
				Message = message
			};
			_documentSignatureLogBAL.Insert(objDocumentLog);
		}
		public static void DocumentSignLog(JobDocumentSignatureDetails objJobDocumentSignatureDetails, IDocumentSignatureLogBAL _documentSignatureLogBAL,string FieldName)
		{
			string message = "<p> <span class='user-details'>" + objJobDocumentSignatureDetails.Firstname + " " + objJobDocumentSignatureDetails.Lastname + "</span> <" + objJobDocumentSignatureDetails.Email + "> has signed the document.</p>";
			DocumentSignatureLog objDocumentLog = new DocumentSignatureLog()
			{
				CreatedOn = DateTime.Now,
				IpAddress = GetIp(),
				JobDocId = objJobDocumentSignatureDetails.jobDocId,
				MessageType = SystemEnums.DocumentChangeLog.Signed.GetHashCode(),
				UserFullName = objJobDocumentSignatureDetails.Firstname + " " + objJobDocumentSignatureDetails.Lastname,
				UserTypeName = FieldName, //objJobDocumentSignatureDetails.fieldName.ToString().Split('_')[0],
				Message = message
			};
			_documentSignatureLogBAL.Insert(objDocumentLog);
            #region Add log in xml 
            DataSet docdata = _documentSignatureLogBAL.GetDocumentPathAndJobIdByJobDocumentId(objJobDocumentSignatureDetails.jobDocId);

            if (docdata != null && docdata.Tables.Count > 0)
            {
                string docPath = docdata.Tables[0].Rows[0]["Path"].ToString();
                string docName = !string.IsNullOrEmpty(docPath) ? Path.GetFileName(docPath) : "";
                string jobId = docdata.Tables[0].Rows[0]["JobId"].ToString();

                //string JobHistoryMessage = "<b class=\"blue-title\">" + docName + "</b>"+ " signature requests have been completed -<b class=\"blue-title\"> (" + jobId + ") JobRefNo </b>";
                string isFromImage = (objJobDocumentSignatureDetails.IsImage) ? " through upload image" : "";
                string JobHistoryMessage = "<b class=\"blue-title\">"+objJobDocumentSignatureDetails.Firstname + objJobDocumentSignatureDetails.Lastname + " - " + objJobDocumentSignatureDetails.Email+ "</b> has completed and signed a signature request for document " + "<b class=\"blue-title\">" + docName + "</b>"+isFromImage  ;
                Common.SaveJobHistorytoXML(Convert.ToInt32(jobId), JobHistoryMessage, "Documents", "CompletedSignRequestDoc", ProjectSession.LoggedInName, false, null);
            }
            #endregion
        }
        public static void DocumentCompletedLog(int JobDocId,IDocumentSignatureLogBAL _documentSignatureLogBAL)
		{
			string message = "<p> Document has been signed and completed by all parties.</p>";
			DocumentSignatureLog objDocumentLog = new DocumentSignatureLog()
			{
				CreatedOn = DateTime.Now,
				IpAddress = GetIp(),
				JobDocId = JobDocId,
				MessageType = SystemEnums.DocumentChangeLog.Completed.GetHashCode(),
				Message = message,
				UserFullName = "",
				UserTypeName = ""
			};
			_documentSignatureLogBAL.Insert(objDocumentLog);
            #region Add log in xml 
            DataSet docdata = _documentSignatureLogBAL.GetDocumentPathAndJobIdByJobDocumentId(JobDocId);

            if (docdata != null && docdata.Tables.Count > 0)
            {
                string docPath = docdata.Tables[0].Rows[0]["Path"].ToString();
                string docName = !string.IsNullOrEmpty(docPath) ? Path.GetFileName(docPath) : "";
                string jobId = docdata.Tables[0].Rows[0]["JobId"].ToString();

                string JobHistoryMessage = "<b class=\"blue-title\">" + docName + "</b>" + " Document has been signed and completed by all parties -<b class=\"blue-title\"> (" + jobId + ") JobRefNo </b>";

               
                Common.SaveJobHistorytoXML(Convert.ToInt32(jobId), JobHistoryMessage, "Documents", "SignCompletedByAllParties", ProjectSession.LoggedInName, false);
                #endregion
            }

        }
        public static void CompletedDocumentSentLog(int JobDocId, IDocumentSignatureLogBAL _documentSignatureLogBAL)
		{
			string message = "<p> Completed signed document has been sent to all parties.</p>";
			DocumentSignatureLog objDocumentLog = new DocumentSignatureLog()
			{
				CreatedOn = DateTime.Now,
				IpAddress = GetIp(),
				JobDocId = JobDocId,
				MessageType = SystemEnums.DocumentChangeLog.CompletedSent.GetHashCode(),
				Message = message,
				UserFullName = "",
				UserTypeName = ""
			};
			_documentSignatureLogBAL.Insert(objDocumentLog);
		}
	}
}