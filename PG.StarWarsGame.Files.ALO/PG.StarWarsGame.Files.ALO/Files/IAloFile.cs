using PG.Commons.Files;
using PG.StarWarsGame.Files.ALO.Data;
using PG.StarWarsGame.Files.ChunkFiles.Files;

namespace PG.StarWarsGame.Files.ALO.Files;

public interface IAloFile<out T, out TFileInfo> : IChunkFile<T, TFileInfo> where T : IAloDataContent where TFileInfo : PetroglyphMegPackableFileInformation;