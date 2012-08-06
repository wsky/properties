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
        private IContextService _context;
        private IApplicationService _appService;
        public ApplicationController(IContextService context, IApplicationService appService)
        {
            this._context = context;
            this._appService = appService;
        }

        public ActionResult Index()
        {
            ViewBag.Apps = this._appService.GetApps(this._context.Current);
            return View();
        }
        [HttpPost]
        public ActionResult Create(string name)
        {
            var app = new Application(this._context.Current);
            app.SetName(name);
            this._appService.Create(app);
            return RedirectToAction(app.ID.ToString());
        }

        public ActionResult Usage(string id)
        {
            this.Prepare(id);
            return View();
        }
        public ActionResult Setting(string id)
        {
            var app = this.Prepare(id);
            return View();
        }
        public ActionResult Share(string id)
        {
            var app = this.Prepare(id);
            return View();
        }
        public ActionResult Integrate(string id)
        {
            var app = this.Prepare(id);
            return View();
        }

        private Application CreateTemp(string name)
        {
            return new Application(new Account(Guid.NewGuid().ToString()));
        }
        private Application Prepare(string id)
        {
            Guid appId;
            Guid.TryParse(id, out appId);
            var apps = this._appService.GetApps(this._context.Current);
            var app = apps.FirstOrDefault(o => o.ID == appId);
            ViewBag.Apps = apps;
            ViewBag.App = app;

            if (app == null)
                throw new KnownException("Application Not Found");
            if (app.CreatorAccountId != this._context.Current.ID)
                throw new KnownException("Donot Have Permission");

            return app;
        }
    }
}
