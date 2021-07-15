// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Splat.Tests.Mocks;
using Xunit;

namespace Splat.Tests
{
    /// <summary>
    /// Unit Tests for the Memoizing MRU Cache.
    /// </summary>
    public class MemoizingMRUCacheTests
    {
        /// <summary>
        /// Checks to ensure an Argument Null Exception is thrown.
        /// </summary>
        [Fact]
        public void ThrowsArgumentNullException()
        {
            var instance = GetTestInstance();
            Assert.Throws<ArgumentNullException>(() => instance.Get(null!));
        }

        /// <summary>
        /// Checks to ensure a value is returned.
        /// </summary>
        [Fact]
        public void ReturnsValue()
        {
            var instance = GetTestInstance();
            var result = instance.Get("Test1");
            Assert.NotNull(result);
        }

        /// <summary>
        /// Checks to ensure a value is returned for 2 duplicate calls.
        /// </summary>
        [Fact]
        public void GetReturnsSameValue()
        {
            var instance = GetTestInstance();
            var result1 = instance.Get("Test1");
            Assert.NotNull(result1);
            var result2 = instance.Get("Test1");
            Assert.NotNull(result2);
            Assert.Same(result1, result2);
        }

        /// <summary>
        /// Checks to ensure 2 different values are returned for 2 different calls.
        /// </summary>
        [Fact]
        public void GetReturnsDifferentValues()
        {
            var instance = GetTestInstance();
            var result1 = instance.Get("Test1");
            Assert.NotNull(result1);
            var result2 = instance.Get("Test2");
            Assert.NotNull(result2);
            Assert.NotSame(result1, result2);
        }

        /// <summary>
        /// Checks to ensure a value is returned for 2 duplicate calls.
        /// </summary>
        [Fact]
        public void TryGetReturnsSameValue()
        {
            var instance = GetTestInstance();
            var result1 = instance.Get("Test1");
            Assert.NotNull(result1);
            instance.TryGet("Test1", out var result2);
            Assert.NotNull(result2);
            Assert.Same(result1, result2);
        }

        /// <summary>
        /// Checks to ensure 2 different values are returned for 2 different calls.
        /// </summary>
        [Fact]
        public void TryGetReturnsDifferentValues()
        {
            var instance = GetTestInstance();
            var p1 = instance.Get("Test1");
            var p2 = instance.Get("Test2");

            var result1 = instance.Get("Test1");
            Assert.NotNull(result1);

            var result2 = instance.Get("Test2");
            Assert.NotNull(result2);

            Assert.NotSame(result1, result2);
            Assert.Same(p1, result1);
            Assert.Same(p2, result2);
        }

        /// <summary>
        /// Crude test for checking thread safety when using Get.
        /// </summary>
        [Fact]
        public void ThreadSafeRetrievalTest()
        {
            var instance = GetTestInstance();

            var tests = Enumerable.Range(0, 100);

            var results = tests.AsParallel().Select(_ => instance.Get("Test1")).ToList();

            var first = results.First();

            foreach (var dummyObjectClass1 in results)
            {
                Assert.Same(first, dummyObjectClass1);
            }
        }

        /// <summary>
        /// Crude test for checking thread safety when using Get and TryGet.
        /// </summary>
        [Fact]
        public void ThreadSafeRetrievalTestWithGetAndTryGet()
        {
            var instance = GetTestInstance();

            var tests = Enumerable.Range(0, 100);

            var results = tests.AsParallel().Select(i =>
            {
                if (i % 2 == 0)
                {
                    return instance.Get("Test1");
                }

                instance.TryGet("Test1", out var result);
                return result;
            }).ToList();

            var first = results.First(x => x is not null);

            foreach (var dummyObjectClass1 in results)
            {
                if (dummyObjectClass1 is not null)
                {
                    Assert.Same(first, dummyObjectClass1);
                }
            }
        }

        /// <summary>
        /// Check that invalidate plays nicely.
        /// </summary>
        [Fact]
        public void GetsResultsFromCacheValuesWhenInvalidateAndGetAreUsed()
        {
            var instance = GetTestInstance();

            var tests = Enumerable.Range(0, 100);

            var results = tests.AsParallel().Select(i =>
            {
                instance.Invalidate("Test1");
                instance.Get("Test1");

                return instance.CachedValues();
            }).ToList();

            // This is actually a crude test of a misuse case.
            // There's no guarantee what comes out is entirely different for every task\thread.
            // Just make sure it doesn't throw an exception.
            Assert.NotNull(results);
        }

        /// <summary>
        /// Check that invalidate plays nicely.
        /// </summary>
        [Fact]
        public void GetsResultsWhenInvalidateAndGetAreUsed()
        {
            var instance = GetTestInstance();

            var tests = Enumerable.Range(0, 100);

            var results = tests.AsParallel().Select(i =>
            {
                instance.Invalidate("Test1");
                var result = instance.Get("Test1");

                // this is here just simply to test cache values plays nicely as well.
                var cachedValues = instance.CachedValues();
                return result;
            }).ToList();

            // This is actually a crude test of a misuse case.
            // There's no guarantee what comes out is entirely different for every task\thread.
            // Just make sure it doesn't throw an exception.
            Assert.NotNull(results);
        }

        /// <summary>
        /// Check that invalidate all plays nicely.
        /// </summary>
        [Fact]
        public void GetsResultsWhenInvalidateAllAndGetAreUsed()
        {
            var instance = GetTestInstance();

            var tests = Enumerable.Range(0, 100);

            var results = tests.AsParallel().Select(i =>
            {
                instance.InvalidateAll();
                return instance.Get("Test1");
            }).ToList();

            // This is actually a crude test of a misuse case.
            // There's no guarantee what comes out is entirely different for every task\thread.
            // Just make sure it doesn't throw an exception.
            Assert.NotNull(results);
        }

        private static MemoizingMRUCache<string, DummyObjectClass1> GetTestInstance()
        {
            return new(
                (param, o) => new DummyObjectClass1(),
                256);
        }
    }
}
