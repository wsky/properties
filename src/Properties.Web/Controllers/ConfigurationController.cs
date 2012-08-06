using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using Properties.Model;

namespace Properties.Web.Controllers
{
    public class ConfigurationController : Controller
    {
        private IContextService _context;
        private IApplicationService _appService;
        private IConfigurationService _configService;
        public ConfigurationController(IContextService context
            , IApplicationService appService
            , IConfigurationService configService)
        {
            this._context = context;
            this._appService = appService;
            this._configService = configService;
        }

        public ActionResult Index(string appId)
        {
            Guid id;
            Guid.TryParse(appId, out id);
            var apps = this._appService.GetApps(this._context.Current);
            var a = Guid.TryParse(appId, out id)
                ? apps.FirstOrDefault(o => o.ID == id)
                : apps.FirstOrDefault();
            ViewBag.Apps = apps;
            ViewBag.App = a;
            ViewBag.Configurations = this._configService.GetConfigurations(a);
            return View();
        }
        [HttpPost]
        public ActionResult Index(string appId, string name)
        {
            var app = this.GetApp(appId);
            this._configService.Create(new Configuration(app, name));
            return Redirect(Url.AppConfigs(app));
        }
        public ActionResult Properties(string id)
        {
            ViewBag.Configuration = this.GetConfig(id);
            return View();
        }
        [HttpPost]
        public ActionResult Properties(string id, string name)
        {
            var c = this.GetConfig(id);
            c.AddProperty(name);
            this._configService.Update(c);
            ViewBag.Configuration = c;
            return View();
        }

        private Configuration CreateTemp(string name)
        {
            var c = new Configuration(new Application(new Account(Guid.NewGuid().ToString())), name);
            c.AddFlag("Debug");
            c.AddFlag("Test");
            c.AddFlag("Release");
            c.AddFlag("Performance");
            c.AddFlag("Try?");
            for (var i = 0; i < 50; i++)
            {
                var p = c.AddProperty("key-" + i);
                p.SetDescription("It's a property named " + p.Name);
                p.Value = "good job, properties dose." + this.GetString(100);
                p["Debug"] = "Debug value " + this.GetString(100);
                p["Test"] = "Test value " + this.GetString(100);
                p["Release"] = "Release value " + this.GetString(100);
            }
            return c;
        }
        private string GetString(int length)
        {
            var str = "";
            for (var i = 0; i < length; i++)
                str += "K";
            return str;
        }
        private Configuration GetConfig(string id)
        {
            Guid configId;
            Guid.TryParse(id, out configId);
            var c = this._configService.GetConfiguration(configId);
            if (c != null)
                this.GetApp(c.AppId);
            return c;
        }
        private Application GetApp(string id)
        {
            Guid appId;
            Guid.TryParse(id, out appId);
            return this.GetApp(appId);
        }
        private Application GetApp(Guid id)
        {
            var app = this._appService.GetApp(id);
            ViewBag.App = app;
            if (app == null)
                throw new KnownException("Application Not Found");
            if (app.CreatorAccountId != this._context.Current.ID)
                throw new KnownException("Donot Have Permission");
            return app;
        }
    }
}