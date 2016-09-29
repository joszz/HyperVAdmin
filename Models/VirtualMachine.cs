using System;

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
    }

    public enum VirtualMachineState
    {
        Enabled = 2,
        Disabled = 3
    }
}