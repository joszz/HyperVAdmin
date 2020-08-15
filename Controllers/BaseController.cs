using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;

namespace HyperVAdmin.Controllers
{
    /// <summary>
    /// BaseController for all controllers.
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// Whether HyperV is enabled in web.config.
        /// </summary>
        protected bool hyperVEnabled = true;

        /// <summary>
        /// Whether IIS is enabled in web.config.
        /// </summary>
        protected bool iisEnabled = true;

        /// <summary>
        /// Sets HyperVEnabled web.config setting to ViewBag.
        /// </summary>
        public BaseController()
        {
            string modules = ConfigurationManager.AppSettings["ModulesEnabled"].ToLower();

            if (modules == "iis")
            {
                hyperVEnabled = false;
            }
            else if (modules == "hyperv")
            {
                iisEnabled = false;
            }

            ViewBag.HyperVEnabled = hyperVEnabled;
            ViewBag.IISEnabled = iisEnabled;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            SetPasswordIfChanged();

            bool loginEnabled = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["Password"]);
            ViewBag.LoginEnabled = loginEnabled;

            if (loginEnabled && string.IsNullOrEmpty(Session["loggedin"]?.ToString()) && GetType() != typeof(SessionController))
            {
                filterContext.Result = RedirectToAction("Index", "Session");
            }

            base.OnActionExecuting(filterContext);
        }

        private void SetPasswordIfChanged()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Password"]) && string.IsNullOrEmpty(ConfigurationManager.AppSettings["PasswordSalt"]))
            {
                var config = WebConfigurationManager.OpenWebConfiguration("~");

                using (HMACSHA512 hmac = new HMACSHA512())
                {
                    config.AppSettings.Settings.Add("PasswordSalt", Convert.ToBase64String(hmac.Key));
                    config.AppSettings.Settings["Password"].Value = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["Password"])));
                }

                config.Save(ConfigurationSaveMode.Minimal, false);
            }
        }
    }
}