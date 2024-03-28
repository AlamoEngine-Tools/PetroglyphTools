using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Attributes;
using PG.Commons.Extensibility;
using PG.StarWarsGame.Files.DAT.Binary;
using PG.StarWarsGame.Files.DAT.Services;

namespace PG.StarWarsGame.Files.DAT;

/// <inheritdoc />
[Order(1100)]
internal sealed class DatServiceContribution : IServiceContribution
{
    /// <inheritdoc />
    public void ContributeServices(IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddSingleton<IDatFileService>(sp => new DatFileService(sp))
            .AddSingleton<IDatBinaryServiceFactory>(sp => new DatBinaryServiceFactory(sp))
            .AddTransient<IDatFileReader>(sp => new DatFileReader(sp))
            .AddTransient<IDatBinaryConverter>(sp => new DatBinaryConverter(sp))
            .AddTransient<IDatFileWriter>(sp => new DatFileWriter(sp));
    }
}