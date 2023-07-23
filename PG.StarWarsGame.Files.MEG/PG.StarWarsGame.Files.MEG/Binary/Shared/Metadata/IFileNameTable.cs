using System.Collections.Generic;
using PG.Commons.Binary;

namespace PG.StarWarsGame.Files.MEG.Binary.Shared.Metadata;

internal interface IFileNameTable : IBinary, IEnumerable<string>
{
    /// <remarks>
    /// The .MEG specification allows <see cref="uint"/>, however in .NET we are
    /// limited to <see cref="int"/> for indexing native list-like structures.  
    /// </remarks>
    string this[int i] { get; }
}