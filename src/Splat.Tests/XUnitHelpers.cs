// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Splat.Tests
{
    /// <summary>
    /// Helpers for XUnit.
    /// </summary>
    public static class XUnitHelpers
    {
        /// <summary>
        /// Gets Theory Permutations for all the values in an enum.
        /// </summary>
        /// <typeparam name="TEnum">The type of enum.</typeparam>
        /// <returns>An XUnit theory data source.</returns>
        public static IEnumerable<object[]> GetEnumAsTestTheory<TEnum>()
        {
            var values = Enum.GetValues(typeof(TEnum));
            var results = new List<object[]>(values.Length);
            foreach (var value in values)
            {
                results.Add(new[] { value! });
            }

            return results;
        }
    }
}
