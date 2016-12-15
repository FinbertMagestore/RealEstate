using RealEstateWebUI.Areas.admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealEstateWebUI.Areas.client.Models
{
    public class HomeClientViewModel
    {
        public List<Product> ProductsBestSelling { get; set; }
        public List<Product> ProductsCreateDESC { get; set; }
        public List<Product> ProductsDiscountPrice { get; set; }


    }
}