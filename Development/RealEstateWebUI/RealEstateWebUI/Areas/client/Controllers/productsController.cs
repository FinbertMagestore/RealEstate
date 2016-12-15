using PagedList;
using RealEstateWebUI.Areas.admin.Models;
using RealEstateWebUI.Areas.admin.Services;
using RealEstateWebUI.Areas.admin.UtilzGeneral;
using RealEstateWebUI.Areas.client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RealEstateWebUI.Areas.client.Controllers
{
    public class productsController : Controller
    {
        private ProductService productService = new ProductService();
        private VariantService variantService = new VariantService();
        private ImageService imageService = new ImageService();
        private OptionService optionService = new OptionService();
        private TagService tagService = new TagService();
        private CollectionProductService collectionProductService = new CollectionProductService();

        // GET: client/products
        public ActionResult index(ProductClientViewModel productClientViewModel)
        {
            try
            {
                string orderby = "ProductName", sortOrder = "asc";
                List<Product> products = null;

                if (!string.IsNullOrEmpty(productClientViewModel.sortOrder))
                {
                    if (productClientViewModel.sortOrder.Equals("name-asc"))
                    {
                        orderby = "ProductName";
                        sortOrder = "asc";
                    }
                    else if (productClientViewModel.sortOrder.Equals("name-desc"))
                    {
                        orderby = "ProductName";
                        sortOrder = "desc";
                    }
                    else if (productClientViewModel.sortOrder.Equals("create-desc"))
                    {
                        orderby = "CreatedDateTime";
                        sortOrder = "desc";
                    }
                    else if (productClientViewModel.sortOrder.Equals("best-selling"))
                    {
                        products = productService.GetBestSelling();
                    }
                }
                if (SNumber.ToNumber(productClientViewModel.numberView) == 0)
                {
                    productClientViewModel.numberView = 9;
                }

                // not find product
                if (products == null || products.Count == 0)
                {
                    products = productService.GetAll(orderby, sortOrder);
                }
                // get variant and images for product
                if (products != null && products.Count > 0)
                {
                    for (int i = 0; i < products.Count; i++)
                    {
                        products[i].Variants = variantService.GetByProductID(products[i].ProductID);
                        products[i].Images = imageService.GetByProductID(products[i].ProductID);
                    }
                }
                if (SNumber.ToNumber(productClientViewModel.pageNumber) == 0)
                {
                    productClientViewModel.pageNumber = 1;
                }
                productClientViewModel.CountProduct = products.Count;
                productClientViewModel.Products = products.ToPagedList(productClientViewModel.pageNumber, productClientViewModel.numberView);

                if (productClientViewModel.view == "list")
                {
                    return View("list", productClientViewModel);
                }
                else
                {
                    return View("grid", productClientViewModel);
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                throw;
            }
        }

        public ActionResult detail(int id)
        {
            try
            {
                Product product = productService.GetByPrimaryKey(id);
                if (product != null)
                {
                    product.Variants = variantService.GetByProductID(product.ProductID);
                    product.Images = imageService.GetByProductID(product.ProductID);
                    product.Options = optionService.GetByProductID(product.ProductID);
                    if (product.Options != null && product.Options.Count > 0)
                    {
                        foreach (var item in product.Options)
                        {
                            if (!string.IsNullOrEmpty(item.OptionValue))
                            {
                                if (item.OptionName != "Title" && item.OptionValue != "DefaultTitle")
                                {
                                    item.OptionValues = (SString.RemoveElementAtBeginEnd(item.OptionValue, ",")).Split(',').ToList<string>();
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(product.Tags))
                    {
                        List<string> listTag = (SString.RemoveElementAtBeginEnd(product.Tags, ",")).Split(',').ToList<string>();
                        foreach (var item in listTag)
                        {
                            product.ListTag.Add(new Tag
                            {
                                TagName = item
                            });
                        }
                    }

                    string where = string.Format("SupplierID={0} and ProductStyleID={1} and ProductID <>{2}", product.SupplierID, product.ProductStyleID, product.ProductID);
                    product.ProductsRelation = productService.GetByWhere(where);
                    if (product.ProductsRelation != null && product.ProductsRelation.Count > 0)
                    {
                        foreach (var item in product.ProductsRelation)
                        {
                            item.Variants = variantService.GetByProductID(item.ProductID);
                            item.Images = imageService.GetByProductID(item.ProductID);
                        }
                    }
                    return View(product);
                }

                return RedirectToAction("grid");
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                throw;
            }
        }

    }
}