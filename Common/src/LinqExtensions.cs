﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Adriva.Common.Core
{
    /// <summary>
    /// Provides extension methods to help with IEnumerable types.
    /// </summary>
    public static class LinqExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<int, T> action)
        {
            if (null == items) throw new ArgumentNullException(nameof(items));

            int loop = 0;
            foreach (T item in items)
            {
                action?.Invoke(loop++, item);
            }
        }

        public static async Task ForEachAsync<T>(this IEnumerable<T> items, Func<int, T, Task> action)
        {
            if (null == items) throw new ArgumentNullException(nameof(items));

            int loop = 0;
            foreach (T item in items)
            {
                await action?.Invoke(loop++, item);
            }
        }

        public static void CopyTo<T>(this IEnumerable<T> items, T[] array)
        {
            items.ForEach((loop, item) =>
            {
                array[loop] = item;
            });
        }
    }
}