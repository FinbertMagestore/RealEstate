using RealEstateWebUI.Areas.admin.Models;
using RealEstateWebUI.Areas.admin.UtilzGeneral;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using RealEstateWebUI.Areas.admin.Services;
using Microsoft.AspNet.Identity;

namespace RealEstateWebUI.Areas.admin.Controllers
{
    public class accountsController : Controller
    {
        /// <summary>
        /// variable save table name for write log
        /// </summary>
        private int TableNameID = (int)Common.TableName.Account;
        private AccountService accountService = new AccountService();
        private IDbConnection connect = new SqlConnection(Common.ConnectString);

        [Authorize(Roles = "Admin")]
        public ActionResult detail(int id)
        {
            try
            {
                AppUser user = accountService.GetByPrimaryKey(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                return View(user);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return RedirectToAction("index", "dashboard");
            }

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult detail([Bind(Include = "Id,Username,Email,UserPhone,MainPage,UserDescription,Password")] AppUser user,
            [Bind(Include = "ConfirmPassword")] string confirmPassword)
        {
            if (ModelState.IsValid || string.IsNullOrEmpty(user.Password)) 
            {
                ViewBag.Success = false;
                string strMessage = "";
                bool changePassword = false;
                try
                {
                    bool checkConfirmPasswordFlg = true;
                    if (!string.IsNullOrEmpty(confirmPassword) || !string.IsNullOrEmpty(user.Password))
                    {
                        if (!confirmPassword.Equals(user.Password))
                        {
                            checkConfirmPasswordFlg = false;
                            strMessage = "Mật khẩu mới không khớp nhau";
                        }
                    }
                    if (checkConfirmPasswordFlg)
                    {
                        AppUser appUser = accountService.GetByPrimaryKey(user.Id);
                        appUser.Username = user.Username;
                        appUser.Email = user.Email;
                        appUser.UserPhone = user.UserPhone;
                        appUser.MainPage = user.MainPage;
                        appUser.UserDescription = user.UserDescription;
                        appUser.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                        if (!string.IsNullOrEmpty(user.Password))
                        {
                            appUser.Password = IdentityModels.AppPasswordHasher.GetMD5Hash(user.Password);
                            changePassword = true;
                        }

                        if (accountService.Update(appUser))
                        {
                            LogService.WriteLog2DB(accountService.GetUserId(User.Identity.GetUserName()), (int)Common.ActionID.Update, appUser.Id, SDateTime.GetYYYYMMddHmmSSNow(), General.GetIPAddress(), TableNameID, appUser.Username);
                        }

                        strMessage = "Cập nhật thành công";
                        ViewBag.Success = true;
                    }
                }
                catch (Exception ex)
                {
                    strMessage = "Cập nhật không thành công";
                    changePassword = false;
                    LogService.WriteException(ex);
                }
                ViewBag.Message = strMessage;
                if (changePassword)
                {
                    return RedirectToAction("logout", "authorization");
                }
            }
            return View(user);
        }
    }
}