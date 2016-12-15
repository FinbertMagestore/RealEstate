namespace RealEstateWebUI.Areas.admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;
    [Table("TblRule")]
    public partial class TblRule
    {
        public TblRule()
        {
            this.ListColums = new SelectList(Common.ListFilterConditionColumn, "Value", "Text", 1);
            this.ListRelation = new SelectList(Common.ListFilterConditionRelation, "Value", "Text", 1);
        }

        public int RuleID { get; set; }
        public int CollectionID { get; set; }
        public string ColumnName { get; set; }
        public string Relation { get; set; }
        public string ConditionValue { get; set; }

        public virtual Collection Collection{get; set;}


        public virtual SelectList ListColums { get; set; }
        public virtual SelectList ListRelation { get; set; }
    }
}
