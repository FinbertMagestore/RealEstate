using RealEstateWebUI.Areas.admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using RealEstateWebUI.Areas.admin.UtilzGeneral;

namespace RealEstateWebUI.Areas.admin.Services
{
    public class AccountService
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);
        public AppUser GetByPrimaryKey(int id)
        {
            try
            {
                string query = "select * from AppUsers where Id = " + id;
                AppUser appUser = connect.Query<AppUser>(query).FirstOrDefault();
                return appUser;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }
        public static string GetUsername(int id)
        {
            IDbConnection connectDB = new SqlConnection(Common.ConnectString);
            try
            {
                string query = "select Username from AppUsers where Id = " + id;
                string username = connectDB.Query<string>(query).FirstOrDefault();
                return SString.ConverToString(username);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return "";
            }
        }

        public int GetUserId(string userName)
        {
            IDbConnection connectDB = new SqlConnection(Common.ConnectString);
            try
            {
                string query = "select Id from AppUsers where Username like N'" + userName + "'";
                int userID = connectDB.Query<int>(query).FirstOrDefault<int>();
                return SNumber.ToNumber(userID);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }

        public bool Update(AppUser appUser)
        {
            try
            {
                string query = "update AppUsers set Username = @Username, Email = @Email, UserPhone = @UserPhone, MainPage = @MainPage, UserDescription = @UserDescription, Password = @Password where Id = @Id";
                int temp = connect.Execute(query, new { appUser.Username, appUser.Email, appUser.UserPhone, appUser.MainPage, appUser.UserDescription, appUser.Password, appUser.Id });
                if (temp > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
                return false;
        }

    }
}