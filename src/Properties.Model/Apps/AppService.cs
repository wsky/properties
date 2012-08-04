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
using CodeSharp.Core.RepositoryFramework;
using Castle.Services.Transaction;

namespace Properties.Model
{
    public interface IAppService
    {
        void Create(App app);
        App GetApp(Guid id);
        App GetApp(string token);
        IEnumerable<App> GetApps(Account account);
    }
    [Transactional]
    public class AppService : IAppService
    {
        private static IAppRepository _repository;
        static AppService()
        {
            _repository = RepositoryFactory.GetRepository<IAppRepository, Guid, App>();
        }

        #region IAppService Members
        [Transaction(TransactionMode.Requires)]
        void IAppService.Create(App app)
        {
            _repository.Add(app);
        }
        App IAppService.GetApp(Guid id)
        {
            return id != Guid.Empty ? _repository.FindBy(id) : null;
        }
        App IAppService.GetApp(string token)
        {
            return !string.IsNullOrWhiteSpace(token) ? _repository.FindByToken(token) : null;
        }
        IEnumerable<App> IAppService.GetApps(Account account)
        {
            return _repository.FindByCreator(account.ID);
        }
        #endregion
    }
}
