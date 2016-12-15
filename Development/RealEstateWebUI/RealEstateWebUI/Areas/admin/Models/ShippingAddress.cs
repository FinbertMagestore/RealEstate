using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstateWebUI.Areas.admin.Models
{
    [Table("ShippingAddress")]
    public class ShippingAddress
    {
        public ShippingAddress()
        {
            this.Country = new Country();
            this.Countries = new List<Country>();
        }

        public int ShippingAddressID { get; set; }
        [Display(Name = "Điện thoại")]
        public string Phone { get; set; }
        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Địa chỉ giao hàng không được để trống")]
        public string HomeAddress { get; set; }
        [Display(Name = "Tỉnh / Thành phố")]
        public string ProvinceName { get; set; }
        public Nullable<int> ProvinceID { get; set; }
        public Nullable<int> DistrictID { get; set; }
        public string DistrictName { get; set; }
        [Display(Name = "Quốc gia")]
        public string CountryName { get; set; }
        public Nullable<int> CountryID { get; set; }
        [Display(Name = "Tên khách hàng")]
        public string CustomerName { get; set; }

        public virtual Country Country { get; set; }
        public virtual List<Country> Countries { get; set; }

        public virtual District District { get; set; }
        public virtual Province Province { get; set; }
    }
}