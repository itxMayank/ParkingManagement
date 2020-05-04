using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParkingManagement.Models
{
    public class ParkingInfo
    {
        public List<string> CarTypes { get; set; }

        public List<string> CarParkingSlotTypes { get; set; }

        public int TotalParkingSlots { get; set; }

        public Dictionary<string, int> SlotTypes { get; set; }

        public int SmallParkingSlotCount { get; set; }

        public int MediumParkingSlotCount { get; set; }

        public int LargeParkingSlotCount { get; set; }

    }
}