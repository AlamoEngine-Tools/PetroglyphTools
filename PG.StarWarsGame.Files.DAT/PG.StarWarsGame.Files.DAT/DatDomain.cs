// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.DAT.Binary;
using PG.StarWarsGame.Files.DAT.Services;

namespace PG.StarWarsGame.Files.DAT;

/// <summary>
/// Provides methods to initialize this library.
/// </summary>
public static class DatDomain
{
    /// <summary>
    /// Adds the required services for this library to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection to populate.</param>
    public static void RegisterServices(IServiceCollection serviceCollection)
    {
        serviceCollection
            //.AddSingleton<IDatModelService>(sp => new DatModelService(sp))
            .AddSingleton<IDatFileService>(sp => new DatFileService(sp))
            .AddSingleton<IDatFileReader>(sp => new DatFileReader(sp))
            .AddSingleton<IDatBinaryConverter>(sp => new DatBinaryConverter(sp));
    }
}