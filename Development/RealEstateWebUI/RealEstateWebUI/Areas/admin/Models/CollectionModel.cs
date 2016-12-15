using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealEstateWebUI.Areas.admin.Models
{
    public class CollectionModel
    {
        public string txtConditionFind { get; set; }
        public string ddlConditionFilter { get; set; }
        public string ddlDisplayStatus { get; set; }
        public string ddlCollectionStyle { get; set; }
        public IPagedList<Collection> lstCollection { get; set; }   
    }


    public class CollectiontDropdownModel
    {
        public int ProductID { get; set; }

        public List<CollectionDropDown> CollectionDropDowns { get; set; }
    }
    public class CollectionDropDown
    {
        public string CollectionName { get; set; }
        public int CollectionID { get; set; }
        public bool Choice { get; set; }
    }
}