// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;

namespace Splat.Autofac
{
    /// <summary>
    /// Autofac implementation for <see cref="IDependencyResolver"/>.
    /// </summary>
    public class AutofacDependencyResolver : IDependencyResolver
    {
        private readonly object _lockObject = new();
        private readonly ContainerBuilder _builder;

        /// <summary>
        ///     The internal container, which takes care of mutability needed for ReactiveUI initialization procedure.
        ///     It is disposed of once the user sets the actual lifetime scope from which to resolve by calling SetLifetimeScope.
        /// </summary>
        private IContainer _internalContainer;

        private ILifetimeScope? _lifetimeScope;
#pragma warning disable CA2213 // _internalLifetimeScope will be disposed, because it is a child of _internalContainer
        private ILifetimeScope _internalLifetimeScope;
#pragma warning restore CA2213 // Disposable fields should be disposed

        /// <summary>
        ///     Set to true, when SetLifetimeScope has been called.
        ///     Prevents mutating the ContainerBuilder or setting the lifetime again.
        /// </summary>
        private bool _lifetimeScopeSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacDependencyResolver" /> class.
        /// </summary>
        /// <param name="builder">Autofac container builder.</param>
        public AutofacDependencyResolver(ContainerBuilder builder)
        {
            _builder = builder;

            _internalContainer = new ContainerBuilder().Build();
            _internalLifetimeScope = _internalContainer.BeginLifetimeScope();
        }

        /// <inheritdoc />
        public virtual object? GetService(Type serviceType, string? contract = null)
        {
            lock (_lockObject)
            {
                return Resolve(serviceType, contract);
            }
        }

        /// <summary>
        ///     Sets the lifetime scope which will be used to resolve ReactiveUI services.
        ///     It should be set after Autofac application-wide container is built.
        /// </summary>
        /// <param name="lifetimeScope">Lifetime scope, which will be used to resolve ReactiveUI services.</param>
        public void SetLifetimeScope(ILifetimeScope lifetimeScope)
        {
            lock (_lockObject)
            {
                if (_lifetimeScopeSet)
                {
                    throw new InvalidOperationException("Lifetime scope of the Autofac resolver has already been set");
                }

                _lifetimeScopeSet = true;
                _lifetimeScope = lifetimeScope;

                // We dispose on the internal container, since it and its many child lifetime scopes are not needed anymore.
                _internalContainer.Dispose();
                _internalContainer = new ContainerBuilder().Build();
                _internalLifetimeScope = _internalContainer.BeginLifetimeScope();
            }
        }

        /// <inheritdoc />
        public virtual IEnumerable<object?> GetServices(Type serviceType, string? contract = null)
        {
            lock (_lockObject)
            {
                try
                {
                    var enumerableType = typeof(IEnumerable<>).MakeGenericType(serviceType);
                    var instance = Resolve(enumerableType, contract);

                    if (instance is null)
                    {
                        return Array.Empty<object?>();
                    }

                    return ((IEnumerable)instance).Cast<object?>();
                }
                catch (DependencyResolutionException)
                {
                    return Array.Empty<object?>();
                }
            }
        }

        /// <inheritdoc />
        public bool HasRegistration(Type serviceType, string? contract = null)
        {
            lock (_lockObject)
            {
                var lifeTimeScope = _lifetimeScope ?? _internalLifetimeScope;

                return contract is null || string.IsNullOrWhiteSpace(contract) ?
                    lifeTimeScope.IsRegistered(serviceType) :
                    lifeTimeScope.IsRegisteredWithName(contract, serviceType);
            }
        }

        /// <summary>
        ///     Important: Because <see href="https://autofaccn.readthedocs.io/en/latest/best-practices/#consider-a-container-as-immutable">Autofac 5+ containers are immutable</see>,
        ///     this method should not be used by the end-user.
        ///     It is still needed to satisfy ReactiveUI initialization procedure.
        ///     Register a function with the resolver which will generate a object
        ///     for the specified service type.
        ///     Optionally a contract can be registered which will indicate
        ///     that registration will only work with that contract.
        ///     Most implementations will use a stack based approach to allow for multiple items to be registered.
        /// </summary>
        /// <param name="factory">The factory function which generates our object.</param>
        /// <param name="serviceType">The type which is used for the registration.</param>
        /// <param name="contract">A optional contract value which will indicates to only generate the value if this contract is specified.</param>
        [Obsolete("Because Autofac 5+ containers are immutable, this method should not be used by the end-user.")]
        public virtual void Register(Func<object?> factory, Type serviceType, string? contract = null)
        {
            lock (_lockObject)
            {
                if (_lifetimeScopeSet)
                {
                    throw new InvalidOperationException("Container has already been built and the lifetime scope set, so it is not possible to modify it anymore.");
                }

                // We register every ReactiveUI service twice.
                // First to the application-wide container, which we are still building.
                // Second to child lifetimes in a temporary container, that is used only to satisfy ReactiveUI dependencies.
                if (contract is null || string.IsNullOrWhiteSpace(contract))
                {
                    _builder.Register(_ => factory()!)
                        .As(serviceType)
                        .AsImplementedInterfaces();
                    _internalLifetimeScope = _internalLifetimeScope.BeginLifetimeScope(internalBuilder =>
                        internalBuilder.Register(_ => factory()!)
                            .As(serviceType)
                            .AsImplementedInterfaces());
                }
                else
                {
                    _builder.Register(_ => factory()!)
                        .Named(contract, serviceType)
                        .AsImplementedInterfaces();
                    _internalLifetimeScope = _internalLifetimeScope.BeginLifetimeScope(internalBuilder =>
                        internalBuilder.Register(_ => factory()!)
                            .Named(contract, serviceType)
                            .AsImplementedInterfaces());
                }
            }
        }

        /// <summary>
        ///     Because <see href="https://autofaccn.readthedocs.io/en/latest/best-practices/#consider-a-container-as-immutable">Autofac 5+ containers are immutable</see>,
        ///     UnregisterCurrent method is not available anymore.
        ///     Instead, simply <see href="https://autofaccn.readthedocs.io/en/latest/register/registration.html#default-registrations">register your service after InitializeReactiveUI</see> to override it.
        /// </summary>
        /// <param name="serviceType">The service type to unregister.</param>
        /// <param name="contract">The optional contract value, which will only remove the value associated with the contract.</param>
        /// <exception cref="System.NotImplementedException">This is not implemented by default.</exception>
        /// <inheritdoc />
        [Obsolete("Because Autofac 5+ containers are immutable, UnregisterCurrent method is not available anymore. " +
                  "Instead, simply register your service after InitializeReactiveUI to override it https://autofaccn.readthedocs.io/en/latest/register/registration.html#default-registrations.")]
        public virtual void UnregisterCurrent(Type serviceType, string? contract = null) =>
            throw new NotImplementedException("Because Autofac 5+ containers are immutable, UnregisterCurrent method is not available anymore. " +
                                              "Instead, simply register your service after InitializeReactiveUI to override it https://autofaccn.readthedocs.io/en/latest/register/registration.html#default-registrations.");

        /// <summary>
        ///     Because <see href="https://autofaccn.readthedocs.io/en/latest/best-practices/#consider-a-container-as-immutable">Autofac 5+ containers are immutable</see>,
        ///     UnregisterAll method is not available anymore.
        ///     Instead, simply <see href="https://autofaccn.readthedocs.io/en/latest/register/registration.html#default-registrations">register your service after InitializeReactiveUI</see> to override it.
        /// </summary>
        /// <param name="serviceType">The service type to unregister.</param>
        /// <param name="contract">The optional contract value, which will only remove the value associated with the contract.</param>
        /// <exception cref="System.NotImplementedException">This is not implemented by default.</exception>
        /// <inheritdoc />
        [Obsolete("Because Autofac 5+ containers are immutable, UnregisterAll method is not available anymore. " +
                  "Instead, simply register your service after InitializeReactiveUI to override it https://autofaccn.readthedocs.io/en/latest/register/registration.html#default-registrations.")]
        public virtual void UnregisterAll(Type serviceType, string? contract = null) =>
            throw new NotImplementedException("Because Autofac 5+ containers are immutable, UnregisterAll method is not available anymore. " +
                                              "Instead, simply register your service after InitializeReactiveUI to override it https://autofaccn.readthedocs.io/en/latest/register/registration.html#default-registrations.");

        /// <inheritdoc />
        public virtual IDisposable ServiceRegistrationCallback(Type serviceType, string? contract, Action<IDisposable> callback) =>
            throw new NotImplementedException();

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
            lock (_lockObject)
            {
                if (disposing)
                {
                    _lifetimeScope?.ComponentRegistry.Dispose();
                    _internalContainer?.Dispose();
                }
            }
        }

        private object Resolve(Type serviceType, string? contract)
        {
            object serviceInstance;

            var lifeTimeScope = _lifetimeScope ?? _internalLifetimeScope;

            if (contract is null || string.IsNullOrWhiteSpace(contract))
            {
                lifeTimeScope.TryResolve(serviceType, out serviceInstance!);
            }
            else
            {
                lifeTimeScope.TryResolveNamed(contract, serviceType, out serviceInstance!);
            }

            return serviceInstance;
        }
    }
}
