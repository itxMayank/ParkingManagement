using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagement.Models
{
    public interface ICarRepository
    {
        CarsInfo AddCarToParking(CarsInfo carsInfo);
        CarsInfo AssignParkingSlotToCar(CarsInfo carsInfo);
        CarsInfo GetCarById(int id);
        List<CarsInfo> GetParkedCars();
        CarsInfo RemoveCarFromParking(int id);
        CarsInfo UpdateCarParkingStatus(CarsInfo carsInfo);
        void UpdateCarCollection(List<CarsInfo> lstCarsInfo);
    }
}
