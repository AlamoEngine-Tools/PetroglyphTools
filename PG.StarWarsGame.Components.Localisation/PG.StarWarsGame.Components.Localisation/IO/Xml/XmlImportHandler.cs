// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PG.Commons;
using PG.Commons.Data.Serialization;
using PG.Commons.Services;
using PG.StarWarsGame.Components.Localisation.IO.Xml.Serializable.v1;
using PG.StarWarsGame.Components.Localisation.Languages;
using PG.StarWarsGame.Components.Localisation.Repository;
using PG.StarWarsGame.Components.Localisation.Repository.Content;
using PG.StarWarsGame.Components.Localisation.Services;
using PG.StarWarsGame.Files.DAT.Services.Builder.Validation;

namespace PG.StarWarsGame.Components.Localisation.IO.Xml;

/// <summary>
///     The Import handler for the <see cref="XmlInputStrategy" />
/// </summary>
public class XmlImportHandler : ServiceBase, IImportHandler<XmlInputStrategy>
{
    /// <inheritdoc />
    public XmlImportHandler(IServiceProvider services) : base(services)
    {
    }

    /// <inheritdoc />
    public void Import(XmlInputStrategy strategy, ITranslationRepository targetRepository)
    {
        var data = Services.GetService<IXmlSerializationSupportService>()
            ?.DeSerializeObjectFromDisc<LocalisationData>(strategy.FilePath);
        if (data == null) return;

        FromXml(data, targetRepository, strategy.Validation);
    }

    /// <summary>
    ///     Translates a
    /// </summary>
    /// <param name="data"></param>
    /// <param name="targetRepository"></param>
    /// <param name="validationLevel"></param>
    /// <exception cref="LibraryInitialisationException"></exception>
    protected void FromXml(LocalisationData data, ITranslationRepository targetRepository,
        IInputStrategy.ValidationLevel validationLevel)
    {
        var languageMap =
            Services.GetService<IAlamoLanguageSupportService>()?.CreateLanguageIdentifierMapping() ??
            throw new LibraryInitialisationException(
                $"Required service {nameof(IAlamoLanguageSupportService)} has not bee initialised properly.");

        var contentMap =
            languageMap.Values
                .ToDictionary<IAlamoLanguageDefinition, IAlamoLanguageDefinition, ISet<ITranslationItem>>(d => d,
                    d => new HashSet<ITranslationItem>());

        foreach (var loc in data.LocalisationHolder)
        {
            var id = OrderedTranslationItemId.Of(loc.Key);
            if (id == null || !Services.GetService<IDatKeyValidator>()!.Validate(loc.Key))
            {
                if (validationLevel != IInputStrategy.ValidationLevel.Lenient)
                    throw new InvalidDataException(
                        $"Invalid data found: Could not create a valid {nameof(OrderedTranslationItemId)} from {loc.Key}");

                Logger.LogWarning("Could not create a valid {} from {}. Skipping.",
                    nameof(OrderedTranslationItemId),
                    loc.Key);
                continue;
            }


            foreach (var t in loc.TranslationData.TranslationHolder)
            {
                if (t == null)
                {
                    Logger.LogWarning("Empty translation for key {} found! Skipping.", loc.Key);
                    continue;
                }

                if (!languageMap.TryGetValue(t.Language, out var lang))
                {
                    if (validationLevel != IInputStrategy.ValidationLevel.Lenient)
                        throw new InvalidDataException(
                            $"Invalid data found: Could not determine the {nameof(IAlamoLanguageDefinition)} for {t.Language}");

                    Logger.LogWarning("Could not determine the language for {}. Skipping.", t.Language);
                    continue;
                }

                var c = new TranslationItemContent
                {
                    Key = id.Unwrap(),
                    Value = t.Text
                };
                contentMap[lang].Add(OrderedTranslationItem.Of(c));
            }
        }

        foreach (var kvp in contentMap.Where(kvp => kvp.Value.Count != 0))
            targetRepository.AddOrUpdateLanguage(kvp.Key, kvp.Value);
    }
}
