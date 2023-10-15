namespace PG.Commons.Binary.File;

/// <summary>
///     A builder that is capable of converting a generic file model to its <see cref="IBinaryFile"/>
///     representation and vice versa.
/// </summary>
/// <typeparam name="TBinaryModel">The associated <see cref="IBinaryFile" /> of this builder.</typeparam>
/// <typeparam name="TFileModel">The associated file model of this builder.</typeparam>
public interface IBinaryConverter<TBinaryModel, TFileModel> where TBinaryModel : IBinaryFile
{
    /// <summary>
    ///     Builds a <typeparamref name="TBinaryModel"/> from a given <typeparamref name="TFileModel"/>.
    /// </summary>
    /// <param name="model">The model of the binary file.</param>
    /// <returns>The converted binary file.</returns>
    TBinaryModel ModelToBinary(TFileModel model);

    /// <summary>
    ///     Builds a <typeparamref name="TFileModel"/> from a given <typeparamref name="TBinaryModel"/>.
    /// </summary>
    /// <param name="binary">The <see cref="IBinaryFile" /> to convert.</param>
    /// <returns>The <typeparamref name="TFileModel"/>.</returns>
    /// <exception cref="BinaryCorruptedException">When <paramref name="binary"/> is not valid.</exception>
    TFileModel BinaryToModel(TBinaryModel binary);
}