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
using System.Text;
using CodeSharp.Core.DomainBase;

namespace Properties.Model
{
    /// <summary>configuration owner
    /// </summary>
    public class App : EntityBase<Guid>, IAggregateRoot
    {
        public string Name { get; private set; }
        public DateTime CreateTime { get; private set; }
        /// <summary>authentication to access api
        /// </summary>
        public string Token { get; private set; }
        public Guid CreatorAccountId { get; private set; }

        protected App()
        {
            this.CreateTime = DateTime.UtcNow;
            this.Token = Guid.NewGuid().ToString();
        }
        public App(Account account)
            : this()
        {
            Assert.IsNotNull(account);
            this.CreatorAccountId = account.ID;
            this.SetName(Guid.NewGuid().ToString());
        }

        public void SetName(string name)
        {
            Assert.IsValidKey(name);
            this.Name = name;
        }
    }
}