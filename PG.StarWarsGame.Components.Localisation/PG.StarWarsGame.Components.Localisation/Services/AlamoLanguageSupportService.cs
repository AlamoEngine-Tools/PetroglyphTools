// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Linq;
using System.Reflection;
using PG.Commons.Services;
using PG.StarWarsGame.Components.Localisation.Attributes;
using PG.StarWarsGame.Components.Localisation.Exceptions;
using PG.StarWarsGame.Components.Localisation.Languages;

namespace PG.StarWarsGame.Components.Localisation.Services;

/// <inheritdoc cref="PG.StarWarsGame.Components.Localisation.Services.IAlamoLanguageSupportService" />
public class AlamoLanguageSupportService : ServiceBase, IAlamoLanguageSupportService
{
    private IAlamoLanguageDefinition _fallbackAlamoLanguageDefinition;

    /// <inheritdoc />
    public AlamoLanguageSupportService(IServiceProvider services) : base(services)
    {
        _fallbackAlamoLanguageDefinition = GetDefaultLanguageDefinition();
    }

    /// <inheritdoc />
    public bool IsBuiltInLanguageDefinition(IAlamoLanguageDefinition definition)
    {
        return definition.GetType().GetCustomAttribute(typeof(OfficiallySupportedLanguageAttribute)) != null;
    }

    /// <inheritdoc />
    public IAlamoLanguageDefinition GetFallbackLanguageDefinition()
    {
        return _fallbackAlamoLanguageDefinition;
    }

    /// <inheritdoc />
    public IAlamoLanguageSupportService WithOverrideFallbackLanguage(IAlamoLanguageDefinition definition)
    {
        _fallbackAlamoLanguageDefinition = definition;
        return this;
    }

    /// <inheritdoc />
    public IAlamoLanguageDefinition GetDefaultLanguageDefinition()
    {
        var languageDefinitions = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assemblyTypes => assemblyTypes.GetTypes())
            .Where(assemblyType => typeof(IAlamoLanguageDefinition).IsAssignableFrom(assemblyType) &&
                                   assemblyType is { IsClass: true, IsAbstract: false })
            .Where(t => t.GetCustomAttribute(typeof(DefaultLanguageAttribute)) != null)
            .ToList();
        if (languageDefinitions.Count != 1)
            throw new InvalidDefaultLanguageDefinitionException(languageDefinitions.Count > 1
                ? "Multiple default languages have been defined."
                : "No default language has been defined.");
        return (IAlamoLanguageDefinition)Activator.CreateInstance(languageDefinitions[0]);
    }
}
