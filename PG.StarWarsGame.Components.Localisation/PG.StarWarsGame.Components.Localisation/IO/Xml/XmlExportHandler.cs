// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Data.Serialization;
using PG.Commons.Services;
using PG.StarWarsGame.Components.Localisation.IO.Xml.Serializable.v1;
using PG.StarWarsGame.Components.Localisation.Languages;
using PG.StarWarsGame.Components.Localisation.Repository;
using PG.StarWarsGame.Components.Localisation.Repository.Content;

namespace PG.StarWarsGame.Components.Localisation.IO.Xml;

/// <inheritdoc cref="IExportHandler{T}" />
public class XmlExportHandler : ServiceBase, IExportHandler<XmlOutputStrategy>
{
    /// <inheritdoc />
    public XmlExportHandler(IServiceProvider services) : base(services)
    {
    }

    /// <inheritdoc />
    public void Export(XmlOutputStrategy strategy, ITranslationRepository repository)
    {
        FileSystem.Directory.CreateDirectory(strategy.ExportBasePath.FullName);
        var xml = ToXml(repository);
        Services.GetService<IXmlSerializationSupportService>()?.SerializeObjectAndStoreToDisc(strategy.FileName, xml);
    }

    /// <summary>
    ///     Converts an <see cref="ITranslationRepository" /> to the corresponding XML file representation.
    /// </summary>
    /// <param name="repository"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected LocalisationData ToXml(ITranslationRepository repository)
    {
        var xml = new LocalisationData();
        var languages = GetLanguages(repository);
        var keys = GetKeys(repository);
        foreach (var key in keys)
        {
            var loc = new Serializable.v1.Localisation
            {
                Key = key.ToUpper(),
                TranslationData = new TranslationData()
            };
            foreach (var t in languages.Select(lang => new Translation
                     {
                         Language = lang.LanguageIdentifier.ToUpper(),
                         Text = repository.GetTranslationItem(lang,
                                 OrderedTranslationItemId.Of(key) ??
                                 throw new InvalidOperationException(
                                     $"No valid {nameof(OrderedTranslationItemId)} could be created from {key}"))
                             .Content
                             .Value
                     }))
                loc.TranslationData.TranslationHolder.Add(t);

            xml.LocalisationHolder.Add(loc);
        }

        return xml;
    }

    /// <summary>
    ///     Fetches all text keys from the repository.
    /// </summary>
    /// <param name="repository"></param>
    /// <returns></returns>
    protected ISet<string> GetKeys(ITranslationRepository repository)
    {
        var keySet = new HashSet<string>();
        foreach (var items in repository.Content.Values)
        foreach (var item in items)
            keySet.Add(item.Content.Key);
        return keySet;
    }

    /// <summary>
    ///     Returns a set of all contained language definitions.
    /// </summary>
    /// <param name="repository"></param>
    /// <returns></returns>
    protected HashSet<IAlamoLanguageDefinition> GetLanguages(ITranslationRepository repository)
    {
        return new HashSet<IAlamoLanguageDefinition>(repository.Content.Keys);
    }
}
