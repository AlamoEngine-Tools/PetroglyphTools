// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.Core.Data.Etl.Reporting
{
    /// <summary>
    /// The base implementation of the <see cref="IEtlMessage"/>
    /// </summary>
    public abstract class AbstractEtlMessage : IEtlMessage
    {
        public int CompareTo(IEtlMessage other)
        {
            return TimeStamp.CompareTo(other.TimeStamp);
        }

        protected AbstractEtlMessage(string message)
        {
            TimeStamp = DateTime.Now;
            Message = message;
        }

        public DateTime TimeStamp { get; }
        public string Message { get; }
    }
}
