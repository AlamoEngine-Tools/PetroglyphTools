// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Repository;
using PG.StarWarsGame.Components.Localisation.Languages;

namespace PG.StarWarsGame.Components.Localisation.Repository.Translation;

/// <inheritdoc cref="ITranslationItemRepository" />
internal sealed class TranslationItemRepository : RepositoryBase<IAlamoLanguageDefinition, ITranslationItem>,
    ITranslationItemRepository
{
    /// <inheritdoc />
    public TranslationItemRepository(IServiceProvider services) : base(services)
    {
    }
}