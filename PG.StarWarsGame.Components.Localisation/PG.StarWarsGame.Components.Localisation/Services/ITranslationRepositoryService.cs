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
    ///     Loads a given file or set of files as a singular <see cref="ITranslationRepository" /> based on the
    ///     <see cref="IInputStrategy" />
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="strategy"></param>
    /// <param name="repository"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    void LoadAs<T>(IImportHandler<T> handler, T strategy, ITranslationRepository repository)
        where T : IInputStrategy; // TODO: Handler-factory based on the strategy.

    /// <summary>
    ///     Stores a <see cref="ITranslationRepository" /> as defined in the <see cref="IOutputStrategy" />
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="strategy"></param>
    /// <param name="repository"></param>
    /// <typeparam name="T"></typeparam>
    void StoreAs<T>(IExportHandler<T> handler, T strategy, ITranslationRepository repository)
        where T : IOutputStrategy; // TODO: Handler-factory based on the strategy.
}
