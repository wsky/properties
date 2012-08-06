//Copyright (c) CodeSharp.  All rights reserved. - http://www.icodesharp.com/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Properties.Model;

namespace Properties.Web.Controllers
{
    public interface IContextService
    {
        Account Current { get; }
    }
    public class WebContextService : IContextService
    {
        private IAccountService _accountService;
        public WebContextService(IAccountService accountService)
        {
            this._accountService = accountService;
        }

        #region IContextService Members

        public Account Current
        {
            get
            {
                return HttpContext.Current.User.Identity.IsAuthenticated
                    ? this._accountService.GetAccount(HttpContext.Current.User.Identity.Name)
                    : null;
            }
        }

        #endregion
    }
}