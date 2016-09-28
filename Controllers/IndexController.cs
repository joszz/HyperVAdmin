using HyperVAdmin.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Web;
using System.Web.Mvc;

namespace HyperVAdmin.Controllers
{
    public class IndexController : Controller
    {
        private ManagementScope scope;
        const int Enabled = 2, Disabled = 3;

        public IndexController() : base()
        {
            scope = new ManagementScope(@"\\.\root\virtualization\v2");
        }

        public ActionResult Index()
        {
            // define the information we want to query - in this case, just grab all properties of the object
            ObjectQuery queryObj = new ObjectQuery("SELECT * FROM Msvm_ComputerSystem");

            // connect and set up our search
            ManagementObjectSearcher vmSearcher = new ManagementObjectSearcher(scope, queryObj);

            return View(vmSearcher.Get());
        }

        public ActionResult StartVM(string vmName)
        {
            ManagementObject vm = HyperVUtility.GetTargetComputer(vmName, scope);

            ManagementBaseObject inParams = vm.GetMethodParameters("RequestStateChange");
            inParams["RequestedState"] = Enabled;

            ManagementBaseObject outParams = vm.InvokeMethod(
                            "RequestStateChange",
                            inParams,
                            null);

            string returnValue = string.Empty;

            if ((UInt32)outParams["ReturnValue"] == ReturnCode.Started)
            {
                if (HyperVUtility.JobCompleted(outParams, scope))
                {
                    returnValue = string.Format("{0} state was changed successfully.", vmName);
                }
                else
                {
                    returnValue = "Failed to change virtual system state";
                }
            }
            else if ((UInt32)outParams["ReturnValue"] == ReturnCode.Completed)
            {
                returnValue = string.Format("{0} state was changed successfully.", vmName);
            }
            else
            {
                returnValue = string.Format("Change virtual system state failed with error {0}.", outParams["ReturnValue"]);
            }

            return RedirectToAction("Index");
        }
    }
}
