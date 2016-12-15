using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RealEstateWebUI.Areas.admin.Models
{
    [Table("TblOption")]
    public class TblOption
    {
        public int OptionID { get; set; }
        public string OptionName { get; set; }
        public int Position { get; set; }
        public string OptionValue { get; set; }
        public int ProductID { get; set; }
        public string CreatedDateTime { get; set; }
        public string ModifiedDateTime { get; set; }

        public List<string> OptionValues { get; set; }
    }

    public class OptionsOfProduct
    {
        public string[] Option1Values { get; set; }
        public string[] Option2Values { get; set; }
        public string[] Option3Values { get; set; }
        public List<TblOption> Options { get; set; }
        public List<Variant> Variants { get; set; }
        public int ProductID { get; set; }
    }
}