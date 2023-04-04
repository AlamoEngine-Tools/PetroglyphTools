// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace PG.Core.Reporting
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public readonly struct ErrorMessage : IErrorMessage
    {
        public DateTime CreatedTimestamp { get; }
        public string MessageContent { get; }
        public Exception AssociatedException { get; }

        public ErrorMessage([NotNull] string messageContent, Exception associatedException = null)
        {
            CreatedTimestamp = DateTime.Now;
            MessageContent = messageContent;
            AssociatedException = associatedException;
        }
    }
}
