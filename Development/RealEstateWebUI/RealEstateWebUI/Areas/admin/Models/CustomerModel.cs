using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RealEstateWebUI.Areas.admin.Models
{
    public class CustomerModel
    {
        public string txtConditionFind { get; set; }
        public string ddlConditionFilter { get; set; }
        public string ddlCompareTotalOrder { get; set; }
        public string txtTotalOrder { get; set; }
        public string ddlCompareTotalCount { get; set; }
        public string txtTotalCount { get; set; }
        public string ddlAcceptsMarketing { get; set; }
        public string ddlState { get; set; }
        public IPagedList<Customer> lstCustomer { get; set; }
    }

    public class ActiveAccountModel
    {
        public int CustomerID { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu mới và xác nhận mật khẩu không trùng nhau")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Gửi email thông báo tới khách hàng")]
        public bool SendEmail2Customer { get; set; }
    }

    public class DisableAccountModel
    {
        public int CustomerID { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu mới và xác nhận mật khẩu không trùng nhau")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Gửi email thông báo tới khách hàng")]
        public bool SendEmail2Customer { get; set; }
    }

    public class CustomerDropdownModel
    {
        public int OrderID { get; set; }

        public List<CustomerDropdown> CustomerDropdowns { get; set; }
    }
    public class CustomerDropdown
    {
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public int CustomerID { get; set; }
        public bool Choice { get; set; }
    }

}