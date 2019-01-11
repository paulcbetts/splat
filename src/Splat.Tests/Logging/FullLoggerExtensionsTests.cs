﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Splat.Tests.Mocks;
using Xunit;

namespace Splat.Tests.Logging
{
    /// <summary>
    /// Tests that check the functionality of the <see cref="FullLoggerExtensions"/> class.
    /// </summary>
    public class FullLoggerExtensionsTests
    {
        /// <summary>
        /// Test to make sure the debug emits nothing when not enabled.
        /// </summary>
        [Fact]
        public void Debug_Disabled_Should_Not_Emit()
        {
            var textLogger = new TextLogger();
            var logger = new WrappingFullLogger(textLogger);
            var invoked = false;
            logger.Level = LogLevel.Fatal;

            logger.Debug<DummyObjectClass1>(
                () =>
                {
                    invoked = true;
                    return "This is a test.";
                });

            Assert.Null(textLogger.Value);
            Assert.Null(textLogger.PassedTypes.FirstOrDefault());
            Assert.False(invoked);
        }

        /// <summary>
        /// Test to make sure the debug emits something when enabled.
        /// </summary>
        [Fact]
        public void Debug_Enabled_Should_Emit()
        {
            var textLogger = new TextLogger();
            bool invoked = false;
            var logger = new WrappingFullLogger(textLogger);
            logger.Level = LogLevel.Debug;

            logger.Debug<DummyObjectClass1>(
                () =>
                {
                    invoked = true;
                    return "This is a test.";
                });

            Assert.Equal("This is a test.\r\n", textLogger.Value);
            Assert.Equal(typeof(DummyObjectClass1), textLogger.PassedTypes.FirstOrDefault());
            Assert.True(invoked);
        }

        /// <summary>
        /// Test to make sure the Info emits nothing when not enabled.
        /// </summary>
        [Fact]
        public void Info_Disabled_Should_Not_Emit()
        {
            var textLogger = new TextLogger();
            var logger = new WrappingFullLogger(textLogger);
            var invoked = false;
            logger.Level = LogLevel.Fatal;

            logger.Info<DummyObjectClass1>(
                () =>
                {
                    invoked = true;
                    return "This is a test.";
                });

            Assert.Null(textLogger.Value);
            Assert.Null(textLogger.PassedTypes.FirstOrDefault());
            Assert.False(invoked);
        }

        /// <summary>
        /// Test to make sure the Info emits something when enabled.
        /// </summary>
        [Fact]
        public void Info_Enabled_Should_Emit()
        {
            var textLogger = new TextLogger();
            bool invoked = false;
            var logger = new WrappingFullLogger(textLogger);
            logger.Level = LogLevel.Debug;

            logger.Info<DummyObjectClass1>(
                () =>
                {
                    invoked = true;
                    return "This is a test.";
                });

            Assert.Equal("This is a test.\r\n", textLogger.Value);
            Assert.Equal(typeof(DummyObjectClass1), textLogger.PassedTypes.FirstOrDefault());
            Assert.True(invoked);
        }

        /// <summary>
        /// Test to make sure the Warn emits nothing when not enabled.
        /// </summary>
        [Fact]
        public void Warn_Disabled_Should_Not_Emit()
        {
            var textLogger = new TextLogger();
            var logger = new WrappingFullLogger(textLogger);
            var invoked = false;
            logger.Level = LogLevel.Fatal;

            logger.Warn<DummyObjectClass1>(
                () =>
                {
                    invoked = true;
                    return "This is a test.";
                });

            Assert.Null(textLogger.Value);
            Assert.Null(textLogger.PassedTypes.FirstOrDefault());
            Assert.False(invoked);
        }

        /// <summary>
        /// Test to make sure the Warn emits something when enabled.
        /// </summary>
        [Fact]
        public void Warn_Enabled_Should_Emit()
        {
            var textLogger = new TextLogger();
            bool invoked = false;
            var logger = new WrappingFullLogger(textLogger);
            logger.Level = LogLevel.Debug;

            logger.Warn<DummyObjectClass1>(
                () =>
                {
                    invoked = true;
                    return "This is a test.";
                });

            Assert.Equal("This is a test.\r\n", textLogger.Value);
            Assert.Equal(typeof(DummyObjectClass1), textLogger.PassedTypes.FirstOrDefault());
            Assert.True(invoked);
        }

        /// <summary>
        /// Test to make sure the Error emits nothing when not enabled.
        /// </summary>
        [Fact]
        public void Error_Disabled_Should_Not_Emit()
        {
            var textLogger = new TextLogger();
            var logger = new WrappingFullLogger(textLogger);
            var invoked = false;
            logger.Level = LogLevel.Fatal;

            logger.Error<DummyObjectClass1>(
                () =>
                {
                    invoked = true;
                    return "This is a test.";
                });

            Assert.Null(textLogger.Value);
            Assert.Null(textLogger.PassedTypes.FirstOrDefault());
            Assert.False(invoked);
        }

        /// <summary>
        /// Test to make sure the Error emits something when enabled.
        /// </summary>
        [Fact]
        public void Error_Enabled_Should_Emit()
        {
            var textLogger = new TextLogger();
            bool invoked = false;
            var logger = new WrappingFullLogger(textLogger);
            logger.Level = LogLevel.Debug;

            logger.Error<DummyObjectClass1>(
                () =>
                {
                    invoked = true;
                    return "This is a test.";
                });

            Assert.Equal("This is a test.\r\n", textLogger.Value);
            Assert.Equal(typeof(DummyObjectClass1), textLogger.PassedTypes.FirstOrDefault());
            Assert.True(invoked);
        }

        /// <summary>
        /// Test to make sure the Fatal emits something when enabled.
        /// </summary>
        [Fact]
        public void Fatal_Enabled_Should_Emit()
        {
            var textLogger = new TextLogger();
            bool invoked = false;
            var logger = new WrappingFullLogger(textLogger);
            logger.Level = LogLevel.Fatal;

            logger.Fatal<DummyObjectClass1>(
                () =>
                {
                    invoked = true;
                    return "This is a test.";
                });

            Assert.Equal("This is a test.\r\n", textLogger.Value);
            Assert.Equal(typeof(DummyObjectClass1), textLogger.PassedTypes.FirstOrDefault());
            Assert.True(invoked);
        }
    }
}
