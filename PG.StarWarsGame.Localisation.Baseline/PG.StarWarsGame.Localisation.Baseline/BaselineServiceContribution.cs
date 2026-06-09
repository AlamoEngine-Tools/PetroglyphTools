// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PG.StarWarsGame.Files.DAT;
using Testably.Abstractions;

namespace PG.StarWarsGame.Localisation.Baseline
{
    /// <summary>
    /// Provides initialization routines for the PG.StarWarsGame.Localisation.Baseline library.
    /// </summary>
    public static class BaselineServiceContribution
    {
        /// <summary>
        /// Adds all services required by this library, including DAT and localisation core services.
        /// </summary>
        public static IServiceCollection SupportLocalisationBaseline(this IServiceCollection serviceCollection)
        {
            // Register the real file system if none has been provided (allows test overrides).
            serviceCollection.TryAddSingleton<System.IO.Abstractions.IFileSystem>(new RealFileSystem());

            serviceCollection.SupportDAT();
            serviceCollection.SupportLocalisation();

            serviceCollection.AddSingleton<IBaselineTranslationProvider>(sp =>
                new BaselineTranslationProvider(sp));

            return serviceCollection;
        }
    }
}
