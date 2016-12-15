using RealEstateWebUI.Areas.admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using RealEstateWebUI.Areas.admin.UtilzGeneral;
using System.IO;

namespace RealEstateWebUI.Areas.admin.Services
{
    public class LogService
    {
        private ProductService productService = new ProductService();
        private AccountService appUserService = new AccountService();
        private CollectionService collectionService = new CollectionService();
        private CustomerService customerService = new CustomerService();
        private OrderService tblOrderService = new OrderService();

        private IDbConnection connect = new SqlConnection(Common.ConnectString);

        /// <summary>
        /// path to directory of project web
        /// </summary>
        private static string PathBase = HttpRuntime.AppDomainAppPath;
        /// <summary>
        /// name of file log
        /// </summary>
        private static string FileName = "LogFile.txt";

        public LogService()
        {
        }

        /// <summary>
        /// write information exception to file log.
        /// </summary>
        /// <param name="ex">exception will be wrote</param>
        public static void WriteException(Exception ex)
        {
            try
            {
                string urlFileLog = PathBase + "/" + FileName;

                string strError = "";
                strError += Environment.NewLine;
                strError += "----------Exception when: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
                strError += "\n" + ex.ToString();
                strError += Environment.NewLine;

                File.AppendAllText(urlFileLog, strError);
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// write information exception to file log.
        /// </summary>
        /// <param name="ex">exception will be wrote</param>
        public static void WriteError(string error)
        {
            try
            {
                string urlFileLog = PathBase + "/" + FileName;

                string strError = "";
                strError += Environment.NewLine;
                strError += "----------Error when: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
                strError += "\n" + error;
                strError += Environment.NewLine;

                File.AppendAllText(urlFileLog, strError);
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// write log to db
        /// </summary>
        /// <param name="userID">user exec event</param>
        /// <param name="actionID">action of event</param>
        /// <param name="objectID">ID of object actioned</param>
        /// <param name="dateTimeLog">date time when event exec at format YYYYMMDD</param>
        /// <param name="ipAddress"></param>
        /// <param name="tableNameID">id of table object belong to follow Common.TableNameID</param>
        /// <param name="objectValue">value of object</param>
        public static void WriteLog2DB(int userID, int actionID, int objectID, string dateTimeLog, string ipAddress, int tableNameID, string objectValue = "")
        {
            try
            {
                LogService tblLogService = new LogService();
                tblLogService.Insert(userID, actionID, objectID, dateTimeLog, ipAddress, tableNameID, objectValue);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return;
        }

        public List<TblLog> GetAll()
        {
            try
            {
                string query = "select * from TblLog order by DataTimeLog desc";
                List<TblLog> lstTblLog = connect.Query<TblLog>(query).ToList<TblLog>();
                return lstTblLog;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }
        public TblLog GetByPrimaryKey(int logID)
        {
            try
            {
                string query = "select * from TblLog where LogID = " + logID;
                TblLog tblLog = connect.Query<TblLog>(query).FirstOrDefault<TblLog>();
                return tblLog;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public bool Insert(int userID, int actionID, int objectID, string dateTimeLog, string ipAddress, int tableNameID, string objectValue = "")
        {
            IDbConnection connect = new SqlConnection(Common.ConnectString);
            try
            {
                string query = "insert into TblLog(UserID,ActionID,ObjectID,DataTimeLog,IP,TableNameID,ObjectValue) values(@UserID,@ActionID,@ObjectID,@DataTimeLog,@IP,@TableName,@ObjectValue) SELECT @@IDENTITY";
                int temp = connect.Query<int>(query, new { UserID = userID, ActionID = actionID, ObjectID = objectID, DataTimeLog = dateTimeLog, IP = ipAddress, TableName = tableNameID, ObjectValue = objectValue }).Single();
                if (temp > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                WriteException(ex);
            }
            return false;
        }

        public string GetHref2ObjectID(int tableNameID, int actionID, int objectID)
        {
            try
            {
                string controllerName = "";
                //string actionName = "";
                string strHref = "";

                if (tableNameID == (int)Common.TableName.Account)
                {
                    controllerName = "accounts";
                }
                else if (tableNameID == (int)Common.TableName.Collection)
                {
                    controllerName = "collections";
                }
                else if (tableNameID == (int)Common.TableName.Customer)
                {
                    controllerName = "customers";
                }
                else if (tableNameID == (int)Common.TableName.Product)
                {
                    controllerName = "products";
                }
                else if (tableNameID == (int)Common.TableName.TblOrder)
                {
                    controllerName = "orders";
                }

                if (actionID == (int)Common.ActionID.Update || actionID == (int)Common.ActionID.Insert || actionID == (int)Common.ActionID.View)
                {
                    //actionName = "detail";
                    strHref = string.Format("/admin/{0}/{1}", controllerName, objectID);
                }
                else
                {
                    strHref = "javascript: void(0)";
                }
                return strHref;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return "";
            }
        }
        public string GetValueLink2ObjectID(int tableNameID, int actionID, int objectID, string objectValue)
        {
            try
            {
                string value = "";
                if (actionID == (int)Common.ActionID.Delete)
                {
                    return objectValue;
                }

                if (tableNameID == (int)Common.TableName.Account)
                {
                    var temp = appUserService.GetByPrimaryKey(objectID);
                    if (temp != null)
                    {
                        value = temp.Username;
                        return value;
                    }
                }
                else if (tableNameID == (int)Common.TableName.Collection)
                {
                    var temp = collectionService.GetByPrimaryKey(objectID);
                    if (temp != null)
                    {
                        value = temp.CollectionName;
                        return value;
                    }
                }
                else if (tableNameID == (int)Common.TableName.Customer)
                {
                    var temp = customerService.GetByPrimaryKey(objectID);
                    if (temp != null)
                    {
                        value = temp.CustomerFirstName + " " + temp.CustomerLastName;
                        return value;
                    }
                }
                else if (tableNameID == (int)Common.TableName.Product)
                {
                    var temp = productService.GetByPrimaryKey(objectID);
                    if (temp != null)
                    {
                        value = temp.ProductName;
                        return value;
                    }
                }
                else if (tableNameID == (int)Common.TableName.TblOrder)
                {
                    var temp = tblOrderService.GetByPrimaryKey(objectID);
                    if (temp != null)
                    {
                        value = "#" + SString.ConverToString(Common.BaseNumberOrder + temp.Number);
                        return value;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return objectValue;
            }
            return objectValue;
        }
    }
}