using PG.Commons.Binary.File;

namespace PG.StarWarsGame.Files.MEG.Binary.Shared.Metadata;

internal interface IMegFileMetadata : IBinaryFile
{
    /// <remarks>
    /// The .MEG specification allows <see cref="uint"/>, however in .NET we are
    /// limited to <see cref="int"/> for indexing native list-like structures.  
    /// </remarks>
    int FileNumber { get; }

    IFileNameTable FileNameTable { get; }

    IFileTable FileTable { get; }
}