using RealEstateWebUI.Areas.admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using PagedList;
using System.Web.UI.WebControls;
using RealEstateWebUI.Areas.admin.UtilzGeneral;
using System.IO;
using RealEstateWebUI.Areas.admin.Services;
using System.Web.Security;
using Microsoft.AspNet.Identity;

namespace RealEstateWebUI.Areas.admin.Controllers
{
    public class productsController : Controller
    {
        /// <summary>
        /// variable save table name for write log
        /// </summary>
        private int TableNameID = (int)Common.TableName.Product;
        private string strErrorMessage = "";

        private ProductStyleService productStyleService = new ProductStyleService();
        private OrderService tblOrderService = new OrderService();
        private SupplierService supplierService = new SupplierService();
        private CollectionProductService collectionProductService = new CollectionProductService();
        private ImageService imageService = new ImageService();
        private ProductService productService = new ProductService();
        private TagService tagService = new TagService();
        private VariantService variantService = new VariantService();
        private OptionService optionService = new OptionService();
        private AccountService accountService = new AccountService();
        private IDbConnection connect = new SqlConnection(Common.ConnectString);

        [Authorize(Roles = "Admin")]
        public ActionResult index(int page = 1, string strMessage = "")
        {
            string strError = "", strSuccess = "";
            if (!string.IsNullOrEmpty(strMessage))
            {
                if (strMessage.Equals("delete1"))
                {
                    strSuccess += "Xoá 1 sản phẩm thành công";
                }
                else if (strMessage.Equals("notExist"))
                {
                    strError += "Sản phẩm không tồn tại";
                }
            }
            ViewBag.strSuccess = strSuccess;
            ViewBag.strError = strError;

            List<ProductStyle> productStyle = productStyleService.GetAll();
            productStyle.Insert(0, new ProductStyle { ProductStyleID = 0, ProductStyleName = "Tất cả" });
            ViewBag.ddlProductStyle = new SelectList(productStyle, "ProductStyleID", "ProductStyleName");

            List<Supplier> supplier = supplierService.GetAll();
            supplier.Insert(0, new Supplier { SupplierID = 0, SupplierName = "Tất cả" });
            ViewBag.ddlSupplier = new SelectList(supplier, "SupplierID", "SupplierName");

            int pageSize = 10;
            List<Product> product = productService.GetAll();
            ProductModel productModel = new ProductModel();
            productModel.lstProduct = product.ToPagedList(page, pageSize);
            return View(productModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult index(ProductModel productModel, int page = 1)
        {
            string strWhere = "";
            List<ProductStyle> productStyle = productStyleService.GetAll();
            productStyle.Insert(0, new ProductStyle { ProductStyleID = 0, ProductStyleName = "Tất cả" });
            ViewBag.ddlProductStyle = new SelectList(productStyle, "ProductStyleID", "ProductStyleName");

            List<Supplier> supplier = supplierService.GetAll();
            supplier.Insert(0, new Supplier { SupplierID = 0, SupplierName = "Tất cả" });
            ViewBag.ddlSupplier = new SelectList(supplier, "SupplierID", "SupplierName");

            int pageSize = int.MaxValue;
            strWhere = "";
            string strCondition = "";
            if (!string.IsNullOrEmpty(productModel.txtConditionFind))
            {
                strCondition += string.Format(" and (ProductName like N'%{0}%' or ProductContent like N'%{1}%' or ProductTitleCard like N'%{2}%' or ProductDescriptionCard like N'%{3}%' or ProductAlias like N'%{4}%' or Tags like N'%{5}%')",
                    productModel.txtConditionFind, productModel.txtConditionFind, productModel.txtConditionFind, productModel.txtConditionFind, productModel.txtConditionFind, productModel.txtConditionFind);
            }

            if (productModel.ddlConditionFilter == "ddlDisplayStatus")
            {
                if (!string.IsNullOrEmpty(productModel.ddlDisplayStatus))
                {
                    strWhere = "ProductState = " + productModel.ddlDisplayStatus + strCondition;
                }
            }
            else if (productModel.ddlConditionFilter == "ddlProductStyle")
            {
                if (SNumber.ToNumber(productModel.ddlProductStyle) > 0)
                {
                    strWhere = "ProductStyleID = " + productModel.ddlProductStyle + strCondition;
                }
            }
            else if (productModel.ddlConditionFilter == "ddlSupplier")
            {
                if (SNumber.ToNumber(productModel.ddlSupplier) > 0)
                {
                    strWhere = "SupplierID = " + productModel.ddlSupplier + strCondition;
                }
            }
            if (string.IsNullOrEmpty(strWhere))
            {
                if (!string.IsNullOrEmpty(strCondition) && strCondition.Length > 4)
                {
                    strWhere = strCondition.Substring(4);
                }
                else
                {
                    strWhere = "";
                }
            }

            List<Product> lst = productService.GetByWhere(strWhere);
            productModel.lstProduct = lst.ToPagedList(page, pageSize);
            return View(productModel);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult detail(int id, string strMessage = "", string messageUpload = "", string messageDelete = "")
        {
            try
            {
                string strError = "", strSuccess = "";
                if (!string.IsNullOrEmpty(strMessage))
                {
                    if (strMessage.Equals("1"))
                    {
                        strSuccess = Common.UpdateSuccess + "</br>";
                    }
                    else if (strMessage.Equals("0"))
                    {
                        strError = Common.UpdateFail + "</br>";
                    }
                    else if (strMessage.Equals("Variant1"))
                    {
                        strSuccess = "Xoá phiên bản thành công" + "</br>";
                    }
                    else if (strMessage.Equals("Variant0"))
                    {
                        strError = "Xoá phiên bản thất bại" + "</br>";
                    }
                    else if (strMessage.Equals("delete0"))
                    {
                        strError = "Xoá sản phẩm thất bại" + "</br>";
                    }
                }
                if (messageUpload == "0")
                {
                    strError += "Upload ảnh không thành công</br>";
                }
                else if (messageUpload == "1")
                {
                    strSuccess += "Upload ảnh thành công</br>";
                }
                else if (messageUpload == "2")
                {
                    strError += "Kích thước file > 1MB</br>";
                }
                if (messageDelete == "0")
                {
                    strError += "Xóa ảnh không thành công</br>";
                }
                else if (messageDelete == "1")
                {
                    strSuccess += "Xóa ảnh thành công</br>";
                }
                ViewBag.strSuccess = strSuccess;
                ViewBag.strError = strError;

                Product product = productService.GetByPrimaryKey(id);

                if (product != null)
                {
                    List<ProductStyle> productStyle = productStyleService.GetAll();
                    productStyle.Insert(0, new ProductStyle { ProductStyleID = 0, ProductStyleName = "Chọn loại sản phẩm" });
                    product.ProductStyles = new SelectList(productStyle, "ProductStyleID", "ProductStyleName", product.ProductStyleID.ToString());

                    List<Supplier> supplier = supplierService.GetAll();
                    supplier.Insert(0, new Supplier { SupplierID = 0, SupplierName = "Chọn nhà sản xuất" });
                    product.Suppliers = new SelectList(supplier, "SupplierID", "SupplierName", product.SupplierID.ToString());

                    List<TblImage> images = imageService.GetByProductID(id);
                    product.Images = images;

                    product.CollectionProducts = collectionProductService.GetByProductID(id);
                    product.Options = optionService.GetByProductID(id);
                    product.ListTag = tagService.GetByTableNameID((int)Common.TableName.Product);
                    product.Variants = variantService.GetByProductID(id);
                    product.Variant = null;
                    if (product.Variants != null && product.Variants.Count > 0)
                    {
                        foreach (var item in product.Variants)
                        {
                            item.Image = imageService.GetByPrimaryKey(item.ImageID);
                        }
                        product.Variant = product.Variants[0];
                    }

                    return View(product);
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return RedirectToAction("", "products", new { strMessage = "notExist"});
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult detail(Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (CheckInput(product))
                    {
                        if (product.SupplierID == 0)
                        {
                            product.SupplierID = null;
                        }
                        if (product.ProductStyleID == 0)
                        {
                            product.ProductStyleID = null;
                        }
                        product.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                        if (!string.IsNullOrEmpty(product.Tags))
                        {
                            string[] tags = product.Tags.Split(',');
                            foreach (var item in tags)
                            {
                                if (!tagService.CheckExistTag(item, (int)Common.TableName.Product))
                                {
                                    Tag tag = new Tag();
                                    tag.TagName = item;
                                    tag.TableNameID = (int)Common.TableName.Product;

                                    int tagID = tagService.Insert(tag);
                                }
                            }
                        }
                        bool flg = productService.Update(product);
                        if (flg)
                        {
                            LogService.WriteLog2DB(accountService.GetUserId(User.Identity.GetUserName()), (int)Common.ActionID.Update, product.ProductID, SDateTime.GetYYYYMMddHmmSSNow(), General.GetIPAddress(), TableNameID, product.ProductName);
                        }

                        // update variant defaul || update list variant
                        if (!product.AutoGenerate)
                        {
                            // variant default
                            if (product.Variant.VariantID > 0)
                            {
                                Variant variant = variantService.GetByPrimaryKey(product.Variant.VariantID);
                                variant.VariantPrice = product.Variant.VariantPrice;
                                variant.CompareWithPrice = product.Variant.CompareWithPrice;
                                variant.Textable = product.Variant.Textable;
                                variant.VariantSKU = product.Variant.VariantSKU;
                                variant.VariantBarcode = product.Variant.VariantBarcode;
                                variant.VariantWeight = product.Variant.VariantWeight;
                                variant.WeightUnit = product.Variant.WeightUnit;
                                variant.RequireShipping = product.Variant.RequireShipping;
                                variant.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                                bool updateVariant = variantService.Update(variant);
                            }
                            // has list variant
                            else
                            {
                                if (product.Variants != null && product.Variants.Count > 0)
                                {
                                    for (int i = 0; i < product.Variants.Count; i++)
                                    {
                                        product.Variants[i].VariantTittle = variantService.GetVariantTittle(product.Variants[i]);
                                        product.Variants[i].ProductID = product.ProductID;
                                        product.Variants[i].ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                                        bool updateVariantFlg = variantService.Update(product.Variants[i]);
                                    }
                                }
                            }
                        }
                        // add variant, add option
                        else
                        {
                            if (product.Variants != null && product.Variants.Count > 0)
                            {
                                variantService.DeleteByProductID(product.ProductID);
                                for (int i = 0; i < product.Variants.Count; i++)
                                {
                                    Variant variant = product.Variants[i];
                                    if (variant.IsCreate)
                                    {
                                        if (SNumber.ToNumber(variant.VariantPrice) <= 0)
                                        {
                                            variant.VariantPrice = SNumber.ToNumber(product.Variant.VariantPrice) >= 0 ? product.Variant.VariantPrice : 0;
                                        }
                                        if (string.IsNullOrEmpty(variant.VariantSKU))
                                        {
                                            variant.VariantSKU = product.Variant.VariantSKU + "_" + (i + 1);
                                        }
                                        if (string.IsNullOrEmpty(variant.VariantBarcode))
                                        {
                                            variant.VariantSKU = product.Variant.VariantBarcode;
                                        }
                                        variant.CompareWithPrice = product.Variant.CompareWithPrice;
                                        variant.Textable = product.Variant.Textable;
                                        variant.VariantWeight = product.Variant.VariantWeight;
                                        variant.WeightUnit = product.Variant.WeightUnit;
                                        variant.RequireShipping = product.Variant.RequireShipping;
                                        variant.VariantBarcode = product.Variant.VariantBarcode;
                                        variant.ProductID = product.ProductID;
                                        variant.VariantTittle = variantService.GetVariantTittle(variant);
                                        variant.CreatedDateTime = variant.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                                        int variantID = variantService.Insert(variant);
                                    }
                                }
                            }

                            //option
                            if (product.Options != null && product.Options.Count > 0)
                            {
                                for (int i = 0; i < product.Options.Count; i++)
                                {
                                    if (i == 0)
                                    {
                                        TblOption optionDefault = optionService.GetOptionDefaultOfProduct(product.ProductID);

                                        if (optionDefault != null)
                                        {
                                            optionDefault.OptionName = product.Options[i].OptionName;
                                            optionDefault.OptionValue = product.Options[i].OptionValue;
                                            optionDefault.ProductID = product.ProductID;
                                            optionDefault.Position = 1;
                                            optionDefault.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                                            bool updateOptionDefault = optionService.Update(optionDefault);

                                        }
                                    }
                                    else
                                    {
                                        TblOption option = product.Options[i];
                                        option.ProductID = product.ProductID;
                                        option.Position = i + 1;
                                        option.CreatedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                                        option.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                                        int optionID = optionService.Insert(option);
                                    }
                                }
                            }
                        }
                        optionService.UpdateOptionOfProduct(product.ProductID);
                        return RedirectToAction("detail", "products", new { id = product.ProductID, strMessage = "1" });
                    }
                }
                catch (Exception ex)
                {
                    LogService.WriteException(ex);
                    return RedirectToAction("", "products");
                }
            }
            product.ListTag = tagService.GetByTableNameID((int)Common.TableName.Product);
            product.Options = optionService.GetByProductID(product.ProductID);
            product.Variants = variantService.GetByProductID(product.ProductID);
            product.Variant = null;
            if (product.Variants != null && product.Variants.Count > 0)
            {
                if (!string.IsNullOrEmpty(product.Variants[0].Option1) && product.Variants[0].Option1.Equals("Default Title"))
                {
                    product.Variant = product.Variants[0];
                    product.Variants = null;
                }
            }
            product.AutoGenerate = false;
            List<TblImage> images = imageService.GetByProductID(product.ProductID);
            product.Images = images;

            List<ProductStyle> productStyle = productStyleService.GetAll();
            productStyle.Insert(0, new ProductStyle { ProductStyleID = 0, ProductStyleName = "Chọn loại sản phẩm" });
            product.ProductStyles = new SelectList(productStyle, "ProductStyleID", "ProductStyleName", product.ProductStyleID.ToString());

            List<Supplier> supplier = supplierService.GetAll();
            supplier.Insert(0, new Supplier { SupplierID = 0, SupplierName = "Chọn nhà sản xuất" });
            product.Suppliers = new SelectList(supplier, "SupplierID", "SupplierName", product.SupplierID);
            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    strErrorMessage += error.ErrorMessage;
                }
            }
            ViewBag.strError = strErrorMessage;
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult delete(int id)
        {
            try
            {
                string folder = Server.MapPath("~/assets/uploads/products/") + id;

                string objectValue = "";
                Product product = productService.GetByPrimaryKey(id);
                if (product != null)
                {
                    objectValue = product.ProductName;
                    // xóa tất cả ảnh trong folder
                    ImageService.DeleteFolder(folder);

                    imageService.DeleteByProductID(id);

                    variantService.DeleteByProductID(id);

                    optionService.DeleteByProductID(id);

                    collectionProductService.DeleteByProductID(id);

                    if (productService.DeleteByPrimary(id))
                    {
                        LogService.WriteLog2DB(accountService.GetUserId(User.Identity.GetUserName()), (int)Common.ActionID.Delete, id, SDateTime.GetYYYYMMddHmmSSNow(), General.GetIPAddress(), TableNameID, product.ProductName);
                        return RedirectToAction("", "products", new { strMessage = "delete1" });
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return RedirectToAction("detail", "products", new { id = id, strMessage = "delete0" });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult create()
        {
            Product product = new Product();

            List<ProductStyle> productStyle = productStyleService.GetAll();
            productStyle.Insert(0, new ProductStyle { ProductStyleID = 0, ProductStyleName = "Chọn loại sản phẩm" });
            product.ProductStyles = new SelectList(productStyle, "ProductStyleID", "ProductStyleName", product.ProductStyleID.ToString());

            List<Supplier> supplier = supplierService.GetAll();
            supplier.Insert(0, new Supplier { SupplierID = 0, SupplierName = "Chọn loại nhà sản xuất" });
            product.Suppliers = new SelectList(supplier, "SupplierID", "SupplierName", product.SupplierID.ToString());

            product.ListTag = tagService.GetByTableNameID((int)Common.TableName.Product);
            product.Variant.WeightUnit = "kg";
            product.AutoGenerate = false;

            return View(product);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public string create(Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!CheckInput(product))
                    {
                        return strErrorMessage;
                    }
                    if (product.SupplierID == 0)
                    {
                        product.SupplierID = null;
                    }
                    if (product.ProductStyleID == 0)
                    {
                        product.ProductStyleID = null;
                    }
                    product.CreatedDateTime = product.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();

                    if (!string.IsNullOrEmpty(product.Tags))
                    {
                        string[] tags = product.Tags.Split(',');
                        if (tags != null && tags.Length > 0)
                        {
                            foreach (var item in tags)
                            {
                                if (!tagService.CheckExistTag(item, (int)Common.TableName.Product))
                                {
                                    Tag tag = new Tag();
                                    tag.TagName = item;
                                    tag.TableNameID = (int)Common.TableName.Product;
                                    int tagID = tagService.Insert(tag);
                                }
                            }
                        }
                    }

                    int productID = productService.Insert(product);
                    if (productID > 0)
                    {
                        LogService.WriteLog2DB(accountService.GetUserId(User.Identity.GetUserName()), (int)Common.ActionID.Insert, productID, SDateTime.GetYYYYMMddHmmSSNow(), General.GetIPAddress(), TableNameID, product.ProductName);

                        //image
                        for (int i = 0; i < product.UploadeImages.Count; i++)
                        {
                            UploadImage(productID, product.UploadeImages[i]);
                        }
                        bool createVariantDefault = true;

                        if (product.AutoGenerate)
                        {
                            //variant
                            if (product.Variants != null && product.Variants.Count > 0)
                            {
                                for (int i = 0; i < product.Variants.Count; i++)
                                {
                                    Variant variant = product.Variants[i];
                                    if (variant.IsCreate)
                                    {
                                        createVariantDefault = false;
                                        if (SNumber.ToNumber(variant.VariantPrice) <= 0)
                                        {
                                            variant.VariantPrice = SNumber.ToNumber(product.Variant.VariantPrice) >= 0 ? product.Variant.VariantPrice : 0;
                                        }
                                        if (string.IsNullOrEmpty(variant.VariantSKU))
                                        {
                                            variant.VariantSKU = product.Variant.VariantSKU + "_" + (i + 1);
                                        }
                                        if (string.IsNullOrEmpty(variant.VariantBarcode))
                                        {
                                            variant.VariantSKU = product.Variant.VariantBarcode;
                                        }
                                        variant.CompareWithPrice = product.Variant.CompareWithPrice;
                                        variant.Textable = product.Variant.Textable;
                                        variant.VariantWeight = product.Variant.VariantWeight;
                                        variant.WeightUnit = product.Variant.WeightUnit;
                                        variant.RequireShipping = product.Variant.RequireShipping;
                                        variant.ProductID = productID;
                                        variant.CreatedDateTime = variant.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                                        variant.VariantTittle = variantService.GetVariantTittle(variant);
                                        variantService.Insert(variant);
                                    }
                                }
                            }
                        }
                        if (createVariantDefault)
                        {
                            product.Variant.ProductID = productID;
                            product.Variant.CreatedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                            product.Variant.Option1 = product.Variant.VariantTittle = "Default Title";
                            product.Variant.Option2 = null;
                            product.Variant.Option3 = null;
                            product.Variant.CreatedDateTime = product.Variant.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                            variantService.Insert(product.Variant);

                            TblOption option = new TblOption();
                            option.OptionName = "Title";
                            option.ProductID = productID;
                            option.OptionValue = "Default Title";
                            option.Position = 1;
                            option.CreatedDateTime = option.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                            optionService.Insert(option);
                        }
                        else
                        {
                            //option
                            if (product.Options != null && product.Options.Count > 0)
                            {
                                for (int i = 0; i < product.Options.Count; i++)
                                {
                                    TblOption option = product.Options[i];
                                    option.ProductID = productID;
                                    option.Position = (i + 1);
                                    option.CreatedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                                    option.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                                    optionService.Insert(option);
                                }
                            }
                        }
                        return productID.ToString();
                    }
                }

                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        strErrorMessage += error.ErrorMessage;
                    }
                }
                return strErrorMessage;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return "Thêm sản phẩm lỗi";
            }
            //return View(product);
        }

        public bool CheckInput(Product product)
        {
            bool result = true;
            try
            {
                if (string.IsNullOrEmpty(product.ProductName))
                {
                    result = false;
                    strErrorMessage += "Tên sản phẩm không được để trống<br/>";
                }
                if (string.IsNullOrEmpty(product.ProductAlias))
                {
                    result = false;
                    strErrorMessage += "Đường dẫn không được để trống<br/>";
                }
                if (product.AutoGenerate)
                {
                    if (product.Options != null && product.Options.Count > 0)
                    {
                        foreach (var item in product.Options)
                        {
                            if (string.IsNullOrEmpty(item.OptionName))
                            {
                                result = false;
                                strErrorMessage += "Tên tuỳ chọn không được để trống<br/>";
                                break;
                            }
                        }
                        foreach (var item in product.Options)
                        {
                            if (string.IsNullOrEmpty(item.OptionValue))
                            {
                                result = false;
                                strErrorMessage += "Giá trị không được để trống<br/>";
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                LogService.WriteException(ex);
            }
            return result;
        }

        private int UploadImage(int productID, HttpPostedFileBase file)
        {
            if (file.ContentLength / 1024 / 1024 > 1)
            {
                return 2;
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
                        string imageUrl = "";
                        imageUrl = HttpContext.Request.Url.Authority == string.Empty ? Common.UrlHost : "http://" + HttpContext.Request.Url.Authority;
                        if (!imageUrl.EndsWith("/")) imageUrl += "/";
                        imageUrl += "assets/uploads/products/" + productID + "/" + file.FileName;

                        TblImage image = new TblImage();
                        image.ImageName = file.FileName;
                        image.ImageUrl = imageUrl;
                        image.ImageSize = file.ContentLength;
                        image.ProductID = SNumber.ToNumber(productID);
                        imageService.Insert(image);
                    }

                    string path = Path.Combine(folder, Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    return 1;
                }
                catch (Exception ex)
                {
                    LogService.WriteException(ex);
                }
            }
            return 0;
        }

        public ActionResult dropdown(int collectionID, string query)
        {
            try
            {
                ProductDropdownModel productDropdownModel = new ProductDropdownModel();
                List<ProductDropDown> productDropdowns = new List<ProductDropDown>();
                List<Product> products = new List<Product>();
                if (string.IsNullOrEmpty(query))
                {
                    products = productService.GetAll();
                }
                else
                {
                    string where = string.Format("ProductName like N'%{0}%'", query);
                    products = productService.GetByWhere(where);
                }
                if (products != null && products.Count > 0)
                {
                    foreach (var item in products)
                    {
                        ProductDropDown temp = new ProductDropDown();
                        temp.ProductID = item.ProductID;
                        temp.ProductName = item.ProductName;
                        if (collectionProductService.CheckExistCollectionProduct(collectionID, item.ProductID))
                        {
                            temp.Choice = true;
                        }
                        else
                        {
                            temp.Choice = false;
                        }
                        productDropdowns.Add(temp);
                    }
                    productDropdownModel.ListProductDropDown = productDropdowns;
                    productDropdownModel.CollectionID = collectionID;
                }
                //productDropdownModel.ConditionFindProduct = query;
                return View(productDropdownModel);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult dropdown(ProductDropdownModel productDropdownModel)
        {
            try
            {
                int numberProduct = 0;
                if (productDropdownModel.ListProductDropDown != null && productDropdownModel.ListProductDropDown.Count > 0)
                {
                    foreach (var item in productDropdownModel.ListProductDropDown)
                    {
                        if (item.Choice)
                        {
                            if (!collectionProductService.CheckExistCollectionProduct(productDropdownModel.CollectionID, item.ProductID))
                            {
                                collectionProductService.Insert(new CollectionProduct
                                {
                                    CollectionID = productDropdownModel.CollectionID,
                                    ProductID = item.ProductID
                                });
                                numberProduct++;
                            }
                        }
                    }
                }
                return RedirectToAction("detail", "collections", new { id = productDropdownModel.CollectionID, strSuccess = "update1" });
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult getLineItem(string variantChoiced, string query)
        {
            try
            {
                LineItemsOfOrder productVariantOfProduct = new LineItemsOfOrder();
                List<Product> products = new List<Product>();
                if (string.IsNullOrEmpty(query))
                {
                    products = productService.GetAll();
                }
                else
                {
                    string where = string.Format("ProductName like N'%{0}%' or Tags like N'%{1}%' ", query, query);
                    where += string.Format(" or ProductID in (select Product.ProductID from Product left join TblOption on Product.ProductID = TblOption.ProductID " +
                                                " where OptionValue like N'%{0}%')", query);
                    products = productService.GetByWhere(where);
                }

                List<int> variantIDs = new List<int>();
                if (!string.IsNullOrEmpty(variantChoiced))
                {
                    variantChoiced = SString.RemoveElementAtBeginEnd(variantChoiced, ",");
                    string[] temp = variantChoiced.Split(',');
                    foreach (var item in temp)
                    {
                        variantIDs.Add(SNumber.ToNumber(item));
                    }
                }

                if (products != null && products.Count > 0)
                {
                    foreach (var item in products)
                    {
                        List<Variant> variants = variantService.GetByProductID(item.ProductID);
                        if (variants != null && variants.Count > 0)
                        {
                            if (variants.Count == 1 && variants[0].Option1 == "Default Title")
                            {
                                LineItem temp = new LineItem();
                                temp.VariantID = variants[0].VariantID;
                                temp.SKU = variants[0].VariantSKU;
                                temp.ProductID = item.ProductID;
                                temp.ObjectName = item.ProductName;
                                temp.IsDefault = true;
                                temp.Price = SNumber.ToNumber(variants[0].VariantPrice);
                                temp.Quantity = 1;

                                var thumb = ImageService.GetPathImageFirstOfProduct(item.ProductID);
                                temp.ImageUrl = thumb;
                                if (!variantIDs.Contains(temp.VariantID))
                                {
                                    temp.CanChoice = true;
                                }
                                else
                                {
                                    temp.CanChoice = false;
                                }
                                productVariantOfProduct.ProductVariants.Add(temp);
                            }
                            else
                            {
                                LineItem product = new LineItem();
                                product.VariantID = 0;
                                product.ProductID = item.ProductID;
                                product.ObjectName = item.ProductName;
                                product.IsDefault = true;
                                product.Price = -1;
                                product.CanChoice = false;
                                product.Quantity = 1;
                                var thumb = ImageService.GetPathImageFirstOfProduct(item.ProductID);
                                product.ImageUrl = thumb;
                                productVariantOfProduct.ProductVariants.Add(product);

                                for (int i = 0; i < variants.Count; i++)
                                {
                                    LineItem temp = new LineItem();
                                    temp.ProductID = item.ProductID;
                                    temp.VariantID = variants[i].VariantID;
                                    temp.ObjectName = variants[i].VariantTittle;
                                    temp.SKU = variants[i].VariantSKU;
                                    temp.IsDefault = false;
                                    temp.Price = SNumber.ToNumber(variants[i].VariantPrice);
                                    temp.Quantity = 1;
                                    if (!variantIDs.Contains(temp.VariantID))
                                    {
                                        temp.CanChoice = true;
                                    }
                                    else
                                    {
                                        temp.CanChoice = false;
                                    }

                                    TblImage image = imageService.GetByPrimaryKey(variants[i].ImageID);
                                    if (image != null)
                                    {
                                        temp.ImageUrl = image.ImageUrl;
                                    }
                                    productVariantOfProduct.ProductVariants.Add(temp);
                                }
                            }
                        }
                    }
                }
                return View(productVariantOfProduct);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult getLineItem(LineItemsOfOrder productVariantOfOrder)
        {
            try
            {
                List<LineItem> lineItems = new List<LineItem>();
                if (productVariantOfOrder != null)
                {
                    if (productVariantOfOrder.ProductVariants != null && productVariantOfOrder.ProductVariants.Count > 0)
                    {
                        foreach (var item in productVariantOfOrder.ProductVariants)
                        {
                            if (item.Choice)
                            {
                                LineItem lineItem = new LineItem();
                                lineItem.ProductID = item.ProductID;
                                lineItem.VariantID = item.VariantID;
                                Variant variant = variantService.GetByPrimaryKey(item.VariantID);

                                if (variant != null)
                                {
                                    if (variant.VariantTittle != "Default Title")
                                    {
                                        lineItem.VariantName = variant.VariantTittle;
                                    }
                                    Product product = productService.GetByPrimaryKey(variant.ProductID);
                                    lineItem.ProductName = product.ProductName;
                                }
                                lineItem.ImageUrl = item.ImageUrl;
                                lineItem.IsDefault = item.IsDefault;
                                lineItem.Quantity = item.Quantity;
                                lineItem.Price = item.Price;
                                lineItem.SKU = item.SKU;
                                lineItems.Add(lineItem);
                            }
                        }
                    }
                }
                return Json(lineItems, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        public ActionResult deleteProductOutCollection(int id, int collectionProductID)
        {
            try
            {
                collectionProductService.DeleteByPrimary(collectionProductID);
                return RedirectToAction("detail", "products", new { id = id, strMessage = "1" });
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return RedirectToAction("detail", "products", new { id = id, strMessage = "0" });
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult updateOptionsOfProduct(int id)
        {
            try
            {
                OptionsOfProduct optionsOfProduct = new OptionsOfProduct();
                Product product = productService.GetByPrimaryKey(id);
                if (product != null)
                {
                    List<TblOption> options = optionService.GetByProductID(id);
                    if (options != null && options.Count > 0)
                    {
                        optionsOfProduct.Options = options;
                        for (int i = 0; i < options.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(options[i].OptionValue))
                            {
                                if (i == 0)
                                {
                                    optionsOfProduct.Option1Values = options[i].OptionValue.Split(',');
                                }
                                else if (i == 1)
                                {
                                    optionsOfProduct.Option2Values = options[i].OptionValue.Split(',');
                                }
                                else if (i == 2)
                                {
                                    optionsOfProduct.Option3Values = options[i].OptionValue.Split(',');
                                }
                            }
                        }
                    }
                }
                optionsOfProduct.ProductID = id;
                return View(optionsOfProduct);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult updateOptionsOfProduct(OptionsOfProduct optionsOfProduct)
        {
            try
            {
                if (optionsOfProduct.Options != null && optionsOfProduct.Options.Count > 0)
                {
                    for (int i = 0; i < optionsOfProduct.Options.Count; i++)
                    {
                        TblOption option = optionsOfProduct.Options[i];
                        option.Position = i + 1;
                        option.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                        optionService.Update(option);
                    }
                }
                return RedirectToAction("detail", "products", new { id = optionsOfProduct.ProductID, strMessage = "1" });
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return RedirectToAction("detail", "products", new { id = optionsOfProduct.ProductID, strMessage = "0" });
        }
    }
}