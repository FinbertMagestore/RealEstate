namespace RealEstateWebUI.Areas.admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("AppUser")]
    public partial class AppUser
    {
        public AppUser()
        {
            this.AspNetUserClaims = new HashSet<AspNetUserClaim>();
            this.AspNetUserLogins = new HashSet<AspNetUserLogin>();
            this.AspNetRoles = new HashSet<AspNetRole>();
        }

        public int Id { get; set; }
        [Display(Name = "Họ tên")]
        public string Username { get; set; }

        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng")]
        public string Email { get; set; }
        public System.DateTime DateCreated { get; set; }
        public Nullable<System.DateTime> LastActivityDate { get; set; }
        public string SecurityStamp { get; set; }

        [Display(Name = "Trang chính")]
        public string MainPage { get; set; }

        [Display(Name = "Thông tin giới thiệu")]
        public string UserDescription { get; set; }

        [Display(Name = "Điện thoại")]
        //[DataType(DataType.PhoneNumber)]
        //[RegularExpression(@"^\(?([0-9]{4})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Điện thoại không đúng định dạng")]
        public string UserPhone { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu mới và xác nhận mật khẩu không trùng nhau")]
        public string ConfirmPassword { get; set; }

        public string CreatedDateTime { get; set; }

        public string ModifiedDateTime { get; set; }
        public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual ICollection<AspNetRole> AspNetRoles { get; set; }
    }
}