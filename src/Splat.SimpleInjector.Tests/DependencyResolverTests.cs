// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using FluentAssertions;

using SimpleInjector;

using Splat.Common.Test;
using Splat.SimpleInjector;

using Xunit;

namespace Splat.Simplnjector
{
    /// <summary>
    /// Tests to show the <see cref="SimpleInjectorDependencyResolver"/> works correctly.
    /// </summary>
    public class DependencyResolverTests
    {
        /// <summary>
        /// Simples the injector dependency resolver should resolve a view model.
        /// </summary>
        [Fact]
        public void SimpleInjectorDependencyResolver_Should_Resolve_View_Model()
        {
            var container = new Container();
            container.Register<ViewModelOne>();
            container.UseSimpleInjectorDependencyResolver(new SimpleInjectorInitializer());

            var viewModel = Locator.Current.GetService(typeof(ViewModelOne));

            viewModel.Should().NotBeNull();
            viewModel.Should().BeOfType<ViewModelOne>();
        }

        /// <summary>
        /// Simples the injector dependency resolver should resolve a view model.
        /// </summary>
        [Fact]
        public void SimpleInjectorDependencyResolver_Should_Resolve_View_Model_Directly()
        {
            var container = new SimpleInjectorInitializer();
            container.Register(() => new ViewModelOne());

            var viewModel = container.GetService<ViewModelOne>();

            viewModel.Should().NotBeNull();
            viewModel.Should().BeOfType<ViewModelOne>();
        }

        /// <summary>
        /// Simples the injector dependency resolver should resolve a view.
        /// </summary>
        [Fact]
        public void SimpleInjectorDependencyResolver_Should_Resolve_View()
        {
            var container = new Container();
            container.Register<IViewFor<ViewModelOne>, ViewOne>();
            container.UseSimpleInjectorDependencyResolver(new SimpleInjectorInitializer());

            var view = Locator.Current.GetService(typeof(IViewFor<ViewModelOne>));

            view.Should().NotBeNull();
            view.Should().BeOfType<ViewOne>();
        }

        /// <summary>
        /// Simples the injector dependency resolver should resolve the screen.
        /// </summary>
        [Fact]
        public void SimpleInjectorDependencyResolver_Should_Resolve_Screen()
        {
            var container = new Container();
            container.RegisterSingleton<IScreen, MockScreen>();
            container.UseSimpleInjectorDependencyResolver(new SimpleInjectorInitializer());

            var screen = Locator.Current.GetService(typeof(IScreen));

            screen.Should().NotBeNull();
            screen.Should().BeOfType<MockScreen>();
        }

        /// <summary>
        /// Should not throw during initialization of ReactiveUI.
        /// </summary>
        [Fact]
        public void SimpleInjectorDependencyResolver_Splat_Initialization_ShouldNotThrow()
        {
            Container container = new();
            SimpleInjectorInitializer initializer = new();

            Locator.SetLocator(initializer);
            Locator.CurrentMutable.InitializeSplat();
            container.UseSimpleInjectorDependencyResolver(initializer);
        }

        /// <summary>
        /// Should resolve dependency registered during Splat initialization.
        /// </summary>
        [Fact]
        public void SimpleInjectorDependencyResolver_ShouldResolveSplatRegisteredDependency()
        {
            Container container = new();
            SimpleInjectorInitializer initializer = new();

            Locator.SetLocator(initializer);
            Locator.CurrentMutable.InitializeSplat();
            container.UseSimpleInjectorDependencyResolver(initializer);

            var dependency = Locator.Current.GetService(typeof(ILogger)) as ILogger;
            Assert.NotNull(dependency);
        }

        /// <summary>
        /// Should resolve dependency registered during Splat initialization.
        /// </summary>
        [Fact]
        public void SimpleInjectorDependencyResolver_CollectionShouldNeverReturnNull()
        {
            var container = new Container();
            container.UseSimpleInjectorDependencyResolver(new SimpleInjectorInitializer());

            var views = Locator.Current.GetServices(typeof(ViewOne));
            Assert.NotNull(views);
        }
    }
}
