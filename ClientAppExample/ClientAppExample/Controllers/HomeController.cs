using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClientAppExample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            HttpCookie rqstUserid = Request.Cookies.Get("userid");
            HttpCookie rqstUsername = Request.Cookies.Get("username");


            // if user is authenticated, move to index page, else move to login page.
            if (rqstUserid == null || rqstUsername == null)
            {
                return RedirectToAction("Login");
            }
            else if (rqstUserid.Value == null || rqstUserid.Value == "" || rqstUsername.Value == null || rqstUsername.Value == "")
            {
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.username = Request.Cookies["username"].Value;
                ViewBag.userid = Request.Cookies["userid"].Value;
                return View();
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login()
        {
            //create authentication cookies

            var username = Request["username"];
            var userid = Request["Id"];
            var rqstUserid = new HttpCookie("userid");
            var rqstUsername = new HttpCookie("username");
            rqstUserid.Value = userid;
            rqstUsername.Value = username;

            DateTime curr = DateTime.Now;
            rqstUserid.Expires = curr.AddDays(7);
            rqstUsername.Expires = curr.AddDays(7);
            rqstUserid.Secure = false;
            rqstUsername.Secure = false;

            Response.Cookies.Add(rqstUserid);
            Response.Cookies.Add(rqstUsername);
            Response.AddHeader("Access-Control-Allow-Origin", "*");

            return View();
        }
    }
}