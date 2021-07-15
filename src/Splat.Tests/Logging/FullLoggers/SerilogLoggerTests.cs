// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Display;
using Splat.Serilog;
using Splat.Tests.Mocks;
using Xunit;

namespace Splat.Tests.Logging
{
    /// <summary>
    /// Tests that verify the <see cref="Logger"/> class.
    /// </summary>
    public class SerilogLoggerTests : FullLoggerTestBase
    {
        private static readonly char[] NewLine = Environment.NewLine.ToCharArray();

        /// <summary>
        /// Gets a list of mappings of Serilog levels and equivalent Splat log levels.
        /// </summary>
        private static readonly Dictionary<LogLevel, LogEventLevel> _mappingsToSerilog = new()
        {
            { LogLevel.Debug, LogEventLevel.Debug },
            { LogLevel.Info, LogEventLevel.Information },
            { LogLevel.Warn, LogEventLevel.Warning },
            { LogLevel.Error, LogEventLevel.Error },
            { LogLevel.Fatal, LogEventLevel.Fatal }
        };

        /// <summary>
        /// Gets a list of mappings of Serilog levels and equivalent Splat log levels.
        /// </summary>
        private static readonly Dictionary<LogEventLevel, LogLevel> _mappingsToSplat = new()
        {
            { LogEventLevel.Debug, LogLevel.Debug },
            { LogEventLevel.Information, LogLevel.Info },
            { LogEventLevel.Warning, LogLevel.Warn },
            { LogEventLevel.Error, LogLevel.Error },
            { LogEventLevel.Fatal, LogLevel.Fatal }
        };

        /// <summary>
        /// Test to make sure the calling `UseSerilogWithWrappingFullLogger` logs.
        /// </summary>
        [Fact]
        public void Configuring_With_Static_Log_Should_Write_Message()
        {
            var originalLocator = Locator.InternalLocator;
            try
            {
                Locator.InternalLocator = new InternalLocator();
                var (seriLogger, target) = CreateSerilogger(LogLevel.Debug);
                Log.Logger = seriLogger;

                Locator.CurrentMutable.UseSerilogFullLogger();

                Assert.Equal(0, target.Logs.Count);

                IEnableLogger logger = null!;
                logger.Log().Debug<DummyObjectClass2>("This is a test.");

                Assert.Equal(1, target.Logs.Count);
                Assert.Equal("This is a test.", target.Logs.Last().message!.Trim(NewLine).Trim());
            }
            finally
            {
                Locator.InternalLocator = originalLocator;
            }
        }

        /// <summary>
        /// Test to make calling `UseSerilogWithWrappingFullLogger(Serilog.ILogger)` logs.
        /// </summary>
        [Fact]
        public void Configuring_With_PreConfigured_Log_Should_Write_Message()
        {
            var originalLocator = Locator.InternalLocator;
            try
            {
                Locator.InternalLocator = new InternalLocator();
                var (seriLogger, target) = CreateSerilogger(LogLevel.Debug);
                Locator.CurrentMutable.UseSerilogFullLogger(seriLogger);

                Assert.Equal(0, target.Logs.Count);

                IEnableLogger logger = null!;

                logger.Log().Debug<DummyObjectClass2>("This is a test.");

                Assert.Equal(1, target.Logs.Count);
                Assert.Equal("This is a test.", target.Logs.Last().message!.Trim(NewLine).Trim());
            }
            finally
            {
                Locator.InternalLocator = originalLocator;
            }
        }

        /// <inheritdoc/>
        protected override (IFullLogger logger, IMockLogTarget mockTarget) GetLogger(LogLevel minimumLogLevel)
        {
            var (log, messages) = CreateSerilogger(minimumLogLevel);
            return (new SerilogFullLogger(log), messages);
        }

        private static (Logger logger, IMockLogTarget mockTarget) CreateSerilogger(LogLevel minimumLogLevel)
        {
            var messages = new LogTarget();

            var log = new LoggerConfiguration()
                .Enrich
                .WithExceptionDetails()
                .MinimumLevel
                .Is(_mappingsToSerilog[minimumLogLevel])
                .WriteTo
                .Sink(messages)
                .CreateLogger();

            return (log, messages);
        }

        private class LogTarget : ILogEventSink, IMockLogTarget
        {
            private static readonly MessageTemplateTextFormatter _formatter = new("{Message} {Exception}", CultureInfo.InvariantCulture);
            private readonly List<(LogLevel logLevel, string message)> _logs = new();

            public ICollection<(LogLevel logLevel, string message)> Logs => _logs;

            public void Emit(LogEvent logEvent)
            {
                using (var buffer = new StringWriter())
                {
                    var logLevel = _mappingsToSplat[logEvent.Level];
                    _formatter.Format(logEvent, buffer);
                    var message = buffer.ToString();

                    _logs.Add((logLevel, message));
                }
            }
        }
    }
}
