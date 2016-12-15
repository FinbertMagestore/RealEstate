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
    public class CollectionService
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);
        private RuleService tblRuleService = new RuleService();

        public List<Collection> GetAll()
        {
            try
            {
                string query = "select * from Collection order by ModifiedDateTime desc";
                List<Collection> collections = connect.Query<Collection>(query).ToList<Collection>();
                if (collections != null && collections.Count > 0)
                {
                    for (int i = 0; i < collections.Count; i++)
                    {
                        collections[i].TblRules = tblRuleService.SelectByCollectionID(collections[i].CollectionID);
                    }
                }
                return collections;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return new List<Collection>();
            }
        }

        public Collection GetByPrimaryKey(int collectionID)
        {
            try
            {
                string query = "select * from Collection where CollectionID = " + SNumber.ToNumber(collectionID);
                Collection collection = connect.Query<Collection>(query).FirstOrDefault<Collection>();
                if (collection != null)
                {
                    collection.TblRules = tblRuleService.SelectByCollectionID(collectionID);
                }
                return collection;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public List<Collection> GetByWhere(string where, string orderby = "ModifiedDateTime")
        {
            try
            {
                string query = "";
                if (!string.IsNullOrEmpty(where))
                {
                    query = string.Format("select * from Collection where " + where + " order by {0} desc", orderby);
                }
                else
                {
                    query = string.Format("select * from Collection order by {0} desc", orderby);
                }
                return connect.Query<Collection>(query).ToList<Collection>();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public int Insert(Collection collection)
        {
            try
            {
                string query = "insert into Collection(CollectionName,CollectionDescription,PageTitle,PageDescription," +
                    " CollectionState,CollectionImage,CollectionType,CreatedDateTime,ModifiedDateTime," +
                    " UrlAlias,TemplateLayout,ConditionForCollection)" +
                        " values (@CollectionName,@CollectionDescription,@PageTitle,@PageDescription," +
                        " @CollectionState,@CollectionImage,@CollectionType,@CreatedDateTime,@ModifiedDateTime," +
                        " @UrlAlias,@TemplateLayout,@ConditionForCollection)" +
                        " SELECT @@IDENTITY";
                int collectionID = connect.Query<int>(query, new
                {
                    collection.CollectionName,
                    collection.CollectionDescription,
                    collection.PageTitle,
                    collection.PageDescription,
                    collection.CollectionState,
                    collection.CollectionImage,
                    collection.CollectionType,
                    collection.CreatedDateTime,
                    collection.ModifiedDateTime,
                    collection.UrlAlias,
                    collection.TemplateLayout,
                    collection.ConditionForCollection,
                }).Single();
                return collectionID;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }

        public bool Update(Collection collection)
        {
            try
            {
                string query = "update Collection set CollectionName=@CollectionName,CollectionDescription=@CollectionDescription," +
                    " PageTitle=@PageTitle,PageDescription=@PageDescription,CollectionState=@CollectionState," +
                    " CollectionImage=@CollectionImage,CollectionType=@CollectionType,ModifiedDateTime=@ModifiedDateTime," +
                    " UrlAlias=@UrlAlias,TemplateLayout=@TemplateLayout,ConditionForCollection=@ConditionForCollection" +
                        " where CollectionID = @CollectionID";
                int temp = connect.Execute(query, new
                {
                    collection.CollectionName,
                    collection.CollectionDescription,
                    collection.PageTitle,
                    collection.PageDescription,
                    collection.CollectionState,
                    collection.CollectionImage,
                    collection.CollectionType,
                    collection.ModifiedDateTime,
                    collection.UrlAlias,
                    collection.TemplateLayout,
                    collection.ConditionForCollection,
                    collection.CollectionID
                });
                return temp > 0;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }
        public List<Collection> SelectByWhere(string where)
        {
            try
            {
                string query = "";
                if (!string.IsNullOrEmpty(where))
                {
                    query = "select * from Collection where " + where + " order by ModifiedDateTime desc";
                }
                else
                {
                    query = "select * from Collection order by ModifiedDateTime desc";
                }
                List<Collection> collections = connect.Query<Collection>(query).ToList<Collection>();
                if (collections != null && collections.Count > 0)
                {
                    for (int i = 0; i < collections.Count; i++)
                    {
                        collections[i].TblRules = tblRuleService.SelectByCollectionID(collections[i].CollectionID);
                    }
                }
                return collections;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public bool DeleteByPrimary(int collectionID)
        {
            try
            {
                if (SNumber.ToNumber(collectionID) <= 0)
                {
                    return false;
                }
                string query = "delete from Collection where CollectionID = " + collectionID;
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