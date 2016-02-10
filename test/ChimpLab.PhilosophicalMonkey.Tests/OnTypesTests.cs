using ChimpLab.PhilosophicalMonkey.Tests.Models;
using System;
using System.Linq;
using System.Reflection;
using TestModels;
using Xunit;

namespace ChimpLab.PhilosophicalMonkey.Tests
{
    public class OnTypesTests
    {
        [Fact]
        public void IsAbstract_OnAbstractType_ReturnsTrue()
        {
            var t = typeof(MyAbstract);
            var result = Reflect.OnTypes.IsAbstract(t);
            Assert.True(result);
        }

        [Fact]
        public void GetAllExportedTypes_FromAsseblyWithPrivateClass_FetchesTypesButNotPrivateType()
        {
            var assembly = typeof(TestModel).GetTypeInfo().Assembly;
            var types = Reflect.OnTypes.GetAllExportedTypes(new Assembly[] { assembly });

            Assert.True(types.Count() > 1);
            Assert.False(types.Any(x => x.Name == "PrivateI"));
        }

        [Fact]
        public void GetAllTypes_FromAsseblyWithPrivateClass_FetchesPrivateTypes()
        {
            var assembly = typeof(TestModel).GetTypeInfo().Assembly;
            var types = Reflect.OnTypes.GetAllTypes(new Assembly[] { assembly });

            Assert.True(types.Count() > 1);
            Assert.True(types.Any(x => x.Name == "PrivateI"));
        }

        [Fact]
        public void GetAssemblies_FromTypes_ReturnsExpectedAssemblies()
        {
            Type t = typeof(Address);
            var assemblies = Reflect.OnTypes.GetAssemblies(new Type[] { t });

            Assert.Equal(1, assemblies.Count());
            Assert.Contains("ChimpLab.PhilosophicalMonkey.Tests", assemblies.First().FullName);
        }

        [Fact]
        public void GetTypesFromNamespace_FromTypes_ReturnsExpectedAssemblies()
        {
            var assembly = typeof(TestModel).GetTypeInfo().Assembly;
            var types = Reflect.OnTypes.GetTypesFromNamespace(assembly, "TestModels");

            Assert.Contains(types, t => t == typeof(TestModel));
        }

    }
}
