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
using System.IO;
using System.Web.Script.Serialization;

namespace NotificationDbAPI.Controllers
{
    public class NotificationsController : ApiController
    {
        private NotificationDbAPIContext db = new NotificationDbAPIContext();

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
        public IHttpActionResult PostNotification(Notification notification)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string[] userids = notification.UserID.Split(',');
            List<User> users = new List<User>();

            foreach (string s in userids)
            {
                int userid  = int.Parse(s);
                var username = db.Users.Find(userid).Username;
                User user = new Models.User { UserID = int.Parse(s), Username = username };
                users.Add(user);

                UserNotification un = new UserNotification { UserID = int.Parse(s), NotificationID = notification.NotificationID, IsRead = false };
                db.UserNotifications.Add(un);
            }
            
            db.Notifications.Add(notification);
            db.SaveChanges();
            

            string URI = "http://localhost:62433/api/Notifications";


            var httpWebRequest = (HttpWebRequest)WebRequest.Create(URI);
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(new
                {
                    ID = notification.NotificationID,
                    Message = notification.Message,
                    Users = users
                });


                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();

            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var streamReader = new StreamReader(httpResponse.GetResponseStream());
            {
                var result = streamReader.ReadToEnd();
            }

            return CreatedAtRoute("DefaultApi", new { id = notification.NotificationID }, notification);
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