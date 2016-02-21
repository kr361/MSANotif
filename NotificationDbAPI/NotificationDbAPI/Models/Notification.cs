using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotificationDbAPI.Models
{
    public class Notification
    {
        public int NotificationID { get; set; }
        public string Message { get; set; }
        public DateTime NotificationTime { get; set; }
        public string UserID { get; set; }
    }
}