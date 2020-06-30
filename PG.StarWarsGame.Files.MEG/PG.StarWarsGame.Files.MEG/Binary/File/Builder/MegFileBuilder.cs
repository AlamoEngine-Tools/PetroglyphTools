using System.Runtime.CompilerServices;
using PG.Commons.Binary.File.Builder;
using PG.StarWarsGame.Files.MEG.Binary.File.Type.Definition;
using PG.StarWarsGame.Files.MEG.Holder;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.MEG.Test")]
namespace PG.StarWarsGame.Files.MEG.Binary.File.Builder
{
    internal class MegFileBuilder : IBinaryFileBuilder<MegFile, MegFileHolder>
    {
        //TODO [gruenwaldlu, 2020-06-30-21:19:52+2]: Implement!
        public MegFile FromBytes(byte[] byteStream)
        {
            throw new System.NotImplementedException();
        }

        //TODO [gruenwaldlu, 2020-06-30-21:20:03+2]: Implement!
        public MegFile FromHolder(MegFileHolder holder)
        {
            throw new System.NotImplementedException();
        }
    }
}