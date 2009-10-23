// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadWithOption.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The load with option.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Basic
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Infrastructure.Linq;

    /// <summary>
    /// The load with option.
    /// </summary>
    public class LoadWithOption
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadWithOption"/> class.
        /// </summary>
        /// <param name="expression">
        /// The LoadWith expression.
        /// </param>
        public LoadWithOption(LambdaExpression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            // checking for simple case - LoadWith without filtering
            var bodyAsMemberExpression = expression.Body as MemberExpression;
            if (bodyAsMemberExpression != null && bodyAsMemberExpression.Expression == expression.Parameters.Single())
            {
                this.Member = expression;
                this.Association = null;
            }
            else
            {
                var splitter = new LoadWithOptionNormalizer();

                splitter.Visit(expression, expression.Parameters.Single());

                this.Member = splitter.MemberExpression;
                this.Association = splitter.AssociationExpression;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the full association lambda.
        /// Eg. project => project.Tasks.Where(...).
        /// </summary>
        /// <value>The member.</value>
        public LambdaExpression Association { get; set; }

        /// <summary>
        /// Gets or sets the member lambda only.
        /// Eg. project => project.Tasks.
        /// </summary>
        /// <value>The member.</value>
        public LambdaExpression Member { get; set; }

        #endregion

        #region Nested classes

        /// <summary>
        /// The private LoadWithOption normalizer.
        /// </summary>
        private class LoadWithOptionNormalizer : ExpressionVisitor
        {
            #region Constants and Fields

            /// <summary>
            /// The as enumerable method.
            /// </summary>
            private static readonly MethodInfo AsEnumerableMethod = typeof(Enumerable).GetMethod("AsEnumerable", BindingFlags.Public | BindingFlags.Static);

            /// <summary>
            /// The to array method.
            /// </summary>
            private static readonly MethodInfo ToArrayMethod = typeof(Enumerable).GetMethod("ToArray", BindingFlags.Public | BindingFlags.Static);

            /// <summary>
            /// The to list method.
            /// </summary>
            private static readonly MethodInfo ToListMethod = typeof(Enumerable).GetMethod("ToList", BindingFlags.Public | BindingFlags.Static);

            #endregion

            #region Properties

            /// <summary>
            /// Gets association expression.
            /// </summary>
            public LambdaExpression AssociationExpression { get; private set; }

            /// <summary>
            /// Gets the member expression.
            /// </summary>
            public LambdaExpression MemberExpression { get; private set; }

            /// <summary>
            /// Gets or sets EntityParameter.
            /// </summary>
            private ParameterExpression EntityParameter { get; set; }

            #endregion

            #region Public Methods

            /// <summary>
            /// The custom visit method.
            /// </summary>
            /// <param name="expression">
            /// The expression.
            /// </param>
            /// <param name="entityParameter">
            /// The entity parameter.
            /// </param>
            public void Visit(Expression expression, ParameterExpression entityParameter)
            {
                this.EntityParameter = entityParameter;
                this.AssociationExpression = (LambdaExpression) this.Visit(expression);
            }

            #endregion

            #region Methods

            /// <summary>
            /// Analyzes the member access expression provided as parameter and
            /// returns an appropiated member access.
            /// </summary>
            /// <param name="member">
            /// The member access to analyze.
            /// </param>
            /// <returns>
            /// A System.Linq.Expressions.Expression.
            /// </returns>
            protected override Expression VisitMemberAccess(MemberExpression member)
            {
                if (member == null)
                {
                    throw new ArgumentNullException("member");
                }

                // checking if we've reached first level property
                if (member.Expression == this.EntityParameter)
                {
                    if (this.MemberExpression != null)
                    {
                        throw new InvalidOperationException("MemberExpression is already set. Check your LoadWith expression.");
                    }

                    this.MemberExpression = Expression.Lambda(member, this.EntityParameter);
                }

                return base.VisitMemberAccess(member);
            }

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

                // todo: maybe better way and place exists
                if (methodCall.Method.IsGenericMethod &&
                    (methodCall.Method.GetGenericMethodDefinition() == ToListMethod ||
                     methodCall.Method.GetGenericMethodDefinition() == ToArrayMethod ||
                     methodCall.Method.GetGenericMethodDefinition() == AsEnumerableMethod))
                {
                    // removing MethodCall from tree  
                    return this.Visit(methodCall.Arguments.Single());
                }

                return base.VisitMethodCall(methodCall);
            }

            #endregion
        }

        #endregion
    }
}