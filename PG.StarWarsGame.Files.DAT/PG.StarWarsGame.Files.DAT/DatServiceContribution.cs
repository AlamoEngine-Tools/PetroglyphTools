// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.DAT.Binary;
using PG.StarWarsGame.Files.DAT.Services;
using PG.StarWarsGame.Files.DAT.Services.Builder.Validation;
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
        serviceCollection
            .AddSingleton<IDatFileService>(sp => new DatFileService(sp))
            .AddSingleton<IDatModelService>(sp => new DatModelService(sp))
            .AddTransient<IDatFileReader>(sp => new DatFileReader(sp))
            .AddTransient<IDatBinaryConverter>(sp => new DatBinaryConverter(sp))
            .AddSingleton(_ => new EmpireAtWarKeyValidator());
    }
}