using System;
using System.Collections.Generic;
using System.Reflection;
using static PhilosophicalMonkey.Reflect;

namespace PhilosophicalMonkey
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> GetInterfaces(this Type type)
        {
            return OnTypes.GetInterfaces(type);
        }

        public static bool IsAbstract(this Type type)
        {
            return OnTypes.IsAbstract(type);
        }

        public static bool IsInterface(this Type type)
        {
            return OnTypes.IsInterface(type);
        }

        public static bool IsClass(this Type type)
        {
            return OnTypes.IsClass(type);
        }

        public static bool IsGenericType(this Type type)
        {
            return OnTypes.IsGenericType(type);
        }

        public static bool IsPrimitive(this Type type)
        {
            return OnTypes.IsPrimitive(type);
        }

        public static Assembly Assembly(this Type type)
        {
            return OnTypes.GetAssembly(type);
        }

        public static bool IsAssignableFrom(this Type abstraction, Type concretion)
        {
            return OnTypes.IsAssignable(concretion, abstraction);
        }

        public static IEnumerable<Type> GetGenericArguments(this Type type)
        {
            return OnTypes.GetGenericArguments(type);
        }
    }
}
