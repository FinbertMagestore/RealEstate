using RealEstateWebUI.Areas.admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using RealEstateWebUI.Areas.admin.UtilzGeneral;

namespace RealEstateWebUI.Areas.admin.Services
{
    public class DistrictService
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);
        public List<District> GetAll()
        {
            try
            {
                string query = "select * from District order by DistrictID";
                List<District> customer = connect.Query<District>(query).ToList<District>();
                return customer;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return new List<District>();
            }
        }

        public static object GetDistrictName(int districtID)
        {
            IDbConnection sqlConnect = new SqlConnection(Common.ConnectString);
            try
            {
                if (SNumber.ToNumber(districtID) <= 0)
                {
                    return "";
                }
                string query = "select DistrictName from District where DistrictID = " + districtID;
                return sqlConnect.Query<string>(query).FirstOrDefault<string>();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return "";
            }
        }

        public District GetByPrimaryKey(int districtID)
        {
            try
            {
                if (SNumber.ToNumber(districtID) <= 0)
                {
                    return null;
                }
                string query = "select * from District where DistrictID = " + districtID;
                District district = connect.Query<District>(query).FirstOrDefault();
                return district;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public List<District> GetByProvinceID(int provinceID)
        {
            try
            {
                if (SNumber.ToNumber(provinceID) <= 0)
                {
                    return null;
                }
                string query = "select * from District where ProvinceID = " + provinceID;
                List<District> districts = connect.Query<District>(query).ToList();
                return districts;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }
    }
}