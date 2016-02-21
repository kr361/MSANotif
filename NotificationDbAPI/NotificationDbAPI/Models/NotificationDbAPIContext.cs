using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace NotificationDbAPI.Models
{
    public class NotificationDbAPIContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public NotificationDbAPIContext() : base("name=NotificationDbAPIContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // set UserID and NotificationID as composite PK for UserNotification
            modelBuilder.Entity<UserNotification>()
                .HasKey(un => new { un.UserID, un.NotificationID });
            base.OnModelCreating(modelBuilder);
        }

        public System.Data.Entity.DbSet<NotificationDbAPI.Models.Notification> Notifications { get; set; }

        public System.Data.Entity.DbSet<NotificationDbAPI.Models.User> Users { get; set; }

        public System.Data.Entity.DbSet<NotificationDbAPI.Models.UserNotification> UserNotifications { get; set; }
    }
}
