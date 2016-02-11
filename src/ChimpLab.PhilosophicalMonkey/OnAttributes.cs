using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace ChimpLab.PhilosophicalMonkey
{
    public static partial class Reflect
    {
        public static class OnAttributes
        {
            public static T GetAttribute<T>(MemberInfo member, bool isRequired = false) where T : Attribute
            {
                Type t = typeof(T);
                var attribute = member.GetCustomAttribute(t, false);

                if (attribute == null && isRequired)
                {
                    throw new ArgumentException(
                        string.Format(CultureInfo.InvariantCulture,
                            "The {0} attribute is required on member {1}",
                            t.Name,
                            member.Name));
                }

                return (T)attribute;
            }

            public static IEnumerable<CustomAttributeData> GetCustomAttributesData(MemberInfo memberInfo)
            {

#if DOTNET5_4 || DNXCORE50
                return memberInfo.CustomAttributes;
#endif

#if NET46 || NET452 || NET451 || DNX46 || DNX452 || DNX451
                return memberInfo.GetCustomAttributesData();
#endif
                throw new NotImplementedException();
            }

            public static ConstructorInfo ConstructorInfo(CustomAttributeData attributeData)
            {
#if DOTNET5_4 || DNXCORE50
                //TODO: check against arguments
                return attributeData.AttributeType.GetTypeInfo().DeclaredConstructors.First();
#endif

#if NET46 || NET452 || NET451 || DNX46 || DNX452 || DNX451
                return attributeData.Constructor;
#endif
                throw new NotImplementedException();
            }
        }
    }
}
