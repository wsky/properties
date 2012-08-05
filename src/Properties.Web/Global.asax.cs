using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using CodeSharp.Core.Castles;
using CodeSharp.Core.Web;
using CodeSharp.Framework.Castles;
using CodeSharp.Framework.Castles.Web;
using Properties.Model;

namespace Properties.Web
{
    public class MvcApplication : CodeSharp.Framework.Castles.Web.WebApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new RequireHttpsAttribute());
        }
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.MapRoute("Configuration", "{app}/Configuration", new
            {
                controller = "Configuration",
                action = "Index",
                app = UrlParameter.Optional
            });
            routes.MapRoute("Properties", "{app}/Properties/{name}", new
            {
                controller = "Configuration",
                action = "Properties",
                app = UrlParameter.Optional,
                name = UrlParameter.Optional
            });
            routes.MapRoute("Default", "{controller}/{action}/{id}", new
            {
                controller = "Application",
                action = "Index",
                id = UrlParameter.Optional
            });
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            CodeSharp.Framework.SystemConfig
                .Configure("PropertiesWeb")
                .Castle()
                .BusinessDependency(Assembly.Load("Properties.Model"), Assembly.Load("Properties.Repositories"))
                .Resolve(o =>
                {
                    var windsor = o.Container;
                    windsor.ControllerFactory();
                    windsor.RegisterControllers(Assembly.GetExecutingAssembly());
                });

            //init something extra
            Lock.InitAll(CodeSharp.Core.Services.DependencyResolver.Resolve<ILockHelper>());
        }
    }
    /// <summary>always http, can deal with stunnel...
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class RequireHttpsAttribute : System.Web.Mvc.RequireHttpsAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsSecureConnection)
                return;
            if (string.Equals(filterContext.HttpContext.Request.Headers["X-Forwarded-Proto"]
                , "https"
                , StringComparison.InvariantCultureIgnoreCase))
                return;
            if (filterContext.HttpContext.Request.IsLocal)
                return;
            this.HandleNonHttpsRequest(filterContext);
        }
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AuthenticationAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }
    }
}