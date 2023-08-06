// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Services;

namespace PG.Testing;

[TestClass]
[TestCategory(TestConstants.TestCategories.SERVICE)]
public abstract class ServiceTestBase
{
    private IFileSystem FileSystem { get; set; }

    protected abstract Type GetServiceClass();
    protected virtual IService GetServiceInstance()
    {
        return GetServiceInstance(GetServiceProviderInternal());
    }

    protected virtual IService GetServiceInstance(IServiceProvider services)
    {
        FileSystem = (IFileSystem) services.GetService(typeof(IFileSystem));
        return (IService)Activator.CreateInstance(GetServiceClass(), services);
    }

    protected internal virtual IServiceProvider GetServiceProviderInternal()
    {
        return TestConstants.Services;
    }
        

    [TestMethod]
    public void Test_ServiceBaseSetup__IsValid()
    {
        TestBaseSetup();
    }

    private void TestBaseSetup()
    {
        using var svc = GetServiceInstance();
        Assert.IsTrue(svc.GetType().IsSubclassOf(typeof(ServiceBase)));
        var svc0 = (ServiceBase)svc;
        Assert.IsNotNull(svc0.Logger);
        Assert.IsNotNull(svc0.FileSystem);
        TestBaseSetupInternal(svc);
    }

    protected internal void TestBaseSetupInternal(IService svc)
    {
        // NOP
    }
}