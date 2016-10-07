using HyperVAdmin.Attributes;
using HyperVAdmin.Models;
using HyperVAdmin.Utilities;
using System;
using System.Management;
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
        /// <returns>JSON with a string indiciating success of the action.</returns>
        [AJAXOnly]
        [HttpPost]
        public JsonResult ToggleState(string vmName, VirtualMachineState state)
        {
            ManagementScope scope = VirtualMachineModel.GetVMScope();
            ManagementObject vm = HyperVUtility.GetTargetComputer(vmName, scope);
            ManagementBaseObject inParams = vm.GetMethodParameters("RequestStateChange");
            inParams["RequestedState"] = state;

            ManagementBaseObject outParams = vm.InvokeMethod("RequestStateChange", inParams, null);

            string returnValue = string.Empty;

            if ((UInt32)outParams["ReturnValue"] == ReturnCode.Started)
            {
                if (HyperVUtility.JobCompleted(outParams, scope))
                {
                    returnValue = string.Format("VM '{0}' state was changed successfully.", vmName);
                }
                else
                {
                    returnValue = "Failed to change virtual system state";
                }
            }
            else if ((UInt32)outParams["ReturnValue"] == ReturnCode.Completed)
            {
                returnValue = string.Format("VM '{0}' state was changed successfully.", vmName);
            }
            else
            {
                returnValue = string.Format("Change virtual system state failed with error {0}.", outParams["ReturnValue"]);
            }

            return Json(returnValue);
        }
    }
}