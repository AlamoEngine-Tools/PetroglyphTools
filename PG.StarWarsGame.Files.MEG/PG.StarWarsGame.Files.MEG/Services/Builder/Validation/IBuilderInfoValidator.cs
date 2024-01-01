using FluentValidation;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// A validator that check an instance of a <see cref="MegFileDataEntryBuilderInfo"/>
/// </summary>
public interface IBuilderInfoValidator : IValidator<MegFileDataEntryBuilderInfo>;