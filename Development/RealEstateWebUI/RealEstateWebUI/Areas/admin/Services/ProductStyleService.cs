using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using RealEstateWebUI.Areas.admin.UtilzGeneral;
using RealEstateWebUI.Areas.admin.Models;
using System.Data.SqlClient;
using System.Data;
namespace RealEstateWebUI.Areas.admin.Services
{
    public class ProductStyleService
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);
        public List<ProductStyle> GetAll()
        {
            try
            {
                string query = "select * from ProductStyle order by ProductStyleID";
                List<ProductStyle> lstProductStyle = connect.Query<ProductStyle>(query).ToList<ProductStyle>();
                return lstProductStyle;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return new List<ProductStyle>();
            }
        }
        public static object GetProductStyleName(int? productStyleID)
        {
            try
            {
                if (SNumber.ToNumber(productStyleID) <= 0)
                {
                    return "";
                }
                IDbConnection connect = new SqlConnection(Common.ConnectString);
                string query = "select ProductStyleName from ProductStyle where ProductStyleID = " + productStyleID.ToString();
                return connect.Query<string>(query).FirstOrDefault<String>();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return "";
            }
        }
    }
}