using PG.Commons.Files;

namespace PG.Commons.Binary.File;

/// <summary>
/// A builder that is capable of converting a <see cref="IFileHolder"/> to its <see cref="IBinaryFile"/> representation 
/// </summary>
/// <typeparam name="TFileToBuild">The associated <see cref="IBinaryFile"/> of this builder.</typeparam>
/// <typeparam name="TFileHolder">The associated <see cref="IFileHolder"/> of this builder.</typeparam>
public interface IBinaryFileBuilder<out TFileToBuild, in TFileHolder>
    where TFileHolder : IFileHolder
    where TFileToBuild : IBinaryFile
{
    /// <summary>
    /// Builds a binary file from a given <see cref="IFileHolder"/>.
    /// </summary>
    /// <param name="holder">The holder of the binary file.</param>
    /// <returns>The converted binary file.</returns>
    TFileToBuild FromHolder(TFileHolder holder);
}