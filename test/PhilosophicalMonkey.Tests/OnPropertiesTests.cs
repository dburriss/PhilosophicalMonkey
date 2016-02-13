using System;
using System.Linq.Expressions;
using System.Reflection;
using TestModels;
using Xunit;

namespace PhilosophicalMonkey.Tests
{
    public class OnPropertiesTests
    {

        [Fact]
        public void GetPropertyInformation_FromClass_ReturnsMemberInfo()
        {
            Expression<Func<TestModel, object>> exp = x => x.MyString;
            MemberInfo memberInfo = Reflect.OnProperties.GetPropertyInformation<TestModel>(exp);

            Assert.NotNull(memberInfo);
        }

        [Fact]
        public void GetPropertyInformation_FromClassUsingMagicString_ReturnsMemberInfo()
        {
            MemberInfo memberInfo = Reflect.OnProperties.GetPropertyInformation(typeof(TestModel), "MyString");
            Assert.NotNull(memberInfo);
        }

        [Fact]
        public void GetPropertyName_FromClass_ReturnsPropertyNameAsString()
        {
            Expression<Func<TestModel, object>> exp = x => x.MyString;
            string name = Reflect.OnProperties.GetPropertyName<TestModel>(exp);

            Assert.Equal("MyString", name);
        }

        [Fact]
        public void GetPropertyType_FromClass_ReturnsPropertyNameAsString()
        {
            Expression<Func<TestModel, object>> exp = x => x.MyString;
            Type t = Reflect.OnProperties.GetPropertyType<TestModel>(exp);

            Assert.Equal(typeof(string), t);
        }

        [Fact]
        public void GetValue_FromInstance_ReturnsValue()
        {
            Expression<Func<TestModel, object>> exp = x => x.MyString;
            var obj = new TestModel
            {
                Id = 1,
                MyString = "The Value"
            };

            var result = Reflect.OnProperties.GetValue<TestModel, object>(obj, exp);

            Assert.Equal("The Value", result);
        }

        [Fact]
        public void NullSafeGetValue_FromInstance_ReturnsValue()
        {
            Expression<Func<TestModel, object>> exp = x => x.MyString;
            var obj = new TestModel
            {
                Id = 1,
                MyString = "The Value"
            };

            var result = Reflect.OnProperties.NullSafeGetValue<TestModel, object>(obj, exp, null);

            Assert.Equal("The Value", result);
        }

        [Fact]
        public void NullSafeGetValue_FromInstanceWithNull_ReturnsSetDefaultValue()
        {
            //Expression<Func<TestModel, object>> exp = x => x.MyString;
            var obj = new TestModel
            {
                Id = 1,
                MyString = null
            };

            var result = Reflect.OnProperties.NullSafeGetValue<TestModel, object>(obj, x => x.MyString, "Default");

            Assert.Equal("Default", result);
        }

        [Fact]
        public void NullSafeGetValue_FromInstanceBaseDate_ReturnsDate()
        {
            Expression<Func<TestModel, object>> exp = x => x.CreatedAt;
            var obj = new TestModel
            {
                Id = 1,
                MyString = null,
                CreatedAt = DateTime.MaxValue
            };

            var date = Reflect.OnProperties.NullSafeGetValue<TestModel, object>(obj, exp, DateTime.MinValue);

            Assert.Equal(DateTime.MaxValue, date);
        }

        [Fact]
        public void NullSafeGetValue_FromInstanceNestedProperty_ReturnsValue()
        {
            Expression<Func<TestModel, object>> exp = x => x.Nested.Deep;
            var obj = new TestModel
            {
                Id = 1,
                MyString = null,
                CreatedAt = DateTime.MaxValue,
                Nested = new NestedModel
                {
                    Deep = "Going Deep"
                }
            };

            var result = Reflect.OnProperties.NullSafeGetValue<TestModel, object>(obj, exp, string.Empty);

            Assert.Equal("Going Deep", result);
        }

        [Fact]
        public void GetFullPropertyPathName_FromInstanceNestedProperty_ReturnsFullPath()
        {
            Expression<Func<TestModel, object>> exp = x => x.Nested.Deep;
            var obj = new TestModel
            {
                Id = 1,
                MyString = null,
                CreatedAt = DateTime.MaxValue,
                Nested = new NestedModel
                {
                    Deep = "Going Deep"
                }
            };

            var result = Reflect.OnProperties.GetFullPropertyPathName<TestModel, object>(exp);

            Assert.Equal("Nested.Deep", result);
        }

        [Fact]
        public void GetPropertyInfo_FromInstanceNestedPropertyUsingPathString_ReturnsPropertyInfo()
        {
            var deepType = Reflect.OnProperties.GetPropertyInfoFromPath<TestModel>("Nested.Deep");
            var deepDeclaringType = Reflect.OnProperties.GetPropertyInfoFromPath<TestModel>("Nested");

            Assert.Equal(typeof(string), deepType.PropertyType);
            Assert.Equal(typeof(NestedModel), deepDeclaringType.PropertyType);
        }

        //http://stackoverflow.com/questions/238765/given-a-type-expressiontype-memberaccess-how-do-i-get-the-field-value
        //http://blog.marcgravell.com/2008/10/express-yourself.html
        //http://www.codeproject.com/Articles/17575/Lambda-Expressions-and-Expression-Trees-An-Introdu
        //http://msdn.microsoft.com/en-us/library/bb882521(v=vs.90).aspx
        //[Fact]
        public void BuildingAnExpression()
        {
            //var t = typeof(TestModel);
            //var param = Expression.Parameter(t, "x");
            //MemberInfo memberInfo = t.GetProperties().Where(x => x.Name == "MyString").First();
            //Member
            //MemberExpression exp = MemberExpression.Add()
            //Expression e = Expression.MakeMemberAccess();
            //var body = Expression.MakeMemberAccess(, memberInfo);
            //Expression<Func<TestModel, object>> exp = Expression.Lambda(;

        }
    }
}
