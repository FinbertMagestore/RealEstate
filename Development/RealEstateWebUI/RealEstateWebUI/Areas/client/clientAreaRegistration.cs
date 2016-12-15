using System.Web.Mvc;

namespace RealEstateWebUI.Areas.client
{
    public class clientAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "client";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "authorization_client",
                "client/authoziration/{action}",
                new { controller = "authorization", action = "index"}
            );

            context.MapRoute(
                "blogs_client",
                "client/blogs/{action}/{id}",
                new { controller = "blogs", action = "index", id = UrlParameter.Optional },
                constraints: new { id = @"\d+" }
            );

            context.MapRoute(
                "blogs_detail_client",
                "client/blogs/{id}",
                new { controller = "blogs", action = "detail", id = UrlParameter.Optional },
                constraints: new { id = @"\d+" }
            );

            context.MapRoute(
                "cart_client",
                "client/cart/{action}/{id}",
                new { controller = "cart", action = "index", id = UrlParameter.Optional },
                constraints: new { id = @"\d+" }
            );

            context.MapRoute(
                "checkout_detail_client",
                "client/checkout/{cookieID}",
                new { controller = "checkout", action = "index", cookieID = UrlParameter.Optional }
            );

            context.MapRoute(
                "checkout_client",
                "client/checkout/{action}/{id}",
                new { controller = "checkout", action = "index", id = UrlParameter.Optional },
                constraints: new { id = @"\d+" }
            );

            context.MapRoute(
                "collections_client",
                "client/collections/{action}/{id}",
                new { controller = "collections", action = "index", id = UrlParameter.Optional },
                constraints: new { id = @"\d+" }
            );

            context.MapRoute(
                "home_skin_clien",
                "client/home/skin",
                new { controller = "home", action = "skin", id = UrlParameter.Optional },
                constraints: new { id = @"\d+" }
            );

            context.MapRoute(
                "home_client",
                "client/home/{action}/{id}",
                new { controller = "home", action = "index", id = UrlParameter.Optional },
                constraints: new { id = @"\d+" }
            );

            context.MapRoute(
                "products_client",
                "client/products/{action}/{id}",
                new { controller = "products", action = "index", id = UrlParameter.Optional },
                constraints: new { id = @"\d+" }
            );

            context.MapRoute(
                "products_detail_client",
                "client/products/{id}",
                new { controller = "products", action = "detail", id = UrlParameter.Optional },
                constraints: new { id = @"\d+" }
            );

            context.MapRoute(
                "Client_default",
                "client/{controller}/{action}/{id}",
                new { controller = "home", action = "index", id = UrlParameter.Optional }
            );
        }
    }
}