using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using RealEstateWebUI.IdentityModels;
using System.Web.Security;

namespace RealEstateWebUI.Controllers
{
    public class IdentityAccountController : Controller
    {
        //
        // GET: /IdentityAccount/Login

        public ActionResult Login()
        {
            return View();
        }

        //
        // POST: /IdentityAccount/Login

        [HttpPost]
        public ActionResult Login(RealEstateWebUI.Models.LogOnModel model, string returnUrl)
        {
            var manager = HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            var user = manager.FindByEmail(model.Email);

            if (user != null)
            {
                if (model.Password.Equals(AppPasswordHasher.GetMD5Hash(user.Password)))
                {
                    // Set auth cookie for authenticated user
                    HttpContext.GetOwinContext().Authentication.SignIn(manager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie));
                    return RedirectToAction("index", "home", new { area = "client" });
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RealEstateWebUI.Models.RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                var manager = HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
                var user = new RealEstateWebUI.IdentityModels.AppUser() { Username = model.UserName, Email = model.Email };

                // Create user with supplied credentials
                var result = manager.Create<RealEstateWebUI.IdentityModels.AppUser, int>(user, model.Password);

                if (result == IdentityResult.Success)
                {
                    // On success set the sign in cookie
                    HttpContext.GetOwinContext().Authentication.SignIn(manager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie));
                    return RedirectToAction("index", "home", new { area = "client" });
                }
                else
                {
                    ModelState.AddModelError("", result.Errors.FirstOrDefault());
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            // Remove signin cookie
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("index", "home", new { area = "client" });
        }
    }
}
