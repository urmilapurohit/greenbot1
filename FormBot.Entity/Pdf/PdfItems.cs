using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity.Pdf
{
    public class PdfItems
    {
        public int? SrNo
        {
            get
            {
                return _SrNo;
            }

            set
            {
                _SrNo = value;
            }
        }
        private float _lineWidth;
        public float lineWidth
        {
            get
            {
                return _lineWidth;
            }
            set
            {
                _lineWidth = value;
            }
        }
        public object FieldName
        {
            get
            {
                return _FieldName;
            }

            set
            {
                _FieldName = value;
            }

        }
        private string _Base64;
        public string Base64
        {
            get
            {
                return _Base64;
            }
            set
            {
                _Base64 = value;
            }
        }

        private string _Value;
        public string Value
        {
            get
            {
                return _Value;
            }

            set
            {
                _Value = value;
            }
        }

        private int _Type;
        public int Type
        {
            get
            {
                return _Type;
            }

            set
            {
                _Type = value;
            }

        }

        private bool _ReadOnly;
        public bool ReadOnly
        {
            get
            {
                return _ReadOnly;
            }

            set
            {
                _ReadOnly = value;
            }

        }
        private bool _IsImageField;

        public bool IsImageField
        {
            get
            {
                return _IsImageField;
            }
            set
            {
                _IsImageField = value;
            }
        }

        private bool _IsDraw;

        public bool IsDraw
        {
            get
            {
                return _IsDraw;
            }
            set
            {
                _IsDraw = value;
            }
        }
        private bool _IsTextArea;
        public bool IsTextArea
        {
            get
            {
                return _IsTextArea;
            }

            set
            {
                _IsTextArea = value;
            }

        }


        private List<string> _AvailableValues;
        public List<string> AvailableValues
        {
            get
            {
                return _AvailableValues;
            }

            set
            {
                _AvailableValues = value;
            }

        }

        private PdfItemProperties _PdfItemProperties;
        public PdfItemProperties PdfItemProperties
        {
            get
            {
                return _PdfItemProperties;
            }

            set
            {
                _PdfItemProperties = value;
            }

        }

        private PdfItemSignatureProperties _PdfItemSignatureProperties;
        public PdfItemSignatureProperties PdfItemSignatureProperties
        {
            get
            {
                return _PdfItemSignatureProperties;
            }

            set
            {
                _PdfItemSignatureProperties = value;
            }
        }

        private int? _SrNo;
        private object _FieldName;

        public PdfItems()
        {
        }

        public PdfItems(int SrNo, object fieldName, string value, int type)
        {
            this.SrNo = SrNo;
            this.Type = type;
            FieldName = fieldName;
            Value = value;
            AvailableValues = new List<string>();
            PdfItemProperties = new PdfItemProperties();
        }

        private string _ImageName;
        public string ImageName
        {
            get
            {
                return _ImageName;
            }

            set
            {
                _ImageName = value;
            }
        }

    }

    public class PdfItemProperties
    {
        private int _MaxLength;
        private float _FontSize;
        private string _FontName;
        private string _HoriAlign;
        private string _VertAlign;
        private float _AspectRatio;
        private string _TextColor;
        private bool _Italic;
        private bool _Bold;
        private string _BackgroundColor;
        private string _BorderColor;
        private string _Alignment;
        private float _FillOption;
        private float _ImageAlign;
        public int MaxLength
        {
            get
            {
                return _MaxLength;
            }

            set
            {
                _MaxLength = value;
            }
        }

        public float FillOption
        {
            get
            {
                return _FillOption;
            }

            set
            {
                _FillOption = value;
            }
        }

        public float ImageAlign
        {
            get
            {
                return _ImageAlign;
            }

            set
            {
                _ImageAlign = value;
            }
        }


        public float FontSize
        {
            get
            {
                return _FontSize;
            }

            set
            {
                _FontSize = value;
            }
        }

        public string Alignment
        {
            get
            {
                return _Alignment;
            }
            set
            {
                _Alignment = value;
            }
        }

        public string BackgroundColor
        {
            get
            {
                return _BackgroundColor;
            }

            set
            {
                _BackgroundColor = value;
            }
        }
        public string BorderColor
        {
            get
            {
                return _BorderColor;
            }

            set
            {
                _BorderColor = value;
            }
        }
        public string FontName
        {
            get
            {
                return _FontName;
            }

            set
            {
                _FontName = value;
            }
        }

        public string HoriAlign
        {
            get
            {
                return _HoriAlign;
            }

            set
            {
                _HoriAlign = value;
            }
        }

        public string VertAlign
        {
            get
            {
                return _VertAlign;
            }

            set
            {
                _VertAlign = value;
            }
        }

        public float AspectRatio
        {
            get
            {
                return _AspectRatio;
            }

            set
            {
                _AspectRatio = value;
            }
        }

        public string TextColor
        {
            get
            {
                return _TextColor;
            }

            set
            {
                _TextColor = value;
            }
        }

        public bool Bold
        {
            get
            {
                return _Bold;
            }

            set
            {
                _Bold = value;
            }
        }

        public bool Italic
        {
            get
            {
                return _Italic;
            }

            set
            {
                _Italic = value;
            }
        }
    }

    public class PdfItemSignatureProperties
    {
        private float _Width;
        public float Width
        {
            get
            {
                return _Width;
            }

            set
            {
                _Width = value;
            }
        }
        private float _Height;
        public float Height
        {
            get
            {
                return _Height;
            }

            set
            {
                _Height = value;
            }
        }
        private float _Left;
        public float Left
        {
            get
            {
                return _Left;
            }

            set
            {
                _Left = value;
            }
        }
        private float _Right;
        public float Right
        {
            get
            {
                return _Right;
            }

            set
            {
                _Right = value;
            }
        }
        private float _Top;
        public float Top
        {
            get
            {
                return _Top;
            }

            set
            {
                _Top = value;
            }
        }
        private float _Bottom;
        public float Bottom
        {
            get
            {
                return _Bottom;
            }

            set
            {
                _Bottom = value;
            }
        }
        private int _PageNum;
        public int PageNum
        {
            get
            {
                return _PageNum;
            }

            set
            {
                _PageNum = value;
            }
        }

    }
}
