using System;
using System.Buffers;
using System.Text;
using AnakinRaW.CommonUtilities.FileSystem.Normalization;
using PG.StarWarsGame.Files.MEG.Binary;
#if NETSTANDARD2_0 || NETFRAMEWORK
using System.Runtime.InteropServices;
#endif

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

/// <summary>
/// Normalizes a path in the same way the Empire at War Alamo engine normalizes meg entry paths (e.g, for file lookups).
/// </summary>
public sealed class EmpireAtWarMegDataEntryPathNormalizer : PetroglyphDataEntryPathNormalizer
{
    /// <summary>
    /// Returns a singleton instance of the <see cref="EmpireAtWarMegDataEntryPathNormalizer"/>.
    /// </summary>
    public static readonly EmpireAtWarMegDataEntryPathNormalizer Instance = new();

    private EmpireAtWarMegDataEntryPathNormalizer()
    {
    }

    private static readonly PathNormalizeOptions PetroglyphNormalizeOptions = new()
    {
        UnifyDirectorySeparators = true,
        UnifySeparatorKind = DirectorySeparatorKind.Windows,
        UnifyCase = UnifyCasingKind.UpperCaseForce
    };

    /// <inheritdoc />
    public override string Normalize(ReadOnlySpan<char> filePath)
    {
        if (filePath.Length == 0)
            return string.Empty;

        char[]? pooledCharArray = null;
        try
        {
            var buffer = filePath.Length > 265
                ? pooledCharArray = ArrayPool<char>.Shared.Rent(filePath.Length)
                : stackalloc char[filePath.Length];

            var normalizedLength = PathNormalizer.Normalize(filePath, buffer, PetroglyphNormalizeOptions);
            var normalized = buffer.Slice(0, normalizedLength);

            SplitPath(normalized, out var path, out var file);

            var sb = new StringBuilder(MegFileConstants.EawMaxEntryPathLength);
            if (path.Length > 0)
            {
                AppendRosToSb(path, sb);
                sb.Append('\\');
            }

            AppendRosToSb(file, sb);
            return sb.ToString();
        }
        finally
        {
            if (pooledCharArray is not null)
                ArrayPool<char>.Shared.Return(pooledCharArray);
        }
    }


    private static void AppendRosToSb(ReadOnlySpan<char> value, StringBuilder sb)
    {
        if (value.Length <= 0)
            return;
#if NETSTANDARD2_0 || NETFRAMEWORK
        unsafe
        {
            fixed (char* valueChars = &MemoryMarshal.GetReference(value))
                sb.Append(valueChars, value.Length);
        }
#else
        sb.Append(value);
#endif
    }


    /// <inheritdoc />
    protected override int Normalize(ReadOnlySpan<char> filePath, Span<char> destination)
    {
        if (filePath.Length == 0)
            return 0;

        char[]? pooledCharArray = null;
        try
        {
            var normalizationBuffer = destination.Length > 265
                ? pooledCharArray = ArrayPool<char>.Shared.Rent(destination.Length)
                : stackalloc char[destination.Length];

            var normalizedLength = PathNormalizer.Normalize(filePath, normalizationBuffer, PetroglyphNormalizeOptions);
            var normalized = normalizationBuffer.Slice(0, normalizedLength);

            SplitPath(normalized, out var path, out var file);

            var pos = 0;
            if (path.Length > 0)
            {
                path.CopyTo(destination);
                destination[path.Length] = '\\';

                pos = path.Length + 1;
            }

            var fileSpan = destination.Slice(pos);
            file.CopyTo(fileSpan);

            return pos + file.Length;
        }
        finally
        {
            if (pooledCharArray is not null)
                ArrayPool<char>.Shared.Return(pooledCharArray);
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