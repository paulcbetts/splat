// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DryIoc;

namespace Splat.DryIoc
{
    /// <summary>
    /// DryIoc implementation for <see cref="IMutableDependencyResolver"/>.
    /// https://bitbucket.org/dadhi/dryioc/wiki/Home.
    /// </summary>
    /// <seealso cref="Splat.IDependencyResolver" />
    public class DryIocDependencyResolver : IDependencyResolver
    {
        private IContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="DryIocDependencyResolver" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public DryIocDependencyResolver(IContainer? container = null)
        {
            _container = container ?? new Container();
        }

        /// <inheritdoc />
        public virtual object? GetService(Type serviceType, string? contract = null) =>
            string.IsNullOrEmpty(contract)
                ? _container.ResolveMany(serviceType).LastOrDefault()
                : _container.ResolveMany(serviceType, serviceKey: contract).LastOrDefault();

        /// <inheritdoc />
        public virtual IEnumerable<object?> GetServices(Type serviceType, string? contract = null) =>
            string.IsNullOrEmpty(contract)
                ? _container.ResolveMany(serviceType)
                : _container.ResolveMany(serviceType, serviceKey: contract);

        /// <inheritdoc />
        public bool HasRegistration(Type serviceType, string? contract = null)
        {
            return _container.GetServiceRegistrations().Any(x =>
            {
                if (x.ServiceType != serviceType)
                {
                    return false;
                }

                if (contract is null)
                {
                    return x.OptionalServiceKey is null;
                }

                return x.OptionalServiceKey is string serviceKeyAsString
                       && contract.Equals(serviceKeyAsString, StringComparison.Ordinal);
            });
        }

        /// <inheritdoc />
        public virtual void Register(Func<object?> factory, Type serviceType, string? contract = null)
        {
            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (string.IsNullOrEmpty(contract))
            {
                _container.UseInstance(serviceType, factory(), IfAlreadyRegistered.AppendNewImplementation);
            }
            else
            {
                _container.UseInstance(serviceType, factory(), IfAlreadyRegistered.AppendNewImplementation, serviceKey: contract);
            }
        }

        /// <inheritdoc />
        public virtual void UnregisterCurrent(Type serviceType, string? contract = null)
        {
            if (string.IsNullOrEmpty(contract))
            {
                _container.Unregister(serviceType);
            }
            else
            {
                _container.Unregister(serviceType, contract);
            }
        }

        /// <inheritdoc />
        public virtual void UnregisterAll(Type serviceType, string? contract = null)
        {
            if (string.IsNullOrEmpty(contract))
            {
                _container.Unregister(serviceType);
            }
            else
            {
                _container.Unregister(serviceType, contract);
            }
        }

        /// <inheritdoc />
        public virtual IDisposable ServiceRegistrationCallback(Type serviceType, string? contract, Action<IDisposable> callback)
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
        /// Disposes of the instance.
        /// </summary>
        /// <param name="disposing">Whether or not the instance is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _container?.Dispose();
            }
        }
    }
}
