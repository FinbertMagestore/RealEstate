using Microsoft.AspNet.Identity;
using RealEstateWebUI.Areas.admin.Models;
using RealEstateWebUI.Areas.admin.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RealEstateWebUI.Areas.admin.Controllers
{
    public class authorizationController : Controller
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);
        [AllowAnonymous]
        public ActionResult login(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            try
            {
                HttpCookie myCookie = Request.Cookies[".ASPXAUTH"];

                if (myCookie != null)
                {
                    if (!string.IsNullOrEmpty(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("index", "dashboard");
                    }
                }
                return View();
            }
            catch
            {
                ViewBag.Fail = "Không thể kết nối tới server";
                return View();
            }

        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult login(LogOnModel model, string ReturnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(model.Email))
                    {
                        string userName = Membership.GetUserNameByEmail(model.Email);
                        if (!string.IsNullOrEmpty(userName))
                        {
                            if (Membership.ValidateUser(userName, model.Password))
                            {
                                FormsAuthentication.SetAuthCookie(userName, model.RememberMe);

                                if (!string.IsNullOrEmpty(ReturnUrl))
                                {
                                    return Redirect(ReturnUrl);
                                }
                                else
                                {
                                    return RedirectToAction("index", "dashboard");
                                }
                            }
                            else
                            {
                                ViewBag.Fail = "Mật khẩu không đúng.";
                                //ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng.");
                            }
                        }
                        else
                        {
                            ViewBag.Fail = "Email không tồn tại.";
                            //ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không tồn tại.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Fail = "Không thể kết nối tới server";
                LogService.WriteException(ex);
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("login", "authorization");
        }
        [AllowAnonymous]
        public ActionResult index()
        {
            return RedirectToAction("login", "authorization");
        }
    }
}