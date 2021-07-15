// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldShouldBePrivate", Justification = "XUnit Theories.")]

namespace Splat.Tests.Colors
{
    /// <summary>
    /// Unit Tests for the Splat Color logic.
    /// </summary>
    public class SplatColorTests
    {
        /// <summary>
        /// Gets the test data for FromKnownColor.
        /// </summary>
        public static IEnumerable<object[]> KnownColorEnums { get; } = XUnitHelpers.GetEnumAsTestTheory<KnownColor>();

        /// <summary>
        /// Tests to check you can get a SplatColor from a KnownColor.
        /// </summary>
        /// <param name="knownColor">The Known Colour to convert.</param>
        [Theory]
        [MemberData(nameof(KnownColorEnums))]
        public void FromKnownColorTests(KnownColor knownColor)
        {
            var splatColor = SplatColor.FromKnownColor(knownColor);

            Assert.NotNull(splatColor.Name);
        }

        /// <summary>
        /// Tests to check you can get a SplatColor from a name.
        /// </summary>
        /// <param name="knownColor">The Known Colour to convert.</param>
        [Theory]
        [MemberData(nameof(KnownColorEnums))]
        public void FromNameTests(KnownColor knownColor)
        {
            var splatColor = SplatColor.FromName(knownColor.ToString());

            Assert.NotNull(splatColor.Name);
        }

        private static IEnumerable<object[]> GetEnumAsTestTheory()
        {
            var values = Enum.GetValues(typeof(KnownColor));
            var results = new List<object[]>(values.Length);
            foreach (var value in values)
            {
                results.Add(new[] { value! });
            }

            return results;
        }
    }
}
