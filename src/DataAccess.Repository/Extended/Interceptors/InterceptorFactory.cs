// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterceptorFactory.cs" company="Logic Software">
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
    public class InterceptorFactory : IInterceptorFactory
    {
        #region Implemented Interfaces (Methods)

        #region IInterceptorFactory methods

        /// <summary>
        /// Creates the interceptor of specified type.
        /// </summary>
        /// <param name="type">
        /// The interceptor type.
        /// </param>
        /// <returns>
        /// Interceptor instance
        /// </returns>
        public IQueryInterceptor CreateQueryInterceptor(Type type)
        {
            // todo: add cache? Expression.New -> Compile
            return (IQueryInterceptor) Activator.CreateInstance(type);
        }

        /// <summary>
        /// Creates the operation interceptor of specified type.
        /// </summary>
        /// <param name="type">The interceptor type.</param>
        /// <returns>Interceptor instance</returns>
        public IOperationInterceptor CreateOperationInterceptor(Type type)
        {
            return (IOperationInterceptor)Activator.CreateInstance(type);
        }

        #endregion

        #endregion
    }
}