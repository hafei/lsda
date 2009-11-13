// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectionQueryInterceptorTest.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   The projection query interceptor test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests
{
    using System.Linq;

    using Basic;

    using Extended;
    using Extended.Interceptors.Common;

    using Memory;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SampleModel;
    using SampleModel.Mapping;
    using SampleModel.Projections;

    /// <summary>
    /// The projection query interceptor test.
    /// </summary>
    [TestClass]
    public class ProjectionQueryInterceptorTest : UnitTestBase
    {
        #region Public Methods

        /// <summary>
        /// The projections_should_support_deep_level_properties.
        /// </summary>
        [TestMethod]
        public void Projections_should_support_deep_level_properties()
        {
            // Arrange
            var repository = CreateSampleEntityRepository();
            this.Container.RegisterInstance<IRepository>(repository);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            var result = extendedRepository.All<SampleChildEntity>().Select(new SampleChildEntityView()).ToList();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count);

            // todo: here we suppose that order of entities is preserved
            Assert.AreEqual("Child '2 1' 1", result[1].Name);
            Assert.AreEqual("Child '2 1' 1", result[1].EntityName);
            Assert.AreEqual("Parent 2 1", result[1].ParentName);
            Assert.AreEqual("SuperParent 2", result[1].SuperParentName);
        }

        /// <summary>
        /// The projections_should_support_expressions.
        /// </summary>
        [TestMethod]
        public void Projections_should_support_expressions()
        {
            // Arrange
            var repository = CreateSampleEntityRepository();
            this.Container.RegisterInstance<IRepository>(repository);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            var result = extendedRepository.All<SampleParentEntity>().Select(new SampleParentEntityView()).ToList();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);

            foreach (var parentEntityView in result)
            {
                Assert.AreEqual(1, parentEntityView.Children.Count);
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

            memoryRepository.Insert(new SampleSuperParentEntity() { Id = 1, Name = "SuperParent 1" });
                memoryRepository.Insert(new SampleParentEntity() { Id = 1, Name = "Parent 1 1", SuperParentId = 1 });
                    memoryRepository.Insert(new SampleChildEntity() { Id = 1, Name = "Child '1 1' 1", ParentId = 1 });

            memoryRepository.Insert(new SampleSuperParentEntity() { Id = 2, Name = "SuperParent 2" });
                memoryRepository.Insert(new SampleParentEntity() { Id = 2, Name = "Parent 2 1", SuperParentId = 2 });
                    memoryRepository.Insert(new SampleChildEntity() { Id = 2, Name = "Child '2 1' 1", ParentId = 2 });
                memoryRepository.Insert(new SampleParentEntity() { Id = 3, Name = "Parent 2 2", SuperParentId = 2 });
                    memoryRepository.Insert(new SampleChildEntity() { Id = 3, Name = "Child '2 2' 1", ParentId = 3 });
                    memoryRepository.Insert(new SampleChildEntity() { Id = 4, Name = "Child '2 2' 2", ParentId = 3 });

            return memoryRepository;
        }

        #endregion
    }
}