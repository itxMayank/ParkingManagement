using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagement.Models
{
    public interface IParkingInfoRepository
    {
        Dictionary<string, int> GetInitialSlotCount();
        int GetTotalParkingSlotCount();
        ParkingInfo GetTotalCountOfParkingTypeSlots();
        ParkingInfo UpdateParkingSlotCount(string slotType, bool isParked, int slotNumber);
        List<string> GetTypesOfCar(string carType = "");
        List<string> GetTypesOfParkingSlots();
        ParkingInfo AddUpdateSlots(string slotName, int count);
        List<int> GetParkingSlotsList(string slotType);
        Dictionary<string, Dictionary<string, int>> GetSetCarWiseSlotTypes(string carType, bool ignoreCarType = false);
        void AddCarWiseSlotType(Dictionary<string, Dictionary<string, int>> slotTypes);
        void RemoveVehicleType(string carType);
        void UpdateListOfParkingSlots(string slotType, int slotNumber, bool isParked);
        void UpdateParkingInfo(ParkingInfo parkingInfo);
    }
}