﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace Splat.NLog
{
    /// <summary>
    /// Helper for using NLog with Splat.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Simple helper to initialize NLog within Splat with the Wrapping Full Logger.
        /// </summary>
        /// <remarks>
        /// You should configure NLog prior to calling this method.
        /// </remarks>
        public static void UseNLogWithWrappingFullLogger()
        {
            var funcLogManager = new FuncLogManager(type =>
            {
                var actualLogger = global::NLog.LogManager.GetLogger(type.ToString());
                var miniLoggingWrapper = new NLogLogger(actualLogger);
                return new WrappingFullLogger(miniLoggingWrapper);
            });

            Locator.CurrentMutable.RegisterConstant(funcLogManager, typeof(ILogManager));
        }
    }
}
