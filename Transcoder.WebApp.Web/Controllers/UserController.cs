using Core.Transcoder.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Transcoder.WebApp.Web.Models.User;

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
            return View();
        }

        [HttpPost]
        public ActionResult Edit(EditUserViewModel model)
        {

            if (ModelState.IsValid)
            {
                bool usernameAlreadyExist = new USER_Service().FindUserByUserName(model.Username);
                if (usernameAlreadyExist)
                {
                    ModelState.AddModelError("", "Le pseudo " + model.Username + " existe déjà, veuillez en choisir un autre.");
                    return View(model);
                }

                var edit = model.EditFromModel();

                bool isEdited = new USER_Service().AddOrUpdateUser(edit);

                if (isEdited)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }
    }
}