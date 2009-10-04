// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewQueryInterceptorTest.cs" company="">
//   
// </copyright>
// <summary>
//   The view query interceptor test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests
{
    using System.Collections.Generic;
    using System.Data.Linq.Mapping;
    using System.Linq;
    using System.Reflection;

    using LogicSoftware.DataAccess.Repository.Extended;
    using LogicSoftware.DataAccess.Repository.Extended.Interceptors.Common;
    using LogicSoftware.DataAccess.Repository.Tests.SampleModel;

    using Memory;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Basic;

    using SampleModel.Mapping;

    /// <summary>
    /// The view query interceptor test.
    /// </summary>
    [TestClass]
    public class ViewQueryInterceptorTest : UnitTestBase
    {
        #region Public Methods

        /// <summary>
        /// The simple_ typed_ view_ no_ parents.
        /// </summary>
        [TestMethod]
        public void Simple_Typed_View_With_Parents()
        {
            // Arrange
            var repository = CreateSampleEntityRepository();
            this.Container.RegisterInstance<IRepository>(repository);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            var result = extendedRepository.All<SampleChildEntity>().Select<SampleChildEntity, SampleChildEntityView>().ToList();

            // Assert
            Assert.IsNotNull(result);
            //Assert.AreEqual(3, result.Count);
            //Assert.AreEqual("SampleEntity 2", result[1].EntityName);
            //Assert.AreEqual(result[2].EntityName, result[2].EntityName);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the sample entity repository mock.
        /// </summary>
        /// <returns>
        /// </returns>
        private static IRepository CreateSampleEntityRepository()
        {
            var memoryRepository = new MemoryRepository(new MappingSourceManager());

            memoryRepository.Insert(new SampleSuperParentEntity() { Id = 1, Name = "SuperParent 1" });
            //memoryRepository.Insert(new SampleSuperParentEntity() { Id = 2, Name = "SuperParent 2" });

            memoryRepository.Insert(new SampleParentEntity() { Id = 1, Name = "Parent 1", SuperParentId = 1 });
            //memoryRepository.Insert(new SampleParentEntity() { Id = 2, Name = "Parent 2", SuperParentId = 1 });
            //memoryRepository.Insert(new SampleParentEntity() { Id = 3, Name = "Parent 3", SuperParentId = 2 });
            //memoryRepository.Insert(new SampleParentEntity() { Id = 4, Name = "Parent 4" });

            memoryRepository.Insert(new SampleChildEntity() { Id = 1, Name = "Child 1", ParentId = 1 });
            //memoryRepository.Insert(new SampleChildEntity() { Id = 2, Name = "Child 2", ParentId = 2 });
            //memoryRepository.Insert(new SampleChildEntity() { Id = 3, Name = "Child 3", ParentId = 3 });
            //memoryRepository.Insert(new SampleChildEntity() { Id = 4, Name = "Child 4", ParentId = 4 });
            //memoryRepository.Insert(new SampleChildEntity() { Id = 5, Name = "Child 5" });

            return memoryRepository;
        }

        #endregion
    }
}