using System;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services.Builder.Normalization;
using PG.StarWarsGame.Files.DAT.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.DAT.Services.Builder;

/// <summary>
/// 
/// </summary>
public sealed class EmpireAtWarCreditsTextFileBuilder : DatBuilderBase
{
    /// <summary>
    /// 
    /// </summary>
    public override DatFileType TargetKeySortOrder => DatFileType.NotOrdered;

    /// <summary>
    /// 
    /// </summary>
    public override BuilderOverrideKind KeyOverwriteBehavior => BuilderOverrideKind.AllowDuplicate;

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
    /// <param name="services"></param>
    public EmpireAtWarCreditsTextFileBuilder(IServiceProvider services) : base(services)
    {
        KeyValidator = Services.GetRequiredService<EmpireAtWarKeyValidator>();
        KeyNormalizer = Services.GetRequiredService<EmpireAtWarKeyNormalizer>();
    }
}