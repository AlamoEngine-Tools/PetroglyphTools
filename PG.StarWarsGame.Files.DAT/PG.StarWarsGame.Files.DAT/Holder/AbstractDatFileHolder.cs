// // Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Runtime.CompilerServices;
using PG.Commons.Files;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.DAT.Test")]

namespace PG.StarWarsGame.Files.DAT.Holder;

/// <summary>
///     Abstract DatFileHolder implementation.
/// </summary>
/// <typeparam name="TContent"></typeparam>
/// <typeparam name="TAlamoFileType"></typeparam>
public abstract class ADatFileHolder<TContent, TAlamoFileType> : IFileHolder<TContent, TAlamoFileType>
    where TAlamoFileType : IAlamoFileType, new()
{
    protected ADatFileHolder(string filePath, string fileName)
    {
        Directory = filePath;
        FileName = fileName;
    }

    public string Directory { get; }
    public string FileName { get; }

    public abstract TAlamoFileType FileType { get; }

    public abstract TContent Content { get; set; }

    public string FilePath => $"{FileName}.{FileType.FileExtension}";
}