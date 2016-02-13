using System;
using System.Linq;
using TestModels;
using Xunit;

namespace PhilosophicalMonkey.Tests
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

        [Fact]
        public void GetCustomAttributeData_OnPropertyWithAttribute_ReturnsResult()
        {
            var memberInfo = Reflect.OnProperties.GetPropertyInformation(typeof(TestModel), "MyString");
            var attribData = Reflect.OnAttributes.GetCustomAttributesData(memberInfo);
            Assert.NotEmpty(attribData);
        }

        [Fact]
        public void ConstructorInfo_OnCustomAttributeData_ReturnsConstructorInfo()
        {
            var memberInfo = Reflect.OnProperties.GetPropertyInformation(typeof(TestModel), "MyString");
            var attributesData = Reflect.OnAttributes.GetCustomAttributesData(memberInfo);
            var data = attributesData.First();
            var constructorInfo = Reflect.OnAttributes.ConstructorInfo(data);
            Assert.NotEmpty(attributesData);
        }
    }
}
