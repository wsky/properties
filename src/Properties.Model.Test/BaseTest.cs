//  Copyright 2012 wsky. wskyhx@gmail.com
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//        http://www.apache.org/licenses/LICENSE-2.0
//  Unless required by applicable law or agreed to in writing, 
//  software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeSharp.Core;
using CodeSharp.Core.Castles;
using CodeSharp.Core.Services;
using NUnit.Framework;

namespace Properties.Model.Test
{
    [TestFixture]
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class BaseTest
    {
        protected ILog _log;
        protected Castle.Facilities.NHibernateIntegration.ISessionManager _sessionManager;
        protected IAccountService _accountService;
        protected IAppService _appService;
        protected IConfigurationService _configService;

        [TestFixtureSetUp]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestInitialize]
        public void TestFixtureSetUp()
        {
            try
            {
                CodeSharp.Core.Configuration.ConfigWithEmbeddedXml(null
                    , "application_config"
                    , Assembly.GetExecutingAssembly()
                    , "Properties.Model.Test.ConfigFiles")
                    .RenderProperties()
                    .Castle(o => this.Resolve(o.Container));

                DependencyResolver.Resolve<ILockHelper>().Init<Account>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            this._log = DependencyResolver.Resolve<ILoggerFactory>().Create(this.GetType());
            this._sessionManager = DependencyResolver.Resolve<Castle.Facilities.NHibernateIntegration.ISessionManager>();
            this._accountService = DependencyResolver.Resolve<IAccountService>();
            this._appService = DependencyResolver.Resolve<IAppService>();
            this._configService = DependencyResolver.Resolve<IConfigurationService>();

            DependencyResolver.Resolve<ILockHelper>().Require<Account>();
        }

        protected virtual void Resolve(Castle.Windsor.IWindsorContainer windsor)
        {
            //常规注册
            windsor.RegisterRepositories(Assembly.Load("Properties.Repositories"));
            windsor.RegisterServices(Assembly.Load("Properties.Model"));
            windsor.RegisterComponent(Assembly.Load("Properties.Model"));
        }
        protected void Evict(object entity)
        {
            this._sessionManager.OpenSession().Evict(entity);
        }
        protected void Idle()
        {
            System.Threading.Thread.Sleep(100);
        }
        protected void Idle(int second)
        {
            System.Threading.Thread.Sleep(second * 1000);
        }
        protected string RandomString()
        {
            return Guid.NewGuid().ToString();
        }
        protected void AssertParallel(Action func, int total, int expected)
        {
            var flag = 0;
            new int[total].AsParallel().ForAll(i =>
            {
                try
                {
                    func();
                    System.Threading.Interlocked.Increment(ref flag);
                }
                catch (Exception e) { this._log.Info(e.Message); }
            });
            Assert.AreEqual(expected, flag);
        }
        protected Account CreateAccount()
        {
            var a = new Account(this.RandomString());
            this._accountService.Create(a);
            return a;
        }
        protected App CreateApp()
        {
            var a = new App(this.CreateAccount());
            this._appService.Create(a);
            return a;
        }
        protected string GetString(int length)
        {
            var str = "";
            for (var i = 0; i < length; i++)
                str += "0";
            return str;
        }
    }
}
