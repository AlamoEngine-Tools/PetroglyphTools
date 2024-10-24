// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Linq;
using PG.StarWarsGame.Components.Localisation.IO;
using PG.StarWarsGame.Components.Localisation.IO.Xml;
using Xunit;

namespace PG.StarWarsGame.Components.Localisation.Test.IO.Xml;

public class XmlInputStrategyTest
{
    [Fact]
    public void Test_XmlInputStrategy_InvalidAttributesThrowOnAccess()
    {
        var strategy = new XmlInputStrategy("test.xml", IInputStrategy.ValidationLevel.Strict);
        Assert.Throws<InvalidOperationException>(() => strategy.BaseDirectory);
        Assert.Throws<InvalidOperationException>(() => strategy.FileFilter);
    }

    [Fact]
    public void Test_XmlInputStrategy_StaticValuesReturnExpectedAttributes()
    {
        var strategy = new XmlInputStrategy("test.xml", IInputStrategy.ValidationLevel.Strict);
        Assert.Equal(IInputStrategy.FileImportGrouping.Single, strategy.ImportGrouping);
        Assert.Equal(strategy.FilePath, strategy.FilePaths.First());
    }

    [Fact]
    public void Test_XmlInputStrategy_MissingFilePathThrowsIllegalArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new XmlInputStrategy(null));
        Assert.Throws<ArgumentException>(() => new XmlInputStrategy(string.Empty));
    }
}
