using FluentValidation;

namespace PG.StarWarsGame.Files.DAT.Services.Builder.Validation;

/// <summary>
/// 
/// </summary>
public class EmpireAtWarKeyValidator : AbstractValidator<string>, IDatKeyValidator
{
    /// <summary>
    /// 
    /// </summary>
    public EmpireAtWarKeyValidator()
    {

    }
}