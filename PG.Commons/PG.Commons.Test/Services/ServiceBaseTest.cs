using System;
using PG.Commons.Services;
using PG.Testing;
using Xunit;

namespace PG.Commons.Test.Services;

public class ServiceBaseTest : CommonTestBase
{
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
        Assert.Equal(FileSystem, service.FileSystem);
        Assert.NotNull(service.Logger);
    }

    private class MyService(IServiceProvider services) : ServiceBase(services);
}