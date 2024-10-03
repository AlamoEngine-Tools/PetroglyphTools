using System;
using System.Text;
using AnakinRaW.CommonUtilities.FileSystem.Normalization;
using PG.Commons.Utilities;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

/// <summary>
/// Normalizes a path in the same way the Empire at War Alamo engine normalizes meg entry paths (e.g, for file lookups).
/// </summary>
public sealed class EmpireAtWarMegDataEntryPathNormalizer(IServiceProvider serviceProvider)
    : PetroglyphDataEntryPathNormalizer(serviceProvider)
{
    private static readonly PathNormalizeOptions PetroglyphNormalizeOptions = new()
    {
        UnifyDirectorySeparators = true,
        UnifySeparatorKind = DirectorySeparatorKind.Windows,
        UnifyCase = UnifyCasingKind.UpperCaseForce
    };

    /// <inheritdoc />
    public override void Normalize(ReadOnlySpan<char> filePath, ref ValueStringBuilder stringBuilder)
    {
        var normalizing = new ValueStringBuilder(stackalloc char[260]);
        normalizing.EnsureCapacity(filePath.Length);

        try
        {
            var ln = PathNormalizer.Normalize(filePath, normalizing.RawChars, PetroglyphNormalizeOptions);
            var normalized = normalizing.AsSpan(0, ln);

            SplitPath(normalized, out var path, out var file);

            if (path.Length > 0)
            {
                stringBuilder.Append(path);
                stringBuilder.Append('\\');
            }
            stringBuilder.Append(file);
        }
        finally
        {
            normalizing.Dispose();
        }
    }

    private static void SplitPath(ReadOnlySpan<char> input, out ReadOnlySpan<char> pathPart, out ReadOnlySpan<char> filePart)
    {
        var fileStartIndex = input.LastIndexOf('\\');
        if (fileStartIndex == -1)
        {
            filePart = input;
            pathPart = ReadOnlySpan<char>.Empty;
            return;
        }

        var pathStart = 0;
        var colon = input.IndexOf(':');
        if (colon != -1)
            pathStart = colon;

        if (input[pathStart] == '.')
            ++pathStart;
        if (input[pathStart] == '\\')
            ++pathStart;


        var length = (int)Math.Min(input.Length - pathStart, (uint)(fileStartIndex - pathStart));
        pathPart = input.Slice(pathStart, length);

        filePart = input.Slice(fileStartIndex + 1);
    }
}