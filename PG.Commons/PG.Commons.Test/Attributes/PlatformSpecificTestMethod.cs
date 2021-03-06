// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PG.Commons.Test.Attributes
{
    public class PlatformSpecificTestMethod : TestMethodAttribute
    {
        public PlatformSpecificTestMethod(params string[] platforms)
        {
            m_platforms = platforms.Select(platformName => OSPlatform.Create(platformName.ToUpper()));
        }

        private readonly IEnumerable<OSPlatform> m_platforms;

        public override TestResult[] Execute(ITestMethod testMethod)
        {
            bool platformMatches = m_platforms.Any(RuntimeInformation.IsOSPlatform);
            return !platformMatches
                ? new[] {new TestResult {Outcome = UnitTestOutcome.Inconclusive}}
                : base.Execute(testMethod);
        }
    }
}
