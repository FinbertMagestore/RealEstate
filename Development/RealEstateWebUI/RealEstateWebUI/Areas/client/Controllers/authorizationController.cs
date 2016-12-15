using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RealEstateWebUI.Areas.client.Controllers
{
    public class authorizationController : Controller
    {
        // GET: client/authorization
        public ActionResult index()
        {
            return RedirectToAction("login");
        }

        public ActionResult login()
        {
            return View();
        }

        public ActionResult register()
        {
            return View();
        }

    }
}