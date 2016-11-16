using HyperVAdmin.Attributes;
using HyperVAdmin.Models;
using System.Web.Mvc;

namespace HyperVAdmin.Controllers
{
    /// <summary>
    /// Controller to manage all VM related actions.
    /// </summary>
    public class VMsController : Controller
    {
        /// <summary>
        /// Shows a list of all VMs and their related actions.
        /// </summary>
        /// <returns>The view showing all VMs</returns>
        public ActionResult Index()
        {
            return View(VirtualMachineModel.GetVMList());
        }

        /// <summary>
        /// Gets a JSON array with all VMs. Called by AJAX to refresh content.
        /// </summary>
        /// <returns>A JSON array with all VMs.</returns>
        [AJAXOnly]
        [HttpPost]
        public JsonResult GetVMs()
        {
            return Json(VirtualMachineModel.GetVMList());
        }

        /// <summary>
        /// Toggles the state of the VM. The vmName is used to target the VM and the state is used to set the state of the VM.
        /// </summary>
        /// <param name="vmName">Which VM to toggle.</param>
        /// <param name="state">What VirtualMachineState to set the VM to.</param>
        /// <returns>JSON with a string indicating success of the action.</returns>
        [AJAXOnly]
        [HttpPost]
        public JsonResult ToggleState(string vmName, VirtualMachineState state)
        {
            return Json(VirtualMachineModel.ToggleState(vmName, state));
        }
    }
}