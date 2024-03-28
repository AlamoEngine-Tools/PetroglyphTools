using Microsoft.Extensions.DependencyInjection;

namespace PG.Commons.Extensibility;

/// <summary>
///     Contract for a class or initialisation helper that contributes services to the <see cref="IServiceCollection" /> of
///     a given application. Must always have a parameterless .ctor.
/// </summary>
public interface IServiceContribution
{
    /// <summary>
    ///     Hook for injecting services into the main application's <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">The main application's <see cref="IServiceCollection" /></param>
    void ContributeServices(IServiceCollection serviceCollection);
}