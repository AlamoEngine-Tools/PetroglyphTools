using Microsoft.Extensions.DependencyInjection;
using PG.Testing;

namespace PG.StarWarsGame.Files.MTD.Test;

public class CommonMtdTestBase : CommonTestBase
{
    protected override void SetupServices(ServiceCollection serviceCollection)
    {
        base.SetupServices(serviceCollection);
        serviceCollection.SupportMTD();
    }
}