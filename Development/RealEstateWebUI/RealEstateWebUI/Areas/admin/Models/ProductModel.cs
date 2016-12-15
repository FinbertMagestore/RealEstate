using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RealEstateWebUI.Areas.admin.Models
{
    public class ProductModel
    {
        public string txtConditionFind { get; set; }
        public string ddlConditionFilter { get; set; }
        public string ddlDisplayStatus { get; set; }
        public string ddlProductStyle { get; set; }
        public string ddlSupplier { get; set; }
        public IPagedList<Product> lstProduct { get; set; }   
    }

    public class ProductDropdownModel
    {
        public int CollectionID { get; set; }

        public List<ProductDropDown> ListProductDropDown { get; set; }
    }
    public class ProductDropDown
    {
        public string ProductName { get; set; }
        public int ProductID { get; set; }
        public bool Choice { get; set; }
    }

    public class LineItemsOfOrder
    {
        public LineItemsOfOrder()
        {
            this.ProductVariants = new List<LineItem>();
        }
        public int OrderID { get; set; }
        public List<LineItem> ProductVariants { get; set; }
    }
    [Table("LineItem")]
    public class LineItem
    {
        public LineItem()
        {
            this.TblOrder = new TblOrder();
        }
        public int LineItemID { get; set; }
        public bool Choice { get; set; }
        public decimal Price { get; set; }
        public bool CanChoice { get; set; }
        public int VariantID { get; set; }
        public int ProductID { get; set; }
        public string ObjectName { get; set; }
        public string ProductName { get; set; }
        public string VariantName { get; set; }
        public bool IsDefault { get; set; }
        public int Quantity { get; set; }
        public string SKU { get; set; }
        public int OrderID { get; set; }
        public string ShippingStatus { get; set; }
        public string ImageUrl { get; set; }
        public virtual TblOrder TblOrder { get; set; }
    }
}