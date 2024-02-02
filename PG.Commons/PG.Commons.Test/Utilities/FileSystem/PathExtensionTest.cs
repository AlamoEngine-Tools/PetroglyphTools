using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Utilities.FileSystem;
using PG.Testing;

namespace PG.Commons.Test.Utilities.FileSystem;

// Based on https://github.com/dotnet/roslyn/ and https://github.com/NuGet/NuGet.Client/
[TestClass]
public class PathExtensionTest
{
    // Using the actual file system here since we are not modifying it.
    // Also, we want to assure that everything works on the real system,
    // not that an arbitrary test implementation works.
    private readonly IFileSystem _fileSystem = new System.IO.Abstractions.FileSystem();

    [PlatformSpecificTestMethod(TestPlatformIdentifier.Windows)]
    [DataRow("", "")]
    [DataRow("/", "/")]
    [DataRow("a", "a\\")]
    [DataRow("\\", "\\")]
    [DataRow("a/b", "a/b/")]
    [DataRow("a\\b", "a\\b\\")]
    [DataRow("a\\b/c", "a\\b/c\\")]
    [DataRow("a/b\\", "a/b\\")]
    [DataRow("a\\b/", "a\\b/")]
    public void Test_EnsureTrailingSeparator_Windows(string input, string expected)
    {
        Assert.AreEqual(expected, _fileSystem.Path.EnsureTrailingSeparator(input));
    }

    [PlatformSpecificTestMethod(TestPlatformIdentifier.Linux)]
    [DataRow("", "")]
    [DataRow("/", "/")]
    [DataRow("\\", "\\/")]
    [DataRow("a", "a/")]
    [DataRow("a/b", "a/b/")]
    [DataRow("a/b\\", "a/b\\/")]
    [DataRow("a\\b\\", "a\\b\\/")]
    public void Test_EnsureTrailingSeparator_Linux(string input, string expected)
    {
        Assert.AreEqual(expected, _fileSystem.Path.EnsureTrailingSeparator(input));
    }

    [TestMethod]
    [DataRow("", false)]
    [DataRow(null, false)]
    public void Test_HasTrailingPathSeparator(string? input, bool expected)
    {
        if (input is null)
            Assert.ThrowsException<ArgumentNullException>(() => _fileSystem.Path.HasTrailingPathSeparator(input));
        else
            Assert.AreEqual(expected, _fileSystem.Path.HasTrailingPathSeparator(input));
        Assert.AreEqual(expected, _fileSystem.Path.HasTrailingPathSeparator(input.AsSpan()));
    }

    [PlatformSpecificTestMethod(TestPlatformIdentifier.Windows)]
    [DataRow("/", true)]
    [DataRow("\\", true)]
    [DataRow("a", false)]
    [DataRow("a/", true)]
    [DataRow("a\\", true)]
    [DataRow("a\\b", false)]
    [DataRow("a/b", false)]
    [DataRow("a/b\\", true)]
    [DataRow("a\\b/", true)]
    public void Test_HasTrailingPathSeparator_Windows(string input, bool expected)
    {
        Assert.AreEqual(expected, _fileSystem.Path.HasTrailingPathSeparator(input));
        Assert.AreEqual(expected, _fileSystem.Path.HasTrailingPathSeparator(input.AsSpan()));
    }

    [PlatformSpecificTestMethod(TestPlatformIdentifier.Linux)]
    [DataRow("/", true)]
    [DataRow("\\", false)]
    [DataRow("a", false)]
    [DataRow("a/", true)]
    [DataRow("a\\", false)]
    [DataRow("a\\b", false)]
    [DataRow("a/b", false)]
    [DataRow("a/b\\", false)]
    [DataRow("a\\b/", true)]
    [DataRow("a\\b\\/", true)]
    [DataRow("a\\b/\\", false)]
    public void Test_HasTrailingPathSeparator_Linux(string input, bool expected)
    {
        Assert.AreEqual(expected, _fileSystem.Path.HasTrailingPathSeparator(input));
        Assert.AreEqual(expected, _fileSystem.Path.HasTrailingPathSeparator(input.AsSpan()));
    }

    [PlatformSpecificTestMethod(TestPlatformIdentifier.Windows)]
    [DynamicData(nameof(NormalizeTestDataSource), DynamicDataSourceType.Method)]
    public void Test_Normalize_Windows(NormalizeTestData testData)
    {
        Assert.AreEqual(testData.ExpectedWindows, _fileSystem.Path.Normalize(testData.Input, testData.Options));
    }

    [PlatformSpecificTestMethod(TestPlatformIdentifier.Linux)]
    [DynamicData(nameof(NormalizeTestDataSource), DynamicDataSourceType.Method)]
    public void Test_Normalize_Linux(NormalizeTestData testData)
    {
        Assert.AreEqual(testData.ExpectedLinux, _fileSystem.Path.Normalize(testData.Input, testData.Options));
    }

    [PlatformSpecificTestMethod(TestPlatformIdentifier.Windows)]
    [DataRow(@"C:\", @"C:\", @".")]
    [DataRow(@"C:\a", @"C:\a\", @".")]
    [DataRow(@"C:\A", @"C:\a\", @".")]
    [DataRow(@"C:\a\", @"C:\a", @".")]
    [DataRow(@"C:\", @"C:\b", @"b")]
    [DataRow(@"C:\a", @"C:\b", @"..\b")]
    [DataRow(@"C:\a", @"C:\b\", @"..\b\")]
    [DataRow(@"C:\a\b", @"C:\a", @"..")]
    [DataRow(@"C:\a\b", @"C:\a\", @"..")]
    [DataRow(@"C:\a\b\", @"C:\a", @"..")]
    [DataRow(@"C:\a\b\", @"C:\a\", @"..")]
    [DataRow(@"C:\a\b\c", @"C:\a\b", @"..")]
    [DataRow(@"C:\a\b\c", @"C:\a\b\", @"..")]
    [DataRow(@"C:\a\b\c", @"C:\a", @"..\..")]
    [DataRow(@"C:\a\b\c", @"C:\a\", @"..\..")]
    [DataRow(@"C:\a\b\c\", @"C:\a\b", @"..")]
    [DataRow(@"C:\a\b\c\", @"C:\a\b\", @"..")]
    [DataRow(@"C:\a\b\c\", @"C:\a", @"..\..")]
    [DataRow(@"C:\a\b\c\", @"C:\a\", @"..\..")]
    [DataRow(@"C:\a\", @"C:\b", @"..\b")]
    [DataRow(@"C:\a", @"C:\a\b", @"b")]
    [DataRow(@"C:\a", @"C:\A\b", @"b")]
    [DataRow(@"C:\a", @"C:\b\c", @"..\b\c")]
    [DataRow(@"C:\a\", @"C:\a\b", @"b")]
    [DataRow(@"C:\", @"D:\", @"D:\")]
    [DataRow(@"C:\", @"D:\b", @"D:\b")]
    [DataRow(@"C:\", @"D:\b\", @"D:\b\")]
    [DataRow(@"C:\a", @"D:\b", @"D:\b")]
    [DataRow(@"C:\a\", @"D:\b", @"D:\b")]
    [DataRow(@"C:\ab", @"C:\a", @"..\a")]
    [DataRow(@"C:\a", @"C:\ab", @"..\ab")]
    [DataRow(@"C:\", @"\\LOCALHOST\Share\b", @"\\LOCALHOST\Share\b")]
    [DataRow(@"\\LOCALHOST\Share\a", @"\\LOCALHOST\Share\b", @"..\b")]
    public void Test_GetRelativePathEx_Windows(string root, string path, string expected)
    {
        var result = _fileSystem.Path.GetRelativePathEx(root, path);
        Assert.AreEqual(expected, result);

        Assert.AreEqual(
            _fileSystem.Path.GetFullPath(path),
            _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(_fileSystem.Path.GetFullPath(root), result)));
    }

    [PlatformSpecificTestMethod(TestPlatformIdentifier.Linux)]
    [DataRow(@"/", @"/", @".")]
    [DataRow(@"/a", @"/a/", @".")]
    [DataRow(@"/a/", @"/a", @".")]
    [DataRow(@"/", @"/b", @"b")]
    [DataRow(@"/a", @"/b", @"../b")]
    [DataRow(@"/a/", @"/b", @"../b")]
    [DataRow(@"/a", @"/a/b", @"b")]
    [DataRow(@"/a", @"/b/c", @"../b/c")]
    [DataRow(@"/a/", @"/a/b", @"b")]
    [DataRow(@"/ab", @"/a", @"../a")]
    [DataRow(@"/a", @"/ab", @"../ab")]
    [DataRow(@"/a", @"/A/", @"../A/")]
    [DataRow(@"/a/", @"/A", @"../A")]
    [DataRow(@"/a/", @"/A/b", @"../A/b")]
    public void Test_GetRelativePathEx_Linux(string root, string path, string expected)
    {
        var result = _fileSystem.Path.GetRelativePathEx(root, path);
        Assert.AreEqual(expected, result);

        Assert.AreEqual(
            _fileSystem.Path.GetFullPath(path),
            _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(_fileSystem.Path.GetFullPath(root), result)));
    }


    [PlatformSpecificTestMethod(TestPlatformIdentifier.Windows)]
    [DataRow(null, false)]
    [DataRow("", false)]
    [DataRow("  ", false)]
    [DataRow(".", false)]
    [DataRow("C", false)]
    [DataRow("C:", false)]
    [DataRow(@"C:\", true)]
    [DataRow("C:/", true)]
    [DataRow("C:/test", true)]
    [DataRow(@"C:\", true)]
    [DataRow(@"C:\test", true)]
    [DataRow("/", false)]
    [DataRow(@"\", false)]
    [DataRow(@"\\Server\Share", false)]
    [DataRow(@"\\?\C:\", false)]
    public void Test_IsDriveAbsolute_Windows(string path, bool expected)
    {
        Assert.AreEqual(expected, _fileSystem.Path.IsDriveAbsolute(path));
    }

    [PlatformSpecificTestMethod(TestPlatformIdentifier.Linux)]
    [DataRow("", false)]
    [DataRow(null, false)]
    [DataRow("  ", false)]
    [DataRow(".", false)]
    [DataRow("C", false)]
    [DataRow("C:", false)]
    [DataRow(@"C:\", false)]
    [DataRow("C:/", false)]
    [DataRow("C:/test", false)]
    [DataRow(@"C:\", false)]
    [DataRow(@"C:\test", false)]
    [DataRow("/", false)]
    [DataRow(@"\", false)]
    [DataRow(@"\\Server\Share", false)]
    [DataRow(@"\\?\C:\", false)]
    public void Test_IsDriveAbsolute_Linux(string path, bool expected)
    {
        Assert.AreEqual(expected, _fileSystem.Path.IsDriveAbsolute(path));
    }


    [PlatformSpecificTestMethod(TestPlatformIdentifier.Windows)]
    [DataRow(null, false)]
    [DataRow("", false)]
    [DataRow("  ", false)]
    [DataRow("C", false)]
    [DataRow(@"C:\", false)]
    [DataRow("C:/", false)]
    [DataRow("C:/test", false)]
    [DataRow(@"C:\", false)]
    [DataRow(@"C:\test", false)]
    [DataRow("/", false)]
    [DataRow(@"\", false)]
    [DataRow(@"\\Server\Share", false)]
    [DataRow(@"\\?\C:\", false)]
    [DataRow("C:", true)]
    [DataRow("C:test", true)]
    [DataRow(@"C:test/a\a", true)]
    public void Test_IsDriveRelative_Windows(string path, bool expected)
    {
        Assert.AreEqual(expected, _fileSystem.Path.IsDriveRelative(path));
    }

    [PlatformSpecificTestMethod(TestPlatformIdentifier.Linux)]
    [DataRow(null, false)]
    [DataRow("", false)]
    [DataRow("  ", false)]
    [DataRow("C", false)]
    [DataRow(@"C:\", false)]
    [DataRow("C:/", false)]
    [DataRow("C:/test", false)]
    [DataRow(@"C:\", false)]
    [DataRow(@"C:\test", false)]
    [DataRow("/", false)]
    [DataRow(@"\", false)]
    [DataRow(@"\\Server\Share", false)]
    [DataRow(@"\\?\C:\", false)]
    [DataRow("C:", false)]
    [DataRow("C:test", false)]
    [DataRow(@"C:test/a\a", false)]
    public void Test_IsDriveRelative_Linux(string path, bool expected)
    {
        Assert.AreEqual(expected, _fileSystem.Path.IsDriveRelative(path));
    }



    public static IEnumerable<object[]> NormalizeTestDataSource()
    {
        yield return
        [
            new NormalizeTestData { Input = "a/b\\C", ExpectedLinux = "a/b\\C", ExpectedWindows = "a/b\\C", Options = new PathNormalizeOptions() }
        ];
        yield return
        [
            new NormalizeTestData
            {
                Input = "a/b\\C", ExpectedLinux = "a/b/C", ExpectedWindows = "a\\b\\C", Options =
                    new PathNormalizeOptions
                    {
                        UnifySlashes = true
                    }
            }
        ];
        yield return
        [
            new NormalizeTestData
            {
                Input = "a/b\\C", ExpectedLinux = "a/b/C", ExpectedWindows = "a/b/C", Options =
                    new PathNormalizeOptions
                    {
                        UnifySlashes = true,
                        SeparatorKind = PathSeparatorKind.Linux
                    }
            }
        ];
        yield return
        [
            new NormalizeTestData
            {
                Input = "a/b\\C", ExpectedLinux = "a\\b\\C", ExpectedWindows = "a\\b\\C", Options =
                    new PathNormalizeOptions
                    {
                        UnifySlashes = true,
                        SeparatorKind = PathSeparatorKind.Windows
                    }
            }
        ];
        yield return
        [
            new NormalizeTestData
            {
                Input = "a/b\\C", ExpectedLinux = "a/b\\C", ExpectedWindows = "a/b\\c", Options =
                    new PathNormalizeOptions
                    {
                        UnifyCase = UnifyCasingKind.LowerCase
                    }
            }
        ];
        yield return
        [
            new NormalizeTestData
            {
                Input = "a/b\\C", ExpectedLinux = "a/b\\c", ExpectedWindows = "a/b\\c", Options =
                    new PathNormalizeOptions
                    {
                        UnifyCase = UnifyCasingKind.LowerCaseForce
                    }
            }
        ];
        yield return
        [
            new NormalizeTestData
            {
                Input = "a/b\\C", ExpectedLinux = "a/b\\C", ExpectedWindows = "A/B\\C", Options =
                    new PathNormalizeOptions
                    {
                        UnifyCase = UnifyCasingKind.UpperCase
                    }
            }
        ];
        yield return
        [
            new NormalizeTestData
            {
                Input = "a/b\\C", ExpectedLinux = "A/B\\C", ExpectedWindows = "A/B\\C", Options =
                    new PathNormalizeOptions
                    {
                        UnifyCase = UnifyCasingKind.UpperCaseForce
                    }
            }
        ];
        yield return
        [
            new NormalizeTestData
            {
                Input = "a/b\\C//\\", ExpectedLinux = "a/b\\C", ExpectedWindows = "a/b\\C", Options =
                    new PathNormalizeOptions
                    {
                        TrimTrailingSeparator = true
                    }
            }
        ];
    }


    public record NormalizeTestData
    {
        public required string Input { get; init; }

        public string ExpectedWindows { get; init; }

        public string ExpectedLinux { get; init; }

        public required PathNormalizeOptions Options { get; init; }
    }
}