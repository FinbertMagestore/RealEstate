namespace RealEstateWebUI.Areas.admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ProductStyle")]
    public partial class ProductStyle
    {
        public ProductStyle()
        {
            this.Products = new HashSet<Product>();
        }

        public int ProductStyleID { get; set; }
        public string ProductStyleName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
