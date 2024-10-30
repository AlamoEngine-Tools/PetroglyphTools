// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Components.Localisation.IO.Xml;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Components.Localisation.Test.IO.Xml;

public class XmlOutputStrategyTest
{
    private readonly MockFileSystem _fileSystem = new();

    [Fact]
    public void Test_XmlOutputStrategy_InvalidArgumentsThrowIllegalArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new XmlOutputStrategy(_fileSystem.DirectoryInfo.New("./"), null));
        Assert.Throws<ArgumentException>(() =>
            new XmlOutputStrategy(_fileSystem.DirectoryInfo.New("./"), string.Empty));
    }
}
