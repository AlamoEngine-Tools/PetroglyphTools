// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.Core.Data.Etl.Reporting
{
    /// <summary>
    /// The base implementation of the <see cref="IEtlError"/>
    /// </summary>
    public abstract class AbstractEtlError : IEtlError
    {
        protected AbstractEtlError(string message, Exception exception = null)
        {
            TimeStamp = DateTime.Now;
            Message = message;
            Exception = exception;
        }

        public int CompareTo(IEtlError other)
        {
            return TimeStamp.CompareTo(other.TimeStamp);
        }

        public DateTime TimeStamp { get; }
        public string Message { get; }
        public Exception Exception { get; }
    }
}
