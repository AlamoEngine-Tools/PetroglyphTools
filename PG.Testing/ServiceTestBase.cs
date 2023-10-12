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
    private IFileSystem FileSystem { get; set; } = null!;

    protected abstract Type GetServiceClass();

    protected virtual T GetServiceInstance<T>() where T : ServiceBase, IService
    {
        return GetServiceInstance<T>(GetServiceProviderInternal());
    }

    protected virtual T GetServiceInstance<T>(IServiceProvider services) where T : ServiceBase, IService
    {
        FileSystem = (IFileSystem)(services.GetService(typeof(IFileSystem)) ?? throw new InvalidOperationException());
        return Activator.CreateInstance(GetServiceClass(), services) as T ?? throw new InvalidOperationException();
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
        var svc = GetServiceInstance<ServiceBase>();
        Assert.IsTrue(svc.GetType().IsSubclassOf(typeof(ServiceBase)));
        Assert.IsNotNull(svc);
        Assert.IsNotNull(svc.Logger);
        Assert.IsNotNull(svc.FileSystem);
        TestBaseSetupInternal(svc);
    }

    protected internal void TestBaseSetupInternal(IService svc)
    {
        // NOP
    }
}