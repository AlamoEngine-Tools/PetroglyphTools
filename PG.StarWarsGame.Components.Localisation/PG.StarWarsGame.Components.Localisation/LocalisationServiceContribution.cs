// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Attributes;
using PG.Commons.Extensibility;
using PG.StarWarsGame.Components.Localisation.Commons.Helper;

namespace PG.StarWarsGame.Components.Localisation;

/// <summary>
///     Provides methods to initialize this library.
/// </summary>
[Order(10000)]
public class LocalisationServiceContribution : IServiceContribution
{
    /// <inheritdoc />
    public void ContributeServices(IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddSingleton<IAlamoLanguageDefinitionHelper>(sp => new AlamoLanguageDefinitionHelper(sp))
            ;
    }
}