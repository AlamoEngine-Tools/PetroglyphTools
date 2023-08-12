// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Repository;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Repository;

/// <inheritdoc cref="ITranslationItemRepository" />
internal sealed class TranslationItemRepository : RepositoryBase<IAlamoLanguageDefinition, ITranslationItem>,
    ITranslationItemRepository
{
    /// <inheritdoc />
    public TranslationItemRepository(IServiceProvider services) : base(services)
    {
    }
}