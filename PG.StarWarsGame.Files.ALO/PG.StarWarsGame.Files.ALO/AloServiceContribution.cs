using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.ALO.Binary;
using PG.StarWarsGame.Files.ALO.Binary.Identifier;
using PG.StarWarsGame.Files.ALO.Services;

namespace PG.StarWarsGame.Files.ALO;

public static class AloServiceContribution
{
    public static void ContributeServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IAloFileService>(sp => new AloFileService(sp));
        serviceCollection.AddSingleton<IAloFileReaderFactory>(sp => new AloFileReaderFactory(sp));
        serviceCollection.AddSingleton<IAloContentInfoIdentifier>(sp => new AloContentInfoIdentifier());
    }
}