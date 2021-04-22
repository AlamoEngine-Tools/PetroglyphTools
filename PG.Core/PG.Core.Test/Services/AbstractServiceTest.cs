// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Core.Services;

namespace PG.Core.Test.Services
{
    [TestClass]
    public abstract class AbstractServiceTest<TService> where TService : AbstractService<TService>
    {
        protected virtual TService GetServiceInstance()
        {
            return GetServiceInstance(GetFileSystemInternal());
        }

        protected abstract TService GetServiceInstance(IFileSystem fileSystem);

        protected internal virtual IFileSystem GetFileSystemInternal()
        {
            return new MockFileSystem();
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

        protected internal void TestBaseSetupInternal(TService svc)
        {
            // NOP
        }
    }
}
