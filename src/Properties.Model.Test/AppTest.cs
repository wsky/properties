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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Properties.Model.Test
{
    [TestFixture]
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class AppTest : BaseTest
    {
        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void Create()
        {
            var account = this.CreateAccount();
            var app = new App(account);
            Assert.IsNotNullOrEmpty(app.Token);

            this._appService.Create(app);

            this.Evict(app);

            var app2 = this._appService.GetApp(app.ID);
            Assert.AreEqual(app2.Name, app.Name);
            Assert.AreEqual(app2.CreatorAccountId, app.CreatorAccountId);
            Assert.AreEqual(app2.Token, app.Token);
        }
        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void SetName()
        {

        }
        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void GetApps()
        {
            var account = this.CreateAccount();
            this._appService.Create(new App(account));
            this._appService.Create(new App(account));
            Assert.AreEqual(2, this._appService.GetApps(account).Count());
        }
    }
}
