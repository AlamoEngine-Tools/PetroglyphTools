// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using PG.StarWarsGame.Components.Localisation.Languages;
using PG.StarWarsGame.Components.Localisation.Repository;
using PG.StarWarsGame.Components.Localisation.Repository.Translation;

namespace PG.StarWarsGame.Components.Localisation.IO.Formats.Builtin.Csv.Translation;

/// <inheritdoc />
public class TranslationCsvFormatter : CsvFormatterBase<ITranslationRepository>
{
    /// <inheritdoc />
    public override void Export(ITranslationRepository repository)
    {
        Param.ValidateAndThrow();
        Debug.Assert(Param.Directory != null, "Param.Directory != null");
        var dirPath = Param.Directory?.ToString() ?? throw new ArgumentNullException(nameof(Param.Directory));
        if (!FileSystem.Directory.Exists(dirPath))
        {
            FileSystem.Directory.CreateDirectory(dirPath);
        }

        var languages = new HashSet<IAlamoLanguageDefinition>();
        foreach (var translationItemRepository in repository.Values)
        {
            foreach (var definition in translationItemRepository.Keys)
            {
                languages.Add(definition);
            }
        }

        var tasks = new Task[languages.Count];
        var i = 0;
        foreach (var definition in languages)
        {
            tasks[i] = ExportAsync(dirPath, definition, repository.Values, Param.AlphabetizeKeys ?? false);
            i++;
        }

        Task.WhenAll(tasks);
    }

    private Task ExportAsync(string dirPath, IAlamoLanguageDefinition definition,
        IEnumerable<ITranslationItemRepository> repositoryValues, bool sorted)
    {
        return sorted
            ? ExportAsyncSorted(dirPath, definition, repositoryValues)
            : ExportAsyncUnsorted(dirPath, definition, repositoryValues);
    }

    private Task ExportAsyncUnsorted(string dirPath, IAlamoLanguageDefinition definition,
        IEnumerable<ITranslationItemRepository> repositoryValues)
    {
        using var writer = new StreamWriter(
            FileSystem.File.Open(FileSystem.Path.Combine(dirPath, Descriptor.ToFileName(this, definition)),
                FileMode.Create));
        foreach (var repositoryValue in repositoryValues)
        {
            if (!repositoryValue.TryRead(definition, out var item))
            {
                continue;
            }

            if ((item == null) || !item.Validate().IsValid)
            {
                continue;
            }

            writer.WriteLine(BuildLine(item));
        }

        writer.Close();
        return Task.CompletedTask;
    }

    private Task ExportAsyncSorted(string dirPath, IAlamoLanguageDefinition definition,
        IEnumerable<ITranslationItemRepository> repositoryValues)
    {
        var lines = new List<string>();
        foreach (var repositoryValue in repositoryValues)
        {
            if (!repositoryValue.TryRead(definition, out var item))
            {
                continue;
            }

            if ((item == null) || !item.Validate().IsValid)
            {
                continue;
            }

            lines.Add(BuildLine(item));
        }

        lines.Sort();
        using var writer = new StreamWriter(
            FileSystem.File.Open(FileSystem.Path.Combine(dirPath, Descriptor.ToFileName(this, definition)),
                FileMode.Create));
        foreach (var line in lines)
        {
            writer.WriteLine(line);
        }

        writer.Close();
        return Task.CompletedTask;
    }


    private string BuildLine(ITranslationItem value)
    {
        var b = new StringBuilder();
        if (Param.KeyWrapper != null)
        {
            b.Append(Param.KeyWrapper)
                .Append(value.Key)
                .Append(Param.KeyWrapper);
        }
        else
        {
            b.Append(value.Key);
        }

        b.Append(Param.Separator);
        if (Param.ValueWrapper != null)
        {
            b.Append(Param.ValueWrapper)
                .Append(value.Value)
                .Append(Param.ValueWrapper);
        }
        else
        {
            b.Append(value.Value);
        }

        return b.ToString();
    }

    /// <inheritdoc />
    public override ITranslationRepository Import()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public TranslationCsvFormatter(
        ILocalisationFormatDescriptor<CsvFormatterProcessingInstructionsParam, ITranslationRepository> descriptor,
        IServiceProvider services) : base(descriptor, services)
    {
    }
}