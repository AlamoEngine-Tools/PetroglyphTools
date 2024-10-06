// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MTD.Binary;
using PG.StarWarsGame.Files.MTD.Services;

namespace PG.StarWarsGame.Files.MTD;

/// <summary>
/// Provides helper methods to initialize this library.
/// </summary>
public static class MtdServiceContribution 
{
    /// <summary>
    /// Registers all required services of this library to the specified service collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection to register MTD services to.</param>
    public static void AddMtdServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IMtdFileService>(sp => new MtdFileService(sp));
        serviceCollection.AddSingleton<IMtdBinaryConverter>(sp => new MtdBinaryConverter(sp));
        serviceCollection.AddSingleton<IMtdFileReader>(sp => new MdtFileReader(sp));
    }
}