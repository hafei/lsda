// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleMappingSourceManager.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The sample mapping source manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BasicSample.Mapping
{
    using System.Reflection;

    using LogicSoftware.DataAccess.Repository.Mapping;

    /// <summary>
    /// The sample mapping source manager.
    /// </summary>
    internal class SampleMappingSourceManager : XmlMappingSourceManager
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleMappingSourceManager"/> class.
        /// </summary>
        public SampleMappingSourceManager()
            : base(Assembly.GetExecutingAssembly().GetManifestResourceStream("BasicSample.Mapping.LinqToSqlMapping.xml"))
        {
        }

        #endregion
    }
}