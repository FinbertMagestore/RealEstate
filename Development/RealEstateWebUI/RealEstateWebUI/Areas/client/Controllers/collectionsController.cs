using RealEstateWebUI.Areas.admin.Models;
using RealEstateWebUI.Areas.admin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RealEstateWebUI.Areas.client.Controllers
{
    public class collectionsController : Controller
    {
        CollectionProductService collectionProductService = new CollectionProductService();
        CollectionService collectionService = new CollectionService();
        // GET: client/collections
        public ActionResult index()
        {
            return RedirectToAction("getAll");
        }

        public ActionResult getAll()
        {
            string where = string.Format("CollectionID NOT IN ({0})",Common.CollectionID_DiscountPrice);
            List<Collection> collections = collectionService.GetByWhere(where);
            if (collections != null)
            {
                for (int i = 0; i < collections.Count; i++)
                {
                    collections[i].CollectionProducts = collectionProductService.GetByCollectionID(collections[i].CollectionID);
                    if (i > 5)
                    {
                        collections[i] = null;
                    }
                }
            }
            return PartialView(collections);
        }
    }
}