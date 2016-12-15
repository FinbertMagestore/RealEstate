namespace RealEstateWebUI.Areas.admin.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TblLog")]
    public partial class TblLog
    {
        public TblLog()
        {
        }

        public decimal LogID { get; set; }
        public int UserID { get; set; }
        public int ActionID { get; set; }
        public int ObjectID { get; set; }
        public string DataTimeLog { get; set; }
        public string IP { get; set; }
        public int TableNameID { get; set; }

        public string Href2Object { get; set; }
        public string ObjectValue { get; set; }

        public virtual AppUser AppUser{get; set;}
        public virtual Product Product{get; set;}
    }
}
