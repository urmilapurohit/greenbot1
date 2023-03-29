using System;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using FormBot.Helper;
using FormBot.BAL.Service.Documents;
using FormBot.Entity.Documents;
using System.IO;
using Newtonsoft.Json;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Data;
using FormBot.Entity.Job;

namespace FormBot.Main.Controllers
{
    public class DocumentTemplateController : BaseController
    {
        #region Properties

        /// <summary>
        /// The _document template bal
        /// </summary>
        private readonly IDocumentTemplateBAL _documentTemplateBAL;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentTemplateController"/> class.
        /// </summary>
        /// <param name="documentTemplateBAL">The document template bal.</param>
        public DocumentTemplateController(IDocumentTemplateBAL documentTemplateBAL)
        {
            this._documentTemplateBAL = documentTemplateBAL;
        }

        #endregion

        #region Actions

        /// <summary>
        /// Gets the document template list.
        /// </summary>
        /// <param name="docTemplateName">Name of the document template.</param>
        /// <param name="solarCompanyId">The solar company identifier.</param>
        public void GetDocumentTemplateList(string docTemplateName, string solarCompanyIds = "", int jobType = 0, int folderTypeId = 0)
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            IList<DocumentTemplate> lstDocumentTemplate = new List<DocumentTemplate>();

            string companyIds = string.Empty;
            if (ProjectSession.UserTypeId == 4)
                companyIds = Convert.ToString(ProjectSession.SolarCompanyId);
            else
                companyIds = solarCompanyIds;

            lstDocumentTemplate = _documentTemplateBAL.DocumentTemplateList(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, docTemplateName, companyIds, ProjectSession.UserTypeId, jobType, folderTypeId);

            if (lstDocumentTemplate.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstDocumentTemplate.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstDocumentTemplate.FirstOrDefault().TotalRecords;
            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstDocumentTemplate, gridParam));
        }

        public JsonResult GetDocumentForCreateJob(int solarCompanyId, int jobType = 0, int folderTypeId = 0) {
            try {
                IList<DocumentTemplate> lstDocumentTemplate = new List<DocumentTemplate>();
                lstDocumentTemplate = _documentTemplateBAL.DocumentTemplateList(solarCompanyId, ProjectSession.UserTypeId, jobType, folderTypeId);
                return Json(new { status = true, lst = lstDocumentTemplate }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = "Error occurred. Error details: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
           
        }

        /// <summary>
        /// Saves the document template.
        /// </summary>
        /// <param name="docTemplateId">The document template identifier.</param>
        /// <param name="docTemplateName">Name of the document template.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveDocumentTemplate(string docTemplateId, string docTemplateName, bool IsDocTemplateChange, int stateID, int jobTypeId, string solarCompanyIds, bool isDefault = false)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);
            try
            {
                int docTemplateID = !string.IsNullOrEmpty(docTemplateId) ? Convert.ToInt32(docTemplateId) : 0;

                string path = string.Empty;
                string fileName = string.Empty;
                fileName = docTemplateName + ".pdf";
                string proofDocumentsFolder = ProjectSession.ProofDocuments;
                proofDocumentsFolder = Path.Combine(proofDocumentsFolder + "\\" + "UserDocuments" + "\\" + ProjectSession.LoggedInUserId + "\\" + "DocumentTemplate" + "\\");
                path = Path.Combine(proofDocumentsFolder + "\\" + fileName.Replace("%", "$"));
                if (IsDocTemplateChange || docTemplateID == 0)
                {
                    // Checking no of files injected in Request object  
                    if (Request.Files.Count > 0)
                    {
                        HttpPostedFileBase fileUpload = Request.Files[0];

                        // fileName = Path.GetFileName(fileUpload.FileName);

                        

                        
                        ////string proofDocumentsFolderURL = ProjectSession.ProofDocumentsURL;

                        
                        ////proofDocumentsFolderURL = Path.Combine(proofDocumentsFolderURL + "\\" + "UserDocuments" + "\\" + ProjectSession.LoggedInUserId + "\\" + "DocumentTemplate" + "\\");

                        if (!Directory.Exists(proofDocumentsFolder))
                        {
                            Directory.CreateDirectory(proofDocumentsFolder);
                        }

                        
                        if (System.IO.File.Exists(path))
                        {
                            string orignalFileName = Path.GetFileNameWithoutExtension(path);
                            string fileExtension = Path.GetExtension(path);
                            string fileDirectory = Path.GetDirectoryName(path);
                            int i = 1;
                            while (true)
                            {
                                string renameFileName = fileDirectory + @"\" + orignalFileName + "(" + i + ")" + fileExtension;
                                if (System.IO.File.Exists(renameFileName))
                                    i++;
                                else
                                {
                                    path = renameFileName;
                                    break;
                                }
                            }
                            fileName = Path.GetFileName(path);
                        }

                        fileName = fileName.Replace("%", "$");
                        string mimeType = MimeMapping.GetMimeMapping(fileName);
                        fileUpload.SaveAs(path);
                    }
                    else
                    {
                        return Json(new { status = false, error = "No files selected." }, JsonRequestBehavior.AllowGet);
                    }
                }

                DocumentTemplate docTemplate = new DocumentTemplate();

                docTemplate.DocumentTemplateId = docTemplateID;
                docTemplate.DocumentTemplateName = docTemplateName;
                docTemplate.SolarCompanyId = ProjectSession.SolarCompanyId;
                docTemplate.CreatedBy = ProjectSession.LoggedInUserId;
                docTemplate.CreatedDate = DateTime.Now;
                docTemplate.ModifiedBy = ProjectSession.LoggedInUserId;
                docTemplate.ModifiedDate = DateTime.Now;
                docTemplate.StateID = stateID;
                docTemplate.JobTypeID = jobTypeId;
                docTemplate.IsDefault = isDefault;

                if (ProjectSession.UserTypeId == 1 && !string.IsNullOrEmpty(solarCompanyIds))
                    docTemplate.SolarCompanyId = Convert.ToInt32(solarCompanyIds.Split(',')[0]);

                if (docTemplateID > 0 && !IsDocTemplateChange)
                {
                    DocumentTemplate documentTemplate = _documentTemplateBAL.GetDocumentTemplate(docTemplateID);
                    docTemplate.Path = documentTemplate.Path;

                    if (Path.GetFileNameWithoutExtension(documentTemplate.Path) != docTemplate.DocumentTemplateName)
                    {
                        string newPath = Path.GetDirectoryName(documentTemplate.Path) + "\\" + docTemplate.DocumentTemplateName + ".pdf";
                        string newFileName = ProjectSession.ProofDocuments + "\\" + newPath;
                        string oldFileName = ProjectSession.ProofDocuments + "\\" + documentTemplate.Path;

                        System.IO.File.Move(oldFileName, newFileName);

                        docTemplate.Path = newPath;
                    }
                }
                else
                {
                    docTemplate.Path = Path.Combine("UserDocuments" + "\\" + ProjectSession.LoggedInUserId + "\\" + "DocumentTemplate" + "\\" + fileName);
                }

                PdfReader reader1 = new PdfReader(ProjectSession.ProofDocuments + "\\" + docTemplate.Path);
                PdfReader.unethicalreading = true;
                //reader1.setUnethicalReading(true);
                string original = path;
                string inPDF = original;
                string tempFileName = Path.GetDirectoryName(original) + "\\temp.pdf";
                if (System.IO.File.Exists(tempFileName))
                {
                    System.IO.File.Delete(tempFileName);
                }
                string outPDF = tempFileName;
                iTextSharp.text.Document doc = new iTextSharp.text.Document(PageSize.A4);
                iTextSharp.text.Document.Compress = true;
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(outPDF, FileMode.Create));
                doc.Open();
                PdfContentByte cb1 = writer.DirectContent;
                PdfImportedPage page;
                for (int i = 1; i < reader1.NumberOfPages + 1; i++)
                {
                    page = writer.GetImportedPage(reader1, i);
                    cb1.AddTemplate(page, PageSize.A4.Width / reader1.GetPageSize(i).Width, 0, 0, PageSize.A4.Height / reader1.GetPageSize(i).Height, 0, 0);
                    doc.NewPage();
                }
                doc.Close();

                //PdfReader pdfFromWord = new PdfReader(outPDF);
                PdfStamper stamper = new PdfStamper(reader1, new FileStream(outPDF, FileMode.Create)); //throws
                //for (int i = 1; i <= reader1.NumberOfPages; ++i)
                //{
                //    stamper.ReplacePage(pdfFromWord, i, i);
                //}

                stamper.Close();
                reader1.Close();

                if (System.IO.File.Exists(ProjectSession.ProofDocuments + "\\" + docTemplate.Path))
                {
                    System.IO.File.Delete(ProjectSession.ProofDocuments + "\\" + docTemplate.Path);
                }

                System.IO.File.Move(outPDF, ProjectSession.ProofDocuments + "\\" + docTemplate.Path);



                int documentTemplateID = _documentTemplateBAL.DocumentTemplateInsertUpdate(docTemplate);
                ////int documentTemplateID = _documentTemplateBAL.DocumentTemplateInsertUpdate(docTemplate, ProjectSession.UserTypeId, solarCompanyIds);
                return Json(new { status = true, id = documentTemplateID }, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                return Json(new { status = false, error = "Error occurred. Error details: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the document template.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetDocumentTemplate(string id)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);
            try
            {
                int docTemplateID = 0;
                if (!string.IsNullOrEmpty(id))
                    int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out docTemplateID);

                DocumentTemplate documentTemplate = _documentTemplateBAL.GetDocumentTemplate(docTemplateID);

                if (documentTemplate != null)
                {
                    documentTemplate.FileName = Path.GetFileName(documentTemplate.Path);
                    return Json(new { model = documentTemplate, status = true }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Deletes the document template.
        /// </summary>
        /// <param name="docTemplateIds">The document template ids.</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DeleteDocumentTemplate(string docTemplateIds)
        {
            if (ProjectSession.LoggedInUserId == 0)
                return Json(new { status = false, error = "sessiontimeout" }, JsonRequestBehavior.AllowGet);

            try
            {
                List<string> lstDocTemplateIds = new List<string>();
                if (!string.IsNullOrEmpty(docTemplateIds))
                    lstDocTemplateIds = JsonConvert.DeserializeObject<List<string>>(docTemplateIds);

                List<int> lstNewTemplateId = new List<int>();

                for (int i = 0; i < lstDocTemplateIds.Count; i++)
                {
                    int templateId = 0;
                    if (!string.IsNullOrEmpty(lstDocTemplateIds[i]))
                    {
                        int.TryParse(QueryString.GetValueFromQueryString(lstDocTemplateIds[i], "id"), out templateId);
                    }
                    lstNewTemplateId.Add(templateId);
                }

                string templateIds = string.Join(",", lstNewTemplateId.ToArray());
                _documentTemplateBAL.DeleteDocumentTemplate(templateIds);
                return Json(new { status = true, id = 1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = ex.Message, id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Saves the open document template.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public ActionResult SaveOpenDocumentTemplate(string docTemplateId, List<Dictionary<string, string>> data)
        {
            string newFile = string.Empty;
            try
            {
                if (data == null || data.Count > 0)
                {
                    int docTemplateID = !string.IsNullOrEmpty(docTemplateId) ? Convert.ToInt32(docTemplateId) : 0;
                    DocumentTemplate documentTemplate = _documentTemplateBAL.GetDocumentTemplate(docTemplateID);
                    if (documentTemplate == null)
                    {
                        return Json(new { status = false, error = "Something went to wrong! please try after some times" }, JsonRequestBehavior.AllowGet);
                    }
                    string path = Path.Combine(ProjectSession.ProofDocumentsURL + documentTemplate.Path);

                    if (System.IO.File.Exists(path))
                    {
                        //PdfReader reader1 = new PdfReader(path);

                        //string original = path;
                        //string inPDF = original;
                        //string outPDF = Directory.GetDirectoryRoot(original) + "temp.pdf";
                        //iTextSharp.text.Document doc = new iTextSharp.text.Document(PageSize.A4);
                        //iTextSharp.text.Document.Compress = true;
                        //PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(outPDF, FileMode.Create));
                        //doc.Open();
                        //PdfContentByte cb1 = writer.DirectContent;
                        //PdfImportedPage page;
                        //for (int i = 1; i < reader1.NumberOfPages + 1; i++)
                        //{
                        //    page = writer.GetImportedPage(reader1, i);
                        //    cb1.AddTemplate(page, PageSize.A4.Width / reader1.GetPageSize(i).Width, 0, 0, PageSize.A4.Height / reader1.GetPageSize(i).Height, 0, 0);
                        //    doc.NewPage();
                        //} 
                        //doc.Close();

                        //path = outPDF;
                        PdfReader reader = new PdfReader(path);
                        newFile = path.Replace(Path.GetFileNameWithoutExtension(path), Path.GetFileNameWithoutExtension(path) + "_blank");

                        if (System.IO.File.Exists(newFile))
                        {
                            System.IO.File.Delete(newFile);
                        }
                        using (var out1 = System.IO.File.Open(newFile, FileMode.Create, FileAccess.Write))
                        {
                            PdfStamper stamp = new PdfStamper(reader, out1);
                            AcroFields pdfFormFields = stamp.AcroFields;
                            int i = 1;
                            int pageNum = 1;
                            int totalfonts = FontFactory.RegisterDirectory("C:\\WINDOWS\\Fonts");

                            for (int j = 1; j <= reader.NumberOfPages; j++)
                            {
                                stamp.AcroFields.RemoveFieldsFromPage(j);

                            }
                            if (data == null)
                            {

                            }
                            else
                            {
                                foreach (Dictionary<string, string> obj in data)
                                {
                                    if (obj.ContainsKey("fieldname"))
                                    {
                                        if (!(obj["fieldname"].Contains("undefined")))
                                        {
                                            List<Dictionary<string, string>> fieldsName = data.Where(a => a.ContainsKey("fieldname") && a["fieldname"].Contains(obj["fieldname"] + "_")).ToList();
                                            //obj["fieldname"] = obj["fieldname"] + "_" + (fieldsName.Count + 1);
                                        }
                                    }
                                    int.TryParse(obj["pagenumber"], out pageNum);

                                    PdfContentByte cb = stamp.GetOverContent(1);
                                    iTextSharp.text.Rectangle rect = new iTextSharp.text.Rectangle(float.Parse(obj["llx"]), float.Parse(obj["lly"]), float.Parse(obj["urx"]), float.Parse(obj["ury"]));

                                    //if (obj["fieldname"].Contains("Reseller_Signature"))
                                    //{
                                    //    TextField field = null;

                                    //    if (obj.ContainsKey("fieldname"))
                                    //        field = new TextField(stamp.Writer, rect, obj["fieldname"].ToString());
                                    //    else
                                    //        field = new TextField(stamp.Writer, rect, i.ToString());
                                    //    field.Options = TextField.READ_ONLY;
                                    //    stamp.AddAnnotation(field.GetTextField(), pageNum);
                                    //}
                                    //else 

                                    if (obj["type"] == "t")
                                    {
                                        TextField field = null;

                                        if (obj.ContainsKey("fieldname"))
                                            field = new TextField(stamp.Writer, rect, obj["fieldname"].ToString());
                                        else
                                            field = new TextField(stamp.Writer, rect, i.ToString());


                                        field.Text = obj["value"].ToString();
                                        field.FontSize = float.Parse((float.Parse(obj["font-size"].Split('p')[0].ToString()) * 0.75292857248934).ToString());

                                        if (!string.IsNullOrEmpty(obj["maxlength"]))
                                        {
                                            field.MaxCharacterLength = Convert.ToInt32(obj["maxlength"]);
                                        }

                                        if (!string.IsNullOrEmpty(obj["font-family"].ToString()))
                                        {
                                            int font = iTextSharp.text.Font.NORMAL;
                                            if (obj["fontWeight"].ToString().ToLower() == "bold")
                                            {
                                                font = iTextSharp.text.Font.BOLD;
                                            }

                                            if (obj["fontStyle"].ToString().ToLower() == "italic")
                                            {
                                                font = font > 0 ? iTextSharp.text.Font.BOLDITALIC : iTextSharp.text.Font.ITALIC;
                                            }
                                            iTextSharp.text.Font fontName = FontFactory.GetFont(obj["font-family"].ToString(), field.FontSize, font, new BaseColor(255, 0, 0));


                                            //if (obj["textDecoration"].ToString().ToLower().Contains("underline"))
                                            //{
                                            //    fontName.SetStyle(Font.UNDERLINE);
                                            //}
                                            field.Font = fontName.BaseFont;
                                            if (!string.IsNullOrEmpty(obj["bgcolor"].ToString()))
                                            {
                                                System.Drawing.Color color = (System.Drawing.Color)System.Drawing.ColorTranslator.FromHtml(obj["bgcolor"].ToString());
                                                BaseColor bgColor = new BaseColor(color);
                                                field.BackgroundColor = bgColor;
                                            }
                                            if (!string.IsNullOrEmpty(obj["bordercolor"].ToString()))
                                            {
                                                System.Drawing.Color color = (System.Drawing.Color)System.Drawing.ColorTranslator.FromHtml(obj["bordercolor"].ToString());
                                                BaseColor borderColor = new BaseColor(color);
                                                field.BorderColor = borderColor;
                                            }



                                        }
                                        if (!string.IsNullOrEmpty(obj["textAlign"].ToString()))
                                        {
                                            if (obj["textAlign"].ToString().ToLower() == "left")
                                                field.Alignment = Element.ALIGN_LEFT;
                                            else if (obj["textAlign"].ToString().ToLower() == "right")
                                                field.Alignment = Element.ALIGN_RIGHT;
                                            else if (obj["textAlign"].ToString().ToLower() == "center")
                                                field.Alignment = Element.ALIGN_CENTER;
                                        }
                                        if (!string.IsNullOrEmpty(obj["textcolor"].ToString()))
                                        {
                                            System.Drawing.Color color = (System.Drawing.Color)System.Drawing.ColorTranslator.FromHtml(obj["textcolor"].ToString());
                                            BaseColor textColor = new BaseColor(color);
                                            field.TextColor = textColor;
                                        }

                                        stamp.AddAnnotation(field.GetTextField(), pageNum);
                                    }
                                    else if (obj["type"] == "ta")
                                    {
                                        TextField field = new TextField(stamp.Writer, rect, i.ToString());
                                        //field.Rotation = 90;
                                        field.Text = obj["value"].ToString();
                                        field.FontSize = float.Parse((float.Parse(obj["font-size"].Split('p')[0].ToString()) * 0.75292857248934).ToString());
                                        field.Options = TextField.MULTILINE;
                                        if (!string.IsNullOrEmpty(obj["maxlength"]))
                                        {
                                            field.MaxCharacterLength = Convert.ToInt32(obj["maxlength"]);
                                        }

                                        if (obj.ContainsKey("fieldname"))
                                        {
                                            field.FieldName = Convert.ToString(obj["fieldname"]);
                                        }

                                        if (!string.IsNullOrEmpty(obj["font-family"].ToString()))
                                        {
                                            int font = iTextSharp.text.Font.NORMAL;
                                            if (obj["fontWeight"].ToString().ToLower() == "bold")
                                            {
                                                font = iTextSharp.text.Font.BOLD;
                                            }

                                            if (obj["fontStyle"].ToString().ToLower() == "italic")
                                            {
                                                font = font > 0 ? iTextSharp.text.Font.BOLDITALIC : iTextSharp.text.Font.ITALIC;
                                            }

                                            iTextSharp.text.Font fontName = FontFactory.GetFont(obj["font-family"].ToString(), field.FontSize, font, new BaseColor(255, 0, 0));
                                            if (obj["textDecoration"].ToString().ToLower().Contains("underline"))
                                            {
                                                fontName.SetStyle(iTextSharp.text.Font.UNDERLINE);
                                            }
                                            field.Font = fontName.BaseFont;

                                        }

                                        if (!string.IsNullOrEmpty(obj["bgcolor"].ToString()))
                                        {
                                            System.Drawing.Color color = (System.Drawing.Color)System.Drawing.ColorTranslator.FromHtml(obj["bgcolor"].ToString());
                                            BaseColor bgColor = new BaseColor(color);
                                            field.BackgroundColor = bgColor;
                                        }
                                        if (!string.IsNullOrEmpty(obj["bordercolor"].ToString()))
                                        {
                                            System.Drawing.Color color = (System.Drawing.Color)System.Drawing.ColorTranslator.FromHtml(obj["bordercolor"].ToString());
                                            BaseColor borderColor = new BaseColor(color);
                                            field.BorderColor = borderColor;
                                        }

                                        if (!string.IsNullOrEmpty(obj["textAlign"].ToString()))
                                        {
                                            if (obj["textAlign"].ToString().ToLower() == "left")
                                                field.Alignment = Element.ALIGN_LEFT;
                                            else if (obj["textAlign"].ToString().ToLower() == "right")
                                                field.Alignment = Element.ALIGN_RIGHT;
                                            else if (obj["textAlign"].ToString().ToLower() == "center")
                                                field.Alignment = Element.ALIGN_CENTER;
                                        }
                                        if (!string.IsNullOrEmpty(obj["textcolor"].ToString()))
                                        {
                                            System.Drawing.Color color = (System.Drawing.Color)System.Drawing.ColorTranslator.FromHtml(obj["textcolor"].ToString());
                                            BaseColor textColor = new BaseColor(color);
                                            field.TextColor = textColor;
                                        }

                                        stamp.AddAnnotation(field.GetTextField(), pageNum);
                                    }
                                    else if (obj["type"] == "c")
                                    {
                                        string fieldname = i.ToString();

                                        if (obj.ContainsKey("fieldname"))
                                        {
                                            fieldname = Convert.ToString(obj["fieldname"]);
                                        }

                                        string value = Convert.ToString(obj["value"]);

                                        RadioCheckField checkbox = new RadioCheckField(stamp.Writer, rect, fieldname, value);
                                        checkbox.CheckType = RadioCheckField.TYPE_CHECK;
                                        checkbox.Checked = Convert.ToBoolean(obj["check"]);
                                        PdfFormField fieldC = checkbox.CheckField;
                                        stamp.AddAnnotation(fieldC, pageNum);
                                        pdfFormFields.SetField(fieldname, value);
                                    }
                                    else if (obj["type"] == "r")
                                    {
                                        //RadioCheckField radio = new RadioCheckField(stamp.Writer, rect, string.IsNullOrEmpty(obj["group"]) ? i.ToString() : obj["group"], i.ToString());
                                        //radio.CheckType = RadioCheckField.TYPE_CIRCLE;
                                        //radio.Checked = Convert.ToBoolean(obj["radio"]);
                                        //PdfFormField field = radio.CheckField;
                                        //stamp.AddAnnotation(field, 1);

                                        PdfFormField radioGroup = PdfFormField.CreateRadioButton(stamp.Writer, true);
                                        //radioGroup.FieldName = i.ToString();         
                                        RadioCheckField radioG;
                                        PdfFormField radioField1;

                                        string btnGroup = i.ToString();
                                        if (obj.ContainsKey("group") && !string.IsNullOrEmpty(obj["group"]))
                                        {
                                            btnGroup = obj["group"];
                                        }
                                        radioG = new RadioCheckField(stamp.Writer, rect, btnGroup, i.ToString());
                                        radioG.BackgroundColor = new GrayColor(0.8f);
                                        radioG.BorderColor = GrayColor.BLACK;
                                        radioG.CheckType = RadioCheckField.TYPE_CIRCLE;
                                        radioG.Checked = Convert.ToBoolean(obj["radio"]);
                                        radioField1 = radioG.RadioField;
                                        radioField1.FieldName = i.ToString();

                                        //ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase(i.ToString(), new Font(Font.FontFamily.HELVETICA, 8)), 70, 790 - i * 40, 0);
                                        radioGroup.AddKid(radioField1);

                                        stamp.AddAnnotation(radioGroup, pageNum);
                                    }
                                    else if (obj["type"] == "sa" || obj["type"] == "btn" || obj["type"] == "dw")
                                    {
                                        string fieldname = i.ToString();
                                        if (obj.ContainsKey("fieldname"))
                                        {
                                            fieldname = obj["fieldname"].ToString();
                                        }
                                        if (fieldname.Equals("Reseller_Signature"))
                                        {
                                            TextField field = null;
                                            field = new TextField(stamp.Writer, rect, fieldname);
                                            field.Options = TextField.READ_ONLY;
                                            stamp.AddAnnotation(field.GetTextField(), pageNum);
                                        }
                                        else
                                        {
                                            string sign = "";
                                            string base30string = "";
                                            int lineWidth = 0;
                                            if (obj.ContainsKey("sign"))
                                            {
                                                sign = obj["sign"].ToString();
                                            }
                                            if (obj.ContainsKey("base30string"))
                                            {
                                                base30string = obj["base30string"].ToString();
                                            }
                                            if (obj.ContainsKey("lineWidth"))
                                            {
                                                lineWidth = Convert.ToInt32(obj["lineWidth"]);
                                            }
                                            PushbuttonField imageField = new PushbuttonField(stamp.Writer, rect, fieldname);
                                            imageField.Layout = PushbuttonField.LAYOUT_ICON_ONLY;
                                            //Store Signature in pdf As Image 
                                            if (fieldname.Contains("undefined"))
                                            {

                                                byte[] bytes = Convert.FromBase64String(sign.Split(',')[1]);
                                                imageField.Image = iTextSharp.text.Image.GetInstance(bytes);


                                            }
                                            //if (!string.IsNullOrEmpty(obj["bgcolor"].ToString()))
                                            //{
                                            //    System.Drawing.Color color = (System.Drawing.Color)System.Drawing.ColorTranslator.FromHtml(obj["bgcolor"].ToString());
                                            //    BaseColor bgColor = new BaseColor(color);
                                            //    imageField.BackgroundColor = bgColor;
                                            //}
                                            if (!string.IsNullOrEmpty(obj["bordercolor"].ToString()))
                                            {
                                                System.Drawing.Color color = (System.Drawing.Color)System.Drawing.ColorTranslator.FromHtml(obj["bordercolor"].ToString());
                                                BaseColor borderColor = new BaseColor(color);
                                                imageField.BorderColor = borderColor;
                                            }
                                            imageField.ScaleIcon = PushbuttonField.SCALE_ICON_ALWAYS;
                                            imageField.ProportionalIcon = false;
                                            imageField.Options = BaseField.READ_ONLY;
                                            imageField.Text = base30string;
                                            imageField.FieldName = fieldname;
                                            imageField.IconHorizontalAdjustment = (float)lineWidth / 10;
                                            imageField.MaxCharacterLength = 1;
                                            imageField.BorderWidth = 0;
                                            if (obj["type"] == "dw")
                                                imageField.IconVerticalAdjustment = 1;
                                            else
                                                imageField.IconVerticalAdjustment = 0;
                                            stamp.AddAnnotation(imageField.Field, pageNum);
                                        }

                                    }
                                    else if (obj["type"] == "im")
                                    {
                                        string fieldname = i.ToString();
                                        if (obj.ContainsKey("fieldname"))
                                        {
                                            fieldname = obj["fieldname"].ToString();
                                        }
                                        string sign = "";
                                        if (obj.ContainsKey("sign"))
                                        {
                                            sign = obj["sign"].ToString();
                                        }
                                        //rect = new iTextSharp.text.Rectangle(float.Parse(obj["llx"]), float.Parse(obj["lly"]), float.Parse(obj["urx"]), float.Parse(obj["ury"]));
                                        PushbuttonField imageField = new PushbuttonField(stamp.Writer, rect, fieldname);
                                        imageField.Layout = PushbuttonField.LAYOUT_ICON_ONLY;
                                        //imageField.Alignment = Element.ALIGN_TOP;
                                        if (obj.ContainsKey("align"))
                                        {
                                            imageField.FontSize = Convert.ToInt32(obj["align"]);
                                        }
                                        //imageField.TextColor = BaseColor.RED;
                                        //Store Signature in pdf As Image 
                                        if (fieldname.Contains("undefined"))
                                        {
                                            if(sign.Split(',').Length> 1 && !string.IsNullOrEmpty(sign.Split(',')[1]))
                                            {
                                                byte[] bytes = Convert.FromBase64String(sign.Split(',')[1]);
                                                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(bytes);
                                                imageField.Image = img;
                                                imageField.Image.ScaleAbsolute((float)Convert.ToDouble(obj["width"].ToString()), (float)Convert.ToDouble(obj["height"].ToString()));
                                            }
                                            if (obj.ContainsKey("aspect-ratio"))
                                            {
                                                imageField.IconVerticalAdjustment = float.Parse((Convert.ToDouble(obj["aspect-ratio"]) / 10000).ToString());

                                            }
                                            if (obj.ContainsKey("fill-option"))
                                            {
                                                imageField.IconHorizontalAdjustment = float.Parse((Convert.ToDouble(obj["fill-option"]) / 10000).ToString());
                                            }
                                        }
                                        imageField.ScaleIcon = PushbuttonField.SCALE_ICON_ALWAYS;
                                        imageField.ProportionalIcon = false;
                                        imageField.Options = BaseField.READ_ONLY;
                                        imageField.Text = sign;
                                        imageField.FieldName = fieldname;
                                        imageField.MaxCharacterLength = -1;
                                        imageField.BorderWidth = 1;
                                        stamp.AddAnnotation(imageField.Field, pageNum);
                                    }
                                    i++;
                                }
                            }
                            stamp.FormFlattening = true; // lock fields and prevent further edits.
                            stamp.Close();
                            stamp.Dispose();
                            reader.Close();
                            reader.Dispose();
                        }

                        string backupDir = Path.Combine(Path.GetDirectoryName(newFile), "BackupFiles");

                        if (!Directory.Exists(backupDir))
                            Directory.CreateDirectory(backupDir);

                        string backUpFiles = Path.Combine(backupDir, Path.GetFileName(newFile));

                        if (System.IO.File.Exists(backUpFiles))
                        {
                            System.IO.File.Delete(backUpFiles);
                        }

                        System.IO.File.Replace(newFile, path, backUpFiles);

                        return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = false, error = "File doesn't exist." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (System.IO.File.Exists(newFile))
                {
                    System.IO.File.Delete(newFile);
                }
                return Json(new { status = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets all document template.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetAllDocumentTemplate(int jobTypeId, int solarCompanyId, bool isSTC = false, bool isCES = false)
        {
            List<DocumentTemplate> documentTempalteList = _documentTemplateBAL.GetAllDocumentTemplate(jobTypeId, solarCompanyId, isSTC, isCES);
            documentTempalteList.Select(m => { m.Path = m.Path.Replace("\\", "/").Replace(@"\", "/"); return m; }).ToList();
            //documentTempalteList.Select(m =>  m.DocumentTemplateName ).ToList();
            return Json(documentTempalteList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetExportValueByJobFieldId(string jobFieldVal)
        {
            List<JobFieldExportValue> lstExportValue = _documentTemplateBAL.GetExportValueByJobFieldId(jobFieldVal);
            return Json(lstExportValue, JsonRequestBehavior.AllowGet);
        }

        //private void WriteToLogFile(string content)
        //{
        //    StreamWriter sw = new StreamWriter(ProjectSession.ProofDocumentsURL + "temp.txt", append: true);
        //    sw.WriteLine(content);
        //    sw.Flush();
        //    sw.Close();
        //}   


        /// <summary>
	    /// Rename Document Template Name
	    /// </summary>
	    /// <param name="docTemplateId"></param>
	    /// <param name="fileName"></param>
	    /// <returns></returns>
        [HttpPost]
	        public JsonResult RenameDocumentTemplateName(int docTemplateId, string fileName)
	        {
	            try
	            {
	                DocumentTemplate documentTemplate = _documentTemplateBAL.GetDocumentTemplate(docTemplateId);
	                var filePath = Path.GetDirectoryName(documentTemplate.Path);
	                var newPath = Path.Combine(filePath, fileName + ".pdf");
	                _documentTemplateBAL.RenameDocumentTemplateName(docTemplateId, newPath);
	                newPath = Path.Combine(ProjectSession.ProofDocumentsURL, newPath);
	                int i = 0;
	                string renameFileName = string.Empty;
	                while (true)
	                {
	                    if (i == 0)
	                        renameFileName = newPath;
	                    else
	                    {
	                        string name = Path.GetFileNameWithoutExtension(newPath);
	                        renameFileName = Path.Combine(Path.GetDirectoryName(newPath), name+"("+i+")" , ".pdf");
	                    }
	                        
	                    if (System.IO.File.Exists(renameFileName))
	                        i++;
	                    else
	                        break;
	                }
	                System.IO.File.Move(Path.Combine(ProjectSession.ProofDocumentsURL + documentTemplate.Path), renameFileName);
	                return Json(new { status = true,filename = Path.GetFileNameWithoutExtension(renameFileName),filepath = Path.Combine(ProjectSession.UploadedDocumentPath, Path.GetDirectoryName(documentTemplate.Path),Path.GetFileName(renameFileName)) }, JsonRequestBehavior.AllowGet);
	            }
	            catch (Exception ex)
	            {
	                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
	            }
	            
	        }
        #endregion
    }
}