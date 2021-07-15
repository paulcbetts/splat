// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using Splat.ApplicationPerformanceMonitoring;
using Xunit;

namespace Splat.Tests.ApplicationPerformanceMonitoring
{
    /// <summary>
    /// Common Unit Tests for Feature Usage Tracking.
    /// </summary>
    public static class BaseFeatureUsageTrackingTests
    {
        /// <summary>
        /// Unit Tests for the constructor.
        /// </summary>
        /// <typeparam name="TFeatureUsageTracking">Type of Feature Usage Tracking Session Class to test.</typeparam>
        public abstract class BaseConstructorTests<TFeatureUsageTracking>
            where TFeatureUsageTracking : IFeatureUsageTrackingSession<Guid>
        {
            /// <summary>
            /// Test to make sure a root tracking session is set up correctly.
            /// </summary>
            [Fact]
            public void ReturnsInstance()
            {
                var featureName = Guid.NewGuid().ToString();
                var instance = GetFeatureUsageTrackingSession(featureName);
                Assert.NotNull(instance);
                Assert.Equal(featureName, instance.FeatureName);
                Assert.NotEqual(Guid.Empty, instance.FeatureReference);
                Assert.Equal(Guid.Empty, instance.ParentReference);
            }

            /// <summary>
            /// Gets a Feature Usage Tracking Session.
            /// </summary>
            /// <param name="featureName">Name of the feature being tracked.</param>
            /// <returns>Feature Usage Tracking Session.</returns>
            protected abstract TFeatureUsageTracking GetFeatureUsageTrackingSession(string featureName);
        }

        /// <summary>
        /// Unit Tests for the sub-feature method.
        /// </summary>
        /// <typeparam name="TFeatureUsageTracking">Type of Feature Usage Tracking Session Class to test.</typeparam>
        public abstract class BaseSubFeatureMethodTests<TFeatureUsageTracking>
            where TFeatureUsageTracking : IFeatureUsageTrackingSession<Guid>
        {
            /// <summary>
            /// Test to make sure a sub-feature tracking session is set up correctly.
            /// </summary>
            [Fact]
            public void ReturnsInstance()
            {
                var featureName = Guid.NewGuid().ToString();
                var instance = GetFeatureUsageTrackingSession(featureName);
                Assert.NotNull(instance);
                Assert.Equal(featureName, instance.FeatureName);
                Assert.NotEqual(Guid.Empty, instance.FeatureReference);
                Assert.Equal(Guid.Empty, instance.ParentReference);

                var subfeatureName = Guid.NewGuid().ToString();
                var subfeature = instance.SubFeature(subfeatureName);
                Assert.NotNull(instance);

                var genericSubfeature = subfeature as IFeatureUsageTrackingSession<Guid>;
                Assert.NotNull(genericSubfeature);
                Assert.Equal(subfeatureName, genericSubfeature?.FeatureName);
                Assert.NotEqual(Guid.Empty, genericSubfeature?.FeatureReference);
                Assert.Equal(instance.FeatureReference, genericSubfeature?.ParentReference);
            }

            /// <summary>
            /// Gets a Feature Usage Tracking Session.
            /// </summary>
            /// <param name="featureName">Name of the feature being tracked.</param>
            /// <returns>Feature Usage Tracking Session.</returns>
            protected abstract TFeatureUsageTracking GetFeatureUsageTrackingSession(string featureName);
        }
    }
}
