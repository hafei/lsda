// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExtendedRepository.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   Interface of repository that allows method ant type interception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Interface of repository that allows method ant type interception.
    /// </summary>
    public interface IExtendedRepository
    {
        #region Public Methods

        /// <summary>
        /// Returns the query for all entities of type T.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the entity.
        /// </typeparam>
        /// <returns>
        /// The query for all entities of type T.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By design, to allow type-safe calls.")]
        IQueryable<T> All<T>() where T : class;

        /// <summary>
        /// Returns the query for all entities of specified type.
        /// </summary>
        /// <param name="entityType">
        /// Type of the entity.
        /// </param>
        /// <returns>
        /// The query for all entities of specified type.
        /// </returns>
        IQueryable All(Type entityType);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the entity.
        /// </typeparam>
        /// <param name="entity">
        /// The entity.
        /// </param>
        void Delete<T>(T entity) where T : class;

        /// <summary>
        /// Executes the specified query.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the element of the result sequence.
        /// </typeparam>
        /// <param name="query">
        /// The query expression.
        /// </param>
        /// <returns>
        /// The result of the query execution.
        /// </returns>
        IEnumerable<TResult> Execute<TResult>(Expression query);

        /// <summary>
        /// Executes the specified query.
        /// </summary>
        /// <param name="query">
        /// The query expression.
        /// </param>
        /// <returns>
        /// The result of the query execution.
        /// </returns>
        int Execute(Expression query);

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the entity.
        /// </typeparam>
        /// <param name="entity">
        /// The entity.
        /// </param>
        void Insert<T>(T entity) where T : class;

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the entity.
        /// </typeparam>
        /// <param name="entity">
        /// The entity.
        /// </param>
        void Update<T>(T entity) where T : class;

        #endregion
    }
}