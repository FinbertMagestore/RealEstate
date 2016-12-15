using System.Web.Mvc;

namespace RealEstateWebUI.Areas.admin
{
    public class adminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "authorization",
                url: "admin/authorization/{action}",
                defaults: new { controller = "authorization", action = "index" }
            );

            context.MapRoute(
                name: "accountDetail",
                url: "admin/accounts/{id}",
                defaults: new { controller = "accounts", action = "detail" },
                constraints: new { id = @"\d+" }
            );

            context.MapRoute(
                name: "dashboard",
                url: "admin/dashboard/",
                defaults: new { controller = "dashboard", action = "index" }
            );

            #region order
            context.MapRoute(
                name: "orders",
                url: "admin/orders/",
                defaults: new { controller = "orders", action = "index" }
            );

            context.MapRoute(
                name: "orderDetail",
                url: "admin/orders/{id}",
                defaults: new { controller = "orders", action = "detail" },
                constraints: new { id = @"\d+" }
            );

            context.MapRoute(
                name: "orderAction",
                url: "admin/orders/{action}",
                defaults: new { controller = "orders", action = "index" }
            );

            context.MapRoute(
                name: "orderDefault",
                url: "admin/orders/{action}/{id}",
                defaults: new { controller = "orders", id = UrlParameter.Optional },
                constraints: new { id = @"\d+" }
            );
            #endregion

            #region variant
            context.MapRoute(
                name: "variantDetail",
                url: "admin/products/{productID}/variants/{id}",
                defaults: new { controller = "variants", action = "detail" },
                constraints: new { id = @"\d+", productID = @"\d+" }
            );

            context.MapRoute(
                name: "variantAction",
                url: "admin/products/{productID}/variants/{action}",
                defaults: new { controller = "variants"},
                constraints: new { productID = @"\d+" }
            );

            context.MapRoute(
                name: "variantDefault",
                url: "admin/products/{productID}/variants/{action}/{id}",
                defaults: new { controller = "variants", id = UrlParameter.Optional },
                constraints: new { id = @"\d+", productID = @"\d+" }
            );
            #endregion

            #region product
            context.MapRoute(
                name: "products",
                url: "admin/products/",
                defaults: new { controller = "products", action = "index" }
            );

            context.MapRoute(
                name: "productDetail",
                url: "admin/products/{id}",
                defaults: new { controller = "products", action = "detail" },
                constraints: new { id = @"\d+" }
            );

            context.MapRoute(
                name: "productAction",
                url: "admin/products/{action}",
                defaults: new { controller = "products", action = "index" }
            );

            context.MapRoute(
                name: "productDefault",
                url: "admin/products/{action}/{id}",
                defaults: new { controller = "products", action = "index", id = UrlParameter.Optional },
                constraints: new { id = @"\d+" }
            );
            #endregion

            #region collection
            context.MapRoute(
                name: "collections",
                url: "admin/collections/",
                defaults: new { controller = "collections", action = "index" }
            );

            context.MapRoute(
                name: "collectionDetail",
                url: "admin/collections/{id}",
                defaults: new { controller = "collections", action = "detail" },
                constraints: new { id = @"\d+" }
            );

            context.MapRoute(
                name: "collectionAction",
                url: "admin/collections/{action}",
                defaults: new { controller = "collections", action = "index" }
            );

            context.MapRoute(
                name: "collectionDefault",
                url: "admin/collections/{action}/{id}",
                defaults: new { controller = "collections", action = "index", id = UrlParameter.Optional },
                constraints: new { id = @"\d+" }
            );
            #endregion

            #region customer
            context.MapRoute(
                name: "customers",
                url: "admin/customers/",
                defaults: new { controller = "customers", action = "index" }
            );

            context.MapRoute(
                name: "customerDetail",
                url: "admin/customers/{id}",
                defaults: new { controller = "customers", action = "detail" },
                constraints: new { id = @"\d+" }
            );

            context.MapRoute(
                name: "customerAction",
                url: "admin/customers/{action}",
                defaults: new { controller = "customers", action = "index" }
            );

            context.MapRoute(
                name: "customerDefault",
                url: "admin/customers/{action}/{id}",
                defaults: new { controller = "customers" , id = UrlParameter.Optional},
                constraints: new { id = @"\d+" }
            );
            #endregion

            context.MapRoute(
                "admin start",
                "admin",
                new { controller = "authorization", action = "login" }
            );

            context.MapRoute(
                "admin_default",
                "admin/{controller}/{action}/{id}",
                new { id = UrlParameter.Optional }
            );
        }
    }
}