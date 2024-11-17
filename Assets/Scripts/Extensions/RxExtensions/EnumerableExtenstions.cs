using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<(T, int index)> WithIndex<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Select((v, i) => (v, i));
        }

        public static int GetNext(this ICollection<int> collection)
        {
            return collection.Count == 0 ? 0 : collection.Max() + 1;
        }

        public static bool HasMoreThenNElements<T>(this IEnumerable<T> enumerable, int targetNumber)
        {
            var count = 0;
            foreach (var element in enumerable)
            {
                count++;
                if (count > targetNumber)
                {
                    return true;
                }
            }

            return false;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
                action(element);
        }

        public static bool IsEqualSequance<T>(this List<T> target, List<T> other) where T : IEquatable<T>
        {
            if (target.Count != other.Count)
            {
                return false;
            }

            for (var i = 0; i < target.Count; i++)
            {
                if (!target[i].Equals(other[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static int GetEqualItemsCount<T>(this List<T> target, List<T> other) where T : IEquatable<T>
        {
            if (target.Count != other.Count)
            {
                return 0;
            }

            var count = 0;

            for (var i = 0; i < target.Count; i++)
            {
                if (target[i].Equals(other[i]))
                {
                    count++;
                }
            }

            return count;
        }
    }
}
