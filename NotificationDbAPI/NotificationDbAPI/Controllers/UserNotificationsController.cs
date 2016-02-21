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
using NotificationDbAPI.Models;

namespace NotificationDbAPI.Controllers
{
    public class UserNotificationsController : ApiController
    {
        private NotificationDbAPIContext db = new NotificationDbAPIContext();

        // GET: api/UserNotifications
        public IQueryable<UserNotification> GetUserNotifications()
        {
            return db.UserNotifications;
        }

        // GET: api/UserNotifications/5
        [ResponseType(typeof(UserNotification))]
        public IHttpActionResult GetUserNotification(int id)
        {
            var userNotification = from b in db.UserNotifications
                                   where b.UserID == id
                                   select b;
            if (userNotification == null)
            {
                return NotFound();
            }
            else
            {
                List<Notification> nlist = new List<Notification>();

                foreach (UserNotification s in userNotification)
                {
                    var notif = db.Notifications
                        .Where(n => n.NotificationID == s.NotificationID)
                        .FirstOrDefault();
                    if (notif != null)
                        nlist.Add(notif);
                }
                if (nlist.Count < 1)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(nlist);
                }
            }
        }

        // PUT: api/UserNotifications/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUserNotification(int userid, int notificationid)
        {
            if (!UserNotificationExists(userid, notificationid))
            {
                return NotFound();
            }

            var entry = db.UserNotifications
                        .Where(un => un.NotificationID == notificationid)
                        .Where(un => un.UserID == userid)
                        .FirstOrDefault();

            if (entry == null)
            {
                return NotFound();
            }

            entry.IsRead = true;

            db.Entry(entry).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserNotificationExists(userid, notificationid))
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

        // POST: api/UserNotifications
        [ResponseType(typeof(UserNotification))]
        public IHttpActionResult PostUserNotification(UserNotification userNotification)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.UserNotifications.Add(userNotification);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (UserNotificationExists(userNotification.UserID, userNotification.NotificationID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = userNotification.UserID }, userNotification);
        }

        // DELETE: api/UserNotifications/5
        [ResponseType(typeof(UserNotification))]
        public IHttpActionResult DeleteUserNotification(int userid, int notificationid)
        {
            if (!UserNotificationExists(userid, notificationid))
            {
                return NotFound();
            }

            var entry = db.UserNotifications
                        .Where(un => un.NotificationID == notificationid)
                        .Where(un => un.UserID == userid)
                        .FirstOrDefault();

            if (entry == null)
            {
                return NotFound();
            }

            db.UserNotifications.Remove(entry);
            db.SaveChanges();

            return Ok(entry);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserNotificationExists(int userid, int notificationid)
        {
            return db.UserNotifications.Count(e => e.UserID == userid) > 0 && db.UserNotifications.Count(e => e.NotificationID == notificationid) > 0;
        }
    }
}