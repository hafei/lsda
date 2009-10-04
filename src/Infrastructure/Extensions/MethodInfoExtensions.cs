// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodInfoExtensions.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The method info extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.Infrastructure.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// The method info extensions.
    /// </summary>
    public static class MethodInfoExtensions
    {
        #region Public Methods

        /// <summary>
        /// Adds MethodCall to query.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the element.
        /// </typeparam>
        /// <typeparam name="TNew">
        /// Type of the element of new IQueryable.
        /// </typeparam>
        /// <param name="methodBase">
        /// The method base.
        /// </param>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// IQueryable of type T with MethodCall added.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "By design.")]
        public static IQueryable<TNew> AddToNewQuery<T, TNew>(
            this MethodBase methodBase, IQueryable<T> query, params Expression[] arguments)
        {
            if (!methodBase.IsStatic)
            {
                throw new InvalidOperationException("Only for static extension methods.");
            }

            if (!methodBase.IsGenericMethod)
            {
                throw new InvalidOperationException("Only for generic methods.");
            }

            // todo: other checks (generic arguments count and type, first parameter type, etc.)
            //// if (!methodBase.GetGenericArguments())
            //// {
            ////     throw new InvalidOperationException("only for generic methods");
            //// }

            return
                query.Provider.CreateQuery<TNew>(
                    Expression.Call(
                        ((MethodInfo) methodBase).MakeGenericMethod(typeof(T), typeof(TNew)),
                        (new[] { query.Expression }).Concat(arguments).ToArray()));
        }

        /// <summary>
        /// Adds MethodCall to query.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the element.
        /// </typeparam>
        /// <param name="methodBase">
        /// The method base.
        /// </param>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// IQueryable of type T with MethodCall added.
        /// </returns>
        public static IQueryable<T> AddToQuery<T>(
            this MethodBase methodBase, IQueryable<T> query, params Expression[] arguments)
        {
            if (!methodBase.IsStatic)
            {
                throw new InvalidOperationException("Only for static extension methods.");
            }

            if (!methodBase.IsGenericMethod)
            {
                throw new InvalidOperationException("Only for generic methods.");
            }

            // todo: other checks (generic arguments count and type, first parameter type, etc.)
            //// if (!methodBase.GetGenericArguments())
            //// {
            ////     throw new InvalidOperationException("only for generic methods");
            //// }

            return
                query.Provider.CreateQuery<T>(
                    Expression.Call(
                        ((MethodInfo) methodBase).MakeGenericMethod(typeof(T)),
                        (new[] { query.Expression }).Concat(arguments).ToArray()));
        }

        /// <summary>
        /// Adds MethodCall to query.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the element.
        /// </typeparam>
        /// <typeparam name="TParent">
        /// The type of parent element.
        /// </typeparam>
        /// <param name="methodBase">
        /// The method base.
        /// </param>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// IQueryable of type T with MethodCall added.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "By design.")]
        public static IQueryable<T> AddToQuery<T, TParent>(
            this MethodBase methodBase, IQueryable<T> query, params Expression[] arguments)
        {
            if (!methodBase.IsStatic)
            {
                throw new InvalidOperationException("Only for static extension methods.");
            }

            if (!methodBase.IsGenericMethod)
            {
                throw new InvalidOperationException("Only for generic methods.");
            }

            // todo: other checks (generic arguments count and type, first parameter type, etc.)
            //// if (!methodBase.GetGenericArguments())
            //// {
            ////     throw new InvalidOperationException("only for generic methods");
            //// }

            return
                query.Provider.CreateQuery<T>(
                    Expression.Call(
                        ((MethodInfo) methodBase).MakeGenericMethod(typeof(T), typeof(TParent)),
                        (new[] { query.Expression }).Concat(arguments).ToArray()));
        }

        /// <summary>
        /// Gets the custom attributes.
        /// </summary>
        /// <typeparam name="T">
        /// The type of custom attribute.
        /// </typeparam>
        /// <param name="methodInfo">
        /// The method info.
        /// </param>
        /// <returns>
        /// Array of custom attributes instances for method.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By design.")]
        public static T[] GetCustomAttributes<T>(this MethodInfo methodInfo) where T : Attribute
        {
            return methodInfo.GetCustomAttributes<T>(false);
        }

        /// <summary>
        /// Gets the custom attributes.
        /// </summary>
        /// <typeparam name="T">
        /// Custom attribute type.
        /// </typeparam>
        /// <param name="methodInfo">
        /// The method info.
        /// </param>
        /// <param name="inherit">
        /// if set to <c>true</c> [inherit].
        /// </param>
        /// <returns>
        /// Array of custom attributes instances for method.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By design.")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "By design.")]
        public static T[] GetCustomAttributes<T>(this MethodInfo methodInfo, bool inherit) where T : Attribute
        {
            return methodInfo.GetCustomAttributes(typeof(T), inherit).Cast<T>().ToArray();
        }

        #endregion
    }
}