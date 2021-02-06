// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.Commons.Data.Holder;
using PG.StarWarsGame.Files.MTD.Files;

namespace PG.StarWarsGame.Files.MTD.Holder
{
    public sealed class MtdFileHolder : IFileHolder<IList<MtdIcon>, MtdAlamoFileType>
    {
        public string FilePath { get; }
        public string FileName { get; }
        public MtdAlamoFileType FileType { get; }
        public IList<MtdIcon> Content { get; set; }
        public string FullyQualifiedName => $"{FileName}.{FileType.FileExtension}";
    }
}
