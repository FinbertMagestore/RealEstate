namespace RealEstateWebUI.Areas.admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TblOrder")]
    public partial class TblOrder
    {
        public TblOrder()
        {
        }

        public int OrderID { get; set; }
        [Display(Name = "Ghi chú")]
        public string OrderNote { get; set; }
        public Nullable<int> OrderStatus { get; set; }
        public string CustomerEmail { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public string OrderName { get; set; }
        public int Number { get; set; }
        public int OrderNumber { get; set; }
        public string CreatedDateTime { get; set; }
        public string ModifiedDateTime { get; set; }
        public decimal TotalCount { get; set; }
        public decimal TotalShipping { get; set; }
        public string ShippingStatus { get; set; }
        public string BillingStatus { get; set; }
        public Nullable<int> ShippingAddressID { get; set; }
        public Nullable<int> BillingAddressID { get; set; }
        public string Tags { get; set; }
    }
}
