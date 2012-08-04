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
using Castle.Services.Transaction;
using CodeSharp.Core;
using CodeSharp.Core.Castles;
using NHibernate;
using NHibernate.Criterion;
using Properties.Model;

namespace Properties.Repositories
{
    /// <summary>simple db lock
    /// </summary>
    [Transactional]
    public class LockRepository : NHibernateRepositoryBase<string, Lock>, ILockHelper
    {
        [Transaction(TransactionMode.Requires)]
        public void Init<T>()
        {
            if (this.Require<T>() == null)
                this.Add(new Lock(typeof(T).Name));
        }

        public Lock Require<T>()
        {
            return this.Require(typeof(T));
        }

        public Lock Require(Type type)
        {
            return this.GetSession()
                .CreateCriteria<Lock>()
                .SetLockMode(LockMode.Upgrade)
                .Add(Expression.IdEq(type.Name))
                .UniqueResult<Lock>();
        }
    }
}