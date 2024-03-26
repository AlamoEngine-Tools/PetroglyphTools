using System;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services.Builder.Normalization;
using PG.StarWarsGame.Files.DAT.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.DAT.Services.Builder;

/// <summary>
/// 
/// </summary>
public sealed class EmpireAtWarMasterTextFileBuilder : DatBuilderBase
{
    /// <summary>
    /// 
    /// </summary>
    public override DatFileType TargetKeySortOrder => DatFileType.OrderedByCrc32;

    /// <summary>
    /// 
    /// </summary>
    public override BuilderOverrideKind KeyOverwriteBehavior { get; }

    /// <summary>
    /// 
    /// </summary>
    public override IDatKeyValidator KeyValidator { get; }

    /// <summary>
    /// 
    /// </summary>
    public override IDatKeyNormalizer? KeyNormalizer { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="overwriteDuplicates"></param>
    /// <param name="services"></param>
    public EmpireAtWarMasterTextFileBuilder(bool overwriteDuplicates, IServiceProvider services) : base(services)
    {
        KeyOverwriteBehavior = overwriteDuplicates ? BuilderOverrideKind.Overwrite : BuilderOverrideKind.NoOverwrite;
        KeyValidator = Services.GetRequiredService<EmpireAtWarKeyValidator>();
        KeyNormalizer = Services.GetRequiredService<EmpireAtWarKeyNormalizer>();
    }
}