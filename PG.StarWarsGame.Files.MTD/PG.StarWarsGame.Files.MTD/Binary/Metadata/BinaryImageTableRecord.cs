using PG.Commons.Binary;

namespace PG.StarWarsGame.Files.MTD.Binary.Metadata;

internal struct BinaryImageTableRecord : IBinary
{
    string Name { get; }

    public uint X { get; }

    public uint Y { get; }

    public uint Width { get; }

    public uint Height { get; }

    public bool Alpha { get; }

    public byte[] Bytes { get; }

    public int Size { get; }
}