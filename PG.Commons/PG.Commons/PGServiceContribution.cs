using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Attributes;
using PG.Commons.Extensibility;
using PG.Commons.Hashing;

namespace PG.Commons;

/// <inheritdoc />
[Order(100)]
// ReSharper disable once InconsistentNaming
internal sealed class PGServiceContribution : IServiceContribution
{
    /// <inheritdoc />
    public void ContributeServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IHashAlgorithmProvider>(sp => new Crc32HashingProvider());
        serviceCollection.AddSingleton<ICrc32HashingService>(sp => new Crc32HashingService(sp));
    }
}