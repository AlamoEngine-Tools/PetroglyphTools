// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.Core.Reporting
{
    public abstract class AbstractError : IError
    {
        public DateTime EventRecorded { get; }
        public string Message { get; }
        public Exception Exception { get; }

        protected AbstractError(string message, Exception exception = null)
        {
            EventRecorded = DateTime.Now;
            Message = message;
            Exception = exception;
        }
    }
}
