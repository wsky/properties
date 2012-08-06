using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Properties.Model;

namespace Properties.Web.Controllers
{
    public class AccountController : Controller
    {
        private string _sysConfig_versionFlag;
        private IAccountService _accountService;
        public AccountController(IAccountService accountService, string sysConfig_versionFlag)
        {
            this._accountService = accountService;
            this._sysConfig_versionFlag = sysConfig_versionFlag;
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var a = this._accountService.GetAccount(username);
            var skip = this._sysConfig_versionFlag != "Release";
            
            if (a == null && skip)
                this._accountService.Create(a = new Account(username));
            if (a.Verify(password) || skip)
                FormsAuthentication.RedirectFromLoginPage(username, true);

            return View();
        }

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return Redirect(FormsAuthentication.LoginUrl);
        }
    }
}