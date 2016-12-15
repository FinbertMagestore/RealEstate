namespace RealEstateWebUI.Areas.admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("AspNetRole")]
    public partial class AspNetRole
    {
        public AspNetRole()
        {
            this.AppUsers = new HashSet<AppUser>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AppUser> AppUsers { get; set; }
    }
}
