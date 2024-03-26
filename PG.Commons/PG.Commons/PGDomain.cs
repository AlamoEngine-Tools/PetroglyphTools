using System;
using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Hashing;

namespace PG.Commons;

/// <summary>
///     Provides methods to initialize this library.
/// </summary>
[Obsolete(
    $"{nameof(PGDomain)} is deprecated and will be removed in a future version. Use {nameof(PGServiceContribution)} instead.")]
public class PGDomain
{
    /// <summary>
    ///     Adds the required services for this library to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection to populate.</param>
    public static void RegisterServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IHashAlgorithmProvider>(sp => new Crc32HashingProvider());
        serviceCollection.AddSingleton<ICrc32HashingService>(sp => new Crc32HashingService(sp));
    }
}