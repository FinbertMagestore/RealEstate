using RealEstateWebUI.Areas.admin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealEstateWebUI.Areas.admin.Models
{
    public class CollectionProduct
    {
        public CollectionProduct()
        {
            CollectionService collectionService = new CollectionService();
            ProductService productService = new ProductService();
            this.Collection = collectionService.GetByPrimaryKey(this.CollectionID);
            this.Product = productService.GetByPrimaryKey(this.ProductID);
            if (this.Collection == null)
            {
                this.Collection = new Collection();
            }
            if (this.Product == null)
            {
                this.Product = new Product();
            }
        }
        public int ID { get; set; }

        public int ProductID { get; set; }

        public int CollectionID { get; set; }

        public virtual Product Product { get; set; }
        public virtual Collection Collection { get; set; }
    }
}