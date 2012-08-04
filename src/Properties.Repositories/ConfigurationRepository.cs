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
using NHibernate.Criterion;
using NHibernate;
using CodeSharp.Core.Castles;
using Properties.Model;

namespace Properties.Repositories
{
    public class ConfigurationRepository : NHibernateRepositoryBase<Guid, Configuration>, IConfigurationRepository
    {
        public Configuration FindBy(Guid appId, string name)
        {
            return this.FindOne(Expression.Eq("AppId", appId), Expression.Eq("Name", name));
        }

        public IEnumerable<Configuration> FindByApp(Guid appId)
        {
            return this.FindAll(Expression.Eq("AppId", appId));
        }
    }
}