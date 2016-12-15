using PagedList;
using RealEstateWebUI.Areas.admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealEstateWebUI.Areas.client.Models
{
    public class ProductClientViewModel
    {
        public string sortOrder { get; set; }
        public int numberView { get; set; }
        public int pageNumber { get; set; }
        public IPagedList<Product> Products { get; set; }
        public int CountProduct { get; set; }
        public string view { get; set; }

    }
}