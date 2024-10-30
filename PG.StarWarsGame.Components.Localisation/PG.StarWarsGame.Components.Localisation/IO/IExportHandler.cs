// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Components.Localisation.Repository;

namespace PG.StarWarsGame.Components.Localisation.IO;

/// <summary>
///     The export handler associated with a given <see cref="IOutputStrategy" />
/// </summary>
public interface IExportHandler<in T> where T : IOutputStrategy
{
    /// <summary>
    ///     Exports the given repository.
    /// </summary>
    /// <param name="strategy"></param>
    /// <param name="repository"></param>
    void Export(T strategy, ITranslationRepository repository);
}
