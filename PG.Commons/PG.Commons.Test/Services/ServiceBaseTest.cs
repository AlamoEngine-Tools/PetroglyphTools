using System;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Services;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.Commons.Test.Services;

public class ServiceBaseTest
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MockFileSystem _fileSystem = new();

    public ServiceBaseTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        _serviceProvider = sc.BuildServiceProvider();
    }

    [Fact]
    public void Test_Ctor_ThrowsNullArg()
    {
        Assert.Throws<ArgumentNullException>(() => new MyService(null!));
    }

    [Fact]
    public void Test_Ctor_SetupProperties()
    {
       var service = new MyService(_serviceProvider);
        Assert.Equal(_serviceProvider, service.Services);
        Assert.Equal(_fileSystem, service.FileSystem);
        Assert.NotNull(service.Logger);
    }

    private class MyService(IServiceProvider services) : ServiceBase(services);
}