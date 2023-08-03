// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Files;

namespace PG.Commons.Binary.File;

/// <summary>
///     A builder that is capable of converting a <see cref="IFileHolder" /> to its <see cref="IBinaryFile" />
///     representation and vice versa.
/// </summary>
/// <typeparam name="TBinaryModel">The associated <see cref="IBinaryFile" /> of this builder.</typeparam>
/// <typeparam name="TFileHolder">The associated <see cref="IFileHolder" /> of this builder.</typeparam>
/// <typeparam name="TFileHolderParam">
///     The <see cref="IFileHolder{TModel,TAlamoFileType}" />'s
///     <see cref="IFileHolderParam" />
/// </typeparam>
public interface IBinaryFileConverter<TBinaryModel, TFileHolder, in TFileHolderParam>
    where TBinaryModel : IBinaryFile
    where TFileHolder : IFileHolder
    where TFileHolderParam : IFileHolderParam
{
    /// <summary>
    ///     Builds a <see cref="IBinaryFile" /> from a given <see cref="IFileHolder" />.
    /// </summary>
    /// <param name="holder">The holder of the binary file.</param>
    /// <returns>The converted binary file.</returns>
    TBinaryModel FromHolder(TFileHolder holder);

    /// <summary>
    ///     Builds a <see cref="IFileHolder" /> from a given <see cref="IBinaryFile" />.
    /// </summary>
    /// <param name="param">The <see cref="IFileHolder{TModel,TAlamoFileType}" />'s <see cref="IFileHolderParam" /></param>
    /// <param name="model">The <see cref="IBinaryFile" /> to convert.</param>
    /// <returns>The <see cref="IFileHolder{TModel,TAlamoFileType}" /></returns>
    TFileHolder ToHolder(TFileHolderParam param, TBinaryModel model);
}