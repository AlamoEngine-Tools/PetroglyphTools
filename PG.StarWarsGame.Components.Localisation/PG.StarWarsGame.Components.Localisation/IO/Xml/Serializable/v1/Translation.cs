// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Xml;
using System.Xml.Serialization;
using PG.Commons.Utilities;

namespace PG.StarWarsGame.Components.Localisation.IO.Xml.Serializable.v1;

/// <summary>
///     XML file definition.
/// </summary>
[XmlRoot(ElementName = "TranslationHolder")]
[ExcludeFromCodeCoverage]
public class Translation
{
    private string _text = null!;

    /// <summary>
    ///     The content's language.
    /// </summary>
    [XmlAttribute(AttributeName = "Language")]
    public required string Language { get; set; }

    /// <summary>
    ///     Raw translation string.
    /// </summary>
    [XmlIgnore]
    public string? Text
    {
        get => _text;
        set => _text = StringUtilities.Validate(value);
    }

    /// <summary>
    ///     Translation content.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    [XmlText]
    public XmlNode[]? CDataContent
    {
        get
        {
            var dummy = new XmlDocument();
            return [dummy.CreateCDataSection(XmlUtilities.EscapeXml(Text))];
        }
        set
        {
            if (value == null)
            {
                Text = "[MISSING]";
                return;
            }

            if (value.Length != 1) throw new InvalidOperationException($"Invalid array length {value.Length}");
            var s = new SecurityElement("a", value[0].Value);
            Text = XmlUtilities.UnescapeXml(s.Text);
        }
    }
}
