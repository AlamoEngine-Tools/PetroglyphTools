// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.IO;

namespace PG.StarWarsGame.Components.Localisation.IO;

/// <summary>
///     The strategy used to load an
///     <see cref="PG.StarWarsGame.Components.Localisation.Repository.ITranslationRepository" />
/// </summary>
public interface IInputStrategy
{
    // TODO: implement DatInputStrategy, XmlInputStrategy, CsvInputStrategy -> this should cover most current community tools.

    /// <summary>
    ///     Determines if we are reading a single file, multiple files or all files from a base directory with an optional file
    ///     filter.
    /// </summary>
    enum FileImportGrouping
    {
        /// <summary>
        ///     Single file import.
        /// </summary>
        Single,

        /// <summary>
        ///     Multiple file import.
        /// </summary>
        Multi,

        /// <summary>
        ///     Imports all files form a directory matching the optional <see cref="IInputStrategy.FileFilter" />
        /// </summary>
        BaseDirectory
    }

    /// <summary>
    ///     The validation level used against the provided source.
    /// </summary>
    enum ValidationLevel
    {
        /// <summary>
        ///     Invalid entires are skipped.
        /// </summary>
        Lenient,

        /// <summary>
        ///     Invalid enrties throw an exception.
        /// </summary>
        Strict
    }

    /// <summary>
    ///     The <see cref="FileImportGrouping" />
    /// </summary>
    FileImportGrouping ImportGrouping { get; }

    /// <summary>
    ///     The validation level used for the source files.
    /// </summary>
    ValidationLevel Validation { get; }

    /// <summary>
    ///     An optional file filter following the established FileDialog.Filter pattern.
    ///     For more info check
    ///     <a
    ///         href="https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.filedialog.filter?view=windowsdesktop-8.0">
    ///         FileDialog.Filter
    ///         Property
    ///     </a>
    /// </summary>
    string FileFilter { get; }

    /// <summary>
    ///     A set of file paths. Can be empty.
    /// </summary>
    ISet<string> FilePaths { get; }

    /// <summary>
    ///     The base directory.
    /// </summary>
    DirectoryInfo BaseDirectory { get; }
}
