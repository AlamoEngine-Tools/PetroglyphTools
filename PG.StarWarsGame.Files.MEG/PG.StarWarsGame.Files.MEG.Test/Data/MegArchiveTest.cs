using System.Collections.Generic;
using DotNet.Globbing;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PG.StarWarsGame.Files.MEG.Test.Data;

[TestClass]
public class MegArchiveTest
{
    [TestMethod]
    public void Test_Match()
    {
        Glob g = Glob.Parse("new/**/test.txt", new GlobOptions{ Evaluation = new EvaluationOptions {CaseInsensitive = true}});

        var r = g.IsMatch("New/../Test.txt");

        var files = new List<string>{"Test.txt", "New/../Test.txt", "new/test.txt"};

        Matcher m = new Matcher();
        m.AddInclude("**/test.txt");

        var fs = new InMemoryDirectoryInfo("x:", files);

        var re = m.Execute(fs);

        var f = fs.EnumerateFileSystemInfos();
    }
}