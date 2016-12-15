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
    public class cartController : Controller
    {
        private CartService cartService = new CartService();
        private CartItemService cartItemService = new CartItemService();
        private VariantService variantService = new VariantService();
        private ProductService productService = new ProductService();
        private ImageService imageService = new ImageService();
        private ProvinceService provinceService = new ProvinceService();
        private DistrictService districtService = new DistrictService();
        private AddressBookService addressBookService = new AddressBookService();
        private CustomerService customerService = new CustomerService();

        // GET: client/cart
        public ActionResult index(string message = "")
        {
            try
            {
                string cookieID = retrieveCookie();
                TblCart cartGetInCookie = cartService.GetByCookieID(cookieID);
                if (cartGetInCookie != null)
                {
                    cartGetInCookie.TotalPriceAddVAT = cartGetInCookie.TotalPrice + (decimal)((double)cartGetInCookie.TotalPrice * 0.1);

                    cartGetInCookie.CartItems = cartItemService.GetByCartID(cartGetInCookie.CartID);
                    if (cartGetInCookie.CartItems != null && cartGetInCookie.CartItems.Count > 0)
                    {
                        foreach (var item in cartGetInCookie.CartItems)
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
                }
                if (!string.IsNullOrEmpty(message))
                {
                    if (message.Equals("delete1"))
                    {
                        ViewBag.success = "Xoá 1 sản phẩm trong giỏ hàng thành công";
                    }
                    else if (message.Equals("delete0"))
                    {
                        ViewBag.error = "Xoá 1 sản phẩm trong giỏ hàng thất bại";
                    }
                    else if (message.Equals("update1"))
                    {
                        ViewBag.success = "Cật nhật giỏ hàng thành công";
                    }
                    else if (message.Equals("update0"))
                    {
                        ViewBag.error = "Cật nhật giỏ hàng thất bại";
                    }
                    else if (message.Equals("pay1"))
                    {
                        ViewBag.success = "Thanh toán giỏ hàng thành công";
                    }
                    else if (message.Equals("pay0"))
                    {
                        ViewBag.error = "Thanh toán giỏ hàng thất bại";
                    }
                }
                return View(cartGetInCookie);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                throw;
            }
        }

        [HttpPost]
        public ActionResult index(TblCart tblCart)
        {
            try
            {
                string cookieID = tblCart.CookieID;
                TblCart cartGetInCookie = cartService.GetByCookieID(cookieID);
                if (cartGetInCookie != null)
                {
                    cartGetInCookie.TotalPriceAddVAT = cartGetInCookie.TotalPrice + (decimal)((double)cartGetInCookie.TotalPrice * 0.1);

                    cartGetInCookie.CartItems = cartItemService.GetByCartID(cartGetInCookie.CartID);
                    if (cartGetInCookie.CartItems != null && cartGetInCookie.CartItems.Count > 0)
                    {
                        if (cartGetInCookie.CartItems.Count == tblCart.CartItems.Count)
                        {
                            for (int i = 0; i < cartGetInCookie.CartItems.Count; i++)
                            {
                                cartGetInCookie.CartItems[i].NumberVariant = tblCart.CartItems[i].NumberVariant;
                                cartItemService.Update(cartGetInCookie.CartItems[i]);
                            }
                        }
                    }
                    cartService.UpdateTotalPrice(cartGetInCookie.CartID);
                    return RedirectToAction("index", new { message = "update1" });
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                throw;
            }
            return RedirectToAction("index", new { message = "update0" });
        }

        public int addVariantToCart(int variantID, int numberVariant)
        {
            try
            {
                string cookieID = retrieveCookie();
                TblCart cart = cartService.GetByCookieID(cookieID);
                if (cart == null)
                {
                    cart = new TblCart();
                    cart.CookieID = cookieID;
                    cart.CreatedDateTime = SDateTime.GetYYYYMMddHmmSSNow();

                    cart.CartID = cartService.Insert(cart);
                }
                cart.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();

                Variant variant = variantService.GetByPrimaryKey(variantID);
                if (variant != null)
                {
                    string where = string.Format("CartID={0} and VariantID={1}", cart.CartID, variantID);
                    List<TblCartItem> cartItems = cartItemService.GetByWhere(where);
                    if (cartItems == null || cartItems.Count == 0)
                    {
                        TblCartItem cartItem = new TblCartItem();
                        cartItem.CartID = cart.CartID;
                        cartItem.VariantID = variantID;
                        cartItem.NumberVariant = numberVariant;
                        cartItem.CreatedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                        cartItemService.Insert(cartItem);
                    }
                    else
                    {
                        cartItems[0].NumberVariant++;
                        cartItemService.Update(cartItems[0]);
                    }

                    cartService.UpdateTotalPrice(cart.CartID);

                    cartItems = cartItemService.GetByCartID(cart.CartID);
                    if (cartItems != null)
                    {
                        return cartItems.Count;
                    }
                }

                return 0;
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

        public int GetNumberVariantInCart()
        {
            try
            {
                string cookieID = retrieveCookie();
                TblCart cart = cartService.GetByCookieID(cookieID);
                if (cart != null)
                {
                    List<TblCartItem> cartItems = cartItemService.GetByCartID(cart.CartID);
                    if (cartItems != null)
                    {
                        return cartItems.Count;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
            return 0;
        }
        
        public ActionResult removeCartItem(int id)
        {
            try
            {
                TblCartItem cartItem = cartItemService.GetByPrimaryKey(id);
                if (cartItem != null)
                {

                    TblCart cart = cartService.GetByPrimaryKey(cartItem.CartID);
                    if (cart != null)
                    {
                        cartItemService.DeleteByPrimary(id);
                        cartService.UpdateTotalPrice(cart.CartID);
                        return RedirectToAction("index", new { message = "delete1" });
                    }

                }
                return RedirectToAction("index", new { message = "delete0" });
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                throw;
            }
        }

        //public ActionResult pay()
        //{
        //    try
        //    {
        //        TblCart cart = cartService.GetByCookieID(retrieveCookie());
        //        cartItemService.DeleteByCartID(cart.CartID);
        //        cartService.DeleteByPrimary(cart.CartID);
        //        return RedirectToAction("index", "cart", new { area = "client", message = "pay1" });
        //    }
        //    catch (Exception ex)
        //    {
        //        LogService.WriteException(ex);
        //        return RedirectToAction("index", "cart", new { area = "client", message = "pay0" });
        //    }
        //}
    }
}