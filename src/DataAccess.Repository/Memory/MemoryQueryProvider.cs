// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryQueryProvider.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Memory
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq.Mapping;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Infrastructure.Helpers;

    using LogicSoftware.Infrastructure.Linq;

    /// <summary>
    /// Query provider for memory repository results
    /// </summary>
    /// <typeparam name="T">
    /// Entity Type
    /// </typeparam>
    internal class MemoryQueryProvider<T> : ExpressionVisitor, IQueryProvider
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryQueryProvider&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="innerQuery">The inner query.</param>
        /// <param name="repository">The repository.</param>
        public MemoryQueryProvider(IQueryable<T> innerQuery, MemoryRepository repository)
        {
            this.InnerQuery = innerQuery;
            this.Repository = repository;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the inner query.
        /// </summary>
        /// <value>The inner query.</value>
        private IQueryable<T> InnerQuery { get; set; }

        /// <summary>
        /// Gets or sets the repository.
        /// </summary>
        /// <value>The repository.</value>
        private MemoryRepository Repository { get; set; }

        #endregion

        #region Implemented Interfaces (Methods)

        #region IQueryProvider methods

        /// <summary>
        /// Constructs an <see cref="T:System.Linq.IQueryable"/> object that can evaluate the query represented by a specified expression tree.
        /// </summary>
        /// <param name="expression">
        /// An expression tree that represents a LINQ query.
        /// </param>
        /// <returns>
        /// An <see cref="T:System.Linq.IQueryable"/> that can evaluate the query represented by the specified expression tree.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public IQueryable CreateQuery(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            Type elementType = TypeSystem.GetElementType(expression.Type);
            if (!typeof(IQueryable<>).MakeGenericType(elementType).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentException("Invalid type expression", "expression");
            }

            return (IQueryable)Activator.CreateInstance(typeof(MemoryQueryable<>).MakeGenericType(elementType), new object[] { this, expression });
        }

        /// <summary>
        /// Creates the query.
        /// </summary>
        /// <typeparam name="TElement">
        /// The type of the element.
        /// </typeparam>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <returns>
        /// The query.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new MemoryQueryable<TElement>(this, expression);
        }

        /// <summary>
        /// Executes the query represented by a specified expression tree.
        /// </summary>
        /// <param name="expression">
        /// An expression tree that represents a LINQ query.
        /// </param>
        /// <returns>
        /// The value that results from executing the specified query.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public object Execute(Expression expression)
        {
            return this.Execute<object>(expression);
        }

        /// <summary>
        /// Executes the specified expression.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <returns>
        /// Execution result
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public TResult Execute<TResult>(Expression expression)
        {
            Expression convertedExpression = this.Visit(expression);

            var innerProvider = this.InnerQuery.Provider;

            return innerProvider.Execute<TResult>(convertedExpression);
        }
        #endregion

        /// <summary>
        /// Analyzes the constant expression provided as parameter and
        /// returns an appropiated constant expression.
        /// </summary>
        /// <param name="constant">The constant expression to analyze.</param>
        /// <returns>A System.Linq.Expressions.Expression.</returns>
        protected override Expression VisitConstant(ConstantExpression constant)
        {
            if (constant.Value is MemoryQueryable<T>)
            {
                return Expression.Constant(this.InnerQuery);
            }

            return base.VisitConstant(constant);
        }

        /// <summary>
        /// Analyzes the member access expression provided as parameter and
        /// returns an appropiated member access.
        /// </summary>
        /// <param name="member">The member access to analyze.</param>
        /// <returns>A System.Linq.Expressions.Expression.</returns>
        protected override Expression VisitMemberAccess(MemberExpression member)
        {
            Type memberType = member.Type;
            if (member.Expression != null)
            {
                Type objectType = member.Expression.Type;

                MetaModel model = this.Repository.GetModel();

                MetaType memberMetaType = model.GetMetaType(memberType);
                MetaType objectMetaType = model.GetMetaType(objectType);

                if (objectMetaType.Table != null)
                {
                    if (memberMetaType.Table != null)
                    {
                        MethodInfo selector = this.Repository.GetType().GetMethod("GetSingleByPropertyValue", BindingFlags.Instance | BindingFlags.NonPublic)
                            .MakeGenericMethod(memberType);

                        return this.CreateExplicitJoinMethodCall(member, objectMetaType, selector);
                    }
                    else
                    {
                        var listInterface = memberType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>)).SingleOrDefault();

                        if (listInterface != null)
                        {
                            var listElementType = listInterface.GetGenericArguments()[0];

                            var listElementMetaType = model.GetMetaType(listElementType);

                            if (listElementMetaType.Table != null)
                            {
                                MethodInfo selector = this.Repository.GetType().GetMethod("GetAllByPropertyValue", BindingFlags.Instance | BindingFlags.NonPublic)
                                    .MakeGenericMethod(listElementType);

                                return this.CreateExplicitJoinMethodCall(member, objectMetaType, selector);
                            }
                        }
                    }
                }
            }
        
            return base.VisitMemberAccess(member);
        }

        /// <summary>
        /// Creates the explicit join method call.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="objectMetaType">Type of the object meta.</param>
        /// <param name="selector">The selector.</param>
        /// <returns>Explicit join method call expression</returns>
        private Expression CreateExplicitJoinMethodCall(MemberExpression member, MetaType objectMetaType, MethodInfo selector)
        {
            MetaAssociation association = objectMetaType.Associations.Where(a => a.ThisMember.Member == member.Member).SingleOrDefault();

            Expression visitedMemberExpression = Visit(member.Expression);

            var thisSideKeyExpression =
                Expression.Convert(
                    Expression.Property(visitedMemberExpression, association.ThisKey.Single().Member as PropertyInfo),
                    typeof(object));

            var thisPropertyExpression = Expression.Constant(association.OtherKey.Single().Member, typeof(PropertyInfo));

            var repositoryConst = Expression.Constant(this.Repository);

            var explicitJoinMethodCallExpression = Expression.Call(repositoryConst, selector, thisPropertyExpression, thisSideKeyExpression);

            return explicitJoinMethodCallExpression;
        }

        #endregion
    }
}