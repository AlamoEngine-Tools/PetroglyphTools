// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Components.Localisation.Commons.Helper;
using PG.StarWarsGame.Files.DAT;

namespace PG.StarWarsGame.Components.Localisation;

/// <summary>
///     Provides methods to initialize this library.
/// </summary>
public static class LocalisationDomain
{
    /// <summary>
    ///     Adds the requires services for this library to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection to populate.</param>
    public static void RegisterServices(IServiceCollection serviceCollection)
    {
        DatDomain.RegisterServices(serviceCollection);
        serviceCollection
            .AddSingleton<IAlamoLanguageDefinitionHelper>(sp => new AlamoLanguageDefinitionHelper(sp))
            ;
    }
}