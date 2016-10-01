using HyperVAdmin.Models;
using Microsoft.Web.Administration;
using System.Web.Mvc;

namespace HyperVAdmin.Controllers
{
    public class IndexController : Controller
    {
        private ServerManager manager = new ServerManager();

        public ActionResult Index()
        {
            ViewBag.Sites = SiteModel.GetSites();
            return View(VirtualMachineModel.GetVMList());
        }
    }
}
