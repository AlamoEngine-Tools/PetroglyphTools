using AET.Testing;
using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons;

namespace PG.Testing;

public abstract class PGTestBase : TestBaseWithFileSystem
{
    protected PGTestBase()
    {
    }

    protected override void SetupServices(IServiceCollection serviceCollection)
    {
        base.SetupServices(serviceCollection);
        serviceCollection.AddSingleton<IHashingService>(sp => new HashingService(sp));
        PetroglyphCommons.ContributeServices(serviceCollection);
    }
}