// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationInterceptorTests.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   Summary description for InterceptorTest
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using Basic;

    using Extended;
    using Extended.Interceptors;

    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SampleModel;
    using SampleModel.Interceptors;

    /// <summary>
    /// Summary description for InterceptorTest
    /// </summary>
    [TestClass]
    public class Operation : UnitTestBase
    {
        #region Public Methods

        /// <summary>
        /// The interceptor_scope_is_set_on_ insert.
        /// </summary>
        [TestMethod]
        public void Interceptor_scope_is_set_on_Insert()
        {
            // Arrange
            var mockRepository = CreateSampleEntityRepositoryMock();
            this.Container.RegisterInstance<IRepository>(mockRepository.Object);

            TestOperationInterceptor interceptor = new TestOperationInterceptor();

            var mockInterceptorFactory = new Mock<IInterceptorFactory>();
            mockInterceptorFactory
                .Setup(f => f.CreateOperationInterceptor(typeof(TestOperationInterceptor)))
                .Returns(interceptor);

            this.Container.RegisterInstance<IInterceptorFactory>(mockInterceptorFactory.Object);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            SampleEntity newEntity = new SampleEntity();
            extendedRepository.Insert(newEntity);

            // Assert
            Assert.IsInstanceOfType(interceptor.PublicScope, typeof(TestScope));
        }

        /// <summary>
        /// The interceptor_should_be_fired_on_ delete.
        /// </summary>
        [TestMethod]
        public void Interceptor_should_be_fired_on_Delete()
        {
            // Arrange
            var mockRepository = CreateSampleEntityRepositoryMock();
            this.Container.RegisterInstance<IRepository>(mockRepository.Object);

            TestOperationInterceptor interceptor = new TestOperationInterceptor();

            var mockInterceptorFactory = new Mock<IInterceptorFactory>();
            mockInterceptorFactory
                .Setup(f => f.CreateOperationInterceptor(typeof(TestOperationInterceptor)))
                .Returns(interceptor);

            this.Container.RegisterInstance<IInterceptorFactory>(mockInterceptorFactory.Object);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            SampleEntity newEntity = new SampleEntity() { Id = 1 };
            extendedRepository.Delete(newEntity);

            // Assert
            Assert.IsNotNull(interceptor.LastDeletingEntity);
            Assert.AreEqual(newEntity, interceptor.LastDeletingEntity);

            Assert.IsNotNull(interceptor.LastDeletedEntity);
            Assert.AreEqual(newEntity, interceptor.LastDeletedEntity);
        }

        /// <summary>
        /// The interceptor_should_be_fired_on_ insert.
        /// </summary>
        [TestMethod]
        public void Interceptor_should_be_fired_on_Insert()
        {
            // Arrange
            var mockRepository = CreateSampleEntityRepositoryMock();
            this.Container.RegisterInstance<IRepository>(mockRepository.Object);

            TestOperationInterceptor interceptor = new TestOperationInterceptor();

            var mockInterceptorFactory = new Mock<IInterceptorFactory>();
            mockInterceptorFactory
                .Setup(f => f.CreateOperationInterceptor(typeof(TestOperationInterceptor)))
                .Returns(interceptor);

            this.Container.RegisterInstance<IInterceptorFactory>(mockInterceptorFactory.Object);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            SampleEntity newEntity = new SampleEntity();
            extendedRepository.Insert(newEntity);

            // Assert
            Assert.IsNotNull(interceptor.LastInsertingEntity);
            Assert.AreEqual(newEntity, interceptor.LastInsertingEntity);

            Assert.IsNotNull(interceptor.LastInsertedEntity);
            Assert.AreEqual(newEntity, interceptor.LastInsertedEntity);
        }

        /// <summary>
        /// The interceptor_should_be_fired_on_ update.
        /// </summary>
        [TestMethod]
        public void Interceptor_should_be_fired_on_Update()
        {
            // Arrange
            var mockRepository = CreateSampleEntityRepositoryMock();
            this.Container.RegisterInstance<IRepository>(mockRepository.Object);

            TestOperationInterceptor interceptor = new TestOperationInterceptor();

            var mockInterceptorFactory = new Mock<IInterceptorFactory>();
            mockInterceptorFactory
                .Setup(f => f.CreateOperationInterceptor(typeof(TestOperationInterceptor)))
                .Returns(interceptor);

            this.Container.RegisterInstance<IInterceptorFactory>(mockInterceptorFactory.Object);

            var extendedRepository = this.Container.Resolve<IExtendedRepository>();

            // Act
            SampleEntity newEntity = new SampleEntity() { Id = 1 };
            extendedRepository.Update(newEntity);

            // Assert
            Assert.IsNotNull(interceptor.LastUpdatingEntity);
            Assert.AreEqual(newEntity, interceptor.LastUpdatingEntity);

            Assert.IsNotNull(interceptor.LastUpdatedEntity);
            Assert.AreEqual(newEntity, interceptor.LastUpdatedEntity);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create sample entity repository mock.
        /// </summary>
        /// <returns>
        /// </returns>
        private static Mock<IRepository> CreateSampleEntityRepositoryMock()
        {
            List<SampleEntity> mockResult = new List<SampleEntity>()
                {
                    new SampleEntity() { Id = 1 }, 
                    new SampleEntity() { Id = 2 }, 
                    new SampleEntity() { Id = 3 }
                };

            var mockRepository = new Mock<IRepository>();

            mockRepository
                .Setup(r => r.All<SampleEntity>(It.IsAny<LoadOptions>()))
                .Returns(mockResult.AsQueryable());
            mockRepository
                .Setup(r => r.All(typeof(SampleEntity), It.IsAny<LoadOptions>()))
                .Returns(mockResult.AsQueryable());

            return mockRepository;
        }

        #endregion
    }
}