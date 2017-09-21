using System;
using System.Linq;
using TestModels;
using Xunit;

namespace PhilosophicalMonkey.Tests
{
    public class OnMethodsTests
    {

        [Fact]
        public void GetMethods_OnClassWithNoMethodsExcludingCtorsAndSpecials_OnlyHasConstructor()
        {
            var methods = Reflect.OnMethods.GetMethods<Address>(true, true);
            Assert.Equal(4, methods.Count());
        }

        [Fact]
        public void GetMethods_OnClassWithNoMethods_ContainsAll()
        {
            var methods = Reflect.OnMethods.GetMethods<Address>();
            Assert.Equal(8, methods.Count());
        }

        [Fact]
        public void GetMethods_OnClassWithNoMethodsExcludingSpecials_ContainsObjectMethods()
        {
            var methods = Reflect.OnMethods.GetMethods<Address>(excludeContructor: false, excludeSpecials: true);
            Assert.Equal(4, methods.Count());
        }

        [Fact]
        public void GetMethod_WhenMethodDoesNotExist_ReturnsNull()
        {
            var method = Reflect.OnMethods.GetMethod<Person>("IDoNotExist");
            Assert.Null(method);
        }

        [Fact]
        public void GetMethod_WhenMethodDoesExist_ReturnsMethod()
        {
            var method = Reflect.OnMethods.GetMethod<Person>("CreateOrder");
            Assert.NotNull(method);
        }

        [Fact]
        public void GetMethod_WhenOverloaded_ReturnsMethod()
        {
            var method = Reflect.OnMethods.GetMethod<OverloadedClass>("Invert", typeof(int));
            Assert.NotNull(method);
        }

        [Fact]
        public void Call_OverloadedMethod_ReturnsValue()
        {
            var instance = new OverloadedClass();
            var result = Reflect.OnMethods.Call<OverloadedClass, int>(instance, "Invert", -1);
            Assert.Equal(1, result);
        }

        [Fact]
        public void Call_PrivateMethod_ReturnsValue()
        {
            var instance = new OverloadedClass();
            var result = Reflect.OnMethods.Call<OverloadedClass, Guid>(instance, "Correlation");
            Assert.NotEqual(Guid.Empty, result);
        }

        [Fact]
        public void Call_VoidMethod_DoesntThrow()
        {
            var instance = new OverloadedClass();
            Reflect.OnMethods.VoidCall(instance, "Print");
        }

        [Fact]
        public void CallStatic_Operator_ReturnsValue()
        {
            TestId instance = 1;
            var result = Reflect.OnMethods.Call<TestId, int>(instance, "op_Implicit", instance);
            Assert.Equal(1, result);
        }
    }
}
