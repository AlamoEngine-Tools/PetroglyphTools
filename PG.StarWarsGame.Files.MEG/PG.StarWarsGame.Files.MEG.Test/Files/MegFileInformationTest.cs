using System;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Test.Files;

[TestClass]
public class MegFileInformationTest
{
    //[TestMethod]
    //public void Test_KeyHandling()
    //{
    //    var keyIv = new byte[16];
    //    RandomNumberGenerator.Create().GetNonZeroBytes(keyIv);

    //    var param = new MegFileInformation { FilePath = "test.meg", FileVersion = MegFileVersion.V3, IV = keyIv, Key = keyIv };
       
    //    // Alter the array reference;
    //    keyIv[0] = 0;
    //    keyIv[1] = 0;

    //    CollectionAssert.AreNotEqual(keyIv, param.IV);
    //    CollectionAssert.AreNotEqual(keyIv, param.Key);

    //    var iv = param.IV;
    //    iv[0] = 0;
    //    iv[1] = 0;

    //    var key = param.Key;
    //    key[0] = 0;
    //    key[1] = 0;

    //    CollectionAssert.AreNotEqual(iv, param.IV);
    //    CollectionAssert.AreNotEqual(key, param.Key);

    //    param.Dispose();

    //    Assert.IsNull(param.Key);
    //    Assert.IsNull(param.IV);
    //}

    //[TestMethod]
    //public void Test_Ctor_Ctor_InvalidKeySizes()
    //{
    //    var baseParam = new MegFileInformation { FilePath = "test.meg", FileVersion = MegFileVersion.V3 };
    //    var model = new Mock<IMegArchive>().Object;

    //    var invalidSize = new byte[4];
    //    var validSize = new byte[16];
    //    RandomNumberGenerator.Create().GetNonZeroBytes(invalidSize);
    //    RandomNumberGenerator.Create().GetNonZeroBytes(validSize);


    //    var param1 = baseParam with { Key = invalidSize, IV = validSize };
    //    var param2 = baseParam with { Key = validSize, IV = invalidSize };
    //    var param3 = baseParam with { Key = validSize, IV = validSize, FileVersion = MegFileVersion.V2 };

    //    Assert.ThrowsException<ArgumentException>(() => new MegFile(model, param1, _serviceProvider.Object));
    //    Assert.ThrowsException<ArgumentException>(() => new MegFile(model, param2, _serviceProvider.Object));
    //    Assert.ThrowsException<ArgumentException>(() => new MegFile(model, param3, _serviceProvider.Object));
    //}
}