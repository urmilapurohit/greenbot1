using FormBot.Entity.Pdf;
using iTextSharp.text.pdf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace FormBot.Main.Infrastructure
{
    public class CommonHelper
    {
        /// <summary>
        /// Get all inputs with it's value in list of Items result. It will use read.
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <returns>item list</returns>
        public static List<PdfItems> GetPDFItems(string fileName)
        {
            string pdfTemplate = fileName;
            bool isError = false;
        ReadAgain:
            isError = false;
            MemoryStream memStream = new MemoryStream();
            using (FileStream fileStream = System.IO.File.OpenRead(pdfTemplate))
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
                    if (k.Type == (int)FormBot.Helper.SystemEnums.InputTypes.RADIO_BUTTON || k.Type == (int)FormBot.Helper.SystemEnums.InputTypes.CHECK_BOX || k.Type == (int)FormBot.Helper.SystemEnums.InputTypes.LIST_BOX || k.Type == (int)FormBot.Helper.SystemEnums.InputTypes.COMBO_BOX)
                    {
                        try
                        {
                            k.AvailableValues.AddRange(GetCheckBoxExportValue(af, Convert.ToString(field.Key)));

                            var item = af.GetFieldItem(field.Key);
                            var value = af.GetField(field.Key);
                        }
                        catch (Exception ex)
                        {
                            Helper.Log.WriteError(ex);
                        }
                    }
                    else if (k.Type == (int)FormBot.Helper.SystemEnums.InputTypes.TEXT_FIELD)
                    {
                        try
                        {
                            //tempcode
                            TextField tx = new TextField(stamp.Writer, rect, Convert.ToString(field.Key));
                            af.DecodeGenericDictionary(af.GetFieldItem(Convert.ToString(field.Key)).GetMerged(0), tx);

                            if (tx.Options == 1)
                            {
                                k.ReadOnly = true;
                            }

                            AcroFields.Item fieldItem = af.GetFieldItem(Convert.ToString(field.Key));
                            PdfDictionary pdfDictionary = (PdfDictionary)fieldItem.GetWidget(0);

                            if (pdfDictionary.GetAsNumber(PdfName.MAXLEN) != null)
                            {
                                int maxFieldLength = Int32.Parse(pdfDictionary.GetAsNumber(PdfName.MAXLEN).ToString());
                                k.PdfItemProperties.MaxLength = maxFieldLength;
                            }

                            if (tx.Options == TextField.MULTILINE)
                                k.IsTextArea = true;

                            if (tx.TextColor != null)
                                k.PdfItemProperties.TextColor = string.Format("rgb({0}, {1}, {2})", tx.TextColor.R, tx.TextColor.G, tx.TextColor.B);
                            if (tx.BackgroundColor != null)
                                k.PdfItemProperties.BackgroundColor = string.Format("rgb({0}, {1}, {2})", tx.BackgroundColor.R, tx.BackgroundColor.G, tx.BackgroundColor.B);
                            if (tx.BorderColor != null)
                                k.PdfItemProperties.BorderColor = string.Format("rgb({0}, {1}, {2})", tx.BorderColor.R, tx.BorderColor.G, tx.BorderColor.B);
                            if (tx.Alignment == 0)
                            {
                                k.PdfItemProperties.Alignment = "left";
                            }
                            else if (tx.Alignment == 2)
                            {
                                k.PdfItemProperties.Alignment = "right";
                            }
                            else if (tx.Alignment == 1)
                            {
                                k.PdfItemProperties.Alignment = "center";
                            }
                            k.PdfItemProperties.FontSize = float.Parse((tx.FontSize * 1.3333333333333333).ToString());

                            if (tx.Font != null)
                            {
                                string[] fontProperty = tx.Font.PostscriptFontName.Split('-');

                                if (fontProperty.Count() > 0)
                                    k.PdfItemProperties.FontName = fontProperty[0];

                                if (fontProperty.Count() > 1)
                                {
                                    if (tx.Font.PostscriptFontName.IndexOf("Bold") >= 0)
                                        k.PdfItemProperties.Bold = true;

                                    if (tx.Font.PostscriptFontName.IndexOf("Oblique") >= 0 || tx.Font.PostscriptFontName.IndexOf("Italic") >= 0)
                                        k.PdfItemProperties.Italic = true;
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            Helper.Log.WriteError(ex);
                        }
                    }
                    else if (k.Type == (int)FormBot.Helper.SystemEnums.InputTypes.BUTTON)
                    {

                        if (!(k.FieldName.ToString().Contains("Reseller_Signature")))
                        {
                            PushbuttonField tx = af.GetNewPushbuttonFromField(Convert.ToString(field.Key));
                            af.DecodeGenericDictionary(af.GetFieldItem(Convert.ToString(field.Key)).GetMerged(0), tx);
                            k.Base64 = tx.Text;
                            k.Value = tx.Text;
                            k.lineWidth = tx.IconHorizontalAdjustment * 10;
                            k.IsImageField = tx.BorderWidth > 0 ? true : false;
                            if (tx.Options == 1)
                            {
                                k.ReadOnly = true;
                            }
                            //k.PdfItemProperties.AspectRatio = tx.FontSize;
                            //k.PdfItemProperties.ImageHeight = Convert.ToInt32(tx.IconHorizontalAdjustment * 10000);
                            //k.PdfItemProperties.ImageWidth = Convert.ToInt32(tx.IconVerticalAdjustment * 10000);
                            if (tx.IconVerticalAdjustment == 1)
                            {
                                k.IsDraw = true;
                            }
                            else
                            {
                                k.IsDraw = false;
                            }
                            if (tx.Options == 1)
                            {
                                k.ReadOnly = true;
                            }
                            if (tx.BackgroundColor != null)
                                k.PdfItemProperties.BackgroundColor = string.Format("rgb({0}, {1}, {2})", tx.BackgroundColor.R, tx.BackgroundColor.G, tx.BackgroundColor.B);
                            if (tx.BorderColor != null)
                                k.PdfItemProperties.BorderColor = string.Format("rgb({0}, {1}, {2})", tx.BorderColor.R, tx.BorderColor.G, tx.BorderColor.B);

                            AcroFields.Item fieldItem = af.GetFieldItem(Convert.ToString(field.Key));
                            PdfDictionary pdfDictionary = (PdfDictionary)fieldItem.GetWidget(0);
                        }
                        else
                        {
                            k.Type = 4;
                        }
                    }

                    if (k.Type == (int)FormBot.Helper.SystemEnums.InputTypes.CHECK_BOX)
                    {
                        string a = af.GetField(Convert.ToString(field.Key));
                    }

                    lstPdfItems.Add(k);
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Input string was not in a correct format.") || ex.Message.Contains("Original message: PDF startxref not found"))
                {
                    isError = true;
                    System.IO.File.AppendAllText(pdfTemplate, System.IO.File.ReadAllText(pdfTemplate).Substring(0, System.IO.File.ReadAllText(pdfTemplate).LastIndexOf("%%EOF") + 5));
                    goto ReadAgain;
                }
                Helper.Log.WriteError(ex);
            }

            finally
            {
                if (!isError)
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
            return lstPdfItems;
        }

        /// <summary>
        /// check export
        /// </summary>
        /// <param name="fields">acro field</param>
        /// <param name="cbFieldName">filed name</param>
        /// <returns>string array</returns>
        public static string[] GetCheckBoxExportValue(AcroFields fields, string cbFieldName)
        {
            AcroFields.Item fd = ((AcroFields.Item)fields.GetFieldItem(cbFieldName));
            var vals = fd.GetValue(0);
            Hashtable names = new Hashtable();
            string[] outs = new string[fd.Size];
            PdfDictionary pd = ((PdfDictionary)fd.GetWidget(0)).GetAsDict(PdfName.AP);
            for (int k1 = 0; k1 < fd.Size; ++k1)
            {
                PdfDictionary dic = (PdfDictionary)fd.GetWidget(k1);
                dic = dic.GetAsDict(PdfName.AP);
                if (dic == null)
                    continue;
                dic = dic.GetAsDict(PdfName.N);
                if (dic == null)
                    continue;
                foreach (PdfName pname in dic.Keys)
                {
                    String name = PdfName.DecodeName(pname.ToString());
                    if (name.ToLower() != "off")
                    {
                        names[name] = null;
                        outs[(outs.Length - k1) - 1] = name;
                    }
                }
            }
            return outs;
        }
    }
}