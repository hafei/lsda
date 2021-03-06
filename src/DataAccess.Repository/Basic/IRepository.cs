﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepository.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   Entities Repository Interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Basic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Entities Repository Interface
    /// </summary>
    public interface IRepository
    {
        #region Public Methods

        /// <summary>
        /// Returns query for all entities of type T.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the entity.
        /// </typeparam>
        /// <returns>
        /// Query for all entities of type T.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By design, to allow type-safe calls.")]
        IQueryable<T> All<T>() where T : class;

        /// <summary>
        /// Returns query for all entities of type T.
        /// Non-generic overload.
        /// </summary>
        /// <param name="entityType">
        /// Type of the entity.
        /// </param>
        /// <returns>
        /// Query for all entities of type T.
        /// </returns>
        IQueryable All(Type entityType);

        /// <summary>
        /// Returns query for all entities of type T with specified load options.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the entity.
        /// </typeparam>
        /// <param name="loadOptions">
        /// The load options.
        /// </param>
        /// <returns>
        /// Query for all entities of type T.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By design, to allow type-safe calls.")]
        IQueryable<T> All<T>(LoadOptions loadOptions) where T : class;

        /// <summary>
        /// Returns query for all entities of type T with specified load options.
        /// Non-generic overload.
        /// </summary>
        /// <param name="entityType">
        /// Type of the entity.
        /// </param>
        /// <param name="loadOptions">
        /// The load options.
        /// </param>
        /// <returns>
        /// Query for all entities of type T.
        /// </returns>
        IQueryable All(Type entityType, LoadOptions loadOptions);

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