// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace PG.Core.Reporting
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public readonly struct Message : IMessage
    {
        public DateTime CreatedTimestamp { get; }
        public string MessageContent { get; }

        public Message([NotNull] string messageContent)
        {
            CreatedTimestamp = DateTime.Now;
            MessageContent = messageContent;
        }
    }
}
