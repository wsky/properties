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

            routes.MapRoute("Application", "Application", new { controller = "Application", action = "Index" });
            routes.MapRoute("Application.Create", "Application/Create", new { controller = "Application", action = "Create" });
            routes.MapRoute("Application.Configuration", "Application/{appId}/Configuration", new { controller = "Configuration", action = "Index", appId = UrlParameter.Optional });
            routes.MapRoute("Application.Detail", "Application/{id}/{action}", new { controller = "Application", action = "Usage", id = UrlParameter.Optional });

            routes.MapRoute("Configuration", "Configuration", new { controller = "Configuration", action = "Index" });
            routes.MapRoute("Configuration.Properties", "Configuration/{id}", new { controller = "Configuration", action = "Properties", id = UrlParameter.Optional });
            routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Application", action = "Index", id = UrlParameter.Optional });
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            CodeSharp.Framework.SystemConfig
                .Configure("PropertiesWeb")
                .Castle()
                .BusinessDependency(Assembly.Load("Properties.Model")
                , Assembly.Load("Properties.Repositories")
                , Assembly.GetExecutingAssembly())
                .Resolve(o =>
                {
                    var windsor = o.Container;
                    windsor.ControllerFactory();
                    windsor.RegisterControllers(Assembly.GetExecutingAssembly());
                });

            //init something extra
            Lock.InitAll(CodeSharp.Core.Services.DependencyResolver.Resolve<ILockHelper>());
        }
        protected override bool IsKnownException(Exception e)
        {
            return base.IsKnownException(e) || e is KnownException;
        }
    }
    public class KnownException : Exception
    {
        public KnownException(string message) : base(message) { }
        public KnownException(string message, Exception inner) : base(message, inner) { }
        //public KnownException(string message, System.Net.WebExceptionStatus status) : base(message, status) { }
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

public static class Extensions
{
    public static string App(this UrlHelper url, Application a)
    {
        return url.Content(string.Format("~/Application/{0}", a.ID));
    }
    public static string Config(this UrlHelper url, Configuration c)
    {
        return url.Content(string.Format("~/Configuration/{0}", c.ID));
    }
    public static string AppConfigs(this UrlHelper url, Application a)
    {
        return url.Content(string.Format("~/Application/{0}/Configuration", a.ID));
    }

    public static string Filter(this string input)
    {
        return string.IsNullOrEmpty(input) ? input : input.Replace("<script", "<").Replace("script>", ">");
    }
}