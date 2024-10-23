// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.Commons.Utilities;

/// <summary>
///     XML file utility.
/// </summary>
public static class XmlUtilities
{
    /// <summary>
    ///     Escape XML Content.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string? EscapeXml(string? s)
    {
        if (string.IsNullOrEmpty(s)) return s;

        var returnString = s!;
        returnString = returnString.Replace("&", "&amp;");
        returnString = returnString.Replace("<", "&lt;");
        returnString = returnString.Replace(">", "&gt;");
        returnString = returnString.Replace("'", "&apos;");
        returnString = returnString.Replace("\"", "&quot;");
        return returnString;
    }

    /// <summary>
    ///     Unescape XML content.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string? UnescapeXml(string? s)
    {
        if (string.IsNullOrEmpty(s)) return s;

        var returnString = s!;
        returnString = returnString.Replace("&apos;", "'");
        returnString = returnString.Replace("&quot;", "\"");
        returnString = returnString.Replace("&gt;", ">");
        returnString = returnString.Replace("&lt;", "<");
        returnString = returnString.Replace("&amp;", "&");
        return returnString;
    }
}
