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
    public class OrderService
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);
        public List<TblOrder> GetAll(string orderBy = "ModifiedDateTime")
        {
            try
            {
                string query = "select * from TblOrder order by "+orderBy+" desc";
                List<TblOrder> tblOrder = connect.Query<TblOrder>(query).ToList<TblOrder>();
                return tblOrder;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return new List<TblOrder>();
            }
        }
        public TblOrder GetByPrimaryKey(int orderID)
        {
            try
            {
                string query = "select * from TblOrder where OrderID = " + orderID;
                TblOrder tblOrder = connect.Query<TblOrder>(query).FirstOrDefault<TblOrder>();
                return tblOrder;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public List<TblOrder> SelectByWhere(string where, string orderBy = "ModifiedDateTime")
        {
            try
            {
                string query = "";
                if (!string.IsNullOrEmpty(where))
                {
                    query = "select * from TblOrder where " + where + " order by "+orderBy+" desc";
                }
                else
                {
                    query = "select * from TblOrder order by " + orderBy + " desc";
                }
                List<TblOrder> orders = connect.Query<TblOrder>(query).ToList<TblOrder>();
                return orders;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public bool DeleteByPrimary(int orderID)
        {
            try
            {
                if (SNumber.ToNumber(orderID) <= 0)
                {
                    return false;
                }
                string query = "delete from TblOrder where OrderID = " + orderID;
                return 0 < connect.Execute(query);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        public bool DeleteByCustomerID(int customerID)
        {
            try
            {
                string query = "delete from TblOrder where CustomerID = " + customerID;
                connect.Execute(query);
                return true;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        public List<TblOrder> GetByCustomerID(int customerID)
        {
            try
            {
                string query = "select * from TblOrder where CustomerID = " + customerID;
                List<TblOrder> lstTblOrder = connect.Query<TblOrder>(query).ToList<TblOrder>();
                return lstTblOrder;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public static int CountAll()
        {
            IDbConnection connectDB = new SqlConnection(Common.ConnectString);
            try
            {
                string query = "select count(*) from TblOrder";
                int countTblOrder = connectDB.Query<int>(query).Single();
                return countTblOrder;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }

        public int CounByWhere(string where)
        {
            try
            {
                IDbConnection connectDB = new SqlConnection(Common.ConnectString);
                string query = "select count(*) from TblOrder where " + where;
                int temp = connectDB.Query<int>(query).FirstOrDefault<int>();
                return SNumber.ToNumber(temp);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }

        public int Insert(TblOrder order)
        {
            try
            {
                string query = "insert into TblOrder(OrderNote,OrderStatus,CustomerEmail," +
                        " CustomerID,CreatedDateTime,ModifiedDateTime,TotalCount,TotalShipping,ShippingStatus," +
                        " BillingStatus,Number,ShippingAddressID,BillingAddressID,Tags)" +
                        " values (@OrderNote,@OrderStatus,@CustomerEmail,@CustomerID,@CreatedDateTime,@ModifiedDateTime,@TotalCount,@TotalShipping,@ShippingStatus," +
                        " @BillingStatus,@Number,@ShippingAddressID,@BillingAddressID,@Tags)" +
                        " SELECT @@IDENTITY";
                int orderID = connect.Query<int>(query, new
                {
                    order.OrderNote,
                    order.OrderStatus,
                    order.CustomerEmail,
                    order.CustomerID,
                    order.CreatedDateTime,
                    order.ModifiedDateTime,
                    order.TotalCount,
                    order.TotalShipping,
                    order.ShippingStatus,
                    order.BillingStatus,
                    order.Number,
                    order.ShippingAddressID,
                    order.BillingAddressID,
                    order.Tags
                }).Single();
                return orderID;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }

        public bool Update(TblOrder order)
        {
            try
            {
                string query = "update TblOrder set OrderNote = @OrderNote, OrderStatus = @OrderStatus, CustomerEmail = @CustomerEmail," +
                    " CustomerID=@CustomerID,ModifiedDateTime=@ModifiedDateTime,TotalCount=@TotalCount,TotalShipping=@TotalShipping," +
                    " ShippingStatus = @ShippingStatus, BillingStatus = @BillingStatus, Number = @Number, ShippingAddressID = @ShippingAddressID, BillingAddressID = @BillingAddressID, Tags = @Tags " +
                        " where OrderID = @OrderID ";
                return 0 < connect.Execute(query, new
                {
                    order.OrderNote,
                    order.OrderStatus,
                    order.CustomerEmail,
                    order.CustomerID,
                    order.ModifiedDateTime,
                    order.TotalCount,
                    order.TotalShipping,
                    order.ShippingStatus,
                    order.BillingStatus,
                    order.Number,
                    order.ShippingAddressID,
                    order.BillingAddressID,
                    order.Tags,
                    OrderID = order.OrderID
                });

            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        public int GetLastNumber()
        {
            try
            {
                string query = "select * from TblOrder order by Number";
                List<TblOrder> orders = connect.Query<TblOrder>(query).ToList<TblOrder>();
                if (orders != null && orders.Count > 0)
                {
                    return orders[orders.Count - 1].Number;
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return 0;
        }

        public int GetQuantityInOrder(int orderID)
        {
            try
            {
                LineItemService lineItemService = new LineItemService();
                int quantity = 0;
                TblOrder order = GetByPrimaryKey(orderID);
                if (order != null)
                {
                    List<LineItem> cartItems = lineItemService.GetByOrderID(orderID);
                    if (cartItems != null && cartItems.Count > 0)
                    {
                        foreach (var item in cartItems)
                        {
                            quantity += item.Quantity;
                        }
                    }
                }
                return quantity;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }
    }

    public class BillingAddressService
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);
        public BillingAddress GetByPrimaryKey(int id)
        {
            try
            {
                string query = "select * from BillingAddress where BillingAddressID = " + id;
                BillingAddress billingAddress = connect.Query<BillingAddress>(query).FirstOrDefault();
                return billingAddress;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public int Insert(BillingAddress billingAddress)
        {
            try
            {
                string query = "insert into BillingAddress(Phone,DistrictID,DistrictName," +
                        " ProvinceID,ProvinceName, CountryID, CountryName,CustomerName,HomeAddress) " +
                        " values (@Phone,@DistrictID,@DistrictName," +
                        " @ProvinceID,@ProvinceName,@CountryID,@CountryName,@CustomerName,@HomeAddress)" +
                        " SELECT @@IDENTITY";
                int billingAddressID = connect.Query<int>(query, new
                {
                    billingAddress.Phone,
                    billingAddress.DistrictID,
                    billingAddress.DistrictName,
                    billingAddress.ProvinceID,
                    billingAddress.ProvinceName,
                    billingAddress.CountryID,
                    billingAddress.CountryName,
                    billingAddress.CustomerName,
                    billingAddress.HomeAddress
                }).Single();
                return billingAddressID;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }
        public bool Update(BillingAddress billingAddress)
        {
            try
            {
                string query = "update BillingAddress set Phone=@Phone, " +
                        " ProvinceName=@ProvinceName, CountryID=@CountryID, CountryName=@CountryName,CustomerName=@CustomerName,HomeAddress=@HomeAddress " +
                        " where BillingAddressID = @BillingAddressID";
                return 0 < connect.Execute(query, new
                {
                    billingAddress.Phone,
                    billingAddress.ProvinceName,
                    billingAddress.CountryID,
                    billingAddress.CountryName,
                    billingAddress.CustomerName,
                    billingAddress.HomeAddress,
                    billingAddress.BillingAddressID
                });
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }
        public List<BillingAddress> SelectByWhere(string where)
        {
            try
            {
                string query = "";
                if (!string.IsNullOrEmpty(where))
                {
                    query = "select * from BillingAddress where " + where;
                }
                else
                {
                    query = "select * from BillingAddress";
                }
                return connect.Query<BillingAddress>(query).ToList<BillingAddress>();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public bool DeleteByPrimary(int billingAddressID)
        {
            try
            {
                if (SNumber.ToNumber(billingAddressID) <= 0)
                {
                    return false;
                }
                string query = "delete from BillingAddress where BillingAddressID = " + billingAddressID;
                return 0 < connect.Execute(query);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }
    }

    public class ShippingAddressService
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);
        public ShippingAddress GetByPrimaryKey(int id)
        {
            try
            {
                string query = "select * from ShippingAddress where ShippingAddressID = " + id;
                ShippingAddress shippingAddress = connect.Query<ShippingAddress>(query).FirstOrDefault();
                return shippingAddress;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public int Insert(ShippingAddress shippingAddress)
        {
            try
            {
                string query = "insert into ShippingAddress(Phone," +
                        " ProvinceName, CountryID, CountryName,CustomerName,HomeAddress) " +
                        " values (@Phone," +
                        " @ProvinceName,@CountryID,@CountryName,@CustomerName,@HomeAddress)" +
                        " SELECT @@IDENTITY";
                int lineItemID = connect.Query<int>(query, new
                {
                    shippingAddress.Phone,
                    shippingAddress.ProvinceName,
                    shippingAddress.CountryID,
                    shippingAddress.CountryName,
                    shippingAddress.CustomerName,
                    shippingAddress.HomeAddress
                }).Single();
                return lineItemID;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }
        public bool Update(ShippingAddress ShippingAddress)
        {
            try
            {
                string query = "update ShippingAddress set Phone=@Phone, " +
                        " ProvinceName=@ProvinceName, CountryID=@CountryID, CountryName=@CountryName,CustomerName=@CustomerName,HomeAddress=@HomeAddress " +
                        " where ShippingAddressID = @ShippingAddressID";
                return 0 < connect.Execute(query, new
                {
                    ShippingAddress.Phone,
                    ShippingAddress.ProvinceName,
                    ShippingAddress.CountryID,
                    ShippingAddress.CountryName,
                    ShippingAddress.CustomerName,
                    ShippingAddress.HomeAddress,
                    ShippingAddress.ShippingAddressID
                });
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }
        public List<ShippingAddress> SelectByWhere(string where)
        {
            try
            {
                string query = "";
                if (!string.IsNullOrEmpty(where))
                {
                    query = "select * from ShippingAddress where " + where;
                }
                else
                {
                    query = "select * from ShippingAddress";
                }
                return connect.Query<ShippingAddress>(query).ToList<ShippingAddress>();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public bool DeleteByPrimary(int ShippingAddressID)
        {
            try
            {
                if (SNumber.ToNumber(ShippingAddressID) <= 0)
                {
                    return false;
                }
                string query = "delete from ShippingAddress where ShippingAddressID = " + ShippingAddressID;
                return 0 < connect.Execute(query);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }
    }

    public class LineItemService
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);
        public LineItem GetByPrimaryKey(int id)
        {
            try
            {
                string query = "select * from LineItem where LineItemID = " + id;
                LineItem lineItem = connect.Query<LineItem>(query).FirstOrDefault();
                return lineItem;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public List<LineItem> GetByOrderID(int orderID)
        {
            try
            {
                string query = "select * from LineItem where OrderID = " + orderID;
                List<LineItem> lineItems = connect.Query<LineItem>(query).ToList();
                return lineItems;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public int Insert(LineItem lineItem)
        {
            try
            {
                string query = "insert into LineItem(Quantity,Price,ProductID," +
                        " SKU, ProductName, VariantID,VariantName,OrderID) " +
                        " values (@Quantity,@Price,@ProductID," +
                        " @SKU, @ProductName, @VariantID,@VariantName,@OrderID)" +
                        " SELECT @@IDENTITY";
                int lineItemID = connect.Query<int>(query, new
                {
                    lineItem.Quantity,
                    lineItem.Price,
                    lineItem.ProductID,
                    lineItem.SKU,
                    lineItem.ProductName,
                    lineItem.VariantID,
                    lineItem.VariantName,
                    lineItem.OrderID
                }).Single();
                return lineItemID;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }
        public bool Update(LineItem lineItem)
        {
            try
            {
                string query = "update LineItem set Quantity=@Quantity, Price=@Price,ProductID=@ProductID, " +
                        " SKU=@SKU, ProductName=@ProductName, VariantID=@VariantID,VariantName=@VariantName,"+
                        " OrderID=@OrderID, ShippingStatus=@ShippingStatus " +
                        " where LineItemID = @LineItemID";
                return 0 < connect.Execute(query, new
                {
                    lineItem.Quantity,
                    lineItem.Price,
                    lineItem.ProductID,
                    lineItem.SKU,
                    lineItem.ProductName,
                    lineItem.VariantID,
                    lineItem.VariantName,
                    lineItem.OrderID,
                    lineItem.ShippingStatus,
                    lineItem.LineItemID
                });
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }
        public List<LineItem> SelectByWhere(string where)
        {
            try
            {
                string query = "";
                if (!string.IsNullOrEmpty(where))
                {
                    query = "select * from LineItem where " + where;
                }
                else
                {
                    query = "select * from LineItem";
                }
                return connect.Query<LineItem>(query).ToList<LineItem>();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public bool DeleteByPrimary(int LineItemID)
        {
            try
            {
                if (SNumber.ToNumber(LineItemID) <= 0)
                {
                    return false;
                }
                string query = "delete from LineItem where LineItemID = " + LineItemID;
                return 0 < connect.Execute(query);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }
    }
}