// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using JetBrains.Annotations;
using PG.Commons.Data.Holder;

namespace PG.Commons.Binary.File.Builder
{
    public interface IBinaryFileBuilder<out TFileToBuild, in TFileHolder>
        where TFileHolder : IFileHolder
        where TFileToBuild : IBinaryFile
    {
        [NotNull] TFileToBuild FromBytes([NotNull] byte[] byteStream);

        [NotNull] TFileToBuild FromHolder([NotNull] TFileHolder holder);
    }
}
