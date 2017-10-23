using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TestModels;
using Xunit;

namespace PhilosophicalMonkey.Tests
{
    public class OnTypesTests
    {
        [Fact]
        public void IsClass_OnClass_ReturnsTrue()
        {
            var t = typeof(MyAbstract);
            var result = Reflect.OnTypes.IsClass(t);
            Assert.True(result);
        }

        [Fact]
        public void IsClass_OnInterface_ReturnsFalse()
        {
            var t = typeof(IInterface);
            var result = Reflect.OnTypes.IsClass(t);
            Assert.False(result);
        }

        [Fact]
        public void IsAbstract_OnAbstractType_ReturnsTrue()
        {
            var t = typeof(MyAbstract);
            var result = Reflect.OnTypes.IsAbstract(t);
            Assert.True(result);
        }

        [Fact]
        public void IsGenericType_OnIEnumerableT_ReturnsTrue()
        {
            var t = typeof(System.Collections.Generic.IEnumerable<>);
            var result = Reflect.OnTypes.IsGenericType(t);
            Assert.True(result);
        }

        [Fact]
        public void IsGenericType_OnIEnumerable_ReturnsFalse()
        {
            var t = typeof(System.Collections.IEnumerable);
            var result = Reflect.OnTypes.IsGenericType(t);
            Assert.False(result);
        }

        [Fact]
        public void IsInterface_OnIEnumerable_ReturnsTrue()
        {
            var t = typeof(System.Collections.IEnumerable);
            var result = Reflect.OnTypes.IsInterface(t);
            Assert.True(result);
        }

        [Fact]
        public void IsInterface_OnAddress_ReturnsFalse()
        {
            var t = typeof(Address);
            var result = Reflect.OnTypes.IsInterface(t);
            Assert.False(result);
        }

        [Fact]
        public void IsPrimitive_OnInt_ReturnsTrue()
        {
            var t = typeof(int);
            var result = Reflect.OnTypes.IsPrimitive(t);
            Assert.True(result);
        }

        [Fact]
        public void IsPrimitive_OnAddress_ReturnsFalse()
        {
            var t = typeof(Address);
            var result = Reflect.OnTypes.IsPrimitive(t);
            Assert.False(result);
        }

        [Fact]
        public void IsSimple_OnAddress_ReturnsFalse()
        {
            var t = typeof(Address);
            var result = Reflect.OnTypes.IsSimple(t);
            Assert.False(result);
        }

        [Fact]
        public void IsSimple_OnLong_ReturnsTrue()
        {
            var t = typeof(long);
            var result = Reflect.OnTypes.IsSimple(t);
            Assert.True(result);
        }

        [Fact]
        public void IsSimple_OnString_ReturnsTrue()
        {
            var t = typeof(string);
            var result = Reflect.OnTypes.IsSimple(t);
            Assert.True(result);
        }

        [Fact]
        public void IsSimple_OnDateTime_ReturnsTrue()
        {
            var t = typeof(DateTime);
            var result = Reflect.OnTypes.IsSimple(t);
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
        public void GetAssemblies_FromTypes_ReturnsTestModelsAssembly()
        {
            Type t = typeof(Address);
            var assemblies = Reflect.OnTypes.GetAssemblies(new Type[] { t }).ToList();

            Assert.Equal(1, assemblies.Count());
            Assert.Contains("TestModels", assemblies.First().FullName);
        }

        [Fact]
        public void GetTypesFromNamespace_FromTypes_ReturnsExpectedAssemblies()
        {
            var assembly = typeof(TestModel).GetTypeInfo().Assembly;
            var types = Reflect.OnTypes.GetTypesFromNamespace(assembly, "TestModels");

            Assert.Contains(types, t => t == typeof(TestModel));
        }

        [Fact]
        public void IsAssignableFrom_TypeNotInInheritance_ReturnsFalse()
        {
            var abstraction = typeof(IInterface);
            var concretion = typeof(Address);
            var result = Reflect.OnTypes.IsAssignable(concretion, abstraction);
            Assert.False(result);
        }

        [Fact]
        public void IsAssignableFrom_TypeIsInInheritance_ReturnsTrue()
        {
            var abstraction = typeof(IInterface);
            var concretion = typeof(Person);
            var result = Reflect.OnTypes.IsAssignable(concretion, abstraction);
            Assert.True(result);
        }


        [Fact]
        public void GetGenericArguments_OnTypeWithNoGenericArgs_ReturnsEmptyList()
        {
            var type = typeof(Person);
            var result = Reflect.OnTypes.GetGenericArguments(type);
            Assert.Empty(result);
        }

        [Fact]
        public void GetGenericArguments_OnTypeWith1GenericArg_ReturnsListWith1Item()
        {
            var type = typeof(IEnumerable<Person>);
            var result = Reflect.OnTypes.GetGenericArguments(type);
            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count());
        }

        [Fact]
        public void ImplicitConvert_UsingTypeOperator_ReturnsValue()
        {
            TestId id = 1;
            var result = Reflect.OnTypes.ImplicitConvert<int>(id);
            Assert.Equal(1, result);
        }

        [Fact]
        public void ImplicitConvert_UsingTypeOperatorToImplicitContainer_ReturnsValue()
        {
            int id = 1;
            var result = Reflect.OnTypes.ImplicitConvert<TestId>(id);
            Assert.Equal(1, (int)result);
        }

        [Fact]
        public void ImplicitConvert_UsingTypeOperatorToImplicitContainerNotExist_ThrowsInvalidOperationException()
        {
            long id = 1;
            Assert.Throws<InvalidOperationException>(() => Reflect.OnTypes.ImplicitConvert<TestId>(id));
        }

        [Fact]
        public void ExplicitConvert_UsingTypeOperatorToImplicitContainer_ReturnsValue()
        {
            TestId id = 1;
            var result = Reflect.OnTypes.ExplicitConvert<bool>(id);
            Assert.True(result);
        }
    }
}
