using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeitechWebApi.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        /// Devuelve verdadero si la variable solicitada es Nulo
        /// </summary>
        /// <param name="value"></param>
        /// <returns>bool</returns>
        public static bool IsNull(this Object value)
        {
            return value == null;
        }

        /// <summary>
        /// Devuelve verdadero si la variable solicitada No es Nulo
        /// </summary>
        /// <param name="value"></param>
        /// <returns>bool</returns>
        public static bool IsNotNull(this Object value)
        {
            return value != null;
        }

        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

    }
}