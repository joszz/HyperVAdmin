using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;

namespace HyperVAdmin.Models
{
    public class VirtualMachine
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public UInt64 MemoryTotal { get; set; }
        public int MemoryUsed { get; set; }
        public string MemoryAllocationUnits { get; set; }
        public VirtualMachineState State { get; set; }
        public UInt64 CoresAmount { get; set; }
        public string MAC { get; set; }
        public DateTime TimeOfLastStateChange { get; set; }
        public UInt64 OnTimeInMilliseconds { get; set; }

        public string TimeOfLastStateChangeFormatted
        {
            get
            {
                return TimeOfLastStateChange.ToString("dd-MM-yyyy HH:mm:ss");
            }
        }

        public TimeSpan GetOnTime
        {
            get
            {
                return TimeSpan.FromMilliseconds(OnTimeInMilliseconds);
            }
        }

        public string GetOnTimeFormatted
        {
            get
            {
                return TimeSpan.FromMilliseconds(OnTimeInMilliseconds).ToString(@"dd\.hh\:mm\:ss");
            }
        }

        public static List<VirtualMachine> GetVMList()
        {
            ManagementScope scope = GetVMScope();

            // define the information we want to query - in this case, just grab all properties of the object
            ObjectQuery queryObj = new ObjectQuery("SELECT * FROM Msvm_ComputerSystem WHERE Description != 'Microsoft Hosting Computer System'");

            // connect and set up our search
            ManagementObjectSearcher vmSearcher = new ManagementObjectSearcher(scope, queryObj);
            List<ManagementObject> vmCollection = vmSearcher.Get().Cast<ManagementObject>().OrderBy(vm => vm["ElementName"]).ToList();
            List<VirtualMachine> vms = new List<VirtualMachine>();

            foreach (ManagementObject vm in vmCollection)
            {
                ManagementObject settings = vm.GetRelated("Msvm_VirtualSystemSettingData").Cast<ManagementObject>().ToList().First();
                ManagementObject memorySettings = settings.GetRelated("Msvm_MemorySettingData").Cast<ManagementObject>().ToList().First();
                ManagementObject cores = settings.GetRelated("Msvm_ProcessorSettingData").Cast<ManagementObject>().ToList().First();
                ManagementObject ethernet = settings.GetRelated("Msvm_SyntheticEthernetPortSettingData").Cast<ManagementObject>().ToList().First();
                string mac = Regex.Replace(ethernet["Address"].ToString(), ".{2}", "$0:");

                vms.Add(new VirtualMachine
                {
                    Name = vm["ElementName"].ToString(),
                    Description = vm["Description"].ToString(),
                    State = (VirtualMachineState)(UInt16)vm["EnabledState"],
                    MemoryTotal = (UInt64)memorySettings["Limit"],
                    MemoryAllocationUnits = memorySettings["AllocationUnits"].ToString() == "byte * 2^20" ? "MB" : memorySettings["AllocationUnits"].ToString(),
                    CoresAmount = (UInt64)cores["VirtualQuantity"],
                    MAC = mac.Substring(0, mac.Length - 1),
                    TimeOfLastStateChange = ManagementDateTimeConverter.ToDateTime(vm["TimeOfLastStateChange"].ToString()),
                    OnTimeInMilliseconds = (UInt64)vm["OnTimeInMilliseconds"]
                });
            }

            return vms;
        }

        public static ManagementScope GetVMScope()
        {
            return new ManagementScope(@"\\.\root\virtualization\v2");
        }
    }

    public enum VirtualMachineState
    {
        Enabled = 2,
        Disabled = 3
    }
}