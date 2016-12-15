using RealEstateWebUI.Areas.admin.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Security;

using Dapper;
using RealEstateWebUI.Areas.admin.Services;

namespace RealEstateWebUI.Areas.admin.Provider
{
    public class CustomRoleProvider : RoleProvider
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

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

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {

            try
            {
                string query = "select * from AppUsers where Username like N'" + username + "'";
                var user = connect.Query<AppUser>(query).FirstOrDefault<AppUser>();
                if (user == null)
                {
                    return null;
                }
                else
                {
                    query = "select Name from AspNetRoles where Id in (select RoleId from AspNetUserRoles where UserId = " + user.Id + ")";
                    string[] listRole = connect.Query<string>(query).ToArray<string>();
                    return listRole;
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
            //throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}