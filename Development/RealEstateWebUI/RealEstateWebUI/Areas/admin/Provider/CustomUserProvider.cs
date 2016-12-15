using RealEstateWebUI.Areas.admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;

using Dapper;
using RealEstateWebUI.Areas.admin.UtilzGeneral;
using RealEstateWebUI.Areas.admin.Services;

namespace RealEstateWebUI.Areas.admin.Provider
{
    public class CustomUserProvider : MembershipProvider
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);
        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion,
            string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {

            ValidatePasswordEventArgs args =
           new ValidatePasswordEventArgs(username, password, true);
            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (RequiresUniqueEmail && !string.IsNullOrEmpty(GetUserNameByEmail(email)))
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            MembershipUser user = GetUser(username, true);

            if (user == null)
            {
                try
                {

                    var userObj = new AppUser();
                    userObj.Username = username;
                    userObj.Password = GetMD5Hash(password);
                    userObj.Email = email;
                    userObj.DateCreated = DateTime.UtcNow;
                    userObj.LastActivityDate = DateTime.UtcNow;
                    userObj.SecurityStamp = Guid.NewGuid().ToString();

                    string query = "insert into AspUser(Username, Password, Email, DateCreated, LastActivityDate,SecurityStamp) values (@Username, @Password, @Email, @DateCreated, @LastActivityDate,@SecurityStamp) SELECT @@IDENTITY";
                    int appUserId = connect.Query<int>(query, new { userObj.Username, userObj.Password, userObj.Email, userObj.DateCreated, userObj.LastActivityDate, userObj.SecurityStamp }).Single();

                    status = MembershipCreateStatus.Success;

                    return GetUser(username, true);
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
            }
            else
            {
                status = MembershipCreateStatus.DuplicateUserName;
            }

            return null;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            try
            {
                if (!string.IsNullOrEmpty(username))
                {
                    string query = "select * from AppUsers where Username like N'" + username + "'";
                    var user = connect.Query<AppUser>(query).FirstOrDefault<AppUser>();

                    if (user != null)
                    {
                        MembershipUser memUser = new MembershipUser("CustomMembershipProvider", username, user.Id, user.Email, string.Empty, string.Empty,
                                                true, false, user.DateCreated, DateTime.MinValue, user.LastActivityDate ?? DateTime.MinValue, DateTime.UtcNow, DateTime.UtcNow);
                        return memUser;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }

            return null;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            try
            {
                if (providerUserKey != null)
                {
                    string query = "select * from AppUsers where Id =" + SNumber.ToNumber(providerUserKey) ;
                    var user = connect.Query<AppUser>(query).FirstOrDefault<AppUser>();

                    if (user != null)
                    {
                        MembershipUser memUser = new MembershipUser("CustomMembershipProvider", user.Username, user.Id, user.Email, string.Empty, string.Empty,
                                                true, false, user.DateCreated, DateTime.MinValue, user.LastActivityDate ?? DateTime.MinValue, DateTime.UtcNow, DateTime.UtcNow);
                        return memUser;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }

            return null;
        }

        public override string GetUserNameByEmail(string email)
        {
            try
            {
                string query = "select * from AppUsers where Email like N'" + email + "'";
                var user = connect.Query<AppUser>(query).FirstOrDefault<AppUser>();
                if (user != null)
                {
                    return user.Username;
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return "";
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return 0; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 0; }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    return false;
                }
                string sha1Pswd = GetMD5Hash(password);

                string query = "select * from AppUsers where Username like N'" + username + "' and Password like N'" + sha1Pswd + "'";
                var userObj = connect.Query<AppUser>(query).ToList<AppUser>();

                if (userObj != null && userObj.Count > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        public static string GetMD5Hash(string value)
        {
            try
            {
                MD5 md5Hasher = MD5.Create();
                byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(value));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return "";
            }
        }
    }
}