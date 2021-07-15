// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Splat.Tests.Mocks;
using Xunit;

namespace Splat.Tests.Logging
{
    /// <summary>
    /// Tests associated with the <see cref="ActionLogger"/> class.
    /// </summary>
    public class ActionLoggerTests
    {
        /// <summary>
        /// Test to make sure the message writes.
        /// </summary>
        [Fact]
        public void Write_Should_Emit_Message()
        {
            string? passedMessage = null;
            LogLevel? passedLevel = null;

            var logger = new ActionLogger(
                (message, level) =>
                {
                    passedMessage = message;
                    passedLevel = level;
                },
                null!,
                null!,
                null!);

            var fullLogger = new WrappingFullLogger(logger);

            fullLogger.Write("This is a test.", LogLevel.Debug);

            Assert.Equal("This is a test.", passedMessage);
            Assert.Equal(LogLevel.Debug, passedLevel);
        }

        /// <summary>
        /// Test to make sure the generic type parameter is passed to the logger.
        /// </summary>
        [Fact]
        public void Debug_With_Generic_Type_Should_Emit_Message_And_Type()
        {
            string? passedMessage = null;
            LogLevel? passedLevel = null;
            Type? passedType = null;

            var logger = new ActionLogger(
                null!,
                (message, type, level) =>
                {
                    passedMessage = message;
                    passedType = type;
                    passedLevel = level;
                },
                null!,
                null!);

            var fullLogger = new WrappingFullLogger(logger);

            fullLogger.Debug<DummyObjectClass1>("This is a test.");

            Assert.Equal("This is a test.", passedMessage);
            Assert.Equal(LogLevel.Debug, passedLevel);
            Assert.Equal(typeof(DummyObjectClass1), passedType);
        }

        /// <summary>
        /// Test to make sure the generic type parameter is passed to the logger.
        /// </summary>
        [Fact]
        public void Debug_With_Generic_Type_Should_Emit_Message_And_Type_Provided()
        {
            string? passedMessage = null;
            LogLevel? passedLevel = null;
            Type? passedType = null;

            var logger = new ActionLogger(
                null!,
                (message, type, level) =>
                {
                    passedMessage = message;
                    passedType = type;
                    passedLevel = level;
                },
                null!,
                null!);

            var fullLogger = new WrappingFullLogger(logger);

            fullLogger.Debug<DummyObjectClass2>("This is a test.");

            Assert.Equal("This is a test.", passedMessage);
            Assert.Equal(LogLevel.Debug, passedLevel);
            Assert.Equal(typeof(DummyObjectClass2), passedType);
        }

        /// <summary>
        /// Test to make sure the generic type parameter is passed to the logger.
        /// </summary>
        [Fact]
        public void Info_With_Generic_Type_Should_Emit_Message_And_Type()
        {
            string? passedMessage = null;
            LogLevel? passedLevel = null;
            Type? passedType = null;

            var logger = new ActionLogger(
                null!,
                (message, type, level) =>
                {
                    passedMessage = message;
                    passedType = type;
                    passedLevel = level;
                },
                null!,
                null!);

            var fullLogger = new WrappingFullLogger(logger);

            fullLogger.Info<DummyObjectClass1>("This is a test.");

            Assert.Equal("This is a test.", passedMessage);
            Assert.Equal(LogLevel.Info, passedLevel);
            Assert.Equal(typeof(DummyObjectClass1), passedType);
        }

        /// <summary>
        /// Test to make sure the generic type parameter is passed to the logger.
        /// </summary>
        [Fact]
        public void Info_With_Generic_Type_Should_Emit_Message_And_Type_Provided()
        {
            string? passedMessage = null;
            LogLevel? passedLevel = null;
            Type? passedType = null;

            var logger = new ActionLogger(
                null!,
                (message, type, level) =>
                {
                    passedMessage = message;
                    passedType = type;
                    passedLevel = level;
                },
                null!,
                null!);

            var fullLogger = new WrappingFullLogger(logger);

            fullLogger.Info<DummyObjectClass2>("This is a test.");

            Assert.Equal("This is a test.", passedMessage);
            Assert.Equal(LogLevel.Info, passedLevel);
            Assert.Equal(typeof(DummyObjectClass2), passedType);
        }

        /// <summary>
        /// Test to make sure the generic type parameter is passed to the logger.
        /// </summary>
        [Fact]
        public void Warn_With_Generic_Type_Should_Emit_Message_And_Type()
        {
            string? passedMessage = null;
            LogLevel? passedLevel = null;
            Type? passedType = null;

            var logger = new ActionLogger(
                null!,
                (message, type, level) =>
                {
                    passedMessage = message;
                    passedType = type;
                    passedLevel = level;
                },
                null!,
                null!);

            var fullLogger = new WrappingFullLogger(logger);

            fullLogger.Warn<DummyObjectClass1>("This is a test.");

            Assert.Equal("This is a test.", passedMessage);
            Assert.Equal(LogLevel.Warn, passedLevel);
            Assert.Equal(typeof(DummyObjectClass1), passedType);
        }

        /// <summary>
        /// Test to make sure the generic type parameter is passed to the logger.
        /// </summary>
        [Fact]
        public void Warn_With_Generic_Type_Should_Emit_Message_And_Type_Provided()
        {
            string? passedMessage = null;
            LogLevel? passedLevel = null;
            Type? passedType = null;

            var logger = new ActionLogger(
                null!,
                (message, type, level) =>
                {
                    passedMessage = message;
                    passedType = type;
                    passedLevel = level;
                },
                null!,
                null!);

            var fullLogger = new WrappingFullLogger(logger);

            fullLogger.Warn<DummyObjectClass2>("This is a test.");

            Assert.Equal("This is a test.", passedMessage);
            Assert.Equal(LogLevel.Warn, passedLevel);
            Assert.Equal(typeof(DummyObjectClass2), passedType);
        }

        /// <summary>
        /// Test to make sure the generic type parameter is passed to the logger.
        /// </summary>
        [Fact]
        public void Error_With_Generic_Type_Should_Emit_Message_And_Type()
        {
            string? passedMessage = null;
            LogLevel? passedLevel = null;
            Type? passedType = null;

            var logger = new ActionLogger(
                null!,
                (message, type, level) =>
                {
                    passedMessage = message;
                    passedType = type;
                    passedLevel = level;
                },
                null!,
                null!);

            var fullLogger = new WrappingFullLogger(logger);

            fullLogger.Error<DummyObjectClass1>("This is a test.");

            Assert.Equal("This is a test.", passedMessage);
            Assert.Equal(LogLevel.Error, passedLevel);
            Assert.Equal(typeof(DummyObjectClass1), passedType);
        }

        /// <summary>
        /// Test to make sure the generic type parameter is passed to the logger.
        /// </summary>
        [Fact]
        public void Error_With_Generic_Type_Should_Emit_Message_And_Type_Provided()
        {
            string? passedMessage = null;
            LogLevel? passedLevel = null;
            Type? passedType = null;

            var logger = new ActionLogger(
                null!,
                (message, type, level) =>
                {
                    passedMessage = message;
                    passedType = type;
                    passedLevel = level;
                },
                null!,
                null!);

            var fullLogger = new WrappingFullLogger(logger);

            fullLogger.Error<DummyObjectClass2>("This is a test.");

            Assert.Equal("This is a test.", passedMessage);
            Assert.Equal(LogLevel.Error, passedLevel);
            Assert.Equal(typeof(DummyObjectClass2), passedType);
        }

        /// <summary>
        /// Test to make sure the generic type parameter is passed to the logger.
        /// </summary>
        [Fact]
        public void Fatal_With_Generic_Type_Should_Emit_Message_And_Type()
        {
            string? passedMessage = null;
            LogLevel? passedLevel = null;
            Type? passedType = null;

            var logger = new ActionLogger(
                null!,
                (message, type, level) =>
                {
                    passedMessage = message;
                    passedType = type;
                    passedLevel = level;
                },
                null!,
                null!);

            var fullLogger = new WrappingFullLogger(logger);

            fullLogger.Fatal<DummyObjectClass1>("This is a test.");

            Assert.Equal("This is a test.", passedMessage);
            Assert.Equal(LogLevel.Fatal, passedLevel);
            Assert.Equal(typeof(DummyObjectClass1), passedType);
        }

        /// <summary>
        /// Test to make sure the generic type parameter is passed to the logger.
        /// </summary>
        [Fact]
        public void Fatal_With_Generic_Type_Should_Emit_Message_And_Type_Provided()
        {
            string? passedMessage = null;
            LogLevel? passedLevel = null;
            Type? passedType = null;

            var logger = new ActionLogger(
                null!,
                (message, type, level) =>
                {
                    passedMessage = message;
                    passedType = type;
                    passedLevel = level;
                },
                null!,
                null!);

            var fullLogger = new WrappingFullLogger(logger);

            fullLogger.Fatal<DummyObjectClass2>("This is a test.");

            Assert.Equal("This is a test.", passedMessage);
            Assert.Equal(LogLevel.Fatal, passedLevel);
            Assert.Equal(typeof(DummyObjectClass2), passedType);
        }
    }
}
