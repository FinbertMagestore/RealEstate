namespace RealEstateWebUI.Areas.admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("AddressBook")]
    public partial class AddressBook
    {
        public AddressBook()
        {
            this.Customers = new HashSet<Customer>();
            //this.Country = new Country();
            this.Countries = new List<Country>();
        }

        public int AddressBookID { get; set; }
        [Display(Name = "Họ")]
        public string AddressBookFirstName { get; set; }
        [Display(Name = "Tên")]
        public string AddressBookLastName { get; set; }
        [Display(Name = "Công ty")]
        public string CompanyName { get; set; }

        [Display(Name = "Điện thoại")]
        //[DataType(DataType.PhoneNumber)]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Điện thoại không đúng định dạng")]
        public string Phone { get; set; }
        [Display(Name = "Địa chỉ")]

        public string HomeAddress { get; set; }

        [Display(Name = "Tỉnh / Thành phố")]
        public string ProvinceName { get; set; }
        public Nullable<int> ProvinceID { get; set; }
        public Nullable<int> DistrictID { get; set; }
        [Display(Name = "Postal / Zip Code")]
        public string Postal { get; set; }
        public Nullable<int> CustomerID { get; set; }
        [Display(Name = "Quốc gia")]
        public Nullable<int> CountryID { get; set; }
        public bool IsDefault { get; set; }

        public virtual Country Country { get; set; }
        public virtual List<Country> Countries { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual District District { get; set; }
        public virtual Province Province { get; set; }
    }
}
