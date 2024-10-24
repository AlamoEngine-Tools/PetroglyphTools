// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace PG.StarWarsGame.Components.Localisation.IO.Xml.Serializable.v1;

/// <summary>
///     XML Element description
/// </summary>
[XmlRoot(ElementName = "TranslationData")]
[ExcludeFromCodeCoverage]
public class TranslationData
{
    /// <summary>
    ///     .ctor
    /// </summary>
    public TranslationData()
    {
        TranslationHolder ??= new List<Translation>();
    }

    /// <summary>
    ///     Holds <see cref="Translation" />s.
    /// </summary>
    [XmlElement(ElementName = "Translation")]
    public List<Translation> TranslationHolder { get; set; }
}
