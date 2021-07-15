using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Splat.Tests.ServiceLocation
{
    /// <summary>
    /// Unit Tests for the Modern Dependency Resolver.
    /// </summary>
    public sealed class MicrosoftDependencyResolverTests : BaseDependencyResolverTests<MicrosoftDependencyResolver>
    {
        /// <summary>
        /// Test to ensure container allows registration with null service type.
        /// Should really be brought down to the <see cref="BaseDependencyResolverTests{T}"/>,
        /// it fails for some of the DIs.
        /// </summary>
        [Fact]
        public void Can_Register_And_Resolve_Null_Types()
        {
            var resolver = GetDependencyResolver();
            var foo = 5;
            resolver.Register(() => foo, null!);

            var value = resolver.GetService(null!);
            Assert.Equal(foo, value);

            var bar = 4;
            var contract = "foo";
            resolver.Register(() => bar, null!, contract);

            value = resolver.GetService(null!, contract);
            Assert.Equal(bar, value);
        }

        /// <inheritdoc />
        protected override MicrosoftDependencyResolver GetDependencyResolver() => new();
    }
}
