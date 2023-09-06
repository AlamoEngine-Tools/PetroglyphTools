using System;
using PG.Commons.Binary.File;
using PG.StarWarsGame.Files.MEG.Binary.V1.Metadata;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Binary;

interface IMegFileBinaryConverter : IBinaryFileConverter<MegMetadata, IMegFile, MegFileHolderParam>,
    IDisposable
{

}