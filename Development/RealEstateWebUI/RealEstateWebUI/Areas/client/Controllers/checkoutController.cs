using RealEstateWebUI.Areas.admin.Models;
using RealEstateWebUI.Areas.admin.Services;
using RealEstateWebUI.Areas.admin.UtilzGeneral;
using RealEstateWebUI.Areas.client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RealEstateWebUI.Areas.client.Controllers
{
    public class checkoutController : Controller
    {
        private OrderService orderService = new OrderService();
        private LineItemService lineItemService = new LineItemService();
        private CartService cartService = new CartService();
        private CartItemService cartItemService = new CartItemService();
        private VariantService variantService = new VariantService();
        private ProductService productService = new ProductService();
        private ImageService imageService = new ImageService();
        private ProvinceService provinceService = new ProvinceService();
        private DistrictService districtService = new DistrictService();
        private CountryService countryService = new CountryService();
        private AddressBookService addressBookService = new AddressBookService();
        private BillingAddressService billingAddressService = new BillingAddressService();
        private ShippingAddressService shippingAddressService = new ShippingAddressService();
        private CustomerService customerService = new CustomerService();
        // GET: client/checkout

        public ActionResult index(string cookieID)
        {
            try
            {
                TblCart cart = cartService.GetByCookieID(cookieID);
                if (cart == null)
                {
                    cookieID = retrieveCookie();
                    return RedirectToAction("index", new { cookieID = cookieID });
                }

                cart.CartItems = cartItemService.GetByCartID(cart.CartID);
                if (cart.CartItems == null || cart.CartItems.Count == 0)
                {
                    return RedirectToAction("index", "cart", new { area = "client"});
                }
                else
                {
                    foreach (var item in cart.CartItems)
                    {
                        item.Variant = variantService.GetByPrimaryKey(item.VariantID);
                        if (item.Variant != null)
                        {
                            item.Variant.Product = productService.GetByPrimaryKey(item.Variant.ProductID);
                            if (item.Variant.Product != null)
                            {
                                item.Variant.Product.Images = imageService.GetByProductID(item.Variant.Product.ProductID);
                            }
                        }
                    }
                }
                cart.TotalPriceAddVAT = cart.TotalPrice + (decimal)((double)cart.TotalPrice * 0.1);

                CheckoutViewModel checkoutViewModel = new CheckoutViewModel();
                checkoutViewModel.CartID = cart.CartID;
                checkoutViewModel.CartItems = cart.CartItems;
                checkoutViewModel.Provinces = provinceService.GetAll();
                checkoutViewModel.BillingProvinceID = checkoutViewModel.ShippingProvinceID = 1;
                checkoutViewModel.Districts = districtService.GetByProvinceID(1);
                checkoutViewModel.Districts.Insert(0, new District { DistrictID = 0, DistrictName = "--- Chọn quận huyện ---" });
                checkoutViewModel.TotalSubPrice = cart.TotalPriceAddVAT;
                checkoutViewModel.TotalPrice = checkoutViewModel.TotalSubPrice + checkoutViewModel.TotalShipping;
                checkoutViewModel.CookieID = cookieID;
                return View(checkoutViewModel);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                throw;
            }
        }
        [HttpPost]
        public ActionResult index(CheckoutViewModel checkoutViewModel)
        {
            try
            {
                if (checkoutViewModel.BillingDistrictID == 0)
                {
                    checkoutViewModel.BillingDistrictID = null;
                }
                if (checkoutViewModel.BillingProvinceID == 0)
                {
                    checkoutViewModel.BillingProvinceID = null;
                }
                if (checkoutViewModel.ShippingDistrictID == 0)
                {
                    checkoutViewModel.ShippingDistrictID = null;
                }
                if (checkoutViewModel.ShippingProvinceID == 0)
                {
                    checkoutViewModel.ShippingProvinceID = null;
                }

                TblCart cart = cartService.GetByCookieID(checkoutViewModel.CookieID);
                cart.CartItems = cartItemService.GetByCartID(cart.CartID);
                if (cart.CartItems != null && cart.CartItems.Count > 0)
                {
                    foreach (var item in cart.CartItems)
                    {
                        item.Variant = variantService.GetByPrimaryKey(item.VariantID);
                        if (item.Variant != null)
                        {
                            item.Variant.Product = productService.GetByPrimaryKey(item.Variant.ProductID);
                        }
                    }
                }
                checkoutViewModel.CartItems = cart.CartItems;
                checkoutViewModel.CartID = cart.CartID;

                // create or update customer
                Customer customer = customerService.GetByEmail(checkoutViewModel.CustomerEmail);
                if (customer == null)
                {
                    customer = new Customer();
                    customer.CustomerFirstName = checkoutViewModel.BillingAddress.CustomerName;
                    customer.CustomerEmail = checkoutViewModel.CustomerEmail;
                    customer.TotalCount = 0;
                    customer.TotalOrder = 1;
                    customer.CreatedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                    customer.CustomerID = customerService.Insert(customer);
                }
                else
                {
                    customer.TotalCount += checkoutViewModel.TotalPrice;
                    customer.TotalOrder += 1;
                    customer.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                    customerService.Update(customer);
                }

                AddressBook addressBook = new AddressBook();
                addressBook.AddressBookFirstName = checkoutViewModel.BillingAddress.CustomerName;
                addressBook.Phone = checkoutViewModel.BillingAddress.Phone;
                addressBook.HomeAddress = checkoutViewModel.BillingAddress.HomeAddress;
                addressBook.DistrictID = checkoutViewModel.BillingDistrictID;
                addressBook.ProvinceID = checkoutViewModel.BillingProvinceID;
                addressBook.ProvinceName = provinceService.GetProvinceName(checkoutViewModel.BillingProvinceID == null ? 0 : checkoutViewModel.BillingProvinceID.Value);
                addressBook.CountryID = Common.Country_VietNameID;
                addressBook.CustomerID = customer.CustomerID;
                addressBook.IsDefault = true;
                addressBookService.Insert(addressBook);

                if (checkoutViewModel.OtherShippingAddress)
                {
                    addressBook.AddressBookFirstName = checkoutViewModel.ShippingAddress.CustomerName;
                    addressBook.Phone = checkoutViewModel.ShippingAddress.Phone;
                    addressBook.HomeAddress = checkoutViewModel.ShippingAddress.HomeAddress;
                    addressBook.DistrictID = checkoutViewModel.ShippingDistrictID;
                    addressBook.ProvinceID = checkoutViewModel.ShippingProvinceID;
                    addressBook.ProvinceName = provinceService.GetProvinceName(checkoutViewModel.ShippingProvinceID == null ? 0 : checkoutViewModel.ShippingProvinceID.Value);
                    addressBook.CountryID = Common.Country_VietNameID;
                    addressBook.CustomerID = customer.CustomerID;
                    addressBook.IsDefault = false;
                    addressBookService.Insert(addressBook);
                }

                int billingAddressID = 0, shippingAddressID = 0;
                // contructor billing address
                checkoutViewModel.BillingAddress.ProvinceID = checkoutViewModel.BillingProvinceID;
                checkoutViewModel.BillingAddress.ProvinceName = provinceService.GetProvinceName(checkoutViewModel.BillingProvinceID == null ? 0 : checkoutViewModel.BillingProvinceID.Value);
                checkoutViewModel.BillingAddress.CountryID = 1;
                if (SNumber.ToNumber(checkoutViewModel.BillingDistrictID) > 0)
                {
                    checkoutViewModel.BillingAddress.DistrictID = checkoutViewModel.BillingDistrictID;
                    //checkoutViewModel.BillingAddress.DistrictName = districtService.get
                }
                else
                {
                    checkoutViewModel.BillingAddress.DistrictID = null;
                }

                // contructor shipping address
                // shipping address different billing address
                if (checkoutViewModel.OtherShippingAddress)
                {
                    checkoutViewModel.ShippingAddress.ProvinceID = checkoutViewModel.ShippingProvinceID;
                    checkoutViewModel.ShippingAddress.ProvinceName = provinceService.GetProvinceName(checkoutViewModel.ShippingProvinceID == null ? 0 : checkoutViewModel.ShippingProvinceID.Value);
                    checkoutViewModel.ShippingAddress.CountryID = 1;
                    if (SNumber.ToNumber(checkoutViewModel.ShippingDistrictID) > 0)
                    {
                        checkoutViewModel.ShippingAddress.DistrictID = checkoutViewModel.ShippingDistrictID;
                    }
                    else
                    {
                        checkoutViewModel.ShippingAddress.DistrictID = null;
                    }
                }
                // shipping address as billing address
                else
                {
                    checkoutViewModel.ShippingAddress.CustomerName = checkoutViewModel.BillingAddress.CustomerName;
                    checkoutViewModel.ShippingAddress.ProvinceID = checkoutViewModel.BillingProvinceID;
                    checkoutViewModel.ShippingAddress.ProvinceName = provinceService.GetProvinceName(checkoutViewModel.BillingProvinceID == null ? 0 : checkoutViewModel.BillingProvinceID.Value);
                    checkoutViewModel.ShippingAddress.CountryID = 1;
                    if (SNumber.ToNumber(checkoutViewModel.BillingDistrictID) > 0)
                    {
                        checkoutViewModel.ShippingAddress.DistrictID = checkoutViewModel.BillingDistrictID;
                    }
                    else
                    {
                        checkoutViewModel.ShippingAddress.DistrictID = null;
                    }
                }

                billingAddressID = billingAddressService.Insert(checkoutViewModel.BillingAddress);
                shippingAddressID = shippingAddressService.Insert(checkoutViewModel.ShippingAddress);

                // create order & line item
                TblOrder order = new TblOrder();
                order.OrderStatus = Common.Active;
                order.CustomerID = customer.CustomerID;
                order.BillingStatus = Common.Pending;
                order.ShippingStatus = Common.Unfulfilled;
                order.OrderNote = checkoutViewModel.CheckoutNote;
                order.TotalCount = checkoutViewModel.TotalSubPrice;
                order.TotalShipping = checkoutViewModel.TotalShipping;
                order.CustomerEmail = checkoutViewModel.CustomerEmail;
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
                order.Number = orderService.GetLastNumber() + 1;
                order.CreatedDateTime = order.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();

                int orderID = orderService.Insert(order);
                if (SNumber.ToNumber(orderID) > 0)
                {
                    if (checkoutViewModel.CartItems != null && checkoutViewModel.CartItems.Count > 0)
                    {
                        foreach (var item in checkoutViewModel.CartItems)
                        {
                            item.OrderID = orderID;
                            LineItem lineItem = cartItemService.ToLineItem(item);
                            lineItemService.Insert(lineItem);
                        }
                    }
                }

                // remove cart and cartItem
                if (cart != null)
                {
                    cartItemService.DeleteByCartID(cart.CartID);
                }
                return RedirectToAction("thankyou", "checkout", new { id = orderID });
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return RedirectToAction("index", "checkout", new { cookieID = checkoutViewModel.CookieID });
        }

        [HttpPost]
        public ActionResult getDistrictsByProvinceID(int id)
        {
            try
            {
                List<District> districts = new List<District>();
                districts = districtService.GetByProvinceID(id);
                return Json(districts, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                throw;
            }
        }
        /// <summary>
        /// create new cookie and return cookie id of cart new or return cookie id of cart exist
        /// </summary>
        /// <returns></returns>
        public string retrieveCookie()
        {
            try
            {
                HttpCookie myCookie = Request.Cookies[Common.CookieCart];
                if (myCookie != null)
                {
                    return myCookie.Value;
                }

                string cookieID = SString.RandomString(32);
                HttpCookie cookieObject = new HttpCookie(Common.CookieCart, cookieID);
                cookieObject.Expires = DateTime.Now.AddDays(14);
                Response.Cookies.Add(cookieObject);
                return cookieID;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                throw;
            }
        }

        public ActionResult thankyou(int id)
        {
            try
            {
                TblOrder order = orderService.GetByPrimaryKey(id);
                if (order == null)
                {
                    return RedirectToAction("index", "cart");
                }

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
                }

                ThankyouViewModel thankyouViewModel = new ThankyouViewModel();
                thankyouViewModel.CustomerEmail = order.CustomerEmail;
                thankyouViewModel.LineItems = lineItems;
                thankyouViewModel.ShippingAddressID = order.ShippingAddressID;
                thankyouViewModel.ShippingAddress = shippingAddressService.GetByPrimaryKey(thankyouViewModel.ShippingAddressID == null ? 0 : thankyouViewModel.ShippingAddressID.Value);
                thankyouViewModel.BillingAddresID = order.BillingAddressID;
                thankyouViewModel.BillingAddress = billingAddressService.GetByPrimaryKey(thankyouViewModel.BillingAddresID == null ? 0 : thankyouViewModel.BillingAddresID.Value);
                thankyouViewModel.TotalSubPrice = order.TotalCount;
                thankyouViewModel.TotalShipping = order.TotalShipping;
                thankyouViewModel.TotalPrice = order.TotalCount + order.TotalShipping;
                return View(thankyouViewModel);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                throw;
            }
        }
    }
}