// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using FluentValidation;

namespace PG.StarWarsGame.Files.DAT.Services.Builder.Validation;

/// <summary>
/// A validator that checks whether a <see cref="string"/> can be used as a key for DAT files.
/// </summary>
public interface IDatKeyValidator : IValidator<string>;