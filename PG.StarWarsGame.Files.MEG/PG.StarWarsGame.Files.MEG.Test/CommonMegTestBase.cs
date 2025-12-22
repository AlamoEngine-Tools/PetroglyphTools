using Microsoft.Extensions.DependencyInjection;
using PG.Testing;

namespace PG.StarWarsGame.Files.MEG.Test;

public abstract class CommonMegTestBase : PGTestBase
{
    protected override void SetupServices(IServiceCollection serviceCollection)
    {
        base.SetupServices(serviceCollection);
        serviceCollection.SupportMEG();
    }
}