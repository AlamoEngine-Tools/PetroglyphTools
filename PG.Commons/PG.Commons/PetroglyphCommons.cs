// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Hashing;

namespace PG.Commons;

/// <summary>
/// Provides initialization routines for this library.
/// </summary>
public static class PetroglyphCommons 
{
    /// <summary>
    /// Adds all necessary services provided by this library to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add services to.</param>
    public static void ContributeServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IHashAlgorithmProvider>(new Crc32HashingProvider());
        serviceCollection.AddSingleton<ICrc32HashingService>(sp => new Crc32HashingService(sp));
    }
}