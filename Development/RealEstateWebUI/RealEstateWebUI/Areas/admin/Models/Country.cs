namespace RealEstateWebUI.Areas.admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Country")]
    public partial class Country
    {
        public Country()
        {
            this.AddressBooks = new HashSet<AddressBook>();
        }

        public int CountryID { get; set; }
        public string CountryName { get; set; }

        public virtual ICollection<AddressBook> AddressBooks { get; set; }
    }
}
