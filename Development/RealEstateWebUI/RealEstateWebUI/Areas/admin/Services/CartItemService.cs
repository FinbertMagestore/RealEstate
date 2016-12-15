using Dapper;
using RealEstateWebUI.Areas.admin.Models;
using RealEstateWebUI.Areas.admin.UtilzGeneral;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace RealEstateWebUI.Areas.admin.Services
{
    public class CartItemService
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);
        #region thao tac voi csdl
        public List<TblCartItem> GetAll(string orderby = "ModifiedDateTime", string sortOrder = "desc")
        {
            try
            {
                string query = string.Format("select * from TblCartItem order by {0} {1}", orderby, sortOrder);
                List<TblCartItem> listCart = connect.Query<TblCartItem>(query).ToList<TblCartItem>();
                return listCart;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }
        public TblCartItem GetByPrimaryKey(int cartItemID)
        {
            try
            {
                string query = "select * from TblCartItem where CartItemID = " + SNumber.ToNumber(cartItemID);
                TblCartItem cart = connect.Query<TblCartItem>(query).FirstOrDefault<TblCartItem>();
                return cart;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public List<TblCartItem> GetByCartID(int cartID)
        {
            try
            {
                string query = "select * from TblCartItem where CartID = " + SNumber.ToNumber(cartID);
                List<TblCartItem> listCartItem = connect.Query<TblCartItem>(query).ToList<TblCartItem>();
                return listCartItem;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public List<TblCartItem> GetByWhere(string where, string orderby = "ModifiedDateTime")
        {
            try
            {
                string query = "";
                if (!string.IsNullOrEmpty(where))
                {
                    query = string.Format("select * from TblCartItem where " + where + " order by {0} desc", orderby);
                }
                else
                {
                    query = string.Format("select * from TblCartItem order by {0} desc", orderby);
                }
                return connect.Query<TblCartItem>(query).ToList<TblCartItem>();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public int Insert(TblCartItem cartItem)
        {
            try
            {
                string query = "insert into TblCartItem (VariantID,NumberVariant,CreatedDateTime,CartID)" +
                        " values (@VariantID,@NumberVariant,@CreatedDateTime,@CartID)" +
                        " SELECT @@IDENTITY";
                int productID = connect.Query<int>(query, new
                {
                    cartItem.VariantID,
                    cartItem.NumberVariant,
                    cartItem.CreatedDateTime,
                    cartItem.CartID
                }).Single();
                return productID;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }
        public bool Update(TblCartItem cart)
        {
            try
            {
                string query = "update TblCartItem set VariantID=@VariantID,NumberVariant=@NumberVariant,ModifiedDateTime=@ModifiedDateTime,CartID=@CartID" +
                        " where CartItemID=@CartItemID ";
                return 0 < connect.Execute(query, new
                {
                    cart.VariantID,
                    cart.NumberVariant,
                    cart.ModifiedDateTime,
                    cart.CartID,
                    cart.CartItemID
                });

            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        public bool DeleteByPrimary(int cartItemID)
        {
            try
            {
                if (SNumber.ToNumber(cartItemID) <= 0)
                {
                    return false;
                }
                string query = "delete from TblCartItem where CartItemID = " + cartItemID;
                return 0 < connect.Execute(query);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        public bool DeleteByCartID(int cartID)
        {
            try
            {
                if (SNumber.ToNumber(cartID) <= 0)
                {
                    return false;
                }
                string query = "delete from TblCartItem where CartID = " + cartID;
                return 0 < connect.Execute(query);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        #endregion

        public LineItem ToLineItem(TblCartItem item)
        {
            VariantService variantService = new VariantService();

            LineItem lineItem = new LineItem();
            lineItem.Quantity = item.NumberVariant;
            lineItem.Price = item.Variant.VariantPrice == null ? 0 : item.Variant.VariantPrice.Value;
            lineItem.ProductID = item.Variant.ProductID;
            lineItem.SKU = item.Variant.VariantSKU;
            lineItem.ProductName = item.Variant.Product == null ? "" : item.Variant.Product.ProductName;
            lineItem.VariantID = item.VariantID;
            lineItem.OrderID = item.OrderID;
            lineItem.ShippingStatus = null;
            List<Variant> variantsOfProduct = variantService.GetByProductID(item.Variant.ProductID);
            if (variantsOfProduct != null && variantsOfProduct.Count > 0)
            {
                if (variantsOfProduct.Count == 1 && variantsOfProduct[0].Option1 == "Default Title")
                {
                    lineItem.VariantName = "Default Title";
                }
                else
                {
                    lineItem.VariantName = item.Variant.VariantTittle;
                }
            }
            return lineItem;
        }
    }
}