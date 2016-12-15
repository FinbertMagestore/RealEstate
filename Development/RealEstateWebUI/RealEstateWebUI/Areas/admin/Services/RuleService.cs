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
    public class RuleService
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);
        public List<TblRule> GetAll()
        {
            try
            {
                string query = "select * from TblRule order by RuleID";
                List<TblRule> lstTblRule = connect.Query<TblRule>(query).ToList<TblRule>();
                return lstTblRule;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public TblRule GetByPrimaryKey(int ruleID)
        {
            try
            {
                string query = "select * from TblRule where RuleID = " + SNumber.ToNumber(ruleID);
                TblRule tblRule = connect.Query<TblRule>(query).FirstOrDefault<TblRule>();
                return tblRule;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public int Insert(TblRule tblRule)
        {
            try
            {
                string query = "insert into TblRule(CollectionID,ColumnName,Relation,ConditionValue)" +
                        " values (@CollectionID,@ColumnName,@Relation,@ConditionValue)" +
                        " SELECT @@IDENTITY";
                int tblRuleID = connect.Query<int>(query, new { tblRule.CollectionID, tblRule.ColumnName, tblRule.Relation, tblRule.ConditionValue}).Single();
                return tblRuleID;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }
        public bool Update(TblRule tblRule)
        {
            try
            {
                string query = "update TblRule set CollectionID=@CollectionID,ColumnName=@ColumnName,Relation=@Relation," +
                        " ConditionValue=@ConditionValue where RuleID = @RuleID";
                int temp = connect.Execute(query, new { tblRule.CollectionID, tblRule.ColumnName, tblRule.Relation, tblRule.ConditionValue, tblRule.RuleID });
                return temp > 0;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }
        public List<TblRule> SelectByWhere(string where)
        {
            try
            {
                string query = "";
                if (!string.IsNullOrEmpty(where))
                {
                    query = "select * from TblRule where " + where; 
                }
                else
                {
                    query = "select * from TblRule";
                }
                return connect.Query<TblRule>(query).ToList<TblRule>();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }


        public List<TblRule> SelectByCollectionID(int collectionID)
        {
            try
            {
                string query = "select * from TblRule where CollectionID = "+  collectionID;
                return connect.Query<TblRule>(query).ToList<TblRule>();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return new List<TblRule>();
            }
        }

        public bool DeleteByPrimary(int ruleID)
        {
            try
            {
                if (SNumber.ToNumber(ruleID) <= 0)
                {
                    return false;
                }
                string query = "delete from TblRule where RuleID = " + ruleID;
                return 0 < connect.Execute(query);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }
        public bool DeleteByCollectionID(int collectionID)
        {
            try
            {
                if (SNumber.ToNumber(collectionID) <= 0)
                {
                    return false;
                }
                string query = "delete from TblRule where CollectionID = " + collectionID;
                return 0 < connect.Execute(query);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        /// <summary>
        /// check all information of rule exist
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public bool CheckRuleValid(TblRule rule)
        {
            try
            {
                if (string.IsNullOrEmpty(rule.ColumnName) || string.IsNullOrEmpty(rule.Relation) || string.IsNullOrEmpty(rule.ConditionValue))
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                throw;
            }
        }

        /// <summary>
        /// compare 2 TblRule if same ColumnName, Relation, Condition, CollectionID
        /// </summary>
        /// <param name="rule1"></param>
        /// <param name="rule2"></param>
        /// <returns></returns>
        public bool Equals(TblRule rule1, TblRule rule2)
        {
            try
            {
                if (CheckRuleValid(rule1) && CheckRuleValid(rule2))
                {
                    if (rule1.ColumnName.Equals(rule2.ColumnName))
                    {
                        if (rule1.Relation.Equals(rule2.Relation))
                        {
                            if (rule1.ConditionValue.Equals(rule2.ConditionValue))
                            {
                                if (rule1.CollectionID == rule2.CollectionID)
                                {
                                    return true;
                                }
                            }
                        }
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
        
        /// <summary>
        /// get condition with a rule to select list product in collection
        /// </summary>
        /// <param name="rule">rule will gen conditon</param>
        /// <returns>condition find</returns>
        public static string GetConditionProductByARule(TblRule rule)
        {
            try
            {
                string condition = "";
                if (!string.IsNullOrEmpty(rule.ColumnName) && !string.IsNullOrEmpty(rule.Relation) && !string.IsNullOrEmpty(rule.ConditionValue))
                {
                    switch (rule.ColumnName)
                    {
                        case "ProductName":
                            switch (rule.Relation)
                            {
                                case "equals":
                                    condition = string.Format(" ProductName like N'{0}' ", rule.ConditionValue);
                                    break;
                                case "contains":
                                    condition = string.Format(" ProductName like N'%{0}%' ", rule.ConditionValue);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "ProductStyle":
                            switch (rule.Relation)
                            {
                                case "equals":
                                    condition = string.Format(" ProductStyleID in (select ProductStyleID from ProductStyle where ProductStyleName like N'{0}') ", rule.ConditionValue);
                                    break;
                                case "contains":
                                    condition = string.Format(" ProductStyleID in (select ProductStyleID from ProductStyle where ProductStyleName like N'%{0}%') ", rule.ConditionValue);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "Supplier":
                            switch (rule.Relation)
                            {
                                case "equals":
                                    condition = string.Format(" SupplierID in (select SupplierID from Supplier where SupplierName like N'{0}') ", rule.ConditionValue);
                                    break;
                                case "contains":
                                    condition = string.Format(" SupplierID in (select SupplierID from Supplier where SupplierName like N'%{0}%') ", rule.ConditionValue);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
                return condition;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return "";
            }
        }
        /// <summary>
        /// get condition with a list rule to select list product in collection
        /// </summary>
        /// <param name="listRule">list rule will gen conditon</param>
        /// <param name="linkCondition">condition link between rules: and|or</param>
        /// <returns>condition find</returns>
        public string GetConditionProductByListRule(List<TblRule> listRule, string linkCondition)
        {
            try
            {
                string condition = "";
                if (listRule != null && listRule.Count > 0)
                {
                    for (int i = 0; i < listRule.Count; i++)
                    {
                        string temp = GetConditionProductByARule(listRule[i]);
                        if (!string.IsNullOrEmpty(temp))
                        {
                            if (i == 0)
                            {
                                condition += temp;
                            }
                            else
                            {
                                condition += " " + linkCondition + " " + temp + " ";
                            }
                        }
                    }
                }
                return condition;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return "";
            }
        }

        /// <summary>
        /// get string contains condition for a rule
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static string GetCondition(TblRule rule)
        {
            try
            {
                string result = "";

                if (!string.IsNullOrEmpty(rule.ColumnName) && !string.IsNullOrEmpty(rule.Relation) && !string.IsNullOrEmpty(rule.ConditionValue))
                {
                    switch (rule.ColumnName)
                    {
                        case "ProductName":
                            result += "Tên sản phẩm ";
                            break;
                        case "ProductStyle":
                            result += "Loại sản phẩm ";
                            break;
                        case "Supplier":
                            result += "Nhà sản xuất ";
                            break;
                        default:
                            break;
                    }
                    switch (rule.Relation)
                    {
                        case "equals":
                            result = "bằng ";
                            break;
                        case "contains":
                            result += "chứa ";
                            break;
                        default:
                            break;
                    }
                    result += rule.ConditionValue;
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