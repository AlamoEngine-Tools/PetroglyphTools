using System;
using System.IO.Abstractions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// 
/// </summary>
public abstract class PetroglyphMegDataEntryValidator : AbstractValidator<MegFileDataEntryBuilderInfo>, IBuilderInfoValidator
{
    /// <summary>
    /// 
    /// </summary>
    protected IFileSystem FileSystem { get; }

    /// <summary>
    /// 
    /// </summary>
    protected PetroglyphMegDataEntryValidator(IServiceProvider serviceProvider)
    {
        FileSystem = serviceProvider.GetRequiredService<IFileSystem>();

        RuleFor(x => x.FilePath);
    }
}