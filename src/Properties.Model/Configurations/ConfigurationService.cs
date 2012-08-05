using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeSharp.Core.RepositoryFramework;
using Castle.Services.Transaction;

namespace Properties.Model
{
    public interface IConfigurationService
    {
        void Create(Configuration config);
        void Update(Configuration config);
        Configuration GetConfiguration(Guid id);
        Configuration GetConfiguration(Application app, string name);
        IEnumerable<Configuration> GetConfigurations(Application app);
    }
    [Transactional]
    public class ConfigurationService : IConfigurationService
    {
        private static IConfigurationRepository _repository;
        static ConfigurationService()
        {
            _repository = RepositoryFactory.GetRepository<IConfigurationRepository, Guid, Configuration>();
        }
        private ILockHelper _locker;
        public ConfigurationService(ILockHelper locker)
        {
            this._locker = locker;
        }

        #region IConfigurationService Members
        [Transaction(TransactionMode.Requires)]
        void IConfigurationService.Create(Configuration config)
        {
            this._locker.Require<Configuration>();//heavy lock
            Assert.IsNull(_repository.FindBy(config.AppId, config.Name));
            _repository.Add(config);
        }
        [Transaction(TransactionMode.Requires)]
        void IConfigurationService.Update(Configuration config)
        {
            _repository.Update(config);
        }
        Configuration IConfigurationService.GetConfiguration(Guid id)
        {
            return _repository.FindBy(id);
        }
        Configuration IConfigurationService.GetConfiguration(Application app, string name)
        {
            return _repository.FindBy(app.ID, name);
        }
        IEnumerable<Configuration> IConfigurationService.GetConfigurations(Application app)
        {
            return _repository.FindByApp(app.ID);
        }
        #endregion
    }
}