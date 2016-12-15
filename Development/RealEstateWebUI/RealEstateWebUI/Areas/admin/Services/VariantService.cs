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
    public class VariantService
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);
        public List<Variant> GetAll()
        {
            try
            {
                string query = "select * from Variant order by ModifiedDateTime desc";
                List<Variant> variants = connect.Query<Variant>(query).ToList<Variant>();
                return variants;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }
        public Variant GetByPrimaryKey(int variantID)
        {
            try
            {
                string query = "select * from Variant where VariantID = " + SNumber.ToNumber(variantID);
                Variant variant = connect.Query<Variant>(query).FirstOrDefault<Variant>();
                return variant;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public List<Variant> GetByProductID(int productID)
        {
            try
            {
                string query = "select * from Variant where ProductID = " + SNumber.ToNumber(productID);
                List<Variant> variants = connect.Query<Variant>(query).ToList<Variant>();
                return variants;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public List<Variant> GetByWhere(string where)
        {
            try
            {
                string query = "select * from Variant where " + where;
                List<Variant> variants = connect.Query<Variant>(query).ToList<Variant>();
                return variants;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public int Insert(Variant variant)
        {
            try
            {
                string query = "insert into Variant(Option1,Option2,Option3," +
                        " VariantPrice,Textable,CompareWithPrice,VariantBarcode,VariantSKU," +
                        " VariantWeight,WeightUnit,RequireShipping,ProductID,ImageID,CreatedDateTime,ModifiedDateTime,VariantTittle)" +
                        " values (@Option1,@Option2,@Option3,@VariantPrice,@Textable,@CompareWithPrice,@VariantBarcode,@VariantSKU," +
                        " @VariantWeight,@WeightUnit,@RequireShipping,@ProductID,@ImageID,@CreatedDateTime,@ModifiedDateTime,@VariantTittle)" +
                        " SELECT @@IDENTITY";
                int variantID = connect.Query<int>(query, new
                {
                    variant.Option1,
                    variant.Option2,
                    variant.Option3,
                    variant.VariantPrice,
                    variant.Textable,
                    variant.CompareWithPrice,
                    variant.VariantBarcode,
                    variant.VariantSKU,
                    variant.VariantWeight,
                    variant.WeightUnit,
                    variant.RequireShipping,
                    variant.ProductID,
                    variant.ImageID,
                    variant.CreatedDateTime,
                    variant.ModifiedDateTime,
                    variant.VariantTittle
                }).Single();
                return variantID;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }

        public bool Update(Variant variant)
        {
            try
            {
                string query = "update Variant set Option1=@Option1,Option2=@Option2,Option3=@Option3,VariantPrice=@VariantPrice,Textable=@Textable,"+
                        " CompareWithPrice=@CompareWithPrice,VariantBarcode=@VariantBarcode,VariantSKU=@VariantSKU," +
                        " VariantWeight=@VariantWeight,WeightUnit=@WeightUnit,RequireShipping=@RequireShipping,ProductID=@ProductID,ImageID=@ImageID,"+
                        " ModifiedDateTime=@ModifiedDateTime,VariantTittle=@VariantTittle" +
                        " where VariantID = @VariantID ";
                bool flg = 0 < connect.Execute(query, new
                {
                    variant.Option1,
                    variant.Option2,
                    variant.Option3,
                    variant.VariantPrice,
                    variant.Textable,
                    variant.CompareWithPrice,
                    variant.VariantBarcode,
                    variant.VariantSKU,
                    variant.VariantWeight,
                    variant.WeightUnit,
                    variant.RequireShipping,
                    variant.ProductID,
                    variant.ImageID,
                    variant.ModifiedDateTime,
                    variant.VariantTittle,
                    VariantID = variant.VariantID
                });
                return flg;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        public List<Variant> SelectByWhere(string where)
        {
            try
            {
                string query = "";
                if (!string.IsNullOrEmpty(where))
                {
                    query = "select * from Variant where " + where + " order by ModifiedDateTime desc";
                }
                else
                {
                    query = "select * from Variant order by ModifiedDateTime desc";
                }
                return connect.Query<Variant>(query).ToList<Variant>();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return new List<Variant>();
            }
        }

        public bool DeleteByPrimary(int variantID)
        {
            try
            {
                if (SNumber.ToNumber(variantID) <= 0)
                {
                    return false;
                }
                string query = "delete from Variant where VariantID = " + variantID;
                return 0 < connect.Execute(query);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        public bool DeleteByProductID(int productID)
        {
            try
            {
                string query = "delete from Variant where ProductID = " + productID;
                return 0 < connect.Execute(query);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        public bool DeleteByImageID(int imageID)
        {
            try
            {
                string query = "delete from Variant where ImageID = " + imageID;
                return 0 < connect.Execute(query);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        public bool DeleteByWhere(string where)
        {
            try
            {
                string query = "delete from Variant where " + where;
                return 0 < connect.Execute(query);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        public string GetVariantTittle(Variant variant)
        {
            try
            {
                string tittle = "";
                if (!string.IsNullOrEmpty(variant.Option1))
                {
                    tittle += variant.Option1;
                    if (!string.IsNullOrEmpty(variant.Option2))
                    {
                        tittle += " / " + variant.Option2;
                        if (!string.IsNullOrEmpty(variant.Option3))
                        {
                            tittle += " / " + variant.Option3;
                        }
                    }
                }
                return tittle;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return "";
            }
        }

        public int CountByProductID(int productID)
        {
            try
            {
                string query = "select count(*) from Variant where ProductID = " + productID;
                int result = connect.Query<int>(query).SingleOrDefault<int>();
                return result;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }

        public bool CheckVariantExist(Variant variant)
        {
            try
            {
                if (variant != null && variant.ProductID > 0)
                {
                    string query = "ProductID = " + variant.ProductID;
                    if (!string.IsNullOrEmpty(variant.Option1))
                    {
                        query += string.Format(" and Option1 like N'{0}'", variant.Option1);
                    }
                    if (!string.IsNullOrEmpty(variant.Option2))
                    {
                        query += string.Format(" and Option2 like N'{0}'", variant.Option2);
                    }
                    if (!string.IsNullOrEmpty(variant.Option3))
                    {
                        query += string.Format(" and Option3 like N'{0}'", variant.Option3);
                    }
                    List<Variant> variants = SelectByWhere(query);
                    if (variants != null && variants.Count > 0)
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
    }
}