// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Ninject;
using Ninject.Planning.Bindings;

namespace Splat.Ninject
{
    /// <summary>
    /// Ninject implementation for <see cref="IMutableDependencyResolver"/>.
    /// </summary>
    /// <seealso cref="IMutableDependencyResolver" />
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel _kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectDependencyResolver"/> class.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public NinjectDependencyResolver(IKernel kernel)
        {
            _kernel = kernel;
        }

        /// <inheritdoc />
        public virtual object? GetService(Type serviceType, string? contract = null) =>
            GetServices(serviceType, contract).LastOrDefault()!;

        /// <inheritdoc />
        public virtual IEnumerable<object?> GetServices(Type serviceType, string? contract = null)
        {
            var isNull = serviceType is null;
            if (isNull)
            {
                serviceType = typeof(NullServiceType);
            }

            if (isNull)
            {
                return _kernel.GetAll(
                    typeof(NullServiceType),
                    contract);
            }

            return _kernel.GetAll(
                serviceType,
                contract);
        }

        /// <inheritdoc />
        public bool HasRegistration(Type serviceType, string? contract = null)
        {
            return _kernel.CanResolve(serviceType, metadata => IsCorrectMetadata(metadata, contract));
        }

        /// <inheritdoc />
        public virtual void Register(Func<object?> factory, Type serviceType, string? contract = null)
        {
            var isNull = serviceType is null;

            if (isNull)
            {
                serviceType = typeof(NullServiceType);
            }

            if (string.IsNullOrWhiteSpace(contract))
            {
                _kernel.Bind(serviceType).ToMethod(_ => factory());
                return;
            }

            _kernel.Bind(serviceType).ToMethod(_ => factory()).Named(contract);
        }

        /// <inheritdoc />
        public virtual void UnregisterCurrent(Type serviceType, string? contract = null)
        {
            var isNull = serviceType is null;

            if (isNull)
            {
                serviceType = typeof(NullServiceType);
            }

            var bindings = _kernel.GetBindings(serviceType).ToArray();

            if (bindings is null || bindings.Length < 1)
            {
                return;
            }

            var matchingBinding = bindings.LastOrDefault(x => IsCorrectMetadata(x.BindingConfiguration.Metadata, contract));

            if (matchingBinding is null)
            {
                return;
            }

            _kernel.RemoveBinding(matchingBinding);
        }

        /// <inheritdoc />
        public virtual void UnregisterAll(Type serviceType, string? contract = null)
        {
            var isNull = serviceType is null;

            if (isNull)
            {
                serviceType = typeof(NullServiceType);
            }

            var bindings = _kernel.GetBindings(serviceType).ToArray();

            if (bindings is null || bindings.Length < 1)
            {
                return;
            }

            var matchingBinding = bindings.Where(x => IsCorrectMetadata(x.BindingConfiguration.Metadata, contract)).ToArray();

            if (matchingBinding.Length < 1)
            {
                return;
            }

            foreach (IBinding binding in matchingBinding)
            {
                _kernel.RemoveBinding(binding);
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
                _kernel?.Dispose();
            }
        }

        private static bool IsCorrectMetadata(global::Ninject.Planning.Bindings.IBindingMetadata metadata, string? contract)
        {
            return (metadata?.Name is null && string.IsNullOrWhiteSpace(contract))
                   || (metadata?.Name is not null && metadata.Name.Equals(contract, StringComparison.OrdinalIgnoreCase));
        }

        [SuppressMessage("Design", "CA1812: Uninitialized class.", Justification = "Used in reflection.")]
        private class NullServiceType
        {
            public NullServiceType(Func<object?> factory)
            {
                Factory = factory;
            }

            public Func<object?> Factory { get; }
        }
    }
}
