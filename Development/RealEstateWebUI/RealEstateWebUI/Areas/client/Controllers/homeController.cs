using RealEstateWebUI.Areas.admin.Models;
using RealEstateWebUI.Areas.admin.Services;
using RealEstateWebUI.Areas.client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RealEstateWebUI.Areas.client.Controllers
{
    public class homeController : Controller
    {
        private ProductService productService = new ProductService();
        private VariantService variantService = new VariantService();
        private ImageService imageService = new ImageService();
        private CollectionProductService collectionProductService = new CollectionProductService();

        // GET: client/home
        public ActionResult index(HomeClientViewModel homeViewModel)
        {
            try
            {
                homeViewModel.ProductsBestSelling = productService.GetBestSelling();
                if (homeViewModel.ProductsBestSelling != null && homeViewModel.ProductsBestSelling.Count > 0)
                {
                    for (int i = 0; i < homeViewModel.ProductsBestSelling.Count; i++)
                    {
                        homeViewModel.ProductsBestSelling[i].Variants = variantService.GetByProductID(homeViewModel.ProductsBestSelling[i].ProductID);
                        homeViewModel.ProductsBestSelling[i].Images = imageService.GetByProductID(homeViewModel.ProductsBestSelling[i].ProductID);
                    }
                }
                homeViewModel.ProductsCreateDESC = productService.GetAll("CreatedDateTime");
                if (homeViewModel.ProductsCreateDESC != null && homeViewModel.ProductsCreateDESC.Count > 0)
                {
                    for (int i = 0; i < homeViewModel.ProductsCreateDESC.Count; i++)
                    {
                        homeViewModel.ProductsCreateDESC[i].Variants = variantService.GetByProductID(homeViewModel.ProductsCreateDESC[i].ProductID);
                        homeViewModel.ProductsCreateDESC[i].Images = imageService.GetByProductID(homeViewModel.ProductsCreateDESC[i].ProductID);
                    }
                }
                List<CollectionProduct> collectionProducts = collectionProductService.GetByCollectionID(Common.CollectionID_DiscountPrice);
                if (collectionProducts != null && collectionProducts.Count > 0)
                {
                    homeViewModel.ProductsDiscountPrice = new List<Product>();
                    foreach (var item in collectionProducts)
                    {
                        Product product = productService.GetByPrimaryKey(item.ProductID);
                        homeViewModel.ProductsDiscountPrice.Add(product);
                    }
                }
                if (homeViewModel.ProductsDiscountPrice != null && homeViewModel.ProductsDiscountPrice.Count > 0)
                {
                    for (int i = 0; i < homeViewModel.ProductsDiscountPrice.Count; i++)
                    {
                        homeViewModel.ProductsDiscountPrice[i].Variants = variantService.GetByProductID(homeViewModel.ProductsDiscountPrice[i].ProductID);
                        homeViewModel.ProductsDiscountPrice[i].Images = imageService.GetByProductID(homeViewModel.ProductsDiscountPrice[i].ProductID);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }

            return View(homeViewModel);
        }
        public ActionResult contact()
        {
            return View();
        }
        public ActionResult newLetterPopup()
        {
            return View();
        }

        public ActionResult skin()
        {
            return View();
        }
    }
}