using System.Runtime.CompilerServices;
using PG.Commons.Binary.File;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.MEG.Test")]
namespace PG.StarWarsGame.Files.MEG.Binary.File.Type.Definition
{
    internal class MegFile : IBinaryFile
    {
        public byte[] ToBytes()
        {
            throw new System.NotImplementedException();
        }
    }
}