// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Repository;
using PG.StarWarsGame.Components.Localisation.Languages;

namespace PG.StarWarsGame.Components.Localisation.Repository.Translation;

/// <summary>
///     A repository to hold <see cref="ITranslationItem" />s.
/// </summary>
public interface ITranslationItemRepository : IRepository<IAlamoLanguageDefinition, ITranslationItem>
{
}