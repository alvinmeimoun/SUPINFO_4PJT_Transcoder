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
            var isLogged = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            return View(homeService.GenerateHomeViewModel(isLogged));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}