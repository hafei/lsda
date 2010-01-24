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
        /// The Select method.
        /// </summary>
        private static readonly MethodInfo SelectProjectionMethod = typeof(QueryableExtensions).GetMethod("Select", BindingFlags.Static | BindingFlags.Public);

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

            if (e.MethodCall.Method.IsGenericMethod &&
                e.MethodCall.Method.GetGenericMethodDefinition() == SelectProjectionMethod)
            {
                var elementType = TypeSystem.GetElementType(e.MethodCall.Arguments.First().Type);
                var resultElementType = TypeSystem.GetElementType(e.MethodCall.Type);

                var entityParameter = Expression.Parameter(elementType, "p");

                var bindings = new List<MemberBinding>();

                foreach (PropertyInfo property in resultElementType.GetProperties())
                {
                    var propertyAttribute = (PropertyAttribute) property.GetCustomAttributes(typeof(PropertyAttribute), false).SingleOrDefault();

                    if (propertyAttribute != null)
                    {
                        var propertyPath = propertyAttribute.Path ?? property.Name;

                        bindings.Add(Expression.Bind(property, entityParameter.PropertyPath(propertyPath)));
                    }

                    var expressionAttribute = (ExpressionAttribute) property.GetCustomAttributes(typeof(ExpressionAttribute), false).SingleOrDefault();

                    if (expressionAttribute != null)
                    {
                        var declaringType = expressionAttribute.DeclaringType ?? resultElementType;
                        var methodName = expressionAttribute.MethodName ?? property.Name;

                        var expressionMethodInfo = declaringType.GetMethod(
                            methodName, 
                            BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

                        if (expressionMethodInfo == null)
                        {
                            throw new ArgumentException(String.Format(
                                CultureInfo.InvariantCulture, 
                                "Method specified in ExpressionMethod attribute of '{0}' property in '{1}' class is not found.", 
                                property.Name, 
                                resultElementType.Name));
                        }

                        var customBindingExpression = (LambdaExpression) expressionMethodInfo.Invoke(null, new object[] { this.Scope });

                        // localize expression (replace its parameter with local entityParameter)
                        var localizedcustomBindingExpression = new ExpressionParameterReplacer(
                            customBindingExpression.Parameters.Single(), 
                            entityParameter)
                            .Visit(customBindingExpression.Body);

                        bindings.Add(Expression.Bind(property, localizedcustomBindingExpression));
                    }
                }

                var initExpression = Expression.MemberInit(Expression.New(resultElementType), bindings);

                // todo: add cache for selectors
                var selectorLambda = entityParameter.ToLambda(initExpression);

                var originalQuery = e.MethodCall.Arguments.First();

                e.SubstituteExpression = originalQuery.Select(selectorLambda);
            }
        }

        #endregion
    }
}