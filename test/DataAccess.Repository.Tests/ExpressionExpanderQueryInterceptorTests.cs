// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionExpanderQueryInterceptorTests.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The expression expander query interceptor tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests
{
    using System.Linq;

    using Basic;

    using Extended;

    using Memory;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SampleModel;
    using SampleModel.Extensions;
    using SampleModel.Mapping;

    /// <summary>
    /// The expression expander query interceptor tests.
    /// </summary>
    [TestClass]
    public class ExpressionExpanderQueryInterceptorTests : UnitTestBase
    {
        #region Public Methods

        /// <summary>
        /// The extension_should_be_expanded_in_ order by_clause.
        /// </summary>
        [TestMethod]
        public void Extension_should_be_expanded_in_OrderBy_clause()
        {
            // Arrange
            var repository = CreateSampleEntityRepository();
            this.Container.RegisterInstance<IRepository>(repository);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            var result = extendedRepository.All<SampleEntity>()
                .OrderBy(e => e.IsSample())
                .ToList();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count);

            Assert.AreNotEqual("Sample", result[0].Name);
            Assert.AreNotEqual("Sample", result[1].Name);
            Assert.AreEqual("Sample", result[2].Name);
            Assert.AreEqual("Sample", result[3].Name);
            Assert.AreEqual("Sample", result[4].Name);
        }

        /// <summary>
        /// The extension_should_be_expanded_in_ select_clause.
        /// </summary>
        [TestMethod]
        public void Extension_should_be_expanded_in_Select_clause()
        {
            // Arrange
            var repository = CreateSampleEntityRepository();
            this.Container.RegisterInstance<IRepository>(repository);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            var result = extendedRepository.All<SampleEntity>()
                .Select(e => new { Name = e.Name, IsSample = e.IsSample() })
                .ToList();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count);

            foreach (var sampleEntity in result)
            {
                Assert.IsTrue(sampleEntity.IsSample ? sampleEntity.Name == "Sample" : sampleEntity.Name != "Sample");
            }
        }

        /// <summary>
        /// The extension_should_be_expanded_in_ where_clause.
        /// </summary>
        [TestMethod]
        public void Extension_should_be_expanded_in_Where_clause()
        {
            // Arrange
            var repository = CreateSampleEntityRepository();
            this.Container.RegisterInstance<IRepository>(repository);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            var result = extendedRepository.All<SampleEntity>()
                .Where(e => e.IsSample())
                .ToList();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);

            foreach (var sampleEntity in result)
            {
                Assert.AreEqual("Sample", sampleEntity.Name);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the sample entity repository mock.
        /// </summary>
        /// <returns>
        /// The sample entity repository mock.
        /// </returns>
        private static IRepository CreateSampleEntityRepository()
        {
            var memoryRepository = new MemoryRepository(new MappingSourceManager());

            memoryRepository.Insert(new SampleEntity() { Id = 1, Name = "Entity 1" });
            memoryRepository.Insert(new SampleEntity() { Id = 2, Name = "Sample" });
            memoryRepository.Insert(new SampleEntity() { Id = 3, Name = "Entity 3" });
            memoryRepository.Insert(new SampleEntity() { Id = 4, Name = "Sample" });
            memoryRepository.Insert(new SampleEntity() { Id = 5, Name = "Sample" });

            return memoryRepository;
        }

        #endregion
    }
}