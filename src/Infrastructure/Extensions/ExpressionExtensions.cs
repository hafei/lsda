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
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var elementType = TypeSystem.GetElementType(source.Type);

            // todo: add cache?
            MethodInfo anyMethod = TypeSystem.FindExtensionMethod("Any", source.Type, new[] { typeof(Func<,>).MakeGenericType(elementType, typeof(bool)) }, null);

            return Expression.Call(
                anyMethod, 
                source, 
                TypeSystem.IsQueryableExtension(anyMethod) ? (Expression)Expression.Quote(predicate) : predicate);
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
        /// Be careful with this method when dealing with IQueryable, because only IEnumerable has extension method with this name.
        /// </remarks>
        public static MethodCallExpression AsEnumerable(this Expression source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            // todo: add cache?
            MethodInfo asEnumerableMethod = TypeSystem.FindExtensionMethod("AsEnumerable", source.Type, null, null);

            return Expression.Call(asEnumerableMethod, source);
        }

        /// <summary>
        /// Non-typed Contains method call.
        /// </summary>
        /// <param name="source">
        /// The source expression.
        /// </param>
        /// <param name="itemExpression">
        /// The item expression.
        /// </param>
        /// <returns>
        /// Contains method call.
        /// </returns>
        public static MethodCallExpression Contains(this Expression source, Expression itemExpression)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (itemExpression == null)
            {
                throw new ArgumentNullException("itemExpression");
            }

            // todo: add cache?
            ////var containsMethod = sequence.Type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
            ////    .Where(m => m.Name == "Contains")
            ////    .Where(m => m.ReturnType == typeof(bool))
            ////    .Where(m => m.GetParameters().Length == 1)
            ////    .SingleOrDefault();
            return Expression.Call(source, "Contains", Type.EmptyTypes, itemExpression);
        }

        /// <summary>
        /// Creates a System.Linq.Expressions.BinaryExpression that represents an equality comparison.
        ///   Overload that gets value to compare to as expression.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="valueExpression">
        /// The value expression.
        /// </param>
        /// <returns>
        /// New BinaryExpression.
        /// </returns>
        public static BinaryExpression Equal(this Expression expression, Expression valueExpression)
        {
            return Expression.Equal(expression, valueExpression);
        }

        /// <summary>
        /// Creates a System.Linq.Expressions.BinaryExpression that represents an equality comparison.
        ///   Overload that gets value to compare to as object.
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
            return expression.Equal(Expression.Constant(value));
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
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            if (collectionType == null)
            {
                throw new ArgumentNullException("collectionType");
            }

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
        /// Creates a System.Linq.Expressions.BinaryExpression that represents a "greater than" numeric comparison.
        ///   Overload that gets value to compare to as object.
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
            return expression.GreaterThan(Expression.Constant(value));
        }

        /// <summary>
        /// Creates a System.Linq.Expressions.BinaryExpression that represents a "greater than" numeric comparison.
        ///   Overload that gets value to compare to as expression.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="valueExpression">
        /// The value expression.
        /// </param>
        /// <returns>
        /// New BinaryExpression.
        /// </returns>
        public static BinaryExpression GreaterThan(this Expression expression, Expression valueExpression)
        {
            return Expression.GreaterThan(expression, valueExpression);
        }

        /// <summary>
        /// Creates a System.Linq.Expressions.BinaryExpression that represents a "greater than or equal" numeric comparison.
        ///   Overload that gets value to compare to as object.
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
            return expression.GreaterThanOrEqual(Expression.Constant(value));
        }

        /// <summary>
        /// Creates a System.Linq.Expressions.BinaryExpression that represents a "greater than or equal" numeric comparison.
        ///   Overload that gets value to compare to as expression.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="valueExpression">
        /// The value expression.
        /// </param>
        /// <returns>
        /// New BinaryExpression.
        /// </returns>
        public static BinaryExpression GreaterThanOrEqual(this Expression expression, Expression valueExpression)
        {
            return Expression.GreaterThanOrEqual(expression, valueExpression);
        }

        /// <summary>
        /// Non-typed GroupBy method call.
        /// </summary>
        /// <param name="source">
        /// The source sequence expression.
        /// </param>
        /// <param name="keySelector">
        /// The key selector.
        /// </param>
        /// <returns>
        /// New sequence with GroupBy method call.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "GroupBy() MethodCall requires lambda as predicate.")]
        public static MethodCallExpression GroupBy(this Expression source, LambdaExpression keySelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }

            var elementType = TypeSystem.GetElementType(source.Type);
            var keyType = keySelector.Body.Type;

            // todo: add cache?
            MethodInfo groupByMethod = TypeSystem.FindExtensionMethod("GroupBy", source.Type, new[] { typeof(Func<,>).MakeGenericType(elementType, keyType) }, new[] { keyType });

            return Expression.Call(
                groupByMethod, 
                source, 
                TypeSystem.IsQueryableExtension(groupByMethod) ? (Expression)Expression.Quote(keySelector) : keySelector);
        }

        /// <summary>
        /// Non-typed GroupBy method call.
        /// </summary>
        /// <param name="source">
        /// The source sequence expression.
        /// </param>
        /// <param name="keySelector">
        /// The key selector.
        /// </param>
        /// <param name="resultSelector">
        /// The result selector.
        /// </param>
        /// <returns>
        /// New sequence with GroupBy method call.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "GroupBy() MethodCall requires lambda as predicate.")]
        public static MethodCallExpression GroupBy(this Expression source, LambdaExpression keySelector, LambdaExpression resultSelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }

            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }

            var elementType = TypeSystem.GetElementType(source.Type);
            var keyType = keySelector.Body.Type;
            var resultType = resultSelector.Body.Type;

            // todo: add cache?
            MethodInfo groupByMethod = TypeSystem.FindExtensionMethod("GroupBy", source.Type, new[] { typeof(Func<,>).MakeGenericType(elementType, keyType), typeof(Func<,,>).MakeGenericType(keyType, typeof(IEnumerable<>).MakeGenericType(elementType), resultType) }, new[] { keyType, resultType });

            return Expression.Call(
                groupByMethod, 
                source, 
                TypeSystem.IsQueryableExtension(groupByMethod) ? (Expression)Expression.Quote(keySelector) : keySelector, 
                TypeSystem.IsQueryableExtension(groupByMethod) ? (Expression)Expression.Quote(resultSelector) : resultSelector);
        }

        /// <summary>
        /// Creates a System.Linq.Expressions.BinaryExpression that represents a "less than" numeric comparison.
        ///   Overload that gets value to compare to as object.
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
            return expression.LessThan(Expression.Constant(value));
        }

        /// <summary>
        /// Creates a System.Linq.Expressions.BinaryExpression that represents a "less than" numeric comparison.
        ///   Overload that gets value to compare to as object.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="valueExpression">
        /// The value expression.
        /// </param>
        /// <returns>
        /// New BinaryExpression.
        /// </returns>
        public static BinaryExpression LessThan(this Expression expression, Expression valueExpression)
        {
            return Expression.LessThan(expression, valueExpression);
        }

        /// <summary>
        /// Creates a System.Linq.Expressions.BinaryExpression that represents a " less than or equal" numeric comparison.
        ///   Overload that gets value to compare to as object.
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
            return expression.LessThanOrEqual(Expression.Constant(value));
        }

        /// <summary>
        /// Creates a System.Linq.Expressions.BinaryExpression that represents a " less than or equal" numeric comparison.
        ///   Overload that gets value to compare to as expression.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="valueExpression">
        /// The value expression.
        /// </param>
        /// <returns>
        /// New BinaryExpression.
        /// </returns>
        public static BinaryExpression LessThanOrEqual(this Expression expression, Expression valueExpression)
        {
            return Expression.LessThanOrEqual(expression, valueExpression);
        }

        /// <summary>
        /// Non-typed Max method call.
        /// </summary>
        /// <param name="source">
        /// The source sequence expression.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// Max method call on top of source sequence.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Max() MethodCall requires lambda as predicate.")]
        public static MethodCallExpression Max(this Expression source, LambdaExpression predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            var elementType = TypeSystem.GetElementType(source.Type);
            var keyType = predicate.Body.Type;

            // todo: add cache?
            // trying to fing special overload of Max method (with one generic parameter)
            MethodInfo maxMethod = TypeSystem.FindExtensionMethod("Max", source.Type, new[] { typeof(Func<,>).MakeGenericType(elementType, keyType) }, null);

            // if special overload is not found, then trying to find generic one
            if (maxMethod == null)
            {
                maxMethod = TypeSystem.FindExtensionMethod("Max", source.Type, new[] { typeof(Func<,>).MakeGenericType(elementType, keyType) }, new[] { keyType });
            }

            return Expression.Call(
                maxMethod, 
                source, 
                TypeSystem.IsQueryableExtension(maxMethod) ? (Expression)Expression.Quote(predicate) : predicate);
        }

        /// <summary>
        /// Non-typed Min method call.
        /// </summary>
        /// <param name="source">
        /// The source sequence expression.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// Min method call on top of source sequence.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Min() MethodCall requires lambda as predicate.")]
        public static MethodCallExpression Min(this Expression source, LambdaExpression predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            var elementType = TypeSystem.GetElementType(source.Type);
            var keyType = predicate.Body.Type;

            // todo: add cache?
            // trying to fing special overload of Min method (with one generic parameter)
            MethodInfo minMethod = TypeSystem.FindExtensionMethod("Min", source.Type, new[] { typeof(Func<,>).MakeGenericType(elementType, keyType) }, null);

            // if special overload is not found, then trying to find generic one
            if (minMethod == null)
            {
                minMethod = TypeSystem.FindExtensionMethod("Min", source.Type, new[] { typeof(Func<,>).MakeGenericType(elementType, keyType) }, new[] { keyType });
            }

            return Expression.Call(
                minMethod, 
                source, 
                TypeSystem.IsQueryableExtension(minMethod) ? (Expression)Expression.Quote(predicate) : predicate);
        }

        /// <summary>
        /// Creates a System.Linq.Expressions.UnaryExpression that represents a bitwise complement operation.
        /// </summary>
        /// <param name="expression">
        /// The source expression.
        /// </param>
        /// <returns>
        /// New UnaryExpression.
        /// </returns>
        public static UnaryExpression Not(this Expression expression)
        {
            return Expression.Not(expression);
        }

        /// <summary>
        /// Creates a System.Linq.Expressions.BinaryExpression that represents an inequality comparison.
        ///   Overload that gets value to compare to as expression.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="valueExpression">
        /// The value expression.
        /// </param>
        /// <returns>
        /// New BinaryExpression.
        /// </returns>
        public static BinaryExpression NotEqual(this Expression expression, Expression valueExpression)
        {
            return Expression.NotEqual(expression, valueExpression);
        }

        /// <summary>
        /// Creates a System.Linq.Expressions.BinaryExpression that represents an inequality comparison.
        ///   Overload that gets value to compare to as object.
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
        public static BinaryExpression NotEqual(this Expression expression, object value)
        {
            return expression.NotEqual(Expression.Constant(value));
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
        /// Non-typed OrderBy method call.
        /// </summary>
        /// <param name="source">
        /// The source sequence expression.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// New sequence with OrderBy method call.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "OrderBy() MethodCall requires lambda as predicate.")]
        public static MethodCallExpression OrderBy(this Expression source, LambdaExpression predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            var elementType = TypeSystem.GetElementType(source.Type);
            var keyType = predicate.Body.Type;

            // todo: add cache?
            MethodInfo orderByMethod = TypeSystem.FindExtensionMethod("OrderBy", source.Type, new[] { typeof(Func<,>).MakeGenericType(elementType, keyType) }, new[] { keyType });

            return Expression.Call(
                orderByMethod, 
                source, 
                TypeSystem.IsQueryableExtension(orderByMethod) ? (Expression)Expression.Quote(predicate) : predicate);
        }

        /// <summary>
        /// Non-typed OrderByDescending method call.
        /// </summary>
        /// <param name="source">
        /// The source sequence expression.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// New sequence with OrderByDescending method call.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "OrderByDescending() MethodCall requires lambda as predicate.")]
        public static MethodCallExpression OrderByDescending(this Expression source, LambdaExpression predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            var elementType = TypeSystem.GetElementType(source.Type);
            var keyType = predicate.Body.Type;

            // todo: add cache?
            MethodInfo orderByDescendingMethod = TypeSystem.FindExtensionMethod("OrderByDescending", source.Type, new[] { typeof(Func<,>).MakeGenericType(elementType, keyType) }, new[] { keyType });

            return Expression.Call(
                orderByDescendingMethod, 
                source, 
                TypeSystem.IsQueryableExtension(orderByDescendingMethod) ? (Expression)Expression.Quote(predicate) : predicate);
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
                expr = expr.Property(property);
            }

            return (MemberExpression)expr;
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
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }

            var elementType = TypeSystem.GetElementType(source.Type);
            var resultType = selector.Body.Type;

            // todo: add cache?
            MethodInfo selectMethod = TypeSystem.FindExtensionMethod("Select", source.Type, new[] { typeof(Func<,>).MakeGenericType(elementType, resultType) }, new[] { resultType });

            return Expression.Call(
                selectMethod, 
                source, 
                TypeSystem.IsQueryableExtension(selectMethod) ? (Expression)Expression.Quote(selector) : selector);
        }

        /// <summary>
        /// Non-typed Single method call.
        /// </summary>
        /// <param name="source">
        /// The source sequence expression.
        /// </param>
        /// <returns>
        /// New sequence with Single method call.
        /// </returns>
        public static MethodCallExpression Single(this Expression source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            // todo: add cache?
            MethodInfo singleMethod = TypeSystem.FindExtensionMethod("Single", source.Type, null, null);

            return Expression.Call(singleMethod, source);
        }

        /// <summary>
        /// Non-typed SingleOrDefault method call.
        /// </summary>
        /// <param name="source">
        /// The source sequence expression.
        /// </param>
        /// <returns>
        /// New sequence with SingleOrDefault method call.
        /// </returns>
        public static MethodCallExpression SingleOrDefault(this Expression source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            // todo: add cache?
            MethodInfo singleOrDefaultMethod = TypeSystem.FindExtensionMethod("SingleOrDefault", source.Type, null, null);

            return Expression.Call(singleOrDefaultMethod, source);
        }

        /// <summary>
        /// Non-typed Skip method call.
        /// </summary>
        /// <param name="source">
        /// The source sequence expression.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// New sequence with Skip method call.
        /// </returns>
        public static MethodCallExpression Skip(this Expression source, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            // todo: add cache?
            MethodInfo skipMethod = TypeSystem.FindExtensionMethod("Skip", source.Type, new[] { typeof(int) }, null);

            return Expression.Call(skipMethod, source, Expression.Constant(count));
        }

        /// <summary>
        /// Non-typed StartsWith method call.
        /// </summary>
        /// <param name="source">
        /// The source expression.
        /// </param>
        /// <param name="valueExpression">
        /// The value expression.
        /// </param>
        /// <returns>
        /// StartsWith method call.
        /// </returns>
        public static MethodCallExpression StartsWith(this Expression source, Expression valueExpression)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (valueExpression == null)
            {
                throw new ArgumentNullException("valueExpression");
            }

            // todo: add cache?
            ////var startsWithMethod = sequence.Type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
            ////    .Where(m => m.Name == "StartsWith")
            ////    .Where(m => m.ReturnType == typeof(bool))
            ////    .Where(m => m.GetParameters().Length == 1)
            ////    .SingleOrDefault();
            return Expression.Call(source, "StartsWith", Type.EmptyTypes, valueExpression);
        }

        /// <summary>
        /// Non-typed Sum method call.
        /// </summary>
        /// <param name="source">
        /// The source sequence expression.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// Sum method call on top of source sequence.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Sum() MethodCall requires lambda as predicate.")]
        public static MethodCallExpression Sum(this Expression source, LambdaExpression predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            var elementType = TypeSystem.GetElementType(source.Type);
            var keyType = predicate.Body.Type;

            // todo: add cache?
            MethodInfo sumMethod = TypeSystem.FindExtensionMethod("Sum", source.Type, new[] { typeof(Func<,>).MakeGenericType(elementType, keyType) }, null);

            return Expression.Call(
                sumMethod, 
                source, 
                TypeSystem.IsQueryableExtension(sumMethod) ? (Expression)Expression.Quote(predicate) : predicate);
        }

        /// <summary>
        /// Non-typed Take method call.
        /// </summary>
        /// <param name="source">
        /// The source sequence expression.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// New sequence with Take method call.
        /// </returns>
        public static MethodCallExpression Take(this Expression source, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            // todo: add cache?
            MethodInfo takeMethod = TypeSystem.FindExtensionMethod("Take", source.Type, new[] { typeof(int) }, null);

            return Expression.Call(takeMethod, source, Expression.Constant(count));
        }

        /// <summary>
        /// Non-typed ThenBy method call.
        /// </summary>
        /// <param name="source">
        /// The source sequence expression.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// New sequence with ThenBy method call.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "ThenBy() MethodCall requires lambda as predicate.")]
        public static MethodCallExpression ThenBy(this Expression source, LambdaExpression predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            var elementType = TypeSystem.GetElementType(source.Type);
            var keyType = predicate.Body.Type;

            // todo: add cache?
            MethodInfo thenByMethod = TypeSystem.FindExtensionMethod("ThenBy", source.Type, new[] { typeof(Func<,>).MakeGenericType(elementType, keyType) }, new[] { keyType });

            return Expression.Call(
                thenByMethod, 
                source, 
                TypeSystem.IsQueryableExtension(thenByMethod) ? (Expression)Expression.Quote(predicate) : predicate);
        }

        /// <summary>
        /// Non-typed ThenByDescending method call.
        /// </summary>
        /// <param name="source">
        /// The source sequence expression.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// New sequence with ThenByDescending method call.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "ThenByDescending() MethodCall requires lambda as predicate.")]
        public static MethodCallExpression ThenByDescending(this Expression source, LambdaExpression predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            var elementType = TypeSystem.GetElementType(source.Type);
            var keyType = predicate.Body.Type;

            // todo: add cache?
            MethodInfo thenByDescendingMethod = TypeSystem.FindExtensionMethod("ThenByDescending", source.Type, new[] { typeof(Func<,>).MakeGenericType(elementType, keyType) }, new[] { keyType });

            return Expression.Call(
                thenByDescendingMethod, 
                source, 
                TypeSystem.IsQueryableExtension(thenByDescendingMethod) ? (Expression)Expression.Quote(predicate) : predicate);
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
        /// Be careful with this method when dealing with IQueryable, because only IEnumerable has extension method with this name.
        /// </remarks>
        public static MethodCallExpression ToArray(this Expression source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

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
        /// New sequence with ToList method call.
        /// </returns>
        /// <remarks>
        /// Be careful with this method when dealing with IQueryable, because only IEnumerable has extension method with this name.
        /// </remarks>
        public static MethodCallExpression ToList(this Expression source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

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
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var elementType = TypeSystem.GetElementType(source.Type);

            // todo: add cache?
            MethodInfo whereMethod = TypeSystem.FindExtensionMethod("Where", source.Type, new[] { typeof(Func<,>).MakeGenericType(elementType, typeof(bool)) }, null);

            return Expression.Call(
                whereMethod, 
                source, 
                TypeSystem.IsQueryableExtension(whereMethod) ? (Expression)Expression.Quote(predicate) : predicate);
        }

        #endregion
    }
}