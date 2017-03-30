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
        /// Sets HyperVEnabled web.config setting to ViewBag.
        /// </summary>
        public BaseController()
        {
            ViewBag.HyperVEnabled = hyperVEnabled = bool.Parse(ConfigurationManager.AppSettings["HyperVEnabled"]);
        }
    }
}