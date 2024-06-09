using System;
using AnakinRaW.CommonUtilities.FileSystem.Validation;
using PG.Commons.Utilities;
using Xunit;

namespace PG.Commons.Test.Utilities;

public class FileNameUtilitiesTest
{
    [Theory]
    [InlineData("123")]
    [InlineData("123.txt")]
    [InlineData("123..txt")]
    [InlineData("fileNameWithCase")]
    [InlineData("fileNameWith_underscore")]
    [InlineData("fileNameWith-hyphen")]
    [InlineData(".test")]
    [InlineData("LPT12")]
    [InlineData("COM12")]
    [InlineData("NUL.txt")] // Though it's not recommend by MS, it's actually allowed to use this name in explorer
    public void Test_IsValidFileName_CorrectFileNames(string fileName)
    {
        Assert.True(FileNameUtilities.IsValidFileName(fileName.AsSpan(), out var result));
        Assert.Equal(FileNameValidationResult.Valid,result);
    }

    [Theory]
    // These are not allowed for PG specifically.
    [InlineData("\u0160", FileNameValidationResult.InvalidCharacter)]
    [InlineData("nameWithNonASCII_ö", FileNameValidationResult.InvalidCharacter)]
    // These are not allowed on Windows in general
    [InlineData(null, FileNameValidationResult.NullOrEmpty)]
    [InlineData("", FileNameValidationResult.NullOrEmpty)]
    [InlineData("     ", FileNameValidationResult.LeadingOrTrailingWhiteSpace)]
    [InlineData("\0", FileNameValidationResult.InvalidCharacter)]
    [InlineData("123\0", FileNameValidationResult.InvalidCharacter)]
    [InlineData("123\t", FileNameValidationResult.InvalidCharacter)]
    [InlineData("123\r", FileNameValidationResult.InvalidCharacter)]
    [InlineData("123\n", FileNameValidationResult.InvalidCharacter)]
    [InlineData("nameWithTrailingSpace ", FileNameValidationResult.LeadingOrTrailingWhiteSpace)]
    [InlineData("   nameWithLeadingSpace", FileNameValidationResult.LeadingOrTrailingWhiteSpace)]
    [InlineData("my\\path", FileNameValidationResult.InvalidCharacter)]
    [InlineData("my/path", FileNameValidationResult.InvalidCharacter)]
    [InlineData("illegalChar_<", FileNameValidationResult.InvalidCharacter)]
    [InlineData("illegalChar_>", FileNameValidationResult.InvalidCharacter)]
    [InlineData("illegalChar_|", FileNameValidationResult.InvalidCharacter)]
    [InlineData("illegalChar_:", FileNameValidationResult.InvalidCharacter)]
    [InlineData("illegalChar_*", FileNameValidationResult.InvalidCharacter)]
    [InlineData("illegalChar_?", FileNameValidationResult.InvalidCharacter)]
    [InlineData(".", FileNameValidationResult.TrailingPeriod)]
    [InlineData("..", FileNameValidationResult.TrailingPeriod)]
    [InlineData("test....", FileNameValidationResult.TrailingPeriod)]
    [InlineData("test..", FileNameValidationResult.TrailingPeriod)]
    [InlineData("test.", FileNameValidationResult.TrailingPeriod)]
    [InlineData("CON", FileNameValidationResult.SystemReserved)]
    [InlineData("con", FileNameValidationResult.SystemReserved)]
    [InlineData("cON", FileNameValidationResult.SystemReserved)]
    [InlineData("PRN", FileNameValidationResult.SystemReserved)]
    [InlineData("AUX", FileNameValidationResult.SystemReserved)]
    [InlineData("NUL", FileNameValidationResult.SystemReserved)]
    [InlineData("COM0", FileNameValidationResult.SystemReserved)]
    [InlineData("COM1", FileNameValidationResult.SystemReserved)]
    [InlineData("COM2", FileNameValidationResult.SystemReserved)]
    [InlineData("COM3", FileNameValidationResult.SystemReserved)]
    [InlineData("COM4", FileNameValidationResult.SystemReserved)]
    [InlineData("COM5", FileNameValidationResult.SystemReserved)]
    [InlineData("COM6", FileNameValidationResult.SystemReserved)]
    [InlineData("COM7", FileNameValidationResult.SystemReserved)]
    [InlineData("COM8", FileNameValidationResult.SystemReserved)]
    [InlineData("COM9", FileNameValidationResult.SystemReserved)]
    [InlineData("LPT0", FileNameValidationResult.SystemReserved)]
    [InlineData("LPT1", FileNameValidationResult.SystemReserved)]
    [InlineData("LPT2", FileNameValidationResult.SystemReserved)]
    [InlineData("LPT3", FileNameValidationResult.SystemReserved)]
    [InlineData("LPT4", FileNameValidationResult.SystemReserved)]
    [InlineData("LPT5", FileNameValidationResult.SystemReserved)]
    [InlineData("LPT6", FileNameValidationResult.SystemReserved)]
    [InlineData("LPT7", FileNameValidationResult.SystemReserved)]
    [InlineData("LPT8", FileNameValidationResult.SystemReserved)]
    [InlineData("LPT9", FileNameValidationResult.SystemReserved)]
    public void Test_IsValidFileName_InvalidFileNames(string? fileName, FileNameValidationResult expectedResult)
    {
        Assert.False(FileNameUtilities.IsValidFileName(fileName.AsSpan(), out var result));
        Assert.Equal(expectedResult, result);
        
    }
}