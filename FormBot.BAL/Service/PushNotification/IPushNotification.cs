using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.BAL.Service
{
    public interface IPushNotification
    {
        /// <summary>
        /// Sends the push notification.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        void SendPushNotification(int userID, string message);
    }
}
