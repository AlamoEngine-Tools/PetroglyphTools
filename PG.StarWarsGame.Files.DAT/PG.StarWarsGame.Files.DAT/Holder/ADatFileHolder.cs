// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using JetBrains.Annotations;
using PG.Commons.Data.Files;
using PG.Commons.Data.Holder;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.DAT.Test")]

namespace PG.StarWarsGame.Files.DAT.Holder
{
    /// <summary>
    /// Abstract DatFileHolder implementation.
    /// </summary>
    /// <typeparam name="TContent"></typeparam>
    /// <typeparam name="TAlamoFileType"></typeparam>
    public abstract class ADatFileHolder<TContent, TAlamoFileType> : IFileHolder<TContent, TAlamoFileType>
        where TAlamoFileType : IAlamoFileType, new()
    {
        protected ADatFileHolder(string filePath, string fileName)
        {
            FilePath = filePath;
            FileName = fileName;
        }

        public string FilePath { get; }
        public string FileName { get; }

        public abstract TAlamoFileType FileType { get; }

        [NotNull] public abstract TContent Content { get; set; }

        [NotNull] public string FullyQualifiedName => $"{FileName}.{FileType.FileExtension}";
    }
}
