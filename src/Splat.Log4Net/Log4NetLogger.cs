﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using log4net;

namespace Splat.Log4Net
{
    /// <summary>
    /// Log4Net Logger integration into Splat.
    /// </summary>
    [DebuggerDisplay("Name={_inner.Logger.Name} Level={Level}")]
    public sealed class Log4NetLogger : ILogger, IDisposable
    {
        private readonly global::log4net.ILog _inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetLogger"/> class.
        /// </summary>
        /// <param name="inner">The actual log4net logger.</param>
        /// <exception cref="ArgumentNullException">Log4Net logger not passed.</exception>
        public Log4NetLogger(global::log4net.ILog inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            SetLogLevel();
            _inner.Logger.Repository.ConfigurationChanged += OnInnerLoggerReconfigured;
        }

        /// <inheritdoc />
        public LogLevel Level { get; private set; }

        /// <inheritdoc />
        public void Dispose()
        {
            _inner.Logger.Repository.ConfigurationChanged -= OnInnerLoggerReconfigured;
        }

        /// <inheritdoc />
        public void Write(string message, LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    if (Level <= LogLevel.Debug)
                    {
                        _inner.Debug(message);
                    }

                    break;
                case LogLevel.Info:
                    if (Level <= LogLevel.Info)
                    {
                        _inner.Info(message);
                    }

                    break;
                case LogLevel.Warn:
                    if (Level <= LogLevel.Warn)
                    {
                        _inner.Info(message);
                    }

                    break;
                case LogLevel.Error:
                    if (Level <= LogLevel.Error)
                    {
                        _inner.Error(message);
                    }

                    break;
                case LogLevel.Fatal:
                    if (Level <= LogLevel.Fatal)
                    {
                        _inner.Fatal(message);
                    }

                    break;
                default:
                    _inner.Debug(message);

                    break;
            }
        }

        /// <inheritdoc />
        public void Write(Exception exception, string message, LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    if (Level <= LogLevel.Debug)
                    {
                        _inner.Debug(message, exception);
                    }

                    break;
                case LogLevel.Info:
                    if (Level <= LogLevel.Info)
                    {
                        _inner.Info(message, exception);
                    }

                    break;
                case LogLevel.Warn:
                    if (Level <= LogLevel.Warn)
                    {
                        _inner.Info(message, exception);
                    }

                    break;
                case LogLevel.Error:
                    if (Level <= LogLevel.Error)
                    {
                        _inner.Error(message, exception);
                    }

                    break;
                case LogLevel.Fatal:
                    if (Level <= LogLevel.Fatal)
                    {
                        _inner.Fatal(message, exception);
                    }

                    break;
                default:
                    _inner.Debug(message, exception);

                    break;
            }
        }

        /// <inheritdoc />
        public void Write(string message, Type type, LogLevel logLevel)
        {
            Write($"{type.Name}: {message}", logLevel);
        }

        /// <inheritdoc />
        public void Write(Exception exception, string message, Type type, LogLevel logLevel)
        {
            Write(exception, $"{type.Name}: {message}", logLevel);
        }

        /// <summary>
        /// Works out the log level.
        /// </summary>
        /// <remarks>
        /// This was done so the Level property doesn't keep getting re-evaluated each time a Write method is called.
        /// </remarks>
        private void SetLogLevel()
        {
            if (_inner.IsDebugEnabled)
            {
                Level = LogLevel.Debug;
                return;
            }

            if (_inner.IsInfoEnabled)
            {
                Level = LogLevel.Info;
                return;
            }

            if (_inner.IsWarnEnabled)
            {
                Level = LogLevel.Warn;
                return;
            }

            if (_inner.IsErrorEnabled)
            {
                Level = LogLevel.Error;
                return;
            }

            Level = LogLevel.Fatal;
        }

        private void OnInnerLoggerReconfigured(object sender, EventArgs e)
        {
            SetLogLevel();
        }
    }
}
