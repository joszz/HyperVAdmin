using HyperVAdmin.Models;
using HyperVAdmin.Utilities;
using System;
using System.Management;
using System.Web.Mvc;

namespace HyperVAdmin.Controllers
{
    public class VMsController : Controller
    {
        public ActionResult Index()
        {
            return View(VirtualMachineModel.GetVMList());
        }

        [HttpPost]
        public JsonResult GetVMs()
        {
            return Json(VirtualMachineModel.GetVMList());
        }

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