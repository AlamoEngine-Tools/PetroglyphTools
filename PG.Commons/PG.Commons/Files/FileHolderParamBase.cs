// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.Commons.Files;

/// <summary>
///     Base parameter class for use with file holders.
/// </summary>
public abstract record FileHolderParamBase : ParamBase, IFileHolderParam
{
    /// <summary>
    ///     <inheritdoc cref="FileHolderBase{TParam,TModel,TFileType}.FilePath" />
    /// </summary>
    public string? FilePath { get; set; }
}