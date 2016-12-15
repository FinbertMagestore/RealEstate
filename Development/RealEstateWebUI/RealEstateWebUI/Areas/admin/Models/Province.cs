using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RealEstateWebUI.Areas.admin.Models
{
    [Table("Province")]
    public class Province
    {
        public int ProvinceID { get; set; }
        public string ProvinceName { get; set; }
    }
}