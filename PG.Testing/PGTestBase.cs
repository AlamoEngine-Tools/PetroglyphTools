using AnakinRaW.CommonUtilities.Hashing;
using AnakinRaW.CommonUtilities.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PG.Commons;
using PG.Commons.Hashing;

namespace PG.Testing;

/// <summary>
/// Provides a base class for tests that require the Petroglyph commons services and an in-memory file system.
/// </summary>
public abstract class PGTestBase : TestBaseWithFileSystem
{
    /// <inheritdoc/>
    protected override void SetupServices(IServiceCollection serviceCollection)
    {
        base.SetupServices(serviceCollection);
        // Consumers have to provide the hashing algorithm themselves; the libraries do not register one.
        serviceCollection.TryAddSingleton<IHashAlgorithmProvider>(new Crc32HashingProvider());
        serviceCollection.TryAddSingleton<IHashingService>(sp => new HashingService(sp));
        serviceCollection.TryAddSingleton<ICrc32HashingService>(sp => new Crc32HashingService(sp));
        PetroglyphCommons.ContributeServices(serviceCollection);
    }
}