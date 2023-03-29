using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Helper
{
    public static class Log
    {
        private static ILog _logger = null;
        private static log4net.ILog Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = log4net.LogManager.GetLogger
                (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                    log4net.Config.XmlConfigurator.Configure();
                }
                return _logger;
            }
        }

        public static void WriteError(Exception ex, string message = "")
        {
            try
            {
                WriteLog("enter in writeError:" + DateTime.Now);
                StringBuilder formattedMessage = new StringBuilder();
                formattedMessage.Append(string.Format("{0} Message : {1}\n InnerException : {2}", message, ex.Message, ex.InnerException));
                formattedMessage.Append("\n=================================================================================================================================================");
                WriteLog("enter in writeError between:" + DateTime.Now);
                //_logger.Error(formattedMessage);
                Logger.Error(formattedMessage);
                formattedMessage.Clear();
            }
            catch(Exception e)
            {
                WriteLog("enter in exception catch:" + DateTime.Now);
            }
           
        }
        public static void WriteLog(string msg)
        {
            Logger.Info(msg);
            //_logger.Error(string.Format("{0} Message : ",msg));
        }
    }
}
