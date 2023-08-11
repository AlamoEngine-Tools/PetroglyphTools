// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using PG.Commons.Attributes;
using PG.Commons.Services;
using PG.StarWarsGame.Localisation.Attributes;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Commons.Helper;

/// <inheritdoc cref="IAlamoLanguageDefinitionHelper" />
internal class AlamoLanguageDefinitionHelper : ServiceBase, IAlamoLanguageDefinitionHelper
{
    private IImmutableDictionary<Type, IAlamoLanguageDefinition> LanguageDefinitionCache { get; }

    /// <inheritdoc cref="ServiceBase" />
    public AlamoLanguageDefinitionHelper(IServiceProvider services) : base(services)
    {
        var d = new Dictionary<Type, IAlamoLanguageDefinition>();
        InitCache(d);
        LanguageDefinitionCache = d.ToImmutableDictionary();
    }

    private void InitCache(IDictionary<Type, IAlamoLanguageDefinition> definitions)
    {
        var registeredLanguages = GetRegisteredTypes();
        foreach (var language in registeredLanguages)
        {
            if (Activator.CreateInstance(language) is IAlamoLanguageDefinition instance)
            {
                definitions.Add(language, instance);
            }
        }
    }

    private static List<Type> GetRegisteredTypes()
    {
        return AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
            .Where(p =>
                typeof(IAlamoLanguageDefinition).IsAssignableFrom(p) &&
                typeof(AlamoLanguageDefinitionBase).IsAssignableFrom(p) &&
                p.IsClass &&
                !p.IsAbstract &&
                !p.IsInterface
            ).ToList();
    }

    /// <inheritdoc />
    public IReadOnlyCollection<IAlamoLanguageDefinition> GetAllRegisteredAlamoLanguageDefinitions()
    {
        return LanguageDefinitionCache.Values.ToImmutableList();
    }

    /// <inheritdoc />
    public IAlamoLanguageDefinition GetDefaultAlamoLanguageDefinition()
    {
        foreach (var definition in GetAllRegisteredAlamoLanguageDefinitions())
        {
            if (definition.GetType()
                .GetAttributeValueOrDefault<DefaultLanguageAttribute, bool>(a => a.IsDefaultLanguage))
            {
                return definition;
            }
        }

        throw new InvalidOperationException(
            $"No default {nameof(IAlamoLanguageDefinition)} is defined. This should not happen.");
    }

    /// <inheritdoc />
    public bool IsOfficiallySupported(IAlamoLanguageDefinition languageDefinition)
    {
        return languageDefinition.GetType()
            .GetAttributeValueOrDefault<OfficiallySupportedLanguageAttribute, bool>(a => a.IsOfficiallySupported);
    }
}