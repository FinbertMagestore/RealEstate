namespace RealEstateWebUI.Areas.admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Tag")]
    public partial class Tag
    {
        public Tag()
        {

        }
        public int TagID { get; set; }
        public string TagName { get; set; }
        public Nullable<int> TableNameID { get; set; }
    }
}
