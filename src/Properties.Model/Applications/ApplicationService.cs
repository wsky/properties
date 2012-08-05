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
    public interface IApplicationService
    {
        void Create(Application app);
        Application GetApp(Guid id);
        Application GetApp(string token);
        IEnumerable<Application> GetApps(Account account);
    }
    [Transactional]
    public class ApplicationService : IApplicationService
    {
        private static IApplicationRepository _repository;
        static ApplicationService()
        {
            _repository = RepositoryFactory.GetRepository<IApplicationRepository, Guid, Application>();
        }

        #region IAppService Members
        [Transaction(TransactionMode.Requires)]
        void IApplicationService.Create(Application app)
        {
            _repository.Add(app);
        }
        Application IApplicationService.GetApp(Guid id)
        {
            return id != Guid.Empty ? _repository.FindBy(id) : null;
        }
        Application IApplicationService.GetApp(string token)
        {
            return !string.IsNullOrWhiteSpace(token) ? _repository.FindByToken(token) : null;
        }
        IEnumerable<Application> IApplicationService.GetApps(Account account)
        {
            return _repository.FindByCreator(account.ID);
        }
        #endregion
    }
}
