using HyperVAdmin.Models;
using Microsoft.Web.Administration;
using System.Web.Mvc;

namespace HyperVAdmin.Controllers
{
    /// <summary>
    /// The homepage
    /// </summary>
    public class IndexController : BaseController
    {
        /// <summary>
        /// The default view for the root of the domain, showing both sites and VMs
        /// </summary>
        /// <returns>The default view</returns>
        public ActionResult Index()
        {
            ViewBag.Sites = SiteModel.GetSites();
            ViewBag.VMs = VirtualMachineModel.GetVMList();

            return View();
        }
    }
}
