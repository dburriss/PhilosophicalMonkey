using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PhilosophicalMonkey
{
    public static partial class Reflect
    {
        public static class OnMethods
        {
            public static IEnumerable<MethodInfo> GetMethods<T>(string name)
            {
                return GetMethods(typeof(T)).Where(mi => mi.Name == name);
            }

            public static IEnumerable<MethodInfo> GetMethods<T>(bool excludeContructor = false, bool excludeSpecials = false)
            {
                return GetMethods(typeof(T), excludeContructor, excludeSpecials);
            }
            
            public static IEnumerable<MethodInfo> GetMethods(Type type, bool excludeContructor = false, bool excludeSpecials = false)
            {
                Func<MethodInfo, bool> predicate = m => FilterMethodInfo(m, excludeContructor, excludeSpecials);
                return GetMethods(type, predicate);
            }

            public static IEnumerable<MethodInfo> GetMethods<T>(Func<MethodInfo, bool> predicate)
            {
                var type = typeof(T);
                return GetMethods(type, predicate);
            }

            public static IEnumerable<MethodInfo> GetMethods(Type type, Func<MethodInfo, bool> predicate)
            {
#if COREFX
                var methods =  type.GetTypeInfo().DeclaredMethods.Where(predicate);
                return methods;
#endif
#if NET
                return type.GetMethods().Where(predicate);
#endif
                throw new NotImplementedException();
            }

            private static bool FilterMethodInfo(MethodInfo mi, bool excludeContructor, bool excludeSpecials)
            {
                if(mi.IsConstructor && excludeContructor)
                {
                    return false;
                }

                if(mi.IsSpecialName && excludeSpecials)
                {
                    return false;
                }

                return true;
            }

            public static MethodInfo GetMethod<T>(string name, params Type[] parameterTypes)
            {
                return GetMethod(typeof(T), name, parameterTypes);
            }

            public static MethodInfo GetMethod(Type type, string name, params Type[] parameterTypes)
            {
#if COREFX
                var methods = type.GetTypeInfo().GetDeclaredMethods(name);
                Func<ParameterInfo, Type> paramType = pi => pi.ParameterType;
                var method = methods.FirstOrDefault(mi => mi.GetParameters().SequenceEqual(paramType, parameterTypes));
                return method;
#endif
#if NET
                var method = parameterTypes.Any() 
                    ? type.GetMethod(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic, null, parameterTypes, null) 
                    : type.GetMethod(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
                return method;
#endif
                throw new NotImplementedException();
            }

            public static TResult Call<TImplement, TResult>(TImplement instance, string name, params object[] args)
            {
                return (TResult)Call(instance, name, args);
            }

            public static object Call(object instance, string name, params object[] args)
            {
                var type = instance.GetType();
                var types = args.Select(a => a.GetType()).ToArray();
                var method = GetMethod(type, name, types);
                var result = method.Invoke(instance, args);
                return result;
            }

            public static void VoidCall<TImplement>(TImplement instance, string name, params object[] args)
            {
                Call(instance, name, args);
            }

            public static void VoidCall(object instance, string name, params object[] args)
            {
                var type = instance.GetType();
                var types = args.Select(a => a.GetType()).ToArray();
                var method = GetMethod(type, name, types);
            }
        }
    }
}
