// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

using FluentAssertions;
using Splat.Tests.Mocks;
using Xunit;

namespace Splat.Tests
{
    /// <summary>
    /// Tests to confirm that the locator is working.
    /// </summary>
    public class LocatorSerialRegisterTests
    {
        /// <summary>
        /// Tests if the registrations are not empty on no external registrations.
        /// </summary>
        [Fact]
        public void InitializeSplat_RegistrationsNotEmptyNoRegistrations()
        {
            // this is using the internal constructor
            var testLocator = new InternalLocator();
            var logManager = testLocator.Current.GetService(typeof(ILogManager));
            var logger = testLocator.Current.GetService(typeof(ILogger));

            Assert.NotNull(logManager);
            Assert.NotNull(logger);

            Assert.IsType<DebugLogger>(logger);
            Assert.IsType<DefaultLogManager>(logManager);
        }

        /// <summary>
        /// Tests that if we use a contract it returns null entries for that type.
        /// </summary>
        [Fact]
        public void InitializeSplat_ContractRegistrationsNullNoRegistration()
        {
            var testLocator = new InternalLocator();
            var logManager = testLocator.Current.GetService(typeof(ILogManager), "test");
            var logger = testLocator.Current.GetService(typeof(ILogger), "test");

            Assert.Null(logManager);
            Assert.Null(logger);
        }

        /// <summary>
        /// Tests using the extension methods that the retrieving of the default InitializeSplat() still work.
        /// </summary>
        [Fact]
        public void InitializeSplat_ExtensionMethodsNotNull()
        {
            var testLocator = new InternalLocator();
            var logManager = testLocator.Current.GetService<ILogManager>();
            var logger = testLocator.Current.GetService<ILogger>();

            Assert.NotNull(logManager);
            Assert.NotNull(logger);

            Assert.IsType<DebugLogger>(logger);
            Assert.IsType<DefaultLogManager>(logManager);
        }

        /// <summary>
        /// Tests to make sure that the locator's fire the resolver changed notifications.
        /// </summary>
        [Fact]
        public void WithoutSuppress_NotificationsHappen()
        {
            var testLocator = new InternalLocator();
            var originalLocator = testLocator.Internal;

            int numberNotifications = 0;
            Action notificationAction = () => numberNotifications++;

            testLocator.RegisterResolverCallbackChanged(notificationAction);

            testLocator.SetLocator(new ModernDependencyResolver());
            testLocator.SetLocator(new ModernDependencyResolver());

            // 2 for the changes, 1 for the callback being immediately called.
            Assert.Equal(3, numberNotifications);

            testLocator.SetLocator(originalLocator);
        }

        /// <summary>
        /// Tests to make sure that the locator's don't fire the resolver changed notifications if they are suppressed.
        /// </summary>
        [Fact]
        public void WithSuppression_NotificationsDontHappen()
        {
            var testLocator = new InternalLocator();
            var originalLocator = testLocator.Internal;

            using (testLocator.SuppressResolverCallbackChangedNotifications())
            {
                int numberNotifications = 0;
                Action notificationAction = () => numberNotifications++;

                testLocator.RegisterResolverCallbackChanged(notificationAction);

                testLocator.SetLocator(new ModernDependencyResolver());
                testLocator.SetLocator(new ModernDependencyResolver());

                Assert.Equal(0, numberNotifications);

                testLocator.SetLocator(originalLocator);
            }
        }

        /// <summary>
        /// Tests to make sure that the locator's don't fire the resolver changed notifications if we use WithResolver().
        /// </summary>
        [Fact]
        public void WithResolver_NotificationsDontHappen()
        {
            int numberNotifications = 0;
            Action notificationAction = () => numberNotifications++;

            var testLocator = new InternalLocator();
            testLocator.RegisterResolverCallbackChanged(notificationAction);

            using (testLocator.Internal.WithResolver())
            {
                using (testLocator.Internal.WithResolver())
                {
                }
            }

            // 1 due to the fact the callback is called when we register.
            Assert.Equal(1, numberNotifications);
        }

        /// <summary>
        /// Tests to make sure that the locator's don't fire the resolver changed notifications if we use WithResolver().
        /// </summary>
        [Fact]
        public void WithResolver_NotificationsNotSuppressedHappen()
        {
            int numberNotifications = 0;
            Action notificationAction = () => numberNotifications++;

            Locator.RegisterResolverCallbackChanged(notificationAction);

            using (Locator.GetLocator().WithResolver(false))
            {
                using (Locator.GetLocator().WithResolver(false))
                {
                }
            }

            // 1 due to the fact the callback is called when we register.
            // 2 for, 1 for change to resolver, 1 for change back
            // 2 for, 1 for change to resolver, 1 for change back
            Assert.Equal(5, numberNotifications);
        }

        /// <summary>
        /// Tests to make sure that the unregister all functions correctly.
        /// This is a test when there are values registered.
        /// </summary>
        [Fact]
        public void ModernDependencyResolver_UnregisterAll_WithValuesWorks()
        {
            var currentMutable = new ModernDependencyResolver();

            var dummy1 = new DummyObjectClass1();
            var dummy2 = new DummyObjectClass2();
            var dummy3 = new DummyObjectClass3();

            var testContracts = new[] { string.Empty, "test" };

            foreach (var testContract in testContracts)
            {
                currentMutable.RegisterConstantAnd<IDummyInterface>(dummy1, testContract)
                                .RegisterConstantAnd<IDummyInterface>(dummy2, testContract)
                                .RegisterConstant<IDummyInterface>(dummy3, testContract);
            }

            foreach (var testContract in testContracts)
            {
                var items = currentMutable.GetServices<IDummyInterface>(testContract);

                items.Should().BeEquivalentTo(new IDummyInterface[] { dummy1, dummy2, dummy3 });

                currentMutable.UnregisterAll<IDummyInterface>(testContract);

                items = currentMutable.GetServices<IDummyInterface>(testContract);

                items.Should().BeEmpty();
            }
        }

        /// <summary>
        /// Tests to make sure that the unregister all functions correctly.
        /// This is a test when there are values not registered.
        /// </summary>
        [Fact]
        public void ModernDependencyResolver_UnregisterAll_NoValuesWorks()
        {
            var currentMutable = new ModernDependencyResolver();

            var items = currentMutable.GetServices<IDummyInterface>();

            items.Should().BeEmpty();

            currentMutable.UnregisterAll<IDummyInterface>();

            items = currentMutable.GetServices<IDummyInterface>();

            items.Should().BeEmpty();
        }

        /// <summary>
        /// Tests tomake sure that the unregister current functions correctly.
        /// This is a test when there are values registered.
        /// </summary>
        [Fact]
        public void ModernDependencyResolver_ConstantUnregisterCurrent_WithValuesWorks()
        {
            var dummy1 = new DummyObjectClass1();
            var dummy2 = new DummyObjectClass2();
            var dummy3 = new DummyObjectClass3();

            var currentMutable = new ModernDependencyResolver();

            var testContracts = new[] { string.Empty, "test" };

            foreach (var testContract in testContracts)
            {
                currentMutable.RegisterConstantAnd<IDummyInterface>(dummy1, testContract)
                                .RegisterConstantAnd<IDummyInterface>(dummy2, testContract)
                                .RegisterConstant<IDummyInterface>(dummy3, testContract);
            }

            foreach (var testContract in testContracts)
            {
                var items = currentMutable.GetServices<IDummyInterface>(testContract);

                items.Should().BeEquivalentTo(new IDummyInterface[] { dummy1, dummy2, dummy3 });

                currentMutable.UnregisterCurrent<IDummyInterface>(testContract);

                items = currentMutable.GetServices<IDummyInterface>(testContract);

                items.Should().BeEquivalentTo(new IDummyInterface[] { dummy1, dummy2 });
            }
        }

        /// <summary>
        /// Tests tomake sure that the unregister current functions correctly.
        /// This is a test when there are values registered.
        /// </summary>
        [Fact]
        public void ModernDependencyResolver_UnregisterCurrent_WithValuesWorks()
        {
            var currentMutable = new ModernDependencyResolver();

            var testContracts = new[] { string.Empty, "test" };

            foreach (var testContract in testContracts)
            {
                currentMutable.RegisterAnd<IDummyInterface, DummyObjectClass1>(testContract)
                                .RegisterAnd<IDummyInterface, DummyObjectClass2>(testContract)
                                .Register<IDummyInterface, DummyObjectClass3>(testContract);
            }

            foreach (var testContract in testContracts)
            {
                var items = currentMutable.GetServices<IDummyInterface>(testContract);

                items.Should().HaveCount(3);

                currentMutable.UnregisterCurrent<IDummyInterface>(testContract);

                items = currentMutable.GetServices<IDummyInterface>(testContract);

                items.Should().HaveCount(2);
            }
        }

        /// <summary>
        /// Tests to make sure that the unregister all functions correctly.
        /// This is a test when there are values not registered.
        /// </summary>
        [Fact]
        public void ModernDependencyResolver_UnregisterCurrent_NoValuesWorks()
        {
            var currentMutable = new ModernDependencyResolver();
            var items = currentMutable.GetServices<IDummyInterface>();

            items.Should().BeEmpty();

            currentMutable.UnregisterCurrent<IDummyInterface>();

            items = currentMutable.GetServices<IDummyInterface>();

            items.Should().BeEmpty();
        }

        /// <summary>
        /// Tests to make sure that the unregister all functions correctly.
        /// This is a test when there are values not registered.
        /// </summary>
        [Fact]
        public void FuncDependencyResolver_UnregisterAll()
        {
            bool unregisterAllCalled = false;
            Type type = null;
            string contract = null;

            var currentMutable = new FuncDependencyResolver(
                (funcType, funcContract) => Array.Empty<object>(),
                unregisterAll: (passedType, passedContract) =>
                {
                    unregisterAllCalled = true;
                    contract = passedContract;
                    type = passedType;
                });

            currentMutable.UnregisterAll<IDummyInterface>();
            type.Should().Be(typeof(IDummyInterface));
            contract.Should().BeNull();
            unregisterAllCalled.Should().BeTrue();

            unregisterAllCalled = false;
            currentMutable.UnregisterAll<IEnableLogger>("test");
            type.Should().Be(typeof(IEnableLogger));
            contract.Should().Be("test");
            unregisterAllCalled.Should().BeTrue();
        }

        /// <summary>
        /// Tests tomake sure that the unregister current functions correctly.
        /// This is a test when there are values registered.
        /// </summary>
        [Fact]
        public void FuncDependencyResolver_UnregisterCurrent()
        {
            bool unregisterAllCalled = false;
            Type type = null;
            string contract = null;

            var currentMutable = new FuncDependencyResolver(
                (funcType, funcContract) => Array.Empty<object>(),
                unregisterCurrent: (passedType, passedContract) =>
                {
                    unregisterAllCalled = true;
                    contract = passedContract;
                    type = passedType;
                });

            currentMutable.UnregisterCurrent<IDummyInterface>();
            type.Should().Be(typeof(IDummyInterface));
            contract.Should().BeNull();
            unregisterAllCalled.Should().BeTrue();

            unregisterAllCalled = false;
            currentMutable.UnregisterCurrent<IEnableLogger>("test");
            type.Should().Be(typeof(IEnableLogger));
            contract.Should().Be("test");
            unregisterAllCalled.Should().BeTrue();
        }
    }
}
