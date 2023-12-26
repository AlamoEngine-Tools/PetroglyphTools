using PG.Commons.Binary;
using PG.Commons.DataTypes;

namespace PG.StarWarsGame.Files.DAT.Binary.Metadata;

internal interface IDatKey : IHasCrc32, IBinary
{
    string Key { get; }
}