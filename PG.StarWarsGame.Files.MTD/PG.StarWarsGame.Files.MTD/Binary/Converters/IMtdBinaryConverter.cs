using PG.Commons.Binary.File;
using PG.StarWarsGame.Files.MTD.Binary.Metadata;
using PG.StarWarsGame.Files.MTD.Data;

namespace PG.StarWarsGame.Files.MTD.Binary;

internal interface IMtdBinaryConverter : IBinaryConverter<MtdBinaryFile, IMegaTextureDirectory>;