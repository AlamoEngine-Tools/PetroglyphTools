using Microsoft.Extensions.DependencyInjection;
using PG.Testing;

namespace PG.StarWarsGame.Files.DAT.Test;

public class CommonDatTestBase : CommonTestBase
{
    protected override void SetupServices(ServiceCollection serviceCollection)
    {
        base.SetupServices(serviceCollection);
        serviceCollection.SupportDAT();
    }
}