using RealEstateWebUI.Areas.admin.Models;
using RealEstateWebUI.Areas.admin.UtilzGeneral;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;

namespace RealEstateWebUI.Areas.admin.Services
{
    public class SupplierService
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);
        public List<Supplier> GetAll()
        {
            try
            {
                string query = "select * from Supplier order by SupplierID";
                List<Supplier> lstSupplier = connect.Query<Supplier>(query).ToList<Supplier>();
                return lstSupplier;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return new List<Supplier>();
            }
        }
        public static object GetSupplierName(int? supplierID)
        {
            try
            {
                if (SNumber.ToNumber(supplierID) <= 0)
                {
                    return "";
                }
                IDbConnection connect = new SqlConnection(Common.ConnectString);
                string query = "select SupplierName from Supplier where SupplierID = " + supplierID.ToString();
                return connect.Query<string>(query).FirstOrDefault<string>();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return "";
            }
        }
    }
}