using Microsoft.Web.Administration;
using System.Web.Mvc;
using System.Linq;

namespace HyperVAdmin.Controllers
{
    public class SiteController : Controller
    {
        private ServerManager manager = new ServerManager();

        public ActionResult Index()
        {
            return View();
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