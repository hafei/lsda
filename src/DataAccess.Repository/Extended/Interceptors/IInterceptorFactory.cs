// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInterceptorFactory.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Extended.Interceptors
{
    using System;

    /// <summary>
    /// Creates interceptor instances
    /// </summary>
    public interface IInterceptorFactory
    {
        #region Public Methods

        /// <summary>
        /// Creates the query interceptor of specified type.
        /// </summary>
        /// <param name="type">
        /// The interceptor type.
        /// </param>
        /// <returns>
        /// Interceptor instance
        /// </returns>
        IQueryInterceptor CreateQueryInterceptor(Type type);

        /// <summary>
        /// Creates the operation interceptor of specified type.
        /// </summary>
        /// <param name="type">
        /// The interceptor type.
        /// </param>
        /// <returns>
        /// Interceptor instance
        /// </returns>
        IOperationInterceptor CreateOperationInterceptor(Type type);

        #endregion
    }
}