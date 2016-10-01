using HyperVAdmin.Models;
using Microsoft.Web.Administration;
using System.Linq;
using System.Web.Mvc;

namespace HyperVAdmin.Controllers
{
    public class SitesController : Controller
    {
        private ServerManager manager = new ServerManager();

        public ActionResult Index()
        {
            return View(SiteModel.GetSites());
        }

        [HttpPost]
        public JsonResult GetSites()
        {
            return Json(SiteModel.GetSites());
        }

        [HttpPost]
        public void StopSite(string sitename)
        {
            Site site = manager.Sites.Where(s => s.Name == sitename).FirstOrDefault();

            if (site != null)
            {
                site.Stop();
            }
        }

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