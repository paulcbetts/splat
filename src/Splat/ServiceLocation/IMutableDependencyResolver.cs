// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace Splat
{
    /// <summary>
    /// Represents a dependency resolver where types can be registered after setup.
    /// </summary>
    public interface IMutableDependencyResolver
    {
        /// <summary>
        /// Check to see if a resolvers has a registration for a type.
        /// </summary>
        /// <param name="serviceType">The type to check for registration.</param>
        /// <returns>Whether there is a registration for the type.</returns>
        /// <param name="contract">A optional contract value which will indicates to only generate the value if this contract is specified.</param>
        bool HasRegistration(Type serviceType, string? contract = null);

        /// <summary>
        /// Register a function with the resolver which will generate a object
        /// for the specified service type.
        /// Optionally a contract can be registered which will indicate
        /// that that registration will only work with that contract.
        /// Most implementations will use a stack based approach to allow for multile items to be registered.
        /// </summary>
        /// <param name="factory">The factory function which generates our object.</param>
        /// <param name="serviceType">The type which is used for the registration.</param>
        /// <param name="contract">A optional contract value which will indicates to only generate the value if this contract is specified.</param>
        void Register(Func<object?> factory, Type serviceType, string? contract = null);

        /// <summary>
        /// Unregisters the current item based on the specified type and contract.
        /// </summary>
        /// <param name="serviceType">The service type to unregister.</param>
        /// <param name="contract">The optional contract value, which will only remove the value associated with the contract.</param>
        void UnregisterCurrent(Type serviceType, string? contract = null);

        /// <summary>
        /// Unregisters all the values associated with the specified type and contract.
        /// </summary>
        /// <param name="serviceType">The service type to unregister.</param>
        /// <param name="contract">The optional contract value, which will only remove the value associated with the contract.</param>
        void UnregisterAll(Type serviceType, string? contract = null);

        /// <summary>
        /// Register a callback to be called when a new service matching the type
        /// and contract is registered.
        ///
        /// When registered, the callback is also called for each currently matching
        /// service.
        /// </summary>
        /// <returns>When disposed removes the callback.</returns>
        /// <param name="serviceType">The type which is used for the registration.</param>
        /// <param name="contract">A optional contract value which will indicates to only generate the value if this contract is specified.</param>
        /// <param name="callback">The callback which will be called when the specified service type and contract are registered.</param>
        IDisposable ServiceRegistrationCallback(Type serviceType, string? contract, Action<IDisposable> callback);
    }
}
