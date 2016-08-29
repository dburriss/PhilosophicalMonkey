using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace PhilosophicalMonkey
{
    public static class Extensions
    {
        public static T GetAttribute<T>(this MemberInfo member, bool isRequired = false) where T : Attribute
        {
            Type t = typeof(T);

#if COREFX
            var attribute = member.GetCustomAttribute(t, false);
#endif
#if NET
            var attribute = member.GetCustomAttribute(t, false);
#endif

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
