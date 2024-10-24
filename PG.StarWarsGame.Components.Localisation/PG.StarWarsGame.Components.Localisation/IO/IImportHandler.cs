// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Components.Localisation.Repository;

namespace PG.StarWarsGame.Components.Localisation.IO;

/// <summary>
///     The import handler associated with a given <see cref="IInputStrategy" />
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IImportHandler<in T> where T : IInputStrategy
{
    /// <summary>
    ///     Imports data into a <see cref="ITranslationRepository" />
    /// </summary>
    /// <param name="strategy"></param>
    /// <param name="targetRepository"></param>
    /// <returns></returns>
    void Import(T strategy, ITranslationRepository targetRepository);
}
