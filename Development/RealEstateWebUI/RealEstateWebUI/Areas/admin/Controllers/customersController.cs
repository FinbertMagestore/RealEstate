using RealEstateWebUI.Areas.admin.Models;
using RealEstateWebUI.Areas.admin.UtilzGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using Dapper;
using RealEstateWebUI.Areas.admin.Services;
using System.Web.Security;
using Microsoft.AspNet.Identity;
namespace RealEstateWebUI.Areas.admin.Controllers
{
    public class customersController : Controller
    {
        /// <summary>
        /// variable save table name for write log
        /// </summary>
        private int TableNameID = (int)Common.TableName.Customer;
        private CustomerService customerService = new CustomerService();
        private OrderService orderService = new OrderService();
        private AddressBookService addressBookService = new AddressBookService();
        private CountryService countryService = new CountryService();
        private OrderService tblOrderService = new OrderService();
        private AccountService accountService = new AccountService();
        private TagService tagService = new TagService();

        private IDbConnection connect = new SqlConnection(Common.ConnectString);

        [Authorize(Roles = "Admin")]
        public ActionResult index(int page = 1, string strMessage = "")
        {
            string strError = "", strSuccess = "";
            if (!string.IsNullOrEmpty(strMessage))
            {
                if (strMessage.Equals("delete1"))
                {
                    strSuccess += "Xoá 1 khách hàng thành công";
                }
                else if (strMessage.Equals("notExist"))
                {
                    strError += "Khách hàng không tồn tại";
                }
            }
            ViewBag.strSuccess = strSuccess;
            ViewBag.strError = strError;

            int pageSize = 10;

            List<Customer> customer = customerService.GetAll();
            CustomerModel customerModel = new CustomerModel();
            customerModel.lstCustomer = customer.ToPagedList(page, pageSize);
            return View(customerModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult index(CustomerModel customerModel, int page = 1)
        {
            try
            {
                int pageSize = int.MaxValue;
                int totalOrder = SNumber.ToNumber(customerModel.txtTotalOrder);

                string strWhere = "1=1";

                if (customerModel.ddlConditionFilter == "TotalOrder")
                {
                    if (!string.IsNullOrEmpty(customerModel.ddlCompareTotalOrder))
                    {
                        string expression = "";
                        if (customerModel.ddlCompareTotalOrder.Equals("equals"))
                        {
                            expression = "=";
                        }
                        else if (customerModel.ddlCompareTotalOrder.Equals("notequals"))
                        {
                            expression = "<>";
                        }
                        else if (customerModel.ddlCompareTotalOrder.Equals("greater"))
                        {
                            expression = ">";
                        }
                        else if (customerModel.ddlCompareTotalOrder.Equals("smaller"))
                        {
                            expression = "<";
                        }
                        if (!string.IsNullOrEmpty(expression))
                        {
                            strWhere += " and TotalOrder " + expression + customerModel.txtTotalOrder;
                        }
                    }
                }
                else if (customerModel.ddlConditionFilter == "TotalCount")
                {
                    if (!string.IsNullOrEmpty(customerModel.ddlCompareTotalCount))
                    {
                        string expression = "";
                        if (customerModel.ddlCompareTotalCount.Equals("equals"))
                        {
                            expression = "=";
                        }
                        else if (customerModel.ddlCompareTotalCount.Equals("notequals"))
                        {
                            expression = "<>";
                        }
                        else if (customerModel.ddlCompareTotalCount.Equals("greater"))
                        {
                            expression = ">";
                        }
                        else if (customerModel.ddlCompareTotalCount.Equals("smaller"))
                        {
                            expression = "<";
                        }
                        if (!string.IsNullOrEmpty(expression))
                        {
                            strWhere += " and TotalCount " + expression + customerModel.txtTotalCount;
                        }
                    }
                }
                else if (customerModel.ddlConditionFilter == "AcceptsMarketing")
                {
                    string acceptsMarketing = "0";
                    if (customerModel.ddlAcceptsMarketing.Equals("true"))
                    {
                        acceptsMarketing = "1";
                    }
                    strWhere += " and AcceptsMarketing = " + acceptsMarketing;
                }
                else if (customerModel.ddlConditionFilter == "State")
                {
                    string state = "0";
                    if (customerModel.ddlState.Equals("enable"))
                    {
                        state = "1";
                    }
                    strWhere += " and CustomerState = " + state;
                }

                if (!string.IsNullOrEmpty(customerModel.txtConditionFind))
                {
                    strWhere += string.Format(" and (CustomerFirstName like N'%{0}%' or CustomerLastName like N'%{1}%' or CustomerEmail like N'%{2}%') order by ModifiedDate,CreatedDate", customerModel.txtConditionFind, customerModel.txtConditionFind, customerModel.txtConditionFind);
                }

                List<Customer> customers = customerService.SelectByWhere(strWhere).ToList<Customer>();
                if (customers != null && customers.Count > 0)
                {
                    customerModel.lstCustomer = customers.ToPagedList(page, pageSize);
                }
                return View(customerModel);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return View(customerModel);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult detail(int id, string strMessage)
        {
            try
            {
                Customer customer = customerService.GetByPrimaryKey(id);
                if (customer != null)
                {
                    customer.TblOrders = tblOrderService.GetByCustomerID(id);
                    foreach (var item in customer.TblOrders)
                    {
                        string orderName = "#" + (Common.BaseNumberOrder + item.Number).ToString();
                        item.OrderName = orderName;
                    }
                    customer.ListTag = tagService.GetByTableNameID((int)Common.TableName.Customer);

                    string strError = "", strSuccess = "";
                    if (!string.IsNullOrEmpty(strMessage))
                    {
                        if (strMessage.Equals("update1"))
                        {
                            strSuccess += "Cập nhật thành công";
                        }
                        else if (strMessage.Equals("update0"))
                        {
                            strError += "Cập nhật thất bại";
                        }
                    }
                    ViewBag.strSuccess = strSuccess;
                    ViewBag.strError = strError;
                    return View(customer);
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return RedirectToAction("index", "customers", new { strMessage = "notExist" });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult detail(Customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Customer temp = customerService.GetByPrimaryKey(customer.CustomerID);
                    if (temp != null)
                    {
                        temp.Tags = customer.Tags;
                        temp.CustomerNote = customer.CustomerNote;
                        temp.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                        if (customerService.Update(temp))
                        {
                            if (!string.IsNullOrEmpty(customer.Tags))
                            {
                                string[] tags = customer.Tags.Split(',');
                                foreach (var item in tags)
                                {
                                    if (!tagService.CheckExistTag(item, (int)Common.TableName.Customer))
                                    {
                                        Tag tag = new Tag();
                                        tag.TagName = item;
                                        tag.TableNameID = (int)Common.TableName.Customer;

                                        int tagID = tagService.Insert(tag);
                                    }
                                }
                            }

                            string customerName = temp.CustomerFirstName + " " + temp.CustomerLastName;
                            LogService.WriteLog2DB(accountService.GetUserId(User.Identity.GetUserName()), (int)Common.ActionID.Update, customer.CustomerID, SDateTime.GetYYYYMMddHmmSSNow(), General.GetIPAddress(), TableNameID, customerName);

                            return RedirectToAction("detail", "customers", new { id = customer.CustomerID, strMessage = "update1" });
                        }
                        return RedirectToAction("detail", "customers", new { id = customer.CustomerID, strMessage = "update0" });
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return RedirectToAction("detail", "customers", new { id = customer.CustomerID, strMessage = "update0" });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult editOverview(int id)
        {
            Customer customer = customerService.GetByPrimaryKey(id);
            return PartialView(customer);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public int editOverview(Customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Customer temp = customerService.GetByPrimaryKey(customer.CustomerID);
                    if (temp != null)
                    {
                        temp.CustomerFirstName = customer.CustomerFirstName;
                        temp.CustomerLastName = customer.CustomerLastName;
                        temp.CustomerEmail = customer.CustomerEmail;
                        temp.AcceptsMarketing = customer.AcceptsMarketing;
                        if (customerService.Update(temp))
                        {
                            string customerName = temp.CustomerFirstName + " " + temp.CustomerLastName;
                            LogService.WriteLog2DB(accountService.GetUserId(User.Identity.GetUserName()), (int)Common.ActionID.Update, customer.CustomerID, SDateTime.GetYYYYMMddHmmSSNow(), General.GetIPAddress(), TableNameID, customerName);
                            return 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return 0;
        }


        [Authorize(Roles = "Admin")]
        public ActionResult addAddressBook(int customerID)
        {
            AddressBook addressBook = new AddressBook();
            if (customerID > 0)
            {
                addressBook.CustomerID = customerID;
            }
            addressBook.Countries = countryService.GetAll();
            return PartialView(addressBook);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public int addAddressBook(AddressBook addressBook)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    addressBook.IsDefault = false;
                    int temp = addressBookService.Insert(addressBook);
                    if (temp > 0)
                    {
                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return 0;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult editAddressBook(int id)
        {
            AddressBook addressBook = addressBookService.GetByPrimaryKey(id);
            addressBook.Countries = countryService.GetAll();
            return PartialView(addressBook);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public int editAddressBook(AddressBook addressBook)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (addressBookService.Update(addressBook))
                    {
                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return 0;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public int deleteAddressBook(int id, AddressBook addressBook)
        {
            try
            {
                if (addressBookService.DeleteByPrimary(id))
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return 0;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult create()
        {
            Customer customer = new Customer();
            customer.AddressBook.Countries = countryService.GetAll();
            customer.ListTag = tagService.GetByTableNameID((int)Common.TableName.Customer);

            return View(customer);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult create(Customer customer)
        {
            customer.AddressBook.Countries = countryService.GetAll();
            if (ModelState.IsValid)
            {
                customer.CreatedDateTime = customer.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                customer.CustomerState = Common.InActive;
                int customerID = customerService.Insert(customer);
                if (SNumber.ToNumber(customerID) > 0)
                {
                    if (!string.IsNullOrEmpty(customer.Tags))
                    {
                        string[] tags = customer.Tags.Split(',');
                        foreach (var item in tags)
                        {
                            if (!tagService.CheckExistTag(item, (int)Common.TableName.Customer))
                            {
                                Tag tag = new Tag();
                                tag.TagName = item;
                                tag.TableNameID = (int)Common.TableName.Customer;

                                int tagID = tagService.Insert(tag);
                            }
                        }
                    }

                    string customerName = customer.CustomerFirstName + " " + customer.CustomerLastName;
                    LogService.WriteLog2DB(accountService.GetUserId(User.Identity.GetUserName()), (int)Common.ActionID.Insert, customerID, SDateTime.GetYYYYMMddHmmSSNow(), General.GetIPAddress(), TableNameID, customerName);

                    customer.AddressBook.CustomerID = customerID;
                    customer.AddressBook.IsDefault = true;
                    int addressBookID = addressBookService.Insert(customer.AddressBook);
                }
                return RedirectToAction("detail", "customers", new { id = customerID });
            }
            return View(customer);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult activeAccount(int id)
        {
            ActiveAccountModel activeAccountModel = new ActiveAccountModel();
            activeAccountModel.CustomerID = id;
            return PartialView(activeAccountModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public int activeAccount(ActiveAccountModel activeAccountModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Membership.ValidateUser(User.Identity.Name, activeAccountModel.Password))
                    {
                        if (activeAccountModel.CustomerID > 0)
                        {
                            Customer customer = customerService.GetByPrimaryKey(activeAccountModel.CustomerID);

                            customer.CustomerState = (int)Common.Active;
                            if (customerService.Update(customer))
                            {
                                string customerName = customer.CustomerFirstName + " " + customer.CustomerLastName;
                                LogService.WriteLog2DB(accountService.GetUserId(User.Identity.GetUserName()), (int)Common.ActionID.Update, customer.CustomerID, SDateTime.GetYYYYMMddHmmSSNow(), General.GetIPAddress(), TableNameID, customerName);

                                if (activeAccountModel.SendEmail2Customer)
                                {
                                    string title = "Kích hoạt tài khoản";
                                    string body = string.Format("Tài khoản của Anh/chị {0} vừa được kích hoạt.", customer.CustomerFirstName + " " + customer.CustomerLastName);
                                    AppUser appUser = accountService.GetByPrimaryKey(accountService.GetUserId(User.Identity.GetUserName()));
                                    if (appUser != null)
                                    {
                                        General.SendEmail(appUser.Email, title, body);

                                    }
                                }
                                return 1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return 0;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult disableAccount(int id)
        {
            DisableAccountModel disableAccountModel = new DisableAccountModel();
            disableAccountModel.CustomerID = id;
            return PartialView(disableAccountModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public int disableAccount(DisableAccountModel activeAccountModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //if (Membership.ValidateUser(User.Identity.Name, activeAccountModel.Password))
                    //{
                    //    if (activeAccountModel.CustomerID > 0)
                    //    {
                    Customer customer = customerService.GetByPrimaryKey(activeAccountModel.CustomerID);
                    customer.CustomerState = Common.InActive;
                    if (customerService.Update(customer))
                    {
                        string customerName = customer.CustomerFirstName + " " + customer.CustomerLastName;
                        LogService.WriteLog2DB(accountService.GetUserId(User.Identity.GetUserName()), (int)Common.ActionID.Update, customer.CustomerID, SDateTime.GetYYYYMMddHmmSSNow(), General.GetIPAddress(), TableNameID, customerName);
                        return 1;
                    }
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return 0;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult delete(int id)
        {
            try
            {
                Customer customer = customerService.GetByPrimaryKey(id);
                if (customer != null)
                {
                    return PartialView(customer);
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return HttpNotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public int delete(Customer customer)
        {
            try
            {
                bool result = false;
                customer = customerService.GetByPrimaryKey(customer.CustomerID);
                tblOrderService.DeleteByCustomerID(customer.CustomerID);
                addressBookService.DeleteByCustomerID(customer.CustomerID);
                result = customerService.DeleteByPrimary(customer.CustomerID);
                if (result)
                {
                    string customerName = customer.CustomerFirstName + " " + customer.CustomerLastName;
                    LogService.WriteLog2DB(accountService.GetUserId(User.Identity.GetUserName()), (int)Common.ActionID.Delete, 0, SDateTime.GetYYYYMMddHmmSSNow(), General.GetIPAddress(), TableNameID, customerName);
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return 0;
        }

        /// <summary>
        /// create popup view list customer
        /// </summary>
        /// <param name="query">param to select customer</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public ActionResult dropdown(string query)
        {
            try
            {
                CustomerDropdownModel customerDropdownModel = new CustomerDropdownModel();
                List<CustomerDropdown> customerDropdowns = new List<CustomerDropdown>();
                List<Customer> customers = new List<Customer>();
                if (string.IsNullOrEmpty(query))
                {
                    customers = customerService.GetAll();
                }
                else
                {
                    string where = string.Format("CustomerFirstName like N'%{0}%' or CustomerLastName like N'%{1}%' or CustomerEmail like N'%{2}%'", query, query, query);
                    customers = customerService.SelectByWhere(where);
                }
                if (customers != null && customers.Count > 0)
                {
                    foreach (var item in customers)
                    {
                        CustomerDropdown temp = new CustomerDropdown();
                        temp.CustomerID = item.CustomerID;
                        temp.CustomerName = item.CustomerFirstName + " " + item.CustomerLastName;
                        temp.CustomerEmail = item.CustomerEmail;
                        temp.Choice = false;
                        customerDropdowns.Add(temp);
                    }
                    customerDropdownModel.CustomerDropdowns = customerDropdowns;
                }
                return View(customerDropdownModel);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        /// <summary>
        /// get information of customer by id: contains order, addressbook
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult getCustomer(int id)
        {
            try
            {
                Customer customer = customerService.GetByPrimaryKey(id);
                if (customer == null)
                {
                    return null;
                }
                customer.AddressBook = addressBookService.SelectDefaultAddressOfCustomer(id);
                if (customer.AddressBook != null)
                {
                    if (customer.AddressBook.CountryID != null)
                    {
                        customer.AddressBook.Country = countryService.GetByPrimaryKey(customer.AddressBook.CountryID.Value);
                    }
                }
                customer.TblOrders = orderService.GetByCustomerID(id);
                return Json(customer, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public ActionResult getEmails(string query = "")
        {
            try
            {
                List<Customer> customers = null;
                if (string.IsNullOrEmpty(query))
                {
                    customers = customerService.GetAll();
                }
                else
                {
                    string where = string.Format("CustomerEmail like N'%{0}%'", query);
                    customers = customerService.GetByWhere(where);
                }
                return Json(customers, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return Json("", JsonRequestBehavior.AllowGet);
                throw;
            }
        }
    }
}