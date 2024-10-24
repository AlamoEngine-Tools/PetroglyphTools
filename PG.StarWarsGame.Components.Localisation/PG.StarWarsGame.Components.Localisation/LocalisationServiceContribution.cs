using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Attributes;
using PG.Commons.Extensibility;
using PG.StarWarsGame.Components.Localisation.IO;
using PG.StarWarsGame.Components.Localisation.IO.Xml;
using PG.StarWarsGame.Components.Localisation.Services;

namespace PG.StarWarsGame.Components.Localisation;

/// <inheritdoc />
[Order(1200)]
public class LocalisationServiceContribution : IServiceContribution
{
    /// <inheritdoc />
    public void ContributeServices(IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddSingleton<IAlamoLanguageSupportService>(sp => new AlamoLanguageSupportService(sp))
            .AddTransient<IImportHandler<XmlInputStrategy>>(sp => new XmlImportHandler(sp));
    }
}
