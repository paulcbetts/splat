// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using log4net;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

using Splat.Log4Net;
using Splat.Tests.Mocks;

namespace Splat.Tests.Logging
{
    /// <summary>
    /// Tests that verify the <see cref="Log4NetLogger"/> class.
    /// </summary>
    public class Log4NetLoggerTests : FullLoggerTestBase
    {
        private static readonly Dictionary<Level, LogLevel> _log4Net2Splat = new()
        {
            { Level.Debug, LogLevel.Debug },
            { Level.Info, LogLevel.Info },
            { Level.Warn, LogLevel.Warn },
            { Level.Error, LogLevel.Error },
            { Level.Fatal, LogLevel.Fatal },
        };

        private static readonly Dictionary<LogLevel, Level> _splat2log4net = new()
        {
            { LogLevel.Debug, Level.Debug },
            { LogLevel.Info, Level.Info },
            { LogLevel.Warn, Level.Warn },
            { LogLevel.Error, Level.Error },
            { LogLevel.Fatal, Level.Fatal },
        };

        /// <inheritdoc/>
        protected override (IFullLogger logger, IMockLogTarget mockTarget) GetLogger(LogLevel minimumLogLevel)
        {
            var logger = LogManager.GetLogger(Guid.NewGuid().ToString());

            var hierarchyLogger = (log4net.Repository.Hierarchy.Logger)logger.Logger;
            hierarchyLogger.Level = _splat2log4net[minimumLogLevel];

            return (new WrappingFullLogger(new Log4NetLogger(logger)), CreateRepository(minimumLogLevel));
        }

        private MemoryTargetWrapper CreateRepository(LogLevel minimumLogLevel)
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository(GetType().Assembly);

            PatternLayout patternLayout = new()
            {
                ConversionPattern = "%m %exception"
            };
            patternLayout.ActivateOptions();

            var memoryAppender = new global::log4net.Appender.MemoryAppender
            {
                Threshold = _splat2log4net[minimumLogLevel],
                Layout = new PatternLayout
                {
                    ConversionPattern = "%m %exception"
                },
            };

            memoryAppender.ActivateOptions();
            var memoryWrapper = new MemoryTargetWrapper(memoryAppender);
            memoryWrapper.MemoryTarget.ActivateOptions();
            hierarchy.Root.AddAppender(memoryWrapper.MemoryTarget);

            hierarchy.Root.Level = _splat2log4net[minimumLogLevel];
            hierarchy.Configured = true;

            return memoryWrapper;
        }

        private class MemoryTargetWrapper : IMockLogTarget
        {
            public MemoryTargetWrapper(global::log4net.Appender.MemoryAppender memoryTarget)
            {
                MemoryTarget = memoryTarget;
            }

            public global::log4net.Appender.MemoryAppender MemoryTarget { get; }

            public ICollection<(LogLevel logLevel, string message)> Logs
            {
                get
                {
                    MemoryTarget.Flush(0);
                    return MemoryTarget.GetEvents().Select(x =>
                    {
                        var currentLevel = _log4Net2Splat[x.Level];

                        if (x.ExceptionObject is not null)
                        {
                            return (currentLevel, $"{x.MessageObject} {x.ExceptionObject}");
                        }

                        return (currentLevel, x.MessageObject.ToString()!);
                    }).ToList();
                }
            }
        }
    }
}
