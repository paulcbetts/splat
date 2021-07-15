// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;

#if !NETSTANDARD2_0

namespace Splat.Tests
{
    /// <summary>
    /// Unit Tests for the Bitmap Loader.
    /// </summary>
    public sealed class BitmapLoaderTests
    {
        /// <summary>
        /// Gets the test data for the Load Suceeds Unit Test.
        /// </summary>
        public static TheoryData<Func<Stream>> LoadSucceedsTestData { get; } = new()
        {
            GetPngStream,
            GetJpegStream,
            GetBitmapStream,
        };

        /// <summary>
        /// Test to ensure the bitmap loader initializes properly.
        /// </summary>
        /// <remarks>
        /// Looks crude and pointless, but was produced to track an issue on Android between VS2017 and VS2019.
        /// </remarks>
        [Fact]
        public void ReturnsInstance()
        {
            var instance = new Splat.PlatformBitmapLoader();
            Assert.NotNull(instance);
        }

        /// <summary>
        /// Test to ensure creating a default bitmap succeeds on all platforms.
        /// </summary>
        [Fact]
        public void Create_Succeeds()
        {
            var instance = new Splat.PlatformBitmapLoader();
            var result = instance.Create(1, 1);

            Assert.NotNull(result);
        }

        /// <summary>
        /// Test to ensure loading a bitmap succeeds on all platforms.
        /// </summary>
        /// <param name="getStream">Function to load a file stream.</param>
        [Theory]
        [MemberData(nameof(LoadSucceedsTestData))]
        public void Load_Succeeds(Func<Stream> getStream)
        {
            if (getStream is null)
            {
                throw new ArgumentNullException(nameof(getStream));
            }

            var instance = new Splat.PlatformBitmapLoader();

            using (var sourceStream = getStream())
            {
                var result = instance.Load(
                    sourceStream,
                    640,
                    480);

                Assert.NotNull(result);
            }
        }

        private static Stream GetBitmapStream()
        {
            return GetStream("splatlogo.bmp");
        }

        private static Stream GetJpegStream()
        {
            return GetStream("splatlogo.jpg");
        }

        private static Stream GetPngStream()
        {
            return GetStream("splatlogo.png");
        }

        private static Stream GetStream(string imageName)
        {
#if ANDROID
            return Android.App.Application.Context.Assets.Open(imageName);
#else
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(imageName)!;
#endif
        }
    }
}

#endif
