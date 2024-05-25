using PG.Commons.Files;
using PG.StarWarsGame.Files.ChunkFiles.Data;

namespace PG.StarWarsGame.Files.ChunkFiles.Files;

public interface IChunkFile<out T, out TFileInfo> : IPetroglyphFileHolder<T, TFileInfo> where T : IChunkData where TFileInfo : PetroglyphMegPackableFileInformation;