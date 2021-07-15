// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Prism.Ioc;

namespace Splat.Prism
{
    /// <summary>
    /// A container for the Prism application.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Analyzers", "CA1065: Don't throw exceptions from properties", Justification = "Deliberate usage")]
    public class SplatContainerExtension : IContainerExtension<IDependencyResolver>, IDisposable
    {
        private readonly ConcurrentDictionary<(Type type, string? contract), Type> _types = new();
        private Action? _disposeAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="SplatContainerExtension"/> class.
        /// </summary>
        public SplatContainerExtension()
        {
            Locator.SetLocator(Instance);
            _disposeAction = () => Locator.SetLocator(new ModernDependencyResolver());
        }

        /// <summary>
        /// Gets the dependency resolver.
        /// </summary>
        public IDependencyResolver Instance { get; } = new ModernDependencyResolver();

        /// <inheritdoc/>
        public IScopedProvider CurrentScope => throw new NotImplementedException();

        /// <inheritdoc/>
        public IScopedProvider CreateScope()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public void FinalizeExtension()
        {
        }

        /// <inheritdoc/>
        public IContainerRegistry RegisterScoped(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public bool IsRegistered(Type type)
        {
            return Instance.HasRegistration(type);
        }

        /// <inheritdoc/>
        public bool IsRegistered(Type type, string name)
        {
            return Instance.HasRegistration(type, name);
        }

        /// <inheritdoc/>
        public IContainerRegistry RegisterManySingleton(Type type, params Type[] serviceTypes)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public IContainerRegistry Register(Type from, Type to)
        {
            _types[(from, null)] = to;
            Instance.Register(() => Activator.CreateInstance(to) ?? throw new InvalidOperationException("Could not create type"), from);
            return this;
        }

        /// <summary>
        /// Registers an object with the default registration func.
        /// </summary>
        /// <param name="from">The type to transform from.</param>
        /// <param name="to">The type to transform to.</param>
        /// <param name="defaultCreationFunc">A creation func for generating the type.</param>
        /// <returns>The container registry for builder operations.</returns>
        public IContainerRegistry Register(Type from, Type to, Func<object> defaultCreationFunc)
        {
            _types[(from, null)] = to;
            Instance.Register(defaultCreationFunc, from);
            return this;
        }

        /// <inheritdoc/>
        public IContainerRegistry Register(Type from, Type to, string name)
        {
            _types[(from, name)] = to;
            Instance.Register(() => Activator.CreateInstance(to) ?? throw new InvalidOperationException("Could not create type"), from, name);
            return this;
        }

        /// <inheritdoc/>
        public IContainerRegistry Register(Type type, Func<object> factoryMethod)
        {
            Instance.Register(factoryMethod, type);
            return this;
        }

        /// <inheritdoc/>
        public IContainerRegistry Register(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            Instance.Register(() => factoryMethod(this), type);
            return this;
        }

        /// <inheritdoc/>
        public IContainerRegistry RegisterMany(Type type, params Type[] serviceTypes)
        {
            if (serviceTypes is null)
            {
                return this;
            }

            foreach (Type serviceType in serviceTypes)
            {
                Instance.Register(() => Activator.CreateInstance(type) ?? throw new InvalidOperationException("Could not create type"), serviceType);
            }

            return this;
        }

        /// <inheritdoc/>
        public IContainerRegistry RegisterScoped(Type @from, Type to)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public IContainerRegistry RegisterScoped(Type type, Func<object> factoryMethod)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Registers an object with the default registration func.
        /// </summary>
        /// <param name="from">The type to transform from.</param>
        /// <param name="to">The type to transform to.</param>
        /// <param name="name">The contract name.</param>
        /// <param name="defaultCreationFunc">A creation func for generating the type.</param>
        /// <returns>The container registry for builder operations.</returns>
        public IContainerRegistry Register(Type from, Type to, string name, Func<object> defaultCreationFunc)
        {
            _types[(from, name)] = to;
            Instance.Register(defaultCreationFunc, from, name);
            return this;
        }

        /// <inheritdoc/>
        public IContainerRegistry RegisterInstance(Type type, object instance)
        {
            Instance.RegisterConstant(instance, type);
            return this;
        }

        /// <inheritdoc/>
        public IContainerRegistry RegisterInstance(Type type, object instance, string name)
        {
            Instance.RegisterConstant(instance, type, name);
            return this;
        }

        /// <inheritdoc/>
        public IContainerRegistry RegisterSingleton(Type from, Type to)
        {
            _types[(from, null)] = to;
            Instance.RegisterLazySingleton(() => Activator.CreateInstance(to) ?? throw new InvalidOperationException("Could not create type"), from);
            return this;
        }

        /// <summary>
        /// Registers an object with the default registration func.
        /// </summary>
        /// <param name="from">The type to transform from.</param>
        /// <param name="to">The type to transform to.</param>
        /// <param name="defaultCreationFunc">A creation func for generating the type.</param>
        /// <returns>The container registry for builder operations.</returns>
        public IContainerRegistry RegisterSingleton(Type from, Type to, Func<object> defaultCreationFunc)
        {
            _types[(from, null)] = to;
            Instance.RegisterLazySingleton(defaultCreationFunc, from);
            return this;
        }

        /// <summary>
        /// Registers an object with the default registration func.
        /// </summary>
        /// <param name="from">The type to transform from.</param>
        /// <param name="to">The type to transform to.</param>
        /// <param name="name">The contract name.</param>
        /// <param name="defaultCreationFunc">A creation func for generating the type.</param>
        /// <returns>The container registry for builder operations.</returns>
        public IContainerRegistry RegisterSingleton(Type from, Type to, string name, Func<object> defaultCreationFunc)
        {
            _types[(from, name)] = to;
            Instance.RegisterLazySingleton(defaultCreationFunc, from);
            return this;
        }

        /// <inheritdoc/>
        public IContainerRegistry RegisterSingleton(Type from, Type to, string name)
        {
            _types[(from, null)] = to;
            Instance.RegisterLazySingleton(() => Activator.CreateInstance(to) ?? throw new InvalidOperationException("Could not create type"), from, name);
            return this;
        }

        /// <inheritdoc/>
        public IContainerRegistry RegisterSingleton(Type type, Func<object> factoryMethod)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public IContainerRegistry RegisterSingleton(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public object? Resolve(Type type)
        {
            return Instance.GetService(type);
        }

        /// <inheritdoc/>
        public object? Resolve(Type type, params (Type Type, object Instance)[] parameters)
        {
            if (_types.TryGetValue((type, null), out var resolvedType))
            {
                return Activator.CreateInstance(resolvedType, parameters.Select(x => x.Instance)) ?? throw new InvalidOperationException("Could not create type");
            }

            return default;
        }

        /// <inheritdoc/>
        public object? Resolve(Type type, string name)
        {
            return Instance.GetService(type, name);
        }

        /// <inheritdoc/>
        public object? Resolve(Type type, string name, params (Type Type, object Instance)[] parameters)
        {
            if (!_types.TryGetValue((type, name), out var resolvedType))
            {
                if (resolvedType is null)
                {
                    return default;
                }

                return Activator.CreateInstance(resolvedType, parameters.Select(x => x.Instance))!;
            }

            return default;
        }

        /// <summary>
        /// Disposes data associated with the extension.
        /// </summary>
        /// <param name="isDisposing">If we are getting called by the Dispose() method rather than a finalizer.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                Interlocked.Exchange(ref _disposeAction, null)?.Invoke();
                _types.Clear();
            }
        }
    }
}
