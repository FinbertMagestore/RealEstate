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
    public class ProductService
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);
        #region thao tac voi csdl
        public List<Product> GetAll(string orderby = "ModifiedDateTime", string sortOrder = "desc")
        {
            try
            {
                string query = string.Format("select * from Product order by {0} {1}", orderby, sortOrder);
                List<Product> lstProduct = connect.Query<Product>(query).ToList<Product>();
                return lstProduct;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return new List<Product>();
            }
        }
        public Product GetByPrimaryKey(int productID)
        {
            try
            {
                string query = "select * from Product where ProductID = " + SNumber.ToNumber(productID);
                Product product = connect.Query<Product>(query).FirstOrDefault<Product>();
                return product;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public List<Product> GetByWhere(string where, string orderby = "ModifiedDateTime")
        {
            try
            {
                string query = "";
                if (!string.IsNullOrEmpty(where))
                {
                    query = string.Format("select * from Product where " + where + " order by {0} desc", orderby);
                }
                else
                {
                    query = string.Format("select * from Product order by {0} desc", orderby);
                }
                return connect.Query<Product>(query).ToList<Product>();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return new List<Product>();
            }
        }

        public int Insert(Product product)
        {
            try
            {
                string query = "insert into Product(ProductName,ProductContent,ProductShortDescription," +
                        " ProductTitleCard,ProductDescriptionCard,ProductAlias,ProductState,SupplierID ," +
                        " ProductStyleID,ProductUrl,CreatedDateTime,ModifiedDateTime,Tags)" +
                        " values (@ProductName, @ProductContent, @ProductShortDescription,@ProductTitleCard,@ProductDescriptionCard," +
                        " @ProductAlias,@ProductState,@SupplierID,@ProductStyleID,@ProductUrl,@CreatedDateTime,@ModifiedDateTime,@Tags)" +
                        " SELECT @@IDENTITY";
                int productID = connect.Query<int>(query, new
                {
                    product.ProductName,
                    product.ProductContent,
                    product.ProductShortDescription,
                    product.ProductTitleCard,
                    product.ProductDescriptionCard,
                    product.ProductAlias,
                    product.ProductState,
                    product.SupplierID,
                    product.ProductStyleID,
                    product.ProductUrl,
                    product.CreatedDateTime,
                    product.ModifiedDateTime,
                    product.Tags
                }).Single();
                return productID;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }
        public bool Update(Product product)
        {
            try
            {
                string query = "update Product set ProductName = @productName, ProductContent = @productContent, ProductShortDescription = @productShortDescription," +
                    " ProductTitleCard=@ProductTitleCard,ProductDescriptionCard=@ProductDescriptionCard,ProductAlias=@ProductAlias," +
                    " ProductState = @productState, SupplierID = @supplierID, ProductStyleID = @productStyleID, ModifiedDateTime = @ModifiedDateTime, Tags = @Tags " +
                        " where ProductID = @productID ";
                return 0 < connect.Execute(query, new
                {
                    product.ProductName,
                    product.ProductContent,
                    product.ProductShortDescription,
                    product.ProductTitleCard,
                    product.ProductDescriptionCard,
                    product.ProductAlias,
                    product.ProductState,
                    product.SupplierID,
                    product.ProductStyleID,
                    product.ModifiedDateTime,
                    product.Tags,
                    productID = product.ProductID
                });

            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        public bool DeleteByPrimary(int productID)
        {
            try
            {
                if (SNumber.ToNumber(productID) <= 0)
                {
                    return false;
                }
                string query = "delete from Product where ProductID = " + productID;
                return 0 < connect.Execute(query);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        public static int CountAll()
        {
            IDbConnection connectDB = new SqlConnection(Common.ConnectString);
            try
            {
                string query = "select count(*) from Product";
                int countProduct = connectDB.Query<int>(query).Single();
                return countProduct;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }

        public List<Product> GetByWhere_Page(string where, string orderby, string sortOrder, int pageNum, int pageSize)
        {
            try
            {
                string procedureName = "Product_GetByWhere_Page";

                return connect.Query<Product>(procedureName,
                    commandType: CommandType.StoredProcedure,
                    param: new
                    {
                        where = where,
                        orderby = orderby,
                        sortOrder = sortOrder,
                        pageNum = pageNum,
                        pageSize = pageSize
                    }).ToList<Product>();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return new List<Product>();
            }
        }

        #endregion

        public List<Product> GetBestSelling()
        {
            try
            {
                string query = "select ProductID,count(ProductID) as NumberOrder into #temp from LineItem group by ProductID;";
                query += "SELECT Product.*, #temp.NumberOrder from product, #temp " +
                        " where Product.ProductID = #temp.ProductID " +
                        " order by #temp.NumberOrder desc;" +
                        " drop table #temp;";
                List<Product> products = connect.Query<Product>(query).ToList<Product>();
                return products;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return new List<Product>();
            }
        }
    }
}