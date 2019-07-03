using HyperVAdmin.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;

namespace HyperVAdmin.Models
{
    /// <summary>
    /// The model representing a VirtualMachine
    /// </summary>
    public class VirtualMachineModel
    {
        /// <summary>
        /// The name of the VM.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the VM.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The total Virtual Memory available for this VM.
        /// </summary>
        public UInt64 MemoryTotal { get; set; }

        /// <summary>
        /// The units of virtual memory allocation as a string (such as MB).
        /// </summary>
        public string MemoryAllocationUnits { get; set; }

        /// <summary>
        /// The current state of the VM. Such as start, stop etc.
        /// </summary>
        public VirtualMachineState State { get; set; }

        /// <summary>
        /// The amounts of virtual cores associated with this VM.
        /// </summary>
        public ushort CoresAmount { get; set; }

        /// <summary>
        /// The MAC address of the virtual network switch adapter.
        /// </summary>
        public string MAC { get; set; }

        private UInt16 _cpuLoad = 0;
        /// <summary>
        /// The CPULoad retrieved from WMI.
        /// </summary>
        public UInt16? CPULoad
        {
            get
            {
                return _cpuLoad;
            }
            set
            {
                _cpuLoad = value == null ? (UInt16)0 : (UInt16)value;
            }
        }

        /// <summary>
        /// The last time the VirtualMachineState was changed.
        /// </summary>
        public DateTime TimeOfLastStateChange { get; set; }

        /// <summary>
        /// The time in ms inidicating how long the VM has been running. Will be 0 when off.
        /// </summary>
        public UInt64 OnTimeInMilliseconds { get; set; }

        /// <summary>
        /// A human readable string of the last time the VirtualMachineState was changed.
        /// </summary>
        public string TimeOfLastStateChangeFormatted
        {
            get
            {
                return TimeOfLastStateChange.ToString("dd-MM-yyyy HH:mm:ss");
            }
        }

        /// <summary>
        /// A TimeSpan inidicating how long the VM has been running.
        /// </summary>
        public TimeSpan GetOnTime
        {
            get
            {
                return TimeSpan.FromMilliseconds(OnTimeInMilliseconds);
            }
        }

        /// <summary>
        /// A human readable represantation of the time the VM has been running.
        /// </summary>
        public string GetOnTimeFormatted
        {
            get
            {
                return TimeSpan.FromMilliseconds(OnTimeInMilliseconds).ToString(@"dd\.hh\:mm\:ss");
            }
        }

        /// <summary>
        /// Gets a list of VirtualMachinesModels.
        /// </summary>
        /// <returns>A list of VirtualMachineModels.</returns>
        public static List<VirtualMachineModel> GetVMList()
        {
            ManagementScope scope = GetVMScope();

            // define the information we want to query - in this case, just grab all properties of the object
            ObjectQuery queryObj = new ObjectQuery(ConfigurationManager.AppSettings["HyperVQueryVMs"].ToString());

            // connect and set up our search
            ManagementObjectSearcher vmSearcher = new ManagementObjectSearcher(scope, queryObj);
            List<ManagementObject> vmCollection = vmSearcher.Get().Cast<ManagementObject>().OrderBy(vm => vm["ElementName"]).ToList();
            List<VirtualMachineModel> vms = new List<VirtualMachineModel>();

            foreach (ManagementObject vm in vmCollection)
            {
                ManagementObject settings = vm.GetRelated("Msvm_VirtualSystemSettingData").Cast<ManagementObject>().ToList().FirstOrDefault();
                ManagementObject memorySettings = settings.GetRelated("Msvm_MemorySettingData").Cast<ManagementObject>().ToList().FirstOrDefault();
                ManagementObject ethernet = settings.GetRelated("Msvm_SyntheticEthernetPortSettingData").Cast<ManagementObject>().ToList().FirstOrDefault();
                ManagementObject information = vm.GetRelated("Msvm_SummaryInformation").Cast<ManagementObject>().ToList().FirstOrDefault();

                string mac = string.Empty;
                if (ethernet == null)
                {
                    ethernet = settings.GetRelated("Msvm_EmulatedEthernetPortSettingData").Cast<ManagementObject>().ToList().FirstOrDefault();
                }

                if(ethernet != null)
                { 
                    mac = Regex.Replace(ethernet["Address"].ToString(), ".{2}", "$0:");
                }

                vms.Add(new VirtualMachineModel
                {
                    Name = vm["ElementName"].ToString(),
                    Description = vm["Description"].ToString(),
                    State = (VirtualMachineState)(UInt16)vm["EnabledState"],
                    MemoryTotal = (UInt64)memorySettings["VirtualQuantity"],
                    MemoryAllocationUnits = memorySettings["AllocationUnits"].ToString() == "byte * 2^20" ? "MB" : memorySettings["AllocationUnits"].ToString(),
                    CoresAmount = information != null ? (ushort)information["NumberOfProcessors"] : (ushort)0,
                    CPULoad = information != null ? (UInt16?)information["ProcessorLoad"] : null,
                    MAC = string.IsNullOrWhiteSpace(mac) ? mac :  mac.Substring(0, mac.Length - 1),
                    TimeOfLastStateChange = ManagementDateTimeConverter.ToDateTime(vm["TimeOfLastStateChange"].ToString()),
                    OnTimeInMilliseconds = (UInt64)vm["OnTimeInMilliseconds"]
                });
            }

            return vms;
        }

        /// <summary>
        /// Toggles the state of the VM. The vmName is used to target the VM and the state is used to set the state of the VM.
        /// </summary>
        /// <param name="vmName">Which VM to toggle.</param>
        /// <param name="state">What VirtualMachineState to set the VM to.</param>
        /// <returns>A string indicating success of the action.</returns>
        public static string ToggleState(string vmName, VirtualMachineState state)
        {
            ManagementScope scope = GetVMScope();
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

            return returnValue;
        }

        /// <summary>
        /// Used to retrieve the VirtualMachineModel List by GetVMList.
        /// </summary>
        /// <returns>The ManagementScope object to use to build the VM List.</returns>
        public static ManagementScope GetVMScope()
        {
            return new ManagementScope(ConfigurationManager.AppSettings["HyperVManagementPath"].ToString());
        }
    }

    /// <summary>The used VirtualMachineStates used internally by WMI. Only supported states for this application are present.</summary>
    public enum VirtualMachineState
    {
        /// <summary>
        /// The on state
        /// </summary>
        Enabled = 2,
        /// <summary>
        /// The off state
        /// </summary>
        Disabled = 3
    }
}