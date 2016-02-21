using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;
using System.Web.Http.Cors;
using NotificationHubAPI.Models;

namespace NotificationSignalR.Hubs
{
    [HubName("notificationHub")]
    public class NotificationHub : Microsoft.AspNet.SignalR.Hub
    {
        public readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();

        public readonly static Dictionary<string, int> userList = new Dictionary<string, int>();

        public void SendMessageOnChat(string users, string message)
        {
            string[] userids = new JavaScriptSerializer().Deserialize<string[]>(users);

            foreach (string s in userids)
            {
                foreach (var connectionId in _connections.GetConnections(s))
                {
                    Clients.Client(connectionId).addMessageToUser(message);
                }
            }

        }
        
        public void readMessage(int NotificationId)
        {
            string userid = Context.Request.Cookies["userid"].Value;

            var request = (HttpWebRequest)WebRequest.Create("http://localhost:52287/api/UserNotifications?userid=" 
                                                            + userid + "&notificationid=" + NotificationId);

            request.Method = "PUT";
            request.ContentType = "text/json";

            using(var requestStream = request.GetRequestStream())
            {
                requestStream.Flush();
                requestStream.Close();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        }

        public void getUnreadMessages(string userid)
        {

            string username = Context.Request.Cookies["username"].Value;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:52287/api/UserNotifications/" + userid);

            try
            {

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
            

                string data = reader.ReadToEnd();

                reader.Close();
                stream.Close();

                List<Notification> msgs = new JavaScriptSerializer().Deserialize<List<Notification>>(data);

                foreach(Notification s in msgs)
                {
                    foreach(var conId in _connections.GetConnections(username))
                    {
                        Clients.Client(conId).addMessagetouser(s.NotificationID, s.Message);
                    }
                }

            } catch (WebException)
            {
                return;
            }
            
        }

        public override Task OnConnected()
        {
            string username = Context.RequestCookies["username"].Value;

            _connections.Add(username, Context.ConnectionId);
                  
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {

             string username = Context.RequestCookies["username"].Value;


            _connections.Remove(username, Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            string username = Context.RequestCookies["username"].Value;

            if (!_connections.GetConnections(username).Contains(Context.ConnectionId))
            {
                _connections.Add(username, Context.ConnectionId);
            }

            return base.OnReconnected();
        }
    }

    public class ConnectionMapping<T>
    {
        private readonly Dictionary<T, HashSet<string>> _connections =
            new Dictionary<T, HashSet<string>>();

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    connections = new HashSet<string>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        public IEnumerable<string> GetConnections(T key)
        {
            HashSet<string> connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        public void Remove(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }
    }
}