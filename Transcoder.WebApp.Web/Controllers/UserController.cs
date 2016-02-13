using Core.Transcoder.DataAccess;
using Core.Transcoder.DataAccess.ViewModels;
using Core.Transcoder.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Transcoder.WebApp.Web.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit()
        {
            int UserID = int.Parse(Request.Cookies["UserID"].Value);
            var model = new USER_Service().GetEditUserViewModelByID(UserID);
           
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(EditUserViewModel model)
        {

            if (ModelState.IsValid)
            {

                int UserID = int.Parse(Request.Cookies["UserID"].Value);
               
                var user = new USER_Service().FindUserByID(UserID);
                
                user.EditFromModel(model);
                //user.PK_ID_USER = UserID;

                bool isEdited = new USER_Service().AddOrUpdateUser(user);

                if (isEdited)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }
    }
}