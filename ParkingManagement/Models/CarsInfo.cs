using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ParkingManagement.Models
{
    public class CarsInfo
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Vehicle Number")]
        public string CarNumber { get; set; }

        [Required]
        [DisplayName("Vehicle Type")]
        public string CarType { get; set; }

        [DisplayName("Parking Slot Type")]
        public string CarParkingSlotType { get; set; }

        [DisplayName("Slot Number")]
        public int CarParkingSlotNumber { get; set; }

        [DisplayName("Park Vehicle")]
        public bool IsParked { get; set; }

        public DateTime CarInTime { get; set; }

        public DateTime? CarOutTime { get; set; }

        public bool IsParkingSlotAvailable { get; set; }

        public ParkingInfo ParkingInfo { get; set; }

    }


}