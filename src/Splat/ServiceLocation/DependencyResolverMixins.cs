// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Splat.ApplicationPerformanceMonitoring;

namespace Splat
{
    /// <summary>
    /// A set of extension methods that assist with the <see cref="IDependencyResolver"/> and <see cref="IMutableDependencyResolver"/> interfaces.
    /// </summary>
    public static class DependencyResolverMixins
    {
        /// <summary>
        /// Gets an instance of the given <typeparamref name="T"/>. Must return <c>null</c>
        /// if the service is not available (must not throw).
        /// </summary>
        /// <typeparam name="T">The type for the object we want to retrieve.</typeparam>
        /// <param name="resolver">The resolver we are getting the service from.</param>
        /// <param name="contract">A optional value which will retrieve only a object registered with the same contract.</param>
        /// <returns>The requested object, if found; <c>null</c> otherwise.</returns>
        public static T? GetService<T>(this IReadonlyDependencyResolver resolver, string? contract = null)
        {
            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            return (T?)resolver.GetService(typeof(T), contract);
        }

        /// <summary>
        /// Gets all instances of the given <typeparamref name="T"/>. Must return an empty
        /// collection if the service is not available (must not return <c>null</c> or throw).
        /// </summary>
        /// <typeparam name="T">The type for the object we want to retrieve.</typeparam>
        /// <param name="resolver">The resolver we are getting the service from.</param>
        /// <param name="contract">A optional value which will retrieve only a object registered with the same contract.</param>
        /// <returns>A sequence of instances of the requested <typeparamref name="T"/>. The sequence
        /// should be empty (not <c>null</c>) if no objects of the given type are available.</returns>
        public static IEnumerable<T> GetServices<T>(this IReadonlyDependencyResolver resolver, string? contract = null)
        {
            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            return resolver.GetServices(typeof(T), contract).Cast<T>();
        }

        /// <summary>
        /// Registers a new callback that occurs when a new service with the specified type is registered.
        /// </summary>
        /// <param name="resolver">The resolver we want to register the callback with.</param>
        /// <param name="serviceType">The service type we are wanting to observe.</param>
        /// <param name="callback">The callback which should be called.</param>
        /// <returns>A disposable which will stop notifications to the callback.</returns>
        public static IDisposable ServiceRegistrationCallback(this IMutableDependencyResolver resolver, Type serviceType, Action<IDisposable> callback)
        {
            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            return resolver.ServiceRegistrationCallback(serviceType, null, callback);
        }

        /// <summary>
        /// Override the default Dependency Resolver until the object returned
        /// is disposed.
        /// </summary>
        /// <param name="resolver">The test resolver to use.</param>
        /// <param name="suppressResolverCallback">If we should suppress the resolver callback notify.</param>
        /// <returns>A disposable which will reset the resolver back to the original.</returns>
        public static IDisposable WithResolver(this IDependencyResolver resolver, bool suppressResolverCallback = true)
        {
            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            var notificationDisposable = suppressResolverCallback ? Locator.SuppressResolverCallbackChangedNotifications() : ActionDisposable.Empty;

            var origResolver = Locator.GetLocator();
            Locator.SetLocator(resolver);

            return new CompositeDisposable(new ActionDisposable(() => Locator.SetLocator(origResolver)), notificationDisposable);
        }

        /// <summary>
        /// Registers a factory for the given <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The service type to register for.</typeparam>
        /// <param name="resolver">The resolver to register the service type with.</param>
        /// <param name="factory">A factory method for generating a object of the specified type.</param>
        /// <param name="contract">A optional contract value which will indicates to only generate the value if this contract is specified.</param>
        public static void Register<T>(this IMutableDependencyResolver resolver, Func<T> factory, string? contract = null)
            where T : notnull
        {
            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            resolver.Register(() => factory(), typeof(T), contract);
        }

        /// <summary>
        /// Registers a factory for the given <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="TAs">The type to register as.</typeparam>
        /// <typeparam name="T">The service type to register for.</typeparam>
        /// <param name="resolver">The resolver to register the service type with.</param>
        /// <param name="contract">A optional contract value which will indicates to only generate the value if this contract is specified.</param>
        public static void Register<TAs, T>(this IMutableDependencyResolver resolver, string? contract = null)
            where T : new()
        {
            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            resolver.Register(() => new T(), typeof(TAs), contract);
        }

        /// <summary>
        /// Registers a constant value which will always return the specified object instance.
        /// </summary>
        /// <param name="resolver">The resolver to register the service type with.</param>
        /// <param name="value">The specified instance to always return.</param>
        /// <param name="serviceType">The type of service to register.</param>
        /// <param name="contract">A optional contract value which will indicates to only return the value if this contract is specified.</param>
        public static void RegisterConstant(this IMutableDependencyResolver resolver, object value, Type serviceType, string? contract = null)
        {
            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            resolver.Register(() => value, serviceType, contract);
        }

        /// <summary>
        /// Registers a constant value which will always return the specified object instance.
        /// </summary>
        /// <typeparam name="T">The service type to register for.</typeparam>
        /// <param name="resolver">The resolver to register the service type with.</param>
        /// <param name="value">The specified instance to always return.</param>
        /// <param name="contract">A optional contract value which will indicates to only return the value if this contract is specified.</param>
        public static void RegisterConstant<T>(this IMutableDependencyResolver resolver, T value, string? contract = null)
            where T : notnull
        {
            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            RegisterConstant(resolver, value, typeof(T), contract);
        }

        /// <summary>
        /// Registers a lazy singleton value which will always return the specified object instance once created.
        /// The value is only generated once someone requests the service from the resolver.
        /// </summary>
        /// <param name="resolver">The resolver to register the service type with.</param>
        /// <param name="valueFactory">A factory method for generating a object of the specified type.</param>
        /// <param name="serviceType">The type of service to register.</param>
        /// <param name="contract">A optional contract value which will indicates to only return the value if this contract is specified.</param>
        public static void RegisterLazySingleton(this IMutableDependencyResolver resolver, Func<object> valueFactory, Type serviceType, string? contract = null)
        {
            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            var val = new Lazy<object>(valueFactory, LazyThreadSafetyMode.ExecutionAndPublication);
            resolver.Register(() => val.Value, serviceType, contract);
        }

        /// <summary>
        /// Registers a lazy singleton value which will always return the specified object instance once created.
        /// The value is only generated once someone requests the service from the resolver.
        /// </summary>
        /// <typeparam name="T">The service type to register for.</typeparam>
        /// <param name="resolver">The resolver to register the service type with.</param>
        /// <param name="valueFactory">A factory method for generating a object of the specified type.</param>
        /// <param name="contract">A optional contract value which will indicates to only return the value if this contract is specified.</param>
        public static void RegisterLazySingleton<T>(this IMutableDependencyResolver resolver, Func<T> valueFactory, string? contract = null)
            where T : notnull
        {
            RegisterLazySingleton(resolver, () => valueFactory(), typeof(T), contract);
        }

        /// <summary>
        /// Unregisters the current the value for the specified type and the optional contract.
        /// </summary>
        /// <typeparam name="T">The type of item to unregister.</typeparam>
        /// <param name="resolver">The resolver to unregister the service with.</param>
        /// <param name="contract">A optional contract which indicates to only removed the item registered with this contract.</param>
        public static void UnregisterCurrent<T>(this IMutableDependencyResolver resolver, string? contract = null)
        {
            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            resolver.UnregisterCurrent(typeof(T), contract);
        }

        /// <summary>
        /// Unregisters the all the values for the specified type and the optional contract.
        /// </summary>
        /// <typeparam name="T">The type of items to unregister.</typeparam>
        /// <param name="resolver">The resolver to unregister the services with.</param>
        /// <param name="contract">A optional contract which indicates to only removed those items registered with this contract.</param>
        public static void UnregisterAll<T>(this IMutableDependencyResolver resolver, string? contract = null)
        {
            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            resolver.UnregisterAll(typeof(T), contract);
        }
    }
}
