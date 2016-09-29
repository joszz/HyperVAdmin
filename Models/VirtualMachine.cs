
namespace HyperVAdmin.Models
{
    public class VirtualMachine
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int MemoryTotal { get; set; }
        public int MemoryUsed { get; set; }
        public string MemoryAllocationUnits { get; set; }
        public VirtualMachineState State { get; set; }
        public int CoresAmount { get; set; }
        public string MAC { get; set; }
    }

    public enum VirtualMachineState
    {
        Enabled = 2,
        Disabled = 3
    }
}