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
        Assert.AreEqual(FileNameUtilities.FileNameValidationResult.Success,result);
    }

    [TestMethod]
    [DataRow(null, FileNameUtilities.FileNameValidationResult.NullOrEmpty)]
    [DataRow("", FileNameUtilities.FileNameValidationResult.NullOrEmpty)]
    [DataRow("     ", FileNameUtilities.FileNameValidationResult.LeadingOrTrailingWhiteSpace)]
    [DataRow("\u0160", FileNameUtilities.FileNameValidationResult.InvalidCharacter)]
    [DataRow("\0", FileNameUtilities.FileNameValidationResult.InvalidCharacter)]
    [DataRow("123\0", FileNameUtilities.FileNameValidationResult.InvalidCharacter)]
    [DataRow("123\t", FileNameUtilities.FileNameValidationResult.InvalidCharacter)]
    [DataRow("123\r", FileNameUtilities.FileNameValidationResult.InvalidCharacter)]
    [DataRow("123\n", FileNameUtilities.FileNameValidationResult.InvalidCharacter)]
    [DataRow("nameWithNonASCII_ö", FileNameUtilities.FileNameValidationResult.InvalidCharacter)]
    [DataRow("nameWithTrailingSpace ", FileNameUtilities.FileNameValidationResult.LeadingOrTrailingWhiteSpace)]
    [DataRow("   nameWithLeadingSpace", FileNameUtilities.FileNameValidationResult.LeadingOrTrailingWhiteSpace)]
    [DataRow("my\\path", FileNameUtilities.FileNameValidationResult.InvalidCharacter)]
    [DataRow("my/path", FileNameUtilities.FileNameValidationResult.InvalidCharacter)]
    [DataRow("illegalChar_<", FileNameUtilities.FileNameValidationResult.InvalidCharacter)]
    [DataRow("illegalChar_>", FileNameUtilities.FileNameValidationResult.InvalidCharacter)]
    [DataRow("illegalChar_|", FileNameUtilities.FileNameValidationResult.InvalidCharacter)]
    [DataRow("illegalChar_:", FileNameUtilities.FileNameValidationResult.InvalidCharacter)]
    [DataRow("illegalChar_*", FileNameUtilities.FileNameValidationResult.InvalidCharacter)]
    [DataRow("illegalChar_?", FileNameUtilities.FileNameValidationResult.InvalidCharacter)]
    [DataRow("illegalChar_?", FileNameUtilities.FileNameValidationResult.InvalidCharacter)]
    [DataRow(".", FileNameUtilities.FileNameValidationResult.TrailingPeriod)]
    [DataRow("..", FileNameUtilities.FileNameValidationResult.TrailingPeriod)]
    [DataRow("test....", FileNameUtilities.FileNameValidationResult.TrailingPeriod)]
    [DataRow("test..", FileNameUtilities.FileNameValidationResult.TrailingPeriod)]
    [DataRow("test.", FileNameUtilities.FileNameValidationResult.TrailingPeriod)]
    [DataRow("CON", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    [DataRow("con", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    [DataRow("cON", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    [DataRow("PRN", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    [DataRow("AUX", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    [DataRow("NUL", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    [DataRow("COM0", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    [DataRow("COM1", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    [DataRow("COM2", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    [DataRow("COM3", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    [DataRow("COM4", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    [DataRow("COM5", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    [DataRow("COM6", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    [DataRow("COM7", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    [DataRow("COM8", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    [DataRow("COM9", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    [DataRow("LPT0", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    [DataRow("LPT1", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    [DataRow("LPT2", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    [DataRow("LPT3", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    [DataRow("LPT4", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    [DataRow("LPT5", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    [DataRow("LPT6", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    [DataRow("LPT7", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    [DataRow("LPT8", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    [DataRow("LPT9", FileNameUtilities.FileNameValidationResult.WindowsReserved)]
    public void Test_IsValidFileName_InvalidFileNames(string fileName, FileNameUtilities.FileNameValidationResult expectedResult)
    {
        Assert.IsFalse(FileNameUtilities.IsValidFileName(fileName, out var result));
        Assert.AreEqual(expectedResult, result);
        
    }
}