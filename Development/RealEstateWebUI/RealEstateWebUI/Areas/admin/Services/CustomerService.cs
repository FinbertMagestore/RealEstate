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
    public class CustomerService
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);
        public List<Customer> GetAll()
        {
            try
            {
                string query = "select * from Customer order by ModifiedDateTime desc";
                List<Customer> customer = connect.Query<Customer>(query).ToList<Customer>();
                return customer;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return new List<Customer>();
            }
        }

        public Customer GetByPrimaryKey(int customerID)
        {
            try
            {
                string query = "select * from Customer where CustomerID = " + SNumber.ToNumber(customerID);
                Customer customer = connect.Query<Customer>(query).FirstOrDefault<Customer>();
                return customer;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public Customer GetByEmail(string customerEmail)
        {
            try
            {
                string query = string.Format("select * from Customer where CustomerEmail like N'{0}'", customerEmail);
                Customer customer = connect.Query<Customer>(query).FirstOrDefault<Customer>();
                return customer;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public List<Customer> GetByWhere(string where, string orderby = "ModifiedDateTime")
        {
            try
            {
                string query = "";
                if (!string.IsNullOrEmpty(where))
                {
                    query = string.Format("select * from Customer where " + where + " order by {0} desc", orderby);
                }
                else
                {
                    query = string.Format("select * from Customer order by {0} desc", orderby);
                }
                return connect.Query<Customer>(query).ToList<Customer>();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public int Insert(Customer customer)
        {
            try
            {
                string query = "insert into Customer(CustomerFirstName,CustomerLastName,CustomerEmail," +
                        " AcceptsMarketing, CustomerNote, CustomerState,CreatedDateTime,ModifiedDateTime," +
                        " TotalOrder,TotalCount,Tags) " +
                        " values (@CustomerFirstName, @CustomerLastName, @CustomerEmail," +
                        " @AcceptsMarketing,@CustomerNote,@CustomerState,@CreatedDateTime,@ModifiedDateTime," +
                        " @TotalOrder,@TotalCount,@Tags)" +
                        " SELECT @@IDENTITY";
                int customerID = connect.Query<int>(query, new
                {
                    customer.CustomerFirstName,
                    customer.CustomerLastName,
                    customer.CustomerEmail,
                    customer.AcceptsMarketing,
                    customer.CustomerNote,
                    customer.CustomerState,
                    customer.CreatedDateTime,
                    customer.ModifiedDateTime,
                    customer.TotalOrder,
                    customer.TotalCount,
                    customer.Tags
                }).Single();
                return customerID;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }
        public bool Update(Customer customer)
        {
            try
            {
                string query = "update Customer set CustomerFirstName = @CustomerFirstName,CustomerLastName=@CustomerLastName,CustomerEmail=@CustomerEmail," +
                        " AcceptsMarketing=@AcceptsMarketing, CustomerNote=@CustomerNote, CustomerState=@CustomerState, ModifiedDateTime = @ModifiedDateTime, " +
                        " TotalOrder=@TotalOrder,TotalCount=@TotalCount,Tags=@Tags" +
                        " where CustomerID = @CustomerID";
                return 0 < connect.Execute(query, new
                {
                    customer.CustomerFirstName,
                    customer.CustomerLastName,
                    customer.CustomerEmail,
                    customer.AcceptsMarketing,
                    customer.CustomerNote,
                    customer.CustomerState,
                    customer.ModifiedDateTime,
                    customer.TotalOrder,
                    customer.TotalCount,
                    customer.Tags,
                    CustomerID = customer.CustomerID
                });
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }
        public List<Customer> SelectByWhere(string where)
        {
            try
            {
                string query = "";
                if (!string.IsNullOrEmpty(where))
                {
                    query = "select * from Customer where " + where + " order by ModifiedDateTime desc";
                }
                else
                {
                    query = "select * from Customer order by ModifiedDateTime desc";
                }
                return connect.Query<Customer>(query).ToList<Customer>();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return new List<Customer>();
            }
        }

        public bool DeleteByPrimary(int customerID)
        {
            try
            {
                if (SNumber.ToNumber(customerID) <= 0)
                {
                    return false;
                }
                string query = "delete from Customer where CustomerID = " + customerID;
                return 0 < connect.Execute(query);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        /// <summary>
        /// get name of customer from firstname and lastname
        /// </summary>
        /// <param name="customerID">id of customer</param>
        /// <returns>name of customer</returns>
        public static object GetCustomerName(int? customerID)
        {
            try
            {
                if (SNumber.ToNumber(customerID) <= 0)
                {
                    return "";
                }
                IDbConnection connect = new SqlConnection(Common.ConnectString);
                string query = "select * from Customer where CustomerID = " + SNumber.ToNumber(customerID);
                Customer customer = connect.Query<Customer>(query).FirstOrDefault<Customer>();
                if (customer != null)
                {
                    return customer.CustomerFirstName + " " + customer.CustomerLastName;
                }
                return "";
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return "";
            }
        }

        /// <summary>
        /// get number order of a customer
        /// </summary>
        /// <param name="customerID">id of customer</param>
        /// <returns>number order</returns>
        public static object GetTotalOrderOfCustomer(int customerID)
        {
            try
            {
                IDbConnection connect = new SqlConnection(Common.ConnectString);
                string query = "select count(*) from TblOrder where CustomerID = " + SNumber.ToNumber(customerID);
                int totalOrder = SNumber.ToNumber(connect.Query<int>(query).Single());
                return totalOrder;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }
        /// <summary>
        /// get total cost of all order of a customer
        /// </summary>
        /// <param name="customerID">id of customer</param>
        /// <returns>total count</returns>
        public static object GetTotalCountOfCustomer(int customerID)
        {
            try
            {
                IDbConnection connect = new SqlConnection(Common.ConnectString);
                string query = "select * from TblOrder where CustomerID = " + SNumber.ToNumber(customerID);
                List<TblOrder> lstTblOrder = connect.Query<TblOrder>(query).ToList<TblOrder>();
                decimal totalCount = 0;
                for (int i = 0; i < lstTblOrder.Count; i++)
                {
                    totalCount += lstTblOrder[i].TotalCount;
                }
                return totalCount;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }

        public static int CountAll()
        {
            IDbConnection connectDB = new SqlConnection(Common.ConnectString);
            try
            {
                string query = "select count(*) from Customer";
                int countCustomer = connectDB.Query<int>(query).Single();
                return countCustomer;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }
    }
}