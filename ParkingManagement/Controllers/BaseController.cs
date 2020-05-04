using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ParkingManagement.Filter;

namespace ParkingManagement.Controllers
{
    [ExceptionFilter]
    [HandleError]
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //add code to that needs to be executed when any is executed
            //ex: use this code to check if the user is logged in if not navigate it to home/login page.
        }
    }
}