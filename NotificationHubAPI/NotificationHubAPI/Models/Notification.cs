using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NotificationHubAPI.Models
{
    public class Notification
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NotificationID { get; set; }
        public string Message { get; set; }
        public DateTime NotificationTime { get; set; }

        public IEnumerable<User> Users { get; set; }
    }
}