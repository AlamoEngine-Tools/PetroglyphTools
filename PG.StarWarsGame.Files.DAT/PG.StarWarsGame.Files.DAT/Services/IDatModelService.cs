using System.Collections.Generic;
using PG.StarWarsGame.Files.DAT.Data;

namespace PG.StarWarsGame.Files.DAT.Services;

/// <summary>
/// A service to work with DAT models.
/// </summary>
public interface IDatModelService
{
    /// <summary>
    /// Gets all duplicate keys of the current model and stores them into a dictionary.
    /// The key of the dictionary represents a duplicate key in model.
    /// The value of the dictionary contains a list of the duplicate entries. This list always has two or more items and preserves the entry's order.
    /// </summary>
    /// <param name="datModel">The DAT model to check.</param>
    /// <returns>A dictionary of duplicate keys in <paramref name="datModel"/>.</returns>
    IReadOnlyDictionary<string, IReadOnlyList<DatStringEntry>> GetDuplicateEntries(IDatModel datModel);

    /// <summary>
    /// Creates a new DAT model by removing any duplicate keys of the input model. The found entry will be used.
    /// </summary>
    /// <param name="datModel">The DAT model.</param>
    /// <returns>A new duplicate-free DAT model.</returns>
    IDatModel RemoveDuplicates(IDatModel datModel);

    /// <summary>
    /// Gets a set of keys missing in a specified DAT model which are present in a specified base model. 
    /// </summary>
    /// <example>
    /// <code>
    /// BaseModel       := [A, B, C]
    /// ModelToCompare  := [A, C, D]
    /// Result          := [B]
    /// </code>
    /// </example>
    /// <param name="baseDatModel">The base DAT model.</param>
    /// <param name="datToCompare">The DAT model to get missing keys from.</param>
    /// <returns>A set of keys missing in <paramref name="datToCompare"/> which are present in <paramref name="baseDatModel"/>.</returns>
    ISet<string> GetMissingKeysFromBase(IDatModel baseDatModel, IDatModel datToCompare);

    /// <summary>
    /// Creates a new <see cref="IDatModel"/> by merging one model into another.
    /// <br/>
    /// Both models are required to have the same key sort order.
    /// <br/>
    /// <br/>
    /// For models with keys sorted by CRC, when <paramref name="mergeOptions"/> is <see cref="DatMergeOptions.Overwrite"/>,
    /// this method overwrites the last occurence of an entry in <paramref name="baseDatModel"/> with the last occurence of <paramref name="datToMerge"/>.
    /// <br/>
    /// <code>
    /// Example:
    /// BaseDat := [A, A', B, C] where A' is a duplicate of the key A with a different value.
    /// ToMerge := [A'', A''', D] where A'' and A''' are duplicates of the key A with a different values
    /// Result  := [A, A''', B, C, D] 
    /// </code>
    /// <br/>
    /// <br/>
    /// For models with keys not sorted, this method merges entries index by index.
    /// <code>
    /// Example (KeepExisting):
    /// BaseDat := [A, B, A', C]
    /// ToMerge := [B, D, A'', E, F]
    /// Result  := [B, D, A', E, F]
    /// </code>
    /// <code>
    /// Example (Overwrite):
    /// BaseDat := [A, B, A', C]
    /// ToMerge := [B, D, A'']
    /// Result  := [B, D, A'', C] // A'' instead of A' compared to KeepExisting
    /// </code>
    /// </summary>
    /// <remarks>
    /// A common use case for this is if you have an English model and a foreign language model.
    /// In most cases the english model is the reference model which contains the newest keys. In that case call
    /// <code>Merge(ForeignModel, EnglishModel, DatMergeOptions.KeepExisting)</code>
    /// This will create a new model will all foreign text, but only adds the new keys (with english values).
    /// </remarks>
    /// <param name="baseDatModel">The base model.</param>
    /// <param name="datToMerge">The model to merge into <paramref name="baseDatModel"/>.</param>
    /// <param name="mergedKeys">Keys that got added or overwritten will be stored into this variable.</param>
    /// <param name="mergeOptions">Specifies how to treat existing keys.</param>
    /// <returns>The merged model.</returns>
    IDatModel Merge(IDatModel baseDatModel, IDatModel datToMerge, out ICollection<MergedKeyResult> mergedKeys,
        DatMergeOptions mergeOptions = DatMergeOptions.KeepExisting);
}