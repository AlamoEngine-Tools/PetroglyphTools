// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Components.Localisation.IO;
using PG.StarWarsGame.Components.Localisation.Repository;

namespace PG.StarWarsGame.Components.Localisation.Services;

/// <summary>
///     Base service for handling a <see cref="ITranslationRepository" />
/// </summary>
public interface ITranslationRepositoryService
{
    /// <summary>
    ///     Loads a given file or set of files as a singular <see cref="ITranslationRepository" />
    /// </summary>
    /// <param name="inputStrategy"></param>
    /// <param name="filePaths"></param>
    /// <returns></returns>
    ITranslationRepository LoadAs(IInputStrategy inputStrategy, params string[] filePaths);

    /// <summary>
    ///     Stores a <see cref="ITranslationRepository" /> as a set of files defined in the <see cref="IOutputStrategy" /> in a
    ///     given base directory.
    /// </summary>
    /// <param name="baseFilePath"></param>
    /// <param name="outputStrategy"></param>
    /// <param name="repository"></param>
    /// <returns></returns>
    void StoreAs(string baseFilePath, IOutputStrategy outputStrategy, ITranslationRepository repository);
}