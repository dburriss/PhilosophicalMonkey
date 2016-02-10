using System;
using TestModels;
using Xunit;

namespace ChimpLab.PhilosophicalMonkey.Tests
{
    public class OnAttributesTests
    {

        [Fact]
        public void GetAttribute_WithAttributePreset_ReturnsAttribute()
        {
            var memberInfo = Reflect.OnProperties.GetPropertyInformation(typeof(TestModel), "MyString");
            var attribute = Reflect.OnAttributes.GetAttribute<PickMeAttribute>(memberInfo);

            Assert.NotNull(attribute);
        }

        [Fact]
        public void GetAttribute_WithoutAttributePresetAndRequired_ThrowsArgumentException()
        {
            var memberInfo = Reflect.OnProperties.GetPropertyInformation(typeof(TestModel), "Id");

            Assert.Throws<ArgumentException>(() => Reflect.OnAttributes.GetAttribute<PickMeAttribute>(memberInfo, true));
        }
    }
}
