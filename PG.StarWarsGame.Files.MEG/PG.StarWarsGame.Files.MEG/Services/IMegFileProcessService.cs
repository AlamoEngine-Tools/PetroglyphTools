using PG.StarWarsGame.Files.MEG.Holder;

namespace PG.StarWarsGame.Files.MEG.Services
{
    /// <summary>
    /// MEF service interface definition for handling sorted *.DAT files.
    /// A default implementation is provided in <see cref=""/>.
    /// When requesting the default implementation via an IoC Container or registering via injection, you may pass
    /// a file system as argument implementing <see cref="System.IO.Abstractions.IFileSystem"/> and a logger
    /// implementing <see cref="Microsoft.Extensions.Logging.ILogger"/>
    /// </summary>
    public interface IMegFileProcessService
    {
        void Unpack(MegFileHolder holder);
        void Pack(MegFileHolder holder);
        MegFileHolder Load(string filePath);
    }
}