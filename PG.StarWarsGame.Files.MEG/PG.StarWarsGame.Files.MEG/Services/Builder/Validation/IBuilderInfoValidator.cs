using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// A validator that checks an instance of a <see cref="MegFileDataEntryBuilderInfo"/>
/// </summary>
public interface IBuilderInfoValidator
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builderInfo"></param>
    /// <returns></returns>
    bool Validate(MegFileDataEntryBuilderInfo builderInfo);
}