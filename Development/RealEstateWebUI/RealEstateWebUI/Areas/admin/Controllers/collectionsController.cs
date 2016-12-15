using RealEstateWebUI.Areas.admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using RealEstateWebUI.Areas.admin.Services;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using RealEstateWebUI.Areas.admin.UtilzGeneral;
using System.IO;
using Microsoft.AspNet.Identity;

namespace RealEstateWebUI.Areas.admin.Controllers
{
    public class collectionsController : Controller
    {
        /// <summary>
        /// variable save table name for write log
        /// </summary>
        private int TableNameID = (int)Common.TableName.Collection;
        private AccountService accountService = new AccountService();
        private RuleService tblRuleService = new RuleService();
        private ProductService productService = new ProductService();
        private CollectionProductService collectionProductService = new CollectionProductService();
        private CollectionService collectionService = new CollectionService();
        private IDbConnection connect = new SqlConnection(Common.ConnectString);

        [Authorize(Roles = "Admin")]
        public ActionResult index(int page = 1, string strMessage = "")
        {
            string strError = "", strSuccess = "";
            if (!string.IsNullOrEmpty(strMessage))
            {
                if (strMessage.Equals("delete1"))
                {
                    strSuccess += "Xoá 1 danh mục thành công";
                }
                else if (strMessage.Equals("notExist"))
                {
                    strError += "Danh mục không tồn tại";
                }
            }
            ViewBag.strSuccess = strSuccess;
            ViewBag.strError = strError;
            int pageSize = 10;

            List<Collection> collection = collectionService.GetAll();
            CollectionModel collectionModel = new CollectionModel();
            collectionModel.lstCollection = collection.ToPagedList(page, pageSize);
            return View(collectionModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult index(CollectionModel collectionModel, int page = 1)
        {
            try
            {
                int pageSize = int.MaxValue;

                string strWhere = "1=1";

                if (collectionModel.ddlConditionFilter == "ddlDisplayStatus")
                {
                    if (!string.IsNullOrEmpty(collectionModel.ddlDisplayStatus))
                    {
                        strWhere = " and CollectionState = " + collectionModel.ddlDisplayStatus;
                    }
                }
                else if (collectionModel.ddlConditionFilter == "ddlCollectionStyle")
                {
                    if (!string.IsNullOrEmpty(collectionModel.ddlCollectionStyle))
                    {
                        strWhere = " and CollectionType like N'" + collectionModel.ddlCollectionStyle + "'";
                    }
                }
                if (!string.IsNullOrEmpty(collectionModel.txtConditionFind))
                {
                    strWhere += string.Format(" and (CollectionName like N'%{0}%' or CollectionDescription like N'%{1}%' or PageTitle like N'%{2}%' or PageDescription like N'%{3}%') ", collectionModel.txtConditionFind, collectionModel.txtConditionFind, collectionModel.txtConditionFind, collectionModel.txtConditionFind);
                }

                collectionModel.lstCollection = collectionService.SelectByWhere(strWhere).ToPagedList(page, pageSize);
                return View(collectionModel);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return View(collectionModel);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult create()
        {
            try
            {
                Collection collection = new Collection();
                collection.Type = true;
                collection.CollectionState = true;
                collection.TemplateLayouts = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem {Selected= true, Text="collection", Value="collection" },
                    new SelectListItem {Selected= true, Text="collection.list", Value="collection.list" },
                }, "Value", "Text", "1");

                TblRule rule = new TblRule();
                //rule.ColumnName = "ProductName";
                //rule.Relation = "equals";
                collection.TblRules.Add(rule);

                return View(collection);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return RedirectToAction("index", "collections");
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public string create(Collection collection, HttpPostedFileBase file)
        {
            collection.TemplateLayouts = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem {Selected= true, Text="collection", Value="collection" },
                    new SelectListItem {Selected= true, Text="collection.list", Value="collection.list" },
                }, "Value", "Text", "1");
            string strErrorMessage = "";
            if (ModelState.IsValid)
            {
                // insert collection
                if (collection.CollectionType == "custom")
                {
                    collection.ConditionForCollection = false;
                }
                else
                {
                    bool flg = false;
                    for (int i = 0; i < collection.TblRules.Count; i++)
                    {
                        if (string.IsNullOrEmpty(collection.TblRules[i].ConditionValue.Trim()))
                        {
                            strErrorMessage += "Giá trị lọc không được để trống<br/>";
                            flg = true;
                        }
                    }
                    if (flg)
                    {
                        return strErrorMessage;
                    }
                }
                collection.CreatedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                int collectionID = collectionService.Insert(collection);

                if (collectionID > 0)
                {
                    collection.CollectionID = collectionID;
                    LogService.WriteLog2DB(accountService.GetUserId(User.Identity.GetUserName()), (int)Common.ActionID.Insert, collectionID, SDateTime.GetYYYYMMddHmmSSNow(), General.GetIPAddress(), TableNameID, collection.CollectionName);
                    // update collection image url
                    string imageUrl = UploadImage(collectionID, file);
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        collection.CollectionImage = imageUrl;
                        collectionService.Update(collection);
                    }
                    // add rule for collection
                    if (collection.CollectionType == "smart")
                    {
                        if (collection.TblRules != null && collection.TblRules.Count > 0)
                        {
                            for (int i = 0; i < collection.TblRules.Count; i++)
                            {
                                TblRule rule = collection.TblRules[i];
                                rule.CollectionID = collectionID;
                                if (tblRuleService.CheckRuleValid(rule))
                                {
                                    tblRuleService.Insert(rule);
                                }
                            }
                        }

                        string linkCondition = "";
                        if (collection.ConditionForCollection)
                        {
                            linkCondition = "and";
                        }
                        else
                        {
                            linkCondition = "or";
                        }
                        string strConditionProductByRule = "1=1 and " + tblRuleService.GetConditionProductByListRule(collection.TblRules, linkCondition);
                        List<Product> listProduct = productService.GetByWhere(strConditionProductByRule);
                        foreach (var item in listProduct)
                        {
                            CollectionProduct collectionProduct = new CollectionProduct();
                            collectionProduct.CollectionID = collectionID;
                            collectionProduct.ProductID = item.ProductID;
                            collectionProductService.Insert(collectionProduct);
                        }
                    }
                    return collectionID.ToString();
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

        [Authorize(Roles = "Admin")]
        private string UploadImage(int collectionID, HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength / 1024 / 1024 > 2)
            {
                return "";
            }
            else if (file.ContentLength > 0)
            {
                try
                {
                    string folder = Server.MapPath("~/assets/uploads/collections/") + collectionID;
                    Directory.CreateDirectory(folder);

                    string strWhere = "";
                    strWhere = "ImageName like N'" + file.FileName + "' and ProductID = " + collectionID.ToString();
                    string imageUrl = "";
                    imageUrl = HttpContext.Request.Url.Authority == string.Empty ? Common.UrlHost : "http://" + HttpContext.Request.Url.Authority;
                    if (!imageUrl.EndsWith("/")) imageUrl += "/";
                    imageUrl += "assets/uploads/collections/" + collectionID + "/" + file.FileName;

                    string path = Path.Combine(folder, Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    return imageUrl;
                }
                catch (Exception ex)
                {
                    LogService.WriteException(ex);
                }
            }
            return "";
        }

        [Authorize(Roles = "Admin")]
        public ActionResult addRule(int index)
        {
            TblRule tblRule = new TblRule();
            ViewBag.index = index;
            return PartialView(tblRule);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult detail(int id, string strError, string strSuccess)
        {
            try
            {
                Collection collection = collectionService.GetByPrimaryKey(id);
                if (collection != null)
                {
                    ViewBag.strError = strError;
                    if (!string.IsNullOrEmpty(strSuccess))
                    {
                        if (strSuccess.Equals("update1"))
                        {
                            strSuccess = "Cập nhật thành công";
                        }
                    }
                    ViewBag.strSuccess = strSuccess;
                    collection.TemplateLayouts = new SelectList(new List<SelectListItem>
                    {
                        new SelectListItem {Selected= true, Text="collection", Value="collection" },
                        new SelectListItem {Selected= true, Text="collection.list", Value="collection.list" },
                    }, "Value", "Text", "1");
                    //collection.Products = collectionProductService.GetProductByCollectionID(id);
                    collection.CollectionProducts = collectionProductService.GetByCollectionID(id);
                    return View(collection);
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return RedirectToAction("index", "collections", new { strMessage = "notExist" });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public string detail(Collection collection, HttpPostedFileBase file)
        {
            try
            {
                collection.TemplateLayouts = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem {Selected= true, Text="collection", Value="collection" },
                    new SelectListItem {Selected= true, Text="collection.list", Value="collection.list" },
                }, "Value", "Text", "1");

                //create collectionID for all rules
                foreach (var item in collection.TblRules)
                {
                    item.CollectionID = collection.CollectionID;
                }

                string strErrorMessage = "";
                bool flg = false;
                if (ModelState.IsValid)
                {
                    // insert collection
                    if (collection.CollectionType == "custom")
                    {
                        collection.ConditionForCollection = false;
                    }
                    else if (collection.CollectionType == "smart")
                    {
                        for (int i = 0; i < collection.TblRules.Count; i++)
                        {
                            if (string.IsNullOrEmpty(collection.TblRules[i].ConditionValue))
                            {
                                strErrorMessage += "Giá trị lọc không được để trống<br/>";
                                flg = true;
                            }
                        }
                    }
                    if (!flg)
                    {
                        collection.ModifiedDateTime = SDateTime.GetYYYYMMddHmmSSNow();
                        bool result = collectionService.Update(collection);

                        if (result)
                        {
                            LogService.WriteLog2DB(accountService.GetUserId(User.Identity.GetUserName()), (int)Common.ActionID.Update, collection.CollectionID, SDateTime.GetYYYYMMddHmmSSNow(), General.GetIPAddress(), TableNameID, collection.CollectionName);

                            // delete image collection
                            string folder = Server.MapPath("~/assets/uploads/collections/") + collection.CollectionID;
                            ImageService.DeleteSubFolder(folder);

                            // update collection image url
                            string imageUrl = UploadImage(collection.CollectionID, file);
                            if (!string.IsNullOrEmpty(imageUrl))
                            {
                                collection.CollectionImage = imageUrl;
                                collectionService.Update(collection);
                            }
                            if (collection.CollectionType == "smart")
                            {
                                List<TblRule> listRuleExisted = tblRuleService.SelectByCollectionID(collection.CollectionID);
                                if (listRuleExisted == null || listRuleExisted.Count <= 0)
                                {
                                    for (int i = 0; i < collection.TblRules.Count; i++)
                                    {
                                        if (tblRuleService.CheckRuleValid(collection.TblRules[i]))
                                        {
                                            tblRuleService.Insert(collection.TblRules[i]);
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < collection.TblRules.Count; i++)
                                    {
                                        bool flgInsert = true;
                                        for (int j = 0; j < listRuleExisted.Count; j++)
                                        {
                                            if (tblRuleService.Equals(listRuleExisted[j], collection.TblRules[i]))
                                            {
                                                flgInsert = false;
                                            }
                                        }
                                        if (flgInsert)
                                        {
                                            tblRuleService.Insert(collection.TblRules[i]);
                                        }
                                    }
                                    for (int i = 0; i < listRuleExisted.Count; i++)
                                    {
                                        bool flgDelete = true;
                                        for (int j = 0; j < collection.TblRules.Count; j++)
                                        {
                                            if (tblRuleService.Equals(listRuleExisted[i], collection.TblRules[j]))
                                            {
                                                flgDelete = false;
                                            }
                                        }
                                        if (flgDelete)
                                        {
                                            tblRuleService.DeleteByPrimary(listRuleExisted[i].RuleID);
                                        }
                                    }
                                }
                                // get list prodct by all condition in collection
                                string linkCondition = "";
                                if (collection.ConditionForCollection)
                                {
                                    linkCondition = "and";
                                }
                                else
                                {
                                    linkCondition = "or";
                                }
                                string strConditionProductByRule = "1=1 and " + tblRuleService.GetConditionProductByListRule(collection.TblRules, linkCondition);
                                List<Product> listProductByRulesOfCollection = productService.GetByWhere(strConditionProductByRule);
                                List<CollectionProduct> listCollectionProductExisted = collectionProductService.GetByCollectionID(collection.CollectionID);
                                if (listProductByRulesOfCollection != null && listProductByRulesOfCollection.Count > 0)
                                {
                                    List<int> listProductAdd = new List<int>();
                                    List<int> listProductDelete = new List<int>();
                                    if (listCollectionProductExisted == null || listCollectionProductExisted.Count <= 0)
                                    {
                                        for (int i = 0; i < listProductByRulesOfCollection.Count; i++)
                                        {
                                            collectionProductService.Insert(new CollectionProduct
                                            {
                                                CollectionID = collection.CollectionID,
                                                ProductID = listProductByRulesOfCollection[i].ProductID,
                                            });
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 0; i < listProductByRulesOfCollection.Count; i++)
                                        {
                                            bool flgInsert = true;
                                            for (int j = 0; j < listCollectionProductExisted.Count; j++)
                                            {
                                                if (listCollectionProductExisted[j].ProductID == listProductByRulesOfCollection[i].ProductID)
                                                {
                                                    flgInsert = false;
                                                }
                                            }
                                            if (flgInsert)
                                            {
                                                collectionProductService.Insert(new CollectionProduct
                                                {
                                                    CollectionID = collection.CollectionID,
                                                    ProductID = listProductByRulesOfCollection[i].ProductID,
                                                });
                                            }
                                        }
                                        for (int i = 0; i < listCollectionProductExisted.Count; i++)
                                        {
                                            bool flgDelete = true;
                                            for (int j = 0; j < listProductByRulesOfCollection.Count; j++)
                                            {
                                                if (listProductByRulesOfCollection[j].ProductID == listCollectionProductExisted[i].ProductID)
                                                {
                                                    flgDelete = false;
                                                }
                                            }
                                            if (flgDelete)
                                            {
                                                collectionProductService.DeleteByPrimary(listCollectionProductExisted[i].ID);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    collectionProductService.DeleteByCollectionID(collection.CollectionID);
                                }
                            }
                            return SString.ConverToString(collection.CollectionID);
                        }
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
                return ex.Message;
            }
        }

        public string deleteImage(int collectionID)
        {
            try
            {
                Collection collection = collectionService.GetByPrimaryKey(collectionID);
                if (collection != null)
                {
                    // delete image collection
                    string folder = Server.MapPath("~/assets/uploads/collections/") + collection.CollectionID;
                    ImageService.DeleteSubFolder(folder);

                    collection.CollectionImage = null;
                    if (collectionService.Update(collection))
                    {
                        return collectionID.ToString();
                    }
                }
                return "Xóa ảnh danh mục thất bại";
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return ex.Message;
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult delete(int id)
        {
            try
            {
                tblRuleService.DeleteByCollectionID(id);
                collectionProductService.DeleteByCollectionID(id);

                // delete image collection
                string folder = Server.MapPath("~/assets/uploads/collections/") + id;
                ImageService.DeleteFolder(folder);

                Collection collection = collectionService.GetByPrimaryKey(id);
                if (collection != null)
                {
                    if (collectionService.DeleteByPrimary(id))
                    {
                        LogService.WriteLog2DB(accountService.GetUserId(User.Identity.GetUserName()), (int)Common.ActionID.Insert, id, SDateTime.GetYYYYMMddHmmSSNow(), General.GetIPAddress(), TableNameID, collection.CollectionName);
                        return RedirectToAction("", "collections", new { strMessage = "delete1" });
                    }
                }
                return RedirectToAction("detail", "collections", new { id = id, strError = "Xóa danh mục lỗi" });
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return RedirectToAction("detail", "collections", new { id = id, strError = "Xóa danh mục lỗi" });
            }
        }

        public ActionResult deleteProductOutCollection(int id, int collectionProductID)
        {
            try
            {
                collectionProductService.DeleteByPrimary(collectionProductID);
                return RedirectToAction("detail", "collections", new { id = id, strSuccess = "update1" });
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return RedirectToAction("detail", "collections", new { id = id, strError = "Xoá sản phẩm khỏi danh mục không thành công" });
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult dropdown(int productID, string query)
        {
            try
            {
                CollectiontDropdownModel collectiontDropdownModel = new CollectiontDropdownModel();
                List<CollectionDropDown> collectionDropdowns = new List<CollectionDropDown>();
                List<Collection> collections = new List<Collection>();
                string where = "";
                if (string.IsNullOrEmpty(query))
                {
                    where = "CollectionType like 'custom'";
                }
                else
                {
                    where = string.Format("CollectionType like 'custom' and CollectionName like N'%{0}%'", query);
                }
                collections = collectionService.SelectByWhere(where);
                if (collections != null && collections.Count > 0)
                {
                    foreach (var item in collections)
                    {
                        CollectionDropDown temp = new CollectionDropDown();
                        temp.CollectionID = item.CollectionID;
                        temp.CollectionName = item.CollectionName;
                        if (collectionProductService.CheckExistCollectionProduct(item.CollectionID, productID))
                        {
                            temp.Choice = true;
                        }
                        else
                        {
                            temp.Choice = false;
                        }
                        collectionDropdowns.Add(temp);
                    }
                    collectiontDropdownModel.CollectionDropDowns = collectionDropdowns;
                    collectiontDropdownModel.ProductID = productID;
                }
                //collectiontDropdownModel.ConditionFindProduct = query;
                return View(collectiontDropdownModel);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult dropdown(CollectiontDropdownModel collectiontDropdownModel)
        {
            try
            {
                if (collectiontDropdownModel.CollectionDropDowns != null && collectiontDropdownModel.CollectionDropDowns.Count > 0)
                {
                    foreach (var item in collectiontDropdownModel.CollectionDropDowns)
                    {
                        if (item.Choice)
                        {
                            if (!collectionProductService.CheckExistCollectionProduct(item.CollectionID, collectiontDropdownModel.ProductID))
                            {
                                collectionProductService.Insert(new CollectionProduct
                                {
                                    ProductID = collectiontDropdownModel.ProductID,
                                    CollectionID = item.CollectionID
                                });
                            }
                        }
                    }
                }
                return RedirectToAction("detail", "products", new { id = collectiontDropdownModel.ProductID, strMessage = "1" });
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }
    }
}