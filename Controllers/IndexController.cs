using HyperVAdmin.Models;
using HyperVAdmin.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace HyperVAdmin.Controllers
{
    public class IndexController : Controller
    {
        private ManagementScope scope;

        public IndexController() : base()
        {
            scope = new ManagementScope(@"\\.\root\virtualization\v2");
        }

        public ActionResult Index()
        {
            // define the information we want to query - in this case, just grab all properties of the object
            ObjectQuery queryObj = new ObjectQuery("SELECT * FROM Msvm_ComputerSystem WHERE Description != 'Microsoft Hosting Computer System'");

            // connect and set up our search
            ManagementObjectSearcher vmSearcher = new ManagementObjectSearcher(scope, queryObj);
            ManagementObjectCollection vmCollection = vmSearcher.Get();

            List<VirtualMachine> vms = new List<VirtualMachine>();
            foreach(ManagementObject vm in vmCollection)
            {
                ManagementObject settings = vm.GetRelated("Msvm_VirtualSystemSettingData").Cast<ManagementObject>().ToList().First();
                ManagementObject memorySettings = settings.GetRelated("Msvm_MemorySettingData").Cast<ManagementObject>().ToList().First();
                ManagementObject cores = settings.GetRelated("Msvm_ProcessorSettingData").Cast<ManagementObject>().ToList().First();
                ManagementObject ethernet = settings.GetRelated("Msvm_SyntheticEthernetPortSettingData").Cast<ManagementObject>().ToList().First();

                string mac = Regex.Replace(ethernet["Address"].ToString(), ".{2}", "$0:");
                mac = mac.Substring(0, mac.Length - 1);

                vms.Add(new VirtualMachine
                {
                    Name = vm["ElementName"].ToString(),
                    Description = vm["Description"].ToString(),
                    State = (VirtualMachineState)Convert.ToInt32(vm["EnabledState"]),
                    MemoryTotal = Convert.ToInt32(memorySettings["Limit"]),
                    MemoryAllocationUnits = memorySettings["AllocationUnits"].ToString(),
                    CoresAmount = Convert.ToInt32(cores["VirtualQuantity"]),
                    MAC = mac
                });
                
            }

            return View(vms);
        }

        public JsonResult ToggleVMState(string vmName, VirtualMachineState state)
        {
            ManagementObject vm = HyperVUtility.GetTargetComputer(vmName, scope);

            ManagementBaseObject inParams = vm.GetMethodParameters("RequestStateChange");
            inParams["RequestedState"] = state;

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
            
            return Json(returnValue);
        }
    }
}
