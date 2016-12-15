using RealEstateWebUI.Areas.admin.Models;
using RealEstateWebUI.Areas.admin.UtilzGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using PagedList;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using RealEstateWebUI.Areas.admin.Services;
using Microsoft.AspNet.Identity;

namespace RealEstateWebUI.Areas.admin.Controllers
{
    public class ordersController : Controller
    {
        /// <summary>
        /// variable save table name for write log
        /// </summary>
        private int TableNameID = (int)Common.TableName.TblOrder;

        private OrderService orderService = new OrderService();
        private BillingAddressService billingAddressService = new BillingAddressService();
        private ShippingAddressService shippingAddressService = new ShippingAddressService();
        private LineItemService lineItemService = new LineItemService();
        private CountryService countryService = new CountryService();
        private ImageService imageService = new ImageService();
        private AccountService accountService = new AccountService();
        private VariantService variantService = new VariantService();
        private CustomerService customerService = new CustomerService();
        private TagService tagService = new TagService();

        private IDbConnection connect = new SqlConnection(Common.ConnectString);

        [Authorize(Roles = "Admin")]
        public ActionResult index(int page = 1, string strMessage = "")
        {

            int pageSize = 10;

            #region tự viết phân trang
            //ViewBag.PageCurrent = page;
            //int totalRecord = db.Orders.Count();
            //var temp = (double)totalRecord / pageSize;
            //if (SNumber.IsNumber(temp))
            //{
            //    ViewBag.PageNum = temp;
            //}
            //else
            //{
            //    ViewBag.PageNum = (int)temp + 1;
            //}

            //List<Order> result = new List<Order>();
            //int startRecord = 0;
            ////page = 1 => startRecord = 0, endRecord = 1: (pageSize - 1)
            ////page = 2 => startRecord = 2, endRecord = 3: (pageSize - 1)
            //startRecord = (page - 1) * pageSize;
            //int numRecord = 0;

            //for (int i = startRecord; i < totalRecord; i++)
            //{
            //    if (numRecord == pageSize)
            //    {
            //        break;
            //    }
            //    result.Add(db.Orders.ToList().ElementAt(i));
            //    numRecord++;
            //}
            #endregion

            string strError = "", strSuccess = "";
            if (!string.IsNullOrEmpty(strMessage))
            {
                if (strMessage.Equals("delete1"))
                {
                    strSuccess += "Xoá 1 đơn hàng thành công";
                }
                else if (strMessage.Equals("notExist"))
                {
                    strError = "Đơn hàng không tồn tại";
                }
            }
            ViewBag.strSuccess = strSuccess;
            ViewBag.strError = strError;

            List<TblOrder> tblOrder = orderService.GetAll();
            foreach (var item in tblOrder)
            {
                string orderName = "#" + (Common.BaseNumberOrder + item.Number).ToString();
                item.OrderName = orderName;
            }

            OrderModel orderModel = new OrderModel();
            orderModel.lstTblOrder = tblOrder.ToPagedList(page, pageSize);
            return View(orderModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult index(OrderModel orderModel, int page = 1)
        {
            try
            {
                int pageSize = int.MaxValue;

                string query = "";
                string strWhere = "";
                if (!string.IsNullOrEmpty(orderModel.txtConditionFind))
                {
                    strWhere += string.Format(" and (TblOrder.OrderNote like N'%{0}%' or TblOrder.CustomerEmail like N'%{1}%') ", orderModel.txtConditionFind, orderModel.txtConditionFind);
                }

                if (orderModel.ddlConditionFilter == "OrderStatus")
                {
                    ViewBag.ddlConditionFilter = "OrderStatus";
                    if (!string.IsNullOrEmpty(orderModel.ddlOrderStatus))
                    {
                        ViewBag.ddlOrderStatus = orderModel.ddlOrderStatus;
                        query = "select * from TblOrder where OrderStatus = " + orderModel.ddlOrderStatus.ToString() + strWhere;
                    }
                }
                else if (orderModel.ddlConditionFilter == "BillingStatus")
                {
                    ViewBag.ddlConditionFilter = "BillingStatus";
                    if (!string.IsNullOrEmpty(orderModel.ddlBillingStatus))
                    {
                        ViewBag.ddlBillingStatus = orderModel.ddlBillingStatus;
                        query = "select * from TblOrder where BillingStatus like N'" + orderModel.ddlBillingStatus.ToString() + "' " + strWhere;
                    }
                }
                else if (orderModel.ddlConditionFilter == "ShippingStatus")
                {
                    ViewBag.ddlConditionFilter = "ShippingStatus";
                    if (!string.IsNullOrEmpty(orderModel.ddlShippingStatus))
                    {
                        ViewBag.ddlShippingStatus = orderModel.ddlShippingStatus;
                        query = "select * from TblOrder where ShippingStatus like N'" + orderModel.ddlShippingStatus.ToString() + "' " + strWhere;
                    }
                }
                else if (orderModel.ddlConditionFilter == "Customer")
                {
                    ViewBag.ddlConditionFilter = "Customer";
                    if (!string.IsNullOrEmpty(orderModel.txtCustomer))
                    {
                        ViewBag.txtCustomer = orderModel.txtCustomer;
                        query = string.Format("select * from TblOrder join Customer on TblOrder.CustomerID = Customer.CustomerID "+
                            " where Customer.CustomerFirstName like N'%{0}%' or Customer.CustomerLastName like N'%{1}%' or Customer.CustomerNote like N'%{2}%' "+
                            " or Customer.CustomerEmail like N'%{3}%' " +
                            " {4} order by TblOrder.OrderID",
                            orderModel.txtCustomer, orderModel.txtCustomer, orderModel.txtCustomer, orderModel.txtCustomer, strWhere);
                    }
                }
                if (string.IsNullOrEmpty(query))
                {
                    if (!string.IsNullOrEmpty(strWhere) && strWhere.Length > 4)
                    {
                        query = "select * from TblOrder where" + strWhere.Substring(4) + " order by OrderID";
                    }
                    else
                    {
                        query = "select * from TblOrder order by OrderID";
                    }
                }

                List<TblOrder> tblOrder = connect.Query<TblOrder>(query).ToList<TblOrder>();

                foreach (var item in tblOrder)
                {
                    string orderName = "#" + (Common.BaseNumberOrder + item.Number).ToString();
                    item.OrderName = orderName;
                }
                orderModel.lstTblOrder = tblOrder.ToPagedList(page, pageSize);
                return View(orderModel);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return View(orderModel);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult create()
        {
            CreateOrderModel createOrderModel = new CreateOrderModel();
            createOrderModel.BillingAddress.Countries = countryService.GetAll();
            createOrderModel.ShippingAddress.Countries = countryService.GetAll();
            return View(createOrderModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult create(CreateOrderModel createOrderModel)
        {
            if (ModelState.IsValid)
            {
                int billingAddressID = 0, shippingAddressID = 0;

                decimal totalCount = 0;
                if (createOrderModel.LineItems != null && createOrderModel.LineItems.Count > 0)
                {
                    foreach (var item in createOrderModel.LineItems)
                    {
                        totalCount += item.Price * item.Quantity;
                    }
                }

                TblOrder order = new TblOrder();
                order.OrderStatus = Common.Active;
                if (createOrderModel.Customer != null)
                {
                    order.CustomerEmail = createOrderModel.Customer.CustomerEmail;
                    if (SNumber.ToNumber(createOrderModel.Customer.CustomerID) > 0)
                    {
                        order.CustomerID = createOrderModel.Customer.CustomerID;

                        BillingAddress billingAddress = new BillingAddress();
                        billingAddress.CountryID = createOrderModel.BillingAddress.CountryID;
                        billingAddress.CountryName = createOrderModel.BillingAddress.CountryName;
                        billingAddress.CustomerName = createOrderModel.BillingAddress.CustomerName;
                        billingAddress.HomeAddress = createOrderModel.BillingAddress.HomeAddress;
                        billingAddress.Phone = createOrderModel.BillingAddress.Phone;
                        billingAddress.ProvinceName = createOrderModel.BillingAddress.ProvinceName;
                        billingAddressID = billingAddressService.Insert(billingAddress);

                        ShippingAddress shippingAddress = new ShippingAddress();
                        shippingAddress.CountryID = createOrderModel.ShippingAddress.CountryID;
                        shippingAddress.CountryName = createOrderModel.ShippingAddress.CountryName;
                        shippingAddress.CustomerName = createOrderModel.ShippingAddress.CustomerName;
                        shippingAddress.HomeAddress = createOrderModel.ShippingAddress.HomeAddress;
                        shippingAddress.Phone = createOrderModel.ShippingAddress.Phone;
                        shippingAddress.ProvinceName = createOrderModel.ShippingAddress.ProvinceName;
                        shippingAddressID = shippingAddressService.Insert(shippingAddress);
                    }
                    else
                    {
                        order.CustomerID = null;
                    }
                    order.BillingStatus = createOrderModel.BillingStatus;
                    order.ShippingStatus = Common.Unfulfilled;
                    order.OrderNote = createOrderModel.OrderNote;
                    order.OrderStatus = Common.Active;
                    if (shippingAddressID == 0)
                    {
                        order.ShippingAddressID = null;
                    }
                    else
                    {
                        order.ShippingAddressID = shippingAddressID;
                    }
                    if (billingAddressID == 0)
                    {
                        order.BillingAddressID = null;
                    }
                    else
                    {
                        order.BillingAddressID = billingAddressID;
                    }
                    order.TotalCount = totalCount;
                    order.Number = orderService.GetLastNumber() + 1;
                    order.CreatedDateTime = order.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();

                    int orderID = orderService.Insert(order);

                    if (SNumber.ToNumber(orderID) > 0)
                    {
                        if (order.BillingStatus == Common.Paid)
                        {
                            if (SNumber.ToNumber(order.CustomerID) > 0)
                            {
                                Customer customer = customerService.GetByPrimaryKey(SNumber.ToNumber(order.CustomerID));
                                customer.TotalCount += totalCount;
                                customer.TotalOrder += 1;
                                customer.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                                customerService.Update(customer);
                            }
                        }

                        string orderName = "#" + (Common.BaseNumberOrder + order.Number).ToString();
                        LogService.WriteLog2DB(accountService.GetUserId(User.Identity.GetUserName()), (int)Common.ActionID.Insert, orderID, SDateTime.GetYYYYMMddHmmSSNow(), General.GetIPAddress(), TableNameID, orderName);
                        if (createOrderModel.LineItems != null && createOrderModel.LineItems.Count > 0)
                        {
                            foreach (var item in createOrderModel.LineItems)
                            {
                                item.OrderID = orderID;
                                item.ShippingStatus = null;
                                if (item.IsDefault)
                                {
                                    item.VariantName = "Default Title";
                                }
                                lineItemService.Insert(item);
                            }
                        }
                        return Json(new { id = orderID });
                    }
                }
            }
            string strErrorMessage = "";
            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    strErrorMessage += error.ErrorMessage + "<br/>";
                }
            }
            return Json(new { id="0", error = strErrorMessage });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult detail(int id, string strMessage = "")
        {
            try
            {
                string strError = "", strSuccess = "";
                if (!string.IsNullOrEmpty(strMessage))
                {
                    if (strMessage.Equals("bill1"))
                    {
                        strSuccess = "Xác nhận thanh toán thành công";
                    }
                    else if (strMessage.Equals("bill0"))
                    {
                        strError = "Xác nhận thanh toán thất bại";
                    }
                    else if (strMessage.Equals("delivery1"))
                    {
                        strSuccess = "Cập nhật thông tin giao hàng thành công";
                    }
                    else if (strMessage.Equals("delivery0"))
                    {
                        strError = "Cập nhật thông tin giao hàng thất bại";
                    }
                    else if (strMessage.Equals("update1"))
                    {
                        strSuccess = "Cập nhật thông tin đơn hàng thành công";
                    }
                    else if (strMessage.Equals("update0"))
                    {
                        strError = "Cập nhật thông tin đơn hàng thất bại";
                    }
                    else if (strMessage.Equals("addShippingAddress1"))
                    {
                        strSuccess = "Thêm mới địa chỉ giao hàng thành công";
                    }
                    else if (strMessage.Equals("addShippingAddress0"))
                    {
                        strError = "Thêm mới địa chỉ giao hàng thất bại";
                    }
                    else if (strMessage.Equals("editShippingAddress1"))
                    {
                        strSuccess = "Sửa địa chỉ giao hàng thành công";
                    }
                    else if (strMessage.Equals("editShippingAddress0"))
                    {
                        strError = "Sửa địa chỉ giao hàng thất bại";
                    }
                    else if (strMessage.Equals("editEmail1"))
                    {
                        strSuccess = "Sửa điạ chỉ email của khách hàng thành công";
                    }
                    else if (strMessage.Equals("editEmail1"))
                    {
                        strError = "Sửa điạ chỉ email của khách hàng thất bại";
                    }
                }
                ViewBag.strSuccess = strSuccess;
                ViewBag.strError = strError;

                TblOrder order = orderService.GetByPrimaryKey(id);
                if (order == null)
                {
                    return RedirectToAction("", "orders", new { strMessage = "notExist" });
                }
                order.OrderName = "#" + (Common.BaseNumberOrder + order.Number).ToString() + " " + SDateTime.ToDateTime(order.CreatedDateTime);

                DetailOrderModel detailOrderModel = new DetailOrderModel();
                detailOrderModel.OrderID = order.OrderID;
                detailOrderModel.CustomerEmail = order.CustomerEmail;
                detailOrderModel.CustomerID = order.CustomerID;
                detailOrderModel.Customer = customerService.GetByPrimaryKey(SNumber.ToNumber(order.CustomerID));
                detailOrderModel.BillingStatus = order.BillingStatus;
                detailOrderModel.ShippingStatus = order.ShippingStatus;
                detailOrderModel.TotalCount = order.TotalCount;
                detailOrderModel.OrderName = order.OrderName;
                detailOrderModel.OrderNote = order.OrderNote;
                detailOrderModel.Tags = order.Tags;
                detailOrderModel.ListTag = tagService.GetByTableNameID((int)Common.TableName.TblOrder);

                detailOrderModel.BillingAddressID = SNumber.ToNumber(order.BillingAddressID);
                detailOrderModel.BillingAddress = billingAddressService.GetByPrimaryKey(SNumber.ToNumber(order.BillingAddressID));
                if (detailOrderModel.BillingAddress != null)
                {
                    detailOrderModel.BillingAddress.Countries = countryService.GetAll();
                }

                detailOrderModel.ShippingAddressID = SNumber.ToNumber(order.ShippingAddressID);
                detailOrderModel.ShippingAddress = shippingAddressService.GetByPrimaryKey(SNumber.ToNumber(order.ShippingAddressID));
                if (detailOrderModel.ShippingAddress == null)
                {
                    detailOrderModel.ShippingAddress = new ShippingAddress();
                }
                detailOrderModel.ShippingAddress.Countries = countryService.GetAll();

                if (order.OrderID > 0)
                {
                    List<LineItem> lineItems = lineItemService.GetByOrderID(order.OrderID);
                    foreach (var item in lineItems)
                    {
                        if (item.VariantName == "Default Title")
                        {
                            var thumb = ImageService.GetPathImageFirstOfProduct(item.ProductID);
                            item.ImageUrl = thumb;
                        }
                        else
                        {
                            Variant variant = variantService.GetByPrimaryKey(item.VariantID);
                            if (variant != null)
                            {
                                TblImage image = imageService.GetByPrimaryKey(variant.ImageID);
                                if (image != null)
                                {
                                    item.ImageUrl = image.ImageUrl;
                                }
                            }
                        }

                        if (item.ShippingStatus == null)
                        {
                            detailOrderModel.LineItemsPending.Add(item);
                        }
                        else
                        {
                            detailOrderModel.LineItemsPaid.Add(item);
                        }
                    }
                }
                return View(detailOrderModel);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return RedirectToAction("", "orders", new { strMessage = "notExist"});
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult detail(DetailOrderModel detailOrderModel)
        {
            try
            {
                if (detailOrderModel != null)
                {
                    TblOrder order = orderService.GetByPrimaryKey(detailOrderModel.OrderID);
                    order.OrderNote = detailOrderModel.OrderNote;
                    order.Tags = detailOrderModel.Tags;
                    if (orderService.Update(order))
                    {
                        string orderName = "#" + (Common.BaseNumberOrder + order.Number).ToString();
                        LogService.WriteLog2DB(accountService.GetUserId(User.Identity.GetUserName()), (int)Common.ActionID.Update, detailOrderModel.OrderID, SDateTime.GetYYYYMMddHmmSSNow(), General.GetIPAddress(), TableNameID, orderName);

                        if (!string.IsNullOrEmpty(detailOrderModel.Tags))
                        {
                            string[] tags = detailOrderModel.Tags.Split(',');
                            foreach (var item in tags)
                            {
                                if (!tagService.CheckExistTag(item, (int)Common.TableName.TblOrder))
                                {
                                    Tag tag = new Tag();
                                    tag.TagName = item;
                                    tag.TableNameID = (int)Common.TableName.TblOrder;

                                    int tagID = tagService.Insert(tag);
                                }
                            }
                        }
                        return RedirectToAction("detail", "orders", new { id = detailOrderModel.OrderID, strMessage = "update1" });
                    }
                }
                return RedirectToAction("detail", "orders", new { id = detailOrderModel.OrderID, strMessage = "update0" });
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return RedirectToAction("detail", "orders", new { id = detailOrderModel.OrderID, strMessage = "update0" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public string deliveryLineItem(int id)
        {
            try
            {
                TblOrder order = orderService.GetByPrimaryKey(id);
                if (order != null)
                {
                    order.ShippingStatus = "fulfilled";
                    order.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                    if (orderService.Update(order))
                    {
                        List<LineItem> lineItems = lineItemService.GetByOrderID(id);
                        if (lineItems != null && lineItems.Count > 0)
                        {
                            foreach (var item in lineItems)
                            {
                                item.ShippingStatus = Common.Fulfilled;
                                lineItemService.Update(item);
                            }
                        }
                        return "delivery1";
                    }
                }
                return "delivery0";
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return "delivery0";
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public string billingOrder(int id)
        {
            try
            {
                TblOrder order = orderService.GetByPrimaryKey(id);
                if (order != null)
                {
                    order.BillingStatus = "paid";
                    order.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                    if (orderService.Update(order))
                    {
                        if (SNumber.ToNumber(order.CustomerID) > 0)
                        {
                            Customer customer = customerService.GetByPrimaryKey(SNumber.ToNumber(order.CustomerID));
                            customer.TotalCount += order.TotalCount;
                            customer.TotalOrder += 1;
                            customer.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                            customerService.Update(customer);
                        }
                        return "bill1";
                    }
                }
                return "bill0";
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return "bill0";
        }

        public ActionResult addShippingAddress(DetailOrderModel detailOrderModel)
        {
            try
            {
                if (detailOrderModel.ShippingAddress != null && detailOrderModel.OrderID > 0)
                {
                    Country country = countryService.GetByPrimaryKey(SNumber.ToNumber(detailOrderModel.ShippingAddress.CountryID));
                    if (country != null)
                    {
                        detailOrderModel.ShippingAddress.CountryName = country.CountryName;
                    }
                    int shippingAddressID = shippingAddressService.Insert(detailOrderModel.ShippingAddress);
                    if (shippingAddressID > 0)
                    {
                        TblOrder order = orderService.GetByPrimaryKey(detailOrderModel.OrderID);
                        order.ShippingAddressID = shippingAddressID;
                        order.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                        if (orderService.Update(order))
                        {
                            return RedirectToAction("detail", "orders", new { id = detailOrderModel.OrderID, strMessage = "addShippingAddress1" });
                        }
                    }
                }
                return RedirectToAction("detail", "orders", new { id = detailOrderModel.OrderID, strMessage = "addShippingAddress0" });
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return RedirectToAction("detail", "orders", new { id = detailOrderModel.OrderID, strMessage = "addShippingAddress0" });
            }
        }

        public ActionResult editShippingAddress(DetailOrderModel detailOrderModel)
        {
            try
            {
                if (detailOrderModel.ShippingAddress != null && detailOrderModel.ShippingAddressID > 0)
                {
                    detailOrderModel.ShippingAddress.ShippingAddressID = detailOrderModel.ShippingAddressID;
                    Country country = countryService.GetByPrimaryKey(SNumber.ToNumber(detailOrderModel.ShippingAddress.CountryID));
                    if (country != null)
                    {
                        detailOrderModel.ShippingAddress.CountryName = country.CountryName;
                    }
                    if (shippingAddressService.Update(detailOrderModel.ShippingAddress))
                    {
                        return RedirectToAction("detail", "orders", new { id = detailOrderModel.OrderID, strMessage = "editShippingAddress1" });
                    }
                    return RedirectToAction("detail", "orders", new { id = detailOrderModel.OrderID, strMessage = "editShippingAddress0" });
                }
                return RedirectToAction("index", "orders", new { strMessage = "notExist" });
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return RedirectToAction("detail", "orders", new { id = detailOrderModel.OrderID, strMessage = "editShippingAddress0" });
            }
        }
        public ActionResult editEmail(DetailOrderModel detailOrderModel)
        {
            try
            {
                if (detailOrderModel.OrderID > 0)
                {
                    TblOrder order = orderService.GetByPrimaryKey(detailOrderModel.OrderID);
                    order.CustomerEmail = detailOrderModel.CustomerEmail;
                    order.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                    if (orderService.Update(order))
                    {
                        return RedirectToAction("detail", "orders", new { id = detailOrderModel.OrderID, strMessage = "editEmail1" });
                    }
                }
                return RedirectToAction("index", "orders", new { strMessage = "notExist" });
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return RedirectToAction("detail", "orders", new { id = detailOrderModel.OrderID, strMessage = "editEmail0" });
            }
        }
    }
}