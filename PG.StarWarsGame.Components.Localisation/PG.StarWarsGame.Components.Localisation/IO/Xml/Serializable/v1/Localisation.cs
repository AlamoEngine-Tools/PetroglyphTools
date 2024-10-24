// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace PG.StarWarsGame.Components.Localisation.IO.Xml.Serializable.v1;

/// <summary>
///     XML file definition.
/// </summary>
[XmlRoot(ElementName = "LocalisationHolder")]
[ExcludeFromCodeCoverage]
public sealed class Localisation
{
    /// <summary>
    ///     .ctor
    /// </summary>
    public Localisation()
    {
        TranslationData ??= new TranslationData();
    }

    /// <summary>
    ///     Translation data associated with the key.
    /// </summary>
    [XmlElement(ElementName = "TranslationData")]
    public TranslationData TranslationData { get; set; }

    /// <summary>
    ///     Translation Key
    /// </summary>
    [XmlAttribute(AttributeName = "Key")]
    public required string Key { get; set; }
}
