namespace RealEstateWebUI.Areas.admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web;
    using System.Web.Mvc;

    [Table("Product")]
    public partial class Product
    {
        public Product()
        {
            this.Variant = new Variant();
            this.Variants = new List<Variant>();
            this.TblOrders = new List<TblOrder>();
            this.Images = new List<TblImage>();
            this.UploadeImages = new List<HttpPostedFileBase>();
            this.ListTag = new List<Tag>();
            this.ProductsRelation = new List<Product>();
        }

        public int ProductID { get; set; }
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; }
        [Display(Name = "Nội dung")]
        [AllowHtml]
        public string ProductContent { get; set; }
        [Display(Name = "Mô tả ngắn")]
        [AllowHtml]
        public string ProductShortDescription { get; set; }
        [Display(Name = "Thẻ tiêu đề")]
        public string ProductTitleCard { get; set; }
        [Display(Name = "Thẻ mô tả")]
        public string ProductDescriptionCard { get; set; }
        [Display(Name = "Đường dẫn / Alias")]
        public string ProductAlias { get; set; }
        public Nullable<bool> ProductState { get; set; }
        [Display(Name = "Nhà cung cấp")]
        public Nullable<int> SupplierID { get; set; }
        [Display(Name = "Loại")]
        public Nullable<int> ProductStyleID { get; set; }
        public string ProductUrl { get; set; }
        public string CreatedDateTime { get; set; }
        public string ModifiedDateTime { get; set; }
        public string Tags { get; set; }
        public virtual List<Tag> ListTag { get; set; }
        public bool IsNew { get; set; }
        public bool IsSale { get; set; }
        /// <summary>
        /// variant is created default
        /// </summary>
        public virtual Variant Variant { get; set; }
        
        public virtual List<Variant> Variants { get; set; }
        /// <summary>
        /// variable check when crate product, is create option and variant
        /// </summary>
        public bool AutoGenerate { get; set; }

        public virtual List<TblOption> Options { get; set; }

        public virtual Collection Collection { get; set; }
        public virtual ProductStyle ProductStyle { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual List<TblOrder> TblOrders { get; set; }
        public virtual List<TblImage> Images { get; set; }
        public SelectList ProductStyles { get; set; }
        public SelectList Suppliers { get; set; }
        public List<HttpPostedFileBase> UploadeImages { get; set; }
        public List<CollectionProduct> CollectionProducts { get; set; }

        //view at client
        public List<Product> ProductsRelation { get; set; }
    }
}