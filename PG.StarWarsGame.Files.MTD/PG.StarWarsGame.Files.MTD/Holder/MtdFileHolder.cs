// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.MTD.Files;
using System.Collections.Generic;
using PG.Commons.Files;

namespace PG.StarWarsGame.Files.MTD.Holder;

public sealed class MtdFileHolder : IPetroglyphFileHolder<,,>
{
    public string FilePath { get; }
    public string FileName { get; }
    public MtdAlamoFileType FileType { get; }
    public IList<MtdIcon> Content { get; set; }
    public string FullyQualifiedName => $"{FileName}.{FileType.FileExtension}";
}
