// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Repository;

namespace PG.StarWarsGame.Components.Localisation.Repository.Translation;

internal sealed class TranslationRepository : RepositoryBase<string, ITranslationItemRepository>, ITranslationRepository
{
    /// <inheritdoc />
    public TranslationRepository(IServiceProvider services) : base(services)
    {
    }
}