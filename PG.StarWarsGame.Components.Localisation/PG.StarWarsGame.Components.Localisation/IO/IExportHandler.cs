// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Components.Localisation.IO;

/// <summary>
///     The export handler associated with a given <see cref="IOutputStrategy" />
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IExportHandler<T> where T : IOutputStrategy
{
}