using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GWLoader;

namespace GWLoader.Domain
{
    internal sealed class DomainManager : AppDomainManager, INetDomain
    {
        public DomainManager() { }
        public override void InitializeNewDomain(AppDomainSetup appDomainInfo) { InitializationFlags = AppDomainManagerInitializationOptions.RegisterWithHost; }

        public void Initialize() => GWLoader.Load();
        public void OnApplicationStart() => GWLoader._self.Start();
    }
}
