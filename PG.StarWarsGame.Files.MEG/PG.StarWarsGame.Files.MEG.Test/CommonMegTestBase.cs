using Microsoft.Extensions.DependencyInjection;
using PG.Testing;

namespace PG.StarWarsGame.Files.MEG.Test;

public abstract class CommonMegTestBase : CommonTestBase
{
    protected override void SetupServices(ServiceCollection serviceCollection)
    {
        base.SetupServices(serviceCollection);
        serviceCollection.SupportMEG();
    }
}