using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RealEstateWebUI.Areas.admin.Models
{
    [Table("District")]
    public class District
    {
        public int DistrictID { get; set; }
        public string DistrictName { get; set; }
    }
}