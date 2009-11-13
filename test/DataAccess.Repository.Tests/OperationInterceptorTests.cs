using System;
using System.Collections.Generic;
using System.Linq;
using LogicSoftware.DataAccess.Repository.Extended;
using LogicSoftware.DataAccess.Repository.Extended.Events;
using LogicSoftware.DataAccess.Repository.Extended.Interceptors;
using LogicSoftware.DataAccess.Repository.Tests.SampleModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LogicSoftware.DataAccess.Repository.Tests
{
    using Basic;

    using SampleModel.Interceptors;

    /// <summary>
    /// Summary description for InterceptorTest
    /// </summary>
    [TestClass]
    public class Operation : UnitTestBase
    {
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
    }
}
