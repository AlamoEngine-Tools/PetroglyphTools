// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;

namespace PG.StarWarsGame.Components.Localisation.IO.Xml;

/// <summary>
///     The XML output strategy.
/// </summary>
[ExcludeFromCodeCoverage]
public readonly record struct XmlOutputStrategy : IOutputStrategy
{
    /// <summary>
    ///     .ctor
    /// </summary>
    /// <param name="exportBasePath"></param>
    /// <param name="fileName"></param>
    /// <exception cref="ArgumentException"></exception>
    public XmlOutputStrategy(IDirectoryInfo exportBasePath, string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(fileName));
        FileName = fileName;
        ExportBasePath = exportBasePath;
    }

    /// <summary>
    ///     Convenience accessor for the full file path as string.
    /// </summary>
    public string FilePath => $"{Path.Combine(ExportBasePath.FullName, FileName)}.{Extension}";

    /// <inheritdoc />
    public IDirectoryInfo ExportBasePath { get; }

    /// <inheritdoc />
    public IOutputStrategy.FileExportGrouping ExportGrouping => IOutputStrategy.FileExportGrouping.Single;

    /// <inheritdoc />
    public string Extension => "xml";

    /// <inheritdoc />
    public string FileName { get; }
}
