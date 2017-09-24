using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilosophicalMonkey
{
    internal static class HelperExtensions
    {
        public static bool SequenceEqual<T, U>(this IEnumerable<T> first, Func<T,U> mapFirst, IEnumerable<U> second)
        {
            var firstMapped = first.Select(mapFirst);
            return firstMapped.SequenceEqual(second); 
        }

        public static bool SequenceEqual<T, U>(this IEnumerable<T> first, IEnumerable<U> second, Func<U, T> mapSecond)
        {
            var secondMapped = second.Select(mapSecond);
            return first.SequenceEqual(secondMapped);
        }
    }
}
