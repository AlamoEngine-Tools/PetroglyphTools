// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace PG.Core.Test
{
    /// <summary>
    /// Constants that can be re-used in all test projects.
    /// </summary>
    public class TestConstants
    {
        /// <summary>
        /// <see cref="Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute"/> for all public facing API tests.
        /// </summary>
        public const string TEST_TYPE_API = "Public API Test";

        /// <summary>
        /// <see cref="Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute"/> for tests that may never fail.
        /// </summary>
        public const string TEST_TYPE_HOLY = "Holy Test";

        /// <summary>
        /// <see cref="Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute"/> for builder tests.
        /// </summary>
        public const string TEST_TYPE_BUILDER = "Builder Test";

        /// <summary>
        /// <see cref="Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute"/> for all static utilities.
        /// </summary>
        public const string TEST_TYPE_UTILITY = "Utility Test";

        /// <summary>
        /// The FreeBSD platform name.
        /// Currently not supported by Alamo Engine Tools!
        /// </summary>
        [Obsolete("This platform is not supported.", true)]
        public const string PLATFORM_FREEBSD = "FREEBSD";

        /// <summary>
        /// The Linux platform name
        /// </summary>
        public const string PLATFORM_LINUX = "LINUX";

        /// <summary>
        /// The OSX platform name
        /// </summary>
        public const string PLATFORM_OSX = "OSX";

        /// <summary>
        /// The Windows platform name
        /// </summary>
        public const string PLATFORM_WINDOWS = "WINDOWS";

        private static IServiceProvider s_serviceProvider;

        public static IServiceProvider Services => GetServiceProvider();

        private static IServiceProvider GetServiceProvider()
        {
            if (s_serviceProvider != null)
            {
                return s_serviceProvider;
            }

            s_serviceProvider = new ServiceCollection()
                .AddSingleton<IFileSystem, MockFileSystem>()
                .AddSingleton<ILoggerFactory, NullLoggerFactory>().BuildServiceProvider();
            return s_serviceProvider;
        }
    }
}
