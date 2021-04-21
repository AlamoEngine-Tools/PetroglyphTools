// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;

namespace PG.Core.Reporting
{
    public interface IReport
    {
        DateTime EventStart { get; }
        DateTime EventEnd { get; }
        public IReadOnlyList<IError> Errors { get; }
        public IReadOnlyList<IMessage> Messages { get; }
    }
}
