// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splat.SimpleInjector
{
    /// <summary>
    /// sad.
    /// </summary>
    public class SimpleInjectorInitializer : IDependencyResolver
    {
        private readonly object _lockObject = new();

        /// <summary>
        /// Gets dictionary of registered factories.
        /// </summary>
        public Dictionary<Type, List<Func<object?>>> RegisteredFactories { get; }
            = new();

        /// <inheritdoc />
        public object? GetService(Type serviceType, string? contract = null)
        {
            lock (_lockObject)
            {
                Func<object?>? fact = RegisteredFactories[serviceType].LastOrDefault();
                return fact?.Invoke()!;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<object?> GetServices(Type serviceType, string? contract = null)
        {
            lock (_lockObject)
            {
                return RegisteredFactories[serviceType]
                    .Select(n => n()!);
            }
        }

        /// <inheritdoc />
        public bool HasRegistration(Type serviceType, string? contract = null)
        {
            lock (_lockObject)
            {
                return RegisteredFactories.TryGetValue(serviceType, out var values)
                       && values.Count > 0;
            }
        }

        /// <inheritdoc />
        public void Register(Func<object?> factory, Type serviceType, string? contract = null)
        {
            lock (_lockObject)
            {
                if (!RegisteredFactories.ContainsKey(serviceType))
                {
                    RegisteredFactories.Add(serviceType, new List<Func<object?>>());
                }

                RegisteredFactories[serviceType].Add(factory);
            }
        }

        /// <inheritdoc />
        public void UnregisterCurrent(Type serviceType, string? contract = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void UnregisterAll(Type serviceType, string? contract = null)
        {
            lock (_lockObject)
            {
                if (RegisteredFactories.ContainsKey(serviceType))
                {
                    RegisteredFactories.Remove(serviceType);
                }
            }
        }

        /// <inheritdoc />
        public IDisposable ServiceRegistrationCallback(Type serviceType, string? contract, Action<IDisposable> callback)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool isDisposing)
        {
        }
    }
}
