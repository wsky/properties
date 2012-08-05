using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Properties.Model;

namespace Properties.Web.Controllers
{
    public class ApplicationController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Apps = new List<Application>() { this.CreateTemp("Cooper"), this.CreateTemp("MyApp") };
            return View();
        }

        private Application CreateTemp(string name)
        {
            return new Application(new Account(Guid.NewGuid().ToString()));
        }

    }
}
