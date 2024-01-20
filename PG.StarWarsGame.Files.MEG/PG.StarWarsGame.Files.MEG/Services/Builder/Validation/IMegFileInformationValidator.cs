using FluentValidation;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// A validator that check an instance of a <see cref="MegBuilderFileInformationValidationData"/>
/// </summary>
public interface IMegFileInformationValidator : IValidator<MegBuilderFileInformationValidationData>;