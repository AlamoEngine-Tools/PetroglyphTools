// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Files;
using PG.Commons.Validation;

namespace PG.Commons.Services.Processing;

/// <summary>
///     Contract for processing instruction parameters.
/// </summary>
public interface IProcessingInstructionsParam : IParam, IValidatable
{
}