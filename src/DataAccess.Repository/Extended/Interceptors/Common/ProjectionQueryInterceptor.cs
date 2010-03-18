// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectionQueryInterceptor.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   Typed Projections interceptor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Interceptors.Common
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Attributes;

    using Events;

    using Infrastructure.Extensions;
    using Infrastructure.Helpers;
    using Infrastructure.Linq;

    /// <summary>
    /// Typed Projections interceptor.
    /// </summary>
    public class ProjectionQueryInterceptor : QueryInterceptor<IScope>
    {
        #region Constants and Fields

        /// <summary>
        /// The enumerable Select method.
        /// </summary>
        private static readonly MethodInfo EnumerableSelectProjectionMethod = typeof(EnumerableExtensions).GetMethod("Select", BindingFlags.Static | BindingFlags.Public);

        /// <summary>
        /// The queryable Select method.
        /// </summary>
        private static readonly MethodInfo QueryableSelectProjectionMethod = typeof(QueryableExtensions).GetMethod("Select", BindingFlags.Static | BindingFlags.Public);

        #endregion

        #region Public Methods

        /// <summary>
        /// Handles the MethodCallVisit event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.MethodCallVisitEventArgs"/> instance containing the event data.
        /// </param>
        public override void OnMethodCallVisit(MethodCallVisitEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            if (e.MethodCall.Method.IsGenericMethod
                && (e.MethodCall.Method.GetGenericMethodDefinition() == QueryableSelectProjectionMethod
                    || e.MethodCall.Method.GetGenericMethodDefinition() == EnumerableSelectProjectionMethod))
            {
                var sourceElementType = TypeSystem.GetElementType(e.MethodCall.Arguments.Single().Type);
                var resultElementType = TypeSystem.GetElementType(e.MethodCall.Type);

                var sourceElementParameter = Expression.Parameter(sourceElementType, "source");

                var resultMemberBindings = new List<MemberBinding>();

                foreach (var resultProperty in resultElementType.GetProperties())
                {
                    var propertyAttribute = (PropertyAttribute) resultProperty.GetCustomAttributes(typeof(PropertyAttribute), false).SingleOrDefault();
                    if (propertyAttribute != null)
                    {
                        var sourcePropertyPath = propertyAttribute.Path ?? resultProperty.Name;
                        var resultPropertyBindingExpression = (Expression) sourceElementParameter.PropertyPath(sourcePropertyPath);

                        var sourcePropertyElementType = TypeSystem.GetElementType(resultPropertyBindingExpression.Type);
                        var resultPropertyElementType = TypeSystem.GetElementType(resultProperty.PropertyType);

                        // note: if both properties are sequence types and element types are differ, then create sub-projection call
                        if (((sourcePropertyElementType != null) && (resultPropertyElementType != null))
                            && (sourcePropertyElementType != resultPropertyElementType))
                        {
                            var genericSelectPropjectionMethod = EnumerableSelectProjectionMethod.MakeGenericMethod(resultPropertyElementType);

                            resultPropertyBindingExpression = Expression.Call(genericSelectPropjectionMethod, resultPropertyBindingExpression)
                                .FixupCollectionType(resultProperty.PropertyType);
                        }

                        resultMemberBindings.Add(Expression.Bind(resultProperty, resultPropertyBindingExpression));

                        // Expression and Property attributes cannot be used together
                        // todo: maybe throw below?
                        continue;
                    }

                    var expressionAttribute = (ExpressionAttribute) resultProperty.GetCustomAttributes(typeof(ExpressionAttribute), false).SingleOrDefault();
                    if (expressionAttribute != null)
                    {
                        var declaringType = expressionAttribute.DeclaringType ?? resultElementType;
                        var methodName = expressionAttribute.MethodName ?? resultProperty.Name;

                        var expressionMethodInfo = declaringType.GetMethod(
                            methodName, 
                            BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

                        if (expressionMethodInfo == null)
                        {
                            throw new ArgumentException(String.Format(
                                CultureInfo.InvariantCulture, 
                                "Method specified in ExpressionMethod attribute of '{0}' property in '{1}' class is not found.", 
                                resultProperty.Name, 
                                resultElementType.Name));
                        }

                        var customBindingExpression = (LambdaExpression) expressionMethodInfo.Invoke(null, new object[] { this.Scope });

                        // localize expression (replace its parameter with local entityParameter)
                        var localizedCustomBindingExpression = new ExpressionParameterReplacer(
                            customBindingExpression.Parameters.Single(), 
                            sourceElementParameter)
                            .Visit(customBindingExpression.Body);

                        resultMemberBindings.Add(Expression.Bind(resultProperty, localizedCustomBindingExpression));
                    }
                }

                var resultInitExpression = Expression.MemberInit(Expression.New(resultElementType), resultMemberBindings);

                // todo: add cache for selectors
                var resultSelectorLambda = sourceElementParameter.ToLambda(resultInitExpression);

                var originalQuery = e.MethodCall.Arguments.First();

                e.SubstituteExpression = originalQuery.Select(resultSelectorLambda);
            }
        }

        #endregion
    }
}