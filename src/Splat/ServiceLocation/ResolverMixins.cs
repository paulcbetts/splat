// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Threading;

namespace Splat
{
    /// <summary>
    /// Resolver Mixins.
    /// </summary>
    public static class ResolverMixins
    {
        /// <summary>
        /// Registers a factory for the given <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The service type to register for.</typeparam>
        /// <param name="resolver">The resolver to register the service type with.</param>
        /// <param name="contract">A optional contract value which will indicates to only generate the value if this contract is specified.</param>
        /// <returns>The resolver.</returns>
        public static IMutableDependencyResolver RegisterAnd<T>(this IMutableDependencyResolver resolver, string? contract = null)
            where T : new()
        {
            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            resolver.Register(() => new T(), typeof(T), contract);
            return resolver;
        }

        /// <summary>
        /// Registers a factory for the given <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The service type to register for.</typeparam>
        /// <param name="resolver">The resolver to register the service type with.</param>
        /// <param name="factory">A factory method for generating a object of the specified type.</param>
        /// <param name="contract">A optional contract value which will indicates to only generate the value if this contract is specified.</param>
        /// <returns>The resolver.</returns>
        public static IMutableDependencyResolver RegisterAnd<T>(this IMutableDependencyResolver resolver, Func<T> factory, string? contract = null)
        {
            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            resolver.Register(() => factory()!, typeof(T), contract);
            return resolver;
        }

        /// <summary>
        /// Registers a factory for the given <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="TAs">The type to register as.</typeparam>
        /// <typeparam name="T">The service type to register for.</typeparam>
        /// <param name="resolver">The resolver to register the service type with.</param>
        /// <param name="contract">A optional contract value which will indicates to only generate the value if this contract is specified.</param>
        /// <returns>The resolver.</returns>
        public static IMutableDependencyResolver RegisterAnd<TAs, T>(this IMutableDependencyResolver resolver, string? contract = null)
            where T : new()
        {
            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            resolver.Register(() => new T(), typeof(TAs), contract);
            return resolver;
        }

        /// <summary>
        /// Registers a factory for the given <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="TAs">The type to register as.</typeparam>
        /// <typeparam name="T">The service type to register for.</typeparam>
        /// <param name="resolver">The resolver to register the service type with.</param>
        /// <param name="factory">A factory method for generating a object of the specified type.</param>
        /// <param name="contract">A optional contract value which will indicates to only generate the value if this contract is specified.</param>
        /// <returns>The resolver.</returns>
        public static IMutableDependencyResolver RegisterAnd<TAs, T>(this IMutableDependencyResolver resolver, Func<T> factory, string? contract = null)
        {
            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            resolver.Register(() => factory()!, typeof(TAs), contract);
            return resolver;
        }

        /// <summary>
        /// Registers a constant value which will always return the specified object instance.
        /// </summary>
        /// <param name="resolver">The resolver to register the service type with.</param>
        /// <param name="value">The specified instance to always return.</param>
        /// <param name="serviceType">The type of service to register.</param>
        /// <param name="contract">A optional contract value which will indicates to only return the value if this contract is specified.</param>
        /// <returns>The resolver.</returns>
        public static IMutableDependencyResolver RegisterConstantAnd(this IMutableDependencyResolver resolver, object value, Type serviceType, string? contract = null)
        {
            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            resolver.Register(() => value, serviceType, contract);
            return resolver;
        }

        /// <summary>
        /// Registers a constant value which will always return the specified object instance.
        /// </summary>
        /// <typeparam name="T">The service type to register for.</typeparam>
        /// <param name="resolver">The resolver to register the service type with.</param>
        /// <param name="contract">A optional contract value which will indicates to only return the value if this contract is specified.</param>
        /// <returns>The resolver.</returns>
        public static IMutableDependencyResolver RegisterConstantAnd<T>(this IMutableDependencyResolver resolver, string? contract = null)
            where T : new()
        {
            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            var value = new T();
            return resolver.RegisterAnd(() => value, contract);
        }

        /// <summary>
        /// Registers a constant value which will always return the specified object instance.
        /// </summary>
        /// <typeparam name="T">The service type to register for.</typeparam>
        /// <param name="resolver">The resolver to register the service type with.</param>
        /// <param name="value">The specified instance to always return.</param>
        /// <param name="contract">A optional contract value which will indicates to only return the value if this contract is specified.</param>
        /// <returns>The resolver.</returns>
        public static IMutableDependencyResolver RegisterConstantAnd<T>(this IMutableDependencyResolver resolver, T value, string? contract = null)
        {
            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            return resolver.RegisterAnd(() => value, contract);
        }

        /// <summary>
        /// Registers a lazy singleton value which will always return the specified object instance once created.
        /// The value is only generated once someone requests the service from the resolver.
        /// </summary>
        /// <param name="resolver">The resolver to register the service type with.</param>
        /// <param name="valueFactory">A factory method for generating a object of the specified type.</param>
        /// <param name="serviceType">The type of service to register.</param>
        /// <param name="contract">A optional contract value which will indicates to only return the value if this contract is specified.</param>
        /// <returns>The resolver.</returns>
        public static IMutableDependencyResolver RegisterLazySingletonAnd(this IMutableDependencyResolver resolver, Func<object> valueFactory, Type serviceType, string? contract = null)
        {
            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            var val = new Lazy<object>(valueFactory, LazyThreadSafetyMode.ExecutionAndPublication);
            resolver.Register(() => val.Value, serviceType, contract);
            return resolver;
        }

        /// <summary>
        /// Registers a lazy singleton value which will always return the specified object instance once created.
        /// The value is only generated once someone requests the service from the resolver.
        /// </summary>
        /// <typeparam name="T">The service type to register for.</typeparam>
        /// <param name="resolver">The resolver to register the service type with.</param>
        /// <param name="contract">A optional contract value which will indicates to only return the value if this contract is specified.</param>
        /// <returns>The resolver.</returns>
        public static IMutableDependencyResolver RegisterLazySingletonAnd<T>(this IMutableDependencyResolver resolver, string? contract = null)
            where T : new()
        {
            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            var val = new Lazy<object>(() => new T(), LazyThreadSafetyMode.ExecutionAndPublication);
            resolver.Register(() => val.Value, typeof(T), contract);
            return resolver;
        }

        /// <summary>
        /// Registers a lazy singleton value which will always return the specified object instance once created.
        /// The value is only generated once someone requests the service from the resolver.
        /// </summary>
        /// <typeparam name="T">The service type to register for.</typeparam>
        /// <param name="resolver">The resolver to register the service type with.</param>
        /// <param name="valueFactory">A factory method for generating a object of the specified type.</param>
        /// <param name="contract">A optional contract value which will indicates to only return the value if this contract is specified.</param>
        /// <returns>The resolver.</returns>
        public static IMutableDependencyResolver RegisterLazySingletonAnd<T>(this IMutableDependencyResolver resolver, Func<T> valueFactory, string? contract = null)
        {
            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            var val = new Lazy<object>(() => valueFactory()!, LazyThreadSafetyMode.ExecutionAndPublication);
            resolver.Register(() => val.Value, typeof(T), contract);
            return resolver;
        }
    }
}
