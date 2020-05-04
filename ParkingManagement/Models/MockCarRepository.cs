using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace ParkingManagement.Models
{
    public class MockCarRepository : ICarRepository
    {

        public List<CarsInfo> _carsInfos;
        readonly ObjectCache cache = MemoryCache.Default;
        const string carsCollectionKey = "CarsInParkingSlot";
        static Random rnd = new Random();

        readonly CacheItemPolicy cacheItemPolicy = new CacheItemPolicy
        {
            AbsoluteExpiration = DateTime.Now.AddHours(1.0)
        };

        IParkingInfoRepository _parkingInfoRepository;

        public MockCarRepository(IParkingInfoRepository parkingInfoRepository)
        {
            _carsInfos = GetParkedCars();
            _parkingInfoRepository = parkingInfoRepository;
        }

        public CarsInfo AddCarToParking(CarsInfo carsInfo)
        {
            CarsInfo info = new CarsInfo();
            if (carsInfo.IsParked == false)
            {
                carsInfo.IsParked = true;
            }
            carsInfo.Id = (_carsInfos.Count + 1);
            carsInfo.CarInTime = DateTime.Now;
            carsInfo = AssignParkingSlotToCar(carsInfo);
            if (carsInfo.IsParkingSlotAvailable)
            {
                info.IsParkingSlotAvailable = true;
                info.ParkingInfo = _parkingInfoRepository.UpdateParkingSlotCount(carsInfo.CarParkingSlotType, carsInfo.IsParked, carsInfo.CarParkingSlotNumber);
                _carsInfos.Add(carsInfo);
                UpdateCarCollection(_carsInfos);
            }
            else
            {
                info.ParkingInfo = _parkingInfoRepository.GetTotalCountOfParkingTypeSlots();
            }
            return info;
        }

        public CarsInfo AssignParkingSlotToCar(CarsInfo carsInfo)
        {
            int slotCount = 0;
            double percentSlotAvailable;
            ParkingInfo parkingInfo = _parkingInfoRepository.GetTotalCountOfParkingTypeSlots();
            List<int> lstParkingSlots = new List<int>();
            Dictionary<string, Dictionary<string, int>> dctCarWiseslots;
            Dictionary<string, int> dctSlotType = new Dictionary<string, int>();
            Dictionary<string, int> initialSlotCounts;

            if (parkingInfo.SlotTypes != null)
            {
                carsInfo.IsParkingSlotAvailable = false;

                dctCarWiseslots = _parkingInfoRepository.GetSetCarWiseSlotTypes(carsInfo.CarType);
                if (dctCarWiseslots != null && dctCarWiseslots.Count > 0)
                {
                    if (dctCarWiseslots.ContainsKey(carsInfo.CarType))
                    {
                        dctCarWiseslots.TryGetValue(carsInfo.CarType, out dctSlotType);
                        if (dctSlotType != null && dctSlotType.Count > 0)
                        {
                            initialSlotCounts = _parkingInfoRepository.GetInitialSlotCount();

                            foreach (string slotNamekey in dctSlotType.Keys)
                            {
                                parkingInfo.SlotTypes.TryGetValue(slotNamekey, out slotCount);
                                percentSlotAvailable = (Convert.ToDouble(slotCount) / initialSlotCounts[slotNamekey]) * 100;

                                if (percentSlotAvailable > 0 && percentSlotAvailable >= dctSlotType[slotNamekey])
                                {
                                    carsInfo.CarParkingSlotType = slotNamekey;
                                    lstParkingSlots = _parkingInfoRepository.GetParkingSlotsList(slotNamekey);
                                    carsInfo.CarParkingSlotNumber = lstParkingSlots[rnd.Next(lstParkingSlots.Count)];
                                    carsInfo.IsParkingSlotAvailable = true;
                                }
                                return carsInfo;
                            }
                        }
                    }

                }
                carsInfo.IsParkingSlotAvailable = false;
                return carsInfo;

                //percentSmallSlotAvailable = (Convert.ToDouble(smallSlotCount) / 50) * 100;
                //percentMediumSlotAvailable = (Convert.ToDouble(mediumSlotCount) / 30) * 100;
                //percentLargeSlotAvailable = (Convert.ToDouble(largeSlotCount) / 10) * 100;

                //if (carsInfo.CarType.Equals("Hatchback", StringComparison.InvariantCultureIgnoreCase) || carsInfo.CarType.Equals("Two Wheeler", StringComparison.InvariantCultureIgnoreCase))
                //{
                //    if (percentSmallSlotAvailable > 0)
                //    {
                //        carsInfo.CarParkingSlotType = "Small";
                //        lstParkingSlots = _parkingInfoRepository.GetParkingSlotsList("Small");
                //        carsInfo.CarParkingSlotNumber = lstParkingSlots[rnd.Next(lstParkingSlots.Count)];
                //        //carsInfo.CarParkingSlotNumber = (50 - smallSlotCount);
                //    }
                //    else if(percentMediumSlotAvailable > 0)
                //    {
                //        carsInfo.CarParkingSlotType = "Medium";
                //        lstParkingSlots = _parkingInfoRepository.GetParkingSlotsList("Medium");
                //        carsInfo.CarParkingSlotNumber = lstParkingSlots[rnd.Next(lstParkingSlots.Count)];
                //        //carsInfo.CarParkingSlotNumber = (30 - mediumSlotCount);
                //    }
                //    else if(percentLargeSlotAvailable > 10)
                //    {
                //        carsInfo.CarParkingSlotType = "Large";
                //        lstParkingSlots = _parkingInfoRepository.GetParkingSlotsList("Large");
                //        carsInfo.CarParkingSlotNumber = lstParkingSlots[rnd.Next(lstParkingSlots.Count)];
                //        //carsInfo.CarParkingSlotNumber = (20 - largeSlotCount);
                //    }
                //    else
                //    {
                //        carsInfo.IsParkingSlotAvailable = false;
                //    }
                //}
                //else if(carsInfo.CarType.Equals("Sedan/Compact SUV", StringComparison.InvariantCultureIgnoreCase))
                //{
                //    if (percentMediumSlotAvailable > 0)
                //    {
                //        carsInfo.CarParkingSlotType = "Medium";
                //        lstParkingSlots = _parkingInfoRepository.GetParkingSlotsList("Medium");
                //        carsInfo.CarParkingSlotNumber = lstParkingSlots[rnd.Next(lstParkingSlots.Count)];
                //        //carsInfo.CarParkingSlotNumber = (30 - mediumSlotCount);
                //    }
                //    else if (percentLargeSlotAvailable > 10)
                //    {
                //        carsInfo.CarParkingSlotType = "Large";
                //        lstParkingSlots = _parkingInfoRepository.GetParkingSlotsList("Large");
                //        carsInfo.CarParkingSlotNumber = lstParkingSlots[rnd.Next(lstParkingSlots.Count)];
                //        //carsInfo.CarParkingSlotNumber = (20 - largeSlotCount);
                //    }
                //    else
                //    {
                //        carsInfo.IsParkingSlotAvailable = false;
                //    }
                //}
                //else if (carsInfo.CarType.Equals("SUV or Large cars", StringComparison.InvariantCultureIgnoreCase) || carsInfo.CarType.Equals("Truck", StringComparison.InvariantCultureIgnoreCase))
                //{
                //    if (percentLargeSlotAvailable > 0)
                //    {
                //        carsInfo.CarParkingSlotType = "Large";
                //        lstParkingSlots = _parkingInfoRepository.GetParkingSlotsList("Large");
                //        carsInfo.CarParkingSlotNumber = lstParkingSlots[rnd.Next(lstParkingSlots.Count)];
                //        //carsInfo.CarParkingSlotNumber = (20 - largeSlotCount);
                //    }
                //    else
                //    {
                //        carsInfo.IsParkingSlotAvailable = false;
                //    }
                //}
                //else
                //{
                //    carsInfo.IsParkingSlotAvailable = false;
                //} 

            }
            if (carsInfo.IsParkingSlotAvailable)
            {
                // carsInfo.CarParkingSlotNumber += 1;
            }

            return carsInfo;
        }

        public CarsInfo GetCarById(int id)
        {
            return _carsInfos.FirstOrDefault(x => x.Id == id);
        }

        public List<CarsInfo> GetParkedCars()
        {
            List<CarsInfo> carsInfo = new List<CarsInfo>();

            if (cache.Contains(carsCollectionKey))
            {
                return (List<CarsInfo>)cache.Get(carsCollectionKey);
            }
            else
            {
                carsInfo = new List<CarsInfo>
                {
                    //new CarsInfo() { Id = 1, CarNumber = "MH-20 DU 3202", CarType="Small", CarParkingSlotNumber = 1, CarInTime = DateTime.Now, CarParkingSlotType = "Small", IsParked = true },
                    //new CarsInfo() { Id = 2, CarNumber = "MH-12 DU 3202", CarType="Sedan", CarParkingSlotNumber = 52, CarInTime = DateTime.Now, CarParkingSlotType = "Medium", IsParked = true },
                    //new CarsInfo() { Id = 3, CarNumber = "MH-15 DU 3202", CarType="Big", CarParkingSlotNumber = 85, CarInTime = DateTime.Now, CarParkingSlotType = "Large", IsParked = true },
                };

                cache.Add(carsCollectionKey, carsInfo, cacheItemPolicy);
            }
            return carsInfo;
        }

        public CarsInfo RemoveCarFromParking(int id)
        {
            CarsInfo carsInfo = _carsInfos.FirstOrDefault(x => x.Id == id);
            if (carsInfo != null)
            {
                carsInfo.CarOutTime = DateTime.Now;
                _carsInfos.Remove(carsInfo);
                _parkingInfoRepository.UpdateParkingSlotCount(carsInfo.CarParkingSlotType, false, carsInfo.CarParkingSlotNumber);
            }
            UpdateCarCollection(_carsInfos);
            return carsInfo;

        }

        public CarsInfo UpdateCarParkingStatus(CarsInfo carsInfo)
        {

            CarsInfo carInfo = _carsInfos.FirstOrDefault(x => x.Id == carsInfo.Id);

            if (carInfo != null)
            {
                carInfo.CarNumber = carsInfo.CarNumber;
                carInfo.CarOutTime = carsInfo.CarOutTime;
                carInfo.CarParkingSlotNumber = carsInfo.CarParkingSlotNumber;
                carInfo.CarParkingSlotType = carsInfo.CarParkingSlotType;
                carInfo.CarType = carsInfo.CarType;
            }
            UpdateCarCollection(_carsInfos);
            return carInfo;

        }

        public void UpdateCarCollection(List<CarsInfo> lstCarsInfo)
        {
            if (cache.Contains(carsCollectionKey))
            {
                cache.Remove(carsCollectionKey);
            }
            cache.Add(carsCollectionKey, lstCarsInfo, cacheItemPolicy);
        }
    }
}