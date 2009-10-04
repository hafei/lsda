// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The enumerable extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.Infrastructure.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The enumerable extensions.
    /// </summary>
    public static class EnumerableExtensions
    {
        #region Public Methods

        /// <summary>
        /// Applies a function to every element of the list.
        /// </summary>
        /// <typeparam name="T">
        /// Element type.
        /// </typeparam>
        /// <param name="enumerable">
        /// The enumerable.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T item in enumerable)
            {
                action(item);
            }
        }

        /// <summary>
        /// Create a multicast action.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the parameter of the method that this delegate encapsulates.
        /// </typeparam>
        /// <param name="actions">
        /// The action.
        /// </param>
        /// <returns>
        /// Multicast action
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "by design, IEnumerable extension method")]
        public static Action<T> Multicast<T>(this IEnumerable<Action<T>> actions)
        {
            return param => actions.ForEach(a => a(param));
        }

        #endregion
    }
}