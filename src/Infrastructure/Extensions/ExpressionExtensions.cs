// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionExtensions.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   Expression extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.Infrastructure.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using System.Reflection;

    using Helpers;

    /// <summary>
    /// Expression extensions.
    /// </summary>
    public static class ExpressionExtensions
    {
        #region Public Methods

        /// <summary>
        /// Ands the specified left.
        /// </summary>
        /// <param name="left">
        /// The left expression.
        /// </param>
        /// <param name="right">
        /// The right expression.
        /// </param>
        /// <returns>
        /// New BinaryExpression.
        /// </returns>
        public static BinaryExpression And(this Expression left, Expression right)
        {
            return Expression.AndAlso(left, right);
        }

        /// <summary>
        /// Non-typed Any method call.
        /// </summary>
        /// <param name="source">
        /// The source sequence expression.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// New sequence with Any method call.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Any() MethodCall requires lambda as predicate.")]
        public static MethodCallExpression Any(this Expression source, LambdaExpression predicate)
        {
            var elementType = TypeSystem.GetElementType(source.Type);

            // todo: add cache?
            MethodInfo anyMethod = TypeSystem.FindExtensionMethod("Any", source.Type, new[] { typeof(Func<,>).MakeGenericType(elementType, typeof(bool)) }, null);

            return Expression.Call(
                anyMethod,
                source,
                TypeSystem.IsQueryableExtension(anyMethod) ? (Expression) Expression.Quote(predicate) : predicate);
        }

        /// <summary>
        /// Non-typed AsEnumerable method call.
        /// </summary>
        /// <param name="source">
        /// The source sequence expression.
        /// </param>
        /// <returns>
        /// New sequence with AsEnumerable method call.
        /// </returns>
        /// <remarks>
        /// It should be used only with IEnumerable sequences.
        /// AsEnumerable MethodCall for IQueryable makes no sense and should throw exception (FindExtensionMethod will return null). 
        /// </remarks>
        public static MethodCallExpression AsEnumerable(this Expression source)
        {
            // todo: add cache?
            MethodInfo asEnumerableMethod = TypeSystem.FindExtensionMethod("AsEnumerable", source.Type, null, null);

            return Expression.Call(asEnumerableMethod, source);
        }

        /// <summary>
        /// Equals the specified member.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// New BinaryExpression.
        /// </returns>
        public static BinaryExpression Equal(this Expression expression, object value)
        {
            return Expression.Equal(expression, Expression.Constant(value));
        }

        /// <summary>
        /// Method needed to generate type-correct expressions.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="collectionType">
        /// The collection type.
        /// </param>
        /// <returns>
        /// New expression of provided type.
        /// </returns>
        public static Expression FixupCollectionType(this Expression expression, Type collectionType)
        {
            if (!collectionType.IsAssignableFrom(expression.Type))
            {
                // todo: refactor to IsAssignableFrom, but will work if List<T> descendants are not used
                // todo: maybe change to IList<> interface check
                if (collectionType.IsGenericType && collectionType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    return expression.ToList();
                }

                if (collectionType.IsArray)
                {
                    return expression.ToArray();
                }

                // AsEnumerable is not necessary
            }

            return expression;
        }

        /// <summary>
        /// Greaters the than.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// New BinaryExpression.
        /// </returns>
        public static BinaryExpression GreaterThan(this Expression expression, object value)
        {
            return Expression.GreaterThan(expression, Expression.Constant(value));
        }

        /// <summary>
        /// Greaters the than or equal.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// New BinaryExpression.
        /// </returns>
        public static BinaryExpression GreaterThanOrEqual(this Expression expression, object value)
        {
            return Expression.GreaterThanOrEqual(expression, Expression.Constant(value));
        }

        /// <summary>
        /// Lesses the than.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// New BinaryExpression.
        /// </returns>
        public static BinaryExpression LessThan(this Expression expression, object value)
        {
            return Expression.LessThan(expression, Expression.Constant(value));
        }

        /// <summary>
        /// Lesses the than or equal.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// New BinaryExpression.
        /// </returns>
        public static BinaryExpression LessThanOrEqual(this Expression expression, object value)
        {
            return Expression.LessThanOrEqual(expression, Expression.Constant(value));
        }

        /// <summary>
        /// Ors the specified left.
        /// </summary>
        /// <param name="left">
        /// The left expression.
        /// </param>
        /// <param name="right">
        /// The right expression.
        /// </param>
        /// <returns>
        /// New BinaryExpression.
        /// </returns>
        public static BinaryExpression Or(this Expression left, Expression right)
        {
            return Expression.OrElse(left, right);
        }

        /// <summary>
        /// Properties the specified parameter.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="propertyName">
        /// Name of the property.
        /// </param>
        /// <returns>
        /// New MemberExpression.
        /// </returns>
        public static MemberExpression Property(this Expression expression, string propertyName)
        {
            return Expression.Property(expression, propertyName);
        }

        /// <summary>
        /// Properties the specified parameter.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="propertyInfo">
        /// PropertyInfo of the property.
        /// </param>
        /// <returns>
        /// New MemberExpression.
        /// </returns>
        public static MemberExpression Property(this Expression expression, PropertyInfo propertyInfo)
        {
            return Expression.Property(expression, propertyInfo);
        }

        /// <summary>
        /// Gets MemberExpression by path.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="memberPath">
        /// The member property path.
        /// </param>
        /// <returns>
        /// New MemberExpression.
        /// </returns>
        public static MemberExpression PropertyPath(this Expression expression, string memberPath)
        {
            if (String.IsNullOrEmpty(memberPath))
            {
                throw new ArgumentException("PropertyPath cannot be empty.", "memberPath");
            }

            string[] properties = memberPath.Split('.');

            Expression expr = expression;
            foreach (string property in properties)
            {
                expr = Expression.Property(expr, property);
            }

            return (MemberExpression) expr;
        }

        /// <summary>
        /// Non-typed Select method call.
        /// </summary>
        /// <param name="source">
        /// The source sequence expression.
        /// </param>
        /// <param name="selector">
        /// The selector.
        /// </param>
        /// <returns>
        /// New sequence with Select method call.
        /// </returns>
        public static MethodCallExpression Select(this Expression source, LambdaExpression selector)
        {
            var elementType = TypeSystem.GetElementType(source.Type);
            var resultType = TypeSystem.GetElementType(selector.Body.Type);

            // todo: add cache?
            MethodInfo selectMethod = TypeSystem.FindExtensionMethod("Select", source.Type, new[] { typeof(Func<,>).MakeGenericType(elementType, resultType) }, new[] { resultType });

            return Expression.Call(
                selectMethod,
                source,
                TypeSystem.IsQueryableExtension(selectMethod) ? (Expression) Expression.Quote(selector) : selector);
        }

        /// <summary>
        /// Non-typed ToArray method call.
        /// </summary>
        /// <param name="source">
        /// The source sequence expression.
        /// </param>
        /// <returns>
        /// New sequence with ToArray method call.
        /// </returns>
        /// <remarks>
        /// It should be used only with IEnumerable sequences.
        /// ToArray MethodCall for IQueryable makes no sense and should throw exception (FindExtensionMethod will return null). 
        /// </remarks>
        public static MethodCallExpression ToArray(this Expression source)
        {
            // todo: add cache?
            MethodInfo toArrayMethod = TypeSystem.FindExtensionMethod("ToArray", source.Type, null, null);

            return Expression.Call(toArrayMethod, source);
        }

        /// <summary>
        /// Gets the lambda expression.
        /// </summary>
        /// <typeparam name="TDelegate">
        /// The type of the delegate.
        /// </typeparam>
        /// <param name="parameter">
        /// The parameter expression.
        /// </param>
        /// <param name="body">
        /// The body expression.
        /// </param>
        /// <returns>
        /// New LambdaExpression.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By design.")]
        public static Expression<TDelegate> ToLambda<TDelegate>(this ParameterExpression parameter, Expression body)
        {
            return Expression.Lambda<TDelegate>(body, parameter);
        }

        /// <summary>
        /// Gets the lambda expression.
        /// </summary>
        /// <param name="parameter">
        /// The parameter expression.
        /// </param>
        /// <param name="body">
        /// The body expression.
        /// </param>
        /// <returns>
        /// New LambdaExpression.
        /// </returns>
        public static LambdaExpression ToLambda(this ParameterExpression parameter, Expression body)
        {
            return Expression.Lambda(body, parameter);
        }

        /// <summary>
        /// Non-typed ToList method call.
        /// </summary>
        /// <param name="source">
        /// The source sequence expression.
        /// </param>
        /// <returns>
        /// New sequence with ToList method call        /// 
        /// </returns>
        /// <remarks>
        /// It should be used only with IEnumerable sequences.
        /// ToList MethodCall for IQueryable makes no sense and should throw exception (FindExtensionMethod will return null). 
        /// </remarks>
        public static MethodCallExpression ToList(this Expression source)
        {
            // todo: add cache?
            MethodInfo toListMethod = TypeSystem.FindExtensionMethod("ToList", source.Type, null, null);

            return Expression.Call(toListMethod, source);
        }

        /// <summary>
        /// Non-typed Where method call.
        /// </summary>
        /// <param name="source">
        /// The source sequence expression.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// New sequence with Where method call.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Where() MethodCall requires lambda as predicate.")]
        public static MethodCallExpression Where(this Expression source, LambdaExpression predicate)
        {
            var elementType = TypeSystem.GetElementType(source.Type);

            // todo: add cache?
            MethodInfo whereMethod = TypeSystem.FindExtensionMethod("Where", source.Type, new[] { typeof(Func<,>).MakeGenericType(elementType, typeof(bool)) }, null);

            return Expression.Call(
                whereMethod,
                source,
                TypeSystem.IsQueryableExtension(whereMethod) ? (Expression) Expression.Quote(predicate) : predicate);
        }

        #endregion
    }
}