// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace PG.StarWarsGame.Components.Localisation.IO.Xml.Serializable.v1;

/// <summary>
/// </summary>
[XmlRoot(ElementName = "LocalisationData")]
[ExcludeFromCodeCoverage]
public sealed class LocalisationData
{
    /// <summary>
    ///     .ctor
    /// </summary>
    public LocalisationData()
    {
        LocalisationHolder ??= new List<Localisation>();
    }

    /// <summary>
    ///     Holds <see cref="Localisation" />s
    /// </summary>
    [XmlElement(ElementName = "Localisation")]
    public List<Localisation> LocalisationHolder { get; set; }
}
