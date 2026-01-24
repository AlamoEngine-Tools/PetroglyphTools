using Microsoft.Extensions.DependencyInjection;
using PG.Testing;

namespace PG.StarWarsGame.Files.MTD.Test;

public class CommonMtdTestBase : PGTestBase
{
    protected override void SetupServices(IServiceCollection serviceCollection)
    {
        base.SetupServices(serviceCollection);
        serviceCollection.SupportMTD();
    }
}