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
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Interface of repository that allows method ant type interception.
    /// </summary>
    public interface IExtendedRepository
    {
        #region Public Methods

        /// <summary>
        /// Returns query for all entities of type T.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type
        /// </typeparam>
        /// <returns>
        /// Query for all entities of type T
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By design, to allow type-safe calls.")]
        IQueryable<T> All<T>() where T : class;

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type
        /// </typeparam>
        /// <param name="entity">
        /// The entity.
        /// </param>
        void Delete<T>(T entity) where T : class;

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type
        /// </typeparam>
        /// <param name="entity">
        /// The entity.
        /// </param>
        void Insert<T>(T entity) where T : class;

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type
        /// </typeparam>
        /// <param name="entity">
        /// The entity.
        /// </param>
        void Update<T>(T entity) where T : class;

        #endregion
    }
}