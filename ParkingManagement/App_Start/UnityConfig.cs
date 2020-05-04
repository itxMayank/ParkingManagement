using System.Web.Mvc;
using ParkingManagement.Models;
using Unity;
using Unity.Mvc5;

namespace ParkingManagement
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<ICarRepository, MockCarRepository>();
            container.RegisterType<IParkingInfoRepository, ParkingInfoRepository>();
            container.RegisterType<IAdminTaskRepository, AdminTaskRepository>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}