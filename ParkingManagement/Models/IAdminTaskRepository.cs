using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagement.Models
{
    public interface IAdminTaskRepository
    {
        void AddVehicles(string VehicleType, Dictionary<string, int> slotTypeSelected);
        AdminTask GetSlotsType(string carType = "");
    }
}
