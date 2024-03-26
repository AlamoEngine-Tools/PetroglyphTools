using System;
using PG.Commons.Services.Builder;

namespace PG.StarWarsGame.Files.DAT.Services.Builder.Normalization;

/// <summary>
/// 
/// </summary>
public class EmpireAtWarKeyNormalizer : BuilderEntryNormalizerBase<string>, IDatKeyNormalizer
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceProvider"></param>
    public EmpireAtWarKeyNormalizer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override string Normalize(string entry)
    {
        throw new NotImplementedException();
    }
}