// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Core.Exceptions;
using PG.Core.Reporting;

namespace PG.Core.Test.Reporting
{
    [TestClass]
    public abstract class AbstractReportTest<T> where T : AbstractReport
    {
        [TestMethod]
        public void Test_GivenOpenReport__AddMessage__IsPossible()
        {
            IList<IMessage> messages = CreateTestMessageList();
            T report = GetConfiguredBaseReportWithMessages(messages);
            foreach (IMessage message in messages)
            {
                report.AddMessage(message);
            }
            Assert.IsTrue(messages.Count < ((IReport)report).Messages.Count, "The report should always contain more messages than the base creation.");
        }

        [TestMethod]
        [ExpectedException(typeof(ReportAlreadyFinalizedException))]
        public void Test_GivenFinalizedReport__AddMessage__ThrowsException()
        {
            IList<IMessage> messages = CreateTestMessageList();
            T report = GetConfiguredBaseReportWithMessages(messages);
            Assert.AreEqual(messages.Count, ((IReport)report).Messages.Count);
            report.FinalizeReport();
            Assert.IsTrue(report.IsFinalized);
            report.AddMessage(new Message("Message"));
        }

        public virtual IList<IMessage> CreateTestMessageList()
        {
            List<IMessage> messages = new()
            {
                new Message("Test message 1"),
                new ErrorMessage("Test message 2"),
                new Message("Test message 3"),
                new ErrorMessage("Test message 4", new ArgumentException("that was a wrong argument!"))
            };
            return messages;
        }

        public abstract T GetConfiguredBaseReport();

        public virtual T GetConfiguredBaseReportWithMessages(IEnumerable<IMessage> messages)
        {
            T report = GetConfiguredBaseReport();
            foreach (IMessage message in messages)
            {
                report.AddMessage(message);
            }

            return report;
        }
    }
}
