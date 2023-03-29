using FormBot.BAL.Service;
using FormBot.Entity.Email;
using FormBot.Helper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace FormBot.Main.Areas.QueuedEmail.Controllers
{
    public class EmailManageController : Controller
    {
		#region Fields
		public string TempEmailAttechmentPath = System.Configuration.ConfigurationManager.AppSettings["TempEmailAttechmentPath"].ToString();
		public string UploadEmailAttechmentPath = System.Configuration.ConfigurationManager.AppSettings["UploadEmailAttechmentPath"].ToString();
		private readonly IEmailBAL _emailBal;
		#endregion
		#region Controller
		public EmailManageController(IEmailBAL emailBal)
		{
			this._emailBal = emailBal;
		}
		#endregion
		#region Actions
		// GET: QueuedEmail/EmailManage
		public ActionResult Index()
        {
            return View();
        }
		public ActionResult UploadFiles(string Guid)
		{
			int EmailAttechmentId = 0;
			try
			{
				string FullPathWithFileName = "";
				string filename = "";
				for (int i = 0; i < Request.Files.Count; i++)
				{
					EmialAttechment objEmialAttechment = new EmialAttechment();
					HttpPostedFileBase file = Request.Files[i];
					string name = EncodeHtml(Path.GetFileName(file.FileName)).Replace(@"'", @"\'");
					objEmialAttechment.FileMimeType = EncodeHtml(file.ContentType);
					string TempFolderNameWithGuid = (UploadEmailAttechmentPath + Guid);
                    //FullPathWithFileName = Server.MapPath(TempFolderNameWithGuid + "/" + name);
                    FullPathWithFileName = ProjectSession.ProofDocuments + "\\" + TempFolderNameWithGuid + "\\" + name;
                    CreateFolder(ProjectSession.ProofDocuments + "\\" + TempFolderNameWithGuid);
					GetFilePathWithCheckingDuplicateFileName(ref FullPathWithFileName);
					file.SaveAs(FullPathWithFileName);
					filename = GetFileNameOverFileFullPath(FullPathWithFileName);
					objEmialAttechment.FileName = filename;
					objEmialAttechment.FilePath = TempFolderNameWithGuid + "\\" + filename;
					objEmialAttechment.Guid = new Guid(Guid);
					EmailAttechmentId = _emailBal.InsertEmailAttechment(objEmialAttechment);
				}
				if(string.IsNullOrEmpty(filename))
					filename = GetFileNameOverFileFullPath(FullPathWithFileName);
				return Json(new { status = 1, filenameWithFullPath = filename, Id = EmailAttechmentId, FullPathWithFileName= FullPathWithFileName });
			}
			catch (Exception ex)
			{
				return Json(new { status = 0, filenameWithFullPath = ex.Message, Id = EmailAttechmentId });
			}
			
		}
		[HttpPost]
		public JsonResult SendMail(FormBot.Entity.Email.QueuedEmail objQueuedEmail)
		{
			try
			{
				objQueuedEmail.CreatedDate = DateTime.Now;
				objQueuedEmail.ModifiedDate = DateTime.Now;
				objQueuedEmail.FromEmail = ProjectSession.MailFrom;
				_emailBal.InsertUpdateQueuedEmail(objQueuedEmail);
				return Json("1");
			}
			catch (Exception ex)
			{
				return Json("0");
			}
		}
		[HttpPost]
		public JsonResult DeleteAttechment(int Id)
		{
			try
			{
				_emailBal.DeleteEmailAttechment(Id);
				return Json("1");
			}
			catch (Exception ex)
			{
				return Json("0");
			}
		}
		public ActionResult DownloadFileFromTheServer(string file_name, string fileFullPath)
		{
			string _download = "1";
			string userAgent;
			try
			{
				var buffer = new byte[0];
				var fullPath = fileFullPath;
				if (System.IO.File.Exists(fullPath))
				{
					using (var fs = System.IO.File.OpenRead(fullPath))
					{
						buffer = new byte[fs.Length];
						fs.Read(buffer, 0, buffer.Length);
					}
				}
				var filename = file_name;
				var download = _download;
				string encodedFilename;
				userAgent = Request.UserAgent;
				if (userAgent.IndexOf("MSIE") > -1)
				{
					encodedFilename = Server.UrlPathEncode(filename);
				}
				else
				{
					encodedFilename = filename;
				}
				Response.Clear();
				if (string.Compare(download, "1", true, CultureInfo.InvariantCulture) == 0)
				{
					Response.Clear();
					Response.ClearHeaders();
					Response.ClearContent();
					Response.AddHeader("Content-Disposition", @"attachment; filename=""" + encodedFilename + @"""");
					Response.AddHeader("Accept-Ranges", "bytes");
					Response.AddHeader("Content-Length", buffer.Length.ToString(CultureInfo.InvariantCulture));
					Response.AddHeader("Content-Transfer-Encoding", "binary");
					Response.ContentType = "application/octet-stream";
				}
				else
				{
					var ext = Path.GetExtension(filename);
					if (!string.IsNullOrEmpty(ext))
					{
						ext = ext.Substring(1, ext.Length - 1); // remove first dot
					}
					Response.ContentType = GetAttachmentMimeTypeFromFileExtension(ext);
				}
				Response.BinaryWrite(buffer);
			}
			catch (Exception ex)
			{
				FormBot.Email.Log.WriteException(ex);
			}
			return Json("", JsonRequestBehavior.AllowGet);
		}

		#endregion
		#region Methods
		public void CreateFolder(string Path)
		{
            bool exists = System.IO.Directory.Exists(Path);
			if (!exists)
                System.IO.Directory.CreateDirectory(Path);
		}
		public string GetFilePathWithCheckingDuplicateFileName(ref string FullPathWithFileName)
		{
			int count = 1;
			string fileNameOnly = Path.GetFileNameWithoutExtension(FullPathWithFileName);
			string extension = Path.GetExtension(FullPathWithFileName);
			string path = Path.GetDirectoryName(FullPathWithFileName);
			string newFullPath = FullPathWithFileName;
			while (System.IO.File.Exists(newFullPath))
			{
				string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
				newFullPath = Path.Combine(path, tempFileName + extension);
			}
			FullPathWithFileName = newFullPath;
			return FullPathWithFileName;
		}
		public string GetFileNameOverFileFullPath(string FullPathWithFileName)
		{
			string fileNameOnly = Path.GetFileNameWithoutExtension(FullPathWithFileName);
			string extension = Path.GetExtension(FullPathWithFileName);
			return fileNameOnly + extension;
		}
		public string EncodeHtml(string s)
		{
			Regex rChar = new Regex("[\x0-\x8\xB-\xC\xE-\x1F]+");
			s = rChar.Replace(s, " ");

			StringBuilder sb = new StringBuilder(s);
			sb.Replace("&", "&amp;");
			sb.Replace("<", "&lt;");
			sb.Replace(">", "&gt;");
			return sb.ToString();
		}
		public string GetAttachmentMimeTypeFromFileExtension(string fileExtension)
		{
			string result = "application/octet-stream";

			if (string.IsNullOrEmpty(fileExtension))
			{
				return result;
			}

			switch (fileExtension.ToLower(CultureInfo.InvariantCulture))
			{
				case "gif": return "image/gif";
				case "png": return "image/png";
				case "jpe":
				case "jpg":
				case "jpeg": return "image/jpeg";
				case "tif":
				case "tiff": return "image/tiff";
				case "bin":
				case "dms":
				case "lha":
				case "lzh":
				case "exe":
				case "class":
				case "dll": return "application/octet-stream";
				case "js": return "application/x-javascript";
				case "swf": return "application/x-shockwave-flash";
				case "doc": return "application/msword";
				case "zip": return "application/zip";
				case "ai":
				case "eps":
				case "ps": return "application/postscript";
				case "pdf": return "application/pdf";
				case "rtf": return "application/rtf";
				case "ppt": return "application/vnd.ms-powerpoint";
				case "htm":
				case "html": return "text/html";
				case "css": return "text/css";
				case "rtx": return "text/richtext";
				case "txt":
				case "asc": return "text/plain";
				case "xml": return "text/xml";
				case "wav": return "audio/x-wav";
				case "mid":
				case "midi": return "audio/midi";
				case "mpga":
				case "mp2":
				case "mp3": return "audio/mpeg";
				case "aif":
				case "aiff": return "audio/x-aiff";
				case "ra": return "audio/x-realaudio";
				case "mpeg":
				case "mpg":
				case "mpe": return "video/mpeg";
				case "qt":
				case "mov": return "video/quicktime";
				case "avi": return "video/x-msvideo";
			}


			RegistryKey regKey = null;
			try
			{
				if (fileExtension[0] == '.')
				{
					fileExtension = fileExtension.Remove(0, 1);
				}

				regKey = Registry.ClassesRoot;
				if (regKey != null)
				{
					regKey = regKey.OpenSubKey(string.Format(CultureInfo.InvariantCulture, ".{0}", fileExtension));
				}
				if (regKey != null)
				{
					result = (string)regKey.GetValue("Content Type");
				}
			}
			catch
			{
				return result;
			}
			finally
			{
				if (regKey != null) regKey.Close();
			}
			return result ?? "application/octet-stream";
		}
		#endregion
	}
}