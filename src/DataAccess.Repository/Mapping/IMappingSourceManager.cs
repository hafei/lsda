// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMappingSourceManager.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Mapping
{
    using System.Data.Linq.Mapping;

    /// <summary>
    /// Mappig source manager interface
    /// </summary>
    public interface IMappingSourceManager
    {
        #region Properties

        /// <summary>
        /// Gets the mapping source.
        /// </summary>
        /// <value>The mapping source.</value>
        MappingSource MappingSource { get; }

        /// <summary>
        /// Gets the mapping source with no associations in mapping.
        /// </summary>
        /// <value>The mapping source with no associations in mapping</value>
        MappingSource NoAssociationsMappingSource { get; }

        #endregion
    }
}