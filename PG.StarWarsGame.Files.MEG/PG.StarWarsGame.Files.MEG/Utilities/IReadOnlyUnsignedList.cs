using System.Collections.Generic;

namespace PG.StarWarsGame.Files.MEG.Utilities;

internal interface IReadOnlyUnsignedList<out T> : IEnumerable<T>
{
    int Count { get; }

    T this[uint index] { get; }
}