using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Security.Cryptography;
using System.Text;
using Core.Transcoder.Service;
using System.Web.Security;
using Core.Transcoder.Service.Utils;
using Core.Transcoder.DataAccess.ViewModels;
using Core.Transcoder.DataAccess;

namespace Transcoder.WebApp.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
   

        public AccountController()
        {
        }


        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (returnUrl == "/Account/LogOff")
            {
               return RedirectToHomeAfterLogout();
            }
            
            return View();
        }

        private ActionResult RedirectToHomeAfterLogout()
        {
            if (Request.Cookies["User"] != null)
            {
                var c = new HttpCookie("User");
                c.Value = null;
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
            }
            return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            model.Password = EncryptionUtil.Encrypt(model.Password);
            var user = new USER_Service().LoginUser(model.Username, model.Password);
                  
            if(user != null)
            {
                SetCurrentUser(user.USERNAME, user.PK_ID_USER);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Tentative de connexion non valide.");
                return View(model);
            }
        }
        public void SetCurrentUser(string Username, int Pk_id_user)
        {
            HttpCookie Cookie = Request.Cookies["User"] ?? new HttpCookie("User");
            Cookie.Value = Username;
            
            Response.SetCookie(Cookie);

            HttpCookie CookieID = Request.Cookies["UserID"] ?? new HttpCookie("UserID");
            CookieID.Value = Pk_id_user.ToString();

            Response.SetCookie(CookieID);
        }


        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool usernameAlreadyExist = new USER_Service().FindUserByUserName(model.Username);
                if(usernameAlreadyExist)
                {
                    ModelState.AddModelError("", "Le pseudo " + model.Username + " existe déjà, veuillez en choisir un autre.");
                    return View(model);
                }
                   
                model.Password = EncryptionUtil.Encrypt(model.Password);
                var user = new USER();
                user.CreateFromModel(model);
                bool isRegistered = new USER_Service().AddOrUpdateUser(user);

                if (isRegistered)
                {
                    SetCurrentUser(user.USERNAME, user.PK_ID_USER);
                    return RedirectToAction("Index", "Home");
                }
               
            }

            // Si nous sommes arrivés là, un échec s’est produit. Réafficher le formulaire
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
           // var result = "";
            //await UserManager.ConfirmEmailAsync(userId, code);
            return View("Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

       

        //
        // POST: /Account/LogOff
        public ActionResult LogOff()
        {
            if (Request.Cookies["User"] != null)
            {
                var c = new HttpCookie("User");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
            }
            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

     

        #region Applications auxiliaires
        // Utilisé(e) pour la protection XSRF lors de l'ajout de connexions externes
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}