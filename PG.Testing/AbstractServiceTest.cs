// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Services;

namespace PG.Testing;

[TestClass]
public abstract class AbstractServiceTest<TService> where TService : AbstractService
{
    protected virtual TService GetServiceInstance()
    {
        return GetServiceInstance(GetServiceProviderInternal());
    }

    protected virtual TService GetServiceInstance(IServiceProvider services)
    {
        return Activator.CreateInstance(typeof(TService), services) as TService;
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
        Assert.IsNotNull(svc.Logger);
        Assert.IsNotNull(svc.FileSystem);
        TestBaseSetupInternal(svc);
    }

    protected internal void TestBaseSetupInternal(TService svc)
    {
        // NOP
    }
}