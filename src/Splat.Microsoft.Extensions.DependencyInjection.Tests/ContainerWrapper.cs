using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Splat.Microsoft.Extensions.DependencyInjection.Tests
{
    internal class ContainerWrapper
    {
        private IServiceProvider _serviceProvider;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public ContainerWrapper()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            ServiceCollection.UseMicrosoftDependencyResolver();
        }

        public IServiceCollection ServiceCollection { get; } = new ServiceCollection();

        public IServiceProvider ServiceProvider => _serviceProvider ??= ServiceCollection.BuildServiceProvider();

        public void BuildAndUse() => ServiceProvider.UseMicrosoftDependencyResolver();
    }
}
