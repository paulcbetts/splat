﻿using System;
using System.Collections.Generic;
using System.Text;
using Splat.ApplicationPerformanceMonitoring;
using Xunit;

namespace Splat.Tests.ApplicationPerformanceMonitoring
{
    /// <summary>
    /// Common unit tests for APM View Tracking.
    /// </summary>
    public static class BaseViewTrackingTests
    {
        /// <summary>
        /// Unit Tests for the View Tracking Constructor.
        /// </summary>
        /// <typeparam name="TViewTracking">The type for the view tracking class to construcst.</typeparam>
        public abstract class ConstructorMethod<TViewTracking>
            where TViewTracking : IViewTracking
        {
            /// <summary>
            /// Test to make sure a view tracking session is set up correctly.
            /// </summary>
            [Fact]
            public void ReturnsInstance()
            {
                var instance = GetViewTracking();
                Assert.NotNull(instance);
            }

            /// <summary>
            /// Gets a View Tracking Instance.
            /// </summary>
            /// <returns>View Tracking Instance.</returns>
            protected abstract TViewTracking GetViewTracking();
        }
    }
}
