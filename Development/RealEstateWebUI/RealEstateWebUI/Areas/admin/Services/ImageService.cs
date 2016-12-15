using RealEstateWebUI.Areas.admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using RealEstateWebUI.Areas.admin.UtilzGeneral;

namespace RealEstateWebUI.Areas.admin.Services
{
    public class ImageService
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);
        public List<TblImage> GetAll()
        {
            try
            {
                string query = "select * from TblImage order by ImageID";
                List<TblImage> lstTblImage = connect.Query<TblImage>(query).ToList<TblImage>();
                return lstTblImage;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return new List<TblImage>();
            }
        }
        public TblImage GetByPrimaryKey(int? imageID)
        {
            try
            {
                if (imageID != null)
                {
                    string query = "select * from TblImage where ImageID = " + imageID;
                    TblImage TblImage = connect.Query<TblImage>(query).FirstOrDefault<TblImage>();
                    return TblImage;
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return null;
        }
        public List<TblImage> GetByProductID(int productID)
        {
            try
            {
                string query = "select * from TblImage where ProductID = " + productID;
                List<TblImage> TblImage = connect.Query<TblImage>(query).ToList<TblImage>();
                return TblImage;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public int Insert(TblImage tblImage)
        {
            try
            {
                string query = string.Format("insert into TblImage(ImageName ,ImageUrl, ImageSize, ProductID) values (N'{0}',N'{1}',{2},{3}) SELECT @@IDENTITY", tblImage.ImageName, tblImage.ImageUrl, tblImage.ImageSize, tblImage.ProductID);
                return connect.Query<int>(query).Single();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }
        public List<TblImage> SelectByWhere(string where)
        {
            try
            {
                string query = "select * from TblImage where " + where;
                return connect.Query<TblImage>(query).ToList<TblImage>();
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }


        public bool DeleteByPrimary(int imageID)
        {
            try
            {
                if (SNumber.ToNumber(imageID) <= 0)
                {
                    return false;
                }
                string query = "delete from TblImage where ImageID = " + imageID;
                return 0 < connect.Execute(query);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }
        public bool DeleteByProductID(int productID)
        {
            try
            {
                if (SNumber.ToNumber(productID) <= 0)
                {
                    return false;
                }
                string query = "delete from TblImage where ProductID = " + productID;
                return 0 < connect.Execute(query);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }
        public bool DeleteByVariantID(int variantID)
        {
            try
            {
                if (SNumber.ToNumber(variantID) <= 0)
                {
                    return false;
                }
                string query = "delete from TblImage where VariantID = " + variantID;
                return 0 < connect.Execute(query);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        /// <summary>
        /// delete all subfolder, subfile, this folder
        /// </summary>
        /// <param name="folder"></param>
        public static void DeleteFolder(string folder)
        {
            try
            {
                if (!string.IsNullOrEmpty(folder) && System.IO.Directory.Exists(folder))
                {
                    System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(folder);

                    foreach (System.IO.FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (System.IO.DirectoryInfo dir in di.GetDirectories())
                    {
                        dir.Delete(true);
                    }
                    System.IO.Directory.Delete(folder);
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
        }

        /// <summary>
        /// delete all subfolder, subfile, not delete this folder
        /// </summary>
        /// <param name="folder"></param>
        public static void DeleteSubFolder(string folder)
        {
            try
            {
                if (!string.IsNullOrEmpty(folder) && System.IO.Directory.Exists(folder))
                {
                    System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(folder);

                    foreach (System.IO.FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (System.IO.DirectoryInfo dir in di.GetDirectories())
                    {
                        dir.Delete(true);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
        }

        /// <summary>
        /// get path to image first of product
        /// </summary>
        /// <param name="productID">id of product</param>
        /// <returns>path to image</returns>
        public static string GetPathImageFirstOfProduct(int productID)
        {
            IDbConnection connect = new SqlConnection(Common.ConnectString);
            string path = "";
            string query = "select * from TblImage where ProductID = " + productID.ToString();
            TblImage image = connect.Query<TblImage>(query).FirstOrDefault<TblImage>();
            if (image != null)
            {
                path = image.ImageUrl;
            }
            return path;
        }
    }
}