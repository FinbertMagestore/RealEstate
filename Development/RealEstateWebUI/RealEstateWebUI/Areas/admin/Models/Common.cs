using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RealEstateWebUI.Areas.admin.Models
{
    public class Common
    {
        #region Trạng thái
        // trạng thái giao hàng
        public static string Unfulfilled = "unfulfilled";
        //public static string Partial = "partial";
        public static string Fulfilled = "fulfilled";

        // trạng thái thanh toán
        public static string Pending = "pending";
        //public static string Partially_paid = "partially_paid";
        public static string Paid = "paid";
        //public static string Partially_refunded = "partially_refunded";
        //public static string Refunded = "refunded";


        public static int Active = 1;
        public static int InActive = 0;

        public enum Status
        {
            [Description("Active")]
            Active = 1,
            [Description("Inactive")]
            InActive = 0,
        }

        #endregion

        #region Thao tác với CSDL
        public enum ActionID
        {
            [Description("Xem")]
            View = 1,
            [Description("Thêm mới")]
            Insert = 2,
            [Description("Cập nhật")]
            Update = 3,
            [Description("Xóa")]
            Delete = 4,
        }
        public enum TableName
        {
            [Description(" tài khoản")]
            Account = 1,
            [Description(" khách hàng")]
            Customer = 2,
            [Description(" đơn hàng")]
            TblOrder = 3,
            [Description(" sản phẩm")]
            Product = 4,
            [Description(" danh mục sản phẩm")]
            Collection = 5,
        }
        #endregion

        #region message
        public static string UpdateSuccess = "Cập nhật thành công";
        public static string UpdateFail = "Cập nhật thất bại";
        public static string InsertSuccess = "Thêm mới thành công";
        public static string InsertFail = "Thêm mới thất bại";
        public static string DeleteSuccess = "Xoá thành công";
        public static string DeleteFail = "Xoá thất bại";
        #endregion

        /// <summary>
        /// email use to send message
        /// </summary>
        public static string Email = "ngovanhuy.cntt2@gmail.com";
        /// <summary>
        /// username to creadential
        /// </summary>
        public static string CredentialUserName = "ngovanhuy.cntt2@gmail.com";
        /// <summary>
        /// passsword to creadential
        /// </summary>
        public static string CredentialPassword = "ngovanhuy";

        /// <summary>
        /// connect string to db
        /// </summary>
        public static string ConnectString = @"Data Source=DESKTOP-JGEUVHH\SUCCESS;Initial Catalog=RealEstate;Persist Security Info=True;User ID=sa;Password=ngovanhuy;MultipleActiveResultSets=True;Application Name=EntityFramework";
        //public static string ConnectString = @"workstation id=RealEstate20160814.mssql.somee.com;packet size=4096;user id=ngovanhuy0241_SQLLogin_1;pwd=7ytr3bbrea;data source=RealEstate20160814.mssql.somee.com;persist security info=False;initial catalog=RealEstate20160814";

        public static string UrlHost = "http://localhost:6060/";

        public static string StoreName = "RealEstate ";

        public static List<SelectListItem> ListFilterConditionColumn = new List<SelectListItem>
                {
                    new SelectListItem {Text="Tên sản phẩm", Value="ProductName" },
                    new SelectListItem {Text="Loại sản phẩm", Value="ProductStyle" },
                    new SelectListItem {Text="Nhà sản xuất", Value="Supplier" },
                };

        public static List<SelectListItem> ListFilterConditionRelation = new List<SelectListItem>
                {
                    new SelectListItem {Text="bằng", Value="equals" },
                    new SelectListItem {Text="chứa từ", Value="contains" },
                    //new SelectListItem {Selected= true, Text="lớn hơn", Value="2" },
                    //new SelectListItem {Selected= true, Text="nhỏ hơn", Value="3" },
                    //new SelectListItem {Selected= true, Text="bắt đầu với", Value="4" },
                    //new SelectListItem {Selected= true, Text="kết thúc với", Value="5" },
                };

        public static string[] Units = { "lb", "oz", "kg", "g" };

        public static int BaseNumberOrder = 1000;

        /// <summary>
        /// id of collection discount price
        /// </summary>
        public static int CollectionID_DiscountPrice = 38;

        public static string CookieCart = "cart";

        public static int Country_VietNameID = 1;
    }
}