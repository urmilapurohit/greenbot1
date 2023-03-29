using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Documents
{
    public class DocumentCollectionView
    {
        public string Data { get; set; }
        public List<Pdf.PdfItems> PdfItems { get; set; }
        public string PDFURL { get; set; }

        public string DocId { get; set; }

        public string JobDocId { get; set; }

        public string JobId { get; set; }
        public string PDFSource { get; set; }
        public string ParsedData
        {
            get
            {
                try
                {
                    return Newtonsoft.Json.JsonConvert.SerializeObject(PdfItems);
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        public List<Pdf.PdfItems> ParsedPdfItems
        {
            get
            {
                List<Pdf.PdfItems> items = new List<Pdf.PdfItems>();
                try
                {
                    items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Pdf.PdfItems>>(Data);
                }
                catch (Exception)
                {
                }

                return items;
            }

        }
    }
}
