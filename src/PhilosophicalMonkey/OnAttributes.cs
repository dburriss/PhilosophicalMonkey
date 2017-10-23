using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace PhilosophicalMonkey
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

#if COREFX
                return memberInfo.CustomAttributes;
#elif NET
                return memberInfo.GetCustomAttributesData();
#endif
                throw new NotImplementedException();
            }

            public static ConstructorInfo ConstructorInfo(CustomAttributeData attributeData)
            {
#if COREFX
                //TODO: check against arguments
                return attributeData.AttributeType.GetTypeInfo().DeclaredConstructors.First();
#elif NET
                return attributeData.Constructor;
#endif
                throw new NotImplementedException();
            }
        }
    }
}
