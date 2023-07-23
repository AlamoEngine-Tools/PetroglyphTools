namespace PG.StarWarsGame.Files.MEG.Utilities;

internal interface IUnsignedList<T> : IReadOnlyUnsignedList<T>
{
    new T this[uint index] { get; set; }

    void Clear();

    void Add(T item);

    bool Remove(T item);

    void RemoveAt(uint index);

    void Insert(uint index, T item);

    bool Contains(T item);

    bool TryGetIndex(T item, out uint? index);
}