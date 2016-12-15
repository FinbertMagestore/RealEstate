using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RealEstateWebUI.Areas.client.Controllers
{
    public class blogsController : Controller
    {
        // GET: client/blogs
        public ActionResult index()
        {
            return View();
        }
        public ActionResult detail(int id)
        {
            return View();
        }
    }
}