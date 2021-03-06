﻿//  Copyright 2012 wsky. wskyhx@gmail.com
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
    public class Lock : EntityBase<string>, IAggregateRoot
    {
        protected Lock() { }
        public Lock(string id) { this.ID = id; }

        public static void InitAll(ILockHelper helper)
        {
            helper.Init<Account>();
            helper.Init<Configuration>();
        }
    }
    /// <summary>provide global lock
    /// </summary>
    public interface ILockHelper
    {
        void Init<T>();
        Lock Require<T>();
        Lock Require(Type type);
    }
}