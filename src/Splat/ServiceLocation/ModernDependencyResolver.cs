﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Splat
{
    /// <summary>
    /// This class is a dependency resolver written for modern C# 5.0 times.
    /// It implements all registrations via a Factory method. With the power
    /// of Closures, you can actually implement most lifetime styles (i.e.
    /// construct per call, lazy construct, singleton) using this.
    ///
    /// Unless you have a very compelling reason not to, this is the only class
    /// you need in order to do dependency resolution, don't bother with using
    /// a full IoC container.
    ///
    /// This container is not thread safe.
    /// </summary>
    public class ModernDependencyResolver : IDependencyResolver
    {
        private Dictionary<Tuple<Type, string>, List<Func<object>>> _registry;
        private Dictionary<Tuple<Type, string>, List<Action<IDisposable>>> _callbackRegistry;

        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModernDependencyResolver"/> class.
        /// </summary>
        public ModernDependencyResolver()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModernDependencyResolver"/> class.
        /// </summary>
        /// <param name="registry">A registry of services.</param>
        protected ModernDependencyResolver(Dictionary<Tuple<Type, string>, List<Func<object>>> registry)
        {
            _registry = registry != null ?
                registry.ToDictionary(k => k.Key, v => v.Value.ToList()) :
                new Dictionary<Tuple<Type, string>, List<Func<object>>>();

            _callbackRegistry = new Dictionary<Tuple<Type, string>, List<Action<IDisposable>>>();
        }

        /// <inheritdoc />
        public void Register(Func<object> factory, Type serviceType, string contract = null)
        {
            var pair = Tuple.Create(serviceType, contract ?? string.Empty);
            if (!_registry.ContainsKey(pair))
            {
                _registry[pair] = new List<Func<object>>();
            }

            _registry[pair].Add(factory);

            if (_callbackRegistry.ContainsKey(pair))
            {
                List<Action<IDisposable>> toRemove = null;

                foreach (var callback in _callbackRegistry[pair])
                {
                    var disp = new BooleanDisposable();

                    callback(disp);

                    if (disp.IsDisposed)
                    {
                        if (toRemove == null)
                        {
                            toRemove = new List<Action<IDisposable>>();
                        }

                        toRemove.Add(callback);
                    }
                }

                if (toRemove != null)
                {
                    foreach (var c in toRemove)
                    {
                        _callbackRegistry[pair].Remove(c);
                    }
                }
            }
        }

        /// <inheritdoc />
        public object GetService(Type serviceType, string contract = null)
        {
            var pair = Tuple.Create(serviceType, contract ?? string.Empty);
            if (!_registry.ContainsKey(pair))
            {
                return default(object);
            }

            var ret = _registry[pair].Last();
            return ret();
        }

        /// <inheritdoc />
        public IEnumerable<object> GetServices(Type serviceType, string contract = null)
        {
            var pair = Tuple.Create(serviceType, contract ?? string.Empty);
            if (!_registry.ContainsKey(pair))
            {
                return Enumerable.Empty<object>();
            }

            return _registry[pair].Select(x => x()).ToList();
        }

        /// <inheritdoc />
        public void UnregisterCurrent(Type serviceType, string contract = null)
        {
            var pair = Tuple.Create(serviceType, contract ?? string.Empty);

            if (!_registry.TryGetValue(pair, out var list))
            {
                return;
            }

            list.RemoveAt(list.Count - 1);
        }

        /// <inheritdoc />
        public void UnregisterAll(Type serviceType, string contract = null)
        {
            var pair = Tuple.Create(serviceType, contract ?? string.Empty);

            _registry[pair] = new List<Func<object>>();
        }

        /// <inheritdoc />
        public IDisposable ServiceRegistrationCallback(Type serviceType, string contract, Action<IDisposable> callback)
        {
            var pair = Tuple.Create(serviceType, contract ?? string.Empty);

            if (!_callbackRegistry.ContainsKey(pair))
            {
                _callbackRegistry[pair] = new List<Action<IDisposable>>();
            }

            _callbackRegistry[pair].Add(callback);

            var disp = new ActionDisposable(() => _callbackRegistry[pair].Remove(callback));

            if (_registry.ContainsKey(pair))
            {
                foreach (var s in _registry[pair])
                {
                    callback(disp);
                }
            }

            return disp;
        }

        /// <summary>
        /// Generates a duplicate of the resolver with all the current registrations.
        /// Useful if you want to generate temporary resolver using the <see cref="DependencyResolverMixins.WithResolver(IDependencyResolver, bool)"/> method.
        /// </summary>
        /// <returns>The newly generated <see cref="ModernDependencyResolver"/> class with the current registrations.</returns>
        public ModernDependencyResolver Duplicate()
        {
            return new ModernDependencyResolver(_registry);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of all managed memory from this class.
        /// </summary>
        /// <param name="isDisposing">If we are currently disposing managed resources.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                _registry = null;
            }

            _isDisposed = true;
        }
    }
}
