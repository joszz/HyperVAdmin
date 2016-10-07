using HyperVAdmin.Attributes;
using HyperVAdmin.Models;
using Microsoft.Web.Administration;
using System.Linq;
using System.Web.Mvc;

namespace HyperVAdmin.Controllers
{
    /// <summary>
    /// Controller to manage all Sites related actions.
    /// </summary>
    public class SitesController : Controller
    {
        private ServerManager manager = new ServerManager();

        /// <summary>
        /// Shows a list of all Sites and their related actions.
        /// </summary>
        /// <returns>The view showing all VMs</returns>
        public ActionResult Index()
        {
            return View(SiteModel.GetSites());
        }

        /// <summary>
        /// Gets a JSON array with all Sites. Called by AJAX to refresh content.
        /// </summary>
        /// <returns></returns>
        [AJAXOnly]
        [HttpPost]
        public JsonResult GetSites()
        {
            return Json(SiteModel.GetSites());
        }

        /// <summary>
        /// Stops a site. Sitename is used to determine which site to stop.
        /// </summary>
        /// <param name="sitename">Which site to stop.</param>
        [AJAXOnly]
        [HttpPost]
        public void StopSite(string sitename)
        {
            Site site = manager.Sites.Where(s => s.Name == sitename).FirstOrDefault();

            if (site != null)
            {
                site.Stop();
            }
        }

        /// <summary>
        /// Starts a site. Sitename is used to determine which site to start.
        /// </summary>
        /// <param name="sitename">Which site to start.</param>
        [AJAXOnly]
        [HttpPost]
        public void StartSite(string sitename)
        {
            Site site = manager.Sites.Where(s => s.Name == sitename).FirstOrDefault();

            if (site != null)
            {
                site.Start();
            }
        }
    }
}