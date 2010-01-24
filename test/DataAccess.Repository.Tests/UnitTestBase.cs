// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitTestBase.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The unit test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests
{
    using Infrastructure.EntLib.Unity;

    using Mapping;

    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SampleModel.Mapping;

    /// <summary>
    /// The unit test base class.
    /// </summary>
    [TestClass]
    public class UnitTestBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets Unity container.
        /// </summary>
        protected IUnityContainer Container { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// The init container TestInitialize method.
        /// </summary>
        [TestInitialize]
        public void InitContainer()
        {
            this.Container = UnityBootstrapper.Bootstrap();

            // todo: move to config?
            this.Container.RegisterType<IMappingSourceManager, MappingSourceManager>();
        }

        #endregion
    }
}