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
        #region Constants and Fields

        /// <summary>
        /// The expand method.
        /// </summary>
        private static readonly MethodInfo ExpandMethod = typeof(QueryableExtensions).GetMethod("Expand", BindingFlags.Static | BindingFlags.Public);

        #endregion

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

            if (e.MethodCall.Method.IsGenericMethod &&
                e.MethodCall.Method.GetGenericMethodDefinition() == ExpandMethod)
            {
                e.SubstituteExpression = e.MethodCall.Arguments.Single();
            }
        }

        /// <summary>
        /// The PreExecute stage handler.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.PreExecuteEventArgs"/> instance containing the event data.
        /// </param>
        public override void OnPreExecute(PreExecuteEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            e.Expression = this.Visit(e.Expression);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Analyzes the method call expression provided as parameter and
        /// returns an appropiated member access.
        /// </summary>
        /// <param name="methodCall">
        /// The method call to analyze.
        /// </param>
        /// <returns>
        /// A System.Linq.Expressions.Expression.
        /// </returns>
        protected override Expression VisitMethodCall(MethodCallExpression methodCall)
        {
            if (methodCall == null)
            {
                throw new ArgumentNullException("methodCall");
            }

            // todo: add cache

            // checking that method is extension method with one argument (todo: maybe better check?)
            if (methodCall.Method.IsStatic && methodCall.Arguments.Count == 1)
            {
                // todo: maybe subscribe to all queries and expand MethodCalls of subscribed entities types only
                var expressionAttribute = (ExpressionAttribute) methodCall.Method.GetCustomAttributes(typeof(ExpressionAttribute), false).SingleOrDefault();

                if (expressionAttribute != null)
                {
                    var declaringType = expressionAttribute.DeclaringType ?? methodCall.Method.DeclaringType;

                    var expressionMethodInfo = declaringType.GetMethod(
                        expressionAttribute.MethodName, 
                        BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

                    if (expressionMethodInfo == null)
                    {
                        throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "Method specified in Expression attribute of '{0}' method in '{1}' class is not found.", methodCall.Method.Name, methodCall.Method.DeclaringType.Name));
                    }

                    var customBindingExpression = (LambdaExpression) expressionMethodInfo.Invoke(null, new object[] { this.Scope });

                    // localize expression (replace its parameter with local object expression)
                    var localizedcustomBindingExpression = new ExpressionParameterReplacer(customBindingExpression.Parameters.Single(), methodCall.Arguments.Single())
                        .Visit(customBindingExpression.Body);

                    return localizedcustomBindingExpression;
                }
            }

            return base.VisitMethodCall(methodCall);
        }

        #endregion
    }
}