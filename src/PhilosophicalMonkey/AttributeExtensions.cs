using System;
using System.Globalization;
using System.Reflection;

namespace PhilosophicalMonkey
{
    public static class AttributeExtensions
    {
        public static T GetAttribute<T>(this MemberInfo member, bool isRequired = false) where T : Attribute
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
    }
}
