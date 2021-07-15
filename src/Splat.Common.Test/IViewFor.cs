// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace Splat.Common.Test
{
#pragma warning disable SA1402 // File may only contain a single type

    /// <summary>
    /// Represents a view bound to a view model.
    /// </summary>
    /// <typeparam name="T">The view model type.</typeparam>
    /// <seealso cref="Splat.Common.Test.IViewFor" />
    public interface IViewFor<T> : IViewFor
        where T : class
    {
        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        new T? ViewModel { get; set; }
    }

    /// <summary>
    /// Represents a view bound to a view model.
    /// </summary>
    /// <seealso cref="Splat.Common.Test.IViewFor" />
    public interface IViewFor
    {
        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        object? ViewModel { get; set; }
    }

#pragma warning restore SA1402 // File may only contain a single type
}
