using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Attributes;
using PG.Commons.Extensibility;

namespace PG.StarWarsGame.Components.Localisation;

/// <inheritdoc />
[Order(1200)]
public class LocalisationServiceContribution : IServiceContribution
{
    /// <inheritdoc />
    public void ContributeServices(IServiceCollection serviceCollection)
    {
        // NOP
    }
}