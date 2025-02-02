// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MTD.Binary;
using PG.StarWarsGame.Files.MTD.Services;

namespace PG.StarWarsGame.Files.MTD;

/// <summary>
/// Provides initialization routines for this library.
/// </summary>
public static class MtdServiceContribution 
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Adds all necessary services provided by this library to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add services to.</param>
    public static void SupportMTD(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IMtdFileService>(sp => new MtdFileService(sp));
        serviceCollection.AddSingleton<IMtdBinaryConverter>(sp => new MtdBinaryConverter(sp));
        serviceCollection.AddSingleton<IMtdFileReader>(sp => new MdtFileReader(sp));
    }
}