// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PG.Commons;
using PG.StarWarsGame.Files.DAT.Binary;
using PG.StarWarsGame.Files.DAT.Services;

namespace PG.StarWarsGame.Files.DAT;

/// <summary>
/// Provides initialization routines for this library.
/// </summary>
public static class DatServiceContribution
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Adds all necessary services provided by this library to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add services to.</param>
    public static void SupportDAT(this IServiceCollection serviceCollection)
    {
        PetroglyphCommons.ContributeServices(serviceCollection);
        serviceCollection.TryAddSingleton<IHashingService>(sp => new HashingService(sp));

        serviceCollection
            .AddSingleton<IDatFileService>(sp => new DatFileService(sp))
            .AddSingleton<IDatModelService>(sp => new DatModelService(sp))
            .AddTransient<IDatFileReader>(sp => new DatFileReader(sp))
            .AddTransient<IDatBinaryConverter>(sp => new DatBinaryConverter(sp));
    }
}
