using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Transcoder.Service.Services;

namespace Transcoder.WebApp.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var homeService = new HomeService();
            var isLogged = Request.Cookies["User"] != null  && Request.Cookies["UserID"] != null;
            if (!isLogged)
            {
                return View(homeService.GenerateHomeViewModel(isLogged));
            }
            else
            {
                int userId = int.Parse(Request.Cookies["UserID"].Value);
                return View(homeService.GetDataFromUserId(userId));
            }
            
        }

     
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}