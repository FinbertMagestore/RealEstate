using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using System.Data.SqlClient;
using RealEstateWebUI.Areas.admin.Models;
using RealEstateWebUI.Areas.admin.UtilzGeneral;

namespace RealEstateWebUI.Areas.admin.Services
{
    public class OptionService
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);

        public List<TblOption> GetAll()
        {
            try
            {
                string query = "select * from TblOption order by ModifiedDateTime desc";
                List<TblOption> TblOptions = connect.Query<TblOption>(query).ToList<TblOption>();
                return TblOptions;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public TblOption GetByPrimaryKey(int optionID)
        {
            try
            {
                string query = "select * from TblOption where OptionID = " + SNumber.ToNumber(optionID);
                TblOption TblOption = connect.Query<TblOption>(query).FirstOrDefault<TblOption>();
                return TblOption;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public List<TblOption> GetByProductID(int productID)
        {
            try
            {
                string query = "select * from TblOption where ProductID = " + SNumber.ToNumber(productID);
                List<TblOption> TblOptions = connect.Query<TblOption>(query).ToList<TblOption>();
                return TblOptions;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return new List<TblOption>();
            }
        }

        public TblOption GetOptionDefaultOfProduct(int productID)
        {
            try
            {
                TblOption optionDefault = null;
                List<TblOption> optionOfProduct = GetByProductID(productID);
                if (optionOfProduct != null && optionOfProduct.Count > 0)
                {
                    optionDefault = optionOfProduct[0];
                    for (int j = 0; j < optionOfProduct.Count; j++)
                    {
                        if (optionOfProduct[j].Position == 1 && optionOfProduct[j].OptionName == "Title" && optionOfProduct[j].OptionValue == "Default Title")
                        {
                            optionDefault = optionOfProduct[j];
                        }
                    }
                }
                return optionDefault;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public int Insert(TblOption option)
        {
            try
            {
                string query = "insert into TblOption(OptionName,Position,OptionValue," +
                        " ProductID,CreatedDateTime,ModifiedDateTime)" +
                        " values (@OptionName,@Position,@OptionValue,@ProductID,@CreatedDateTime,@ModifiedDateTime)" +
                        " SELECT @@IDENTITY";
                int optionID = connect.Query<int>(query, new
                {
                    option.OptionName,
                    option.Position,
                    option.OptionValue,
                    option.ProductID,
                    option.CreatedDateTime,
                    option.ModifiedDateTime
                }).Single();
                return optionID;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }

        public bool Update(TblOption option)
        {
            try
            {
                string query = "update TblOption set OptionName=@OptionName,Position=@Position,OptionValue=@OptionValue," +
                        " ProductID=@ProductID,ModifiedDateTime=@ModifiedDateTime " +
                        " where OptionID = @OptionID ";
                return 0 < connect.Execute(query, new
                {
                    option.OptionName,
                    option.Position,
                    option.OptionValue,
                    option.ProductID,
                    option.ModifiedDateTime,
                    option.OptionID
                });

            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        public List<TblOption> SelectByWhere(string where)
        {
            try
            {
                string query = "";
                if (!string.IsNullOrEmpty(where))
                {
                    query = "select * from TblOption where " + where + " order by ModifiedDateTime desc";
                }
                else
                {
                    query = "select * from TblOption order by ModifiedDateTime desc";
                }
                return connect.Query<TblOption>(query).ToList<TblOption>();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return new List<TblOption>();
            }
        }

        public bool DeleteByPrimary(int optionID)
        {
            try
            {
                if (SNumber.ToNumber(optionID) <= 0)
                {
                    return false;
                }
                string query = "delete from TblOption where OptionID = " + optionID;
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
                string query = "delete from TblOption where ProductID = " + productID;
                return 0 < connect.Execute(query);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        public string UpdateOptionOfProduct(int productID)
        {
            try
            {
                string result = "";
                List<Variant> variants = new VariantService().GetByProductID(productID);
                string optionValue1 = "", optionValue2 = "", optionValue3 = "";
                if (variants != null && variants.Count > 0)
                {
                    foreach (var item in variants)
                    {
                        if (!string.IsNullOrEmpty(item.Option1))
                        {
                            if (!optionValue1.Contains(item.Option1))
                            {
                                optionValue1 += item.Option1 + ",";
                            }
                        }
                        if (!string.IsNullOrEmpty(item.Option2))
                        {
                            if (!optionValue2.Contains(item.Option2))
                            {
                                optionValue2 += item.Option2 + ",";
                            }
                        }
                        if (!string.IsNullOrEmpty(item.Option3))
                        {
                            if (!optionValue3.Contains(item.Option3))
                            {
                                optionValue3 += item.Option3 + ",";
                            }
                        }
                    }
                    optionValue1 = SString.RemoveElementAtBeginEnd(optionValue1, ",");
                    optionValue2 = SString.RemoveElementAtBeginEnd(optionValue2, ",");
                    optionValue3 = SString.RemoveElementAtBeginEnd(optionValue3, ",");
                }
                List<TblOption> options = GetByProductID(productID);
                if (options != null && options.Count > 0)
                {
                    for (int i = 0; i < options.Count; i++)
                    {
                        if (i == 0)
                        {
                            options[i].OptionValue = optionValue1;
                        }
                        else if (i == 1)
                        {
                            options[i].OptionValue = optionValue2;
                        }
                        else if (i == 2)
                        {
                            options[i].OptionValue = optionValue3;
                        }
                        Update(options[i]);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return "";
            }
        }
    }
}