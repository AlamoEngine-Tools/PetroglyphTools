// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.Core.Reporting
{
    public interface IMessage
    {
        DateTime CreatedTimestamp { get; }
        string MessageContent { get; }
    }
}
