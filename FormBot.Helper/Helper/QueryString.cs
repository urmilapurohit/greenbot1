using FormBot.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Helper
{
    public static class QueryString
    {
        #region Variable Declaration
        public static string KeyString
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["GUIDKey"].ToString(); }
        }
        #endregion

        public static string QueryStringEncode(string value)
        {
            return EncryptionDecryption.GetEncrypt(value);
        }
        public static string QueryStringDecode(string value)
        {
            return EncryptionDecryption.GetDecrypt(value);
        }

        public static string GetValueFromQueryString(string value, string key)
        {
            string strValue = string.Empty;
            if (value != null)
            {
                string DataString = EncryptionDecryption.GetDecrypt(value);
                Hashtable objHash = QueryString.GetQueryString(DataString);
                strValue = Convert.ToString(objHash[key]);
                return strValue;
            }
            else
            {
                return "";
            }
        }

        public static Hashtable GetQueryString(string DataValue)
        {
            string[] AndArray = DataValue.Split(new Char[] { '&' });
            Hashtable objHash = new Hashtable();
            string[] splitArray;
            try
            {
                if (AndArray.Length > 0)
                {
                    for (int i = 0; i <= AndArray.Length - 1; i++)
                    {
                        splitArray = AndArray[i].Split(new Char[] { '=' });
                        objHash.Add(splitArray[0], splitArray[1]);
                    }
                }
            }
            catch (Exception)
            {
                return objHash;
            }
            return objHash;
        }
    }
}
