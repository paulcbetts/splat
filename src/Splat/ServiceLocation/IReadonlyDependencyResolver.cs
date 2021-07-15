// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Splat
{
    /// <summary>
    /// An interface for interacting with a dependency resolver in a read-only fashion.
    /// </summary>
    public interface IReadonlyDependencyResolver
    {
        /// <summary>
        /// Gets an instance of the given <paramref name="serviceType"/>. Must return <c>null</c>
        /// if the service is not available (must not throw).
        /// </summary>
        /// <param name="serviceType">The object type.</param>
        /// <param name="contract">A optional value which will retrieve only a object registered with the same contract.</param>
        /// <returns>The requested object, if found; <c>null</c> otherwise.</returns>
        object? GetService(Type serviceType, string? contract = null);

        /// <summary>
        /// Gets all instances of the given <paramref name="serviceType"/>. Must return an empty
        /// collection if the service is not available (must not return <c>null</c> or throw).
        /// </summary>
        /// <param name="serviceType">The object type.</param>
        /// <param name="contract">A optional value which will retrieve only objects registered with the same contract.</param>
        /// <returns>A sequence of instances of the requested <paramref name="serviceType"/>. The sequence
        /// should be empty (not <c>null</c>) if no objects of the given type are available.</returns>
        IEnumerable<object?> GetServices(Type serviceType, string? contract = null);
    }
}
