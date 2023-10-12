// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Services.Processing;

namespace PG.StarWarsGame.Components.Localisation.IO.Formats;

/// <inheritdoc />
public interface ILocalisationFormatterProcessingInstructionsParam : IProcessingInstructionsParam
{
    /// <summary>
    ///     The target Directory for the import/export.
    /// </summary>
    public string? Directory { get; }

    /// <summary>
    ///     The desired file name.
    /// </summary>
    public string? FileName { get; }

    /// <summary>
    ///     The desired file extension.
    /// </summary>
    public string? FileExtension { get; }
}