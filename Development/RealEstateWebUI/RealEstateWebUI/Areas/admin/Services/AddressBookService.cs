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
    public class AddressBookService
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);
        public List<AddressBook> GetAll()
        {
            try
            {
                string query = "select * from AddressBook order by AddressBookID";
                List<AddressBook> lstAddressBook = connect.Query<AddressBook>(query).ToList<AddressBook>();
                return lstAddressBook;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return new List<AddressBook>();
            }
        }

        public AddressBook GetByPrimaryKey(int addressBookID)
        {
            try
            {
                string query = "select * from AddressBook where AddressBookID = " + SNumber.ToNumber(addressBookID);
                AddressBook AddressBook = connect.Query<AddressBook>(query).FirstOrDefault<AddressBook>();
                return AddressBook;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return new AddressBook();
            }
        }

        public int Insert(AddressBook addressBook)
        {
            try
            {
                string query = "insert into AddressBook(AddressBookFirstName,AddressBookLastName,CompanyName," +
                        " Phone , HomeAddress , ProvinceName, Postal, CustomerID, CountryID,IsDefault) values (@AddressBookFirstName,@AddressBookLastName,@CompanyName," +
                        " @Phone , @HomeAddress , @ProvinceName, @Postal, @CustomerID, @CountryID, @IsDefault)" +
                        " SELECT @@IDENTITY";
                int addressBookID = connect.Query<int>(query, new { addressBook.AddressBookFirstName, addressBook.AddressBookLastName, addressBook.CompanyName, addressBook.Phone, addressBook.HomeAddress, addressBook.ProvinceName, addressBook.Postal, addressBook.CustomerID, addressBook.CountryID, addressBook.IsDefault }).Single();
                return addressBookID;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }
        public bool Update(AddressBook addressBook)
        {
            try
            {
                string query = "update AddressBook set AddressBookFirstName=@AddressBookFirstName,AddressBookLastName=@AddressBookLastName,CompanyName=@CompanyName," +
                        " Phone=@Phone , HomeAddress=@HomeAddress , ProvinceName=@ProvinceName, Postal=@Postal, CountryID=@CountryID, IsDefault = @IsDefault where AddressBookID = @AddressBookID";
                int temp = connect.Execute(query, new { addressBook.AddressBookFirstName, addressBook.AddressBookLastName, addressBook.CompanyName, addressBook.Phone, addressBook.HomeAddress, addressBook.ProvinceName, addressBook.Postal, addressBook.CountryID,addressBook.IsDefault, addressBook.AddressBookID });
                return temp > 0;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }
        public List<AddressBook> SelectByWhere(string where)
        {
            try
            {
                string query = "";
                if (!string.IsNullOrEmpty(where))
                {
                    query = "select * from AddressBook where " + where; 
                }
                else
                {
                    query = "select * from AddressBook";
                }
                return connect.Query<AddressBook>(query).ToList<AddressBook>();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return new List<AddressBook>();
            }
        }


        public List<AddressBook> SelectByCustomerID(int customerID)
        {
            try
            {
                string query = "select * from AddressBook where CustomerID = "+  customerID;
                return connect.Query<AddressBook>(query).ToList<AddressBook>();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return new List<AddressBook>();
            }
        }

        /// <summary>
        /// count number addressbook of customer not default address
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public int CountByCustomerIDNotDefault(int customerID)
        {
            try
            {
                string query = "select count(*) from AddressBook where IsDefault = 0 and CustomerID = " + customerID;
                int temp = connect.Query<int>(query).Single();
                return SNumber.ToNumber(temp);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }


        public AddressBook SelectDefaultAddressOfCustomer(int customerID)
        {
            try
            {
                string query = "select * from AddressBook where IsDefault = 1 and CustomerID = " + customerID;
                return connect.Query<AddressBook>(query).FirstOrDefault<AddressBook>();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public bool DeleteByPrimary(int addressBookID)
        {
            try
            {
                if (SNumber.ToNumber(addressBookID) <= 0)
                {
                    return false;
                }
                string query = "delete from AddressBook where AddressBookID = " + addressBookID;
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
                if (SNumber.ToNumber(customerID) <= 0)
                {
                    return false;
                }

                string query = "delete from AddressBook where CustomerID = " + customerID;
                connect.Execute(query);
                
                return true;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }
    }
}