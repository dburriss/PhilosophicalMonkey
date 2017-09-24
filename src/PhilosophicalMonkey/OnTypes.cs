using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PhilosophicalMonkey
{
    public static partial class Reflect
    {
        public static class OnTypes
        {

            public static IEnumerable<Type> GetTypesFromNamespace(Assembly assembly, params string[] @namespaces)
            {
                return assembly.GetTypes().Where(type => @namespaces.Contains(type.Namespace));
            }

            public static Type[] GetAllExportedTypes(IEnumerable<Assembly> assemblies)
            {
                List<Type> result = new List<Type>();
                foreach (var ass in assemblies)
                {
                    result.AddRange(ass.GetExportedTypes());
                }
                return result.ToArray();
            }

            public static Type[] GetAllTypes(IEnumerable<Assembly> assemblies)
            {
                List<Type> result = new List<Type>();
                foreach (var ass in assemblies)
                {
                    result.AddRange(ass.GetTypes());
                }
                return result.ToArray();
            }


            public static IEnumerable<Type> GetInterfaces(Type type)
            {
#if COREFX
                return type.GetTypeInfo().ImplementedInterfaces;
#endif
#if NET
            return type.GetInterfaces().AsEnumerable();
#endif
                throw new NotImplementedException();
            }

            public static bool IsAbstract(Type type)
            {
#if COREFX
                return type.GetTypeInfo().IsAbstract;
#endif
#if NET
                return type.IsAbstract;
#endif
                throw new NotImplementedException();
            }

            public static bool IsInterface(Type type)
            {
#if COREFX
                return type.GetTypeInfo().IsInterface;
#endif
#if NET
                return type.IsInterface;
#endif
                throw new NotImplementedException();
            }

            public static bool IsClass(Type type)
            {
#if COREFX
                return type.GetTypeInfo().IsClass;
#endif
#if NET
                return type.IsClass;
#endif
                throw new NotImplementedException();
            }

            public static bool IsGenericType(Type type)
            {
#if COREFX
                return type.GetTypeInfo().IsGenericType;
#endif
#if NET
                return type.IsGenericType;
#endif
                throw new NotImplementedException();
            }

            public static bool IsPrimitive(Type type)
            {
#if COREFX
                return type.GetTypeInfo().IsPrimitive;
#endif
#if NET
                return type.IsPrimitive;
#endif
                throw new NotImplementedException();
            }

            private static Func<Type, bool> isSimple = type => type == typeof(string)
                                                                || type == typeof(DateTime)
                                                                || type == typeof(DateTimeOffset);
            public static bool IsSimple(Type type)
            {
#if COREFX
                return type.GetTypeInfo().IsPrimitive || isSimple(type);
#endif
#if NET
                return type.IsPrimitive || isSimple(type);
#endif
                throw new NotImplementedException();
            }

            public static IEnumerable<Assembly> GetAssemblies(IEnumerable<Type> types)
            {
                foreach (var type in types)
                {
                    yield return GetAssembly(type);
                }
            }

            public static Assembly GetAssembly(Type type)
            {
#if COREFX
                return type.GetTypeInfo().Assembly;
#endif
#if NET
                return type.Assembly;
#endif
                throw new NotImplementedException();
            }

            public static bool IsAssignable(Type concretion, Type abstraction)
            {
#if COREFX
                return abstraction.GetTypeInfo().IsAssignableFrom(concretion.GetTypeInfo());
#endif
#if NET
                return abstraction.IsAssignableFrom(concretion);
#endif
                throw new NotImplementedException();
            }


            public static IEnumerable<Type> GetGenericArguments(Type type)
            {
#if COREFX
                return type.GetGenericArguments();
#endif
#if NET
                return type.GetGenericArguments();
#endif
                throw new NotImplementedException();
            }

            public static T ImplicitConvert<T>(object obj)
            {
                var toType = typeof(T);
                return (T)ImplicitConvert(toType, obj);
            }

            public static object ImplicitConvert(Type toType, object obj)
            {
                var op = "op_Implicit";                
                return Call(toType, op, obj);
            }

            public static T ExplicitConvert<T>(object obj)
            {
                var toType = typeof(T);
                return (T)ExplicitConvert(toType, obj);
            }

            public static object ExplicitConvert(Type toType, object obj)
            {
                var op = "op_Explicit";
                return Call(toType, op, obj);
            }

            private static object Call(Type toType, string name, object obj)
            {
                var argType = obj.GetType();
                var argMi = OnMethods.GetMethod(argType, name, new Type[] { argType });

                if (argMi != null)
                {
                    return argMi.Invoke(obj, new[] { obj });
                }

                var toMi = OnMethods.GetMethod(toType, name, new Type[] { argType });
                if (toMi != null)
                {
                    return toMi.Invoke(obj, new[] { obj });
                }

                throw new InvalidOperationException($"No {name} exists on type {toType.Name} or {argType.Name}");
            }

        }
    }
}
