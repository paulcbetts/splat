﻿using System;
using System.Collections.Generic;
using System.IO;
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
        /// Test data for the Load Suceeds Unit Test.
        /// </summary>
        public static TheoryData<Func<Stream>> LoadSucceedsTestData = new TheoryData<Func<Stream>>
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
            var cwd = Path.GetDirectoryName(typeof(BitmapLoaderTests).Assembly.Location);
            var path = Path.Combine(cwd, imageName);
            return File.OpenRead(path);
#endif
        }
    }
}
#endif
