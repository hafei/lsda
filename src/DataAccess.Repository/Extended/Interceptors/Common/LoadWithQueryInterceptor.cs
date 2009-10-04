// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadWithQueryInterceptor.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The Load With query interceptor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Interceptors.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Events;

    /// <summary>
    /// The Load With query interceptor.
    /// </summary>
    public class LoadWithQueryInterceptor : QueryInterceptor<IScope>
    {
        #region Constants and Fields

        /// <summary>
        /// The load with method.
        /// </summary>
        private static readonly MethodInfo LoadWithMethod = typeof(QueryableExtensions).GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(m => m.Name == "LoadWith")
            .Where(m => m.GetGenericArguments().Length == 1)
            .Single();

        /// <summary>
        /// The load with parent method.
        /// </summary>
        private static readonly MethodInfo LoadWithParentMethod = typeof(QueryableExtensions).GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(m => m.Name == "LoadWith")
            .Where(m => m.GetGenericArguments().Length == 2)
            .Single();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadWithQueryInterceptor"/> class.
        /// </summary>
        public LoadWithQueryInterceptor()
        {
            this.LoadWithExpressions = new List<LambdaExpression>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the load with expressions.
        /// </summary>
        /// <value>The load with expressions.</value>
        private List<LambdaExpression> LoadWithExpressions { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// The LoadOptionsCreating stage handler.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Extended.Events.LoadOptionsCreatingEventArgs"/> instance containing the event data.
        /// </param>
        public override void OnLoadOptionsCreating(LoadOptionsCreatingEventArgs e)
        {
            // getting all LoadWith expressions from tree and removing all LoadWith method calls
            e.Expression = this.Visit(e.Expression);

            foreach (LambdaExpression expression in this.LoadWithExpressions)
            {
                e.LoadOptions.LoadWith(expression);
            }
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
            if (methodCall.Method.IsGenericMethod && 
                (methodCall.Method.GetGenericMethodDefinition() == LoadWithMethod ||
                 methodCall.Method.GetGenericMethodDefinition() == LoadWithParentMethod))
            {
                // saving LoadWith expression
                this.LoadWithExpressions.Add((LambdaExpression)((UnaryExpression) methodCall.Arguments[1]).Operand);

                // removing MethodCall from tree
                return this.Visit(methodCall.Arguments.First());
            }

            return base.VisitMethodCall(methodCall);
        }

        #endregion
    }
}