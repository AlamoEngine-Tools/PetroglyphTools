using System;
using System.Collections.Generic;
using System.Linq;
using DotNet.Globbing;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Test.Data;

[TestClass]
public class MegArchiveTest
{
    [TestMethod]
    public void Test_FindAllEntries_FindAny()
    {
        var entry = CreateFromFile("test.txt");

        var a = new MegArchive(new List<MegFileDataEntry>
        {
            entry
        });

        var entries = a.FindAllEntries("*").ToList();

        CollectionAssert.Contains(entries, entry);


        //Glob g = Glob.Parse("new/**/test.txt", new GlobOptions{ Evaluation = new EvaluationOptions {CaseInsensitive = true}});

        //var r = g.IsMatch("New/../Test.txt");

        //var files = new List<string>{"Test.txt", "New/../Test.txt", "new/test.txt", "x:/test.txt"};

        //var fs = new InMemoryDirectoryInfo("c:", files);

        //var f = fs.EnumerateFileSystemInfos().ToList();

        //var m = new Matcher(StringComparison.InvariantCultureIgnoreCase);
        //m.AddInclude("**/test.txt");
        //var re = m.Execute(fs).Files.ToList();
    }

    [TestMethod]
    public void Test_FindAllEntries_Empty()
    {
        var a = new MegArchive(new List<MegFileDataEntry>
        {
            Capacity = 0
        });

        var entries = a.FindAllEntries("*").ToList();
        Assert.AreEqual(0, entries.Count);
    }

    [TestMethod]
    [DataRow("", new[] { "", "" }, new[] { "", "" })]
    public void Test_FindAllEntries(string pattern, string[] files, string[] expectedMatches)
    {
        var megFiles = files.Select(CreateFromFile).ToList();

        var a = new MegArchive(megFiles);

        var entries = a.FindAllEntries(pattern).Select(e => e.FilePath).ToList();

        CollectionAssert.AreEqual(expectedMatches, entries);
    }

    private static MegFileDataEntry CreateFromFile(string path)
    {
        return new(new Crc32(0), path, 0, 0);
    }
}