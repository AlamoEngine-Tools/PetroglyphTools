// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.Core.Reporting
{
    public interface IMessage
    {
        DateTime EventRecorded { get; }
        string Message { get; }
    }
}
