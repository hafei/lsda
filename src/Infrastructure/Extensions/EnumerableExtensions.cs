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
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The enumerable extensions.
    /// </summary>
    public static class EnumerableExtensions
    {
        #region Public Methods

        /// <summary>
        /// Applies an accumulator function over a sequence getting seed element from first element of the sequence.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the element of the source.
        /// </typeparam>
        /// <typeparam name="TAccumulate">
        /// The type of the accumulate element.
        /// </typeparam>
        /// <param name="source">
        /// The source sequence.
        /// </param>
        /// <param name="seed">
        /// An seed function to be invoked on first element. 
        /// </param>
        /// <param name="func">
        /// An accumulator function to be invoked on each element.
        /// </param>
        /// <returns>
        /// The final accumulator value.
        /// </returns>
        public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, Func<TSource, TAccumulate> seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (seed == null)
            {
                throw new ArgumentNullException("seed");
            }

            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    throw new InvalidOperationException("Source sequence contains no elements.");
                }

                TAccumulate current = seed(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    current = func(current, enumerator.Current);
                }

                return current;
            }
        }

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
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

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