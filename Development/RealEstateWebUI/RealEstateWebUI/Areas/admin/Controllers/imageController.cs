using RealEstateWebUI.Areas.admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using System.IO;
using RealEstateWebUI.Areas.admin.UtilzGeneral;
using System.Net;
using RealEstateWebUI.Areas.admin.Services;

namespace RealEstateWebUI.Areas.admin.Controllers
{
    public class imageController : Controller
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);
        private VariantService variantService = new VariantService();
        private ImageService imageService = new ImageService();

        /// <summary>
        /// delete Image in table TblImage and delete image in folder
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productID"></param>
        /// <returns></returns>
        public ActionResult delete(int id, int productID)
        {
            try
            {
                string query = "select * from TblImage where ImageID = " + id.ToString();
                TblImage image = connect.Query<TblImage>(query).FirstOrDefault<TblImage>();
                if (image != null)
                {
                    string folder = Server.MapPath("~/assets/uploads/products/") + productID;
                    string pathImage = folder + "/" + image.ImageName;
                    System.IO.File.Delete(pathImage);
                    string where = "ProductID = " + productID + " and ImageID = " + id;
                    List<Variant> variants = variantService.GetByWhere(where);
                    if (variants != null && variants.Count > 0)
                    {
                        foreach (var item in variants)
                        {
                            item.ImageID = null;
                            variantService.Update(item);
                        }
                    }
                    if (imageService.DeleteByPrimary(id))
                    {
                        return RedirectToAction("detail", "products", new { id = productID, messageDelete = 1 });
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return RedirectToAction("detail", "products", new { id = productID, messageDelete = 0 });
        }

        public List<TblImage> GetListImageForProduct(int productID)
        {
            string query = "select * from TblImage where ProductID = " + productID.ToString();
            List<TblImage> image = connect.Query<TblImage>(query).ToList<TblImage>();
            return image;
        }

        /// <summary>
        /// intMessage = 1: if successs, = 0 if upload fail, 2 if file size > 1MB
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fileProductDetail"></param>
        /// <returns></returns>
        [HttpPost]
        public int uploadImageProduct(int id, HttpPostedFileBase fileProductDetail)
        {
            int intMessage = 0;
            try
            {
                if (fileProductDetail != null)
                {
                    if (fileProductDetail.ContentLength / 1024 / 1024 > 1)
                    {
                        intMessage = 2;
                    }
                    else if (fileProductDetail.ContentLength > 0)
                    {
                        try
                        {
                            string folder = Server.MapPath("~/assets/uploads/products/") + id;
                            Directory.CreateDirectory(folder);

                            string strWhere = "";
                            strWhere = "ImageName like N'" + fileProductDetail.FileName + "' and ProductID = " + id.ToString();
                            TblImage imageOfProduct = imageService.SelectByWhere(strWhere).FirstOrDefault();
                            if (imageOfProduct == null || imageOfProduct.ImageID <= 0)
                            {
                                string imageUrl = "";
                                imageUrl = HttpContext.Request.Url.Authority == string.Empty ? Common.UrlHost : "http://" + HttpContext.Request.Url.Authority;
                                if (!imageUrl.EndsWith("/")) imageUrl += "/";
                                imageUrl += "assets/uploads/products/" + id + "/" + fileProductDetail.FileName;

                                TblImage image = new TblImage();
                                image.ImageName = fileProductDetail.FileName;
                                image.ImageUrl = imageUrl;
                                image.ImageSize = fileProductDetail.ContentLength;
                                image.ProductID = id;
                                imageService.Insert(image);
                            }

                            string path = Path.Combine(folder, Path.GetFileName(fileProductDetail.FileName));
                            fileProductDetail.SaveAs(path);
                            intMessage = 1;
                        }
                        catch (Exception ex)
                        {
                            intMessage = 0;
                            LogService.WriteException(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return intMessage;
        }

        public ActionResult addImageFromUrl(int productID, string url)
        {
            int intMessage = 0;
            try
            {
                //save path to image in disk
                string pathImage = "";

                string folder = Server.MapPath("~/assets/uploads/products/") + productID;
                Directory.CreateDirectory(folder);

                if (!string.IsNullOrEmpty(url))
                {
                    //save name of file
                    string name = GetNameImageFromUrl(url);

                    if (!string.IsNullOrEmpty(name))
                    {
                        string strWhere = "";
                        strWhere = "ImageName like N'" + name + "' and ProductID = " + productID.ToString();
                        TblImage imageOfProduct = imageService.SelectByWhere(strWhere).FirstOrDefault();
                        if (imageOfProduct == null || imageOfProduct.ImageID <= 0)
                        {
                            pathImage = folder + "/" + name;
                            if (DownloadRemoteImageFile(url, pathImage))
                            {
                                //save path in web
                                string temp = "";
                                temp = HttpContext.Request.Url.Authority == string.Empty ? Common.UrlHost : "http://" + HttpContext.Request.Url.Authority;
                                if (!temp.EndsWith("/")) temp += "/";
                                temp += "assets/uploads/products/" + productID + "/" + name;
                                FileInfo fileInfo = new FileInfo(pathImage);

                                TblImage image = new TblImage();
                                image.ImageName = name;
                                image.ImageUrl = temp;
                                image.ImageSize = fileInfo.Length;
                                image.ProductID = productID;
                                imageService.Insert(image);

                                intMessage = 1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return RedirectToAction("detail", "products", new { id = productID, messageUpload = intMessage });
        }

        private string GetNameImageFromUrl(string imageUrl)
        {
            try
            {
                //string[] temp = imageUrl.Split('/');
                //if (temp != null && temp.Length > 0)
                //{
                //    if (!string.IsNullOrEmpty(temp[temp.Length - 1]))
                //    {
                //        return temp[temp.Length - 1];
                //    }
                //}
                Uri uri = new Uri(imageUrl);
                if (uri.IsAbsoluteUri)
                {
                    if (uri.Segments != null && uri.Segments.Length > 0)
                    {
                        return uri.Segments[uri.Segments.Length - 1];
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return "";
            }
        }

        private bool DownloadRemoteImageFile(string uri, string fileName)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if ((response.StatusCode == HttpStatusCode.OK ||
                    response.StatusCode == HttpStatusCode.Moved ||
                    response.StatusCode == HttpStatusCode.Redirect) &&
                    response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
                {
                    using (Stream inputStream = response.GetResponseStream())
                    using (Stream outputStream = System.IO.File.OpenWrite(fileName))
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        do
                        {
                            bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                            outputStream.Write(buffer, 0, bytesRead);
                        } while (bytesRead != 0);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        [HttpPost]
        public string uploadImageVariant(int id, HttpPostedFileBase imageVariant)
        {
            string strMessage = "upload0";
            try
            {
                if (imageVariant != null)
                {
                    if (imageVariant.ContentLength / 1024 / 1024 > 1)
                    {
                        strMessage = "upload2";
                    }
                    else if (imageVariant.ContentLength > 0)
                    {
                        VariantService variantService = new VariantService();
                        Variant variant = variantService.GetByPrimaryKey(id);
                        if (variant != null)
                        {
                            string folder = Server.MapPath("~/assets/uploads/products/") + variant.ProductID;
                            Directory.CreateDirectory(folder);

                            string path = Path.Combine(folder, Path.GetFileName(imageVariant.FileName));
                            imageVariant.SaveAs(path);

                            string strWhere = "";
                            strWhere = "ImageName like N'" + imageVariant.FileName + "' and ProductID = " + variant.ProductID;
                            TblImage imageOfProduct = imageService.SelectByWhere(strWhere).FirstOrDefault();
                            if (imageOfProduct == null || imageOfProduct.ImageID <= 0)
                            {
                                string imageUrl = "";
                                imageUrl = HttpContext.Request.Url.Authority == string.Empty ? Common.UrlHost : "http://" + HttpContext.Request.Url.Authority;
                                if (!imageUrl.EndsWith("/")) imageUrl += "/";
                                imageUrl += "assets/uploads/products/" + variant.ProductID + "/" + imageVariant.FileName;

                                TblImage image = new TblImage();
                                image.ImageName = imageVariant.FileName;
                                image.ImageUrl = imageUrl;
                                image.ImageSize = imageVariant.ContentLength;
                                image.ProductID = variant.ProductID;
                                int imageID = imageService.Insert(image);

                                variant.ImageID = imageID;
                                variantService.Update(variant);
                                strMessage = "upload1";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return strMessage;
        }
    }
}