using PG.Commons.Data.Files;

namespace PG.Commons.Data.Holder
{
    /// <summary>
    /// A generic wrapper around alamo file types that holds the file content in an accessible data structure.
    /// </summary>
    public interface IFileHolder
    {
        /// <summary>
        /// The path to the directory that holds the file on disc. 
        /// </summary>
        string FilePath { get; }

        /// <summary>
        /// The desired file name without the file extension.
        /// </summary>
        string FileName { get; }
    }

    /// <summary>
    /// A generic wrapper around alamo file types that holds the file content in an accessible data structure.
    /// </summary>
    /// <typeparam name="TContent">The content of the alamo file in a usable data format.</typeparam>
    /// <typeparam name="TAlamoFileType">The alamo file type definition implementing <see cref="IAlamoFileType"/></typeparam>
    public interface IFileHolder<TContent, out TAlamoFileType> : IFileHolder where TAlamoFileType : IAlamoFileType
    {
        /// <summary>
        /// The alamo file type definition implementing <see cref="IAlamoFileType"/>
        /// </summary>
        TAlamoFileType FileType { get; }

        /// <summary>
        /// The content of the alamo file in a usable data format.
        /// </summary>
        TContent Content { get; set; }

        /// <summary>
        /// The file name excluding the full path, eg. "myfile.txt"
        /// </summary>
        string FullyQualifiedName { get; }
    }
}
