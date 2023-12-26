// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PG.Testing;

public class PlatformSpecificTestMethodAttribute : TestMethodAttribute
{
    private readonly IEnumerable<OSPlatform> _platforms;

    public PlatformSpecificTestMethodAttribute(params TestPlatformIdentifier[] platforms)
    {
        _platforms = platforms.Select(targetPlatform => OSPlatform.Create(Enum.GetName(typeof(TestPlatformIdentifier), targetPlatform)!.ToUpper()));
    }

    public override TestResult[] Execute(ITestMethod testMethod)
    {
        var platformMatches = _platforms.Any(RuntimeInformation.IsOSPlatform);
        return !platformMatches
            ? new[] { new TestResult { Outcome = UnitTestOutcome.Inconclusive } }
            : base.Execute(testMethod);
    }
}