using RealEstateWebUI.Areas.admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using RealEstateWebUI.Areas.admin.UtilzGeneral;

namespace RealEstateWebUI.Areas.admin.Services
{
    public class TagService
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);

        public List<Tag> GetAll()
        {
            try
            {
                string query = "select * from Tag";
                List<Tag> Tags = connect.Query<Tag>(query).ToList<Tag>();
                return Tags;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }
        public Tag GetByPrimaryKey(int tagID)
        {
            try
            {
                string query = "select * from Tag where TagID = " + SNumber.ToNumber(tagID);
                Tag Tag = connect.Query<Tag>(query).FirstOrDefault<Tag>();
                return Tag;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }
        public List<Tag> GetByTableNameID(int tableNameID)
        {
            try
            {
                string query = "select * from Tag where TableNameID = " + SNumber.ToNumber(tableNameID);
                List<Tag> Tags = connect.Query<Tag>(query).ToList<Tag>();
                return Tags;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return new List<Tag>();
            }
        }
        public int Insert(Tag tag)
        {
            try
            {
                string query = "insert into Tag(TagName,TableNameID)" +
                        " values (@TagName,@TableNameID)" +
                        " SELECT @@IDENTITY";
                int optionID = connect.Query<int>(query, new
                {
                    tag.TagName,
                    tag.TableNameID
                }).Single();
                return optionID;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }

        public bool CheckExistTag(string tagName, int tableNameID)
        {
            try
            {
                string query = "";
                if (!string.IsNullOrEmpty(tagName))
                {
                    query = "select * from Tag where TagName like N'" + tagName + "' and TableNameID = " + tableNameID;
                    List<Tag> tags = connect.Query<Tag>(query).ToList<Tag>();
                    if (tags != null && tags.Count > 0)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return false;
        }

        public List<Tag> SelectByWhere(string where)
        {
            try
            {
                string query = "";
                if (!string.IsNullOrEmpty(where))
                {
                    query = "select * from Tag where " + where;
                }
                else
                {
                    query = "select * from Tag";
                }
                return connect.Query<Tag>(query).ToList<Tag>();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return new List<Tag>();
            }
        }

        public bool DeleteByPrimary(int tagID)
        {
            try
            {
                if (SNumber.ToNumber(tagID) <= 0)
                {
                    return false;
                }
                string query = "delete from Tag where TagID = " + tagID;
                return 0 < connect.Execute(query);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }
        public bool DeleteByTablNameID(int tableNameID)
        {
            try
            {
                string query = "delete from Tag where TableNameID = " + tableNameID;
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