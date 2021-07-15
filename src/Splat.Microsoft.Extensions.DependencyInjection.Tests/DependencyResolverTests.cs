// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Splat.Common.Test;
using Splat.NLog;
using Xunit;

namespace Splat.Microsoft.Extensions.DependencyInjection.Tests
{
    /// <summary>
    /// Tests to show the <see cref="MicrosoftDependencyResolver"/> works correctly.
    /// </summary>
    public class DependencyResolverTests
    {
        /// <summary>
        /// Should resolve views.
        /// </summary>
        [Fact]
        public void MicrosoftDependencyResolver_Should_Resolve_Views()
        {
            var wrapper = new ContainerWrapper();
            var services = wrapper.ServiceCollection;
            services.AddTransient<IViewFor<ViewModelOne>, ViewOne>();
            services.AddTransient<IViewFor<ViewModelTwo>, ViewTwo>();

            wrapper.BuildAndUse();

            var viewOne = Locator.Current.GetService(typeof(IViewFor<ViewModelOne>));
            var viewTwo = Locator.Current.GetService(typeof(IViewFor<ViewModelTwo>));

            viewOne.Should().NotBeNull();
            viewOne.Should().BeOfType<ViewOne>();
            viewTwo.Should().NotBeNull();
            viewTwo.Should().BeOfType<ViewTwo>();
        }

        /// <summary>
        /// Should resolve views.
        /// </summary>
        [Fact]
        public void MicrosoftDependencyResolver_Should_Resolve_Named_View()
        {
            var wrapper = new ContainerWrapper();
            var services = wrapper.ServiceCollection;
            services.AddTransient<IViewFor<ViewModelTwo>, ViewTwo>();

            wrapper.BuildAndUse();

            var viewTwo = Locator.Current.GetService(typeof(IViewFor<ViewModelTwo>));

            viewTwo.Should().NotBeNull();
            viewTwo.Should().BeOfType<ViewTwo>();
        }

        /// <summary>
        /// Should resolve view models.
        /// </summary>
        [Fact]
        public void MicrosoftDependencyResolver_Should_Resolve_View_Models()
        {
            var wrapper = new ContainerWrapper();
            var services = wrapper.ServiceCollection;
            services.AddTransient<ViewModelOne>();
            services.AddTransient<ViewModelTwo>();

            wrapper.BuildAndUse();

            var vmOne = Locator.Current.GetService<ViewModelOne>();
            var vmTwo = Locator.Current.GetService<ViewModelTwo>();

            vmOne.Should().NotBeNull();
            vmTwo.Should().NotBeNull();
        }

        /// <summary>
        /// Should resolve screen.
        /// </summary>
        [Fact]
        public void MicrosoftDependencyResolver_Should_Resolve_Screen()
        {
            var wrapper = new ContainerWrapper();
            var services = wrapper.ServiceCollection;
            services.AddSingleton<IScreen>(new MockScreen());

            wrapper.BuildAndUse();

            var screen = Locator.Current.GetService<IScreen>();

            screen.Should().NotBeNull();
            screen.Should().BeOfType<MockScreen>();
        }

        /// <summary>
        /// Should unregister all.
        /// </summary>
        [Fact]
        public void MicrosoftDependencyResolver_Should_UnregisterAll()
        {
            var wrapper = new ContainerWrapper();
            var services = wrapper.ServiceCollection;

            services.AddSingleton<IScreen>(new MockScreen());

            Locator.CurrentMutable.HasRegistration(typeof(IScreen))
                .Should().BeTrue();

            Locator.CurrentMutable.UnregisterAll(typeof(IScreen));

            var result = Locator.Current.GetService<IScreen>();
            result.Should().BeNull();
        }

        /// <summary>
        /// Should throw an exception if service registration call back called.
        /// </summary>
        [Fact]
        public void MicrosoftDependencyResolver_Should_Throw_If_ServiceRegistionCallback_Called()
        {
            var wrapper = new ContainerWrapper();
            wrapper.BuildAndUse();

            var result = Record.Exception(() =>
                Locator.CurrentMutable.ServiceRegistrationCallback(typeof(IScreen), disposable => { }));

            result.Should().BeOfType<NotImplementedException>();
        }

        /// <summary>
        /// Should throw an exception if trying to register services when the container is registered as immutable.
        /// </summary>
        [Fact]
        public void MicrosoftDependencyResolver_Should_Throw_If_Attempt_Registration_After_Build()
        {
            var wrapper = new ContainerWrapper();

            wrapper.BuildAndUse();

            var result = Record.Exception(() => Locator.CurrentMutable.Register(() => new ViewOne()));

            result.Should().BeOfType<InvalidOperationException>();
        }

        /// <summary>
        /// Tests to ensure NLog registers correctly with different service locators.
        /// Based on issue reported in #553.
        /// </summary>
        [Fact]
        public void ILogManager_Resolvable()
        {
            var wrapper = new ContainerWrapper();
            var services = wrapper.ServiceCollection;

            // Setup NLog for Logging (doesn't matter if I actually configure NLog or not)
            var funcLogManager = new FuncLogManager(type => new NLogLogger(LogResolver.Resolve(type)));
            services.AddSingleton<ILogManager>(funcLogManager);

            wrapper.BuildAndUse();

            // Get the ILogManager instance.
            ILogManager lm = Locator.Current.GetService<ILogManager>();

            var mgr = lm.GetLogger<NLogLogger>();
            Assert.NotNull(mgr);
        }
    }
}
