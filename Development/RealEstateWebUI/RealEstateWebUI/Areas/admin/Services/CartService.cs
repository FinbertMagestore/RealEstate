using RealEstateWebUI.Areas.admin.Models;
using RealEstateWebUI.Areas.admin.UtilzGeneral;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using System.Text;
using System.Web;
using System.Web.Security;

namespace RealEstateWebUI.Areas.admin.Services
{
    public class CartService
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);
        
        #region thao tac voi csdl
        public List<TblCart> GetAll(string orderby = "ModifiedDateTime", string sortOrder = "desc")
        {
            try
            {
                string query = string.Format("select * from TblCart order by {0} {1}", orderby, sortOrder);
                List<TblCart> listCart = connect.Query<TblCart>(query).ToList<TblCart>();
                return listCart;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public TblCart GetByPrimaryKey(int cartID)
        {
            try
            {
                string query = "select * from TblCart where CartID = " + SNumber.ToNumber(cartID);
                TblCart cart = connect.Query<TblCart>(query).FirstOrDefault<TblCart>();
                return cart;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public TblCart GetByCookieID(string cookieID)
        {
            try
            {
                string query = string.Format("select * from TblCart where CookieID like N'{0}'", cookieID);
                TblCart cart = connect.Query<TblCart>(query).FirstOrDefault<TblCart>();
                return cart;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }


        public List<TblCart> GetByWhere(string where, string orderby = "ModifiedDateTime")
        {
            try
            {
                string query = "";
                if (!string.IsNullOrEmpty(where))
                {
                    query = string.Format("select * from TblCart where " + where + " order by {0} desc", orderby);
                }
                else
                {
                    query = string.Format("select * from TblCart order by {0} desc", orderby);
                }
                return connect.Query<TblCart>(query).ToList<TblCart>();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public int Insert(TblCart cart)
        {
            try
            {
                if (SNumber.ToNumber(cart.CustomerID) == 0)
                {
                    cart.CustomerID = null;
                }
                string query = "insert into TblCart (CookieID,CustomerID,CreatedDateTime,TotalPrice)" +
                        " values (@CookieID,@CustomerID,@CreatedDateTime,@TotalPrice)" +
                        " SELECT @@IDENTITY";
                int productID = connect.Query<int>(query, new
                {
                    cart.CookieID,
                    cart.CustomerID,
                    cart.CreatedDateTime,
                    cart.TotalPrice
                }).Single();
                return productID;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }

        public bool Update(TblCart cart)
        {
            try
            {
                if (SNumber.ToNumber(cart.CustomerID) == 0)
                {
                    cart.CustomerID = null;
                }
                string query = "update TblCart set CookieID=@CookieID,CustomerID=@CustomerID,ModifiedDateTime=@ModifiedDateTime,TotalPrice=@TotalPrice" +
                        " where CartID=@CartID ";
                return 0 < connect.Execute(query, new
                {
                    cart.CookieID,
                    cart.CustomerID,
                    cart.ModifiedDateTime,
                    cart.TotalPrice,
                    cart.CartID
                });

            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        public bool DeleteByPrimary(int cartID)
        {
            try
            {
                if (SNumber.ToNumber(cartID) <= 0)
                {
                    return false;
                }
                string query = "delete from TblCart where CartID = " + cartID;
                return 0 < connect.Execute(query);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        #endregion

        public bool UpdateTotalPrice(int cartID)
        {
            try
            {
                CartItemService cartItemService = new CartItemService();
                VariantService variantService = new VariantService();
                TblCart cart = GetByPrimaryKey(cartID);
                if (cart != null)
                {
                    List<TblCartItem> cartItems = cartItemService.GetByCartID(cartID);
                    decimal totalPrice = 0;
                    if (cartItems != null && cartItems.Count > 0)
                    {
                        foreach (var item in cartItems)
                        {
                            Variant variant = variantService.GetByPrimaryKey(item.VariantID);
                            if (variant != null)
                            {
                                totalPrice += (decimal)item.NumberVariant * (variant.VariantPrice != null ? variant.VariantPrice.Value : 0);
                            }
                        }
                    }
                    cart.TotalPrice = totalPrice;
                    if (Update(cart))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        public static int GetNumberQuantityInCart(string cookieID)
        {
            try
            {
                CartItemService cartItemService = new CartItemService();
                CartService cartSerice = new CartService();
                int quantity = 0;
                TblCart cart = cartSerice.GetByCookieID(cookieID);
                if (cart != null)
                {
                    List<TblCartItem> cartItems = cartItemService.GetByCartID(cart.CartID);
                    if (cartItems != null && cartItems.Count > 0)
                    {
                        foreach (var item in cartItems)
                        {
                            quantity += item.NumberVariant;
                        }
                    }
                }
                return quantity;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                throw;
            }
        }
    }
}