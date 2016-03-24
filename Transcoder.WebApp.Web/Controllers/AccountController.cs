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
using Core.Transcoder.DataAccess.ViewModels.User;
using Core.Transcoder.Utils.Resources;
using Vereyon.Web;

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
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            model.Password = string.IsNullOrWhiteSpace(model.Password) ? "" : EncryptionUtil.Encrypt(model.Password);
            var user = new USER_Service().LoginUser(model.Username, model.Password);
                  
            if(user != null)
            {
                SetCurrentUser(user.USERNAME, user.PK_ID_USER);
                FlashMessage.Confirmation(UiStrings.login_message_disconnected);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", UiStrings.login_error_invalid_connexion);
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
                USER_Service userService = new USER_Service();
                bool userAlreadyExist = userService.FindUserByUserName(model.Username) != null ||
                     userService.FindByEmail(model.Email) != null;
                if(userAlreadyExist)
                {
                    ModelState.AddModelError("", UiStrings.register_error_user_already_exists);
                    return View(model);
                }
                   
                model.Password = EncryptionUtil.Encrypt(model.Password);
                var user = new USER().CreateFromModel(model);
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

        [HttpPost]
        [AllowAnonymous]
        public ActionResult LoginOrRegisterExternal(LoginExternalViewModel model)
        {
            if (ModelState.IsValid)
            {
                USER_Service userService = new USER_Service();
                USER user = userService.FindByEmail(model.Email);

                if (user == null)
                {
                    user = new USER().CreateFromExternalLoginModel(model);

                    //Génération du username
                    var generatedUsername = user.FIRSTNAME.ToLower() + user.LASTNAME.ToLower();
                    var usernameAlreadyExistsWithSameBaseCount =
                        userService.FindAllByUsernameStartWith(generatedUsername).Count();
                    if (usernameAlreadyExistsWithSameBaseCount > 0)
                        generatedUsername += usernameAlreadyExistsWithSameBaseCount;
                    user.USERNAME = generatedUsername;

                    //Ajout en BDD
                    bool isRegistered = userService.AddOrUpdateUser(user);

                    if (isRegistered)
                    {
                        SetCurrentUser(user.USERNAME, user.PK_ID_USER);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    switch (model.ProviderType)
                    {
                        case LoginExternalViewModel.Provider.Google:
                            user.GOOGLEID = model.ProviderUserId;
                            break;
                        case LoginExternalViewModel.Provider.Facebook:
                            user.FACEBOOKID = model.ProviderUserId;
                            break;
                    }
                    userService.AddOrUpdateUser(user);

                    SetCurrentUser(user.USERNAME, user.PK_ID_USER);
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", UiStrings.login_error_auth);
            return View("Login", new LoginViewModel());
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