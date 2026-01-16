using System;
using System.IO.Abstractions;
using AnakinRaW.CommonUtilities.Testing;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Services;
using Testably.Abstractions;
using Xunit;

namespace PG.Commons.Test.Services;

public class ServiceBaseTest : TestBaseWithServiceProvider
{
    private readonly IFileSystem _fileSystem = new RealFileSystem();
    
    protected override void SetupServices(IServiceCollection serviceCollection)
    {
        base.SetupServices(serviceCollection);
        serviceCollection.AddSingleton(_fileSystem);

    }

    [Fact]
    public void Ctor_ThrowsNullArg()
    {
        Assert.Throws<ArgumentNullException>(() => new MyService(null!));
    }

    [Fact]
    public void Ctor_SetupProperties()
    {
       var service = new MyService(ServiceProvider);
        Assert.Equal(ServiceProvider, service.Services);
        Assert.Equal(_fileSystem, service.FileSystem);
        Assert.NotNull(service.Logger);
    }

    private class MyService(IServiceProvider services) : ServiceBase(services);
}