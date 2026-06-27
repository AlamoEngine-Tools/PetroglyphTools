using AnakinRaW.CommonUtilities.Hashing;
using AnakinRaW.CommonUtilities.Testing;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons;

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
        serviceCollection.AddSingleton<IHashingService>(sp => new HashingService(sp));
        PetroglyphCommons.ContributeServices(serviceCollection);
    }
}