using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace ParkingManagement.Models
{
    public class ParkingInfoRepository : IParkingInfoRepository
    {
        readonly ObjectCache _cache = MemoryCache.Default;
        const string TotalCountKey = "CountsKey", CarTypes = "TypeOfCars", SlotTypes = "TypeOfSlots";
        readonly ParkingInfo _parkingInfo;
        Dictionary<string, int> _constSlotTypes;
        readonly CacheItemPolicy _cacheItemPolicy = new CacheItemPolicy
        {
            AbsoluteExpiration = DateTime.Now.AddHours(1.0)
        };

        public ParkingInfoRepository()
        {
            _parkingInfo = GetTotalCountOfParkingTypeSlots();
        }

        public ParkingInfo GetTotalCountOfParkingTypeSlots()
        {

            if (_cache.Contains(TotalCountKey))
            {
                return (ParkingInfo)_cache.Get(TotalCountKey);
            }
            else
            {
                Dictionary<string, int> slotTypes = new Dictionary<string, int>
                {
                    { "Small", 50 },
                    { "Medium", 30 },
                    { "Large", 10 }
                };

                ParkingInfo parkingInfo = new ParkingInfo
                {
                    TotalParkingSlots = 100,
                    SlotTypes = slotTypes
                };

                _cache.Add(TotalCountKey, parkingInfo, _cacheItemPolicy);
                return parkingInfo;
            }
        }

        public int GetTotalParkingSlotCount()
        {
            if (_parkingInfo != null)
            {
                return _parkingInfo.TotalParkingSlots;
            }
            else
            {
                return 0;
            }
        }

        public List<string> GetTypesOfCar(string carType = "")
        {
            List<string> lstCarTypes = new List<string>();

            if (_cache.Contains(CarTypes))
            {
                lstCarTypes = (List<string>)_cache.Get(CarTypes);
                if (carType != "")
                {
                    lstCarTypes.Add(carType);
                    _cache.Add(CarTypes, lstCarTypes, _cacheItemPolicy);
                }
                return lstCarTypes;
            }
            else
            {
                Dictionary<string, Dictionary<string, int>> parkingSlots = new Dictionary<string, Dictionary<string, int>>();
                parkingSlots = GetSetCarWiseSlotTypes("", true);

                if (parkingSlots != null && parkingSlots.Count > 0)
                {
                    lstCarTypes = parkingSlots.Keys.ToList();
                }
                else
                {
                    lstCarTypes = new List<string>
                    {
                        "Hatchback",
                        "Sedan/Compact SUV",
                        "SUV or Large cars"
                    };
                }

                if (carType != "")
                {
                    lstCarTypes.Add(carType);
                }

                _cache.Add(CarTypes, lstCarTypes, _cacheItemPolicy);
                return lstCarTypes;
            }
        }

        public List<string> GetTypesOfParkingSlots()
        {
            if (_cache.Contains(SlotTypes))
            {
                return (List<string>)_cache.Get(SlotTypes);
            }
            else
            {
                List<string> lstSlotTypes = new List<string>
                {
                    "Small",
                    "Medium",
                    "Large"
                };

                _cache.Add(SlotTypes, lstSlotTypes, _cacheItemPolicy);
                return lstSlotTypes;
            }
        }

        public ParkingInfo UpdateParkingSlotCount(string slotType, bool isParked, int slotNumber)
        {
            int slotCount = 0;

            if (_parkingInfo.SlotTypes != null && _parkingInfo.SlotTypes.ContainsKey(slotType))
            {
                _parkingInfo.SlotTypes.TryGetValue(slotType, out slotCount);

                if (isParked)
                {
                    slotCount -= 1;
                    _parkingInfo.TotalParkingSlots -= 1;
                }
                else
                {
                    slotCount += 1;
                    _parkingInfo.TotalParkingSlots += 1;
                }

                _parkingInfo.SlotTypes[slotType] = slotCount;
            }
            UpdateListOfParkingSlots(slotType, slotNumber, isParked);
            _cache.Add(TotalCountKey, _parkingInfo, _cacheItemPolicy);
            return _parkingInfo;
        }

        public ParkingInfo AddUpdateSlots(string slotName, int count)
        {
            Dictionary<string, int> dctParkingslotsTypes = _parkingInfo.SlotTypes;
            if (dctParkingslotsTypes == null)
            {
                dctParkingslotsTypes = new Dictionary<string, int>();
            }

            dctParkingslotsTypes.Add(slotName, count);
            _parkingInfo.TotalParkingSlots += count;
            _parkingInfo.SlotTypes = dctParkingslotsTypes;
            UpdateParkingInfo(_parkingInfo);
            return _parkingInfo;

        }

        public List<int> GetParkingSlotsList(string slotType)
        {
            string smallSlotNumbers = "SmallSlotNumbers", mediumSlotNumbers = "MediumSlotNumbers", largeSlotNumbers = "LargSlotNumbers";
            List<int> lstSlotsNumbers = new List<int>();

            if (slotType == "Small")
            {
                if (_cache.Contains(smallSlotNumbers))
                {
                    return (List<int>)_cache.Get(smallSlotNumbers);
                }
                else
                {
                    lstSlotsNumbers = Enumerable.Range(1, 50).ToList();
                    _cache.Add(smallSlotNumbers, lstSlotsNumbers, _cacheItemPolicy);
                    return lstSlotsNumbers;
                }
            }
            else if (slotType == "Medium")
            {
                if (_cache.Contains(mediumSlotNumbers))
                {
                    return (List<int>)_cache.Get(mediumSlotNumbers);
                }
                else
                {
                    lstSlotsNumbers = Enumerable.Range(1, 30).ToList();
                    _cache.Add(mediumSlotNumbers, lstSlotsNumbers, _cacheItemPolicy);
                    return lstSlotsNumbers;
                }
            }
            else if (slotType == "Large")
            {
                if (_cache.Contains(largeSlotNumbers))
                {
                    return (List<int>)_cache.Get(largeSlotNumbers);
                }
                else
                {
                    lstSlotsNumbers = Enumerable.Range(1, 10).ToList();
                    _cache.Add(largeSlotNumbers, lstSlotsNumbers, _cacheItemPolicy);
                    return lstSlotsNumbers;
                }
            }

            return lstSlotsNumbers;
        }

        public Dictionary<string, Dictionary<string, int>> GetSetCarWiseSlotTypes(string carType, bool ignoreCarType = false)
        {
            string carWiseSlot = "CarWiseSlot";
            Dictionary<string, Dictionary<string, int>> dctCarWiseSlots = new Dictionary<string, Dictionary<string, int>>();

            if (!ignoreCarType)
            {
                List<string> lstCarType = GetTypesOfCar();

                if (lstCarType != null && !lstCarType.Contains(carType) && !ignoreCarType)
                {
                    return dctCarWiseSlots;
                }
            }

            if (!_cache.Contains(carWiseSlot))
            {
                Dictionary<string, int> lstSlotTypes = new Dictionary<string, int>();

                dctCarWiseSlots.Add("Hatchback", lstSlotTypes = new Dictionary<string, int>
                {
                    { "Small", 0 },{"Medium", 0},{"Large", 10 }
                });

                dctCarWiseSlots.Add("Sedan/Compact SUV", lstSlotTypes = new Dictionary<string, int>
                {
                    {"Medium", 0},{"Large", 10 }
                });

                dctCarWiseSlots.Add("SUV or Large cars", lstSlotTypes = new Dictionary<string, int>
                {
                    { "Large", 0 }
                });

                _cache.Add(carWiseSlot, dctCarWiseSlots, _cacheItemPolicy);
                return dctCarWiseSlots;
            }
            else
            {
                return (Dictionary<string, Dictionary<string, int>>)_cache.Get(carWiseSlot);
            }

        }

        public void AddCarWiseSlotType(Dictionary<string, Dictionary<string, int>> slotTypes)
        {
            string carWiseSlot = "CarWiseSlot";
            if (slotTypes != null && slotTypes.Count > 0)
            {
                Dictionary<string, Dictionary<string, int>> addedSlotsTypes = GetSetCarWiseSlotTypes("", true);
                addedSlotsTypes.Add(slotTypes.FirstOrDefault().Key, slotTypes.FirstOrDefault().Value);
                GetTypesOfCar(slotTypes.FirstOrDefault().Key);
                _cache.Add(carWiseSlot, addedSlotsTypes, _cacheItemPolicy);
            }
        }

        public void UpdateParkingInfo(ParkingInfo parkingInfo)
        {
            _cache.Add(TotalCountKey, parkingInfo, _cacheItemPolicy);
        }

        public void UpdateListOfParkingSlots(string slotType, int slotNumber, bool isParked)
        {
            string smallSlotNumbers = "SmallSlotNumbers", mediumSlotNumbers = "MediumSlotNumbers", largeSlotNumbers = "LargSlotNumbers";
            List<int> lstParkingSlotNumbers = new List<int>();

            string cacheKeyToUpdate = "";

            if (slotType == "Small")
            {
                if (_cache.Contains(smallSlotNumbers))
                {
                    cacheKeyToUpdate = smallSlotNumbers;
                }
            }
            else if (slotType == "Medium")
            {
                if (_cache.Contains(mediumSlotNumbers))
                {
                    cacheKeyToUpdate = mediumSlotNumbers;
                }
            }
            else if (slotType == "Large")
            {
                if (_cache.Contains(largeSlotNumbers))
                {
                    cacheKeyToUpdate = largeSlotNumbers;
                }
            }

            if (!string.IsNullOrEmpty(cacheKeyToUpdate))
            {
                lstParkingSlotNumbers = (List<int>)_cache.Get(cacheKeyToUpdate);
                if (isParked)
                {
                    lstParkingSlotNumbers.Remove(slotNumber);
                }
                else
                {
                    lstParkingSlotNumbers.Add(slotNumber);
                }
                _cache.Add(cacheKeyToUpdate, lstParkingSlotNumbers, _cacheItemPolicy);
            }

        }

        public Dictionary<string, int> GetInitialSlotCount()
        {
            _constSlotTypes = new Dictionary<string, int>
                {
                    { "Small", 50 },
                    { "Medium", 30 },
                    { "Large", 10 }
                };
            return _constSlotTypes;
        }

        public void RemoveVehicleType(string carType)
        {
            string carWiseSlot = "CarWiseSlot";
            Dictionary<string, Dictionary<string, int>> dctCarWiseSlots = new Dictionary<string, Dictionary<string, int>>();

            List<string> lstVehicleType = GetTypesOfCar();
            lstVehicleType.Remove(carType);
            dctCarWiseSlots = GetSetCarWiseSlotTypes("", true);
            dctCarWiseSlots.Remove(carType);

            _cache.Add(CarTypes, lstVehicleType, _cacheItemPolicy);
            _cache.Add(carWiseSlot, dctCarWiseSlots, _cacheItemPolicy);
        }
    }
}