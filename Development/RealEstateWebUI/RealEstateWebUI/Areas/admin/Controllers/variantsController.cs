using RealEstateWebUI.Areas.admin.Models;
using RealEstateWebUI.Areas.admin.Services;
using RealEstateWebUI.Areas.admin.UtilzGeneral;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RealEstateWebUI.Areas.admin.Controllers
{
    public class variantsController : Controller
    {
        private string strErrorMessage = "";
        private OptionService optionService = new OptionService();
        private VariantService variantService = new VariantService();
        private ProductService productService = new ProductService();
        private ImageService imageService = new ImageService();

        [Authorize(Roles = "Admin")]
        public ActionResult create(int productID)
        {
            try
            {
                Variant variant = new Variant();
                variant.ProductID = productID;
                variant.Product = productService.GetByPrimaryKey(productID);
                variant.Product.Variants = variantService.GetByProductID(productID);
                if (variant.Product.Variants != null && variant.Product.Variants.Count > 0)
                {
                    foreach (var item in variant.Product.Variants)
                    {
                        item.Image = imageService.GetByPrimaryKey(item.ImageID);
                    }
                }
                variant.Product.Options = optionService.GetByProductID(productID);
                variant.Product.Images = imageService.GetByProductID(productID);
                variant.WeightUnit = "kg";
                return View(variant);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return RedirectToAction("detail", "products", new { id = productID });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult create(Variant variant)
        {
            try
            {
                if (CheckInput(variant))
                {
                    if (ModelState.IsValid)
                    {
                        int imageID = UploadImage(variant);
                        if (imageID == 0)
                        {
                            variant.ImageID = null;
                        }
                        else
                        {
                            variant.ImageID = imageID;
                        }
                        variant.VariantTittle = variantService.GetVariantTittle(variant);
                        variant.CreatedDateTime = variant.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                        int variantID = variantService.Insert(variant);
                        if (variantID > 0)
                        {
                            optionService.UpdateOptionOfProduct(variant.ProductID);
                            return RedirectToAction("detail", "variants", new { id = variantID, strMessage = "create1" });
                        }
                    }
                }
                ViewBag.strError += strErrorMessage;
                variant.Product = productService.GetByPrimaryKey(variant.ProductID);
                variant.Product.Variants = variantService.GetByProductID(variant.ProductID);
                if (variant.Product.Variants != null && variant.Product.Variants.Count > 0)
                {
                    foreach (var item in variant.Product.Variants)
                    {
                        item.Image = imageService.GetByPrimaryKey(item.ImageID);
                    }
                }
                variant.Product.Options = optionService.GetByProductID(variant.ProductID);
                variant.Product.Images = imageService.GetByProductID(variant.ProductID);
                variant.WeightUnit = "kg";
                return View(variant);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return RedirectToAction("detail", "products", new { id = variant.ProductID });
        }
        public bool CheckInput(Variant variant)
        {
            bool flg = true;
            try
            {
                if (variant != null)
                {
                    if (variant.imageVariant != null && variant.imageVariant.ContentLength / 1024 / 1024 > 1)
                    {
                        strErrorMessage = "Kích thước file > 1MB";
                        flg = false;
                    }

                    bool changeOptionFlg = false;
                    Variant variantOld = variantService.GetByPrimaryKey(variant.VariantID);
                    if (variant.Option1 != variantOld.Option1 || variant.Option2 != variantOld.Option2 || variant.Option3 != variantOld.Option3)
                    {
                        changeOptionFlg = true;
                    }
                    if (changeOptionFlg)
                    {
                        if (variantService.CheckVariantExist(variant))
                        {
                            strErrorMessage = "Phiên bản này đã tồn tại";
                            flg = false;
                        }
                    }
                }
                else
                {
                    flg = false;
                }
            }
            catch (Exception ex)
            {
                flg = false;
                LogService.WriteException(ex);
            }
            return flg;
        }
        private int UploadImage(Variant variant)
        {
            if (variant == null || variant.imageVariant == null)
            {
                return 0;
            }
            int variantID = variant.VariantID, productID = variant.ProductID;
            HttpPostedFileBase file = variant.imageVariant;
            if (file.ContentLength / 1024 / 1024 > 1)
            {
                return 0;
            }
            else if (file.ContentLength > 0)
            {
                try
                {
                    string folder = Server.MapPath("~/assets/uploads/products/") + productID;
                    Directory.CreateDirectory(folder);

                    string strWhere = "";
                    strWhere = "ImageName like N'" + file.FileName + "' and ProductID = " + productID.ToString();
                    TblImage imageOfProduct = imageService.SelectByWhere(strWhere).FirstOrDefault();
                    if (imageOfProduct == null || imageOfProduct.ImageID <= 0)
                    {
                        string path = Path.Combine(folder, Path.GetFileName(file.FileName));
                        file.SaveAs(path);

                        string imageUrl = "";
                        imageUrl = HttpContext.Request.Url.Authority == string.Empty ? Common.UrlHost : "http://" + HttpContext.Request.Url.Authority;
                        if (!imageUrl.EndsWith("/")) imageUrl += "/";
                        imageUrl += "assets/uploads/products/" + productID + "/" + file.FileName;

                        TblImage image = new TblImage();
                        image.ImageName = file.FileName;
                        image.ImageUrl = imageUrl;
                        image.ImageSize = file.ContentLength;
                        image.ProductID = SNumber.ToNumber(productID);
                        int imageID = imageService.Insert(image);

                        return imageID;
                    }
                }
                catch (Exception ex)
                {
                    LogService.WriteException(ex);
                    strErrorMessage += "Upload file thất bại<br/>";
                }
            }
            return 0;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult delete(int id)
        {
            Variant variant = variantService.GetByPrimaryKey(id);
            try
            {
                int numVariantOfProduct = variantService.CountByProductID(variant.ProductID);
                if (numVariantOfProduct == 1)
                {
                    variant.Option1 = variant.VariantTittle = "Default Title";
                    variant.Option2 = null;
                    variant.Option3 = null;
                    variant.ImageID = null;
                    variant.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                    variantService.Update(variant);

                    List<TblOption> optionOfProduct = optionService.GetByProductID(variant.ProductID);
                    if (optionOfProduct != null && optionOfProduct.Count > 0)
                    {
                        for (int i = 0; i < optionOfProduct.Count; i++)
                        {
                            if (i == 0)
                            {
                                TblOption option = optionOfProduct[i];
                                option.OptionName = "Title";
                                option.CreatedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                                option.OptionValue = "Default Title";
                                option.Position = 1;
                                optionService.Update(option);
                            }
                            else
                            {
                                optionService.DeleteByPrimary(optionOfProduct[i].OptionID);
                            }
                        }
                    }

                }
                else
                {
                    variantService.DeleteByPrimary(id);
                }
                optionService.UpdateOptionOfProduct(variant.ProductID);
                return RedirectToAction("detail", "products", new { id = variant.ProductID, strMessage = "Variant1" });
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return RedirectToAction("detail", "products", new { id = variant.ProductID, strMessage = "Variant0" });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult detail(int id, int productID, string strMessage = "")
        {
            try
            {
                Variant variant = variantService.GetByPrimaryKey(id);
                if (variant != null)
                {
                    variant.Image = imageService.GetByPrimaryKey(variant.ImageID);
                    variant.Product = productService.GetByPrimaryKey(productID);
                    variant.Product.Variants = variantService.GetByProductID(productID);
                    if (variant.Product.Variants != null && variant.Product.Variants.Count > 0)
                    {
                        foreach (var item in variant.Product.Variants)
                        {
                            item.Image = imageService.GetByPrimaryKey(item.ImageID);
                        }
                    }
                    variant.Product.Options = optionService.GetByProductID(productID);
                    variant.Product.Images = imageService.GetByProductID(productID);

                    if (!string.IsNullOrEmpty(strMessage))
                    {
                        if (strMessage.Equals("upload0"))
                        {
                            ViewBag.strError = "Upload ảnh không thành công";
                        }
                        else if (strMessage.Equals("upload1"))
                        {
                            ViewBag.strSuccess = "Upload ảnh thành công";
                        }
                        else if (strMessage.Equals("upload2"))
                        {
                            ViewBag.strSuccess = "Kích thước file > 1MB";
                        }
                        else if (strMessage.Equals("update0"))
                        {
                            ViewBag.strError = "Cập nhật phiên bản không thành công";
                        }
                        else if (strMessage.Equals("update1"))
                        {
                            ViewBag.strSuccess = "Cập nhật phiên bản thành công";
                        }
                        else if (strMessage.Equals("create1"))
                        {
                            ViewBag.strSuccess = "Thêm mới thành công";
                        }
                    }
                    return View(variant);
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return RedirectToAction("detail", "products", new { id = productID });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult detail(Variant variant)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (CheckInput(variant))
                    {
                        List<TblOption> optionOfProduct = optionService.GetByProductID(variant.ProductID);
                        if (optionOfProduct != null && optionOfProduct.Count > 0)
                        {
                            if (optionOfProduct.Count == 1)
                            {
                                variant.Option2 = variant.Option3 = null;
                            }
                            else if (optionOfProduct.Count == 2)
                            {
                                variant.Option3 = null;
                            }
                        }

                        if (variant.ImageID == 0)
                        {
                            variant.ImageID = null;
                        }
                        variant.VariantTittle = variantService.GetVariantTittle(variant);
                        variant.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                        if (variantService.Update(variant))
                        {
                            optionService.UpdateOptionOfProduct(variant.ProductID);
                            return RedirectToAction("detail", "variants", new { id = variant.VariantID, strMessage = "update1" });
                        }
                        else
                        {
                            return RedirectToAction("detail", "variants", new { id = variant.VariantID, strMessage = "update0" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            ViewBag.strError = strErrorMessage;
            variant.VariantTittle = variantService.GetVariantTittle(variant);
            variant.Image = imageService.GetByPrimaryKey(variant.ImageID);
            variant.Product = productService.GetByPrimaryKey(variant.ProductID);
            variant.Product.Variants = variantService.GetByProductID(variant.ProductID);
            if (variant.Product.Variants != null && variant.Product.Variants.Count > 0)
            {
                foreach (var item in variant.Product.Variants)
                {
                    item.Image = imageService.GetByPrimaryKey(item.ImageID);
                }
            }
            variant.Product.Options = optionService.GetByProductID(variant.ProductID);
            variant.Product.Images = imageService.GetByProductID(variant.ProductID);
            return View(variant);
        }
    }
}