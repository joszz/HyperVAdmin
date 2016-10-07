using HyperVAdmin.Models;
using Microsoft.Web.Administration;
using System.Web.Mvc;

namespace HyperVAdmin.Controllers
{
    /// <summary>
    /// The homepage
    /// </summary>
    public class IndexController : Controller
    {
        private ServerManager manager = new ServerManager();

        /// <summary>
        /// The default view for the root of the domain, showing both sites and VMs
        /// </summary>
        /// <returns>The default view</returns>
        public ActionResult Index()
        {
            ViewBag.Sites = SiteModel.GetSites();
            return View(VirtualMachineModel.GetVMList());
        }
    }
}
