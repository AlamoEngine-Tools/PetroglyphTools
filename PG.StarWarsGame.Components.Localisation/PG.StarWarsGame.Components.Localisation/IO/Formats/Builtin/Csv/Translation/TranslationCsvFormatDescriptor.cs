// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Components.Localisation.Languages;
using PG.StarWarsGame.Components.Localisation.Repository.Translation;

namespace PG.StarWarsGame.Components.Localisation.IO.Formats.Builtin.Csv.Translation;

/// <inheritdoc />
public class TranslationCsvFormatDescriptor : FormatDescriptorBase<CsvFormatterProcessingInstructionsParam,
    ITranslationRepository>
{
    private static string FallbackFileExtension => "csv";

    /// <inheritdoc />
    public TranslationCsvFormatDescriptor(IServiceProvider services) : base(services)
    {
    }

    /// <inheritdoc />
    public override string ToFileName(
        ILocalisationFormatter<CsvFormatterProcessingInstructionsParam, ITranslationRepository> formatter,
        IAlamoLanguageDefinition definition)
    {
        var fileNameBase = formatter.Param.FileName ?? FallbackFileNameBase;
        var fileExtension = formatter.Param.FileExtension ?? FallbackFileExtension;
        return $"{fileNameBase}.{definition.Culture.TwoLetterISOLanguageName.ToLower()}.{fileExtension}";
    }

    /// <inheritdoc />
    public override ILocalisationFormatter<CsvFormatterProcessingInstructionsParam, ITranslationRepository>
        CreateFormatter(CsvFormatterProcessingInstructionsParam param)
    {
        return new TranslationCsvFormatter(this, Services)
            .WithProcessingInstructions(param);
    }
}