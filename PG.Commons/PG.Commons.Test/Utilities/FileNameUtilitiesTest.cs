using AnakinRaW.CommonUtilities.FileSystem.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Utilities;

namespace PG.Commons.Test.Utilities;

[TestClass]
public class FileNameUtilitiesTest
{
    [TestMethod]
    [DataRow("123")]
    [DataRow("123.txt")]
    [DataRow("123..txt")]
    [DataRow("fileNameWithCase")]
    [DataRow("fileNameWith_underscore")]
    [DataRow("fileNameWith-hyphen")]
    [DataRow(".test")]
    [DataRow("LPT12")]
    [DataRow("COM12")]
    [DataRow("NUL.txt")] // Though it's not recommend by MS, it's actually allowed to use this name in explorer
    public void Test_IsValidFileName_CorrectFileNames(string fileName)
    {
        Assert.IsTrue(FileNameUtilities.IsValidFileName(fileName, out var result));
        Assert.AreEqual(FileNameValidationResult.Valid,result);
    }

    [TestMethod]
    // These are not allowed for PG specifically.
    [DataRow("\u0160", FileNameValidationResult.InvalidCharacter)]
    [DataRow("nameWithNonASCII_ö", FileNameValidationResult.InvalidCharacter)]
    // These are not allowed on Windows in general
    [DataRow(null, FileNameValidationResult.NullOrEmpty)]
    [DataRow("", FileNameValidationResult.NullOrEmpty)]
    [DataRow("     ", FileNameValidationResult.LeadingOrTrailingWhiteSpace)]
    [DataRow("\0", FileNameValidationResult.InvalidCharacter)]
    [DataRow("123\0", FileNameValidationResult.InvalidCharacter)]
    [DataRow("123\t", FileNameValidationResult.InvalidCharacter)]
    [DataRow("123\r", FileNameValidationResult.InvalidCharacter)]
    [DataRow("123\n", FileNameValidationResult.InvalidCharacter)]
    [DataRow("nameWithTrailingSpace ", FileNameValidationResult.LeadingOrTrailingWhiteSpace)]
    [DataRow("   nameWithLeadingSpace", FileNameValidationResult.LeadingOrTrailingWhiteSpace)]
    [DataRow("my\\path", FileNameValidationResult.InvalidCharacter)]
    [DataRow("my/path", FileNameValidationResult.InvalidCharacter)]
    [DataRow("illegalChar_<", FileNameValidationResult.InvalidCharacter)]
    [DataRow("illegalChar_>", FileNameValidationResult.InvalidCharacter)]
    [DataRow("illegalChar_|", FileNameValidationResult.InvalidCharacter)]
    [DataRow("illegalChar_:", FileNameValidationResult.InvalidCharacter)]
    [DataRow("illegalChar_*", FileNameValidationResult.InvalidCharacter)]
    [DataRow("illegalChar_?", FileNameValidationResult.InvalidCharacter)]
    [DataRow("illegalChar_?", FileNameValidationResult.InvalidCharacter)]
    [DataRow(".", FileNameValidationResult.TrailingPeriod)]
    [DataRow("..", FileNameValidationResult.TrailingPeriod)]
    [DataRow("test....", FileNameValidationResult.TrailingPeriod)]
    [DataRow("test..", FileNameValidationResult.TrailingPeriod)]
    [DataRow("test.", FileNameValidationResult.TrailingPeriod)]
    [DataRow("CON", FileNameValidationResult.SystemReserved)]
    [DataRow("con", FileNameValidationResult.SystemReserved)]
    [DataRow("cON", FileNameValidationResult.SystemReserved)]
    [DataRow("PRN", FileNameValidationResult.SystemReserved)]
    [DataRow("AUX", FileNameValidationResult.SystemReserved)]
    [DataRow("NUL", FileNameValidationResult.SystemReserved)]
    [DataRow("COM0", FileNameValidationResult.SystemReserved)]
    [DataRow("COM1", FileNameValidationResult.SystemReserved)]
    [DataRow("COM2", FileNameValidationResult.SystemReserved)]
    [DataRow("COM3", FileNameValidationResult.SystemReserved)]
    [DataRow("COM4", FileNameValidationResult.SystemReserved)]
    [DataRow("COM5", FileNameValidationResult.SystemReserved)]
    [DataRow("COM6", FileNameValidationResult.SystemReserved)]
    [DataRow("COM7", FileNameValidationResult.SystemReserved)]
    [DataRow("COM8", FileNameValidationResult.SystemReserved)]
    [DataRow("COM9", FileNameValidationResult.SystemReserved)]
    [DataRow("LPT0", FileNameValidationResult.SystemReserved)]
    [DataRow("LPT1", FileNameValidationResult.SystemReserved)]
    [DataRow("LPT2", FileNameValidationResult.SystemReserved)]
    [DataRow("LPT3", FileNameValidationResult.SystemReserved)]
    [DataRow("LPT4", FileNameValidationResult.SystemReserved)]
    [DataRow("LPT5", FileNameValidationResult.SystemReserved)]
    [DataRow("LPT6", FileNameValidationResult.SystemReserved)]
    [DataRow("LPT7", FileNameValidationResult.SystemReserved)]
    [DataRow("LPT8", FileNameValidationResult.SystemReserved)]
    [DataRow("LPT9", FileNameValidationResult.SystemReserved)]
    public void Test_IsValidFileName_InvalidFileNames(string fileName, FileNameValidationResult expectedResult)
    {
        Assert.IsFalse(FileNameUtilities.IsValidFileName(fileName, out var result));
        Assert.AreEqual(expectedResult, result);
        
    }
}