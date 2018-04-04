using HyperVAdmin.Utilities;
using System;
using System.Web;
using System.Web.Mvc;

namespace HyperVAdmin.Controllers
{
    public class SessionController : Controller
    {
        public SessionController()
        {
            ViewBag.ContainerClass = "full-height";
        }

        public ActionResult Index()
        {
            if(Request.Cookies["rememberme"] != null)
            {
                string[] credentials = Request.Cookies["rememberme"].Value.ToString().Split(':');
                UserInstanceElement userInstance = UserConfig.Users(credentials[0]);

                if (userInstance != null && userInstance.Password == credentials[1])
                {
                    Session["user"] = userInstance.Displayname;

                    return RedirectToAction("Index", "Index");
                }
            }

            return View(true);
        }

        [HttpPost]
        public ActionResult Index(string user, string password, bool rememberme = true)
        {
            UserInstanceElement userInstance = UserConfig.Users(user);

            if (userInstance != null && userInstance.Password == password)
            {
                Session["user"] = userInstance.Displayname;

                if (rememberme)
                {
                    HttpCookie cookie = new HttpCookie("rememberme", userInstance.Username + ":" + userInstance.Password);
                    cookie.Expires = DateTime.Today.AddDays(365);
                    Response.Cookies.Add(cookie);
                }

                return RedirectToAction("Index", "Index");
            }

            return View(false);
        }

        public ActionResult Logoff()
        {
            Session["user"] = null;

            if (Request.Cookies["rememberme"] != null)
            {
                HttpCookie cookie = Request.Cookies["rememberme"];
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }

            return RedirectToAction("Index");
        }
    }
}