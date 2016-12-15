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
    public class ProvinceService
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);
        public List<Province> GetAll()
        {
            try
            {
                string query = "select * from Province order by ProvinceID";
                List<Province> customer = connect.Query<Province>(query).ToList<Province>();
                return customer;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return new List<Province>();
            }
        }

        //public static object GetProvinceName(int provinceID)
        //{
        //    IDbConnection sqlConnect = new SqlConnection(Common.ConnectString);
        //    try
        //    {
        //        if (SNumber.ToNumber(provinceID) <= 0)
        //        {
        //            return "";
        //        }
        //        string query = "select ProvinceName from Province where ProvinceID = " + provinceID;
        //        return sqlConnect.Query<string>(query).FirstOrDefault<string>();
        //    }
        //    catch (Exception ex)
        //    {
        //        LogService.WriteException(ex);
        //        return "";
        //    }
        //}

        public Province GetByPrimaryKey(int provinceID)
        {
            try
            {
                if (SNumber.ToNumber(provinceID) <= 0)
                {
                    return null;
                }
                string query = "select * from Province where ProvinceID = " + provinceID;
                Province province = connect.Query<Province>(query).FirstOrDefault();
                return province;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }
        public string GetProvinceName(int provinceID)
        {
            try
            {
                if (SNumber.ToNumber(provinceID) <= 0)
                {
                    return "";
                }
                string query = "select * from Province where ProvinceID = " + provinceID;
                Province province = connect.Query<Province>(query).FirstOrDefault();
                return province.ProvinceName;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
                return "";
        }
    }
}