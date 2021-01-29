using System;

namespace PG.StarWarsGame.Files.MEG.Holder
{
    /// <summary>
    /// An object representing a file packaged in a *.MEG file.
    /// </summary>
    [Serializable]
    public sealed class MegFileDataEntry
    {
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="relativeFilePath">The relative file path as defined in the *.MEG file.<br/>
        /// Usually this file path is relative to the game or mod's DATA directory, e.g. Data/My/file.xml
        /// when the containing meg file resides in c:/My/Game/Data</param>
        /// <param name="offset">Optional offset from the start of the *.MEG file archive.
        /// If not set, this offset is -1. If a *.MEG file is read from disc using the standard implementation of the
        /// <see cref="PG.StarWarsGame.Files.MEG.Services.IMegFileProcessService"/> this is the byte offset from the
        /// start of the file.</param>
        /// <param name="size">Optional file size in byte. If not set, this is 0. If a *.MEG file is read from disc
        /// using the standard implementation of the <see cref="PG.StarWarsGame.Files.MEG.Services.IMegFileProcessService"/>
        /// this is set to the size in bytes of the packaged file.</param>
        public MegFileDataEntry(string relativeFilePath, int offset = -1, uint size = 0)
        {
            RelativeFilePath = relativeFilePath;
            Offset = offset;
            Size = size;
        }

        /// <summary>
        /// The relative file path as defined in the *.MEG file.<br/>
        /// Usually this file path is relative to the game or mod's DATA directory, e.g. Data/My/file.xml
        /// </summary>
        public string RelativeFilePath { get; }
        /// <summary>
        /// Optional offset from the start of the *.MEG file archive.
        /// If not set, this offset is -1. If a *.MEG file is read from disc using the standard implementation of the
        /// <see cref="PG.StarWarsGame.Files.MEG.Services.IMegFileProcessService"/> this is the byte offset from the
        /// start of the file.
        /// </summary>
        public int Offset { get; }
        /// <summary>
        /// File size in byte. If not set via the .ctor, this is -1.
        /// If a *.MEG file is read from disc using the standard implementation of the
        /// <see cref="PG.StarWarsGame.Files.MEG.Services.IMegFileProcessService"/> this is set to the size
        /// in bytes of the packaged file.
        /// </summary>
        public uint Size { get; }
    }
}