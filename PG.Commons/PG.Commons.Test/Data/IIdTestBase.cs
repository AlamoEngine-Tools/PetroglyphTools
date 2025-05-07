// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Linq;
using PG.Commons.Data;
using Xunit;

namespace PG.Commons.Test.Data;

// ReSharper disable once InconsistentNaming
public abstract class IIdTestBase<T> where T : IId
{
    [Fact]
    public void Test_NullIIdCreationReturnsNull()
    {
        foreach (var nullKeys in GetConfiguredNullIIds()) Assert.Null(nullKeys);
    }

    [Fact]
    public void Test_IIdBehavesAsExpectedWithHash()
    {
        var keyInitialisers = GetConfiguredValidIIdInitialisers();
        var dict = new Dictionary<T, string>();
        foreach (var key in keyInitialisers.Select(keyInitialiser => CreateId(keyInitialiser)))
        {
            Assert.NotNull(key);
            dict.Add(key, $"{key}: Hash: {key.GetHashCode()}");
        }

        foreach (var key in keyInitialisers.Select(keyInitialiser => CreateId(keyInitialiser)))
        {
            Assert.NotNull(key);
            Assert.Contains(key, dict.Keys);
            Assert.NotNull(dict[key]);
        }
    }

    protected abstract List<T?> GetConfiguredNullIIds();

    protected abstract List<object[]> GetConfiguredValidIIdInitialisers();

    protected abstract T CreateId(object[] keyInitialiser);
}
