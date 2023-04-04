// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Core.Services;
using System;

namespace PG.Core.Test.Services
{
    [TestClass]
    public abstract class AbstractServiceTest<TService> where TService : AbstractService<TService>
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
            using TService svc = GetServiceInstance();
            Assert.IsNotNull(svc.Logger);
            Assert.IsNotNull(svc.FileSystem);
            TestBaseSetupInternal(svc);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Extention method. Can be overwritten to accomodate more complex service setups.")]
        protected internal void TestBaseSetupInternal(TService svc)
        {
            // NOP
        }
    }
}
