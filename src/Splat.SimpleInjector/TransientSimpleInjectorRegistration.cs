// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using SimpleInjector;

namespace Splat.SimpleInjector
{
    internal class TransientSimpleInjectorRegistration : Registration
    {
        public TransientSimpleInjectorRegistration(Container container, Type implementationType, Func<object?>? instanceCreator = null)
            : base(Lifestyle.Transient, container, implementationType, instanceCreator!)
        {
        }

        public override Expression BuildExpression() => BuildTransientExpression();
    }
}
