// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Holder;
using PG.StarWarsGame.Files.MEG.Holder.V1;

namespace PG.StarWarsGame.Files.MEG.Test.Holder.V1
{
    [TestClass]
    public class MegFileHolderTest
    {
        private static MegFileHolder s_holder;
        private const string FILE_NAME_1 = "my_file.txt";
        private const string FILE_NAME_2 = "another_file.txt";

        [TestInitialize]
        public void SetUp()
        {
            s_holder = new MegFileHolder("test", "test");
            s_holder.Content.Add(new MegFileDataEntry(FILE_NAME_1, "C:/my_absolute/my_file.txt"));
            s_holder.Content.Add(new MegFileDataEntry(FILE_NAME_2, "C:/my_absolute/another_file.txt"));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("    ")]
        [DataRow("  \t\r\n  ")]
        [DataRow("someFileMatch")]
        public void TryGetFirstMegFileDataEntryWithMatchingName_Test__NoEntryFound(string fileMatch)
        {
            Assert.IsFalse(s_holder.TryGetFirstMegFileDataEntryWithMatchingName(fileMatch, out MegFileDataEntry _));
        }
        [TestMethod]
        [DataRow(FILE_NAME_1)]
        [DataRow(FILE_NAME_2)]
        [DataRow("FILE")]
        public void TryGetFirstMegFileDataEntryWithMatchingName_Test__EntryFound(string fileMatch)
        {
            Assert.IsTrue(s_holder.TryGetFirstMegFileDataEntryWithMatchingName(fileMatch, out MegFileDataEntry _));
        }
        
        
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("    ")]
        [DataRow("  \t\r\n  ")]
        [DataRow("someFileMatch")]
        public void TryGetAllMegFileDataEntriesWithMatchingName_Test__NoEntryFound(string fileMatch)
        {
            Assert.IsFalse(s_holder.TryGetAllMegFileDataEntriesWithMatchingName(fileMatch, out IList<MegFileDataEntry> _));
        }
        [TestMethod]
        [DataRow(FILE_NAME_1)]
        [DataRow(FILE_NAME_2)]
        [DataRow("FILE")]
        public void TryGetAllMegFileDataEntriesWithMatchingName_Test__EntryFound(string fileMatch)
        {
            Assert.IsTrue(s_holder.TryGetAllMegFileDataEntriesWithMatchingName(fileMatch, out IList<MegFileDataEntry> _));
        }
    }
}
