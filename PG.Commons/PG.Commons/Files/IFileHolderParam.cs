// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.Commons.Files;

/// <summary>
///     Parameter wrapper for <see cref="FileHolderBase{IFileHolderParam,TModel,TAlamoFileType}" /> creation.
/// </summary>
public interface IFileHolderParam : IParam
{
    /// <inheritdoc cref="FileHolderBase{TParam,TModel,TFileType}.FilePath" />
    public string FilePath { get; }
}