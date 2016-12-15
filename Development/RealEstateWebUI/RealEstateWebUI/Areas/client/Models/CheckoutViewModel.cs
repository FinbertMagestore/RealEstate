using RealEstateWebUI.Areas.admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RealEstateWebUI.Areas.client.Models
{
    public class CheckoutViewModel
    {
        public CheckoutViewModel()
        {
            TotalShipping = 500000;
            this.Provinces = new List<Province>();
            this.Districts = new List<District>();
            this.ShippingDistrict = this.BillingDistrict = new District();
            this.ShippingProvince = this.BillingProvince = new Province();
            this.CartItems = new List<TblCartItem>();
        }

        public string CookieID { get; set; }
        public int CartID { get; set; }
        [Required]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng")]
        public string CustomerEmail { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        //public string BillingName { get; set; }
        //public string BillingPhone { get; set; }
        //public string BillingAddress { get; set; }
        public Nullable<int> BillingProvinceID { get; set; }
        public Nullable<int> BillingDistrictID { get; set; }
        //public string ShippingName { get; set; }
        //public string ShippingPhone { get; set; }
        //public string ShippingAddress { get; set; }
        public Nullable<int> ShippingProvinceID { get; set; }
        public Nullable<int> ShippingDistrictID { get; set; }
        public string CheckoutNote { get; set; }
        [Display(Name = "Giao hàng đến địa chỉ khác")]
        public bool OtherShippingAddress { get; set; }
        public int CheckoutType { get; set; }
        public decimal TotalSubPrice { get; set; }
        public decimal TotalShipping { get; set; }
        public decimal TotalPrice { get; set; }

        public BillingAddress BillingAddress { get; set; }
        public ShippingAddress ShippingAddress { get; set; }

        public virtual District BillingDistrict { get; set; }
        public virtual District ShippingDistrict { get; set; }
        public virtual List<District> Districts { get; set; }
        public virtual Province BillingProvince { get; set; }
        public virtual Province ShippingProvince { get; set; }
        public virtual List<Province> Provinces { get; set; }
        public virtual List<TblCartItem> CartItems { get; set; }
    }
    
    public class ThankyouViewModel
    {
        public int OrderID { get; set; }
        public virtual TblOrder Order { get; set; }
        public decimal TotalSubPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalShipping { get; set; }
        public virtual List<TblCartItem> CartItems { get; set; }
        public virtual List<LineItem> LineItems { get; set; }
        public Nullable<int> BillingAddresID { get; set; }
        public BillingAddress BillingAddress { get; set; }
        public Nullable<int> ShippingAddressID { get; set; }
        public ShippingAddress ShippingAddress { get; set; }
        public string CustomerEmail { get; set; }
    }
}