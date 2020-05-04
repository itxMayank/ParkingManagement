using System.Collections.Generic;

namespace ParkingManagement.Models
{
    public class AdminTask
    {
        public Dictionary<string, Dictionary<string, int>> SlotTypes { get; set; }
        public List<string> lstSlots { get; set; }
    }
}