using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotificationDbAPI.Models
{
    public class UserNotification
    {
        public int UserID { get; set; }
        public int NotificationID { get; set; }
        public bool IsRead { get; set; }
    }
}