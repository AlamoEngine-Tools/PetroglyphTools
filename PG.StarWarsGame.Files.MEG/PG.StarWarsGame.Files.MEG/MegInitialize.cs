using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Binary;

namespace PG.StarWarsGame.Files.MEG;

/// <summary>
/// Provides methods to initialize this library.
/// </summary>
public static class MegInitialize
{
    /// <summary>
    /// Adds the requires services for this library to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection to populate.</param>
    public static void InitializeMegServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IMegBinaryServiceFactory>(sp => new MegBinaryServiceFactory(sp));
    }
}