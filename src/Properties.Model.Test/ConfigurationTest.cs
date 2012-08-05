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
    public class ConfigurationTest : BaseTest
    {
        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void Configuration()
        {
            var app = this.GetTempApp();
            var c = new Configuration(app, this.RandomString());
            Assert.DoesNotThrow(() => c.SetDescription(null));
            Assert.DoesNotThrow(() => c.SetDescription(string.Empty));
            Assert.Throws<AssertionException>(() => c.SetDescription(this.GetString(1000)));
        }
        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void Flag()
        {
            var app = this.GetTempApp();
            var c = new Configuration(app, this.RandomString());
            Assert.IsNull(c.LastTime);
            Assert.IsNotNull(c.Flags);
            Assert.IsEmpty(c.Flags);

            var flag = "flag";
            c.AddFlag(flag);
            //add flag will make LastTime change
            Assert.IsNotNull(c.LastTime);
            Assert.AreEqual(1, c.Flags.Count());
            Assert.Contains(flag, c.Flags.ToList());

            DateTime prev = c.LastTime.Value;
            this.Idle();
            c.RemoveFlag(flag);
            //lasttime change
            Assert.Greater(c.LastTime, prev);
            Assert.IsEmpty(c.Flags);
            Assert.DoesNotThrow(() => c.RemoveFlag(flag));

            //can not duplicate
            c.AddFlag(flag);
            Assert.Throws<AssertionException>(() => c.AddFlag(flag));
        }
        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void AddOrGetProperty()
        {
            var app = this.GetTempApp();
            var c = new Configuration(app, this.RandomString());
            Assert.IsNotNull(c.Properties);
            Assert.IsEmpty(c.Properties);

            var pn = "key";
            Assert.IsNull(c.GetProperty(pn));
            c.AddProperty(pn);
            //can not duplicate
            Assert.Throws<AssertionException>(() => c.AddProperty(pn));
            //find pn
            Assert.AreEqual(1, c.Properties.Count());
            Assert.Contains(pn, c.Properties.ToList());

            Assert.AreEqual(pn, c.GetProperty(pn).Name);
        }
        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void Property()
        {
            var c = new Configuration(this.GetTempApp(), this.RandomString());
            Assert.IsNull(c.LastTime);

            var val = "abc";
            var p = c.AddProperty("key");
            //add will make LastTime change
            Assert.IsNotNull(c.LastTime);

            //value
            Assert.IsNullOrEmpty(p.Value);
            DateTime prev;
            p.Value = val;
            //configuration will make change
            Assert.IsNotNull(c.LastTime);
            prev = c.LastTime.Value;
            this.Idle();
            p.Value = val;
            //lasttime change
            Assert.Greater(c.LastTime, prev);

            //description
            Assert.DoesNotThrow(() => p.SetDescription(null));
            Assert.DoesNotThrow(() => p.SetDescription(string.Empty));
            Assert.Throws<AssertionException>(() => p.SetDescription(this.GetString(1000)));

            //set flag value
            var flag = "flag";
            p[flag] = val;
            //only effect uncommitted
            Assert.IsNullOrEmpty(p.Committed()[flag]);
            Assert.AreEqual(val, p.Uncommitted()[flag]);

            //trash
            p.Trash();
            Assert.IsFalse(p.Committed().IsTrashed);
            Assert.IsTrue(p.Uncommitted().IsTrashed);

            //do commit
            Assert.IsNull(p.LastCommitTime);
            p.DoCommit();
            Assert.AreEqual(val, p.Committed()[flag]);
            Assert.IsTrue(p.Committed().IsTrashed);
            Assert.IsNotNull(p.LastCommitTime);
        }
        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void Create()
        {
            var str = "abc";
            var cn = this.RandomString();
            var c = new Configuration(this.CreateApp(), cn);
            c.SetDescription(str);
            c.AddFlag(str);
            c.AddProperty(str);
            this._configService.Create(c);

            this.Evict(c);

            var c2 = this._configService.GetConfiguration(c.ID);
            Assert.AreEqual(c.Name, c2.Name);
            Assert.AreEqual(c.Description, c2.Description);
            Assert.Contains(str, c2.Flags.ToList());
            Assert.Contains(str, c2.Properties.ToList());
            Assert.IsNotNull(c2.LastTime);
        }
        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void NameCanNotDuplicateInApp()
        {
            var app = this.CreateApp();
            var cn = this.RandomString();
            this.AssertParallel(() => this._configService.Create(new Configuration(app, cn)), 4, 1);
        }
        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void Update()
        {
            var c = new Configuration(this.CreateApp(), this.RandomString());
            this._configService.Create(c);
            var str = "abc";
            c.SetDescription(str);
            c.AddFlag(str);
            c.AddProperty(str)[str] = str;
            c.GetProperty(str).Trash();
            c.GetProperty(str).DoCommit();
            this._configService.Update(c);

            this.Evict(c);

            var c2 = this._configService.GetConfiguration(c.ID);
            Assert.IsNotNull(c2.LastTime);
            Assert.Contains(str, c2.Flags.ToList());
            Assert.Contains(str, c2.Properties.ToList());
            Assert.IsNotNull(c2.GetProperty(str).LastCommitTime);
            Assert.IsTrue(c2.GetProperty(str).Committed().IsTrashed);
            Assert.IsTrue(c2.GetProperty(str).Uncommitted().IsTrashed);
            Assert.AreEqual(str, c2.GetProperty(str).Committed()[str]);
            Assert.AreEqual(str, c2.GetProperty(str).Uncommitted()[str]);
        }
        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void Get()
        {
            var cn = this.RandomString();
            var app = this.CreateApp();
            this._configService.Create(new Configuration(app, cn));
            this._configService.Create(new Configuration(app, cn + 1));
            this._configService.Create(new Configuration(app, cn + 2));
            Assert.IsNotNull(this._configService.GetConfiguration(app, cn));
            Assert.AreEqual(3, this._configService.GetConfigurations(app).Count());
        }

        private Application GetTempApp()
        {
            return new Application(new Account(this.RandomString()));
        }
    }
}
