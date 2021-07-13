// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using Splat.NLog;
using Xunit;

namespace Splat.Tests.ServiceLocation
{
    /// <summary>
    /// Common tests for Dependency Resolver interaction with Splat.
    /// </summary>
    /// <typeparam name="T">The dependency resolver to test.</typeparam>
    public abstract class BaseDependencyResolverTests<T>
        where T : IDependencyResolver
    {
        /// <summary>
        /// Test to ensure Unregister doesn't cause an IndexOutOfRangeException.
        /// </summary>
        [Fact]
        public virtual void UnregisterCurrent_Doesnt_Throw_When_List_Empty()
        {
            var resolver = GetDependencyResolver();
            var type = typeof(ILogManager);
            resolver.Register(() => new DefaultLogManager(), type);
            resolver.Register(() => new DefaultLogManager(), type, "named");
            resolver.UnregisterCurrent(type);
            resolver.UnregisterCurrent(type);
        }

        /// <summary>
        /// Test to ensure UnregisterCurrent removes last entry.
        /// </summary>
        [Fact]
        public virtual void UnregisterCurrent_Remove_Last()
        {
            var resolver = GetDependencyResolver();
            var type = typeof(ILogManager);
            resolver.Register(() => new DefaultLogManager(), type);
            resolver.Register(() => new FuncLogManager(_ => new WrappingFullLogger(new DebugLogger())), type);
            resolver.Register(() => new DefaultLogManager(), type, "named");

            var service = resolver.GetService(type);
            Assert.IsType<FuncLogManager>(service);

            resolver.UnregisterCurrent(type);

            service = resolver.GetService(type);
            Assert.IsType<DefaultLogManager>(service);
        }

        /// <summary>
        /// Test to ensure Unregister doesn't cause an IndexOutOfRangeException.
        /// </summary>
        [Fact]
        public virtual void UnregisterCurrentByName_Doesnt_Throw_When_List_Empty()
        {
            var resolver = GetDependencyResolver();
            var type = typeof(ILogManager);
            var contract = "named";
            resolver.Register(() => new DefaultLogManager(), type);
            resolver.Register(() => new DefaultLogManager(), type, contract);
            resolver.UnregisterCurrent(type, contract);
            resolver.UnregisterCurrent(type, contract);
        }

        /// <summary>
        /// Test to ensure Unregister doesn't cause an IndexOutOfRangeException.
        /// </summary>
        [Fact]
        public virtual void UnregisterAll_UnregisterCurrent_Doesnt_Throw_When_List_Empty()
        {
            var resolver = GetDependencyResolver();
            var type = typeof(ILogManager);
            resolver.Register(() => new DefaultLogManager(), type);
            resolver.Register(() => new DefaultLogManager(), type, "named");
            resolver.UnregisterAll(type);
            resolver.UnregisterCurrent(type);
        }

        /// <summary>
        /// Test to ensure Unregister doesn't cause an IndexOutOfRangeException.
        /// </summary>
        [Fact]
        public virtual void UnregisterAllByContract_UnregisterCurrent_Doesnt_Throw_When_List_Empty()
        {
            var resolver = GetDependencyResolver();
            var type = typeof(ILogManager);
            var contract = "named";
            resolver.Register(() => new DefaultLogManager(), type);
            resolver.Register(() => new DefaultLogManager(), type, contract);
            resolver.UnregisterAll(type, contract);
            resolver.UnregisterCurrent(type, contract);
        }

        /// <summary>
        /// Ensures <see cref="IReadonlyDependencyResolver.GetServices(Type, string)"/> never returns null.
        /// </summary>
        [Fact]
        public void GetServices_Should_Never_Return_Null()
        {
            var resolver = GetDependencyResolver();

            Assert.NotNull(resolver.GetServices<string>());
            Assert.NotNull(resolver.GetServices<string>("Landscape"));
        }

        /// <summary>
        /// Tests for ensuring hasregistration behaves when using contracts.
        /// </summary>
        [Fact]
        public virtual void HasRegistration()
        {
            var type = typeof(string);
            const string contractOne = "ContractOne";
            const string contractTwo = "ContractTwo";
            var resolver = GetDependencyResolver();

            Assert.False(resolver.HasRegistration(type));
            Assert.False(resolver.HasRegistration(type, contractOne));
            Assert.False(resolver.HasRegistration(type, contractTwo));

            resolver.Register(() => "unnamed", type);
            Assert.True(resolver.HasRegistration(type));
            Assert.False(resolver.HasRegistration(type, contractOne));
            Assert.False(resolver.HasRegistration(type, contractTwo));
            resolver.UnregisterAll(type);

            resolver.Register(() => contractOne, type, contractOne);
            Assert.False(resolver.HasRegistration(type));
            Assert.True(resolver.HasRegistration(type, contractOne));
            Assert.False(resolver.HasRegistration(type, contractTwo));
            resolver.UnregisterAll(type, contractOne);

            resolver.Register(() => contractTwo, type, contractTwo);
            Assert.False(resolver.HasRegistration(type));
            Assert.False(resolver.HasRegistration(type, contractOne));
            Assert.True(resolver.HasRegistration(type, contractTwo));
        }

        /// <summary>
        /// Tests to ensure NLog registers correctly with different service locators.
        /// Based on issue reported in #553.
        /// </summary>
        [Fact]
        public void ILogManager_Resolvable()
        {
            var resolver = GetDependencyResolver();

            // Setup NLog for Logging (doesn't matter if I actually configure NLog or not)
            resolver.UseNLogWithWrappingFullLogger();

            // Get the ILogManager instance (this should succeed, but fails in current code)
            ILogManager lm = Locator.Current.GetService<ILogManager>();
            Assert.NotNull(lm);
        }

        /// <summary>
        /// Nulls the resolver tests.
        /// </summary>
        [Fact]
        public void NullResolverTests()
        {
            IReadonlyDependencyResolver resolver = default;
            IMutableDependencyResolver resolver1 = default;
            IDependencyResolver resolver2 = default;
            Assert.Throws<ArgumentNullException>(() => resolver.GetService<ILogManager>());
            Assert.Throws<ArgumentNullException>(() => resolver.GetServices<ILogManager>());
            Assert.Throws<ArgumentNullException>(() => resolver1.ServiceRegistrationCallback(typeof(ILogManager), (IDisposable d) => { d.Dispose(); }));
            Assert.Throws<ArgumentNullException>(() => resolver2.WithResolver().Dispose());
            Assert.Throws<ArgumentNullException>(() => resolver1.Register<ILogManager>(() => new DefaultLogManager()));
            Assert.Throws<ArgumentNullException>(() => resolver1.RegisterConstant<ILogManager>(new DefaultLogManager()));
            Assert.Throws<ArgumentNullException>(() => resolver1.RegisterLazySingleton(() => new DefaultLogManager(), typeof(ILogManager)));
            Assert.Throws<ArgumentNullException>(() => resolver1.RegisterLazySingleton(() => new DefaultLogManager()));
            Assert.Throws<ArgumentNullException>(() => resolver1.UnregisterCurrent<ILogManager>());
            Assert.Throws<ArgumentNullException>(() => resolver1.UnregisterAll<ILogManager>());
        }

        /// <summary>
        /// Gets an instance of a dependency resolver to test.
        /// </summary>
        /// <returns>Dependency Resolver.</returns>
        protected abstract T GetDependencyResolver();
    }
}
