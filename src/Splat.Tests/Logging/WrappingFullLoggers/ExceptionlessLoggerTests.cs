// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if !WINDOWS_UWP && !ANDROID

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exceptionless;
using Exceptionless.Models;
using Exceptionless.Plugins;
using Splat.Exceptionless;
using Splat.Tests.Mocks;

namespace Splat.Tests.Logging.WrappingFullLoggers
{
    /// <summary>
    /// Unit tests for the Exceptionless Logger.
    /// </summary>
    public class ExceptionlessLoggerTests : FullLoggerTestBase
    {
        private static readonly Dictionary<global::Exceptionless.Logging.LogLevel, LogLevel> _exceptionless2Splat = new()
        {
            { global::Exceptionless.Logging.LogLevel.Debug, LogLevel.Debug },
            { global::Exceptionless.Logging.LogLevel.Info, LogLevel.Info },
            { global::Exceptionless.Logging.LogLevel.Warn, LogLevel.Warn },
            { global::Exceptionless.Logging.LogLevel.Error, LogLevel.Error },
            { global::Exceptionless.Logging.LogLevel.Fatal, LogLevel.Fatal },
        };

        private static readonly Dictionary<LogLevel, global::Exceptionless.Logging.LogLevel> _splat2Exceptionless = new()
        {
            { LogLevel.Debug, global::Exceptionless.Logging.LogLevel.Debug },
            { LogLevel.Info, global::Exceptionless.Logging.LogLevel.Info },
            { LogLevel.Warn, global::Exceptionless.Logging.LogLevel.Warn },
            { LogLevel.Error, global::Exceptionless.Logging.LogLevel.Error },
            { LogLevel.Fatal, global::Exceptionless.Logging.LogLevel.Fatal },
        };

        /// <inheritdoc/>
        protected override (IFullLogger logger, IMockLogTarget mockTarget) GetLogger(LogLevel minimumLogLevel)
        {
            var logTarget = new InMemoryExceptionlessLogTarget();

            var exceptionlessClient = new ExceptionlessClient();
            exceptionlessClient.Configuration.ApiKey = "someapikey";

            var exceptionlessMinLogLevel = _splat2Exceptionless[minimumLogLevel];
            exceptionlessClient.Configuration.UseInMemoryLogger(exceptionlessMinLogLevel);
            exceptionlessClient.Configuration.SetDefaultMinLogLevel(exceptionlessMinLogLevel);
            exceptionlessClient.Configuration.AddPlugin("Use Mock Log Target Interceptor", context => PluginAction(context, logTarget));
            var inner = new ExceptionlessSplatLogger(typeof(ExceptionlessLoggerTests), exceptionlessClient);
            return (new WrappingFullLogger(inner), logTarget);
        }

        private static LogLevel GetSplatLogLevel(global::Exceptionless.Logging.LogLevel logLevel)
        {
            return _exceptionless2Splat[logLevel];
        }

        private static void PluginAction(EventPluginContext obj, InMemoryExceptionlessLogTarget logTarget)
        {
            obj.Cancel = true;
            if (!obj.Event.Type.Equals(Event.KnownTypes.Log, StringComparison.Ordinal))
            {
                return;
            }

            var level = obj.Event.Data.GetValueOrDefault(Event.KnownDataKeys.Level) as string;

            var logLevel = global::Exceptionless.Logging.LogLevel.FromString(level!);

            var splatLogLevel = GetSplatLogLevel(logLevel);
            var exception = obj.ContextData.HasException() ? obj.ContextData.GetException() : null;

            var exceptionMessageSuffix = exception is not null ? $" {exception}" : string.Empty;
            (LogLevel logLevel, string message) tuple = (splatLogLevel, $"{obj.Event.Message}{exceptionMessageSuffix}");
            logTarget.Logs.Add(tuple);
        }

        private class InMemoryExceptionlessLogTarget : IMockLogTarget
        {
            public InMemoryExceptionlessLogTarget()
            {
                Logs = new List<(LogLevel logLevel, string message)>();
            }

            public ICollection<(LogLevel logLevel, string message)> Logs { get; }
        }
    }
}

#endif
