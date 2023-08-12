// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Repository;

namespace PG.StarWarsGame.Components.Localisation.Repository;

/// <summary>
///     Repository for sorted localisations.
/// </summary>
public interface ITranslationRepository : IRepository<string, ITranslationItemRepository>
{
}