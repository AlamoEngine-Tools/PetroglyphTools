// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.IO;

namespace PG.StarWarsGame.Components.Localisation.IO.Xml;

/// <summary>
///     The XML output strategy.
/// </summary>
public readonly record struct XmlOutputStrategy : IOutputStrategy
{
    /// <summary>
    ///     Convenience accessor for the full file path as string.
    /// </summary>
    public string FilePath => $"{Path.Combine(ExportBasePath.FullName, FileName)}.{Extension}";

    /// <inheritdoc />
    public IOutputStrategy.FileExportGrouping ExportGrouping => IOutputStrategy.FileExportGrouping.Single;

    /// <inheritdoc />
    public string Extension => "xml";

    /// <inheritdoc />
    public required string FileName { get; init; }

    /// <inheritdoc />
    public required DirectoryInfo ExportBasePath { get; init; }
}
