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
        private IConfigurationService _configService;
        public ConfigurationController(IConfigurationService configService)
        {
            this._configService = configService;
        }

        public ActionResult Index(string app)
        {
            ViewBag.Configurations = new List<Configuration>()
            {
                this.CreateTemp("CooperWeb"),
                this.CreateTemp("AppDemo"),
                this.CreateTemp("MyApp2"),
                this.CreateTemp("HelloWorld"),
                this.CreateTemp("Sample")
            };
            return View();
        }
        public ActionResult Properties(string app, string name)
        {
            ViewBag.Configuration = this.CreateTemp(name);
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
    }
}