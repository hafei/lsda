﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewQueryInterceptor.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   Typed Views interceptor
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

    using Attributes.Views;

    using Events;

    using Infrastructure.Extensions;
    using Infrastructure.Helpers;
    using Infrastructure.Linq;

    /// <summary>
    /// Typed Views interceptor
    /// </summary>
    public class ViewQueryInterceptor : QueryInterceptor<IScope>
    {
        #region Constants and Fields

        /// <summary>
        /// The Select method.
        /// </summary>
        private static readonly MethodInfo SelectViewMethod = typeof(QueryableExtensions).GetMethod("Select", BindingFlags.Static | BindingFlags.Public);

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
            if (e.MethodCall.Method.IsGenericMethod &&
                e.MethodCall.Method.GetGenericMethodDefinition() == SelectViewMethod)
            {
                var elementType = TypeSystem.GetElementType(e.MethodCall.Arguments.First().Type);
                var resultElementType = TypeSystem.GetElementType(e.MethodCall.Type);

                var entityParameter = Expression.Parameter(elementType, "p");

                var bindings = new List<MemberBinding>();

                foreach (PropertyInfo property in resultElementType.GetProperties())
                {
                    var attribute = (PropertyAttribute) property.GetCustomAttributes(typeof(PropertyAttribute), false).SingleOrDefault();

                    if (attribute != null)
                    {
                        bindings.Add(Expression.Bind(property, entityParameter.PropertyPath(attribute.Path)));
                    }

                    var expressionAttribute = (ExpressionAttribute) property.GetCustomAttributes(typeof(ExpressionAttribute), false).SingleOrDefault();

                    if (expressionAttribute != null)
                    {
                        var declaringType = expressionAttribute.DeclaringType ?? resultElementType;

                        var expressionMethodInfo = declaringType.GetMethod(
                            expressionAttribute.MethodName,
                            BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

                        if (expressionMethodInfo == null)
                        {
                            throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "Method specified in ExpressionMethod attribute of '{0}' property in '{1}' class is not found", property.Name, resultElementType.Name));
                        }

                        var customBindingExpression = (LambdaExpression) expressionMethodInfo.Invoke(null, new object[] { this.Scope });

                        // localize expression (replace its parameter with local entityParameter)
                        var localizedcustomBindingExpression = new ExpressionParameterReplacer(customBindingExpression.Parameters.Single(), entityParameter)
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