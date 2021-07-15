// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using DiffEngine;

using PublicApiGenerator;

using Xunit;

namespace Splat.Tests
{
    /// <summary>
    /// A helper for doing API approvals.
    /// </summary>
    public static class ApiExtensions
    {
        private static readonly Regex _removeCoverletSectionRegex = new(@"^namespace Coverlet\.Core\.Instrumentation\.Tracker.*?^}", RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.Compiled);

        /// <summary>
        /// Checks to make sure the API is approved.
        /// </summary>
        /// <param name="assembly">The assembly that is being checked.</param>
        /// <param name="memberName">The caller member.</param>
        /// <param name="filePath">The caller file path.</param>
        public static void CheckApproval(this Assembly assembly, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null)
        {
            var targetFrameworkName = Assembly.GetExecutingAssembly().GetTargetFrameworkName();

            var sourceDirectory = Path.GetDirectoryName(filePath);

            var approvedFileName = Path.Combine(sourceDirectory!, $"ApiApprovalTests.{memberName}.{targetFrameworkName}.approved.txt");
            var receivedFileName = Path.Combine(sourceDirectory!, $"ApiApprovalTests.{memberName}.{targetFrameworkName}.received.txt");

            string approvedPublicApi = string.Empty;

            if (File.Exists(approvedFileName))
            {
                approvedPublicApi = File.ReadAllText(approvedFileName);
            }

            var generatorOptions = new ApiGeneratorOptions { WhitelistedNamespacePrefixes = new[] { "Splat" } };
            var receivedPublicApi = Filter(ApiGenerator.GeneratePublicApi(assembly, generatorOptions));

            if (!string.Equals(receivedPublicApi, approvedPublicApi, StringComparison.Ordinal))
            {
                File.WriteAllText(receivedFileName, receivedPublicApi);
                DiffRunner.Launch(receivedFileName, approvedFileName);
            }

            Assert.Equal(approvedPublicApi, receivedPublicApi);
        }

        private static string Filter(string text)
        {
            text = _removeCoverletSectionRegex.Replace(text, string.Empty);
            return string.Join(Environment.NewLine, text.Split(
                new[]
                {
                    Environment.NewLine
                },
                StringSplitOptions.RemoveEmptyEntries)
                    .Where(l =>
                    !l.StartsWith("[assembly: AssemblyVersion(", StringComparison.InvariantCulture) &&
                    !l.StartsWith("[assembly: AssemblyFileVersion(", StringComparison.InvariantCulture) &&
                    !l.StartsWith("[assembly: AssemblyInformationalVersion(", StringComparison.InvariantCulture) &&
                    !l.StartsWith("[assembly: System.Reflection.AssemblyMetadata(", StringComparison.InvariantCulture) &&
                    !string.IsNullOrWhiteSpace(l)));
        }
    }
}
