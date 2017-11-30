using System.Configuration;
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

            if(modules == "iis")
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
    }
}