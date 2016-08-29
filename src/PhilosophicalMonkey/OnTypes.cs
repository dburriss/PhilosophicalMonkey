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

            public static bool IsSimple(Type type)
            {
#if COREFX
                return type.GetTypeInfo().IsPrimitive || type == typeof(string) || type == typeof(DateTime);
#endif
#if NET
                return type.IsPrimitive || type == typeof(string) || type == typeof(DateTime);
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
        }
    
    }
}
