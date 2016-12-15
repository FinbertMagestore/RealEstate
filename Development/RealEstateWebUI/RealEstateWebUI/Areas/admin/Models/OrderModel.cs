using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RealEstateWebUI.Areas.admin.Models
{
    public class OrderModel
    {
        public string txtConditionFind { get; set; }

        public string ddlConditionFilter { get; set; }

        public string ddlOrderStatus { get; set; }

        public string ddlBillingStatus { get; set; }

        public string ddlShippingStatus { get; set; }

        public string txtCustomer { get; set; }

        public IPagedList<TblOrder> lstTblOrder { get; set; }
    }

    public class CreateOrderModel
    {
        public CreateOrderModel()
        {
            this.Customer = new Customer();
            this.ShippingAddress = new ShippingAddress();
            this.BillingAddress = new BillingAddress();
            this.LineItems = new List<LineItem>();
        }

        public int OrderID { get; set; }
        [Display(Name = "Ghi chú")]
        public string OrderNote { get; set; }
        public Nullable<int> OrderStatus { get; set; }
        //public Nullable<decimal> ShippingCost { get; set; }
        //public Nullable<int> MethodShipping { get; set; }
        public string CustomerEmail { get; set; }
        //public string UrlProduct { get; set; }
        public Nullable<int> CustomerID { get; set; }
        //public Nullable<int> ProductID { get; set; }
        public string OrderName { get; set; }
        public int Number { get; set; }
        public int OrderNumber { get; set; }
        public string CreatedDateTime { get; set; }
        public string ModifiedDateTime { get; set; }
        public decimal TotalCount { get; set; }
        public string ShippingStatus { get; set; }
        public string BillingStatus { get; set; }
        public int ShippingAddressID { get; set; }
        public int BillingAddressID { get; set; }
        public string Tags { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ShippingAddress ShippingAddress { get; set; }
        public virtual BillingAddress BillingAddress { get; set; }
        public virtual List<LineItem> LineItems { get; set; }

    }

    public class DetailOrderModel
    {
        public DetailOrderModel()
        {
            this.Customer = new Customer();
            this.ShippingAddress = new ShippingAddress();
            this.BillingAddress = new BillingAddress();
            this.LineItemsPending = new List<LineItem>();
            this.LineItemsPaid = new List<LineItem>();
            this.ListTag = new List<Tag>();
        }

        public int OrderID { get; set; }
        [Display(Name = "Ghi chú")]
        public string OrderNote { get; set; }
        public Nullable<int> OrderStatus { get; set; }
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng")]
        public string CustomerEmail { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public string OrderName { get; set; }
        public int Number { get; set; }
        public int OrderNumber { get; set; }
        public string CreatedDateTime { get; set; }
        public string ModifiedDateTime { get; set; }
        public decimal TotalCount { get; set; }
        public string ShippingStatus { get; set; }
        public string BillingStatus { get; set; }
        public int ShippingAddressID { get; set; }
        public int BillingAddressID { get; set; }
        public string Tags { get; set; }
        public virtual List<Tag> ListTag { get; set; }


        public virtual Customer Customer { get; set; }
        public virtual ShippingAddress ShippingAddress { get; set; }
        public virtual BillingAddress BillingAddress { get; set; }
        public virtual List<LineItem> LineItemsPending { get; set; }
        public virtual List<LineItem> LineItemsPaid { get; set; }

    }
}