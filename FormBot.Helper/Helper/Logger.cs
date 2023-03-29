using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Helper
{
    public interface ILogger
    {
        void LogException(SystemEnums.Severity severity, string message, Exception ex);
        void LogException(SystemEnums.Severity severity, string message);
        void LogException(string message, Exception ex);
        void LogFormat(SystemEnums.Severity severity, string message, params object[] args);
        void Log(SystemEnums.Severity severity, string message);
    }
    public class Logger : ILogger
    {
        private readonly ILog _log;

        public Logger()
        {
            XmlConfigurator.Configure();
            _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        public void LogException(SystemEnums.Severity severity, string message, Exception ex)
        {
            StringBuilder formattedMessage = new StringBuilder();
            Exception innerException = (ex == null ? null : ex.InnerException);
            formattedMessage.Append(string.Format("{0} Message : {1}\n InnerException : {2}", message, (ex == null ? "" : ex.Message), innerException));
            formattedMessage.Append("\n=================================================================================================================================================");
            Log(severity, formattedMessage.ToString());
        }

        public void LogException(SystemEnums.Severity severity, string message)
        {
            StringBuilder formattedMessage = new StringBuilder();
            formattedMessage.Append(string.Format("Message : {0}", message));
            Log(severity, formattedMessage.ToString());
        }

        public void LogException(string message, Exception ex)
        {
            LogException(SystemEnums.Severity.Error, message, ex);
        }

        public void LogFormat(SystemEnums.Severity severity, string message, params object[] args)
        {
            string formattedMessage = string.Format(message, args);
            Log(severity, formattedMessage);
        }

        public void Log(SystemEnums.Severity severity, string message)
        {
            switch (severity)
            {
                case SystemEnums.Severity.Debug:
                    _log.Debug(message);
                    break;
                case SystemEnums.Severity.Info:
                    _log.Info(message);
                    break;
                case SystemEnums.Severity.Warning:
                    _log.Warn(message);
                    break;
                case SystemEnums.Severity.Error:
                    _log.ErrorFormat(message);
                    break;
            }
        }
    }
}
