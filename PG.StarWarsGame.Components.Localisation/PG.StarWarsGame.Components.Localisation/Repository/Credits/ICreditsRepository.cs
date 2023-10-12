// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.Commons.Repository;
using PG.Commons.Validation;

namespace PG.StarWarsGame.Components.Localisation.Repository.Credits;

/// <summary>
///     A repository for credit text files.
/// </summary>
public interface ICreditsRepository : IRepository, IValidatable
{
    /// <inheritdoc cref="List{T}.Add" />
    void Add(ITranslationItem item);

    /// <inheritdoc cref="List{T}.Clear" />
    void Clear();

    /// <inheritdoc cref="List{T}.Contains" />
    bool Contains(ITranslationItem item);

    /// <inheritdoc cref="List{T}.Remove" />
    bool Remove(ITranslationItem item);
}