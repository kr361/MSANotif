using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using NotificationHubAPI.Models;
using Microsoft.AspNet.SignalR;
using NotificationSignalR.Hubs;

namespace NotificationHubAPI.Controllers
{
    public class NotificationsController : ApiController
    {
        private NotificationHubAPIContext db = new NotificationHubAPIContext();

        // GET: api/Notifications
        public IQueryable<Notification> GetNotifications()
        {
            return db.Notifications;
        }

        // GET: api/Notifications/5
        [ResponseType(typeof(Notification))]
        public IHttpActionResult GetNotification(int id)
        {
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return NotFound();
            }

            return Ok(notification);
        }

        // PUT: api/Notifications/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutNotification(int id, Notification notification)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != notification.NotificationID)
            {
                return BadRequest();
            }

            db.Entry(notification).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotificationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Notifications
        [ResponseType(typeof(Notification))]
        public IHttpActionResult PostNotification(int id, Notification notification)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            // connect this controller with notification hub
            var _context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            

            //Add new notification to target users
            using(var userenum = notification.Users.GetEnumerator())
            {
                while(userenum.MoveNext())
                {
                    string username = userenum.Current.Username;
                    if (!NotificationHub.userList.ContainsKey(username)) {
                        NotificationHub.userList.Add(username, userenum.Current.UserID);
                    }

                    var target = NotificationHub._connections.GetConnections(username);
                    foreach (string connectionIds in target)
                    {
                        _context.Clients.Client(connectionIds).addmessagetouser(id, notification.Message, false);
                    }
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = id }, notification);
        }

        // DELETE: api/Notifications/5
        [ResponseType(typeof(Notification))]
        public IHttpActionResult DeleteNotification(int id)
        {
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return NotFound();
            }

            db.Notifications.Remove(notification);
            db.SaveChanges();

            return Ok(notification);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool NotificationExists(int id)
        {
            return db.Notifications.Count(e => e.NotificationID == id) > 0;
        }
    }
}