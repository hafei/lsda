// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionExpanderQueryInterceptor.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The custom expression expander query interceptor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Interceptors.Common
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Attributes;

    using Events;

    using Infrastructure.Linq;

    /// <summary>
    /// The custom expression expander query interceptor.
    /// </summary>
    public class ExpressionExpanderQueryInterceptor : QueryInterceptor<IScope>
    {
        #region Public Methods

        /// <summary>
        /// The MethodCallVisit stage handler.
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

            // working with static extension methods with one "this" argument and with instance methods with no arguments
            // todo: methods with multiple arguments can be implemented too
            // todo: maybe better check?
            if ((e.MethodCall.Method.IsStatic && e.MethodCall.Arguments.Count == 1)
                || (!e.MethodCall.Method.IsStatic && e.MethodCall.Arguments.Count == 0))
            {
                var expressionAttribute = (ExpandWithExpressionAttribute) e.MethodCall.Method.GetCustomAttributes(typeof(ExpandWithExpressionAttribute), false).SingleOrDefault();

                if (expressionAttribute == null)
                {
                    throw new InvalidOperationException(String.Format(
                        CultureInfo.InvariantCulture, 
                        "Method '{0}' in '{1}' class has no ExpandWithExpression attribute.", 
                        e.MethodCall.Method.Name, 
                        e.MethodCall.Method.DeclaringType.Name));
                }

                var declaringType = expressionAttribute.DeclaringType ?? e.MethodCall.Method.DeclaringType;
                var methodName = expressionAttribute.MethodName ?? e.MethodCall.Method.Name;

                var expressionMethodInfo = declaringType.GetMethod(
                    methodName, 
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                if (expressionMethodInfo == null)
                {
                    throw new ArgumentException(String.Format(
                        CultureInfo.InvariantCulture, 
                        "Method specified in ExpandWithExpression attribute of '{0}' method in '{1}' class is not found.", 
                        e.MethodCall.Method.Name, 
                        e.MethodCall.Method.DeclaringType.Name));
                }

                var customExpandedExpression = (LambdaExpression) expressionMethodInfo.Invoke(null, new object[] { this.Scope });

                // parameterExpression is object in case of instance method or single (todo: first) argument in case of extension method
                var parameterExpression = e.MethodCall.Method.IsStatic ? e.MethodCall.Arguments.Single() : e.MethodCall.Object;

                // validating custom expanded expression
                if (customExpandedExpression.Parameters.Single().Type != parameterExpression.Type
                    || customExpandedExpression.Body.Type != e.MethodCall.Type)
                {
                    throw new InvalidOperationException(String.Format(
                        CultureInfo.InvariantCulture, 
                        "Method '{0}' in '{1}' class returns invalid expression.", 
                        expressionMethodInfo.Name, 
                        expressionMethodInfo.DeclaringType.Name));
                }

                // localize expression (replace its parameter with local object expression)
                var localizedCustomExpandedExpression = new ExpressionParameterReplacer(
                        customExpandedExpression.Parameters.Single(), 
                        parameterExpression)
                    .Visit(customExpandedExpression.Body);

                e.SubstituteExpression = localizedCustomExpandedExpression;
            }
        }

        #endregion
    }
}