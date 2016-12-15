using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using RealEstateWebUI.IdentityModels;
using System;

namespace RealEstateWebUI
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            //app.CreatePerOwinContext<AppDbContext>(AppDbContext.Create);
            //app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);

            // Configure cookiemiddleware to set cookies for authenticated users
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/admin/authorization/login"),
                Provider = new CookieAuthenticationProvider()
                {
                    // Use the callback method to refresh the cookie after certain time period
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<AppUserManager, AppUser, int>(
                         validateInterval: TimeSpan.FromMinutes(int.MaxValue),
                         regenerateIdentityCallback: (manager, user) => user.GenerateUserIdentity(manager),
                         getUserIdCallback: (id) => (Int32.Parse(id.GetUserId())))
                }
            });

            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication();
        }
    }
}