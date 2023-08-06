// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace PG.Testing;

/// <summary>
/// Constants that can be re-used in all test projects.
/// </summary>
public class TestConstants
{
    /// <summary>
    /// The Linux platform name
    /// </summary>
    public const string PLATFORM_LINUX = "LINUX";
    /// <summary>
    /// The Windows platform name
    /// </summary>
    public const string PLATFORM_WINDOWS = "WINDOWS";

    public struct TestCategories
    {
        public const string HOLY = "Holy Test";
        public const string SERVICE = "Service Test";
        public const string UTILITY = "Utility Test";
    }

    private static IServiceProvider s_serviceProvider;

    public static IServiceProvider Services => GetServiceProvider();

    private static IServiceProvider GetServiceProvider()
    {
        if (s_serviceProvider != null)
        {
            return s_serviceProvider;
        }

        var collection = new ServiceCollection();
        RegisterServicesInternal(collection);
        
        s_serviceProvider = collection
            .BuildServiceProvider();
        return s_serviceProvider;
    }

    private static void RegisterServicesInternal(ServiceCollection collection)
    {
        collection
            .AddSingleton<IFileSystem, MockFileSystem>()
            .AddSingleton<ILoggerFactory, NullLoggerFactory>();
    }
}