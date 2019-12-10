using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;

namespace Test.SimpleKit.Infrastructure.Repository.EfCore.MySQL.Extensions
{
    public static class CustomFixtureExtensions
    {
        public static void ChooseTheMostConstructorWithParameter(this IFixture fixture, Type type)
        {
            fixture.Customize(new ConstructorCustomization(type, new TheMostParametersConstructor()));
        }
        internal class TheMostParametersConstructor : IMethodQuery
        {
            public IEnumerable<IMethod> SelectMethods(Type type)
            {
                return type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                    .Select(x => new ConstructorMethod(x));
            }
        }
    }
}