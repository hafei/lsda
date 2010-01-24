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

            // working only with static extension methods with one "this" argument
            // todo: instance methods can be added too
            // todo: maybe better check?
            if (e.MethodCall.Method.IsStatic && e.MethodCall.Arguments.Count == 1)
            {
                var expressionAttribute = (ExpressionAttribute) e.MethodCall.Method.GetCustomAttributes(typeof(ExpressionAttribute), false).SingleOrDefault();

                if (expressionAttribute == null)
                {
                    throw new InvalidOperationException(String.Format(
                        CultureInfo.InvariantCulture, 
                        "Method '{0}' in '{1}' class has no Expression attribute.", 
                        e.MethodCall.Method.Name, 
                        e.MethodCall.Method.DeclaringType.Name));
                }

                var declaringType = expressionAttribute.DeclaringType ?? e.MethodCall.Method.DeclaringType;
                var methodName = expressionAttribute.MethodName ?? e.MethodCall.Method.Name;

                var expressionMethodInfo = declaringType.GetMethod(
                    methodName, 
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

                if (expressionMethodInfo == null)
                {
                    throw new ArgumentException(String.Format(
                        CultureInfo.InvariantCulture, 
                        "Method specified in Expression attribute of '{0}' method in '{1}' class is not found.", 
                        e.MethodCall.Method.Name, 
                        e.MethodCall.Method.DeclaringType.Name));
                }

                var customExpandedExpression = (LambdaExpression) expressionMethodInfo.Invoke(null, new object[] { this.Scope });

                // validating custom expanded expression
                if (customExpandedExpression.Parameters.Single().Type != e.MethodCall.Arguments.Single().Type
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
                    e.MethodCall.Arguments.Single())
                    .Visit(customExpandedExpression.Body);

                e.SubstituteExpression = localizedCustomExpandedExpression;
            }
        }

        #endregion
    }
}