// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PG.StarWarsGame.Components.Localisation.IO.Xml;

/// <summary>
///     The XML input strategy.
/// </summary>
public readonly record struct XmlInputStrategy : IInputStrategy
{
    /// <summary>
    ///     .ctor
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="validation"></param>
    public XmlInputStrategy(string filePath,
        IInputStrategy.ValidationLevel validation = IInputStrategy.ValidationLevel.Lenient)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException($"'{filePath}' is not a valid file path.", nameof(filePath));

        FilePaths = new HashSet<string> { filePath };
        Validation = validation;
    }

    /// <summary>
    ///     Convenience method for accessing the only file path required for this strategy.
    /// </summary>
    public string FilePath => FilePaths.Single();

    /// <inheritdoc />
    public IInputStrategy.FileImportGrouping ImportGrouping => IInputStrategy.FileImportGrouping.Single;

    /// <inheritdoc />
    public IInputStrategy.ValidationLevel Validation { get; }

    /// <inheritdoc />
    public string FileFilter => throw new InvalidOperationException(
        $"{nameof(XmlInputStrategy)}.{nameof(FileFilter)} is not supported for {nameof(ImportGrouping)}={nameof(IInputStrategy.FileImportGrouping.Single)}");

    /// <inheritdoc />
    public ISet<string> FilePaths { get; }

    /// <inheritdoc />
    public DirectoryInfo BaseDirectory => throw new InvalidOperationException(
        $"{nameof(XmlInputStrategy)}.{nameof(DirectoryInfo)} is not supported for {nameof(ImportGrouping)}={nameof(IInputStrategy.FileImportGrouping.Single)}");
}
