// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Components.Localisation.IO;

/// <summary>
///     The import handler associated with a given <see cref="IInputStrategy" />
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IImportHander<T> where T : IInputStrategy
{
}