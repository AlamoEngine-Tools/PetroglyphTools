// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Validation;

namespace PG.StarWarsGame.Localisation.Repository;

/// <summary>
///     A simple translation item.
/// </summary>
public interface ITranslationItem : IValidatable
{
    /// <summary>
    ///     The item's key.
    /// </summary>
    string Key { get; }

    /// <summary>
    ///     The item's value.
    /// </summary>
    string Value { get; }
}