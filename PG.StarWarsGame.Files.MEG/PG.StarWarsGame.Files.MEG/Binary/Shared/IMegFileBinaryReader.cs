using System;
using PG.Commons.Binary.File;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal interface IMegFileBinaryReader : IBinaryFileReader<IMegFileMetadata>, IDisposable;