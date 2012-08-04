using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeSharp.Core.RepositoryFramework;

namespace Properties.Model
{
    public interface IConfigurationRepository : IRepository<Guid, Configuration>
    {
        Configuration FindBy(Guid appId, string name);
        IEnumerable<Configuration> FindByApp(Guid appId);
    }
}