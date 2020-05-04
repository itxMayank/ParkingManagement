using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ParkingManagement.Models;

namespace ParkingManagement.Controllers
{
    public class HomeController : BaseController
    {
        readonly ICarRepository _carRepository;
        readonly IParkingInfoRepository _parkingInfoRepository;
        readonly IAdminTaskRepository _adminTaskRepository;

        public HomeController(ICarRepository carRepository, IAdminTaskRepository adminTaskRepository, IParkingInfoRepository parkingInfoRepository)
        {
            _carRepository = carRepository;
            _parkingInfoRepository = parkingInfoRepository;
            _adminTaskRepository = adminTaskRepository;
        }

        public ActionResult Index()
        {
            CarsInfo carsInfo = new CarsInfo
            {
                ParkingInfo = _parkingInfoRepository.GetTotalCountOfParkingTypeSlots()
            };
            SetValueToViewBags();
            carsInfo.IsParked = true;
            carsInfo.IsParkingSlotAvailable = true;
            return View(carsInfo);
        }

        [HttpPost]
        public ActionResult Index(CarsInfo model)
        {
            if (ModelState.IsValid)
            {
                CarsInfo carsInfo = _carRepository.AddCarToParking(model);
                SetValueToViewBags();
                carsInfo.IsParked = true;
                return View(carsInfo);
            }
            else
            {
                return View(model);
            }
        }

        public PartialViewResult GetParkedCars()
        {
            List<CarsInfo> lstCarsInfo = _carRepository.GetParkedCars();
            return PartialView("_ParkedVehicles", lstCarsInfo);
        }

        public JsonResult RemoveCarFromParking(int id, string slotType)
        {
            CarsInfo carInfo = _carRepository.RemoveCarFromParking(id);
            carInfo.ParkingInfo = _parkingInfoRepository.GetTotalCountOfParkingTypeSlots();
            return Json(carInfo, JsonRequestBehavior.AllowGet);
            //return carInfo;
        }

        public JsonResult GetParkedCar(int id)
        {
            CarsInfo info = _carRepository.GetCarById(id);

            return Json(info, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult AdminPanel(string carType = "")
        {
            AdminTask adminTask = _adminTaskRepository.GetSlotsType();
            return PartialView("_AdminPanel", adminTask);
        }

        [HttpPost]
        public JsonResult AddVehicleType(string vehicleType, Dictionary<string, int> dctVehicleTypeSlots)
        {
            Dictionary<string, Dictionary<string, int>> dctVSlotType = new Dictionary<string, Dictionary<string, int>>
            {
                { vehicleType, dctVehicleTypeSlots }
            };
            _parkingInfoRepository.AddCarWiseSlotType(dctVSlotType);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RemoveVehicleType(string carType)
        {
            _parkingInfoRepository.RemoveVehicleType(carType);
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        private void SetValueToViewBags()
        {
            List<string> lstCarTypes = _parkingInfoRepository.GetTypesOfCar();
            List<string> lstSlotTypes = _parkingInfoRepository.GetTypesOfParkingSlots();

            ViewBag.CarTypes = lstCarTypes.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();
            ViewBag.SlotTypes = lstSlotTypes.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();
        }

    }
}