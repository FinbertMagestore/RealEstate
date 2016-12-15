namespace RealEstateWebUI.Areas.admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Supplier")]
    public partial class Supplier
    {
        public Supplier()
        {
            this.Products = new HashSet<Product>();
        }

        public int SupplierID { get; set; }
        public string SupplierName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
