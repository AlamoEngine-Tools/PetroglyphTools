// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Components.Localisation.IO;

/// <summary>
///     The strategy used to store an
///     <see cref="PG.StarWarsGame.Components.Localisation.Repository.ITranslationRepository" />
/// </summary>
public interface IOutputStrategy
{
    // TODO: implement DatOutputStrategy, XmlOutputStrategy, CsvOutputStrategy -> this should cover most current community tools.

    /// <summary>
    ///     Determines if the <see cref="PG.StarWarsGame.Components.Localisation.Repository.ITranslationRepository" /> is being
    ///     exported to a single or multiple files.
    /// </summary>
    enum FileExportGrouping
    {
        /// <summary>
        ///     Single file export.
        /// </summary>
        Single,

        /// <summary>
        ///     Multiple fles export
        /// </summary>
        Multi
    }

    /// <summary>
    ///     The desired export grouping.
    /// </summary>
    FileExportGrouping ExportGrouping { get; }

    /// <summary>
    ///     The export file extension
    /// </summary>
    string Extension { get; }

    /// <summary>
    ///     Creates a fresh instance of a given associated <see cref="IExportHandler{T}" />
    /// </summary>
    /// <returns></returns>
    IExportHandler<IOutputStrategy> CreateExportHandler();
}