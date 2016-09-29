using HyperVAdmin.Models;
using Microsoft.Web.Administration;
using System.Management;
using System.Web.Mvc;

namespace HyperVAdmin.Controllers
{
    public class IndexController : Controller
    {
        public ActionResult Index()
        {
            return View(VirtualMachine.GetVMList());
        }
    }
}
