namespace RealEstateWebUI.Areas.admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;
    [Table("Collection")]
    public partial class Collection
    {
        public Collection()
        {
            this.TblRules = new List<TblRule>();
        }

        public int CollectionID { get; set; }

        /// <summary>
        /// name of collection
        /// </summary>
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [Display(Name = "Tên danh mục")]
        public string CollectionName { get; set; }

        /// <summary>
        /// description of collection
        /// </summary>
        [AllowHtml]
        [Display(Name = "Mô tả")]
        public string CollectionDescription { get; set; }

        [Display(Name = "Thẻ tiêu đề")]
        public string PageTitle { get; set; }

        [Display(Name = "Thẻ mô tả")]
        public string PageDescription { get; set; }

        public bool CollectionState { get; set; }

        public string CollectionImage { get; set; }

        /// <summary>
        /// collection type: "custom" or "smart"
        /// </summary>
        public string CollectionType { get; set; }

        /// <summary>
        /// check CollectionType: true if is "custom", else false
        /// </summary>
        public bool Type { get; set; }

        public string CreatedDateTime { get; set; }

        public string ModifiedDateTime { get; set; }

        [Display(Name = "Đường dẫn / Alias")]
        public string UrlAlias { get; set; }

        [Display(Name = "Khung giao diện")]
        public string TemplateLayout { get; set; }

        /// <summary>
        /// if collection type is smart: true if product must unsatisfactorily all condition then belong to this collection
        /// false if need unsatisfactorily a in all condition
        /// </summary>
        public bool ConditionForCollection { get; set; }

        public virtual List<Product> Products { get; set; }

        public virtual List<CollectionProduct> CollectionProducts { get; set; }

        /// <summary>
        /// list template layout: collection or collection.list
        /// </summary>
        public virtual SelectList TemplateLayouts { get; set; }

        /// <summary>
        /// list all rule of collection if collection type is "smart"
        /// </summary>
        public virtual List<TblRule> TblRules { get; set; }

    }
}
