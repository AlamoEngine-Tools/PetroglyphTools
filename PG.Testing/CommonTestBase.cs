using System;
using System.IO.Abstractions;
using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons;
using Testably.Abstractions.Testing;

namespace PG.Testing;

public abstract class CommonTestBase
{
    protected readonly MockFileSystem FileSystem = new();
    protected readonly IServiceProvider ServiceProvider;

    protected CommonTestBase()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IHashingService>(sp => new HashingService(sp));
        sc.AddSingleton<IFileSystem>(FileSystem);
        PetroglyphCommons.ContributeServices(sc);
        SetupServices(sc);
        ServiceProvider = sc.BuildServiceProvider();
    }

    protected virtual void SetupServices(ServiceCollection serviceCollection)
    {
    }
}