using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParkingManagement.Models
{
    public class AdminTaskRepository: IAdminTaskRepository
    {
        IParkingInfoRepository _parkingInfoRepository;
        AdminTask adminTask;

        public AdminTaskRepository(IParkingInfoRepository parkingInfoRepository)
        {
            _parkingInfoRepository = parkingInfoRepository;
            adminTask = GetSlotsType();
        }

        public void AddVehicles(string VehicleType, Dictionary<string, int> slotTypeSelected)
        {
            throw new NotImplementedException();
        }

        public AdminTask GetSlotsType(string carType = "")
        {
            adminTask = new AdminTask
            {
                //SlotTypes = _parkingInfoRepository.GetSetCarWiseSlotTypes("", true)
                lstSlots = _parkingInfoRepository.GetTypesOfParkingSlots()
            };
            if (carType != "")
            {
                adminTask.SlotTypes = adminTask.SlotTypes.Where(x => x.Key == carType).ToDictionary(x => x.Key, x => x.Value);
            }
            return adminTask;
        }


    }
}