namespace RealEstateWebUI.Areas.admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Customer")]
    public partial class Customer
    {
        public Customer()
        {
            this.AddressBook = new AddressBook();
            this.AddressBooks = new HashSet<AddressBook>();
            this.TblOrders = new List<TblOrder>();
            this.ListTag = new List<Tag>();
        }

        public int CustomerID { get; set; }

        [Display(Name = "Họ")]
        public string CustomerFirstName { get; set; }

        [Display(Name = "Tên")]
        public string CustomerLastName { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng")]
        public string CustomerEmail { get; set; }

        [Display(Name="Đăng ký nhận tin quảng cáo")]
        public bool AcceptsMarketing { get; set; }

        [Display(Name="Ghi chú")]
        public string CustomerNote { get; set; }

        public int CustomerState { get; set; }

        public string CreatedDateTime { get; set; }

        public string ModifiedDateTime { get; set; }

        public int TotalOrder { get; set; }

        public decimal TotalCount { get; set; }

        public string Tags { get; set; }

        public virtual List<Tag> ListTag { get; set; }

        public virtual ICollection<AddressBook> AddressBooks { get; set; }

        public virtual List<TblOrder> TblOrders { get; set; }

        public virtual AddressBook AddressBook { get; set; }
    }
}
