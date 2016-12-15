namespace RealEstateWebUI.Areas.admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web;

    [Table("Variant")]
    public partial class Variant
    {
        public Variant()
        {
            this.Units = Common.Units;
        }

        public int VariantID { get; set; }
        public int ProductID { get; set; }
        public Nullable<int> ImageID { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }

        [Display(Name = "Giá")]
        //[RegularExpression("\\d+", ErrorMessage = "Giá không phải là số")]
        [Range(0, int.MaxValue, ErrorMessage = "Giá phải lớn hơn {1}")]
        public Nullable<decimal> VariantPrice { get; set; }
        [Display(Name = "Giá so sánh")]
        //[RegularExpression("\\d+", ErrorMessage = "Giá so sánh không phải là số")]
        [Range(0, int.MaxValue, ErrorMessage = "Giá so sánh phải phải lớn hơn {1}")]
        public Nullable<decimal> CompareWithPrice { get; set; }
        [Display(Name = "Giá đã bao gồm VAT")]
        public bool Textable { get; set; }
        [Display(Name = "Mã vạch / Barcode")]
        public string VariantBarcode { get; set; }
        [Display(Name = "Mã sản phẩm / SKU")]
        public string VariantSKU { get; set; }
        [Display(Name = "Cân nặng")]
        [Range(0, int.MaxValue, ErrorMessage = "Cân nặng phải phải lớn hơn {1}")]
        public Nullable<int> VariantWeight { get; set; }
        public string WeightUnit { get; set; }
        public virtual string[] Units { get; set; }
        [Display(Name = "Sản phẩm này yêu cầu vận chuyển")]
        public bool RequireShipping { get; set; }
        public string CreatedDateTime { get; set; }
        public string ModifiedDateTime { get; set; }
        public string VariantTittle { get; set; }

        public bool IsCreate { get; set; }
        public HttpPostedFileBase imageVariant { get; set; }
        public virtual Product Product { get; set; }
        public virtual TblImage Image { get; set; }
    }
}
