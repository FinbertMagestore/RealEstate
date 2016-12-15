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
    public class CountryService
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);
        public List<Country> GetAll()
        {
            try
            {
                string query = "select * from Country order by CountryID";
                List<Country> customer = connect.Query<Country>(query).ToList<Country>();
                return customer;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return new List<Country>();
            }
        }

        public static object GetCountryName(int countryID)
        {
            IDbConnection sqlConnect = new SqlConnection(Common.ConnectString);
            try
            {
                if (SNumber.ToNumber(countryID) <= 0)
                {
                    return "";
                }
                string query = "select CountryName from Country where CountryID = " + countryID;
                return sqlConnect.Query<string>(query).FirstOrDefault<string>();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return "";
            }
        }

        public Country GetByPrimaryKey(int countryID)
        {
            try
            {
                if (SNumber.ToNumber(countryID) <= 0)
                {
                    return null;
                }
                string query = "select * from Country where CountryID = " + countryID;
                Country country = connect.Query<Country>(query).FirstOrDefault();
                return country;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }
    }
}