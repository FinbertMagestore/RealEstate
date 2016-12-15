using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RealEstateWebUI.Areas.admin.Models
{
    [Table("Cart")]
    public class TblCart
    {
        public int CartID { get; set; }
        public string CookieID { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public string CreatedDateTime { get; set; }
        public string ModifiedDateTime { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalPriceAddVAT { get; set; }


        public virtual Customer Customer { get; set; }
        public virtual List<TblCartItem> CartItems { get; set; }
    }
}