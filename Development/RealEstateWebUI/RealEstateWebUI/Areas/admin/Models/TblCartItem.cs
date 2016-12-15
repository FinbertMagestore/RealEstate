using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealEstateWebUI.Areas.admin.Models
{
    public class TblCartItem
    {
        public int CartItemID { get; set; }
        public int VariantID { get; set; }
        public int NumberVariant { get; set; }
        public int CartID { get; set; }
        public string CreatedDateTime { get; set; }
        public string ModifiedDateTime { get; set; }
        public int OrderID { get; set; }

        public virtual TblCart TblCart { get; set; }
        public virtual Variant Variant { get; set; }
    }
}